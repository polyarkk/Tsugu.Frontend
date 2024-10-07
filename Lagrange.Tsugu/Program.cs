using Lagrange.Tsugu;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddHostedService<TsuguHostedService>();
builder.Services.AddScoped<MessageResolver>();

IHost host = builder.Build();

await host.RunAsync();
