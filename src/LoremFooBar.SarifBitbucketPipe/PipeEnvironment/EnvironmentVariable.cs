namespace LoremFooBar.SarifBitbucketPipe.PipeEnvironment;

public record struct EnvironmentVariable
{
    public readonly string Name;

    private EnvironmentVariable(string name) => Name = name;

    public static EnvironmentVariable DotnetEnvironment { get; } = new("DOTNET_ENVIRONMENT");

    // pipe options
    public static EnvironmentVariable Debug { get; } = new("DEBUG");
    public static EnvironmentVariable CreateBuildStatus { get; } = new("CREATE_BUILD_STATUS");
    public static EnvironmentVariable FailWhenIssuesFound { get; } = new("FAIL_WHEN_ISSUES_FOUND");
    public static EnvironmentVariable SarifFilePath { get; } = new("SARIF_FILE_PATH");
    public static EnvironmentVariable IncludeOnlyIssuesInDiff { get; } = new("INCLUDE_ONLY_ISSUES_IN_DIFF");

    // auth options
    public static EnvironmentVariable BitbucketUsername { get; } = new("BITBUCKET_USERNAME");
    public static EnvironmentVariable BitbucketAppPassword { get; } = new("BITBUCKET_APP_PASSWORD");

    // bitbucket environment
    public static EnvironmentVariable BitbucketCommit { get; } = new("BITBUCKET_COMMIT");
    public static EnvironmentVariable BitbucketWorkspace { get; } = new("BITBUCKET_WORKSPACE");
    public static EnvironmentVariable BitbucketRepoSlug { get; } = new("BITBUCKET_REPO_SLUG");
    public static EnvironmentVariable BitbucketCloneDir { get; } = new("BITBUCKET_CLONE_DIR");
    public static EnvironmentVariable BitbucketPrId { get; } = new("BITBUCKET_PR_ID");
}
