using Tsugu.Api.Enum;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查谱面", "查铺面"],
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
        
        string base64 = await ctx.Tsugu.Query.SongChart(
            ctx.TsuguUser.DisplayedServerList,
            (uint)args.GetUInt32(0)!,
            args.GetEnum<ChartDifficulty>(1) ?? ChartDifficulty.Expert,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
