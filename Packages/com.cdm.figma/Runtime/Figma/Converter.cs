using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Cdm.Figma
{
    internal class NonPublicPropertiesResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
            var prop = base.CreateProperty(member, memberSerialization);
            if (member is PropertyInfo pi) {
                prop.Readable = (pi.GetMethod != null);
                prop.Writable = (pi.SetMethod != null);
            }
            return prop;
        }
    }
    
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            ContractResolver = new NonPublicPropertiesResolver(),
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal},
                new AffineTransformConverter()
            },
            NullValueHandling = NullValueHandling.Ignore,
        };
    }
}