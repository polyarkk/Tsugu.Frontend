﻿using Lagrange.Tsugu.Api.Enum;
using Lagrange.Tsugu.Api.Rest;
using Lagrange.Tsugu.Command;

namespace Lagrange.Tsugu.Api.Endpoint;

[ApiCommand(
    Alias = "lsycx",
    Description = "查询与指定活动相关的指定档位的历史预测线",
    UsageHint = "<档位> [活动ID] [cn|jp|tw|kr|en]"
)]
public class CutoffListOfRecentEvent : BaseCommand {
    public async override Task Invoke(Context ctx, ParsedCommand args) {
        if (!args.HasArgument(0)) {
            await ctx.SendPlainText(GetErrorAndHelpText("未指定档位！"));

            return;
        }

        Dictionary<string, object?> p = new() {
            ["tier"] = args.GetInt32(0)
        };

        if (args.HasArgument(1)) {
            p["eventId"] = args.GetBoolean(1);
        }

        p["mainServer"] = args.GetEnum<BandoriServer>(2) ?? BandoriServer.Cn;

        p["compress"] = ctx.Settings.Compress;

        using SugaredHttpClient rest = ctx.Rest;

        RestResponse response = (await rest.TsuguPost("/cutoffListOfRecentEvent", p))[0];

        if (response.IsImageBase64()) {
            await ctx.SendImage(response.String!);
        }
    }
}