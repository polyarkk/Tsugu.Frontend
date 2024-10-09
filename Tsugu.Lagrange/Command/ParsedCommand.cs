namespace Tsugu.Lagrange.Command;

public class ParsedCommand {
    private readonly string[] _args;

    public string ConcatenatedArgs => string.Join(" ", _args);

    public string Alias { get; set; }

    public string? this[int index] => HasArgument(index) ? _args[index] : null;

    public string[] this[Range range] => _args[range];

    public ParsedCommand(string[] args) {
        if (args.Length < 1) {
            throw new Exception("empty command");
        }

        Alias = args[0];
        _args = args.Skip(1).ToArray();
    }

    public bool HasArgument(int index) { return index >= 0 && index < _args.Length; }

    public string? GetString(int index) { return !HasArgument(index) ? null : _args[index]; }

    public int Length => _args.Length;

    public uint? GetUInt32(int index) {
        string? v = GetString(index);

        if (v == null) {
            return null;
        }

        if (!uint.TryParse(v, out uint i)) {
            throw new CommandParseException($"参数{index + 1}类型错误，需要整数！");
        }

        return i;
    }

    public bool? GetBoolean(int index) {
        string? v = GetString(index);

        if (v == null) {
            return null;
        }

        if (!bool.TryParse(v, out bool b)) {
            throw new CommandParseException($"参数{index + 1}类型错误，需要布尔值！（true|false）");
        }

        return b;
    }

    public TEnum? GetEnum<TEnum>(int index) where TEnum : struct, Enum {
        string? v = GetString(index);

        if (v == null) {
            return null;
        }

        if (!Enum.TryParse(v, true, out TEnum e)) {
            throw new CommandParseException(
                $"参数{index + 1}类型错误，需要枚举{typeof(TEnum).Name}！" +
                $"（{string.Join("|", Enum.GetValues(typeof(TEnum)).Cast<object>().ToArray())}）"
            );
        }

        return e;
    }
}
