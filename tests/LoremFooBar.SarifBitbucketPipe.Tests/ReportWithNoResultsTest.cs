using LoremFooBar.SarifBitbucketPipe.PipeEnvironment;

namespace LoremFooBar.SarifBitbucketPipe.Tests;

public class ReportWithNoResultsTest
{
    [Fact]
    public async Task Test()
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

            [EnvironmentVariable.SarifFilePath] = "test-data/without-results.sarif",
        };

        // act
        await new PipeRunner(environment, bitbucketApi.CreateDefaultClient()).RunPipe();

        // assert
        await Verify(bitbucketApi.Store);
    }
}
