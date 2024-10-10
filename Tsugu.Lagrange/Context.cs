using Lagrange.Core;
using Lagrange.Core.Event;
using Lagrange.Core.Message;
using Microsoft.Extensions.DependencyInjection;
using Tsugu.Api;
using Tsugu.Api.Entity;

namespace Tsugu.Lagrange;

public class Context : IDisposable {
    public Context(
        AppSettings appAppSettings,
        BotContext botContext,
        EventBase @event,
        MessageChain messageChain
    ) {
        AppSettings = appAppSettings;
        Bot = botContext;
        Event = @event;
        Chain = messageChain;
        Tsugu = new TsuguClient(appAppSettings.BackendUrl);
        _tsuguUser = new Lazy<TsuguUser>(() => Tsugu.User.GetUserData(Chain.FriendUin.ToString()).Result);
    }

    private readonly Lazy<TsuguUser> _tsuguUser;

    public AppSettings AppSettings { get; }
    
    public ServiceProvider Services { get; }

    public BotContext Bot { get; }

    public EventBase Event { get; }

    public MessageChain Chain { get; }

    public TsuguClient Tsugu { get; }

    public TsuguUser TsuguUser => _tsuguUser.Value;

    public void Dispose() {
        GC.SuppressFinalize(this);
        Tsugu.Dispose();
    }
}
