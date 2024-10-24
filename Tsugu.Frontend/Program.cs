using Tsugu.Frontend;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Tsugu.Frontend.Command;
using Tsugu.Frontend.Filter;
using Tsugu.Frontend.Service;
using Tsugu.Frontend.Util;

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
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddScoped<FilterService>();
builder.Services.AddHostedService<LagrangeHostedService>();
builder.Services.AddHostedService<SatoriHostedService>();

builder.Logging.ClearProviders()
    .AddSimpleConsole(options => {
        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
    });

IHost host = builder.Build();

LoggerUtil.InitLoggerFactory(host.Services.GetService<ILoggerFactory>()!);

await host.RunAsync();
