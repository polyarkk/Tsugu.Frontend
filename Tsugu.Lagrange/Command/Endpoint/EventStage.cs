using Tsugu.Api.Enum;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查试炼", "查stage", "查舞台", "查festival", "查5v5"],
    Description = "查询活动的试炼信息",
    Example = """
              查试炼 true 157 jp：返回日服的157号活动的试炼信息，包含歌曲meta
              查试炼 false 157：返回157号活动的试炼信息，不包含歌曲meta
              查试炼 true：返回当前活动的试炼信息，包含歌曲meta
              查试炼：返回当前活动的试炼信息
              """
)]
public class EventStage : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        OptionalArgument<bool>("是否显示歌曲Meta"),
        OptionalArgument<uint>("活动ID"),
        OptionalArgument<Server>("服务器"),
    ];

    protected async override Task Invoke(Context ctx, ParsedCommand args) {
        string base64 = await ctx.Tsugu.Query.EventStage(
            args.GetEnum<Server>(2) ?? ctx.TsuguUser.MainServer,
            args.GetUInt32(1),
            args.GetBoolean(0) ?? false,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
