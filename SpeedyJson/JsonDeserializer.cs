using SpeedyJson.Providers;
using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson
{
    public static class JsonDeserializer
    {
        public static T Deserialize<T>(string value, JsonSettings settings = null)
        {
            return (T)DeserializeImpl(typeof(T), value, settings);
        }

        public static object Deserialize(Type type, string value, JsonSettings settings = null)
        {
            return DeserializeImpl(type, value, settings);
        }

        private static unsafe object DeserializeImpl(Type type, string value, JsonSettings settings)
        {
            Guard.ArgumentNotNull(type, nameof(value));
            Guard.ArgumentNotNull(value, nameof(value));

            fixed (char* pointer = value)
            {
                var jsonReader = new JsonReader(pointer, value.Length);
                return DeserializeImpl(type, jsonReader, settings);
            }
        }

        internal static unsafe object DeserializeImpl(Type type, JsonReader jsonReader, JsonSettings settings)
        {
            var info = TypeInfoProvider.GetOrCreate(type);
            return info.Deserializer(jsonReader, settings ?? JsonSettings.Default);
        }
    }
}
