using FakeBitbucketServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace LoremFooBar.SarifBitbucketPipe.Tests;

public class FakeBitbucketApiApplicationFactory : WebApplicationFactory<Program>
{
    public readonly BitbucketServerStore Store = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var storeService = services.FirstOrDefault(s => s.ServiceType == typeof(BitbucketServerStore));
            var diffService = services.FirstOrDefault(s => s.ServiceType == typeof(Diff));

            if (storeService is not null) services.Remove(storeService);
            if (diffService is not null) services.Remove(diffService);

            services
                .AddSingleton(Store)
                .AddSingleton(new Diff { Value = File.ReadAllText("test-data/diff/diff.txt") });
        });
    }
}
