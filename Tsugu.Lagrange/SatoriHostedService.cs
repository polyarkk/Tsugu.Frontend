using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Satori.Client;
using Satori.Protocol.Elements;
using Satori.Protocol.Events;
using Tsugu.Lagrange.Util;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using SatoriLogLevel = Satori.Client.LogLevel;

namespace Tsugu.Lagrange;

internal class SatoriHostedService : IHostedService, IDisposable {
    private readonly SatoriClient _client;

    private readonly Dictionary<string, SatoriBot> _bots;

    private readonly ILogger<SatoriHostedService> _logger;

    public SatoriHostedService(ILogger<SatoriHostedService> logger, IConfiguration configuration) {
        _logger = logger;
        _bots = [];
        
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

            string key = string.Concat(botConfig.Platform, ":", botConfig.SelfId);
            
            _bots.Add(key, bot);
            
            _logger.LogInformation("Registered bot: {key}", key);
        }
    }

    private async void OnMessageCreated(object? sender, Event e) {
        // todo message content broken for sandbox?
        _logger.LogInformation("Received message from {e.Channel!.Id}: {e.Message!.Content}", e.Channel!.Id, e.Message!.Content);
        
        await _bots[GetBotKey(e.Platform, e.SelfId)].CreateMessageAsync(e.Channel.Id,
            new TextElement { Text = "非常好 Satori，爱来自 Satori.Client" }
        );
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
