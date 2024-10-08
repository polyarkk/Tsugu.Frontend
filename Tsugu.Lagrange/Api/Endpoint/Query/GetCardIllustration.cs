using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Message;
using Tsugu.Lagrange.Api.Rest;
using Tsugu.Lagrange.Command;

namespace Tsugu.Lagrange.Api.Endpoint.Query;

[ApiCommand(Aliases = ["查卡面", "查插画"], Description = "获取卡面图片", UsageHint = "<卡面ID>")]
public class GetCardIllustration : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        int? cardId = args.GetInt32(0);

        if (cardId == null) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定卡面ID！"));

            return;
        }

        using SugaredHttpClient rest = ctx.Rest;

        List<RestResponse> responses = await rest.TsuguPost(
            "/getCardIllustration",
            new Dictionary<string, object?> { ["cardId"] = cardId }
        );

        if (responses.Count == 1 && !responses[0].IsImageBase64()) {
            await ctx.SendPlainText("错误：卡面不存在！");
            
            return;
        }

        MessageBuilder messageBuilder = Util.GetDefaultMessageBuilder(ctx);

        foreach (RestResponse response in responses.Where(response => response.IsImageBase64())) {
            messageBuilder.Image(Convert.FromBase64String(response.String!));
        }

        await ctx.Bot.SendMessage(messageBuilder.Build());
    }
}
