using System.Collections;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EMG.Extensions.Logging.Loggly
{
    public class LogglyMessage
    {
        public string MachineName { get; set; }

        public string ApplicationName { get; set; }

        public string Category { get; set; }

        public EventId EventId { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel Level { get; set; }

        public Error Error { get; set; }
    }

    public class Error
    {
        public string Source { get; set; }

        public string Type { get; set; }

        public string Message { get; set; }

        public string InnerError { get; set; }

        public string StackTrace { get; set; }

        public IDictionary Data { get; set; }
    }
}