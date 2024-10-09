using System.Reflection;
using System.Text;

namespace Tsugu.Lagrange.Command;

public abstract class BaseCommand {
    protected abstract Task Invoke(Context ctx, ParsedArgs args);

    /// <summary>
    /// 参数，注意可选参数一定要在必需参数之后，否则会出现异常
    /// </summary>
    protected virtual ArgumentMeta[] Arguments { get; } = [];
    
    public async Task InvokePre(Context ctx, string[] tokens) {
        await Invoke(ctx, new ParsedArgs(Arguments, tokens));
    }

    private ApiCommandAttribute GetAttribute() {
        return GetType().GetCustomAttribute<ApiCommandAttribute>()!;
    }

    public string GetErrorAndHelpText(string error) {
        return $"""
                错误：{error}
                {GetHelpText()}
                """;
    }

    private string GetArgumentHelpText() {
        StringBuilder argumentBuilder = new();

        foreach ((_, string name, Type type, bool optional) in Arguments) {
            char pre = optional ? '[' : '<';
            char suf = optional ? ']' : '>';
            argumentBuilder.Append($"{pre}{name}: {type.Name}{suf} ");
        }

        argumentBuilder.Remove(argumentBuilder.Length - 1, 1);

        return argumentBuilder.ToString();
    }

    public string GetHelpText() {
        ApiCommandAttribute attr = GetAttribute();

        StringBuilder sb = new();

        sb.Append(
            $"""
             {string.Join("|", attr.Aliases)} {GetArgumentHelpText()}
             {attr.Description}
             """
        );

        if (!string.IsNullOrWhiteSpace(attr.Example)) {
            sb.Append(
                $"""
                 
                 使用示例：
                 {attr.Example}
                 """
            );
        }

        return sb.ToString();
    }

    protected static ArgumentMeta Argument<T>(string key, string name) {
        return (key, name, typeof(T), false);
    }

    protected static ArgumentMeta OptionalArgument<T>(string key, string name) {
        return (key, name, typeof(T), true);
    }
}
