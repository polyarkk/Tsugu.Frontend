namespace Tsugu.Lagrange.Command;

[AttributeUsage(AttributeTargets.Class)]
public class ApiCommand : Attribute {
    public required string Alias { get; set; }
    
    public required string Description { get; set; }
    
    public string? UsageHint { get; set; }
}
