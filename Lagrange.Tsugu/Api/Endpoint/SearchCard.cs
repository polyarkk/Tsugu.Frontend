using Lagrange.Tsugu.Api.Rest;
using Lagrange.Tsugu.Command;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(
    Alias = "查卡",
    Description = """
                  查询指定卡面的信息，或查询符合条件的卡面列表
                  查卡 1399：返回1399号卡面的信息
                  查卡 红 ars 4x：返回角色为ars，稀有度为4星的卡面列表
                  """,
    UsageHint = "<关键词>"
)]
public class SearchCard : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        string arg = args.ConcatenatedArgs;

        if (string.IsNullOrWhiteSpace(arg)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定查询关键词！"));

            return;
        }

        var p = new Dictionary<string, object?> {
            ["displayedServerList"] = new[] { 3, 0 },
            ["useEasyBG"] = true,
            ["compress"] = ctx.Settings.Compress
        };

        using SugaredHttpClient rest = ctx.Rest;

        await rest.InjectFuzzySearchResult(p, arg);

        RestResponse response = (await rest.TsuguPost("/searchCard", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText(
                (string.IsNullOrWhiteSpace(response.String) ? "错误：请求失败" : response.String) + "！"
            );
        }
    }
}
