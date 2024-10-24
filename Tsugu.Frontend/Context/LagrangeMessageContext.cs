using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;
using Tsugu.Frontend.Enum;
using Tsugu.Frontend.Util;

namespace Tsugu.Frontend.Context;

public class LagrangeMessageContext : IMessageContext {
    private readonly BotContext _botContext;

    public LagrangeMessageContext(BotContext botContext, MessageChain messageChain, MessageSource messageSource) {
        _botContext = botContext;

        BotId = botContext.BotUin.ToString();
        
        FriendId = messageChain.FriendUin.ToString();
        FriendName = messageChain.FriendInfo?.Nickname ?? FriendId;
        
        GroupId = messageChain.GroupUin?.ToString();
        GroupName = GroupId;
        StringifiedContent = messageChain.ToPreviewText();
        MessageSource = messageSource;

        List<string> textOnlyTokens = [];

        foreach (IMessageEntity entity in messageChain) {
            if (entity is MentionEntity me && me.Uin == botContext.BotUin) {
                MentionedMe = true;
            }

            if (entity is TextEntity te) {
                textOnlyTokens.AddAll(te.ToPreviewText().Split(" "));
            }
        }

        textOnlyTokens.RemoveAll(string.IsNullOrWhiteSpace);

        TextOnlyTokens = textOnlyTokens;
    }

    public string Protocol => "lagrange";
    
    public string Platform => "red";
    
    public string BotId { get; }
    
    public string FriendName { get; }

    public string FriendId { get; }

    public string? GroupName { get; }

    public string? GroupId { get; }

    public bool MentionedMe { get; }

    public string StringifiedContent { get; }

    public IReadOnlyList<string> TextOnlyTokens { get; }

    public MessageSource MessageSource { get; }

    public async Task ReplyPlainText(string text) {
        await _botContext.SendMessage(GetDefaultMessageBuilder().Text(text).Build());
    }

    public async Task ReplyImage(params string[] base64List) {
        MessageBuilder mb = GetDefaultMessageBuilder();

        foreach (string b in base64List) {
            mb.Image(Convert.FromBase64String(b));
        }

        await _botContext.SendMessage(mb.Build());
    }

    public async Task ReplyPoke() {
        _ = MessageSource == MessageSource.Group
            ? await _botContext.GroupPoke(uint.Parse(GroupId!), uint.Parse(FriendId))
            : await _botContext.FriendPoke(uint.Parse(FriendId));
    }

    private MessageBuilder GetDefaultMessageBuilder() {
        return MessageSource == MessageSource.Group
            ? MessageBuilder.Group(uint.Parse(GroupId!)).Mention(uint.Parse(FriendId))
            : MessageBuilder.Friend(uint.Parse(FriendId));
    }
}
