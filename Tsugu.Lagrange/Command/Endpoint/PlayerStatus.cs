using Tsugu.Api.Entity;
using Tsugu.Api.Enum;
using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Context;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["玩家状态"],
    Description = "查询自己的玩家状态",
    Example = """
              玩家状态：查询你当前主账号的玩家状态
              玩家状态 2：查询在第二个绑定的玩家的状态
              玩家状态 2 jp：查询在日服绑定的第二个玩家的状态
              """
)]
public class PlayerStatus : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("index", "玩家编号").AsOptional(),
        Argument<Server>("server", "服务器").AsOptional()
            .WithMatcher(ArgumentMatchers.ToServerEnumMatcher),
    ];

    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        uint index = args["index"].GetOr(() => 0u);
        Server? server = args["server"].GetOrNull<Server>();

        TsuguUser.TsuguUserServerInList[] players;

        if (server == null) {
            players = ctx.TsuguUser.UserPlayerList;
        } else {
            players = (
                from player in ctx.TsuguUser.UserPlayerList
                where player.Server == server
                select player
            ).ToArray();
        }

        if (index >= players.Length) {
            await ctx.ReplyPlainText(
                (server != null ? $"服务器 [{server.Value.ToChineseString()}] " : "")
                + $"未找到记录 {index}，请先绑定"
            );

            return;
        }

        string base64 = await ctx.Tsugu.Query.SearchPlayer(players[0].PlayerId, players[0].Server);

        await ctx.ReplyImage(base64);
    }
}
