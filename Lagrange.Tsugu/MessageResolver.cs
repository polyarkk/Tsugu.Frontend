using Lagrange.Core;
using Lagrange.Core.Event;
using Lagrange.Core.Event.EventArg;
using Lagrange.Core.Message;
using Lagrange.Tsugu.Api;
using Lagrange.Tsugu.Api.Endpoint;
using Lagrange.Tsugu.Command;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;
using BindingFlags = System.Reflection.BindingFlags;

namespace Lagrange.Tsugu;

public class MessageResolver {
    private readonly Dictionary<string, Type> _apis;

    private readonly IHttpClientFactory _httpClientFactory;

    private readonly ILogger<MessageResolver> _logger;

    private readonly ILoggerFactory _loggerFactory;

    private readonly IConfiguration _configuration;

    private readonly AppSettings _appSettings;

    public MessageResolver(
        ILogger<MessageResolver> logger,
        IHttpClientFactory httpClientFactory,
        ILoggerFactory loggerFactory,
        IConfiguration configuration
    ) {
        _logger = logger;
        _apis = new Dictionary<string, Type>();
        _httpClientFactory = httpClientFactory;
        _loggerFactory = loggerFactory;
        _configuration = configuration;
        _appSettings = configuration.GetSection("Tsugu").Get<AppSettings>()!;

        LoadEndpoints();
    }

    private void LoadEndpoints() {
        _apis.Clear();
        
        var typesWithApiCommand =
            from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            let attributes = t.GetCustomAttributes(typeof(ApiCommand), true)
            where attributes is { Length: > 0 }
            select new { Attribute = attributes.Cast<ApiCommand>().First()!, Type = t };

        foreach (var t in typesWithApiCommand) {
            if (t.Type.BaseType != typeof(BaseCommand)) {
                _logger.LogWarning("command [{type}] won't register because it does not inherit BaseCommand class",
                    t.Type.Name
                );

                continue;
            }

            _apis[t.Attribute.Alias] = t.Type;
        }
    }

    private string GetHelpPlainText() {
        StringBuilder stringBuilder = new();

        stringBuilder.AppendLine("可用指令：");

        foreach (ApiCommand attr in _apis.Values.Select(at => at.GetCustomAttribute<ApiCommand>()!)) {
            stringBuilder.AppendLine($"{attr.Alias} {attr.UsageHint}\n{attr.Description}\n");
        }

        stringBuilder.Remove(stringBuilder.Length - 2, 2);

        return stringBuilder.ToString();
    }

    public async Task InvokeCommand(
        BotContext botContext,
        EventBase @event,
        MessageChain messageChain,
        string prompt
    ) {
        if ((@event is FriendMessageEvent && !_appSettings.IsFriendWhitelisted(messageChain.FriendUin))
            || (@event is GroupMessageEvent && !_appSettings.IsGroupWhitelisted(messageChain.GroupUin))
        ) {
            _logger.LogInformation("message from [{uin}] won't handle due to app settings", messageChain.FriendUin);

            return;
        }

        string[] tokens = prompt.Split(" ");

        if (tokens.Length == 0) {
            return;
        }

        Context context = new(_appSettings, botContext, @event, messageChain, _httpClientFactory, _loggerFactory);
        
        if (string.Equals(tokens[0], "tsugu_reload_appsettings", StringComparison.OrdinalIgnoreCase)) {
            // todo
            
            await context.SendPlainText("已重新加载配置信息");

            return;
        }

#if DEBUG
        if (string.Equals(tokens[0], "tsugu_reload_commands", StringComparison.OrdinalIgnoreCase)) {
            LoadEndpoints();
            
            await context.SendPlainText("已重新加载指令集，" + GetHelpPlainText());

            return;
        }
#endif

        if (string.Equals(tokens[0], "tsugu_help", StringComparison.OrdinalIgnoreCase)) {
            await context.SendPlainText(GetHelpPlainText());

            return;
        }

        if (!_apis.TryGetValue(tokens[0], out Type? apiType)) {
            return;
        }

        ConstructorInfo? ctor =
            apiType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, []);

        if (ctor == null) {
            _logger.LogWarning(
                "constructor of command [{cmd}] not found! make sure the no-params constructor is defined and public.",
                apiType.Name
            );

            return;
        }

        BaseCommand api = (BaseCommand)ctor.Invoke(null);

        try {
            await api.Invoke(context, new ParsedCommand(tokens));
        } catch (CommandParseException e) {
            await context.SendPlainText(api.GetErrorAndHelpText(e.Message));
        } catch (Exception e) {
            _logger.LogError("exception raised upon resolving command!\n{e}", e.ToString());
        }
    }
}
