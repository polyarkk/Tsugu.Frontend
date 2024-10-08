using Lagrange.Tsugu.Command;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(
    Alias = "查卡",
    Description = """
                  查询指定卡牌的信息，或查询符合条件的卡牌列表
                  查卡 1399：返回1399号卡牌的信息
                  查卡 红 ars 4x：返回角色 ars 的 4x 卡片的信息
                  """,
    UsageHint = "<关键词>"
)]
public class SearchCard : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        string arg = args.ConcatenatedArgs;

        if (string.IsNullOrWhiteSpace(arg)) {
            await ctx.SendPlainText($"错误：未指定查询关键词！用法：{GetAttribute().Alias} {GetAttribute().UsageHint}");

            return;
        }

        var p = new Dictionary<string, object> {
            ["displayedServerList"] = new[] { 0, 3 },
            ["useEasyBG"] = true
        };

        using SugaredHttpClient rest = ctx.Rest;

        if (int.TryParse(arg, out _)) {
            p["text"] = arg;
        } else {
            // listed fuzzy search
            var fuzzySearchResult =
                await rest.ExternalPost<Dictionary<string, object>>("/fuzzySearch",
                    new Dictionary<string, object> {
                        ["text"] = arg
                    }
                );

            p["fuzzySearchResult"] = fuzzySearchResult.Data!;
        }

        RestResponse response = await rest.TsuguPost("/searchCard", p);

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        } else {
            await ctx.SendPlainText("错误：" + response.String! + "！");
        }
    }
}
