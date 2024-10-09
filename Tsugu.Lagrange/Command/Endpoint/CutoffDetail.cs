namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["预测线", "ycx"],
    Description = "查询指定档位预测线",
    UsageHint = "<档位> [活动ID]"
)]
public class CutoffDetail : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
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
