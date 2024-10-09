using System.Reflection;

namespace Tsugu.Lagrange.Command;

public abstract class BaseCommand {
    public abstract Task Invoke(Context ctx, ParsedCommand args);

    public ApiCommandAttribute GetAttribute() {
        return GetType().GetCustomAttribute<ApiCommandAttribute>()!;
    }

    public string GetErrorAndHelpText(string error) {
        ApiCommandAttribute attr = GetAttribute();

        return $"""
                错误：{error}
                {string.Join("|", attr.Aliases)} {attr.UsageHint}
                {attr.Description}
                """;
    }
}
