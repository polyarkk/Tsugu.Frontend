using Tsugu.Lagrange.Api.Enum;
using Tsugu.Lagrange.Api.Rest;
using Tsugu.Lagrange.Command;

namespace Tsugu.Lagrange.Api.Endpoint.Query;

[ApiCommand(
    Aliases = ["随机曲"],
    Description = """
                  根据关键词或曲目ID随机曲目信息
                  使用示例:
                  随机曲 lv27：在所有包含27等级难度的曲中, 随机返回其中一个
                  随机曲 ag en：返回国际服存在的随机的 Afterglow 曲目
                  """,
    UsageHint = "<关键词> [cn|jp|tw|kr|en]"
)]
public class SongRandom : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        Dictionary<string, object?> p = new() {
            ["compress"] = ctx.Settings.Compress,
        };

        if (System.Enum.TryParse(args[0], true, out BandoriServer mainServer)) {
            p["mainServer"] = mainServer;
            
            if (args.Length > 1) {
                p["text"] = string.Join(" ", args[..^1]);
            }
        } else {
            p["mainServer"] = BandoriServer.Cn;
            
            if (args.Length >= 1) {
                p["text"] = args.ConcatenatedArgs;
            }
        }

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = (await rest.TsuguPost("/songRandom", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText(
                (string.IsNullOrWhiteSpace(response.String) ? "错误：请求失败" : response.String) + "！"
            );
        }
    }
}
