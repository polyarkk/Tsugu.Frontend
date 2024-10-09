using System.Text.Json.Serialization;

namespace Tsugu.Api.Entity;

public class VerifyCode {
    [JsonPropertyName("verifyCode")]
    public uint Code { get; set; }
}
