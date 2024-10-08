using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Event.EventArg;
using Lagrange.Core.Message;
using System.Text.Json;

namespace Tsugu.Lagrange;

internal static class Util {
    public readonly static JsonSerializerOptions JsonConfig = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static T? DeserializeJson<T>(this string json) { return JsonSerializer.Deserialize<T>(json, JsonConfig); }

    public static string SerializeJson<T>(this T obj) { return JsonSerializer.Serialize(obj, JsonConfig); }
    
    public static MessageBuilder GetDefaultMessageBuilder(Context ctx) {
        return ctx.Event is GroupMessageEvent
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
