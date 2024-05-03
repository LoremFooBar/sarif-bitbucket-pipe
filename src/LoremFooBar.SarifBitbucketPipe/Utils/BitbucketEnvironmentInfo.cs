using System.Diagnostics.CodeAnalysis;
using LoremFooBar.SarifBitbucketPipe.PipeEnvironment;

namespace LoremFooBar.SarifBitbucketPipe.Utils;

public class BitbucketEnvironmentInfo
{
    public required string Workspace { get; init; }

    public required string RepoSlug { get; init; }

    public required string CommitHash { get; init; }

    public required string CloneDir { get; init; }

    public required string? PullRequestId { get; init; }

    [MemberNotNullWhen(true, nameof(PullRequestId))]
    public bool IsPullRequest => !string.IsNullOrEmpty(PullRequestId);

    public static BitbucketEnvironmentInfo FromEnvironment(IEnvironment environment) =>
        new()
        {
            CommitHash = environment.GetRequiredString(EnvironmentVariable.BitbucketCommit),
            Workspace = environment.GetRequiredString(EnvironmentVariable.BitbucketWorkspace),
            RepoSlug = environment.GetRequiredString(EnvironmentVariable.BitbucketRepoSlug),
            CloneDir = environment.GetRequiredString(EnvironmentVariable.BitbucketCloneDir),
            PullRequestId = environment.GetString(EnvironmentVariable.BitbucketPrId),
        };
}
