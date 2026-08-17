using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cdm.Figma.Json
{
    public abstract class SubTypeJsonConverter<TObjectType, TTypeToken> : JsonConverter
    {
        // A document has many values but few distinct discriminators, so both lookups are memoised.
        // Not thread safe, and does not need to be: an instance belongs to one serializer.
        private readonly Dictionary<string, TTypeToken> _typeTokens =
            new Dictionary<string, TTypeToken>(StringComparer.Ordinal);

        private readonly Dictionary<Type, Func<object>> _creators = new Dictionary<Type, Func<object>>();

        protected virtual string GetTypeToken()
        {
            return "type";
        }

        protected abstract bool TryGetActualType(TTypeToken typeToken, out Type type);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            // Nested values arrive through a JTokenReader over a tree that already exists, and
            // JToken.Load would deep copy the subtree at every level, copying a node once per
            // ancestor. Reuse the token that is there and step the reader past it.
            JToken token;
            if (reader is JTokenReader tokenReader && tokenReader.CurrentToken != null)
            {
                token = tokenReader.CurrentToken;
                reader.Skip();
            }
            else
            {
                token = JToken.Load(reader);
            }

            var typeToken = token[GetTypeToken()];
            if (typeToken == null)
                throw new JsonReaderException($"Missing {typeof(TTypeToken).Name} type.");

            TTypeToken typeTokenValue;
            if (typeToken.Type == JTokenType.String)
            {
                var key = (string)typeToken;
                if (!_typeTokens.TryGetValue(key, out typeTokenValue))
                {
                    // Through the serializer, so enum member naming and any converter on the enum
                    // still apply.
                    typeTokenValue = typeToken.ToObject<TTypeToken>(serializer);
                    _typeTokens.Add(key, typeTokenValue);
                }
            }
            else
            {
                typeTokenValue = typeToken.ToObject<TTypeToken>(serializer);
            }

            if (!TryGetActualType(typeTokenValue, out var actualType))
                throw new JsonReaderException($"Unknown {nameof(PaintType)} got: '{typeToken}'.");

            if (existingValue == null || existingValue.GetType() != actualType)
            {
                if (!_creators.TryGetValue(actualType, out var creator))
                {
                    creator = serializer.ContractResolver.ResolveContract(actualType)?.DefaultCreator;
                    _creators.Add(actualType, creator);
                }

                existingValue = creator?.Invoke();
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