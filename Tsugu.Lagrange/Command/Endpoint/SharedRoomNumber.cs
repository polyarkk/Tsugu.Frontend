using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["车牌转发", "设置车牌转发"],
    Description = "开启/关闭个人车牌转发"
)]
public class SharedRoomNumber : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<bool>("toggle", "是否开启"),
    ];

    protected async override Task Invoke(Context ctx, ParsedArgs args) {
        bool toggle = args["toggle"].Get<bool>();
        
        await ctx.Tsugu.User.ChangeUserData(
            ctx.TsuguUser.UserId,
            sharedRoomNumber: toggle
        );

        await ctx.SendPlainText($"已{(toggle ? "开启" : "关闭")}个人车牌转发");
    }
}
