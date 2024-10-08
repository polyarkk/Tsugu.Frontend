using Lagrange.Core;
using Lagrange.Core.Event;
using Lagrange.Core.Message;
using Lagrange.Tsugu.Api;
using Microsoft.Extensions.Logging;

namespace Lagrange.Tsugu;

public class Context(
    AppSettings appSettings,
    BotContext botContext,
    EventBase @event,
    MessageChain messageChain,
    IHttpClientFactory httpClientFactory,
    ILoggerFactory loggerFactory
) {
    public AppSettings Settings => appSettings;
    
    public BotContext Bot => botContext;

    public EventBase Event => @event;

    public MessageChain Chain => messageChain;

    public SugaredHttpClient Rest => new(httpClientFactory, loggerFactory, appSettings);
}
