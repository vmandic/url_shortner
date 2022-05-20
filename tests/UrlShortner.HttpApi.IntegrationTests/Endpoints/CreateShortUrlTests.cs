
using System;
using System.Dynamic;
using System.Net.Http.Json;
using System.Threading.Tasks;

using UrlShortner.HttpApi.IntegrationTests.Helpers;

using Xunit;
using Xunit.Abstractions;

namespace UrlShortner.HttpApi.IntegrationTests.Endpoints;

public class CreateShortUrlTests : IClassFixture<UrlShortnerHttpApiWebApplicationFactory>
{
    private readonly UrlShortnerHttpApiWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public CreateShortUrlTests(UrlShortnerHttpApiWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Should_create_long_url()
    {
        // ARRANGE
        var client = _factory.CreateClient();
        var expectedLongUrl = $"http://www.longurl.com?x={Guid.NewGuid():N}";

        // ACT
        var response = await client.PostAsJsonAsync("/", new { url = expectedLongUrl });

        // ASSERT
        response.EnsureSuccessStatusCode();
        dynamic? payload = await response.Content.ReadFromJsonAsync<ExpandoObject>();

        var strResult = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"Result: {strResult}");

        Assert.Equal(expectedLongUrl, payload?.url?.ToString());
        Assert.NotNull(payload?.short_url);
    }
}