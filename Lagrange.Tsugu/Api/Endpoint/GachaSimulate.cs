using Lagrange.Tsugu.Command;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(Alias = "卡池模拟", Description = "模拟抽卡", UsageHint = "[cn|jp|tw|kr|en] [次数] [卡池ID] [是否压缩图片]")]
public class GachaSimulate : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        Dictionary<string, object?> p = new() {
            ["mainServer"] = args.GetEnum<BandoriServer>(0) ?? BandoriServer.Cn
        };

        if (args.HasArgument(1)) {
            p["times"] = args.GetInt32(1);
        }

        if (args.HasArgument(2)) {
            p["gachaId"] = args.GetBoolean(2);
        }

        p["compress"] = args.GetBoolean(3) ?? ctx.Settings.Compress;

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = await rest.TsuguPost("/gachaSimulate", p);

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        }
    }
}
