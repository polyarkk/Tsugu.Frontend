using Lagrange.Tsugu.Command;
using System.Reflection;

namespace Lagrange.Tsugu.Api;

public interface ICommand {
    /// <summary>
    /// 运行指令
    /// </summary>
    /// <param name="ctx">上下文</param>
    /// <param name="args">参数</param>
    Task Invoke(Context ctx, ParsedCommand args);
}
