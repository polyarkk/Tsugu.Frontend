using Microsoft.Extensions.Logging;
using Tsugu.Frontend.Util;

namespace Tsugu.Frontend.Command.Argument;

public class ArgumentMeta {
    private readonly static ILogger<ArgumentMeta> Logger = LoggerUtil.GetLogger<ArgumentMeta>();
    
    public ArgumentMeta(string key, string name, Type type, bool optional) {
        Key = key;
        Name = name;
        Type = type;
        Optional = optional;
        _matcher = s => ConvertUtil.To(Type, s);
    }

    /// <summary>
    /// 参数标识符
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 对用户显示的名字
    /// </summary>
    public string Name { get; }

    public Type Type { get; }

    public bool Optional { get; private set; }

    private Func<string?, object?> _matcher;

    public ArgumentMeta AsOptional() {
        Optional = true;

        return this;
    }

    /// <summary>
    /// 设置将字符串参数转换为特定类型的方法
    /// </summary>
    /// <param name="fn">方法</param>
    public ArgumentMeta WithMatcher(Func<string?, object?> fn) {
        _matcher = fn;

        return this;
    }

    public object? InvokeMatcher(string? arg) {
        object? o = _matcher.Invoke(arg);

        if (o != null && Type != o.GetType()) {
            Logger.LogWarning("matched type mismatch! expected {expected}, got {got}", Type.Name, o.GetType().Name);
        }
        
        return o;
    }
}
