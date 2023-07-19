using System.Globalization;
using System.Reflection;
using Cdm.Figma.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Cdm.Figma
{
    public static class JsonSerializerHelper
    {
        public static JsonSerializerSettings settings => CreateSettings();

        public static JsonSerializerSettings CreateSettings()
        {
            return new JsonSerializerSettings()
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                Converters =
                {
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal },
                    new StringEnumConverter(),
                    new AffineTransformConverter(),
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