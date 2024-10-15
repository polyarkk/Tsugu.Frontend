using Lagrange.Core;
using Lagrange.Core.Message;
using Tsugu.Frontend.Context;

namespace Tsugu.Frontend.Filter;

public interface IFilter {
    Task DoFilterAsync(IMessageContext messageContext);
}
