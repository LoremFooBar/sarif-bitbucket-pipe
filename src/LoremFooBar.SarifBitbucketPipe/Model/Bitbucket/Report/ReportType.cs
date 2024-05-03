using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.Report;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
[PublicAPI]
public enum ReportType
{
    [EnumMember(Value = "SECURITY")] Security,

    [EnumMember(Value = "COVERAGE")] Coverage,

    [EnumMember(Value = "TEST")] Test,

    [EnumMember(Value = "BUG")] Bug,
}
