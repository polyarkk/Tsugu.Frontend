using Tsugu.Api.Enum;

namespace Tsugu.Api.Entity;

public class Player {
    public uint Id { get; set; }
    
    public Server MainServer { get; set; }
}
