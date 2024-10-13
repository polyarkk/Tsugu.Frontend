using Lagrange.Core;
using Lagrange.Core.Message;
using Tsugu.Api;
using Tsugu.Api.Entity;

namespace Tsugu.Lagrange.Context;

public class TsuguContext : IDisposable {
    public TsuguContext(
        AppSettings appAppSettings,
        IMessageContext messageContext
    ) {
        AppSettings = appAppSettings;
        MessageContext = messageContext;
        Tsugu = new TsuguClient(appAppSettings.BackendUrl);
        _tsuguUser = new Lazy<TsuguUser>(() => Tsugu.User.GetUserData(messageContext.FriendId, messageContext.Platform).Result);
    }

    private readonly Lazy<TsuguUser> _tsuguUser;

    public AppSettings AppSettings { get; }

    public IMessageContext MessageContext { get; }

    public TsuguClient Tsugu { get; }

    public TsuguUser TsuguUser => _tsuguUser.Value;

    public async Task ReplyPlainText(string text) {
        await MessageContext.ReplyPlainText(text);
    }

    public async Task ReplyImage(params string[] base64List) {
        await MessageContext.ReplyImage(base64List);
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
        Tsugu.Dispose();
    }
}
