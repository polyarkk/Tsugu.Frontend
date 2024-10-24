using Tsugu.Api.Enum;
using Tsugu.Frontend.Command.Argument;
using Tsugu.Frontend.Context;

namespace Tsugu.Frontend.Command.Endpoint;

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
        Argument<uint>("eventId", "活动ID").AsOptional(),
        Argument<Server>("server", "服务器").AsOptional()
            .WithMatcher(ArgumentMatchers.ToServerEnumMatcher),
    ];

    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.CutoffAll(
            args["server"].GetOr(() => ctx.TsuguUser.MainServer),
            args["eventId"].GetOrNull<uint>(),
            ctx.AppSettings.Compress
        );

        await ctx.ReplyImage(base64);
    }
}
