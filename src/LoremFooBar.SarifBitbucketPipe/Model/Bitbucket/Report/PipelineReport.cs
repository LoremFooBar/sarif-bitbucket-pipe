using System.Text.Json.Serialization;
using JetBrains.Annotations;
using LoremFooBar.SarifBitbucketPipe.Utils;

namespace LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.Report;

[Serializable]
[PublicAPI]
public class PipelineReport
{
    //public string Type => "report";
    public string? Uuid { get; set; }
    public string? Title { get; set; }
    public string? Details { get; set; }
    public string? ExternalId { get; set; }
    public string? Reporter { get; set; }
    public Uri? Link { get; set; }
    public ReportType ReportType { get; set; }
    public Result Result { get; set; }
    public ICollection<ReportDataItem> Data { get; set; } = new List<ReportDataItem>();
    public DateTime? CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }

    [JsonIgnore]
    public int TotalIssues { get; set; }

    public static PipelineReport CreateFromSarifResults(IReadOnlyList<ResultWithRun> results)
    {
        int totalIssues = results.Count;

        return new PipelineReport
        {
            Title = "Static Analysis",
            Details = PipeUtils.GetFoundIssuesString(totalIssues),
            TotalIssues = totalIssues,
            ExternalId = "sarif-report",
            Reporter = "Sarif Pipe",
            ReportType = ReportType.Bug,
            Result = totalIssues == 0 ? Result.Passed : Result.Failed,
        };
    }
}
