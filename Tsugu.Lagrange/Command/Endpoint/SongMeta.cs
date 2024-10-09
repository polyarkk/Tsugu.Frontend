namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查询分数表", "查分数表", "查询分数榜", "查分数榜"],
    Description = "查询歌曲分数排行表",
    UsageHint = "[cn|jp|tw|kr|en]"
)]
public class SongMeta : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        string base64 = await ctx.Tsugu.Query.SongMeta(
            ctx.TsuguUser.DisplayedServerList,
            ctx.TsuguUser.MainServer,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
