using Microsoft.Extensions.Logging;

namespace Tsugu.Frontend.Logging;

public class FileLoggerProvider : ILoggerProvider {
    private readonly StreamWriter _logFileWriter;

    public FileLoggerProvider(StreamWriter logFileWriter) {
        _logFileWriter = logFileWriter ?? throw new ArgumentNullException(nameof(logFileWriter));
    }

    public ILogger CreateLogger(string categoryName) {
        return new FileLogger(categoryName, _logFileWriter);
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
        _logFileWriter.Dispose();
    }
}
