namespace Lagrange.Tsugu.Api;

public interface ICommand {
    string Name { get; set; }
    string Alias { get; set; }
    string HelpHint { get; set; }

    /// <summary>
    ///     运行指令
    /// </summary>
    /// <param name="ctx">上下文</param>
    /// <param name="args">参数，第 0 个参数为指令名本身</param>
    Task Invoke(Context ctx, ParsedCommand args);
}
