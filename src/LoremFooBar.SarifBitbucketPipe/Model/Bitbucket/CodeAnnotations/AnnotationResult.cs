using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.CodeAnnotations;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
[PublicAPI]
public enum AnnotationResult
{
    [EnumMember(Value = "PASSED")] Passed,

    [EnumMember(Value = "FAILED")] Failed,

    [EnumMember(Value = "SKIPPED")] Skipped,

    [EnumMember(Value = "IGNORED")] Ignored,
}
