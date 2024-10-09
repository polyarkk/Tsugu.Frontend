using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查角色"],
    Description = "根据角色名、乐队、昵称等查询角色信息",
    Example = """
              查角色 10：返回10号角色的信息
              查角色 吉他：返回所有角色模糊搜索标签中包含吉他的角色列表
              """
)]
public class SearchCharacter : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<string>("keywords", "关键词"),
    ];

    protected async override Task Invoke(Context ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.SearchCharacter(
            ctx.TsuguUser.DisplayedServerList,
            args.ConcatenatedArgs,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
