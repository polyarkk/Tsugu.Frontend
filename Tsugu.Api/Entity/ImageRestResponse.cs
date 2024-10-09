using System.Text.Json.Serialization;

namespace Tsugu.Api.Entity;

[Serializable]
public class ImageRestResponse {
    public ImageRestResponse(string type, string @string) {
        Type = type;
        String = @string;
    }

    [JsonConstructor]
    public ImageRestResponse() { }

    /// <summary>
    /// "string" | "base64"
    /// </summary>
    public string? Type { get; set; }

    public string? String { get; set; }

    public bool IsImageBase64 => string.Equals(Type, "base64", StringComparison.CurrentCultureIgnoreCase);

    public override string ToString() { return $"[RestResponse Type=[{Type}] String=[{String}]"; }
}
