using Lagrange.Tsugu.Command;
using System.Reflection;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(
    Alias = "ycx",
    Name = "查询指定档位预测线",
    UsageHint = "<tier> [cn|jp|tw|kr|en] [eventId] [compress]"
)]
public class CutoffDetail : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        if (!args.HasArgument(0)) {
            ApiCommand attr = GetAttribute();
            
            await ctx.SendPlainText($"错误：未指定排名档位！用法：{attr.Alias} {attr.UsageHint}");

            return;
        }

        Dictionary<string, object?> p = new();

        p["tier"] = args.GetInt32(0);

        p["mainServer"] = args.GetEnum<BandoriServer>(1) ?? BandoriServer.Cn;

        if (args.HasArgument(2)) {
            p["eventId"] = args.GetBoolean(2);
        }

        if (args.HasArgument(3)) {
            p["compress"] = args.GetBoolean(3);
        }

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = await rest.Post("/cutoffDetail", p);

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        }
    }
}
