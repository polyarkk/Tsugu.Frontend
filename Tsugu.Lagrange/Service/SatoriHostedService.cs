using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Satori.Client;
using Satori.Protocol.Events;
using Tsugu.Lagrange.Context;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Tsugu.Lagrange.Service;

internal class SatoriHostedService : IHostedService, IDisposable {
    private readonly SatoriClient _client;

    private readonly Dictionary<string, SatoriBot> _bots;

    private readonly ILogger<SatoriHostedService> _logger;

    private readonly FilterService _filterService;

    public SatoriHostedService(
        ILogger<SatoriHostedService> logger, IConfiguration configuration, FilterService filterService
    ) {
        _logger = logger;
        
        _logger.LogInformation("--- TSUGU SATORI FRONTEND IS NOW STARTING!!! ---");
        
        _bots = [];
        _filterService = filterService;

        AppSettings appSettings = configuration.GetSection("Tsugu").Get<AppSettings>()!;

        if (!appSettings.Satori.Enabled) {
            _client = null!;

            return;
        }

        _client = new SatoriClient(new Uri(appSettings.Satori.Server), appSettings.Satori.Token);

        _client.Logging += (_, log) => {
            _logger.Log((LogLevel)log.LogLevel, "[{log.LogTime}] {log.Message}", log.LogTime, log.Message);
        };

        foreach (AppSettings.SatoriConfig.BotConfig botConfig in appSettings.Satori.Bots) {
            SatoriBot bot = _client.Bot(botConfig.Platform, botConfig.SelfId);

            bot.MessageCreated += OnMessageCreated;

            string key = GetBotKey(botConfig.Platform, botConfig.SelfId);

            _bots.Add(key, bot);

            _logger.LogInformation("Registered bot: {key}", key);
        }
    }

    private async void OnMessageCreated(object? sender, Event e) {
        // koishi sandbox will not work here
        await _filterService.InvokeFilters(new SatoriMessageContext(_bots[GetBotKey(e.Platform, e.SelfId)], e));
    }

    private static string GetBotKey(string platform, string selfId) {
        return string.Concat(platform, ":", selfId);
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public void Dispose() {
        _client.Dispose();
    }
}
