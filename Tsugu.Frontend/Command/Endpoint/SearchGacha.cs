using Tsugu.Frontend.Command.Argument;
using Tsugu.Frontend.Context;
using Tsugu.Frontend.Util;

namespace Tsugu.Frontend.Command.Endpoint;

[ApiCommand(
    Aliases = ["查卡池"],
    Description = "查询指定卡池的信息",
    Example = "查卡池 947：返回947号卡池的信息"
)]
public class SearchGacha : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("gachaId", "卡池ID"),
    ];

    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.SearchGacha(
            ctx.TsuguUser.DisplayedServerList,
            args["gachaId"].Get<uint>(),
            false,
            ctx.AppSettings.Compress
        );

        await ctx.ReplyImage(base64);
    }
}
