﻿using Api.Model;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Api.Utilities
{
    public class CharacterSeparatedStringConverter<T> : JsonConverter<T> where T: CharacterSeparatedString
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Activator.CreateInstance(typeof(T), reader.GetString()) as T;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
