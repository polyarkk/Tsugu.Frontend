using Lagrange.Tsugu.Command;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(
    Alias = "ycxall",
    Name = "查询全档位预测线",
    UsageHint = "[cn|jp|tw|kr|en] [eventId] [compress]"
)]
public class CutoffAll : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        Dictionary<string, object?> p = new() {
            ["mainServer"] = args.GetEnum<BandoriServer>(0) ?? BandoriServer.Cn
        };

        if (args.HasArgument(1)) {
            p["eventId"] = args.GetInt32(1);
        }

        if (args.HasArgument(2)) {
            p["compress"] = args.GetBoolean(2);
        }

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = await rest.Post("/cutoffAll", p);

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        }
    }
}
