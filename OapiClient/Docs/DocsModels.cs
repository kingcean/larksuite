using LarkSuite.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Net;
using Trivial.Security;
using Trivial.Text;

namespace LarkSuite.OapiModels;

public class LarkWikiNodesRequestOptions : BaseQueryRequestInfo
{
    /// <summary>
    /// Gets or sets the space ID.
    /// </summary>
    public string SpaceId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of top node doc if limit searching in such scope.
    /// </summary>
    public string? ParentNodeToken { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        q.SetIfNotEmpty("parent_node_token", ParentNodeToken);
    }
}

public class LarkWikiSearchOptions : LarkPageTokenInfo, IJsonObjectHost
{
    /// <summary>
    /// The query string (keyword) to search.
    /// </summary>
    [JsonPropertyName("query")]
    public string Query { get; set; }

    /// <summary>
    /// Gets or sets the identifier of wiki space if limit searching in such scope.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("space_id")]
    public string? SpaceId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of top node doc if limit searching in such scope.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("node_id")]
    public string? ParentNodeId { get; set; }

    /// <inheritdoc />
    public JsonObjectNode ToJson()
    {
        var json = new JsonObjectNode
        {
            { "query", Query }
        };
        json.SetValueIfNotEmpty("space_id", SpaceId);
        json.SetValueIfNotEmpty("node_id", ParentNodeId);
        return json;
    }
}

public class LarkWikiDocMarkdownOptions : LarkResourceRequestOptions
{
    public string Id { get; set; }

    protected override void OnQueryDataFill(QueryData q)
    {
        base.OnQueryDataFill(q);
        q["doc_token"] = Id;
        q["doc_type"] = "docx";
        q["content_type"] = "markdown";
    }
}

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
}

public class LarkDocsDocInfo
{
    [JsonPropertyName("display_settings")]
    public LarkDocsDocDisplaySettings? DisplaySettings { get; set; }

    [JsonPropertyName("title")]
    public string Name { get; set; }

    [JsonPropertyName("document_id")]
    public string Id { get; set; }

    [JsonPropertyName("revision_id")]
    public int Revision { get; set; }
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

public class LarkDocsBaseTableInfo
{
    [JsonPropertyName("app_token")]
    public string Token { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("revision")]
    public int Revision { get; set; }

    [JsonPropertyName("is_advanced")]
    public bool HasAdvancedPermission { get; set; }

    [JsonPropertyName("time_zone")]
    public string TimeZone { get; set; }

    [JsonPropertyName("formula_type")]
    public int FormulaType { get; set; }

    [JsonPropertyName("advance_version")]
    public string AdvanceVersion { get; set; }
}

public class LarkDocsBaseTableTableInfo
{
    [JsonPropertyName("table_id")]
    public string Id { get; set; }

    [JsonPropertyName("revision")]
    public int Revision { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
