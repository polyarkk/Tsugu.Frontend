using Tsugu.Lagrange.Api.Rest;
using Tsugu.Lagrange.Command;

namespace Tsugu.Lagrange.Api.Endpoint.Query;

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

        Dictionary<string, object?> p = new() {
            ["displayedServerList"] = new[] { 3, 0 },
            ["compress"] = ctx.Settings.Compress
        };

        using SugaredHttpClient rest = ctx.Rest;

        await rest.InjectFuzzySearchResult(p, arg);

        RestResponse response = (await rest.TsuguPost("/searchCharacter", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText(
                (string.IsNullOrWhiteSpace(response.String) ? "错误：请求失败" : response.String) + "！"
            );
        }
    }
}
