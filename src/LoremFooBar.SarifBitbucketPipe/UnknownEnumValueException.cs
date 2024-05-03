namespace LoremFooBar.SarifBitbucketPipe;

public class UnknownEnumValueException<TEnum>(TEnum value)
    : Exception($"Unknown value {value} for enum {typeof(TEnum)}") where TEnum : Enum;
