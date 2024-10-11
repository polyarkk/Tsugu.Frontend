using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["抽卡模拟", "卡池模拟"],
    Description = "就像真的抽卡一样"
)]
public class GachaSimulate : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("times", "次数").AsOptional(),
        Argument<uint>("gachaId", "卡池ID").AsOptional(),
    ];

    protected async override Task InvokeInternal(Context ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.GachaSimulate(
            ctx.TsuguUser.MainServer,
            args["gachaId"].GetOrNull<uint>(),
            args["times"].GetOr(() => 10u),
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
