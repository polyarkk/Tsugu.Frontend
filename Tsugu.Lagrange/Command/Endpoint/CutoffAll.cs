using Tsugu.Api.Enum;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["预测线总览", "ycxall"],
    Description = "指定档位的预测线",
    Example = """
              ycxall 177：返回177号活动的全部档位预测线
              ycxall 177 jp：返回日服177号活动的全部档位预测线
              """
)]
public class CutoffAll : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        OptionalArgument<uint>("活动ID"),
        OptionalArgument<Server>("服务器"),
    ];

    protected async override Task Invoke(Context ctx, ParsedCommand args) {
        string base64 = await ctx.Tsugu.Query.CutoffAll(
            args.GetEnum<Server>(1) ?? ctx.TsuguUser.MainServer,
            args.GetUInt32(0),
            ctx.AppSettings.Compress
        );

        await ctx.SendImage(base64);
    }
}
