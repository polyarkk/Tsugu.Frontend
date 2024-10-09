using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["抽卡模拟", "卡池模拟"],
    Description = "就像真的抽卡一样"
)]
public class GachaSimulate : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        OptionalArgument<uint>("times", "次数"),
        OptionalArgument<uint>("gachaId", "卡池ID"),
    ];

    protected async override Task Invoke(Context ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.GachaSimulate(
            ctx.TsuguUser.MainServer,
            args["gachaId"].GetOrNull<uint>(),
            args["times"].GetOr(() => 10u),
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
