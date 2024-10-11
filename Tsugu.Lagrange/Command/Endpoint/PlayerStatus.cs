using Tsugu.Api.Entity;
using Tsugu.Api.Enum;
using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["玩家状态"],
    Description = "查询自己的玩家状态",
    Example = """
              玩家状态：查询你当前默认服务器的玩家状态
              玩家状态 jp：查询日服玩家状态
              """
)]
public class PlayerStatus : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<Server>("server", "服务器").AsOptional(),
    ];

    protected async override Task InvokeInternal(Context ctx, ParsedArgs args) {
        Server server = args["server"].GetOr(() => ctx.TsuguUser.MainServer);

        foreach (TsuguUser.TsuguUserServerInList item in ctx.TsuguUser.UserPlayerList) {
            if (item.Server != server) {
                continue;
            }

            string base64 = await ctx.Tsugu.Query.SearchPlayer(item.PlayerId, server);

            await ctx.SendImage(base64);

            return;
        }

        await ctx.SendPlainText($"服务器 [{server.ToLowerString()}] 未绑定过玩家！");
    }
}
