using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Context;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查询分数表", "查分数表", "查询分数榜", "查分数榜"],
    Description = "查询歌曲分数排行表"
)]
public class SongMeta : BaseCommand {
    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.SongMeta(
            ctx.TsuguUser.DisplayedServerList,
            ctx.TsuguUser.MainServer,
            ctx.AppSettings.Compress
        );

        await ctx.ReplyImage(base64);
    }
}
