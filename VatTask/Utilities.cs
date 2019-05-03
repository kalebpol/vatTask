using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VatTask
{
    public static class Utilities
    {
        // handle zero date
        private class MyDateConverter : IsoDateTimeConverter
        {
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value != null && reader.Value.ToString().StartsWith("0000")) 
                    return DateTime.MinValue;
                else 
                    return base.ReadJson(reader, objectType, existingValue, serializer);
            }
        }
        //--------------------------------------------------------------------//

        // generic json deserialization from stream.
        public static T Deserialize<T>(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Converters.Add(new MyDateConverter());
                return ser.Deserialize<T>(jsonReader);
            }
        }
        //--------------------------------------------------------------------//
    }
}
