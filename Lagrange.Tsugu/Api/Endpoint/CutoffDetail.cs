using Lagrange.Tsugu.Command;
using System.Reflection;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(
    Alias = "ycx",
    Description = "查询指定档位预测线",
    UsageHint = "<档位> [cn|jp|tw|kr|en] [活动ID] [是否压缩图片]"
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

        p["compress"] = args.GetBoolean(3) ?? ctx.Settings.Compress;

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = await rest.TsuguPost("/cutoffDetail", p);

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        }
    }
}
