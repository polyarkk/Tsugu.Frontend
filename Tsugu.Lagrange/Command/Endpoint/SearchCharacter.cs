namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查角色"],
    Description = """
                  根据角色名、乐队、昵称等查询角色信息
                  使用示例:
                  查角色 10：返回10号角色的信息
                  查角色 吉他：返回所有角色模糊搜索标签中包含吉他的角色列表
                  """,
    UsageHint = "<关键词>"
)]
public class SearchCharacter : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        string arg = args.ConcatenatedArgs;

        if (string.IsNullOrWhiteSpace(arg)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定查询关键词！"));

            return;
        }

        string base64 = await ctx.Tsugu.Query.SearchCharacter(
            ctx.TsuguUser.DisplayedServerList,
            arg,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
