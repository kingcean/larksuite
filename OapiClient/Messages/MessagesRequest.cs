using LarkSuite.OapiModels;
using LarkSuite.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Transactions;
using Trivial.Collection;
using Trivial.Net;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.OapiModels;

/// <summary>
/// The request options to send message.
/// </summary>
public class LarkMessageRequest : BaseQueryRequestInfo
{
    /// <summary>
    /// Initializes a new instance of the LarkMessageRequest class.
    /// </summary>
    public LarkMessageRequest()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkMessageRequest class.
    /// </summary>
    /// <param name="receiveIdType">The user identifier type, e.g. open_id, union_id, user_id, email, chat_id.</param>
    /// <param name="receiveId">The identifier of the receive user or chat group.</param>
    public LarkMessageRequest(string receiveIdType, string receiveId)
    {
        ReceiveIdType = receiveIdType;
        ReceiveId = receiveId;
    }

    /// <summary>
    /// Gets or sets the user identifier type.
    /// e.g. open_id, union_id, user_id, email, chat_id.
    /// </summary>
    [JsonIgnore]
    public string ReceiveIdType { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the receive user or chat group.
    /// </summary>
    [Description("The identifier of the receive user or chat group.")]
    [JsonPropertyName("receive_id")]
    public string ReceiveId { get; set; }

    /// <summary>
    /// Gets or sets the message content type.
    /// text, post (richtext), image, file, audio, media (video), sticker, interactive (card), share_chat (namecard of group with 7d expiration), share_user (namecard of user), system (seperator notification).
    /// </summary>
    [Description("The message content type, including text, post (richtext), image, file, audio, media (video), sticker, interactive (card), share_chat (namecard of group with 7d expiration), share_user (namecard of user), system (seperator notification).")]
    [JsonPropertyName("msg_type")]
    public string MessageType { get; set; }

    /// <summary>
    /// Gets or sets the message content in JSON string format.
    /// </summary>
    [Description("The message content in JSON string format.")]
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
    [Description("The message identifier (UUID).")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("uuid")]
    public string? Id { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        if (!string.IsNullOrWhiteSpace(ReceiveIdType)) q["receive_id_type"] = ReceiveIdType;
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

    public void SetMarkdown(string md)
    {
        MessageType = "post";
        var json = new JsonObjectNode
        {
            { "content", new JsonArrayNode
            {
                new JsonArrayNode
                {
                    new JsonObject {
                        { "tag", "md" },
                        { "text", md },
                    },
                },
            }
            }
        };
        Content = new()
        {
            { "zh_cn", json },
            { "en_us", json },
        };
    }

    public void SetMarkdown(string zh, string en)
    {
        MessageType = "post";
        Content = new()
        {
            { "zh_cn", new JsonObjectNode
            {
                { "content", new JsonArrayNode
                {
                    new JsonArrayNode
                    {
                        new JsonObject {
                            { "tag", "md" },
                            { "text", zh },
                        },
                    }
                }
                },
            } },
            { "en_us", new JsonObjectNode
            {
                { "content", new JsonArrayNode
                {
                    new JsonArrayNode
                    {
                        new JsonObject {
                            { "tag", "md" },
                            { "text", en },
                        },
                    }
                }
                },
            } },
        };
    }

    public void SetDivider(string text, JsonObjectNode? i18n = null, JsonObjectNode? options = null)
    {
        MessageType = "system";
        var json = new JsonObjectNode
        {
            { "text", text }
        };
        if (i18n is not null) json.SetValue("i18n_text", i18n);
        else json.SetValue("i18n_text", new JsonObjectNode());
        json = new()
        {
            { "type", "divider" },
            { "params", new JsonObjectNode
            {
                { "divider_text", json }
            }
            }
        };
        if (options is not null) json.SetValue("options", options);
        else json.SetValue("options", new JsonObjectNode());
        Content = json;
    }

    public void SetFile(string? messageType, string fileKey)
    {
        MessageType = messageType ?? "file";
        Content = new()
        {
            { "file_key", fileKey },
        };
    }
}

public class LarkMessageHistoryRequest : BaseQueryRequestInfo
{
    /// <summary>
    /// Gets or sets the type of container ID.
    /// e.g.: chat (including p2p and group), thread.
    /// </summary>
    public string ContainerIdType { get; set; }

    /// <summary>
    /// Gets the container ID.
    /// </summary>
    public string ContainerId { get; set; }

    /// <summary>
    /// Gets or sets the start date time of message creation.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date time of message creation.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the sort type.
    /// e.g.: ByCreateTimeAsc, ByCreateTimeDesc.
    /// </summary>
    public string? SortType { get; set; }

    /// <summary>
    /// Gets or sets the card type.
    /// e.g. user_card_content.
    /// </summary>
    public string? CardType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether return the root messages only for thread.
    /// </summary>
    public bool RootMessagesOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether return the user name.
    /// </summary>
    public bool SenderName { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        if (!string.IsNullOrWhiteSpace(ContainerIdType)) q["container_id_type"] = ContainerIdType;
        if (!string.IsNullOrWhiteSpace(ContainerId)) q["container_id"] = ContainerId;
        if (StartDate.HasValue) q["start_time"] = WebFormat.ParseDate(StartDate.Value).ToString("D");
        if (EndDate.HasValue) q["end_time"] = WebFormat.ParseDate(EndDate.Value).ToString("D");
        if (!string.IsNullOrWhiteSpace(SortType)) q["sort_type"] = SortType;
        if (!string.IsNullOrWhiteSpace(CardType)) q["card_msg_content_type"] = CardType;
        if (SenderName) q["with_sender_name"] = "true";
        if (RootMessagesOnly) q["only_thread_root_messages"] = "true";
    }

}

public class LarkMessageJsonCardRequest
{
    [JsonPropertyName("schema")]
    public string Schema { get; set; } = "2.0";

    [JsonPropertyName("config")]
    public JsonObjectNode Config { get; set; } = [];

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("card_link")]
    public JsonObjectNode? Link { get; set; }

    [JsonPropertyName("header")]
    public JsonObjectNode Header { get; set; } = [];

    [JsonPropertyName("body")]
    public JsonObjectNode Body { get; set; } = [];

    public JsonObjectNode ToJson()
        => new()
        {
            { "schema", Schema },
            { "config", Config },
            { "card_link", Link },
            { "header", Header },
            { "body", Body },
        };
}

public class LarkMessageTemplateCardRequest
{
    [JsonPropertyName("template_id")]
    public string Id { get; set; }

