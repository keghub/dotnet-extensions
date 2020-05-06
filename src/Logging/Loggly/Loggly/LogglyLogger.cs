using System;
using Microsoft.Extensions.Logging;

namespace EMG.Extensions.Logging.Loggly
{
    public class LogglyLogger : ILogger
    {
        private readonly ILogglyProcessor _processor;
        private readonly LogglyOptions _options;
        
        public LogglyLogger(string name, ILogglyProcessor processor, LogglyOptions options)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (Name == typeof(LogglyHttpClient).FullName)
            {
                return;
            }

            if (!_options.Filter(Name, eventId, logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (message == null)
            {
                return;
            }

            var logglyEvent = new LogglyMessage
            {
                ApplicationName = _options.ApplicationName,
                MachineName = Environment.MachineName,
                Category = Name,
                Data = state,
                Event = eventId,
                Message = message,
                Level = logLevel,
                Error = FormatException(exception)
            };

            _options.PreProcessMessage?.Invoke(logglyEvent);

            _processor.EnqueueMessage(logglyEvent);

        }

        private static Error FormatException(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            var error = new Error
            {
                Source = exception.Source,
                Type = exception.GetType().ToString(),
                Message = exception.Message,
                Data = exception.Data,
                InnerError = exception.InnerException?.Message,
                StackTrace = exception.StackTrace
            };

            return error;
        }

        public bool IsEnabled(LogLevel logLevel) => _options.Filter(Name, 0, logLevel);

        public IDisposable BeginScope<TState>(TState state) => NoopDisposable.Instance;

        private class NoopDisposable : IDisposable
        {
            public static readonly NoopDisposable Instance = new NoopDisposable();

            public void Dispose() { }
        }
    }
}