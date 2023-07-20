using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cdm.Figma.Json
{
    public abstract class SubTypeJsonConverter<TObjectType, TTypeToken> : JsonConverter
    {
        protected virtual string GetTypeToken()
        {
            return "type";
        }

        protected abstract bool TryGetActualType(TTypeToken typeToken, out Type type);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            var typeToken = token[GetTypeToken()];
            if (typeToken == null)
                throw new JsonReaderException($"Missing {typeof(TTypeToken).Name} type.");

            if (!TryGetActualType(typeToken.ToObject<TTypeToken>(serializer), out var actualType))
                throw new JsonReaderException($"Unknown {nameof(PaintType)} got: '{typeToken}'.");

            if (existingValue == null || existingValue.GetType() != actualType)
            {
                var contract = serializer.ContractResolver.ResolveContract(actualType);
                existingValue = contract?.DefaultCreator?.Invoke();
            }

            if (existingValue == null)
                return null;

            using (var subReader = token.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                serializer.Populate(subReader, existingValue);
            }

            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(TObjectType).IsAssignableFrom(objectType);
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}