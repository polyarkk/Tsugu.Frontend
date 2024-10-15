using Tsugu.Frontend.Command.Argument;
using Tsugu.Frontend.Context;
using Tsugu.Frontend.Util;

namespace Tsugu.Frontend.Command.Endpoint;

[ApiCommand(
    Aliases = ["抽卡模拟", "卡池模拟"],
    Description = "就像真的抽卡一样"
)]
public class GachaSimulate : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("times", "次数").AsOptional(),
        Argument<uint>("gachaId", "卡池ID").AsOptional(),
    ];

    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.GachaSimulate(
            ctx.TsuguUser.MainServer,
            args["gachaId"].GetOrNull<uint>(),
            args["times"].GetOr(() => 10u),
            ctx.AppSettings.Compress
        );

        await ctx.ReplyImage(base64);
    }
}
