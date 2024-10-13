using Tsugu.Api;
using Tsugu.Api.Enum;
using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Context;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["绑定玩家", "解除绑定"],
    Description = "请求绑定/解除绑定游戏账号",
    Example = """
              绑定玩家 114514：绑定默认服务器中玩家ID为114514的玩家
              绑定玩家 1919810 jp：绑定日服玩家ID为1919810的玩家
              解除绑定 666：解绑默认服务器中玩家ID为666的玩家
              """
)]
public class BindPlayerRequest : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("playerId", "玩家ID"),
        Argument<Server>("server", "服务器").AsOptional()
            .WithMatcher(ArgumentMatchers.ToServerEnumMatcher),
    ];

    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        Server server = args["server"].GetOr(() => ctx.TsuguUser.MainServer);
        uint playerId = args["playerId"].Get<uint>();

        using BestdoriClient bestdori = new();

        if (!await bestdori.IsValidPlayer(playerId, server)) {
            await ctx.ReplyPlainText($"服务器 [{server.ToChineseString()}] 不存在该玩家！");

            return;
        }
        
        bool unbind = args.Alias == "解除绑定";
        bool bound = ctx.TsuguUser.UserPlayerList.Any(player =>
            player.PlayerId == playerId && player.Server == server
        );

        if (!unbind && bound) {
            await ctx.ReplyPlainText($"服务器 [{server.ToChineseString()}] 已绑定过该玩家！");

            return;
        }

        if (unbind && !bound) {
            await ctx.ReplyPlainText($"服务器 [{server.ToChineseString()}] 没有绑定过该玩家！");

            return;
        }

        string userId = ctx.MessageContext.FriendId;

        uint verifyCode = await ctx.Tsugu.User.BindPlayerRequest(userId, ctx.MessageContext.Platform);

        _ = new BindPlayerVerificationTimer(
            ctx.AppSettings.BackendUrl, ctx.MessageContext, playerId, server, unbind
        );

        await ctx.ReplyPlainText(
            $"""
             已进入{(unbind ? "解除绑定" : "绑定")}流程，请在 1.5 分钟内将游戏账号的 *个性签名* 或者 *当前使用的乐队编队名称* 改为
             {verifyCode}
             期间将自动进行验证流程并返回结果
             """
        );
    }
}
