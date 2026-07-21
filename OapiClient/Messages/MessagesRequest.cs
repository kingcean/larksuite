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

public class LarkMessageRequest : BaseQueryRequestInfo
{
    /// <summary>
    /// Gets or sets the user identifier type.
    /// open_id, union_id, user_id, email, chat_id.
    /// </summary>
    [JsonIgnore]
    public string UserIdType { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the receive user or chat group.
    /// </summary>
    [JsonPropertyName("receive_id")]
    public string ReceiveId { get; set; }

    /// <summary>
    /// Gets or sets the message content type.
    /// text, post (richtext), image, file, audio, media (video), sticker, interactive (card), share_chat (namecard of group with 7d expiration), share_user (namecard of user), system (seperator notification).
    /// </summary>
    [JsonPropertyName("msg_type")]
    public string MessageType { get; set; }

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

    /// <summary>
    /// Gets or sets the message identifier (UUID).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("uuid")]
    public string? Id { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        if (!string.IsNullOrWhiteSpace(UserIdType)) q["receive_id_type"] = UserIdType;
    }

    /// <summary>
    /// Sets text content.
    /// </summary>
    /// <param name="text">The text.</param>
    public void SetTextContent(string text)
    {
        MessageType = "text";
        Content = new()
        {
            { "text", text }
        };
    }
}
