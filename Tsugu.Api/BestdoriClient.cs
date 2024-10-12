using System.Text.Json;
using Tsugu.Api.Enum;
using Tsugu.Api.Util;

namespace Tsugu.Api;

// actually i don't wanna write whole completed api library for bestdori...
public class BestdoriClient : IDisposable {
    private readonly HttpClient _httpClient = new() {
        BaseAddress = new Uri("https://bestdori.com"),
    };

    public async Task<bool> IsValidPlayer(uint playerId, Server server) {
        JsonElement element = await Get($"/player/{server.ToString().ToLower()}/{playerId}?mode=2")
            ?? throw new NullReferenceException();

        return element.GetProperty("data").GetProperty("profile").ValueKind != JsonValueKind.Null;
    }

    private async Task<JsonElement?> Get(string endpoint) {
        HttpRequestMessage msg = new(HttpMethod.Get, $"/api{endpoint}");

        HttpResponseMessage response = await _httpClient.SendAsync(msg);

        string content = await response.Content.ReadAsStringAsync();

        return content.DeserializeJson<JsonElement>();
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
        _httpClient.Dispose();
    }
}
