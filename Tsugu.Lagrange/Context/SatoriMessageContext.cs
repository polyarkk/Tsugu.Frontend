using Satori.Client;
using Satori.Protocol.Elements;
using Satori.Protocol.Events;
using Satori.Protocol.Models;
using Tsugu.Lagrange.Enum;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Context;

public class SatoriMessageContext : IMessageContext {
    private readonly SatoriBot _satoriBot;

    public SatoriMessageContext(SatoriBot satoriBot, Event e) {
        _satoriBot = satoriBot;

        Platform = e.Platform;
        FriendId = e.User?.Id!;
        GroupId = e.Channel?.Id;
        StringifiedContent = e.Message?.Content ?? "";
        MessageSource = e.Channel?.Type == ChannelType.Text ? MessageSource.Friend : MessageSource.Group;

        List<string> textOnlyTokens = [];

        foreach (Element element in ElementSerializer.Deserialize(StringifiedContent)) {
            if (element is AtElement ae && ae.Id == _satoriBot.SelfId) {
                MentionedMe = true;
            }

            if (element is TextElement te) {
                textOnlyTokens.AddAll(te.Text.Split(" "));
            }
        }

        textOnlyTokens.RemoveAll(string.IsNullOrWhiteSpace);

        TextOnlyTokens = textOnlyTokens;
    }

    public string Protocol => "satori";

    public string Platform { get; }

    public string FriendId { get; }

    // 无论私聊还是群发该字段都不为 null，私聊会话也被视为频道
    public string? GroupId { get; }
    
    public bool MentionedMe { get; }

    public string StringifiedContent { get; }

    public IReadOnlyList<string> TextOnlyTokens { get; }

    public MessageSource MessageSource { get; }

    public async Task ReplyPlainText(string text) {
        await _satoriBot.CreateMessageAsync(GroupId!, GetDefaultElement(new TextElement { Text = text }));
    }

    public async Task ReplyImage(params string[] base64List) {
        await _satoriBot.CreateMessageAsync(GroupId!,
            GetDefaultElement(
                (
                    from b in base64List
                    select new ImageElement {
                        Src = $"data:image/{WhichTypeOfImageIsThis(b)};base64,{b}",
                    }
                ).ToArray<Element>()
            )
        );
    }

    private static string WhichTypeOfImageIsThis(string base64) {
        string data = base64[..5];

        return data.ToUpper() switch {
            "IVBOR" => "png",
            "/9J/4" => "jpg",
            "JVBER" => "pdf",
            "AAABA" => "ico",
            "R0lGO" => "gif",
            _ => string.Empty,
        };
    }

    private List<Element> GetDefaultElement(params Element[] elements) {
        List<Element> e = [];

        if (MessageSource == MessageSource.Group) {
            e.Add(new AtElement { Id = FriendId });
        }

        e.AddAll(elements);

        return e;
    }
}
