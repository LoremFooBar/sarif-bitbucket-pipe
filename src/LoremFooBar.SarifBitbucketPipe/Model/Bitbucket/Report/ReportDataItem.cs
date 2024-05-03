using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.Report;

[Serializable]
[ExcludeFromCodeCoverage(Justification = "not used by this pipe")]
[PublicAPI]
public class ReportDataItem
{
    public ReportDataType Type { get; set; }
    public string Title { get; set; } = null!;
    public object Value { get; set; } = null!;
}
