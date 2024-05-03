#pragma warning disable ASP0018 // unused route parameters

// ReSharper disable RouteTemplates.RouteParameterIsNotPassedToMethod

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using FakeBitbucketServer;
using JetBrains.Annotations;
using LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.CodeAnnotations;
using LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.CommitStatuses;
using LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.Report;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
});

var app = builder.Build();

var store = app.Services.GetService<BitbucketServerStore>() ?? new BitbucketServerStore();
string diff = app.Services.GetRequiredService<Diff>().Value;

var group = app.MapGroup("2.0/repositories/{workspace}/{repoSlug}");

group.MapPost("commit/{commitHash}/statuses/build",
    ([FromBody] BuildStatus buildStatus) => store.BuildStatus = buildStatus);

group.MapPut("commit/{commitHash}/reports/{externalId}",
    ([FromBody] PipelineReport report) => store.Report = report);

group.MapPost("commit/{commitHash}/reports/{externalId}/annotations",
    ([FromBody] List<Annotation> annotations) => store.Annotations.AddRange(annotations));

group.MapGet("pullRequests/{id}/diff", () => diff);

group.MapGet("diff/{commitHash}", () => diff);

// app.MapGet("store", () => JsonSerializer.Serialize(store));
app.MapGet("debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
    string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));
app.Run();

namespace FakeBitbucketServer
{
    public class BitbucketServerStore
    {
        public BuildStatus? BuildStatus { get; set; }
        public PipelineReport? Report { get; set; }
        public List<Annotation> Annotations { get; } = [];
    }

    public class Diff
    {
        public required string Value { get; set; }
    }
}

[UsedImplicitly]
public partial class Program;
