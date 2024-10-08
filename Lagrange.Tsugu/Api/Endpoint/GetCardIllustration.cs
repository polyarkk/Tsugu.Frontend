using Lagrange.Tsugu.Command;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(Alias = "查卡面", Description = "获取卡面图片", UsageHint = "<卡面ID>")]
public class GetCardIllustration : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        int? cardId = args.GetInt32(0);

        if (cardId == null) {
            await ctx.SendPlainText($"错误：未指定卡面 ID！用法：{GetAttribute().Alias} {GetAttribute().UsageHint}");

            return;
        }

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = await rest.TsuguPost(
            "/getCardIllustration",
            new Dictionary<string, object?> { ["cardId"] = cardId }
        );

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText("错误：卡面不存在！");
        }
    }
}
