using System.Text.Json.Serialization;

namespace Lagrange.Tsugu.Api.Rest;

public class ExternalRestResponse<TData> {
    public ExternalRestResponse(string status, TData data) {
        Status = status;
        Data = data;
    }

    [JsonConstructor]
    public ExternalRestResponse() { }

    public string? Status { get; set; }
    
    public TData? Data { get; set; }
}
