using LarkSuite.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Trivial.Net;
using Trivial.Security;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.OapiModels;

public class LarkWikiSpaceInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("space_id")]
    public string Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("space_type")]
    public string SpaceType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("visibility")]
    public string Visibility { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("open_sharing")]
    public string ShareState { get; set; }

    public static LarkWikiSpaceInfo Convert(JsonObjectNode json)
        => (json.TryGetObjectValue("space") ?? json)?.TryConvert<LarkWikiSpaceInfo>();

    /// <inheritdoc />
    public override string ToString()
        => $"{Name ?? "?"} (Space ID = {Id} & Type = {SpaceType})";
}

public class LarkDocsNodeInfo
{
    [JsonPropertyName("title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Name { get; set; }

    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Url { get; set; }

    [JsonPropertyName("creator")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string CreatorId { get; set; }

    [JsonPropertyName("owner")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string OwnerId { get; set; }

    [JsonPropertyName("has_child")]
    public bool HasChild { get; set; }

    [JsonPropertyName("node_create_time")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime NodeCreationTime { get; set; }

    [JsonPropertyName("node_token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string NodeToken { get; set; }

    [JsonPropertyName("node_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string NodeType { get; set; }

    [JsonPropertyName("space_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string SpaceId { get; set; }

    [JsonPropertyName("obj_create_time")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime DocCreationTime { get; set; }

    [JsonPropertyName("obj_edit_time")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime DocLastModificationTime { get; set; }

    [JsonPropertyName("obj_token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string DocToken { get; set; }

    [JsonPropertyName("obj_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string DocType { get; set; }

    [JsonPropertyName("origin_node_token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string OriginNodeToken { get; set; }

    [JsonPropertyName("origin_space_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string OriginSpaceId { get; set; }

    [JsonPropertyName("parent_node_token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string ParentNodeToken { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{Name ?? "?"} (Doc Type = {DocType} & Node Token = {NodeToken} & Doc Token = {DocToken} & Space ID = {SpaceId} & {(HasChild ? "Has Child" : "No Child")})";
}

public class LarkDocsDocInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("display_settings")]
    public LarkDocsDocDisplaySettings? DisplaySettings { get; set; }

    [JsonPropertyName("title")]
    public string Name { get; set; }

    [JsonPropertyName("document_id")]
    public string Id { get; set; }

    [JsonPropertyName("revision_id")]
    public int Revision { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cover")]
    public LarkDocsDocCoverInfo Cover { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{Name ?? "?"} (Doc ID = {Id} & Rev = {Revision})";
}

public class LarkDocsDocDisplaySettings
{
    [JsonPropertyName("show_authors")]
    public bool Authors { get; set; }

    [JsonPropertyName("show_comment_count")]
    public bool CommentCount { get; set; }

    [JsonPropertyName("show_create_time")]
    public bool CreateTime { get; set; }

    [JsonPropertyName("show_like_count")]
    public bool LikeCount { get; set; }

    [JsonPropertyName("show_pv")]
    public bool PV { get; set; }

    [JsonPropertyName("show_related_matters")]
    public bool RelatedMatters { get; set; }

    [JsonPropertyName("show_uv")]
    public bool UV { get; set; }
}

public class LarkDocsDocCoverInfo
{
    [JsonPropertyName("token")]
    public string ImageToken { get; set; }

    [JsonPropertyName("offset_ratio_x")]
    public double OffsetRatioX { get; set; }

    [JsonPropertyName("offset_ratio_y")]
    public double OffsetRatioY { get; set; }
}

public class LarkDocsFileTextResponse(LarkDocsNodeInfo node, string value)
{
    public LarkDocsNodeInfo Node { get; } = node;

    public string Value { get; } = value;
}

public class LarkDocsImageUrlMappingResponse
{
    [JsonPropertyName("block_id")]
    public string BlockId { get; set; }

    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; }
}

public class LarkDocsMarkdownConvertResponse
{
    [JsonPropertyName("first_level_block_ids")]
    public List<string> TopBlockIds { get; set; }

    [JsonPropertyName("blocks")]
    public List<JsonObjectNode> Blocks { get; set; }

    [JsonPropertyName("block_id_to_image_urls")]
    public List<LarkDocsImageUrlMappingResponse> ImageMapping { get; set; }

}

public abstract class BaseLarkDocsDriveMetaInfo
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    public abstract string OwnerUserId { get; set; }
}

public class LarkDocsDriveMetaInfo : BaseLarkDocsDriveMetaInfo
{

    [JsonPropertyName("user_id")]
    public override string OwnerUserId { get; set; }
}

public class LarkDocsFolderMetaInfo : BaseLarkDocsDriveMetaInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("createUid")]
    public string CreationUserId { get; set; }

    [JsonPropertyName("editUid")]
    public string LastModificationUserId { get; set; }

    [JsonPropertyName("ownUid")]
    public override string OwnerUserId { get; set; }

    [JsonPropertyName("parentId")]
    public string ParentId { get; set; }

    public override string ToString()
        => $"{Name ?? "?"} ({Token})";
}

public class LarkDocsDriveShortcutNodeInfo
{
    [JsonPropertyName("target_token")]
    public string Token { get; set; }

    [JsonPropertyName("target_type")]
    public string DocType { get; set; }
}

public class LarkDocsDriveNodeInfo
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string DocType { get; set; }

    [JsonPropertyName("parent_token")]
    public string ParentToken { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("shortcut_info")]
    public LarkDocsDriveShortcutNodeInfo Shortcut { get; set; }

    [JsonPropertyName("created_time")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime CreationDate { get; set; }

    [JsonPropertyName("modified_time")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime LastModificationDate { get; set; }

    [JsonPropertyName("owner_id")]
    public string OwnerUserId { get; set; }

    public override string ToString()
    {
        var shortcut = Shortcut;
        if (string.IsNullOrWhiteSpace(shortcut?.Token))
            return $"{DocType} | {Name ?? "?"} ({Token})";
        else
            return $"{DocType} {shortcut.DocType} | {Name ?? "?"} ({Token} → {shortcut.Token})";
    }
}

public class LarkDocsDriveFileMoveTaskInfo : BaseLarkTaskInfo
{
    [JsonPropertyName("wiki_token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Token { get; set; }

    [JsonPropertyName("applied")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool ApplyPermission { get; set; }
}
