using IMinimalDAGInterfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Collections.Generic;

namespace MinimalDAGImplementations.Serializer
{
    public class MinimalDAGNodeConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MinimalDAGNode<T>) || objectType == typeof(IMinimalDAGNode<T>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            MinimalDAGNode<T> node = new MinimalDAGNode<T>(obj["Value"].ToObject<T>(), obj["ID"].ToObject<Guid>(), obj["Children"].ToObject<List<Guid>>());
            //node.Children = obj["Children"].ToObject<List<Guid>>();
            return node;


            //return serializer.Deserialize(reader, typeof(MinimalDAGNode<T>));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            MinimalDAGNode<T> node = (MinimalDAGNode<T>)value;
            //JArray array = new JArray();
            JObject obj = new JObject();
            obj.Add("ID", JToken.FromObject(node.ID));
            var childValueList = node.Children.Values.SelectMany(y => y.Select(x => x.ID)).ToList();
            obj.Add("Children", JToken.FromObject(childValueList)); //may still need work
            obj.Add("Value", JToken.FromObject(node.Value));
            //obj.Add("ValueType", JToken.FromObject(node.Value.GetType()));
            obj.WriteTo(writer);

            //serializer.Serialize(writer, value);
        }
    }
}
