using LoremFooBar.SarifBitbucketPipe.PipeEnvironment;

namespace LoremFooBar.SarifBitbucketPipe.Tests;

public class ReportWithResultsAndBuildStatusTest
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Test(bool failWhenIssuesFound)
    {
        // arrange
        var bitbucketApi = new FakeBitbucketApiApplicationFactory();
        var environment = new DictionaryEnvironment
        {
            [EnvironmentVariable.Debug] = "true",

            [EnvironmentVariable.BitbucketCommit] = "123456789",
            [EnvironmentVariable.BitbucketWorkspace] = "test-workspace",
            [EnvironmentVariable.BitbucketRepoSlug] = "test-repo",
            [EnvironmentVariable.BitbucketCloneDir] = "/repos",
            [EnvironmentVariable.BitbucketPrId] = null,

            [EnvironmentVariable.BitbucketUsername] = "username",
            [EnvironmentVariable.BitbucketAppPassword] = "1234",

            [EnvironmentVariable.SarifFilePath] = "test-data/with-results.sarif",
            [EnvironmentVariable.FailWhenIssuesFound] = failWhenIssuesFound.ToString().ToLower(),
        };

        // act
        IssuesFoundException issuesFoundException = null;

        try {
            await new PipeRunner(environment, bitbucketApi.CreateDefaultClient()).RunPipe();
        }
        catch (IssuesFoundException ex) {
            issuesFoundException = ex;
        }

        // assert
        if (failWhenIssuesFound) issuesFoundException.Should().NotBeNull();
        else issuesFoundException.Should().BeNull();

        await Verify(bitbucketApi.Store)
            .DisableRequireUniquePrefix();
    }
}
