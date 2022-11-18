using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EMG.Extensions.Logging.Loggly
{
    public interface ILogglyClient
    {
        Task PublishAsync(LogglyMessage message);

        Task PublishManyAsync(IEnumerable<LogglyMessage> messages);
    }

    public class LogglyHttpClient : ILogglyClient
    {
        private readonly HttpClient _http;
        private readonly LogglyOptions _options;

        public LogglyHttpClient(HttpClient http, LogglyOptions options)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task PublishAsync(LogglyMessage message)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, $"/inputs/{_options.ApiKey}/tag/{_options.Environment}"))
                {
                    var content = JsonConvert.SerializeObject(FixData(message), SerializerSettings);
                    request.Content = new StringContent(content, _options.ContentEncoding, "application/json");

                    using (var response = await _http.SendAsync(request).ConfigureAwait(false))
                    {
                        if (!response.IsSuccessStatusCode)
                        {

                        }
                    }
                }
            }
            catch (Exception) when (_options.SuppressExceptions)
            {

            }
        }

        public async Task PublishManyAsync(IEnumerable<LogglyMessage> messages)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, $"/bulk/{_options.ApiKey}/tag/bulk"))
                {
                    var tags = new List<string>();

                    if (request.Headers.TryGetValues("X-LOGGLY-TAG", out var defaultTags))
                    {
                        tags.AddRange(defaultTags);
                    }

                    tags.Add(_options.Environment);
                    request.Headers.Add("X-LOGGLY-TAG", tags);

                    var fixedMessages = string.Join("\n", messages.Select(FixData).Select(s => JsonConvert.SerializeObject(s, SerializerSettings)));
                    request.Content = new StringContent(fixedMessages, _options.ContentEncoding, "application/json");

                    using (var response = await _http.SendAsync(request).ConfigureAwait(false))
                    {
                        if (!response.IsSuccessStatusCode)
                        {

                        }
                    }
                }
            }
            catch (Exception) when (_options.SuppressExceptions)
            {

            }
        }

        private static LogglyMessage FixData(LogglyMessage message)
        {
            if (message.Data is IEnumerable<KeyValuePair<string, object>> values)
            {
                var newData = values.ToDictionary(k => k.Key, v => v.Value);
                return CloneLogglyEvent(message, newData);
            }

            return message;
        }

        private static LogglyMessage CloneLogglyEvent(LogglyMessage logglyMessage, object newData)
        {
            return new LogglyMessage
            {
                ApplicationName = logglyMessage.ApplicationName,
                Category = logglyMessage.Category,
                Data = newData,
                Error = logglyMessage.Error,
                EventId = logglyMessage.EventId,
                Level = logglyMessage.Level,
                MachineName = logglyMessage.MachineName,
                Message = logglyMessage.Message
            };
        }

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
	        Converters = { new FormattedIdConverter() }
        };
    }

    private class FormattedIdConverter : JsonConverter
    {
        private static HashSet<Type> IdNumericTypes = new HashSet<Type>
        {
            typeof(short), typeof(int), typeof(long), typeof(byte),
            typeof(ushort), typeof(uint), typeof(ulong), typeof(sbyte),
        };

        internal static bool IsIdNumericType(Type type)
        {		
            return IdNumericTypes.Contains(type) ||
                IdNumericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        public override bool CanConvert(Type objectType)
        {
            return IsIdNumericType(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {		
            if(writer.Path?.EndsWith("Id", StringComparison.OrdinalIgnoreCase) == true)
                writer.WriteValue(Convert.ToString(value));
            else
                writer.WriteValue(value);
        }

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}