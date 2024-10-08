using Tsugu.Lagrange.Api.Rest;
using Tsugu.Lagrange.Command;

namespace Tsugu.Lagrange.Api.Endpoint.Query;

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

        Dictionary<string, object?> p = new() {
            ["displayedServerList"] = new[] { 3, 0 },
            ["useEasyBG"] = true,
            ["compress"] = ctx.Settings.Compress
        };

        using SugaredHttpClient rest = ctx.Rest;

        await rest.InjectFuzzySearchResult(p, arg);

        RestResponse response = (await rest.TsuguPost("/searchEvent", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText(
                (string.IsNullOrWhiteSpace(response.String) ? "错误：请求失败" : response.String) + "！"
            );
        }
    }
}
