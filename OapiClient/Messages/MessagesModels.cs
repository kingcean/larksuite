using LarkSuite.OapiModels;
using LarkSuite.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Trivial.Collection;
using Trivial.Data;
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
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    public DateTime CreationDate { get; set; }

    [JsonPropertyName("update_time")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
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

    [JsonIgnore]
    public JsonObjectNode? ContentJson => Content?.Content;

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

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (IsDeleted) sb.Append("Deleted | ");
        sb.Append(Sender is null ? "? " : $"[{Sender.SenderType}] {Sender.SenderName ?? Sender.Id} ");
        sb.Append(CreationDate.ToString("f"));
        sb.Append(" | ");
        var content = Content?.Content;
        sb.Append(MessageType);
        if (content is null)
        {
            sb.Append(" without content");
        }
        else if (MessageType == "text" || MessageType == "hongbao")
        {
            var text = Content?.Content.TryGetStringTrimmedValue("text", true);
            if (string.IsNullOrWhiteSpace(text))
            {
                sb.Append(" empty");
            }
            else
            {
                sb.Append(" | ");
                sb.Append(text);
            }
        }

        return sb.ToString();
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

    public override string ToString()
        => $"[{SenderType}] {SenderName ?? "?"} ({IdType} {Id})";
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

    public override string ToString()
        => $"{Name ?? "?"} ({IdType} {Id})";
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

    public override string ToString()
        => $"{EventType} {CreationDate:f} {EventId}";
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
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    public DateTime CreationDate { get; set; } = DateTime.Now;

    [JsonPropertyName("app_id")]
    public string AppId { get; set; }

    [JsonPropertyName("tenant_key")]
    public string TenantKey { get; set; }

    public override string ToString()
        => $"{EventType} {CreationDate:f} {Id}";
}

public class LarkMessageSendResult
{
    public LarkMessageSendResult()
    {
    }

    public LarkMessageSendResult(string id, LarkIdNameInfo recipient, string? message = null)
    {
        MessageId = id;
        Recipient = recipient;
        ResultMessage = message;
    }

    public LarkMessageSendResult(bool isError, string? message = null)
    {
        IsError = isError;
        ResultMessage = message;
    }

    public LarkMessageSendResult(string id, bool isError, string? message = null)
        : this(isError, message)
    {
        MessageId = id;
    }

    public LarkMessageSendResult(string? id, bool isError, LarkIdNameInfo? recipient, string? message = null)
        : this(isError, message)
    {
        MessageId = id;
        Recipient = recipient;
    }

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MessageId { get; set; }

    [JsonPropertyName("recipient")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public LarkIdNameInfo? Recipient { get; set; }

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ResultMessage { get; set; }

    [JsonPropertyName("error")]
    public bool IsError { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(MessageId);
        if (IsError) sb.Append(" Error");
        if (!string.IsNullOrWhiteSpace(ResultMessage))
        {
            sb.Append(' ');
            sb.Append(ResultMessage);
        }

        if (Recipient is not null)
        {
            sb.Append(" | Recipient ");
            sb.Append(Recipient.ToString());
        }

        return sb.ToString();
    }
}

public class LarkMessageGroupRestrictInfo
{
    [JsonPropertyName("status")]
    public bool Status { get; set; }

    [JsonPropertyName("screenshot_has_permission_setting")]
    public string Screenshot { get; set; }

    [JsonPropertyName("download_has_permission_setting")]
    public string Download { get; set; }

    [JsonPropertyName("message_has_permission_setting")]
    public string Copy { get; set; }
}

public class BaseLarkMessageGroupInfo : IIdPropertyModel
{
    [JsonPropertyName("chat_id")]
    public string Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("avatar")]
    public string? AvatarUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("i18n_names")]
    public JsonObjectNode LocaleNames { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("owner_id")]
    public string? OwnerId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("owner_id_type")]
    public string? OwnerIdType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the message group is available for the users out of current organization.
    /// </summary>
    [JsonPropertyName("external")]
    [Description("A value indicating whether the message group is available for the users out of current organization.")]
    public bool IsExternal { get; set; }

    /// <summary>
    /// Gets or sets the tenant key.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("tenant_key")]
    [Description("The tenant key.")]
    public string? TenantKey { get; set; }

    /// <summary>
    /// Gets or sets the permission for hide member count setting.
    /// <list type="bullet">
    /// <item>normal</item>
    /// <item>dissolved</item>
    /// <item>dissolved_save</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("chat_status")]
    [Description("The status of the message group, including `normal`, `dissolved` and `dissolved_save`.")]
    public string? Status { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}

