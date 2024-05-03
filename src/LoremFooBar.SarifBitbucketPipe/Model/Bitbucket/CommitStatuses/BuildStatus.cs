using System.Text.Json.Serialization;
using JetBrains.Annotations;
using LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.Report;
using LoremFooBar.SarifBitbucketPipe.Utils;

namespace LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.CommitStatuses;

[Serializable]
[PublicAPI]
public class BuildStatus
{
    [JsonConstructor]
    public BuildStatus(string key, string name, State state, string description, Uri url)
    {
        Key = key;
        Name = name;
        State = state;
        Description = description;
        Url = url;
    }

    public BuildStatus(string key, string name, State state, string description, string? workspace = null,
        string? repoSlug = null) : this(key, name, state, description,
        new Uri($"https://bitbucket.org/{workspace}/{repoSlug}")) { }

    public string Key { get; set; }
    public string Name { get; set; }
    public State State { get; set; }
    public string Description { get; set; }
    public Uri Url { get; set; }

    // ReSharper disable once StringLiteralTypo
    [JsonPropertyName("refname")]
    public string? RefName { get; set; }

    public static BuildStatus CreateFromPipelineReport(PipelineReport report, string workspace, string repoSlug)
    {
        const string key = "static-analysis";
        const string name = "Static Analysis";
        var state = report.Result switch //todo make it an error to miss branches in switch
        {
            Result.Failed => State.Failed,
            Result.Passed => State.Successful,
            Result.Pending => throw new Exception(
                "Can't create build status from a pipeline report with status 'Pending'"),
            _ => throw new UnknownEnumValueException<Result>(report.Result),
        };

        string description = PipeUtils.GetFoundIssuesString(report.TotalIssues);

        return new BuildStatus(key, name, state, description, workspace, repoSlug);
    }
}
