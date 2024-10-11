using System.Text.Json.Serialization;
using Tsugu.Api.Enum;

namespace Tsugu.Api.Entity;

public class TsuguUser {
    public TsuguUser(
        string? id, string userId, string platform, 
        Server mainServer, Server[] displayedServerList, bool shareRoomNumber, 
        TsuguUserServerInList[] userPlayerList
    ) {
        Id = id;
        UserId = userId;
        Platform = platform;
        MainServer = mainServer;
        DisplayedServerList = displayedServerList;
        ShareRoomNumber = shareRoomNumber;
        UserPlayerList = userPlayerList;
    }

    /// <summary>
    /// 若用户已存在，此字段为用户的唯一标识符。
    /// </summary>
    [JsonPropertyName("_id")] public string? Id { get; set; }

    /// <summary>
    /// 用户 ID。
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 用户所在平台。
    /// </summary>
    public string Platform { get; set; }

    /// <summary>
    /// 用户的主服务器模式。
    /// </summary>
    public Server MainServer { get; set; }

    /// <summary>
    /// 用户的默认服务器列表。
    /// </summary>
    public Server[] DisplayedServerList { get; set; }

    /// <summary>
    /// 是否转发该用户的房间号。false 则会忽视来自该用户的房间号。
    /// </summary>
    public bool ShareRoomNumber { get; set; }
    
    /// <summary>
    /// 主账号
    /// </summary>
    public int UserPlayerIndex { get; set; }

    /// <summary>
    /// 该用户绑定的服务器数据列表。
    /// </summary>
    public TsuguUserServerInList[] UserPlayerList { get; set; }

    public class TsuguUserServerInList {
        /// <summary>
        /// 玩家 ID。
        /// </summary>
        public uint PlayerId { get; set; }

        /// <summary>
        /// 玩家 ID 对应的服务器。
        /// </summary>
        public Server Server { get; set; }
    }
}
