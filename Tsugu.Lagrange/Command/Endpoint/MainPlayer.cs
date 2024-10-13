using System.Text;
using Tsugu.Api.Entity;
using Tsugu.Api.Enum;
using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Context;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["主账号"],
    Description = "设定默认玩家状态、车牌展示中的主账号使用第几个账号",
    Example = """
              主账号：返回所有账号列表
              主账号 2：将第二个账号设置为主账号
              """
)]
public class MainPlayer : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("index", "编号").AsOptional(),
    ];

    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        if (ctx.TsuguUser.UserPlayerList.Length == 0) {
            await ctx.ReplyPlainText($"未绑定任何账号，请先绑定");

            return;
        }

        uint? index = args["index"].GetOrNull<uint>();

        if (index != null) {
            if ((int)index - 1 >= ctx.TsuguUser.UserPlayerList.Length || (int)index - 1 < 0) {
                await ctx.ReplyPlainText($"未找到记录 {index}，请先绑定");

                return;
            }

            await ctx.Tsugu.User.ChangeUserData(ctx.MessageContext.FriendId, ctx.MessageContext.Platform,
                userPlayerIndex: index - 1
            );

            await ctx.ReplyPlainText($"已将玩家 #{index} 设置为主账号");

            return;
        }

        StringBuilder sb = new("请选择你要设置为主账号的账号数字：");

        for (int i = 0; i < ctx.TsuguUser.UserPlayerList.Length; i++) {
            sb.Append(
                $"\n{i + 1}. [{ctx.TsuguUser.UserPlayerList[i].Server.ToChineseString()}] {MaskPlayerId(ctx.TsuguUser.UserPlayerList[i].PlayerId)}"
            );
        }

        sb.Append("\n例如：主账号 1");

        await ctx.ReplyPlainText(sb.ToString());
    }

    private static string MaskPlayerId(uint playerId) {
        string pid = playerId.ToString();

        if (string.IsNullOrWhiteSpace(pid)) {
            return pid;
        }

        int length = pid.Length;

        if (length <= 4) {
            return pid;
        }

        int startLength = (length - 4) / 2;

        string maskedPart = new('*', 4);
        string maskedNumber = string.Concat(pid.AsSpan()[..startLength], maskedPart, pid.AsSpan(startLength + 4));

        return maskedNumber;
    }
}
