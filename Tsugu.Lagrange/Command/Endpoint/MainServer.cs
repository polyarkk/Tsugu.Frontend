using Tsugu.Api.Enum;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["主服务器", "设置主服务器"],
    Description = "将指定的服务器设置为你的主服务器",
    UsageHint = "<cn|jp|tw|en|kr>"
)]
public class MainServer : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        Server? mainServer = args.GetEnum<Server>(0);

        if (mainServer == null) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定主服务器"));

            return;
        }

        await ctx.Tsugu.User.ChangeUserData(
            ctx.TsuguUser.UserId,
            mainServer: mainServer
        );

        await ctx.SendPlainText($"主服务器已设定为：{mainServer.ToString()!.ToLower()}");
    }
}
