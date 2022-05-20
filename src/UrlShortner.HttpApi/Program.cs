using System.Collections.Concurrent;

using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

ConfigureServices(builder);
var app = builder.Build();

ConfigureHttpPipeline(app);
app.Run();

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.Configure<JsonOptions>(options =>
    {
        // NOTE: lets loosen up deserialization and allow case insensitivity
        // ref: https://www.meziantou.net/configuring-json-options-in-asp-net-core.htm
        options.SerializerOptions.PropertyNameCaseInsensitive = true;
        options.SerializerOptions.PropertyNamingPolicy = null;
        options.SerializerOptions.WriteIndented = false;
    });

    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen();
}

static void ConfigureHttpPipeline(WebApplication app)
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
}

public partial class Program
{
    /// <summary>
    /// Simulates a datastore ie. Database in a thread-safe manner.
    /// </summary>
    public static readonly IDictionary<string, string> ShortLinksMap = new ConcurrentDictionary<string, string>();
}
