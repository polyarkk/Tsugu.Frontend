using Lagrange.Tsugu.Api.Enum;
using Lagrange.Tsugu.Api.Rest;
using Lagrange.Tsugu.Command;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(
    Alias = "ycxall",
    Description = "查询全档位预测线",
    UsageHint = "[活动ID] [cn|jp|tw|kr|en]"
)]
public class CutoffAll : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        Dictionary<string, object?> p = new() {
            ["mainServer"] = args.GetEnum<BandoriServer>(1) ?? BandoriServer.Cn
        };

        if (args.HasArgument(0)) {
            p["eventId"] = args.GetInt32(0);
        }

        p["compress"] = ctx.Settings.Compress;

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = (await rest.TsuguPost("/cutoffAll", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        }
    }
}
