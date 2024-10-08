namespace Lagrange.Tsugu;

public class ParsedCommand {
    private readonly string[] _args;

    public string ConcatenatedArgs => string.Join(" ", _args);

    public string Alias { get; set; }

    public ParsedCommand(string[] args) {
        if (args.Length < 1) {
            throw new Exception("empty command");
        }

        Alias = args[0];
        _args = args.Skip(1).ToArray();
    }

    public bool HasArgument(int index) { return index >= 0 && index < _args.Length; }

    public string? GetString(int index) { return !HasArgument(index) ? null : _args[index]; }

    public int? GetInt32(int index) {
        string? v = GetString(index);

        if (v == null || !int.TryParse(v, out int i)) {
            return null;
        }

        return i;
    }

    public bool? GetBoolean(int index) {
        string? v = GetString(index);

        if (v == null || !bool.TryParse(v, out bool b)) {
            return null;
        }

        return b;
    }

    public TEnum? GetEnum<TEnum>(int index) where TEnum : struct, Enum {
        string? v = GetString(index);

        if (v == null || !Enum.TryParse(v, true, out TEnum e)) {
            return null;
        }

        return e;
    }
}
