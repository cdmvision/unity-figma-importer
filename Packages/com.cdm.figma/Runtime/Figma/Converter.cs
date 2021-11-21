using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cdm.Figma
{
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
            NullValueHandling = NullValueHandling.Ignore,
        };
    }
}