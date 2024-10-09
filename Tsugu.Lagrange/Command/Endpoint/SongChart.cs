using Tsugu.Api.Enum;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查谱面", "查铺面"],
    Description = "根据曲目ID与难度查询铺面信息",
    Example = """
              查谱面 1：返回1号曲的ex难度谱面
              查谱面 128 special：返回128号曲的special难度谱面
              """
)]
public class SongChart : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("乐曲ID"),
        OptionalArgument<ChartDifficulty>("难度"),
    ];

    protected async override Task Invoke(Context ctx, ParsedCommand args) {
        string base64 = await ctx.Tsugu.Query.SongChart(
            ctx.TsuguUser.DisplayedServerList,
            (uint)args.GetUInt32(0)!,
            args.GetEnum<ChartDifficulty>(1) ?? ChartDifficulty.Expert,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
