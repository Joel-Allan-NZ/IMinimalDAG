using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using IMinimalDAGInterfaces;
using Newtonsoft.Json.Serialization;

namespace MinimalDAGImplementations.Serializer
{ 
    public static class MinimalDAGSerializer
    {
        public static void Compress<T>(IMinimalDAG<T> MinimalDAG, string filePath, JsonConverter tConverter = null)
        {
            try
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                if (tConverter != null)
                    settings.Converters.Add(tConverter);
                settings.Converters.Add(new DictionaryConverter<T, List<Guid>>());
                settings.Converters.Add(new MinimalDAGNodeConverter<T>());
                settings.Converters.Add(new MinimalDAGNodeFactoryConverter<T>());

                JsonExtensions.SerializeToFileCompressed(MinimalDAG, filePath, settings);
            }
            catch(UnauthorizedAccessException e)
            {
                throw e;
            }
        }

        public static IMinimalDAG<T> ReadCompressed<T>(string filePath, JsonConverter tConverter = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath.ToString() + "is not a valid file or directory");
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            if (tConverter != null)
                settings.Converters.Add(tConverter);
            settings.Converters.Add(new DictionaryConverter<T, List<Guid>>());
            settings.Converters.Add(new MinimalDAGNodeConverter<T>());
            settings.Converters.Add(new MinimalDAGNodeFactoryConverter<T>());

            return (IMinimalDAG<T>)JsonExtensions.DeserializeFromFileCompressed<IMinimalDAG<T>>(filePath, settings);
        }
    }
}
