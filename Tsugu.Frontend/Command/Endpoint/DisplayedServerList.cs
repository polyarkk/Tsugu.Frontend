﻿using Tsugu.Api.Enum;
using Tsugu.Frontend.Command.Argument;
using Tsugu.Frontend.Context;

namespace Tsugu.Frontend.Command.Endpoint;

[ApiCommand(
    Aliases = ["设置默认服务器", "默认服务器"],
    Description = "设定信息显示中的默认服务器排序"
)]
public class DisplayedServerList : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<Server>("displayedServerList", "若干默认服务器")
            .WithMatcher(ArgumentMatchers.ToServerEnumMatcher),
    ];

    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        Server[] displayedServerList = args.GetVararg("displayedServerList")
            .Select(e => e.Get<Server>()).ToArray();

        await ctx.Tsugu.User.ChangeUserData(
            ctx.TsuguUser.UserId, ctx.MessageContext.Platform,
            displayedServerList: displayedServerList
        );

        await ctx.ReplyPlainText($"默认服务器排序已设定为：{string.Join(", ", displayedServerList.Select(e => e.ToChineseString()))}");
    }
}