public class LarkMessageGroupInfo : BaseLarkMessageGroupInfo
{
    /// <summary>
    /// Gets or sets the permission for sending urgent messages.
    /// <list type="bullet">
    /// <item>only_owner</item>
    /// <item>all_members</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("urgent_setting")]
    [Description("The permission for sending urgent messages, including `only_owner` and `all_members`.")]
    public string? UrgentMessagePermission { get; set; }

    /// <summary>
    /// Gets or sets the permission for video conference setting.
    /// <list type="bullet">
    /// <item>only_owner</item>
    /// <item>all_members</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("video_conference_setting")]
    [Description("The permission for video conference setting, including `only_owner` and `all_members`.")]
    public string? VideoConferencePermission { get; set; }

    /// <summary>
    /// Gets or sets the permission for adding member.
    /// <list type="bullet">
    /// <item>only_owner</item>
    /// <item>all_members</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("add_member_permission")]
    [Description("The permission for adding member, including `only_owner` and `all_members`.")]
    public string? AddMemberPermission { get; set; }

    /// <summary>
    /// Gets or sets the permission for sharing this group.
    /// <list type="bullet">
    /// <item>only_owner</item>
    /// <item>all_members</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("share_card_permission")]
    [Description("The permission for sharing this group, including `only_owner` and `all_members`.")]
    public string? SharePermission { get; set; }

    /// <summary>
    /// Gets or sets the permission for at (@, mentions) all members.
    /// <list type="bullet">
    /// <item>only_owner</item>
    /// <item>all_members</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("at_all_permission")]
    [Description("The permission for at (@, mentions) all members, including `only_owner` and `all_members`.")]
    public string? AtAllPermission { get; set; }

    /// <summary>
    /// Gets or sets the permission for edit message setting.
    /// <list type="bullet">
    /// <item>only_owner</item>
    /// <item>all_members</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("edit_permission")]
    [Description("The permission for edit message setting, including `only_owner` and `all_members`.")]
    public string? EditMessagePermission { get; set; }

    /// <summary>
    /// Gets or sets the mode of message sending in group.
    /// <list type="bullet">
    /// <item>chat</item>
    /// <item>thread</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("group_message_type")]
    [Description("The mode of message sending in group, including `chat` and `thread`.")]
    public string? MessageMode { get; set; }

    /// <summary>
    /// Gets or sets the visibility of the message group.
    /// <list type="bullet">
    /// <item>private</item>
    /// <item>public</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("chat_type")]
    [Description("The visibility of the message group, including `private` and `public`.")]
    public string? Visibility { get; set; }

    /// <summary>
    /// Gets or sets the message group kind.
    /// <list type="bullet">
    /// <item>inner</item>
    /// <item>tenant</item>
    /// <item>department</item>
    /// <item>edu</item>
    /// <item>meeting</item>
    /// <item>customer_service</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("chat_tag")]
    [Description("The message group kind, including `inner`, `tenant`, `department`, `edu`, `meeting` and `customer_service`.")]
    public string? Kind { get; set; }

    /// <summary>
    /// Gets or sets the visibility of user join notification.
    /// <list type="bullet">
    /// <item>only_owner</item>
    /// <item>all_members</item>
    /// <item>not_anyone</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("join_message_visibility")]
    [Description("The visibility of user join notification, including `only_owner`, `all_members` and `not_anyone`.")]
    public string? JoinNotification { get; set; }

    /// <summary>
    /// Gets or sets the visibility of user leave notification.
    /// <list type="bullet">
    /// <item>only_owner</item>
    /// <item>all_members</item>
    /// <item>not_anyone</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("leave_message_visibility")]
    [Description("The visibility of user leave notification, including `only_owner`, `all_members` and `not_anyone`.")]
    public string? LeaveNotification { get; set; }

    /// <summary>
    /// Gets or sets the membership approval status.
    /// <list type="bullet">
    /// <item>no_approval_required</item>
    /// <item>approval_required</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("membership_approval")]
    [Description("The membership approval status, including `no_approval_required` and `approval_required`.")]
    public string? MembershipApproval { get; set; }

    /// <summary>
    /// Gets or sets the permission for moderation (sending message)g.
    /// <list type="bullet">
    /// <item>only_owner</item>
    /// <item>moderator_list</item>
    /// <item>all_members</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("moderation_permission")]
    [Description("The permission for moderation (sending message), including `only_owner`, `moderator_list` and `all_members`.")]
    public string? ModerationPermission { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("restricted_mode_setting")]
    public LarkMessageGroupRestrictInfo? RestrictSettings { get; set; }

    /// <summary>
    /// Gets or sets the permission for hide member count setting.
    /// <list type="bullet">
    /// <item>only_owner</item>
    /// <item>all_members</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("hide_member_count_setting")]
    [Description("The permission for hide member count setting, including `only_owner` and `all_members`.")]
    public string? HideMemberCountPermission { get; set; }
}
