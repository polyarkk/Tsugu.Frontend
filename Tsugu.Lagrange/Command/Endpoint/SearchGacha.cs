namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查卡池"],
    Description = "查询指定卡池的信息",
    UsageHint = "<卡池ID>"
)]
public class SearchGacha : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        if (!args.HasArgument(0)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定卡池ID！"));

            return;
        }

        string base64 = await ctx.Tsugu.Query.SearchGacha(
            ctx.TsuguUser.DisplayedServerList,
            (uint)args.GetUInt32(0)!,
            false,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
