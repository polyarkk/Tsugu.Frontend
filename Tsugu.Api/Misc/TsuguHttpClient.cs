using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Tsugu.Api.Entity;

namespace Tsugu.Api.Misc;

public class TsuguHttpClient : HttpClient {
    /// <summary>
    /// 对图片接口发送请求
    /// </summary>
    /// <param name="endpoint">端点</param>
    /// <param name="params">参数</param>
    /// <returns>图片 Base64 数组</returns>
    /// <exception cref="EndpointCallException">任何原因导致获取到的不是图片则抛出此异常</exception>
    public async Task<string[]> TsuguPost(string endpoint, object? @params = null) {
        ImageRestResponse[] imageRestResponses =
            await BaseHttpCall<ImageRestResponse[]>(HttpMethod.Post, endpoint, @params ?? new { })
            ?? throw new EndpointCallException("null");

        if (imageRestResponses.Length == 0) {
            return [];
        }

        List<string> base64List = [];

        foreach (ImageRestResponse imageRestResponse in imageRestResponses) {
            if (!imageRestResponse.IsImageBase64) {
                throw new EndpointCallException(imageRestResponse.String ?? endpoint);
            }

            base64List.Add(imageRestResponse.String!);
        }

        return base64List.ToArray();
    }

    /// <summary>
    /// 对非图片接口发送请求
    /// </summary>
    /// <param name="method">请求方法</param>
    /// <param name="endpoint">端点</param>
    /// <param name="params">参数</param>
    /// <typeparam name="T">任意可序列化类</typeparam>
    /// <returns>数据实例</returns>
    /// <exception cref="EndpointCallException">任何原因导致状态不是 success 都将抛出此异常</exception>
    public async Task<T> StationSend<T>(
        HttpMethod method, string endpoint, object? @params = null
    ) {
        StationRestResponse stationRestResponse =
            await BaseHttpCall<StationRestResponse>(method, endpoint, @params ?? new { })
            ?? throw new EndpointCallException("null");

        if (!stationRestResponse.IsSuccess) {
            throw new EndpointCallException(stationRestResponse.Data.GetString());
        }

        return stationRestResponse.Data.DeserializeJson<T>()!;
    }

    /// <inheritdoc cref="StationSend{T}"/>
    public async Task StationSend(
        HttpMethod method, string endpoint, object? @params = null
    ) {
        StationRestResponse stationRestResponse =
            await BaseHttpCall<StationRestResponse>(method, endpoint, @params ?? new { })
            ?? throw new EndpointCallException("null");

        if (!stationRestResponse.IsSuccess) {
            throw new EndpointCallException(stationRestResponse.Data.GetString());
        }
    }

    private async Task<T?> BaseHttpCall<T>(HttpMethod method, string endpoint, object? @params) {
        string json = @params.SerializeJson();

        HttpRequestMessage msg = new(method, $"{endpoint}");

        msg.Headers.Accept.Clear();
        msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        msg.Content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await SendAsync(msg);

        string content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK) {
            return content.DeserializeJson<T>();
        }

        try {
            return content.DeserializeJson<T>();
        } catch {
            throw new EndpointCallException(
                $"failed to fetch data from endpoint [{endpoint}], status: {response.StatusCode}, message: {content}"
            );
        }
    }
}
