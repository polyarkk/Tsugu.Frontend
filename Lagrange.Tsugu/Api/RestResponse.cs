using System.Text.Json.Serialization;

namespace Lagrange.Tsugu.Api;

[Serializable]
public class RestResponse {
    public RestResponse(string type, string @string) {
        Type = type;
        String = @string;
    }

    [JsonConstructor]
    public RestResponse() { }

    /// <summary>
    /// "string" | "base64"
    /// </summary>
    public string? Type { get; set; }

    public string? String { get; set; }

    public bool IsImageBase64() { return string.Equals(Type, "base64", StringComparison.CurrentCultureIgnoreCase); }

    public override string ToString() { return $"[RestResponse Type=[{Type}] String=[{String}]"; }
}
