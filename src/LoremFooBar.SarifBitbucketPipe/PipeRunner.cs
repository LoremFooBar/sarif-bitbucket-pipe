using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using LoremFooBar.SarifBitbucketPipe.BitbucketApiClient;
using LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.Report;
using LoremFooBar.SarifBitbucketPipe.Model.Diff;
using LoremFooBar.SarifBitbucketPipe.Options;
using LoremFooBar.SarifBitbucketPipe.PipeEnvironment;
using LoremFooBar.SarifBitbucketPipe.Utils;
using Microsoft.CodeAnalysis.Sarif;
using Serilog;
using Serilog.Events;
using Result = Microsoft.CodeAnalysis.Sarif.Result;

namespace LoremFooBar.SarifBitbucketPipe;

public class PipeRunner
{
    private readonly BitbucketAuthenticationOptions _authOptions;
    private readonly BitbucketClient _bitbucketClient;
    private readonly BitbucketEnvironmentInfo _bitbucketEnvironmentInfo;
    private readonly HttpClient? _bitbucketHttpClient;
    private readonly IEnvironment _environment;
    private readonly PipeOptions _pipeOptions;

    public PipeRunner(IEnvironment? environment = null, HttpClient? bitbucketHttpClient = null)
    {
        _bitbucketHttpClient = bitbucketHttpClient;
        _environment = environment ?? new DefaultEnvironment();
        _bitbucketEnvironmentInfo = BitbucketEnvironmentInfo.FromEnvironment(_environment);
        _authOptions = BitbucketAuthenticationOptions.FromEnvironment(_environment);
        _pipeOptions = PipeOptions.FromEnvironment(_environment);
        _bitbucketClient = CreateBitbucketClient(_bitbucketEnvironmentInfo, _pipeOptions);
    }

    public async Task RunPipe()
    {
        bool isDebug = _environment.GetBool(EnvironmentVariable.Debug) == true;
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(isDebug ? LogEventLevel.Debug : LogEventLevel.Warning)
            .CreateLogger();

        var file = GetSarifFile(_pipeOptions);

        SarifLog sarif;

        await using (var fileStream = file.OpenRead()) {
            sarif = SarifLog.Load(fileStream);
        }

        // todo filter issues by diff
        var results = await GetFilteredResultsByDiff(sarif);
        var pipelineReport = PipelineReport.CreateFromSarifResults(results);
        var annotations = new AnnotationsCreator(_bitbucketEnvironmentInfo)
            .CreateAnnotationsFromSarifResults(results).ToList();

        await _bitbucketClient.CreateReportAsync(pipelineReport, annotations);
        await _bitbucketClient.CreateBuildStatusAsync(pipelineReport);

        if (_pipeOptions.FailWhenIssuesFound && pipelineReport is { TotalIssues: > 0 })
            throw new IssuesFoundException(pipelineReport.TotalIssues);
    }

    private BitbucketClient CreateBitbucketClient(BitbucketEnvironmentInfo bitbucketEnvironmentInfo,
        PipeOptions pipeOptions)
    {
        var httpClient = ConfigureClient(_bitbucketHttpClient ?? CreateHttpClient());

        var bitbucketClient =
            new BitbucketClient(httpClient, _authOptions, pipeOptions, bitbucketEnvironmentInfo);

        return bitbucketClient;

        [ExcludeFromCodeCoverage(Justification = "http client for tests is injected")]
        HttpClient CreateHttpClient()
        {
            HttpClient client;

            if (_authOptions.UseAuthentication) {
                Log.Debug("Authenticating using app password");
                client = new HttpClient();

                if (_authOptions.UseAuthentication) {
                    client.DefaultRequestHeaders.Authorization =
                        new BasicAuthenticationHeaderValue(_authOptions.Username, _authOptions.AppPassword);
                }
            }
            else {
                // set proxy for pipe when running in pipelines
                const string proxyUrl = "http://host.docker.internal:29418";
                Log.Debug("Using proxy {Proxy}", proxyUrl);
                Log.Information("Not using authentication - can't create build status");
                var httpClientHandler = new HttpClientHandler
                    { Proxy = new WebProxy(proxyUrl) };
                client = new HttpClient(httpClientHandler);
            }

            return client;
        }

        HttpClient ConfigureClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress =
                new Uri($"{(_authOptions.UseAuthentication ? "https" : "http")}://api.bitbucket.org" +
                        "/2.0/repositories/" +
                        $"{bitbucketEnvironmentInfo.Workspace}/{bitbucketEnvironmentInfo.RepoSlug}/");

            return client;
        }
    }

    private static FileInfo GetSarifFile(PipeOptions pipeOptions)
    {
        FileInfo file;

        if (Path.IsPathRooted(pipeOptions.SarifPathOrPattern))
            file = new FileInfo(pipeOptions.SarifPathOrPattern);
        else {
            var currentDir = new DirectoryInfo(Environment.CurrentDirectory);
            file = currentDir.GetFiles(pipeOptions.SarifPathOrPattern).FirstOrDefault() ??
                   throw new Exception($"No files found for {pipeOptions.SarifPathOrPattern} " +
                                       $"relative to current dir {currentDir.FullName}");
        }

        if (file is not { Exists: true })
            throw new FileNotFoundException(file.FullName);

        return file;
    }

    private async Task<IReadOnlyList<ResultWithRun>> GetFilteredResultsByDiff(SarifLog sarif)
    {
        var results = sarif.FlatResults();

        if (!_pipeOptions.IncludeOnlyIssuesInDiff || results.Count == 0) return results;

        Log.Debug("filtering issues by changes in PR/commit. Total issues: {TotalIssues}", results.Count);

        var codeChanges = await _bitbucketClient.GetCodeChangesAsync();
        var filteredIssues = results.Where(result => IsResultInChanges(result.Result, codeChanges)).ToList();

        Log.Debug("Total issues after filter: {TotalFilteredIssues}", filteredIssues.Count);

        return filteredIssues;

        static bool IsResultInChanges(Result result, IReadOnlyDictionary<string, AddedLinesInFile> codeChanges)
        {
            var physicalLocation = result.Locations.FirstOrDefault()?.PhysicalLocation;

            if (physicalLocation is not { ArtifactLocation: not null, Region: not null }) return false;

            string file = physicalLocation.ArtifactLocation.Uri.OriginalString;
            int line = physicalLocation.Region.StartLine;

            return codeChanges.ContainsKey(file) &&
                   codeChanges[file].LinesAdded.Any(addedLineNumber => line == addedLineNumber);
        }
    }
}

public class IssuesFoundException(int numberOfIssuesFound) : Exception($"Found {numberOfIssuesFound} issue(s)");
