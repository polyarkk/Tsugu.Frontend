namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(Aliases = ["抽卡模拟", "卡池模拟"], Description = "模拟抽卡", UsageHint = "[次数] [卡池ID]")]
public class GachaSimulate : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        
    }
}
