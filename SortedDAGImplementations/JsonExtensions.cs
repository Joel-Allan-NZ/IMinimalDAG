using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGImplementations
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Converts an object to JSON and compresses it, writing to a selected filepath.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="settings"></param>
        public static void WriteCompressedJSONToFile(object value, string path, JsonSerializerSettings settings = null)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                CompressJsonStream(value, fs, settings);
        }

        /// <summary>
        /// Converts an object to a GZIP compressed JSON
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        /// <param name="settings"></param>
        public static void CompressJsonStream(object value, Stream stream, JsonSerializerSettings settings = null)
        {
            //using (var compressor = new GZipStream(stream, CompressionMode.Compress))
            using (var writer = new StreamWriter(new GZipStream(stream, CompressionMode.Compress)))
            {
                settings.Formatting = Formatting.Indented;
                var serializer = JsonSerializer.CreateDefault(settings);
                serializer.Serialize(writer, value);
            }
        }

        /// <summary>
        /// Reads a GZIP file containing a JSON, returning a CLR object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T ReadCompressedJSONFromFile<T>(string path, JsonSerializerSettings settings = null)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                return DeserializeCompressedJSON<T>(fs, settings);
        }

        /// <summary>
        /// Uncompresses and deserializes a GZIP compressed JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T DeserializeCompressedJSON<T>(Stream stream, JsonSerializerSettings settings = null)
        {
            //using (var compressor = new GZipStream(stream, CompressionMode.Decompress))
            //using (var reader = new StreamReader(new GZipStream(stream, CompressionMode.Decompress)))
            using (var jsonReader = new JsonTextReader(new StreamReader(new GZipStream(stream, CompressionMode.Decompress))))
            {
                var serializer = JsonSerializer.CreateDefault(settings);
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }

    public class DictionaryConverter<T, U> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Dictionary<T, U>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            Dictionary<T, U> dict = new Dictionary<T, U>();
            foreach(JObject obj in array.Children<JObject>())
            {
                var key = obj["Key"].ToObject<T>();
                var value = obj["Value"].ToObject<U>();
                dict.Add(key, value);
            }
            return dict;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Dictionary<T, U> dict = (Dictionary<T, U>)value;

            JArray array = new JArray();
            foreach(KeyValuePair<T, U> kvp in dict)
            {
                JObject obj = new JObject();
                obj.Add("Key", JToken.FromObject(kvp.Key));
                obj.Add("Value", JToken.FromObject(kvp.Value));
                array.Add(obj);
            }
            array.WriteTo(writer);
        }
    }
}
