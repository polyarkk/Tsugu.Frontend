using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Message;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(
    Aliases = ["查卡面", "查插画"],
    Description = "根据卡面ID查询卡面插画"
)]
public class GetCardIllustration : BaseCommand {
    protected override ArgumentMeta[] Arguments { get; } = [
        Argument<uint>("卡面ID"),
    ];

    protected async override Task Invoke(Context ctx, ParsedCommand args) {
        string[] base64List = await ctx.Tsugu.Query.GetCardIllustration((uint)args.GetUInt32(0)!);

        MessageBuilder messageBuilder = Util.MessageUtil.GetDefaultMessageBuilder(ctx);

        foreach (string base64 in base64List) {
            messageBuilder.Image(Convert.FromBase64String(base64));
        }

        await ctx.Bot.SendMessage(messageBuilder.Build());
    }
}
