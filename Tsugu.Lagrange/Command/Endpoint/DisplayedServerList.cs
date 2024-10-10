using Tsugu.Api.Enum;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["设置默认服务器", "默认服务器"],
    Description = "设定信息显示中的默认服务器排序"
)]
public class DisplayedServerList : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<Server>("displayedServerList", "默认服务器"),
    ];

    protected async override Task Invoke(Context ctx, ParsedArgs args) {
        Server[] displayedServerList = args[..]
            .Select(e => e.Get<Server>()).ToArray();

        await ctx.Tsugu.User.ChangeUserData(
            ctx.TsuguUser.UserId,
            displayedServerList: displayedServerList
        );

        await ctx.SendPlainText($"默认服务器排序已设定为：{string.Join(", ", displayedServerList.Select(e => e.ToString().ToLower()))}");
    }
}
