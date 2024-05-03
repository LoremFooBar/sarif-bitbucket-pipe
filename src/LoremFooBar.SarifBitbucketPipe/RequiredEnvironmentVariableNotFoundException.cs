namespace LoremFooBar.SarifBitbucketPipe;

public class RequiredEnvironmentVariableNotFoundException(string variableName)
    : Exception($"Required environment variable {variableName} not found");
