using Tsugu.Api.Entity;
using Tsugu.Frontend.Command.Argument;
using Tsugu.Frontend.Context;
using Tsugu.Frontend.Util;

namespace Tsugu.Frontend.Command.Endpoint;

[ApiCommand(
    Aliases = ["有车吗", "车来", "ycm"],
    Description = "获取车站信息"
)]
public class RoomList : BaseCommand {
    protected async override Task InvokeInternal(TsuguContext ctx, ParsedArgs args) {
        Room[] rooms = await ctx.Tsugu.Station.QueryAllRoom();

        string base64 = await ctx.Tsugu.Query.RoomList(rooms);

        await ctx.ReplyImage(base64);
    }
}
