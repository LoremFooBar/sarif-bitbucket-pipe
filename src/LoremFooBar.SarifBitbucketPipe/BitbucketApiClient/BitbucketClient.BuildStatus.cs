using LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.CommitStatuses;
using LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.Report;

namespace LoremFooBar.SarifBitbucketPipe.BitbucketApiClient;

public partial class BitbucketClient
{
    public async Task CreateBuildStatusAsync(PipelineReport report)
    {
        if (!_pipeOptions.CreateBuildStatus) return;

        if (!_authOptions.UseAuthentication) {
            _logger.Warning("Will not create build status because authentication info was not provided");

            return;
        }

        var buildStatus = BuildStatus.CreateFromPipelineReport(report, _bitbucketEnvironmentInfo.Workspace,
            _bitbucketEnvironmentInfo.RepoSlug);
        string serializedBuildStatus = Serialize(buildStatus);

        _logger.Debug("POSTing build status: {BuildStatus}", serializedBuildStatus);

        var response = await _httpClient.PostAsync($"commit/{_bitbucketEnvironmentInfo.CommitHash}/statuses/build",
            CreateStringContent(serializedBuildStatus));

        await VerifyResponseAsync(response);
    }
}
