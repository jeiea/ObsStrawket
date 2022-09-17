using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket.Test.Utilities {
  internal class MemoryLogger : ILogger {
    public LogLevel LogLevel { get; set; } = LogLevel.Trace;

    [ThreadStatic]
    private static StringBuilder? _builder;

    private readonly IExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();
    private readonly Channel<string> _messages = Channel.CreateUnbounded<string>();

    public IDisposable BeginScope<TState>(TState state) where TState : notnull {
      return _scopeProvider.Push(state);
    }

    public bool IsEnabled(LogLevel logLevel) {
      return LogLevel <= logLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
      if (!IsEnabled(logLevel)) {
        return;
      }

      var builder = _builder ??= new();
      builder.Append(logLevel switch {
        LogLevel.Trace => "[T] ",
        LogLevel.Debug => "[D] ",
        LogLevel.Information => "[I] ",
        LogLevel.Warning => "[W] ",
        LogLevel.Error => "[E] ",
        LogLevel.Critical => "[C] ",
        _ => "[?] ",
      });
      _scopeProvider.ForEachScope((o, s) => {
        builder.Append(o);
        builder.Append("| ");
      }, "");
      builder.Append(formatter(state, exception));
      _ = _messages.Writer.WriteAsync(builder.ToString());
      builder.Clear();
    }

    public async Task<string> GetAllAsync() {
      _messages.Writer.TryComplete();

      var builder = new StringBuilder();
      await foreach (string message in _messages.Reader.ReadAllAsync()) {
        builder.AppendLine(message);
      }
      return builder.ToString();
    }
  }
}
