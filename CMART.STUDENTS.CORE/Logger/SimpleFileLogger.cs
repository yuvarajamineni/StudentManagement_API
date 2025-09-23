﻿using Microsoft.Extensions.Logging;

public class SimpleFileLogger : ILogger
{
    private readonly string _filePath;
    private readonly string _categoryName;
    private static readonly object _lock = new();

    public SimpleFileLogger(string categoryName, string filePath)
    {
        _categoryName = categoryName;
        _filePath = filePath;
    }

    public IDisposable BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel logLevel) =>
        logLevel >= LogLevel.Information;

    public void Log<TState>(LogLevel logLevel, EventId eventId,
        TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = $"{DateTime.UtcNow:u} [{logLevel}] {_categoryName}: {formatter(state, exception)}";
        if (exception != null)
            message += Environment.NewLine + exception;

        lock (_lock)
        {
            File.AppendAllText(_filePath, message + Environment.NewLine);
        }
    }
}