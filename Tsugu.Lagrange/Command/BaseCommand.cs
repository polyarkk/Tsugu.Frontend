using System.Reflection;
using System.Text;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command;

public abstract class BaseCommand {
    protected abstract Task Invoke(Context ctx, ParsedCommand args);

    /// <summary>
    /// 参数，注意可选参数一定要在必需参数之后，否则会出现异常
    /// </summary>
    protected virtual ArgumentMeta[] Arguments { get; } = [];
    
    public async Task InvokePre(Context ctx, ParsedCommand args) {
        // validate fields
        for (int i = 0; i < Arguments.Length; i++) {
            (string name, Type? type, bool optional) = Arguments[i];

            string? arg = args[i];

            if (!optional && arg == null) {
                await ctx.SendPlainText(GetErrorAndHelpText($"未提供关键参数 [{name}]！"));

                return;
            }

            try {
                _ = ConvertUtil.To(type, arg);
            } catch (Exception) {
                if (type.IsEnum) {
                    string candidates = string.Join("|", Enum.GetNames(type).Select(n => n.ToLower()));

                    await ctx.SendPlainText(GetErrorAndHelpText($"参数 [{name}] 非法，需要 {type.Name}！({candidates})"));
                } else {
                    await ctx.SendPlainText(GetErrorAndHelpText($"参数 [{name}] 非法，需要 {type.Name}！"));
                }

                return;
            }
        }

        await Invoke(ctx, args);
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
        StringBuilder argumentBuilder = new StringBuilder();

        foreach ((string name, Type type, bool optional) in Arguments) {
            char pre = optional ? '[' : '<';
            char suf = optional ? ']' : '>';
            argumentBuilder.Append($"{pre}{name}: {type.Name}{suf} ");
        }

        argumentBuilder.Remove(argumentBuilder.Length - 1, 1);

        return argumentBuilder.ToString();
    }

    public string GetHelpText() {
        ApiCommandAttribute attr = GetAttribute();

        StringBuilder sb = new StringBuilder();

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

    protected static ArgumentMeta Argument<T>(string name) {
        return (name, typeof(T), false);
    }

    protected static ArgumentMeta OptionalArgument<T>(string name) {
        return (name, typeof(T), true);
    }
}
