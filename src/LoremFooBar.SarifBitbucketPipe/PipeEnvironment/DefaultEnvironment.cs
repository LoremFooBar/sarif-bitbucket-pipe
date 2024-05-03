using System.Diagnostics.CodeAnalysis;

namespace LoremFooBar.SarifBitbucketPipe.PipeEnvironment;

[ExcludeFromCodeCoverage(Justification = "can't test System.Environment usages")]
public class DefaultEnvironment : IEnvironment
{
    public string? GetString(EnvironmentVariable variable) => Environment.GetEnvironmentVariable(variable.Name);
}
