using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["抽卡模拟", "卡池模拟"],
    Description = "就像真的抽卡一样"
)]
public class GachaSimulate : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        OptionalArgument<uint>("次数"),
        OptionalArgument<uint>("卡池ID"),
    ];

    protected async override Task Invoke(Context ctx, ParsedCommand args) {
        string base64 = await ctx.Tsugu.Query.GachaSimulate(
            ctx.TsuguUser.MainServer,
            args.GetUInt32(1),
            args.GetUInt32(0) ?? 10,
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
