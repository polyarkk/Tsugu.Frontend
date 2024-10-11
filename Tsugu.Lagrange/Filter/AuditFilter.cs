using Lagrange.Core;
using Lagrange.Core.Message;
using Microsoft.Extensions.Logging;
using System.Text;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Filter;

public class AuditFilter : IFilter {
    private readonly static ILogger<AuditFilter> Logger = LoggerUtil.GetLogger<AuditFilter>();
    
    public Task DoFilterAsync(BotContext botContext, MessageChain messageChain, MessageType messageType) {
        StringBuilder sb = new();

        if (messageType == MessageType.Group) {
            sb.AppendLine($"group          : {messageChain.GroupUin}");
            sb.AppendLine($"user           : {messageChain.GroupMemberInfo?.MemberName}({messageChain.GroupMemberInfo?.Uin})");
        } else {
            sb.AppendLine($"user           : {messageChain.FriendInfo?.Nickname}({messageChain.FriendInfo?.Uin})");
        }

        StringBuilder previewBuilder = new();

        foreach (IMessageEntity entity in messageChain) {
            previewBuilder.Append(entity.ToPreviewText());
        }

        sb.Append($"preview text   : {previewBuilder.ToString()}");

        Logger.LogInformation("{msg}", sb.ToString());

        return Task.CompletedTask;
    }
}
