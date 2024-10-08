namespace Lagrange.Tsugu;

[Serializable]
public class AppSettings {
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
