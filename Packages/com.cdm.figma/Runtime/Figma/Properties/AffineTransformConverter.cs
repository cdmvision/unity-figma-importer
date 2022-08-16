using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cdm.Figma
{
    public class AffineTransformConverter : JsonConverter<AffineTransform>
    {
        public override void WriteJson(JsonWriter writer, AffineTransform value, JsonSerializer serializer)
        {
            if (value != null)
            {
                serializer.Serialize(writer, value.values);
            }
        }

        public override AffineTransform ReadJson(JsonReader reader, Type objectType, AffineTransform existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                var values = token.ToObject<float[][]>();
                return new AffineTransform(values);
            }

            return existingValue;
        }
    }
}