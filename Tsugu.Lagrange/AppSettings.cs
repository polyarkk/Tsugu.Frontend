namespace Tsugu.Lagrange;

[Serializable]
public class AppSettings {
    /// <summary>
    /// Tsugu 后端地址
    /// </summary>
    public string BackendUrl { get; set; } = "http://tsugubot.com:8080";
    
    /// <summary>
    /// 是否启用白名单模式，启用后只允许与 Groups 和 Friends 中列出的群或用户对话
    /// </summary>
    public bool Whitelisted { get; set; } = true;

    /// <summary>
    /// 白/黑名单群
    /// </summary>
    public uint[] Groups { get; set; } = [];

    /// <summary>
    /// 白/黑名单私聊用户
    /// </summary>
    public uint[] Friends { get; set; } = [];

    /// <summary>
    /// 默认返回时压缩图片
    /// </summary>
    public bool Compress { get; set; } = true;

    /// <summary>
    /// 群聊天是否需要被@才会触发指令
    /// </summary>
    public bool NeedMentioned { get; set; }

    /// <summary>
    /// 机器人管理员
    /// </summary>
    public uint[] Admins { get; set; } = [];

    /// <summary>
    /// Satori 配置
    /// </summary>
    public SatoriConfig Satori { get; set; } = new();
    
    public class SatoriConfig {
        /// <summary>
        /// 是否启用 Satori
        /// </summary>
        public bool Enabled { get; set; } = false;
        
        /// <summary>
        /// Koishi 中将 server-satori 的 path 配置修改为 "/"
        /// </summary>
        public string Server { get; set; } = "http://127.0.0.1:5140/";

        /// <summary>
        /// 若服务端无需 Token 则填入 null
        /// </summary>
        public string? Token { get; set; } = null;

        /// <summary>
        /// 服务端中需要接管的 Bot
        /// </summary>
        public BotConfig[] Bots { get; set; } = [];

        public class BotConfig {
            public string Platform { get; set; } = "";

            public string SelfId { get; set; } = "";
        }
    }

    public bool IsGroupWhitelisted(uint? groupUin) {
        // 忽略私聊情况
        if (groupUin == null) {
            return true;
        }

        bool whitelisted = Groups.Contains((uint)groupUin);

        return Whitelisted ? whitelisted : !whitelisted;
    }

    public bool IsFriendWhitelisted(uint friendUin) {
        bool whitelisted = Friends.Contains(friendUin);

        return Whitelisted ? whitelisted : !whitelisted;
    }
}
