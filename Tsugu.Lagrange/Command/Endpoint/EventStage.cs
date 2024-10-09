namespace Tsugu.Lagrange.Command.Endpoint;

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
    UsageHint = "[活动ID] [-m|+m]"
)]
public class EventStage : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        string base64 = await ctx.Tsugu.Query.EventStage(
            ctx.TsuguUser.MainServer,
            args.GetUInt32(0),
            args.GetString(1) == "+m",
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
