using Lagrange.Tsugu.Command;
using System.Reflection;

namespace Lagrange.Tsugu.Api;

public abstract class BaseCommand : ICommand {
    public abstract Task Invoke(Context ctx, ParsedCommand args);

    private ApiCommand GetAttribute() {
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
