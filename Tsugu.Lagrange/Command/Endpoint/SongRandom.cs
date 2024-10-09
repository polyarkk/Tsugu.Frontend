namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["随机曲"],
    Description = """
                  根据关键词或曲目ID随机曲目信息
                  使用示例:
                  随机曲 lv27：在所有包含27等级难度的曲中, 随机返回其中一个
                  随机曲 ag en：返回国际服存在的随机的 Afterglow 曲目
                  """,
    UsageHint = "<关键词> [cn|jp|tw|kr|en]"
)]
public class SongRandom : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        string base64 = await ctx.Tsugu.Query.SongRandom(
            ctx.TsuguUser.MainServer,
            args.ConcatenatedArgs,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
