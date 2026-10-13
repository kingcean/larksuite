using LarkSuite.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Trivial.Data;
using Trivial.Net;
using Trivial.Security;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.OapiModels;

public class LarkWikiSpaceInfo : IIdPropertyModel, INamePropertyModel
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

public class LarkDocsNodeInfo : INamePropertyModel
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
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
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
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    public DateTime DocCreationTime { get; set; }

    [JsonPropertyName("obj_edit_time")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
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

public class LarkDocsDocInfo : IIdPropertyModel, INamePropertyModel
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

    /// <inheritdoc />
    public override string ToString()
        => $"Image Token = {ImageToken} & X = {OffsetRatioX} & Y = {OffsetRatioY}";
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

    /// <inheritdoc />
    public override string ToString()
        => $"Block = {BlockId} & Image = {ImageUrl}";
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

public abstract class BaseLarkDocsDriveMetaInfo : IIdPropertyModel
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

public class LarkDocsFolderMetaInfo : BaseLarkDocsDriveMetaInfo, INamePropertyModel
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

public class LarkDocsDriveNodeInfo : INamePropertyModel
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
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    public DateTime CreationDate { get; set; }

    [JsonPropertyName("modified_time")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
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

public class LarkDocsUserSearchItemInfo
{
    /// <summary>
    /// Gets or sets the node token.
    /// </summary>
    [JsonPropertyName("node_id")]
    public string NodeToken { get; set; }

    /// <summary>
    /// Gets or sets the space identifier.
    /// </summary>
    [JsonPropertyName("space_id")]
    public string SpaceId { get; set; }

    /// <summary>
    /// Gets or sets the doc type.
    /// </summary>
    [JsonPropertyName("obj_type")]
    public string DocType { get; set; }

    /// <summary>
    /// Gets or sets the doc token.
    /// </summary>
    [JsonPropertyName("obj_token")]
    public string DocToken { get; set; }

    /// <summary>
    /// Gets or sets the parent node identifier.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public string Parent { get; set; }

    /// <summary>
    /// Gets or sets the index of the item in the search result. Start from one (1).
    /// </summary>
    [JsonPropertyName("sort_id")]
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the icon URL.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }
}

public class LarkDocsSearchItemMeta
{
    [JsonPropertyName("doc_types")]
    public string DocType { get; set; }

    [JsonConverter(typeof(JsonUnixTimestampConverter))]
    [JsonPropertyName("update_time")]
    public DateTime ModificationDate { get; set; }

    [JsonPropertyName("url")]
    public string URL { get; set; }

    [JsonPropertyName("owner_name")]
    public string OwnerName { get; set; }

    [JsonPropertyName("owner_id")]
    public string OwnerId { get; set; }

    [JsonPropertyName("is_cross_tenant")]
    public bool IsCrossTenant { get; set; }

    [JsonConverter(typeof(JsonUnixTimestampConverter))]
    [JsonPropertyName("create_time")]
    public DateTime CreationDate { get; set; }

    [JsonConverter(typeof(JsonUnixTimestampConverter))]
    [JsonPropertyName("last_open_time")]
    public bool LatestAccessDate { get; set; }

    [JsonPropertyName("edit_user_id")]
    public string ModificationUserId { get; set; }

    [JsonPropertyName("edit_user_name")]
    public string ModificationUserName { get; set; }

    [JsonPropertyName("token")]
    public string DocToken { get; set; }

    [JsonPropertyName("file_type")]
    public string FileType { get; set; }

    [JsonPropertyName("icon_info")]
    public string Icon { get; set; }
}

public class LarkDocsSearchItemInfo
{
    /// <summary>
    /// Gets or sets the title with highlighted keywords.
    /// </summary>
    [JsonPropertyName("title_highlighted")]
    public string TitleHighlighted { get; set; }

    /// <summary>
    /// Gets or sets the highlighted summary.
    /// </summary>
    [JsonPropertyName("summary_highlighted")]
    public string SummaryHighlighted { get; set; }

    /// <summary>
    /// Gets or sets the entity type.
    /// <list type="bullet">
    /// <item>DOC</item>
    /// <item>WIKI</item>
    /// </list>
    /// </summary>
    [JsonPropertyName("entity_type")]
    public string EntityType { get; set; }

    /// <summary>
    /// Gets or sets the metadata.
    /// </summary>
    [JsonPropertyName("result_meta")]
    public LarkDocsSearchItemMeta Info { get; set; }
}

/// <summary>
/// The result of searching docs.
/// </summary>
public class LarkDocsSearchResult
{
    /// <summary>
    /// Gets the result collection.
    /// </summary>
    [JsonPropertyName("res_units")]
    public IReadOnlyList<LarkDocsSearchItemInfo> Data { get; }

    /// <summary>
    /// Gets the count of items.
    /// </summary>
    [JsonIgnore]
    public int Count => Data.Count;

    /// <summary>
    /// Gets the token used to get next page.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("pageToken")]
    public string? PageToken { get; private set; }

    /// <summary>
    /// Gets a value indicating whether has next page.
    /// </summary>
    [JsonPropertyName("has_more")]
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Gets the total count; or null, if not provided.
    /// </summary>
    [JsonPropertyName("total")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TotalCount { get; set; }
}
