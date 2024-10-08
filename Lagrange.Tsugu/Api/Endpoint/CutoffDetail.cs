using Lagrange.Tsugu.Api.Enum;
using Lagrange.Tsugu.Api.Rest;
using Lagrange.Tsugu.Command;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(
    Alias = "ycx",
    Description = "查询指定档位预测线",
    UsageHint = "<档位> [活动ID] [cn|jp|tw|kr|en]"
)]
public class CutoffDetail : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        if (!args.HasArgument(0)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定档位！"));

            return;
        }

        Dictionary<string, object?> p = new() {
            ["tier"] = args.GetInt32(0)
        };

        if (args.HasArgument(1)) {
            p["eventId"] = args.GetBoolean(1);
        }

        p["mainServer"] = args.GetEnum<BandoriServer>(2) ?? BandoriServer.Cn;

        p["compress"] = ctx.Settings.Compress;

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = (await rest.TsuguPost("/cutoffDetail", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        }
    }
}
