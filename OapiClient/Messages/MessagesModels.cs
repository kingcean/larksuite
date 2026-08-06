using LarkSuite.OapiModels;
using LarkSuite.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Trivial.Collection;
using Trivial.Net;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.OapiModels;

public class LarkMessageResponse
{
    [JsonPropertyName("message_id")]
    public string Id { get; set; }

    [JsonPropertyName("root_id")]
    public string RootId { get; set; }

    [JsonPropertyName("parent_id")]
    public string ParentId { get; set; }

    [JsonPropertyName("thread_id")]
    public string ThreadId { get; set; }

    [JsonPropertyName("msg_type")]
    public string MessageType { get; set; }

    [JsonPropertyName("create_time")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime CreationDate { get; set; }

    [JsonPropertyName("update_time")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime LastModificationDate { get; set; }

    [JsonPropertyName("deleted")]
    public bool IsDeleted { get; set; }

    [JsonPropertyName("updated")]
    public bool IsModified { get; set; }

    [JsonPropertyName("chat_id")]
    public string ChatGroupId { get; set; }

    [JsonPropertyName("sender")]
    public LarkMessageSenderInfo Sender { get; set; }

    [JsonPropertyName("body")]
    public LarkMessageContentInfo Content { get; set; }

    [JsonPropertyName("mentions")]
    public List<LarkMessageMentionInfo> Mentions { get; set; }

    [JsonPropertyName("upper_message_id")]
    public string UpperMessageId { get; set; }

    public string? GetContentString()
    {
        var content = Content?.Content;
        if (content is null) return null;
        return MessageType switch
        {
            "text" or "hongbao" => content.TryGetStringTrimmedValue("text", true),
            "post" => LarkApiUtils.GetRichMessageText(content.TryGetStringValue("title"), content.TryGetArrayValue("content_v2") ?? content.TryGetArrayValue("content")),
            "card" or "interactive" => LarkApiUtils.GetRichMessageText(content.TryGetStringValue("title"), content.TryGetArrayValue("elements") ?? content.TryGetObjectValue("body")?.TryGetArrayValue("elements")),
            "image" => $"Image `{content.TryGetStringTrimmedValue("image_key")}`",
            "file" => $"File `{content.TryGetStringTrimmedValue("file_key")}` - {content.TryGetStringTrimmedValue("file_name")}",
            "folder" => $"Folder `{content.TryGetStringTrimmedValue("file_key")}` - {content.TryGetStringTrimmedValue("file_name")}",
            "audio" => $"Audio `{content.TryGetStringTrimmedValue("file_key")}`",
            "media" => $"Video `{content.TryGetStringTrimmedValue("file_key")}` - {content.TryGetStringTrimmedValue("file_name")}",
            "sticker" => $"Sticker `{content.TryGetStringTrimmedValue("file_key")}`",
            "share_user" => $"User `{content.TryGetStringTrimmedValue("user_id")}`",
            "share_chat" => $"Chat `{content.TryGetStringTrimmedValue("chat_id")}`",
            "calendar" or "share_calendar_event" or "general_calendar" => $"{content.TryGetStringTrimmedValue("summary") ?? "Calendar item"} | {WebFormat.ParseDate(content.TryGetInt64Value("start_time"))?.ToString() ?? "?"} → {WebFormat.ParseDate(content.TryGetInt64Value("end_time"))?.ToString() ?? "?"}",
            "location" => $"{content.TryGetStringTrimmedValue("name") ?? "Location"} (Longitude = {content.TryGetStringTrimmedValue("longitude") ?? "unknown"} & Latitude = {content.TryGetStringTrimmedValue("latitude") ?? "unknown"})",
            "vote" => $"""
                        ## {content.TryGetStringTrimmedValue("topic")}

                        {string.Join(Environment.NewLine, content.TryGetStringListValue("options", true))}
                        """,
            "todo" => $"ToDo `{content.TryGetStringTrimmedValue("task_id")}`",
            "system" => "---",
            "merge_forward" => $"> {content.TryGetStringValue("content")}",
            _ => null,
        };
    }
}

public class LarkMessageSenderInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("id_type")]
    public string IdType { get; set; }

    [JsonPropertyName("sender_type")]
    public string SenderType { get; set; }

    [JsonPropertyName("tenant_key")]
    public string TenantKey { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("sender_name")]
    public string? SenderName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("sender_i18n_names")]
    public JsonObjectNode? SenderNames { get; set; }
}

public class LarkMessageContentInfo
{
    /// <summary>
    /// Gets or sets the message content in JSON string format.
    /// </summary>
    [JsonPropertyName("content")]
    public string ContentSerialized
    {
        get => (Content ?? [])?.ToString()!;
        set => Content = JsonObjectNode.TryParse(value);
    }

    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    [JsonIgnore]
    public JsonObjectNode Content { get; set; }
}

public class LarkMessageMentionInfo
{
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("id_type")]
    public string IdType { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("tenant_key")]
    public string TenantKey { get; set; }
}

public class LarkSimpleStreamingMessageOptions
{
    /// <summary>
    /// Gets or sets the placeholder in markdown format. This is the initialized content.
    /// </summary>
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the title in plain text.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the subtitle in plain text.
    /// </summary>
    public string? Subtitle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether need disable the permission to forward.
    /// </summary>
    public bool DisableForward { get; set; }

    /// <summary>
    /// Gets or sets the background color of title.
    /// e.g. blue, wathet, turquoise, green, yellow, orange, red, carmine, violet, purple, indigo, grey, default.
    /// </summary>
    public string? TitleBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the width mode.
    /// e.g. default (600px), compact (400px), fill.
    /// </summary>
    public string? WidthMode { get; set; }
}

public class LarkEventMessageArgs
{
    public LarkEventMessageArgs(LarkEventMessageHeader header, JsonObjectNode body)
    {
        Header = header ?? new();
        Body = body;
    }

    public LarkEventMessageHeader Header { get; }

    public string EventId => Header.Id;

    public string EventType => Header.EventType;

    public string VerificationToken { get; set; }

    public DateTime CreationDate => Header.CreationDate;

    public JsonObjectNode Body { get; }

    public T GetBody<T>()
        => Body is null ? default : Body.Deserialize<T>();
}

public class LarkEventMessage
{
    [JsonPropertyName("schema")]
    public string Schema { get; set; } = "2.0";

    [JsonPropertyName("header")]
    public LarkEventMessageHeader Header { get; set; }

    [JsonPropertyName("event")]
    public JsonObjectNode Body { get; set; }
}

public class LarkEventMessageHeader
{
    [JsonPropertyName("event_id")]
    public string Id { get; set; }

    [JsonPropertyName("event_type")]
    public string EventType { get; set; }

    [JsonPropertyName("token")]
    public string VerificationToken { get; set; }

    [JsonPropertyName("create_time")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime CreationDate { get; set; } = DateTime.Now;

    [JsonPropertyName("app_id")]
    public string AppId { get; set; }

    [JsonPropertyName("tenant_key")]
    public string TenantKey { get; set; }
}
