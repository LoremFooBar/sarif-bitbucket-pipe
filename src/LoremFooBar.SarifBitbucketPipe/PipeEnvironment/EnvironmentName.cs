namespace LoremFooBar.SarifBitbucketPipe.PipeEnvironment;

public class EnvironmentName
{
    private EnvironmentName(string value) => Value = value;

    private string Value { get; }

    public static EnvironmentName Development { get; } = new("Development");
    public static EnvironmentName Production { get; } = new("Production");

    public static implicit operator string(EnvironmentName environmentName) => environmentName.Value;
}
