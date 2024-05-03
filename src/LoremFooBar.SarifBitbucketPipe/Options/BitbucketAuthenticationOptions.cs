using System.Diagnostics.CodeAnalysis;
using LoremFooBar.SarifBitbucketPipe.PipeEnvironment;

namespace LoremFooBar.SarifBitbucketPipe.Options;

[Serializable]
public class BitbucketAuthenticationOptions
{
    public string? Username { get; set; }
    public string? AppPassword { get; set; }

    [MemberNotNullWhen(true, nameof(Username), nameof(AppPassword))]
    public bool UseAuthentication => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(AppPassword);

    public static BitbucketAuthenticationOptions FromEnvironment(IEnvironment environment) =>
        new()
        {
            Username = environment.GetString(EnvironmentVariable.BitbucketUsername),
            AppPassword = environment.GetString(EnvironmentVariable.BitbucketAppPassword),
        };
}
