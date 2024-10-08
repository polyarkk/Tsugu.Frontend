using Tsugu.Lagrange.Api.Rest;
using Tsugu.Lagrange.Command;

namespace Tsugu.Lagrange.Api.Endpoint;

[ApiCommand(
    Alias = "查卡池",
    Description = "查询指定卡池的信息",
    UsageHint = "<卡池ID>"
)]
public class SearchGacha : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        if (!args.HasArgument(0)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定卡池ID！"));

            return;
        }

        var p = new Dictionary<string, object?> {
            ["displayedServerList"] = new[] { 3, 0 },
            ["gachaId"] = args.GetInt32(0),
            ["useEasyBG"] = true,
            ["compress"] = ctx.Settings.Compress
        };

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = (await rest.TsuguPost("/searchGacha", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText(
                (string.IsNullOrWhiteSpace(response.String) ? "错误：请求失败" : response.String) + "！"
            );
        }
    }
}
