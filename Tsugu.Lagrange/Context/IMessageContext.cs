using Tsugu.Lagrange.Enum;

namespace Tsugu.Lagrange.Context;

public interface IMessageContext {
    string UserIdentifier => $"{Protocol}:{Platform}:{FriendId}";
    
    string Protocol { get; }
    
    string Platform { get; }
    
    string FriendName { get; }
    
    string FriendId { get; }
    
    string? GroupName { get; }
    
    string? GroupId { get; }
    
    bool MentionedMe { get; }
    
    /// <summary>
    /// 消息内容
    /// </summary>
    string StringifiedContent { get; }
    
    /// <summary>
    /// 与 StringifiedContent 不同，该字段只包含文字内容
    /// </summary>
    IReadOnlyList<string> TextOnlyTokens { get; }
    
    MessageSource MessageSource { get; }
    
    Task ReplyPlainText(string text);

    Task ReplyImage(params string[] base64List);
}
