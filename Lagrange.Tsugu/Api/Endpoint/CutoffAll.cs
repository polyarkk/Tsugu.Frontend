using Lagrange.Tsugu.Command;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(
    Alias = "ycxall",
    Description = "查询全档位预测线",
    UsageHint = "[cn|jp|tw|kr|en] [活动ID] [是否压缩图片]"
)]
public class CutoffAll : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        Dictionary<string, object?> p = new() {
            ["mainServer"] = args.GetEnum<BandoriServer>(0) ?? BandoriServer.Cn
        };

        if (args.HasArgument(1)) {
            p["eventId"] = args.GetInt32(1);
        }

        p["compress"] = args.GetBoolean(2) ?? ctx.Settings.Compress;

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = (await rest.TsuguPost("/cutoffAll", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        }
    }
}
