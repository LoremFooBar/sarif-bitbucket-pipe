using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using LoremFooBar.SarifBitbucketPipe.Options;
using LoremFooBar.SarifBitbucketPipe.Utils;
using Serilog;

namespace LoremFooBar.SarifBitbucketPipe.BitbucketApiClient;

public partial class BitbucketClient
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly BitbucketAuthenticationOptions _authOptions;
    private readonly BitbucketEnvironmentInfo _bitbucketEnvironmentInfo;
    private readonly HttpClient _httpClient;
    private readonly PipeOptions _pipeOptions;

    public BitbucketClient(HttpClient client, BitbucketAuthenticationOptions authOptions,
        PipeOptions pipeOptions, BitbucketEnvironmentInfo bitbucketEnvironmentInfo)
    {
        _httpClient = client;
        _bitbucketEnvironmentInfo = bitbucketEnvironmentInfo;
        _pipeOptions = pipeOptions;
        _authOptions = authOptions;

        Log.Debug("Base address: {BaseAddress}", client.BaseAddress);
    }

    private static string Serialize(object obj) => JsonSerializer.Serialize(obj, _jsonSerializerOptions);

    private static StringContent CreateStringContent(string str) =>
        new(str, Encoding.Default, "application/json");

    private static async Task VerifyResponseAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) {
            string error = await response.Content.ReadAsStringAsync();
            Log.Error("Error response: {Error} for request: {RequestUrl}", error,
                response.RequestMessage?.RequestUri);
        }

        response.EnsureSuccessStatusCode();
    }
}
