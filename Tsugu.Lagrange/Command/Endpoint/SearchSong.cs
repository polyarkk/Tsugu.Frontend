namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查曲"],
    Description = """
                  根据关键词或曲目ID查询曲目信息
                  使用示例:
                  查曲 1：返回1号曲的信息
                  查曲 ag lv27：返回所有难度为27的ag曲列表
                  """,
    UsageHint = "<关键词>"
)]
public class SearchSong : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        string arg = args.ConcatenatedArgs;

        if (string.IsNullOrWhiteSpace(arg)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定查询关键词！"));

            return;
        }

        string base64 = await ctx.Tsugu.Query.SearchSong(
            ctx.TsuguUser.DisplayedServerList,
            arg,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
