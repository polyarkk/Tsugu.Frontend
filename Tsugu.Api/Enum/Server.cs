namespace Tsugu.Api.Enum;

public enum Server {
    Jp,

    En,

    Tw,

    Cn,

    Kr,
}

public static class ServerExtension {
    public static string ToChineseString(this Server server) {
        return server switch {
            Server.Jp => "日服",
            Server.En => "国际服",
            Server.Tw => "台服",
            Server.Cn => "国服",
            Server.Kr => "韩服",
            _ => throw new ArgumentOutOfRangeException(nameof(server), server, null),
        };
    }
}
