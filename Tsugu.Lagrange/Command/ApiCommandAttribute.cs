namespace Tsugu.Lagrange.Command;

[AttributeUsage(AttributeTargets.Class)]
public class ApiCommandAttribute : Attribute {
    public required string[] Aliases { get; set; }
    
    public required string Description { get; set; }
    
    public string? UsageHint { get; set; }
}
