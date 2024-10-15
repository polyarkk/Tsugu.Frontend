using Tsugu.Api.Enum;

namespace Tsugu.Frontend.Command.Argument;

public static class ArgumentMatchers {
    public static object? ToServerEnumMatcher(string? str) {
        return str switch {
            "日服" => Server.Jp,
            "国际服" => Server.En,
            "台服" => Server.Tw,
            "繁中服" => Server.Tw,
            "国服" => Server.Cn,
            "简中服" => Server.Cn,
            "韩服" => Server.Kr,
            null => null,
            _ => System.Enum.Parse<Server>(str, true),
        };
    }

    public static object? ToChartDifficultyEnumMatcher(string? str) {
        return str switch {
            "ez" => ChartDifficulty.Easy,
            "nm" => ChartDifficulty.Normal,
            "hd" => ChartDifficulty.Hard,
            "ex" => ChartDifficulty.Expert,
            "sp" => ChartDifficulty.Special,
            null => null,
            _ => System.Enum.Parse<ChartDifficulty>(str, true),
        };
    }
}
