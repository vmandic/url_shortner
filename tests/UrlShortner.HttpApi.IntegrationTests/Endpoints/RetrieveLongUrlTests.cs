
using System;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;

using UrlShortner.HttpApi.IntegrationTests.Helpers;

using Xunit;
using Xunit.Abstractions;

namespace UrlShortner.HttpApi.IntegrationTests.Endpoints;

public class RetrieveLongUrlTests : IClassFixture<UrlShortnerHttpApiWebApplicationFactory>
{
    private readonly UrlShortnerHttpApiWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public RetrieveLongUrlTests(UrlShortnerHttpApiWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Should_get_long_url_redirection()
    {
        // ARRANGE

        // ref: https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler.allowautoredirect?view=net-6.0 disable auto redirect
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var expectedLongUrl = $"http://www.longurl.com?x={Guid.NewGuid():N}";
        var createUrlResponse = await client.PostAsJsonAsync("/", new { url = expectedLongUrl });
        createUrlResponse.EnsureSuccessStatusCode();
        dynamic? createPayload = await createUrlResponse.Content.ReadFromJsonAsync<ExpandoObject>();
        Assert.NotNull(createPayload);
        string shortUrl = createPayload!.short_url.ToString();

        // ACT
        var redirectionResponse = await client.GetAsync(shortUrl);

        // ASSERT
        Assert.Equal(301, (int)redirectionResponse.StatusCode);
        dynamic? redirectPayload = await redirectionResponse.Content.ReadFromJsonAsync<ExpandoObject>();
        Assert.NotNull(redirectPayload);
        Assert.Equal(expectedLongUrl, redirectPayload?.url?.ToString());

        // NOTE: check OriginalString as .NET mumbles up AbsoluteUri (read through on accessing Location) with a final '/' for no actual value here
        Assert.Equal(expectedLongUrl, redirectionResponse.Headers.Location?.OriginalString?.ToString());
    }
}