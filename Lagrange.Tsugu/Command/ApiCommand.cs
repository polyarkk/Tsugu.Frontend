namespace Lagrange.Tsugu.Command;

[AttributeUsage(AttributeTargets.Class)]
public class ApiCommand : Attribute {
    public required string Alias { get; set; }
    
    public required string Name { get; set; }
    
    public string? UsageHint { get; set; }
}
