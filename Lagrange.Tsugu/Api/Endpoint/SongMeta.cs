using Lagrange.Tsugu.Api.Enum;
using Lagrange.Tsugu.Api.Rest;
using Lagrange.Tsugu.Command;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(
    Alias = "查分数表",
    Description = "查询歌曲分数排行表",
    UsageHint = "[cn|jp|tw|kr|en]"
)]
public class SongMeta : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        var p = new Dictionary<string, object?> {
            ["displayedServerList"] = new[] { 3, 0 },
            ["mainServer"] = args.GetEnum<BandoriServer>(0) ?? BandoriServer.Cn
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
