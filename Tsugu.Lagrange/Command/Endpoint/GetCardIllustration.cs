using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Message;

namespace Tsugu.Lagrange.Command.Endpoint;

[ApiCommand(Aliases = ["查卡面", "查插画"], Description = "获取卡面图片", UsageHint = "<卡面ID>")]
public class GetCardIllustration : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        uint? cardId = args.GetUInt32(0);

        if (cardId == null) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定卡面ID！"));

            return;
        }

        string[] base64List = await ctx.Tsugu.Query.GetCardIllustration((uint)cardId);

        MessageBuilder messageBuilder = Util.GetDefaultMessageBuilder(ctx);

        foreach (string base64 in base64List) {
            messageBuilder.Image(Convert.FromBase64String(base64));
        }

        await ctx.Bot.SendMessage(messageBuilder.Build());
    }
}
