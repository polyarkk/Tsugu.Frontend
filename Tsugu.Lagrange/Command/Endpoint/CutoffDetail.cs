using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["预测线", "ycx"],
    Description = "指定档位的预测线"
)]
public class CutoffDetail : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("tier", "档位"),
        Argument<uint>("eventId", "活动ID").AsOptional(),
    ];

    protected async override Task Invoke(Context ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.CutoffDetail(
            ctx.TsuguUser.MainServer,
            args["tier"].Get<uint>(),
            args["eventId"].GetOrNull<uint>(),
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
