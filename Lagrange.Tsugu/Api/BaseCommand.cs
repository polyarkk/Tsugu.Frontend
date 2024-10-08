using Lagrange.Tsugu.Command;
using System.Reflection;

namespace Lagrange.Tsugu.Api;

public abstract class BaseCommand : ICommand {
    public abstract Task Invoke(Context ctx, ParsedCommand args);

    public ApiCommand GetAttribute() {
        return GetType().GetCustomAttribute<ApiCommand>()!;
    }
}
