namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查玩家", "查询玩家"],
    Description = "查询指定服务器的指定玩家的状态图片，仅能查询到已公开的信息",
    UsageHint = "<玩家ID>"
)]
public class SearchPlayer : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        if (!args.HasArgument(0)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定玩家ID！"));
            
            return;
        }

        string base64 = await ctx.Tsugu.Query.SearchPlayer(
            (uint)args.GetUInt32(0)!,
            ctx.TsuguUser.DisplayedServerList,
            false,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
