using Tsugu.Api.Enum;
using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["主服务器", "设置主服务器"],
    Description = "将指定的服务器设置为你的主服务器"
)]
public class MainServer : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<Server>("mainServer", "主服务器")
            .WithMatcher(ArgumentMatchers.ToServerEnumMatcher),
    ];

    protected async override Task InvokeInternal(Context ctx, ParsedArgs args) {
        Server mainServer = args["mainServer"].Get<Server>();

        await ctx.Tsugu.User.ChangeUserData(
            ctx.TsuguUser.UserId, Constant.Platform,
            mainServer: mainServer
        );

        await ctx.SendPlainText($"主服务器已设定为：{mainServer.ToChineseString()}");
    }
}
