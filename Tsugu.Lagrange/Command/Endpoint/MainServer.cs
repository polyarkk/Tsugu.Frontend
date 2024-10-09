using Tsugu.Api.Enum;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["主服务器", "设置主服务器"],
    Description = "将指定的服务器设置为你的主服务器"
)]
public class MainServer : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<Server>("主服务器"),
    ];

    protected async override Task Invoke(Context ctx, ParsedCommand args) {
        Server mainServer = (Server)args.GetEnum<Server>(0)!;

        await ctx.Tsugu.User.ChangeUserData(
            ctx.TsuguUser.UserId,
            mainServer: mainServer
        );

        await ctx.SendPlainText($"主服务器已设定为：{mainServer.ToString()!.ToLower()}");
    }
}
