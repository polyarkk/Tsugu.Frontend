using Tsugu.Frontend.Command.Argument;
using Tsugu.Frontend.Context;
using Tsugu.Frontend.Util;

namespace Tsugu.Frontend.Command.Endpoint;

[ApiCommand(
    Aliases = ["查卡"],
    Description = "查询指定卡面的信息，或查询符合条件的卡面列表",
    Example = """
              查卡 1399：返回1399号卡面的信息
              查卡 红 ars 4x：返回角色为ars，稀有度为4星的卡面列表
              """
)]
public class SearchCard : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<string>("keywords", "关键词"),
    ];

    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.SearchCard(
            ctx.TsuguUser.DisplayedServerList,
            args.ConcatenatedArgs,
            false,
            ctx.AppSettings.Compress
        );

        await ctx.ReplyImage(base64);
    }
}
