using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.Text;

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

public class JsonLarkBaseStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.Number:
                return reader.GetDouble().ToString();
            case JsonTokenType.StartObject:
                var json = JsonObjectNode.ParseValue(ref reader);
                return json?.TryGetStringTrimmedValue("text");
            case JsonTokenType.StartArray:
                var arr = JsonArrayNode.ParseValue(ref reader);
                var sb = new StringBuilder();
                foreach (var item in arr)
                {
                    if (item is null) continue;
                    if (item is IJsonValueNode<string> s)
                    {
                        sb.Append(s.Value);
                        continue;
                    }

                    if (item is IJsonValueNode<long> i1)
                    {
                        sb.Append(i1);
                        continue;
                    }

                    if (item is IJsonValueNode<double> i2)
                    {
                        sb.Append(i2);
                        continue;
                    }

                    if (item is JsonObjectNode j)
                    {
                        sb.Append(j.TryGetStringValue("text"));
                        continue;
                    }
                }

                return sb.ToString();
            default:
                throw new JsonException();
        }
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteStringValue(value);
    }
}
