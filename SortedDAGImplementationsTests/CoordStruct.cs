using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDAGImplementations.Tests
{
    public struct Coord : IComparable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            var otherCoord = (Coord)obj;

            var XCompare = X.CompareTo(otherCoord.X);
            if (XCompare != 0)
                return Y.CompareTo(otherCoord.Y);
            else
                return XCompare;


        }

        public override int GetHashCode()
        {
            return (X.GetHashCode() * Y.GetHashCode()).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj != null && obj is Coord)
            {
                var other = (Coord)obj;
                if(other.X == this.X)
                {
                    if (other.Y == this.Y)
                        return true;
                }
            }
            return false;
        }
    }


    public class CoordConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Coord);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var x = default(int);
            var y = default(int);
            //bool retrievedX = false;
            //bool retrievedY = false;
            while( reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    break;

                var propertyName = (string)reader.Value;

                if (!reader.Read())
                    continue;

                if(propertyName == "X")
                {
                    x = serializer.Deserialize<int>(reader);
                    //retrievedX = true;
                }
                if (propertyName == "Y")
                {
                    y = serializer.Deserialize<int>(reader);
                    //retrievedY = true;
                }
            }
            return new Coord(x, y);
            //return serializer.Deserialize(reader, typeof(Coord));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("X");
            serializer.Serialize(writer, ((Coord)value).X);
            writer.WritePropertyName("Y");
            serializer.Serialize(writer, ((Coord)value).Y);
            writer.WriteEndObject();
            //serializer.Serialize(writer, value);
        }
    }
}
