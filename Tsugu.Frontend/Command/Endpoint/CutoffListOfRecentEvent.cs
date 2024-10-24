using Tsugu.Api.Enum;
using Tsugu.Frontend.Command.Argument;
using Tsugu.Frontend.Context;

namespace Tsugu.Frontend.Command.Endpoint;

[ApiCommand(
    Aliases = ["历史预测线", "lsycx"],
    Description = "查询指定档位的预测线与最近的4期活动类型相同的活动的档线数据",
    Example = """
              ycx 1000：返回默认服务器当前活动1000档位的档线与预测线
              ycx 1000 jp 177：返回日服177号活动1000档位的档线与预测线
              """
)]
public class CutoffListOfRecentEvent : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("tier", "档位"),
        Argument<Server>("server", "服务器").AsOptional(),
        Argument<uint>("eventId", "活动ID").AsOptional(),
    ];

    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.CutoffListOfRecentEvent(
            args["server"].GetOr(() => ctx.TsuguUser.MainServer),
            args["tier"].Get<uint>(),
            args["eventId"].GetOrNull<uint>(),
            ctx.AppSettings.Compress
        );

        await ctx.ReplyImage(base64);
    }
}
