using IMinimalDAGInterfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGImplementations
{
    public class MinimalDAGConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(MinimalDAG<T>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            var source = obj["Source"].ToObject<MinimalDAGNode<T>>(serializer);
            var sink = obj["Sink"].ToObject<MinimalDAGNode<T>>(serializer);//(MinimalDAGNode<T>, serializer);
            var nodes = obj["Nodes"].ToObject<Dictionary<Guid, MinimalDAGNode<T>>>(serializer);
            var Inodes = nodes.Select(d => new KeyValuePair<Guid, IMinimalDAGNode<T>>(d.Key, d.Value))
                              .ToDictionary(k => k.Key, k => k.Value);
            var nodeFactory = obj["nodeFactory"].ToObject<MinimalDAGNodeFactory<T>>(serializer);
            return new MinimalDAG<T>(source, sink, nodeFactory, Inodes);
            //obj["Nodes"].
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            MinimalDAG<T> dag = (MinimalDAG<T>)value;
            JObject obj = new JObject();
            obj.Add("Source", JToken.FromObject(dag.Source, serializer));
            obj.Add("Sink", JToken.FromObject(dag.Source, serializer));
            obj.Add("nodeFactory", JToken.FromObject(dag.dagNodeFactory, serializer));
            obj.Add("Nodes", JToken.FromObject(dag.Nodes, serializer));

            obj.WriteTo(writer);

            //throw new NotImplementedException();
        }
    }
}
