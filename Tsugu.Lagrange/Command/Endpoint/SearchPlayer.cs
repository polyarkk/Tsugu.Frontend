using Tsugu.Api.Enum;
using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Context;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查玩家", "查询玩家"],
    Description = "查询指定服务器的指定玩家的状态图片，仅能查询到已公开的信息",
    Example = """
              查玩家 10000000： 查询你当前默认服务器中，玩家ID为10000000的玩家信息
              查玩家 40474621 jp： 查询日服玩家ID为40474621的玩家信息
              """
)]
public class SearchPlayer : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("playerId", "玩家ID"),
        Argument<Server>("server", "服务器").AsOptional()
            .WithMatcher(ArgumentMatchers.ToServerEnumMatcher),
    ];

    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        string base64 = await ctx.Tsugu.Query.SearchPlayer(
            args["playerId"].Get<uint>(),
            args["server"].GetOr(() => ctx.TsuguUser.MainServer),
            false,
            ctx.AppSettings.Compress
        );

        await ctx.ReplyImage(base64);
    }
}
