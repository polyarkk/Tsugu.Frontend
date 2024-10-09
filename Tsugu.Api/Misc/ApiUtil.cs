using System.Text.Json;

namespace Tsugu.Api.Misc;

public static class ApiUtil {
    private readonly static JsonSerializerOptions JsonConfig = new() {
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
