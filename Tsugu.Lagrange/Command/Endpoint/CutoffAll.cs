namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["预测线总览", "ycxall"],
    Description = "查询全档位预测线",
    UsageHint = "[活动ID] [cn|jp|tw|kr|en]"
)]
public class CutoffAll : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        string base64 = await ctx.Tsugu.Query.CutoffAll(
            ctx.TsuguUser.MainServer,
            args.GetUInt32(0),
            ctx.AppSettings.Compress
        );
        
        await ctx.SendImage(base64);
    }
}
