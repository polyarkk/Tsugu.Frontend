using System.Text.Json.Serialization;

namespace Lagrange.Tsugu.Api;

/// <summary>
///     用户数据/车站 API 返回的结构有 status、data，其余 API 返回有 status、string
/// </summary>
[Serializable]
public class RestResponse {
    public RestResponse(string type, string @string) {
        Type = type;
        String = @string;
    }

    [JsonConstructor]
    public RestResponse() { }

    /// <summary>
    ///     <para>若为 Tsugu API 返回的数据，则表示数据类型（"string" | "base64"）</para>
    ///     <para>若为其他 API 返回的数据，该字段恒为 null</para>
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    ///     Tsugu API 返回的数据，非 Tsugu API 该字段恒为 null
    /// </summary>
    public string? String { get; set; }

    public bool IsImageBase64() { return string.Equals(Type, "base64", StringComparison.CurrentCultureIgnoreCase); }

    public override string ToString() { return $"[RestResponse Type=[{Type}] String=[{String}]"; }
}
