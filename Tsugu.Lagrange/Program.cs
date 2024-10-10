global using ArgumentMeta = (string, string, System.Type, bool);
using Tsugu.Lagrange;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Tsugu.Lagrange.Command;

if (!File.Exists("appsettings.json")) {
    Dictionary<string, dynamic> conf = new() {
        ["Tsugu"] = new AppSettings(),
    };

#pragma warning disable CA1869
    JsonSerializerOptions options = new() {
        WriteIndented = true,
    };
#pragma warning restore CA1869

    string json = JsonSerializer.Serialize(conf, options);

    File.WriteAllText("appsettings.json", json);

    Console.Error.WriteLine("configure appsettings.json before running the application!");

    return;
}

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<TsuguHostedService>();
builder.Services.AddScoped<MessageResolver>();
builder.Services.AddScoped<BindPlayerVerificationTimer>();

builder.Configuration.AddJsonFile("appsettings.json");

IHost host = builder.Build();

await host.RunAsync();
