using Lagrange.Core;
using Lagrange.Core.Message;

namespace Tsugu.Lagrange.Filter;

public interface IFilter {
    Task DoFilterAsync(BotContext botContext, MessageChain messageChain, MessageType messageType);
}
