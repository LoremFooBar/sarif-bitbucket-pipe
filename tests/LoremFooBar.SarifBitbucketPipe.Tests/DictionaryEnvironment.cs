using LoremFooBar.SarifBitbucketPipe.PipeEnvironment;

namespace LoremFooBar.SarifBitbucketPipe.Tests;

public class DictionaryEnvironment : Dictionary<EnvironmentVariable, string>, IEnvironment
{
    public string GetString(EnvironmentVariable variable)
    {
        TryGetValue(variable, out string value);

        return value;
    }
}
