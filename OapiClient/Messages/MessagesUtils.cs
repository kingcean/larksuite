using LarkSuite.Docs;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Collection;
using Trivial.Text;

namespace LarkSuite.OapiModels;

/// <summary>
/// The utilities of Lark doc content.
/// </summary>
public static partial class LarkApiUtils
{
    public static LarkEventMessageArgs? ToEventMessage(JsonObjectNode json)
    {
        if (json is null) return null;
        LarkEventMessageHeader header;
        var headerJson = json.TryGetObjectValue("header");
        if (headerJson is null)
        {
            var uuid = json.TryGetStringTrimmedValue("uuid", true);
            var eventType = json.TryGetStringTrimmedValue("type", true);
            if (uuid is null || eventType is null) return null;
            header = new()
            {
                Id = uuid,
                EventType = eventType,
                VerificationToken = json.TryGetStringTrimmedValue("token", true),
            };
        }
        else
        {
            header = headerJson.Deserialize<LarkEventMessageHeader>();
            if (string.IsNullOrWhiteSpace(header?.Id) || string.IsNullOrWhiteSpace(header.EventType)) return null;
        }

        return new(header, json.TryGetObjectValue("event"));
    }

    public static LarkEventMessageArgs? ToEventMessage(LarkApi larkApi, JsonObjectNode json)
    {
        var args = ToEventMessage(json);
        if (args is null) return null;
        larkApi.OnEventReceived(args);
        return args;
    }

    internal static string GetRichMessageText(string title, JsonArrayNode? content)
    {
        var col = new List<string>();
        if (!string.IsNullOrWhiteSpace(title))
        {
            col.Add($"# {title}");
        }

        if (content is not null)
        {
            foreach (var element1 in content)
            {
                if (element1 is not JsonArrayNode line)
                {
                    if (element1 is JsonObjectNode elementJson) line = new()
                    {
                        elementJson
                    };
                    else continue;
                }

                var sb = new StringBuilder();
                var needBreak = false;
                foreach (var element2 in line)
                {
                    if (element2 is not JsonObjectNode json) continue;
                    if (needBreak)
                    {
                        sb.AppendLine();
                        sb.AppendLine();
                        needBreak = false;
                    }

                    var tag = json.TryGetStringTrimmedValue("tag", true);
                    switch (tag)
                    {
                        case "a":
                            sb.Append($"[{json.TryGetStringValue("text")}]({json.TryGetStringValue("href")})");
                            break;
                        case "at":
                            sb.Append(json.TryGetStringValue("user_id"));
                            var userName = json.TryGetStringTrimmedValue("user_name", true);
                            if (userName is not null) sb.Append($" ({userName})");
                            break;
                        case "hr":
                            if (sb.Length > 0) sb.AppendLine();
                            sb.Append("---");
                            needBreak = true;
                            break;
                        case "code_block":
                            if (sb.Length > 0) sb.AppendLine();
                            sb.Append("```");
                            sb.AppendLine(json.TryGetStringTrimmedValue("language", true)?.ToLowerInvariant() ?? string.Empty);
                            sb.AppendLine(json.TryGetStringValue("text"));
                            sb.Append("```");
                            needBreak = true;
                            break;
                        case "note":
                            var elements = json.TryGetObjectListValue("elements", true);
                            if (elements is null) break;
                            foreach (var element3 in elements)
                            {
                                var elementText = element3.TryGetStringValue("text");
                                if (!string.IsNullOrEmpty(elementText)) sb.Append(elementText);
                            }

                            break;
                        default:
                            var text = json.TryGetStringValue("text") ?? json.TryGetStringValue("content");
                            if (string.IsNullOrEmpty(text)) break;
                            sb.Append(text);
                            if (tag == "md" || tag == "markdown") needBreak = true;
                            break;
                    }
                }

                var s = sb.ToString();
                if (!string.IsNullOrEmpty(s)) col.Add(s);
            }
        }

        return string.Join(string.Concat(Environment.NewLine, Environment.NewLine), col);
    }
}
