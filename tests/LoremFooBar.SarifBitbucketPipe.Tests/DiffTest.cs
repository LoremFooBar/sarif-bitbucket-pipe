using LoremFooBar.SarifBitbucketPipe.PipeEnvironment;

namespace LoremFooBar.SarifBitbucketPipe.Tests;

public class DiffTest
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
            [EnvironmentVariable.BitbucketPrId] = "51",

            [EnvironmentVariable.SarifFilePath] = "test-data/diff/with-results.sarif",
            [EnvironmentVariable.IncludeOnlyIssuesInDiff] = "true",
        };

        // act
        await new PipeRunner(environment, bitbucketApi.CreateDefaultClient()).RunPipe();

        // assert
        await Verify(bitbucketApi.Store);
    }
}
