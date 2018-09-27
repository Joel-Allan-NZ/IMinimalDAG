using IMinimalDAGInterfaces;
using Newtonsoft.Json;
using System;

namespace MinimalDAGImplementations.Serializer
{
    public class MinimalDAGNodeConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IMinimalDAGNode<T>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(MinimalDAGNode<T>));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
