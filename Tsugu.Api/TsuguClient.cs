using Tsugu.Api.Endpoint;
using Tsugu.Api.Misc;

namespace Tsugu.Api;

public class TsuguClient : IDisposable {
    /// <summary>
    /// 查询 API
    /// </summary>
    public QueryEndpoint Query { get; private set; }

    /// <summary>
    /// 用户数据 API
    /// </summary>
    public UserEndpoint User { get; private set; }

    /// <summary>
    /// 车站 API
    /// </summary>
    public StationEndpoint Station { get; private set; }

    private TsuguHttpClient _httpClient;

    public TsuguClient(string baseAddress = "http://tsugubot.com:8080") {
        TsuguHttpClient httpClient = new() {
            BaseAddress = new Uri(baseAddress),
        };

        Query = new QueryEndpoint(httpClient);
        User = new UserEndpoint(httpClient);
        Station = new StationEndpoint(httpClient);

        _httpClient = httpClient;
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
        _httpClient.Dispose();
    }
}
