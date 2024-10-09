namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查活动"],
    Description = """
                  查询指定活动的信息，或查询符合条件的活动列表。
                  使用示例:
                  查活动 253：返回253期活动的信息
                  查活动 ag 蓝：返回Afterglow乐队、Cool属性的活动列表
                  """,
    UsageHint = "<关键词>"
)]
public class SearchEvent : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        string arg = args.ConcatenatedArgs;

        if (string.IsNullOrWhiteSpace(arg)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定查询关键词！"));

            return;
        }
        
        string base64 = await ctx.Tsugu.Query.SearchEvent(
            ctx.TsuguUser.DisplayedServerList,
            arg,
            false,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
