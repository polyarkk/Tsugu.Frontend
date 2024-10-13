using Lagrange.Core;
using Lagrange.Core.Message;
using Tsugu.Lagrange.Context;

namespace Tsugu.Lagrange.Filter;

public interface IFilter {
    Task DoFilterAsync(IMessageContext messageContext);
}
