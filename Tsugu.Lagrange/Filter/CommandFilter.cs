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

    public async Task DoFilterAsync(BotContext botContext, MessageChain messageChain, MessageType messageType) {
        if ((messageType == MessageType.Friend && !_appSettings.IsFriendWhitelisted(messageChain.FriendUin))
            || (messageType == MessageType.Group && !_appSettings.IsGroupWhitelisted(messageChain.GroupUin))
        ) {
            Logger.LogInformation("message from [{uin}] won't handle because friend or group is not whitelisted",
                messageChain.FriendUin
            );

            return;
        }

        if (messageType == MessageType.Group
            && _appSettings.NeedMentioned && !messageChain.OfType<MentionEntity>().Any()
        ) {
            Logger.LogInformation("message from [{uin}] won't handle because it did not mention bot",
                messageChain.FriendUin
            );

            return;
        }

        List<string> tokens = [];

        foreach (TextEntity entity in messageChain.OfType<TextEntity>()) {
            tokens.AddAll(entity.ToPreviewText().Split(" "));
        }

        tokens.RemoveAll(string.IsNullOrWhiteSpace);

        if (tokens.Count == 0) {
            return;
        }

        using Context context = new(_appSettings, botContext, messageType, messageChain);

        bool isAdmin = _appSettings.Admins.Contains(messageChain.FriendUin);

        if (isAdmin && string.Equals(tokens[0], "tsugu_reload_appsettings", StringComparison.OrdinalIgnoreCase)) {
            _appSettings = _configuration.GetSection("Tsugu").Get<AppSettings>()!;
                
            await context.SendPlainText("已重新加载配置信息");

            return;
        }

#if DEBUG
        if (isAdmin && string.Equals(tokens[0], "tsugu_reload_commands", StringComparison.OrdinalIgnoreCase)) {
            LoadEndpoints();

            await context.SendPlainText("已重新加载指令集\n" + GetHelpPlainText());

            return;
        }
#endif

        if (string.Equals(tokens[0], "tsugu_help", StringComparison.OrdinalIgnoreCase)) {
            await context.SendPlainText(
                $"当前主服务器: {context.TsuguUser.MainServer.ToChineseString()}\n{GetHelpPlainText()}"
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
            await api.Invoke(context, tokens);
        } catch (ArgumentParseException e) {
            await context.SendPlainText(api.GetErrorAndHelpText(e.Message));
        } catch (EndpointCallException e) {
            await context.SendPlainText(e.Message);
        } catch (Exception e) {
            Logger.LogError("exception raised upon resolving command!\n{e}", e.ToString());
            await context.SendPlainText($"后台异常！Endpoint: {api.GetType().Name}");
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
