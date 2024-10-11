using Tsugu.Api.Entity;
using Tsugu.Api.Enum;
using Tsugu.Api.Misc;

namespace Tsugu.Api.Endpoint;

public class UserEndpoint(TsuguHttpClient client) {
    /// <summary>
    /// 获取指定用户的数据。
    /// </summary>
    /// <param name="userId">用户的 ID。</param>
    /// <param name="platform">用户的平台名称。</param>
    /// <returns>用户数据</returns>
    public async Task<TsuguUser> GetUserData(string userId, string platform) {
        return await client.StationSend<TsuguUser>(
            HttpMethod.Post, "/user/getUserData", new { UserId = userId, Platform = platform }
        );
    }

    /// <summary>
    /// 修改指定用户的主服务器、默认服务器列表或房间号，不传则不修改。
    /// </summary>
    /// <param name="userId">用户的 ID。</param>
    /// <param name="platform">用户的平台名称。</param>
    /// <param name="mainServer">用户的主服务器模式。</param>
    /// <param name="displayedServerList">用户的默认服务器列表。</param>
    /// <param name="shareRoomNumber">是否转发该用户的房间号。false 则会忽视来自该用户的房间号。</param>
    public async Task ChangeUserData(
        string userId, string platform,
        Server? mainServer = null, Server[]? displayedServerList = null, bool? shareRoomNumber = null
    ) {
        Dictionary<string, object> o = new();

        if (mainServer != null) {
            o["mainServer"] = mainServer;
        }

        if (displayedServerList != null) {
            o["displayedServerList"] = displayedServerList;
        }

        if (shareRoomNumber != null) {
            o["shareRoomNumber"] = shareRoomNumber;
        }

        await client.StationSend(
            HttpMethod.Post, "/user/changeUserData",
            new { UserId = userId, Update = o, Platform = platform }
        );
    }

    /// <summary>
    /// 向后端发送绑定或解绑游戏内玩家的请求。
    /// </summary>
    /// <param name="userId">用户的 ID。</param>
    /// <param name="platform">用户的平台名称。</param>
    /// <returns>验证码</returns>
    public async Task<uint> BindPlayerRequest(string userId, string platform) {
        VerifyCode verifyCode = await client.StationSend<VerifyCode>(
            HttpMethod.Post, "/user/bindPlayerRequest",
            new {
                UserId = userId,
                Platform = platform,
            }
        );

        return verifyCode.Code;
    }

    /// <summary>
    /// 向后端发送验证绑定/解绑玩家的请求。
    /// </summary>
    /// <param name="userId">用户的 ID。</param>
    /// <param name="server">要绑定的服务器。</param>
    /// <param name="playerId">要进行操作的玩家 ID。</param>
    /// <param name="platform">用户的平台名称。</param>
    /// <param name="unbind">是否进行解绑操作。</param>
    /// <exception cref="EndpointCallException">任何原因导致绑定/解绑失败都将抛出异常</exception>
    public async Task BindPlayerVerification(
        string userId, Server server, uint playerId, string platform, bool unbind = false
    ) {
        await client.StationSend(
            HttpMethod.Post, "/user/bindPlayerVerification",
            new {
                UserId = userId,
                Server = server,
                PlayerId = playerId,
                BindingAction = unbind ? "unbind" : "bind",
                Platform = platform,
            }
        );
    }
}
