using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Event.EventArg;
using Lagrange.Core.Message;

namespace Tsugu.Lagrange.Util;

internal static class MessageUtil {
    public static MessageBuilder GetDefaultMessageBuilder(uint friendUin, uint? groupUin) {
        return groupUin != null
            ? MessageBuilder.Group((uint)groupUin).Mention(friendUin)
            : MessageBuilder.Friend(friendUin);
    }
    
    public static MessageBuilder GetDefaultMessageBuilder(Context ctx) {
        return ctx.MessageType == MessageType.Group
            ? MessageBuilder.Group((uint)ctx.Chain.GroupUin!).Mention(ctx.Chain.FriendUin)
            : MessageBuilder.Friend(ctx.Chain.FriendUin);
    }

    public async static Task SendPlainText(this Context ctx, string str) {
        await ctx.Bot.SendMessage(GetDefaultMessageBuilder(ctx).Text(str).Build());
    }

    public async static Task SendImage(this Context ctx, string base64) {
        await ctx.Bot.SendMessage(GetDefaultMessageBuilder(ctx).Image(Convert.FromBase64String(base64)).Build());
    }

    public async static Task SendHybrid(this Context ctx, Action<MessageBuilder> action) {
        MessageBuilder messageBuilder = GetDefaultMessageBuilder(ctx);

        action.Invoke(messageBuilder);

        await ctx.Bot.SendMessage(messageBuilder.Build());
    }
}
