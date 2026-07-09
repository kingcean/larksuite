using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Web;

namespace LarkSuite.Text;

internal class JsonDateTimeTickStringConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return long.TryParse(reader.GetString(), out var i) ? WebFormat.ParseDate(i) : throw new JsonException();
            case JsonTokenType.Number:
                return WebFormat.ParseDate(reader.GetInt64());
            default:
                throw new JsonException();
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var s = WebFormat.ParseDate(value).ToString("G");
        writer.WriteStringValue(s);
    }
}

internal class JsonDateTimeTickNumberConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return long.TryParse(reader.GetString(), out var i) ? WebFormat.ParseDate(i) : throw new JsonException();
            case JsonTokenType.Number:
                return WebFormat.ParseDate(reader.GetInt64());
            default:
                throw new JsonException();
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var s = WebFormat.ParseDate(value);
        writer.WriteNumberValue(s);
    }
}

internal class JsonDateTimeSecondNumberConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return long.TryParse(reader.GetString(), out var i) ? WebFormat.ParseDate(i * 1000) : throw new JsonException();
            case JsonTokenType.Number:
                return WebFormat.ParseDate(reader.GetInt64());
            default:
                throw new JsonException();
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var s = WebFormat.ParseDate(value) / 1000;
        writer.WriteNumberValue(s);
    }
}
