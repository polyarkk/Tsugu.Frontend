using Tsugu.Lagrange.Api.Enum;
using Tsugu.Lagrange.Api.Rest;
using Tsugu.Lagrange.Command;

namespace Tsugu.Lagrange.Api.Endpoint.Query;

[ApiCommand(
    Aliases = ["查询分数表", "查分数表", "查询分数榜", "查分数榜"],
    Description = "查询歌曲分数排行表",
    UsageHint = "[cn|jp|tw|kr|en]"
)]
public class SongMeta : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        Dictionary<string, object?> p = new() {
            ["displayedServerList"] = new[] { 3, 0 },
            ["mainServer"] = args.GetEnum<BandoriServer>(0) ?? BandoriServer.Cn,
        };

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = (await rest.TsuguPost("/songMeta", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText(
                (string.IsNullOrWhiteSpace(response.String) ? "错误：请求失败" : response.String) + "！"
            );
        }
    }
}
