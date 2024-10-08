using System.Reflection;

namespace Lagrange.Tsugu.Command;

public abstract class BaseCommand {
    public abstract Task Invoke(Context ctx, ParsedCommand args);

    public ApiCommand GetAttribute() {
        return GetType().GetCustomAttribute<ApiCommand>()!;
    }

    public string GetErrorAndHelpText(string error) {
        ApiCommand attr = GetAttribute();

        return $"""
                错误：{error}
                {attr.Alias} {attr.UsageHint}
                {attr.Description}
                """;
    }
}
