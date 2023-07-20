﻿using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Cdm.Figma.Json
{
    public class ColorHexJsonConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteValue($"#{ColorUtility.ToHtmlStringRGBA((UnityEngine.Color)value).ToLower()}");
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (ColorUtility.TryParseHtmlString((string)reader.Value, out var color))
                return (Color)color;

            if (ColorUtility.TryParseHtmlString($"#{(string)reader.Value}", out color))
                return (Color)color;

            if (hasExistingValue)
                return existingValue;

            return default;
        }
    }
}