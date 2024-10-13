using Lagrange.Core;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;
using Tsugu.Api.Enum;
using Tsugu.Api.Misc;
using Tsugu.Lagrange.Command;
using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Context;
using Tsugu.Lagrange.Enum;
using Tsugu.Lagrange.Util;
using BindingFlags = System.Reflection.BindingFlags;

namespace Tsugu.Lagrange.Filter;

public class CommandFilter : IFilter {
    private readonly Dictionary<string, BaseCommand> _commands;

    private readonly static ILogger<CommandFilter> Logger = LoggerUtil.GetLogger<CommandFilter>();

    private AppSettings _appSettings;

    private readonly IConfiguration _configuration;

    public CommandFilter(
        IConfiguration configuration
    ) {
        _commands = new Dictionary<string, BaseCommand>();
        _appSettings = configuration.GetSection("Tsugu").Get<AppSettings>()!;
        _configuration = configuration;

        LoadEndpoints();
    }

    public async Task DoFilterAsync(IMessageContext messageContext) {
        if ((messageContext.MessageSource == MessageSource.Friend &&
                !_appSettings.IsFriendWhitelisted(messageContext.UserIdentifier)
            ) || (messageContext.MessageSource == MessageSource.Group &&
                !_appSettings.IsGroupWhitelisted(messageContext.Protocol, messageContext.Platform,
                    messageContext.GroupId
                )
            )
        ) {
            Logger.LogInformation("message from [{uid}] won't handle because friend or group is not whitelisted",
                messageContext.UserIdentifier
            );

            return;
        }

        if (messageContext.MessageSource == MessageSource.Group
            && _appSettings.NeedMentioned && !messageContext.MentionedMe
        ) {
            Logger.LogInformation("message from [{uid}] won't handle because it did not mention bot",
                messageContext.UserIdentifier
            );

            return;
        }

        using TsuguContext tsuguContext = new(_appSettings, messageContext);

        if (tsuguContext.MessageContext.TextOnlyTokens.Count == 0) {
            return;
        }

        bool isAdmin = _appSettings.Admins.Contains(messageContext.UserIdentifier);

        if (isAdmin && string.Equals(tsuguContext.MessageContext.TextOnlyTokens[0], "tsugu_reload_appsettings",
            StringComparison.OrdinalIgnoreCase
        )) {
            _appSettings = _configuration.GetSection("Tsugu").Get<AppSettings>()!;

            await tsuguContext.ReplyPlainText("已重新加载配置信息");

            return;
        }

#if DEBUG
        if (isAdmin && string.Equals(tsuguContext.MessageContext.TextOnlyTokens[0], "tsugu_reload_commands",
            StringComparison.OrdinalIgnoreCase
        )) {
            LoadEndpoints();

            await tsuguContext.ReplyPlainText("已重新加载指令集\n" + GetHelpPlainText());

            return;
        }
#endif

        if (string.Equals(tsuguContext.MessageContext.TextOnlyTokens[0], "tsugu_help",
            StringComparison.OrdinalIgnoreCase
        )) {
            await tsuguContext.ReplyPlainText(
                $"当前主服务器: {tsuguContext.TsuguUser.MainServer.ToChineseString()}\n{GetHelpPlainText()}"
            );

            return;
        }

        if (!_commands.TryGetValue(tsuguContext.MessageContext.TextOnlyTokens[0], out BaseCommand? api)) {
            return;
        }

        if (tsuguContext.MessageContext.TextOnlyTokens.Contains("--help")) {
            await tsuguContext.ReplyPlainText(api.GetHelpText());

            return;
        }

        try {
            await api.Invoke(tsuguContext, tsuguContext.MessageContext.TextOnlyTokens);
        } catch (ArgumentParseException e) {
            await tsuguContext.ReplyPlainText(api.GetErrorAndHelpText(e.Message));
        } catch (EndpointCallException e) {
            await tsuguContext.ReplyPlainText(e.Message);
        } catch (Exception e) {
            Logger.LogError("exception raised upon resolving command!\n{e}", e.ToString());
            await tsuguContext.ReplyPlainText($"后台异常！Endpoint: {api.GetType().Name}");
        }
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
                Logger.LogWarning("command [{type}] won't register because it does not inherit BaseCommand class",
                    t.Type.Name
                );

                continue;
            }

            ConstructorInfo? ctor = t.Type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, []
            );

            if (ctor == null) {
                Logger.LogWarning(
                    "command constructor {a}::{a}() not found!",
                    t.Type.Name, t.Type.Name
                );

                return;
            }

            BaseCommand command = (BaseCommand)ctor.Invoke(null);

            foreach (string alias in t.Attribute.Aliases) {
                if (!string.IsNullOrWhiteSpace(alias)) {
                    _commands[alias] = command;
                } else {
                    Logger.LogWarning("empty alias string detected! command: {type}", t.Type.Name);
                }
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
}
