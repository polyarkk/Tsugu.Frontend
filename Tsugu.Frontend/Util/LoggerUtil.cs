using Microsoft.Extensions.Logging;

namespace Tsugu.Frontend.Util;

public static class LoggerUtil {
    private static ILoggerFactory LoggerFactory { get; set; } = null!;

    public static void InitLoggerFactory(ILoggerFactory loggerFactory) {
        LoggerFactory = loggerFactory;
    }

    public static ILogger<T> GetLogger<T>() {
        return LoggerFactory.CreateLogger<T>();
    }
}
