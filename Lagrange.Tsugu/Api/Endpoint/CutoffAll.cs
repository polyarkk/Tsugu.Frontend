using Lagrange.Tsugu.Command;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(
    Alias = "ycxall",
    Name = "查询全档位预测线",
    UsageHint = "[mainServer] [eventId] [compress]"
)]
public class CutoffAll : ICommand {
    public string Name { get; set; } = "查询全档位预测线";
    public string Alias { get; set; } = "ycxall";
    public string HelpHint { get; set; } = "[mainServer] [eventId] [compress]";

    public async Task Invoke(Context ctx, ParsedCommand args) {
        Dictionary<string, object?> p = new();

        p["mainServer"] = args.GetEnum<BandoriServer>(0) ?? BandoriServer.Cn;

        if (args.HasArgument(1)) {
            p["eventId"] = args.GetInt32(1);
        }

        if (args.HasArgument(2)) {
            p["compress"] = args.GetBoolean(2);
        }

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = await rest.Post("/cutoffAll", p);

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        }
    }
}
