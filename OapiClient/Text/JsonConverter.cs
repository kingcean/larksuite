using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.Text;

public class JsonDateTimeTickStringConverter : JsonConverter<DateTime>
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

public class JsonUrlFieldStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.StartObject:
                var json = JsonObjectNode.ParseValue(ref reader);
                return json?.TryGetStringTrimmedValue("url");
            default:
                throw new JsonException();
        }
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("url", value);
        writer.WriteEndObject();
    }
}
