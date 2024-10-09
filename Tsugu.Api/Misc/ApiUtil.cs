using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tsugu.Api.Entity;

namespace Tsugu.Api.Misc;

public static class ApiUtil {
    public static string BaseAddress { get; set; } = "http://tsugubot.com:8080";

    public static HttpClient CreateHttpClient() {
        return new HttpClient {
            BaseAddress = new Uri(BaseAddress),
        };
    }

    public readonly static JsonSerializerOptions JsonConfig = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static T? DeserializeJson<T>(this string json) { return JsonSerializer.Deserialize<T>(json, JsonConfig); }

    public static T? DeserializeJson<T>(this JsonElement json) { return json.Deserialize<T>(JsonConfig); }

    public static string SerializeJson<T>(this T obj) { return JsonSerializer.Serialize(obj, JsonConfig); }

    public static long ToUnixTimeMilliseconds(this DateTime dateTime) {
        DateTimeOffset offset = dateTime.ToUniversalTime();

        return offset.ToUnixTimeMilliseconds();
    }
}
