using Tsugu.Api.Enum;
using Tsugu.Frontend.Command.Argument;
using Tsugu.Frontend.Context;
using Tsugu.Frontend.Util;

namespace Tsugu.Frontend.Command.Endpoint;

[ApiCommand(
    Aliases = ["主服务器", "设置主服务器"],
    Description = "将指定的服务器设置为你的主服务器"
)]
public class MainServer : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<Server>("mainServer", "主服务器")
            .WithMatcher(ArgumentMatchers.ToServerEnumMatcher),
    ];

    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        Server mainServer = args["mainServer"].Get<Server>();

        await ctx.Tsugu.User.ChangeUserData(
            ctx.TsuguUser.UserId, ctx.MessageContext.Platform,
            mainServer: mainServer
        );

        await ctx.ReplyPlainText($"主服务器已设定为：{mainServer.ToChineseString()}");
    }
}
