using Tsugu.Lagrange.Api.Enum;
using Tsugu.Lagrange.Api.Rest;
using Tsugu.Lagrange.Command;

namespace Tsugu.Lagrange.Api.Endpoint;

[ApiCommand(
    Alias = "查谱面",
    Description = """
                  根据曲目ID与难度查询铺面信息
                  查谱面 1：返回1号曲的ex难度谱面
                  查谱面 128 special：返回128号曲的special难度谱面
                  """,
    UsageHint = "<乐曲ID> [难度]"
)]
public class SongChart : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        if (!args.HasArgument(0)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定乐曲ID！"));

            return;
        }

        var p = new Dictionary<string, object?> {
            ["displayedServerList"] = new[] { 3, 0 },
            ["songId"] = args.GetInt32(0),
            ["compress"] = ctx.Settings.Compress
        };

        if (args.HasArgument(1)) {
            p["difficultyId"] = args.GetString(1)!.ToChartDifficulty() ?? ChartDifficulty.Expert;
        }

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = (await rest.TsuguPost("/songChart", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText(
                (string.IsNullOrWhiteSpace(response.String) ? "错误：请求失败" : response.String) + "！"
            );
        }
    }
}
