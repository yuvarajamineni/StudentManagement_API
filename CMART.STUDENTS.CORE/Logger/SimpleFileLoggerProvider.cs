using Microsoft.Extensions.Logging;

public class SimpleFileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;

    public SimpleFileLoggerProvider(string filePath)
    {
        _filePath = filePath;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new SimpleFileLogger(categoryName, _filePath);
    }

    public void Dispose()
    {
    }
}