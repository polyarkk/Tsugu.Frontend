using Tsugu.Api.Entity;
using Tsugu.Api.Misc;

namespace Tsugu.Api.Endpoint;

public class StationEndpoint(TsuguHttpClient client) {
    /// <summary>
    /// QQ、Onebot、Chronocat 的平台名称。
    /// </summary>
    private const string Platform = "red";

    /// <summary>
    /// 向车站上传房间信息。
    /// </summary>
    /// <param name="number">上传的房间号。</param>
    /// <param name="rawMessage">房间号的备注信息，用于对房间进行说明。</param>
    /// <param name="userId">用户的 ID。</param>
    /// <param name="userName">用户的名称。</param>
    /// <param name="time">房间上传的时间。</param>
    /// <param name="platform">用户所在平台。</param>
    /// <param name="bandoriStationToken">使用的车站令牌，不传入则使用 Tsugu 的令牌。</param>
    /// <exception cref="EndpointCallException">任何原因导致上传失败都将抛出异常</exception>
    public async Task SubmitRoomNumber(
        uint number, string rawMessage, string userId, string userName,
        ulong? time = null, string platform = Platform, string? bandoriStationToken = null
    ) {
        Dictionary<string, object> o = new() {
            ["number"] = number,
            ["rawMessage"] = rawMessage,
            ["userId"] = userId,
            ["userName"] = userName,
            ["platform"] = platform,
        };

        if (time == null) {
            o["time"] = DateTime.Now.ToUnixTimeMilliseconds();
        } else {
            o["time"] = time;
        }

        if (bandoriStationToken != null) {
            o["bandoriStationToken"] = bandoriStationToken;
        }

        await client.StationSend<object>(HttpMethod.Post, "/station/submitRoomNumber", o);
    }

    /// <summary>
    /// 查询车站里最近的房间数据列表。
    /// </summary>
    /// <returns>房间列表</returns>
    public async Task<Room[]> QueryAllRoom() {
        return await client.StationSend<Room[]>(HttpMethod.Get, "/station/queryAllRoom");
    }

    /// <summary>
    /// 备用模糊搜索过滤器接口
    /// </summary>
    /// <param name="text">关键词</param>
    /// <returns>过滤结果</returns>
    /// <seealso cref="Tsugu.Api.Misc.FuzzySearch" />
    public async Task<Dictionary<string, List<string>>> FuzzySearch(string text) {
        return await client.StationSend<Dictionary<string, List<string>>>(
            HttpMethod.Get, "/station/queryAllRoom",
            new { Text = text }
        );
    }
}
