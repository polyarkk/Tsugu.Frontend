using Lagrange.Core;
using Lagrange.Core.Message;
using Microsoft.Extensions.Logging;
using System.Text;
using Tsugu.Lagrange.Context;
using Tsugu.Lagrange.Enum;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Filter;

public class AuditFilter : IFilter {
    private readonly static ILogger<AuditFilter> Logger = LoggerUtil.GetLogger<AuditFilter>();

    public Task DoFilterAsync(IMessageContext messageContext) {
        StringBuilder sb = new();

        if (messageContext.MessageSource == MessageSource.Group) {
            sb.AppendLine($"group          : {messageContext.GroupId}");
        }

        sb.AppendLine($"user           : {messageContext.FriendId}({messageContext.FriendId})");
        sb.Append($"preview text   : {messageContext.StringifiedContent}");

        Logger.LogInformation("{msg}", sb.ToString());

        return Task.CompletedTask;
    }
}
