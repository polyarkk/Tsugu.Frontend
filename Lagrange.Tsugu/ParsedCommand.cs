namespace Lagrange.Tsugu;

public class ParsedCommand {
    private readonly string[] _args;

    private string _command;

    public ParsedCommand(string[] args) {
        if (args.Length < 1) {
            throw new Exception("empty command");
        }

        _command = args[0];
        _args = args.Skip(1).ToArray();
    }

    public bool HasArgument(int index) { return index >= 0 && index < _args.Length; }

    public string? GetString(int index) { return !HasArgument(index) ? null : _args[index]; }

    public int? GetInt32(int index) {
        string? v = GetString(index);

        if (v == null) {
            return null;
        }

        return Convert.ToInt32(v);
    }

    public bool? GetBoolean(int index) {
        string? v = GetString(index);

        if (v == null) {
            return null;
        }

        return Convert.ToBoolean(v);
    }

    public TEnum? GetEnum<TEnum>(int index) where TEnum : struct, Enum {
        string? v = GetString(index);

        if (v == null) {
            return null;
        }

        return Enum.Parse<TEnum>(v, true);
    }
}
