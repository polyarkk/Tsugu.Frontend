using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["预测线", "ycx"],
    Description = "指定档位的预测线"
)]
public class CutoffDetail : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("档位"),
        OptionalArgument<uint>("活动ID"),
    ];

    protected async override Task Invoke(Context ctx, ParsedCommand args) {
        if (!args.HasArgument(0)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定档位！"));

            return;
        }

        string base64 = await ctx.Tsugu.Query.CutoffDetail(
            ctx.TsuguUser.MainServer,
            (uint)args.GetUInt32(0)!,
            args.GetUInt32(1),
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
