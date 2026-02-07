using System.Diagnostics.CodeAnalysis;
using LoremFooBar.SarifBitbucketPipe.PipeEnvironment;

namespace LoremFooBar.SarifBitbucketPipe.Options;

[Serializable]
public class BitbucketAuthenticationOptions
{
    public string AccountEmail { get; set; } = string.Empty;
    public string ApiToken { get; set; } = string.Empty;

    [MemberNotNull(nameof(AccountEmail), nameof(ApiToken))]
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AccountEmail)) throw new ArgumentException("ACCOUNT_EMAIL is required.");
        if (string.IsNullOrWhiteSpace(ApiToken)) throw new ArgumentException("API_TOKEN is required.");
    }

    public static BitbucketAuthenticationOptions FromEnvironment(IEnvironment environment) =>
        new()
        {
            AccountEmail = environment.GetRequiredString(EnvironmentVariable.AccountEmail),
            ApiToken = environment.GetRequiredString(EnvironmentVariable.ApiToken),
        };
}
