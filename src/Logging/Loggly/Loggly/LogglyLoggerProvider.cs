using System;
using Microsoft.Extensions.Logging;

namespace EMG.Extensions.Logging.Loggly
{
    public class LogglyLoggerProvider : ILoggerProvider
    {
        private readonly ILogglyProcessor _processor;
        private readonly LogglyOptions _options;

        public LogglyLoggerProvider(ILogglyProcessor processor, LogglyOptions options)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Dispose()
        {
            _processor.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new LogglyLogger(categoryName, _processor, _options);
        }
    }
}
