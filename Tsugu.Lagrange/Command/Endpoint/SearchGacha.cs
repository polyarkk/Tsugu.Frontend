using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查卡池"],
    Description = "查询指定卡池的信息"
)]
public class SearchGacha : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("卡池ID"),
    ];

    protected async override Task Invoke(Context ctx, ParsedCommand args) {
        string base64 = await ctx.Tsugu.Query.SearchGacha(
            ctx.TsuguUser.DisplayedServerList,
            (uint)args.GetUInt32(0)!,
            false,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
