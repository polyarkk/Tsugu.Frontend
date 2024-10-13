using Tsugu.Lagrange;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Tsugu.Lagrange.Command;
using Tsugu.Lagrange.Filter;
using Tsugu.Lagrange.Util;

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

builder.Services.AddScoped<FilterService>();
builder.Services.AddHostedService<LagrangeHostedService>();
builder.Services.AddHostedService<SatoriHostedService>();

builder.Configuration.AddJsonFile("appsettings.json");

IHost host = builder.Build();

LoggerUtil.InitLoggerFactory(host.Services.GetService<ILoggerFactory>()!);

await host.RunAsync();
