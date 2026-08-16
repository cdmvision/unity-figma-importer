using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cdm.Figma.Json
{
    public class AffineTransformJsonConverter : JsonConverter<AffineTransform>
    {
        public override void WriteJson(JsonWriter writer, AffineTransform value, JsonSerializer serializer)
        {
            if (value != null)
            {
                serializer.Serialize(writer, value.values);
            }
        }

        public override AffineTransform ReadJson(JsonReader reader, Type objectType, AffineTransform existingValue, 
            bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            
            if (token.Type == JTokenType.Array)
            {
                var values = token.ToObject<float[][]>(serializer);
                return new AffineTransform(values);
            }

            return existingValue;
        }
    }
}