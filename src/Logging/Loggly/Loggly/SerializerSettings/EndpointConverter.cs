using Newtonsoft.Json;
using System;

namespace EMG.Extensions.Logging.Loggly.SerializerSettings;

public class EndpointConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(System.Net.IPEndPoint);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is System.Net.IPEndPoint endpoint)
        {
            writer.WriteValue(endpoint.ToString());
        }
        else
        {
            writer.WriteValue(value);
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}