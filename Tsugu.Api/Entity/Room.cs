namespace Tsugu.Api.Entity;

public class Room {
    public Room(
        uint number, string rawMessage, string source,
        uint userId, ulong time, Player player, 
        string? avatarUrl, string? userName
    ) {
        Number = number;
        RawMessage = rawMessage;
        Source = source;
        UserId = userId;
        Time = time;
        Player = player;
        AvatarUrl = avatarUrl;
        UserName = userName;
    }

    public uint Number { get; set; }

    public string RawMessage { get; set; }

    public string Source { get; set; }

    public uint UserId { get; set; }

    public ulong Time { get; set; }

    public Player Player { get; set; }

    public string? AvatarUrl { get; set; }

    public string? UserName { get; set; }
}
