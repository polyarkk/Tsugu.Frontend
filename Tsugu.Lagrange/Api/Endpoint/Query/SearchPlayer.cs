using Tsugu.Lagrange.Api.Enum;
using Tsugu.Lagrange.Api.Rest;
using Tsugu.Lagrange.Command;

namespace Tsugu.Lagrange.Api.Endpoint.Query;

[ApiCommand(
    Aliases = ["查玩家", "查询玩家"],
    Description = "查询指定服务器的指定玩家的状态图片，仅能查询到已公开的信息",
    UsageHint = "<玩家ID> [cn|jp|tw|kr|en]"
)]
public class SearchPlayer : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        if (!args.HasArgument(0)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定玩家ID！"));
            
            return;
        }
        
        var p = new Dictionary<string, object?> {
            ["playerId"] = args.GetInt32(0),
            ["mainServer"] = args.GetEnum<BandoriServer>(1) ?? BandoriServer.Cn,
            ["useEasyBG"] = true,
            ["compress"] = ctx.Settings.Compress
        };

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = (await rest.TsuguPost("/searchPlayer", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText(
                (string.IsNullOrWhiteSpace(response.String) ? "错误：请求失败" : response.String) + "！"
            );
        }
    }
}
