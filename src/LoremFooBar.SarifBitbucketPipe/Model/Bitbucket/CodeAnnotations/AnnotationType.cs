using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.CodeAnnotations;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
[PublicAPI]
public enum AnnotationType
{
    [EnumMember(Value = "VULNERABILITY")] Vulnerability,

    [EnumMember(Value = "CODE_SMELL")] CodeSmell,

    [EnumMember(Value = "BUG")] Bug,
}
