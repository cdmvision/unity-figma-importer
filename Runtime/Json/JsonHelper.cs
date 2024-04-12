using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Cdm.Figma.Json
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(string json)
        {
            var serializer = JsonSerializer.Create(CreateSettings());

            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                return serializer.Deserialize<T>(reader);
            }
        }

        public static string Serialize(object value, Formatting formatting = Formatting.None)
        {
            var serializer = JsonSerializer.Create(CreateSettings());
            serializer.Formatting = formatting;

            var stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
            using (var jsonTextWriter = new JsonTextWriter(stringWriter))
            {
                jsonTextWriter.Formatting = serializer.Formatting;
                serializer.Serialize(jsonTextWriter, value, null);
            }

            return stringWriter.ToString();
        }

        public static JsonSerializerSettings CreateSettings()
        {
            return new JsonSerializerSettings()
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                Converters =
                {
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal },
                    new StringEnumConverter(),
                    new AffineTransformJsonConverter(),
                    new NodeJsonConverter(),
                    new EffectJsonConverter(),
                    new PaintJsonConverter(),
                    new ComponentPropertyDefinitionJsonConverter(),
                    new ComponentPropertyJsonConverter()
                },
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };
        }
    }

    internal class NonPublicPropertiesResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (member is PropertyInfo pi)
            {
                prop.Readable = (pi.GetMethod != null);
                prop.Writable = (pi.SetMethod != null);
            }

            return prop;
        }
    }
}