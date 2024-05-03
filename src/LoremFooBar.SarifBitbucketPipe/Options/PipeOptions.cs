using LoremFooBar.SarifBitbucketPipe.PipeEnvironment;

namespace LoremFooBar.SarifBitbucketPipe.Options;

[Serializable]
public class PipeOptions
{
    /// <summary>
    /// Whether to create a build status representing whether any issues were found.
    /// </summary>
    public bool CreateBuildStatus { get; set; }

    /// <summary>
    /// Path to SARIF file. Can use patterns that are supported by <see cref="DirectoryInfo.GetFiles()" />
    /// </summary>
    public string SarifPathOrPattern { get; set; } = "";

    /// <summary>
    /// Whether to fail only for issues found in diff. For PRs - the PR diff, otherwise - diff with previous commit.
    /// </summary>
    public bool IncludeOnlyIssuesInDiff { get; set; }

    /// <summary>
    /// Whether to fail current build step if any issues found.
    /// </summary>
    public bool FailWhenIssuesFound { get; set; }

    public static PipeOptions FromEnvironment(IEnvironment environment) =>
        new()
        {
            CreateBuildStatus = environment.GetBool(EnvironmentVariable.CreateBuildStatus) ?? true,
            FailWhenIssuesFound = environment.GetBool(EnvironmentVariable.FailWhenIssuesFound) ?? false,
            SarifPathOrPattern = environment.GetRequiredString(EnvironmentVariable.SarifFilePath),
            IncludeOnlyIssuesInDiff = environment.GetBool(EnvironmentVariable.IncludeOnlyIssuesInDiff) ?? false,
        };
}