    [JsonPropertyName("template_version_name")]
    public string Version { get; set; }

    [JsonPropertyName("template_variable")]
    public JsonObjectNode Variables { get; set; } = new();

    public JsonObjectNode ToJson()
        => new()
        {
            { "template_id", Id },
            { "template_version_name", Version },
            { "template_variable", Variables },
        };
}

public class LarkMessageElementUpdateRequest
{
    public LarkMessageElementUpdateRequest()
    {
    }

    public LarkMessageElementUpdateRequest(string cardId, string elementId)
    {
        CardId = cardId;
        ElementId = elementId;
    }

    public LarkMessageElementUpdateRequest(string cardId, string elementId, int sequence, string? textContent, string? actionId = null)
    : this(cardId, elementId)
    {
        Sequence = sequence;
        TextContent = textContent;
        ActionId = actionId;
    }

    public LarkMessageElementUpdateRequest(string cardId, string elementId, int sequence, JsonObjectNode? elementValue, string? actionId = null)
    : this(cardId, elementId)
    {
        Sequence = sequence;
        ElementSerialized = elementValue?.ToString();
        ActionId = actionId;
    }

    [JsonIgnore]
    public string CardId { get; set; }

    [JsonIgnore]
    public string ElementId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("uuid")]
    public string? ActionId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("content")]
    public string? TextContent { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("element")]
    public string? ElementSerialized { get; set; }

    [JsonPropertyName("sequence")]
    public int Sequence { get; set; }

    public void SetElementValue(JsonObjectNode value)
        => ElementSerialized = value?.ToString();
}

public class LarkMessageSettingsUpdateRequest
{
    public LarkMessageSettingsUpdateRequest()
    {
    }

    public LarkMessageSettingsUpdateRequest(string cardId)
    {
        CardId = cardId;
    }

    public LarkMessageSettingsUpdateRequest(string cardId, int sequence, JsonObjectNode? settings, string? actionId = null)
        : this(cardId)
    {
        Sequence = sequence;
        SettingsSerialized = settings?.ToString();
        ActionId = actionId;
    }

    [JsonIgnore]
    public string CardId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("uuid")]
    public string? ActionId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("settings")]
    public string? SettingsSerialized { get; set; }

    [JsonPropertyName("sequence")]
    public int Sequence { get; set; }

    public void SetSettings(JsonObjectNode value)
        => SettingsSerialized = value?.ToString();
}

public class LarkMessageStreamingRequest(LarkMessageRequest request, string cardId, string elementId)
{
    public string CardId { get; } = cardId;

    public string ElementId { get; } = elementId;

    public int Sequence { get; private set; }

    public LarkMessageRequest Request { get; } = request;

    public LarkMessageElementUpdateRequest Update()
    {
        Sequence++;
        return new(CardId, ElementId)
        {
            Sequence = Sequence
        };
    }

    public LarkMessageElementUpdateRequest Update(string textContent, string actionId = null)
    {
        Sequence++;
        return new(CardId, ElementId, Sequence, textContent, actionId);
    }

    public LarkMessageElementUpdateRequest Update(StringBuilder textContent, string actionId = null)
    {
        Sequence++;
        return new(CardId, ElementId, Sequence, textContent?.ToString(), actionId);
    }

    public LarkMessageElementUpdateRequest Update(JsonObjectNode elementValue, string actionId = null)
    {
        Sequence++;
        return new(CardId, ElementId, Sequence, elementValue, actionId);
    }

    public LarkMessageSettingsUpdateRequest Finish(string actionId = null)
    {
        Sequence++;
        return new LarkMessageSettingsUpdateRequest(CardId, Sequence, new()
        {
            { "config", new JsonObjectNode
            {
                { "streaming_mode", false },
            }
            }
        }, actionId);
    }
}
