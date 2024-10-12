using Tsugu.Api.Enum;
using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查谱面", "查铺面"],
    Description = "根据曲目ID与难度查询铺面信息",
    Example = """
              查谱面 1：返回1号曲的expert难度谱面
              查谱面 128 sp：返回128号曲的special难度谱面
              """
)]
public class SongChart : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("songId", "乐曲ID"),
        Argument<ChartDifficulty>("difficulty", "难度")
            .AsOptional()
            .WithMatcher(ArgumentMatchers.ToChartDifficultyEnumMatcher),
    ];

    protected async override Task InvokeInternal(Context ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.SongChart(
            ctx.TsuguUser.DisplayedServerList,
            args["songId"].Get<uint>(),
            args["difficulty"].GetOr(() => ChartDifficulty.Expert),
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
