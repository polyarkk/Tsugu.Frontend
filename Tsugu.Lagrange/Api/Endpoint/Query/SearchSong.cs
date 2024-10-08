using Tsugu.Lagrange.Api.Rest;
using Tsugu.Lagrange.Command;

namespace Tsugu.Lagrange.Api.Endpoint.Query;

[ApiCommand(
    Aliases = ["查曲"],
    Description = """
                  根据关键词或曲目ID查询曲目信息
                  查曲 1：返回1号曲的信息
                  查曲 ag lv27：返回所有难度为27的ag曲列表
                  """,
    UsageHint = "<关键词>"
)]
public class SearchSong : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        string arg = args.ConcatenatedArgs;

        if (string.IsNullOrWhiteSpace(arg)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定查询关键词！"));

            return;
        }

        var p = new Dictionary<string, object?> {
            ["displayedServerList"] = new[] { 3, 0 },
            ["compress"] = ctx.Settings.Compress
        };

        using SugaredHttpClient rest = ctx.Rest;

        await rest.InjectFuzzySearchResult(p, arg);

        RestResponse response = (await rest.TsuguPost("/searchSong", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText(
                (string.IsNullOrWhiteSpace(response.String) ? "错误：请求失败" : response.String) + "！"
            );
        }
    }
}
