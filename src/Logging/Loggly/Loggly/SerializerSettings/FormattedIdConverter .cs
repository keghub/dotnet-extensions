using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EMG.Extensions.Logging.Loggly.SerializerSettings;

public class FormattedIdConverter : JsonConverter
{
    private readonly HashSet<Type> IdNumericTypes = new HashSet<Type>
    {
        typeof(byte), typeof(short), typeof(int), typeof(long),
        typeof(sbyte), typeof(ushort), typeof(uint), typeof(ulong),
    };

    public override bool CanConvert(Type objectType)
    {
        return IdNumericTypes.Contains(objectType);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (writer.Path.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
            writer.WriteValue(Convert.ToString(value));
        else
            writer.WriteValue(value);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}