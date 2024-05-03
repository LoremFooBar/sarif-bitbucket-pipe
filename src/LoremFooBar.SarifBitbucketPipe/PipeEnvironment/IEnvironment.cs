namespace LoremFooBar.SarifBitbucketPipe.PipeEnvironment;

public interface IEnvironment
{
    string? GetString(EnvironmentVariable variable);

    string GetRequiredString(EnvironmentVariable variable) =>
        GetString(variable) ??
        throw new RequiredEnvironmentVariableNotFoundException(variable.Name);

    string GetStringOrDefault(EnvironmentVariable variable, string defaultValue) =>
        GetString(variable) ?? defaultValue;

    bool? GetBool(EnvironmentVariable variable) =>
        GetString(variable)?.Equals("true", StringComparison.OrdinalIgnoreCase);

    bool IsEnvironment(EnvironmentName environmentName) =>
        (GetString(EnvironmentVariable.DotnetEnvironment) ?? EnvironmentName.Production)
        .Equals(environmentName, StringComparison.OrdinalIgnoreCase);
}
