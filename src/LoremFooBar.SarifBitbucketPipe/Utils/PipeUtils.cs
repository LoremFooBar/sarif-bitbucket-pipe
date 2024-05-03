namespace LoremFooBar.SarifBitbucketPipe.Utils;

internal static class PipeUtils
{
    public static string GetFoundIssuesString(int numOfIssues) =>
        numOfIssues > 0 ? $"Found {numOfIssues} issue(s)" : "No issues found";
}
