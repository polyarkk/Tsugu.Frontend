using System.Reflection;
using System.Text;
using Tsugu.Frontend.Command.Argument;
using Tsugu.Frontend.Context;

namespace Tsugu.Frontend.Command;

public abstract class BaseCommand {
    protected abstract Task InvokeInternal(TsuguContext ctx, ParsedArgs args);

    /// <summary>
    /// 参数，注意可选参数一定要在必需参数之后，否则会出现异常
    /// </summary>
    protected virtual ArgumentMeta[] Arguments { get; } = [];
    
    public async Task Invoke(TsuguContext ctx, IReadOnlyList<string> tokens) {
        await InvokeInternal(ctx, new ParsedArgs(Arguments, tokens));
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

        foreach (ArgumentMeta meta in Arguments) {
            char pre = meta.Optional ? '[' : '<';
            char suf = meta.Optional ? ']' : '>';
            argumentBuilder.Append($"{pre}{meta.Name}: {meta.Type.Name}{suf} ");
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
        return new ArgumentMeta(key, name, typeof(T), false);
    }
}
