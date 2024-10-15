namespace Tsugu.Frontend;

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
    /// 白/黑名单QQ群/服务器，格式：协议名:平台名:服务器ID，对于 Lagrange 协议，平台只能为 red
    /// </summary>
    public string[] Groups { get; set; } = [];
    
    /// <summary>
    /// 白/黑名单私聊用户，格式：协议名:平台名:用户ID，对于 Lagrange 协议，平台只能为 red
    /// </summary>
    public string[] Friends { get; set; } = [];

    /// <summary>
    /// 默认返回时压缩图片
    /// </summary>
    public bool Compress { get; set; } = true;

    /// <summary>
    /// 群聊天是否需要被@才会触发指令
    /// </summary>
    public bool NeedMentioned { get; set; }

    /// <summary>
    /// 机器人管理员，格式：协议名:平台名:用户ID，对于 Lagrange 协议，平台只能为 red
    /// </summary>
    public string[] Admins { get; set; } = [];

    public LagrangeConfig Lagrange { get; set; } = new();

    /// <summary>
    /// Satori 配置
    /// </summary>
    public SatoriConfig Satori { get; set; } = new();

    public class LagrangeConfig {
        /// <summary>
        /// 是否启用 Lagrange
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
    
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

    public bool IsGroupAllowed(string protocol, string platform, string? groupId) {
        // 忽略私聊情况
        if (groupId == null) {
            return true;
        }

        bool whitelisted = Groups.Contains($"{protocol}:{platform}:{groupId}");

        return Whitelisted ? whitelisted : !whitelisted;
    }

    public bool IsFriendAllowed(string userIdentifier) {
        bool whitelisted = Friends.Contains(userIdentifier);

        return Whitelisted ? whitelisted : !whitelisted;
    }
}
