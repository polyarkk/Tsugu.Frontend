using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查玩家", "查询玩家"],
    Description = "查询指定服务器的指定玩家的状态图片，仅能查询到已公开的信息"
)]
public class SearchPlayer : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("playerId", "玩家ID"),
    ];

    protected async override Task Invoke(Context ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.SearchPlayer(
            args["playerId"].Get<uint>(),
            ctx.TsuguUser.DisplayedServerList,
            false,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
