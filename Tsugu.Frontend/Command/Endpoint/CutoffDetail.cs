using Tsugu.Frontend.Command.Argument;
using Tsugu.Frontend.Context;
using Tsugu.Frontend.Util;

namespace Tsugu.Frontend.Command.Endpoint;

[ApiCommand(
    Aliases = ["预测线", "ycx"],
    Description = "指定档位的预测线"
)]
public class CutoffDetail : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("tier", "档位"),
        Argument<uint>("eventId", "活动ID").AsOptional(),
    ];

    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.CutoffDetail(
            ctx.TsuguUser.MainServer,
            args["tier"].Get<uint>(),
            args["eventId"].GetOrNull<uint>(),
            ctx.AppSettings.Compress
        );

        await ctx.ReplyImage(base64);
    }
}
