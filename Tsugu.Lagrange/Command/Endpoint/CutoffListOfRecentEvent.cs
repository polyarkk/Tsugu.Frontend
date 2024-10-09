namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["历史预测线", "lsycx"],
    Description = "查询与指定活动相关的指定档位的历史预测线",
    UsageHint = "<档位> [活动ID]"
)]
public class CutoffListOfRecentEvent : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        if (!args.HasArgument(0)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定档位！"));

            return;
        }
        
        string base64 = await ctx.Tsugu.Query.CutoffListOfRecentEvent(
            ctx.TsuguUser.MainServer,
            (uint)args.GetUInt32(0)!,
            args.GetUInt32(1),
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
