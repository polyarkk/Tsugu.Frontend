using Lagrange.Core;
using Lagrange.Core.Message;
using Microsoft.Extensions.Logging;
using System.Text;
using Tsugu.Frontend.Context;
using Tsugu.Frontend.Enum;
using Tsugu.Frontend.Util;

namespace Tsugu.Frontend.Filter;

public class AuditFilter : IFilter {
    private readonly static ILogger<AuditFilter> Logger = LoggerUtil.GetLogger<AuditFilter>();

    public Task DoFilterAsync(IMessageContext messageContext) {
        StringBuilder sb = new();
        
        sb.AppendLine($"platform       : {messageContext.Protocol}:{messageContext.Platform}");

        if (messageContext.MessageSource == MessageSource.Group) {
            sb.AppendLine($"group          : {messageContext.GroupName}({messageContext.GroupId})");
        }

        sb.AppendLine($"user           : {messageContext.FriendName}({messageContext.FriendId})");
        sb.Append($"preview text   : {messageContext.StringifiedContent}");

        Logger.LogInformation("{msg}", sb.ToString());

        return Task.CompletedTask;
    }
}
