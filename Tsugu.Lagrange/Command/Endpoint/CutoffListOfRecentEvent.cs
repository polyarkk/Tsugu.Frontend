using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["历史预测线", "lsycx"],
    Description = "查询指定档位的预测线与最近的4期活动类型相同的活动的档线数据"
)]
public class CutoffListOfRecentEvent : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("档位"),
        OptionalArgument<uint>("活动ID"),
    ];

    protected async override Task Invoke(Context ctx, ParsedCommand args) {
        string base64 = await ctx.Tsugu.Query.CutoffListOfRecentEvent(
            ctx.TsuguUser.MainServer,
            (uint)args.GetUInt32(0)!,
            args.GetUInt32(1),
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
