namespace Tsugu.Lagrange.Command;

[AttributeUsage(AttributeTargets.Class)]
public class ApiCommandAttribute : Attribute {
    /// <summary>
    /// 指定调用方式
    /// </summary>
    public required string[] Aliases { get; set; }

    /// <summary>
    /// 指令描述
    /// </summary>
    public string Description { get; set; } = "没有描述";

    /// <summary>
    /// 使用示例
    /// </summary>
    public string Example { get; set; } = string.Empty;
}
