using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查活动"],
    Description = "查询指定活动的信息，或查询符合条件的活动列表。",
    Example = """
              查活动 253：返回253期活动的信息
              查活动 ag 蓝：返回Afterglow乐队、Cool属性的活动列表
              """
)]
public class SearchEvent : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<string>("keywords", "关键词"),
    ];

    protected async override Task Invoke(Context ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.SearchEvent(
            ctx.TsuguUser.DisplayedServerList,
            args.ConcatenatedArgs,
            false,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
