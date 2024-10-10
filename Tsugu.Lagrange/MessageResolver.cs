using Lagrange.Core;
using Lagrange.Core.Event;
using Lagrange.Core.Event.EventArg;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;
using Tsugu.Lagrange.Command;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;
using Tsugu.Api.Misc;
using Tsugu.Lagrange.Util;
using BindingFlags = System.Reflection.BindingFlags;

namespace Tsugu.Lagrange;

public class MessageResolver {
    private readonly Dictionary<string, BaseCommand> _commands;

    private readonly ILogger<MessageResolver> _logger;

    private readonly AppSettings _appSettings;

    public MessageResolver(
        ILogger<MessageResolver> logger,
        IConfiguration configuration
    ) {
        _logger = logger;
        _commands = new Dictionary<string, BaseCommand>();
        _appSettings = configuration.GetSection("Tsugu").Get<AppSettings>()!;

        LoadEndpoints();
    }

    private void LoadEndpoints() {
        _commands.Clear();
        GC.Collect();

        var typesWithApiCommand =
            from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            let attributes = t.GetCustomAttributes(typeof(ApiCommandAttribute), true)
            where attributes is { Length: > 0 }
            select new { Attribute = attributes.Cast<ApiCommandAttribute>().First()!, Type = t };

        foreach (var t in typesWithApiCommand) {
            if (t.Type.BaseType != typeof(BaseCommand)) {
                _logger.LogWarning("command [{type}] won't register because it does not inherit BaseCommand class",
                    t.Type.Name
                );

                continue;
            }
            
            ConstructorInfo? ctor = t.Type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, []
            );

            if (ctor == null) {
                _logger.LogWarning(
                    "command constructor {a}::{a}() not found!",
                    t.Type.Name, t.Type.Name
                );

                return;
            }

            BaseCommand command = (BaseCommand)ctor.Invoke(null);

            foreach (string alias in t.Attribute.Aliases) {
                _commands[alias] = command;
            }
        }
    }

    private string GetHelpPlainText() {
        StringBuilder stringBuilder = new();

        HashSet<BaseCommand> hashSet = [];

        stringBuilder.AppendLine("可用指令：");

        foreach (ApiCommandAttribute attr in
            from command in _commands.Values
            where hashSet.Add(command)
            select command.GetType().GetCustomAttribute<ApiCommandAttribute>()!
        ) {
            stringBuilder.AppendLine($" - {string.Join("|", attr.Aliases)}: {attr.Description}");
        }

        stringBuilder.AppendLine("* 指令尾随 --help 将输出指令的详细帮助");

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

        if (@event is GroupMessageEvent && _appSettings.NeedMentioned) {
            foreach (IMessageEntity entity in messageChain) {
                if (entity is not MentionEntity me || me.Uin != botContext.BotUin) {
                    continue;
                }

                _logger.LogInformation("message from [{uin}] won't handle because it did not mention bot",
                    messageChain.FriendUin
                );

                return;
            }
        }

        string[] tokens = prompt.Split(" ");

        if (tokens.Length == 0) {
            return;
        }

        using Context context = new(_appSettings, botContext, @event, messageChain);

        if (string.Equals(tokens[0], "tsugu_reload_appsettings", StringComparison.OrdinalIgnoreCase)) {
            // todo
            // await context.SendPlainText("已重新加载配置信息");

            return;
        }

#if DEBUG
        if (string.Equals(tokens[0], "tsugu_reload_commands", StringComparison.OrdinalIgnoreCase)) {
            LoadEndpoints();

            await context.SendPlainText("已重新加载指令集\n" + GetHelpPlainText());

            return;
        }
#endif

        if (string.Equals(tokens[0], "tsugu_help", StringComparison.OrdinalIgnoreCase)) {
            await context.SendPlainText(
                $"当前主服务器: {context.TsuguUser.MainServer.ToString().ToLower()}\n{GetHelpPlainText()}"
            );

            return;
        }

        if (!_commands.TryGetValue(tokens[0], out BaseCommand? api)) {
            return;
        }

        if (tokens.Contains("--help")) {
            await context.SendPlainText(api.GetHelpText());

            return;
        }

        try {
            await api.InvokePre(context, tokens);
        } catch (CommandParseException e) {
            await context.SendPlainText(api.GetErrorAndHelpText(e.Message));
        } catch (EndpointCallException e) {
            await context.SendPlainText(e.Message);
        } catch (Exception e) {
            _logger.LogError("exception raised upon resolving command!\n{e}", e.ToString());
        }
    }
}
