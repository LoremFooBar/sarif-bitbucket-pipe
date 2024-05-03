using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using DiffPatch;
using LoremFooBar.SarifBitbucketPipe.Model.Diff;

namespace LoremFooBar.SarifBitbucketPipe.BitbucketApiClient;

public partial class BitbucketClient
{
    public async Task<IReadOnlyDictionary<string, AddedLinesInFile>> GetCodeChangesAsync()
    {
        // ReSharper disable once StringLiteralTypo
        string requestUri = _bitbucketEnvironmentInfo.IsPullRequest
            ? $"pullrequests/{_bitbucketEnvironmentInfo.PullRequestId}/diff"
            : $"diff/{_bitbucketEnvironmentInfo.CommitHash}";

        var request = new HttpRequestMessage(HttpMethod.Get, requestUri)
        {
            Headers = { Accept = { new MediaTypeWithQualityHeaderValue("text/plain") } },
        };

        _logger.Debug("GET diff {RequestUri}", requestUri);
        var response = await _httpClient.SendAsync(request);
        string diffStr;

        // this happens due to credentials not being forward on redirect
        // https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler.allowautoredirect?view=net-6.0#remarks
        if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized) {
            Debug.Assert(response.RequestMessage != null, "response.RequestMessage != null");
            var response2 = await _httpClient.GetAsync(response.RequestMessage.RequestUri);
            await VerifyResponseAsync(response2);
            diffStr = await response2.Content.ReadAsStringAsync();
        }
        else {
            await VerifyResponseAsync(response);
            diffStr = await response.Content.ReadAsStringAsync();
        }

        var fileDiffs = DiffParserHelper.Parse(diffStr, Environment.NewLine);

        var diffDictionary = fileDiffs
            .Where(fd => !fd.Deleted)
            .Select(fd => new
            {
                fd.To,
                LineNumbers = fd.Chunks.SelectMany(chunk =>
                    chunk.Changes.Where(change => change.Add).Select(change => change.Index)).ToList(),
            })
            .GroupBy(x => x.To)
            .ToDictionary(x => x.Key, x =>
                new AddedLinesInFile(x.Key, x.SelectMany(y => y.LineNumbers).ToList()));

        return diffDictionary;
    }
}
