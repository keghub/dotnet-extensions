using System;
using System.Collections.Generic;
using System.Text;
using EMG.Extensions.Logging.Loggly.SerializerSettings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EMG.Extensions.Logging.Loggly
{
    public class LogglyOptions
    {
        public string ApiKey { get; set; }

        public string Environment { get; set; } = "Development";

        public string ApplicationName { get; set; }

        public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();

        public bool SuppressExceptions { get; set; }

        public FilterDelegate Filter { get; set; } = (name, eventId, level) => level >= LogLevel.Information;

        public string LogglyHost { get; set; } = "logs-01.loggly.com";

        public string LogglyScheme { get; set; } = "https";

        public Action<LogglyMessage> PreProcessMessage { get; set; }

        public Encoding ContentEncoding { get; set; } = Encoding.UTF8;

        public TimeSpan Buffer { get; set; } = TimeSpan.FromMilliseconds(50);

        public JsonSerializerSettings SerializerSettings { get; set; } = JsonSettings.SerializerSettings;
    }

    public delegate bool FilterDelegate(string categoryName, EventId eventId, LogLevel logLevel);
}