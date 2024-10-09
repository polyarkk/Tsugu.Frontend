using Tsugu.Lagrange.Api.Enum;
using Tsugu.Lagrange.Api.Rest;
using Tsugu.Lagrange.Command;

namespace Tsugu.Lagrange.Api.Endpoint.Query;

[ApiCommand(
    Aliases = ["预测线总览", "ycxall"],
    Description = "查询全档位预测线",
    UsageHint = "[活动ID] [cn|jp|tw|kr|en]"
)]
public class CutoffAll : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        Dictionary<string, object?> p = new() {
            ["mainServer"] = args.GetEnum<BandoriServer>(1) ?? BandoriServer.Cn,
        };

        if (args.HasArgument(0)) {
            p["eventId"] = args.GetInt32(0);
        }

        p["compress"] = ctx.Settings.Compress;

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = (await rest.TsuguPost("/cutoffAll", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText(
                (string.IsNullOrWhiteSpace(response.String) ? "错误：请求失败" : response.String) + "！"
            );
        }
    }
}
