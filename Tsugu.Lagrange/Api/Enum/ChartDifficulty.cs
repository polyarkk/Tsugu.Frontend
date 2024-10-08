namespace Tsugu.Lagrange.Api.Enum;

public enum ChartDifficulty {
    Easy,

    Normal,

    Hard,

    Expert,

    Special
}

public static class ChartDifficultyExtension {
    public static ChartDifficulty? ToChartDifficulty(this string str) {
        if (System.Enum.TryParse(str, true, out ChartDifficulty e)) {
            return e;
        }

        return str.ToLower() switch {
            "ez" => ChartDifficulty.Easy,
            "nm" => ChartDifficulty.Normal,
            "hd" => ChartDifficulty.Hard,
            "ex" => ChartDifficulty.Expert,
            "sp" => ChartDifficulty.Special,
            _ => null
        };
    }
}
