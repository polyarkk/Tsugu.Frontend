using Tsugu.Api.Entity;
using Tsugu.Lagrange.Command.Argument;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["有车吗", "车来", "ycm"],
    Description = "获取车站信息"
)]
public class RoomList : BaseCommand {
    protected async override Task InvokeInternal(Context ctx, ParsedArgs args) {
        Room[] rooms = await ctx.Tsugu.Station.QueryAllRoom();

        string base64 = await ctx.Tsugu.Query.RoomList(rooms);

        await ctx.SendImage(base64);
    }
}
