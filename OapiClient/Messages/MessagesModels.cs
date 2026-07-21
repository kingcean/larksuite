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
