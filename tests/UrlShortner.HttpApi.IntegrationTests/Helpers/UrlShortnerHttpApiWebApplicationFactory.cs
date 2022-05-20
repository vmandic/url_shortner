using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace UrlShortner.HttpApi.IntegrationTests.Helpers
{
    public class UrlShortnerHttpApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // NOTE: shared extra set up goes here
            return base.CreateHost(builder);
        }
    }
}
