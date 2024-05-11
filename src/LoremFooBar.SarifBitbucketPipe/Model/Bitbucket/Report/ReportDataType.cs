﻿using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.Report;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
[PublicAPI]
public enum ReportDataType
{
    [EnumMember(Value = "BOOLEAN")] Boolean,

    [EnumMember(Value = "DATE")] Date,

    [EnumMember(Value = "DURATION")] Duration,

    [EnumMember(Value = "LINK")] Link,

    [EnumMember(Value = "NUMBER")] Number,

    [EnumMember(Value = "PERCENTAGE")] Percentage,

    [EnumMember(Value = "TEXT")] Text,
}
