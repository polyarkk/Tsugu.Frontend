using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tsugu.Frontend.Context;
using Tsugu.Frontend.Filter;

namespace Tsugu.Frontend.Service;

public class FilterService {
    private readonly ILogger<FilterService> _logger;
    
    private readonly List<IFilter> _filters;

    public FilterService(ILogger<FilterService> logger, IConfiguration configuration) {
        _logger = logger;
        _filters = [];
        
        RegisterFilter(new AuditFilter());
        RegisterFilter(new CommandFilter(configuration));
    }
    
    private void RegisterFilter(IFilter filter) {
        _filters.Add(filter);
    }

    public async Task InvokeFilters(IMessageContext messageContext) {
        foreach (IFilter filter in _filters) {
            try {
                await filter.DoFilterAsync(messageContext);
            } catch (Exception ex) {
                _logger.LogError("exception raised upon handling filter [{filter}]!\n{e}", filter.GetType().Name, ex.ToString());
            }
        }
    }
}
