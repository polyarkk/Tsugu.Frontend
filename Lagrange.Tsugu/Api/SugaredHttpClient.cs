﻿using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Lagrange.Tsugu.Api;

public class SugaredHttpClient : IDisposable {
    // private const string ApiUrl = "http://tsugubot.com:8080";
    private const string ApiUrl = "http://localhost:3000";

    private readonly HttpClient _httpClient;

    private readonly ILogger _logger;

    public SugaredHttpClient(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory) {
        _httpClient = httpClientFactory.CreateClient();
        _logger = loggerFactory.CreateLogger("Lagrange.Tsugu.Api.SugaredHttpClient");
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
        _httpClient.Dispose();
    }

    public async Task<RestResponse> Post(string endpoint, Dictionary<string, object?> bodyParams) {
        string json = Util.SerializeJson(bodyParams);

        _logger.LogInformation("endpoint: {ep}, json: {json}", endpoint, json);

        HttpRequestMessage msg = new(HttpMethod.Post, $"{ApiUrl}{endpoint}");

        msg.Headers.Accept.Clear();
        msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        msg.Content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.SendAsync(msg);

        string content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK) {
            return Util.DeserializeJson<List<RestResponse>>(content)![0];
        }

        _logger.LogError("failed to fetch data from endpoint [{ep}], status: {status}, message: {msg}",
            endpoint, response.StatusCode, content
        );

        return new RestResponse("string", "");
    }

    public async Task<RestResponse> Get(string endpoint, Dictionary<string, object> bodyParams) {
        throw new NotImplementedException();
    }
}
