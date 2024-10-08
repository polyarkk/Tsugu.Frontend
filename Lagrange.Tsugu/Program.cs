using Lagrange.Tsugu;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

if (!File.Exists("appsettings.json")) {
    var conf = new Dictionary<string, dynamic>() {
        ["Tsugu"] = new AppSettings(),
    };

#pragma warning disable CA1869
    JsonSerializerOptions options = new JsonSerializerOptions() {
        WriteIndented = true
    };
#pragma warning restore CA1869
    
    string json = JsonSerializer.Serialize(conf, options);

    File.WriteAllText("appsettings.json", json);

    Console.Error.WriteLine("configure appsettings.json before running the application!");

    return;
}

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddHostedService<TsuguHostedService>();
builder.Services.AddScoped<MessageResolver>();
builder.Configuration.AddJsonFile("appsettings.json");

IHost host = builder.Build();

await host.RunAsync();
