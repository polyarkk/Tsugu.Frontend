using Tsugu.Lagrange.Api.Enum;
using Tsugu.Lagrange.Api.Rest;
using Tsugu.Lagrange.Command;

namespace Tsugu.Lagrange.Api.Endpoint.Query;

[ApiCommand(
    Aliases = ["查试炼", "查stage", "查舞台", "查festival", "查5v5"],
    Description = """
                  查询活动的试炼信息
                  使用示例:
                  查试炼 +m 157 jp：返回日服的157号活动的试炼信息，包含歌曲meta
                  查试炼 -m 157：返回157号活动的试炼信息，不包含歌曲meta
                  查试炼 +m：返回当前活动的试炼信息，包含歌曲meta
                  查试炼：返回当前活动的试炼信息
                  """,
    UsageHint = "[-m|+m] [活动ID] [cn|jp|tw|kr|en]"
)]
public class EventStage : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        Dictionary<string, object?> p = new() {
            ["mainServer"] = args.GetEnum<BandoriServer>(2) ?? BandoriServer.Cn,
            ["meta"] = args.GetString(0) == "+m",
            ["compress"] = ctx.Settings.Compress,
        };

        if (args.HasArgument(1)) {
            p["eventId"] = args.GetInt32(1);
        }

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = (await rest.TsuguPost("/eventStage", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText(
                (string.IsNullOrWhiteSpace(response.String) ? "错误：请求失败" : response.String) + "！"
            );
        }
    }
}
