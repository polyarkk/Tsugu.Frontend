using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查曲"],
    Description = "根据关键词或曲目ID查询曲目信息",
    Example = """
              查曲 1：返回1号曲的信息
              查曲 ag lv27：返回所有难度为27的ag曲列表
              """
)]
public class SearchSong : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<string>("songId", "关键词"),
    ];

    protected async override Task Invoke(Context ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.SearchSong(
            ctx.TsuguUser.DisplayedServerList,
            args.ConcatenatedArgs,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
