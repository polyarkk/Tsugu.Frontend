using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tsugu.Api.Entity;

public class StationRestResponse {
    public StationRestResponse(string status, JsonElement data) {
        Status = status;
        Data = data;
    }

    [JsonConstructor]
    public StationRestResponse() { }

    public string? Status { get; set; }
    
    public JsonElement Data { get; set; }

    public bool IsSuccess => !string.IsNullOrWhiteSpace(Status) && Status != "failed";
}
