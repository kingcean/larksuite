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

public static class LarkDocsFieldsHelper
{
    public static void SetUser(JsonObjectNode node, string key, string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        node.SetValue(key, new JsonArrayNode
        {
            new JsonObjectNode
            {
                { "id", id },
            }
        });
    }

    public static void SetUser(JsonObjectNode node, string key, IEnumerable<string> id)
    {
        if (id is null) return;
        var arr = new JsonArrayNode();
        arr.AddRange(id.Where(ele => !string.IsNullOrWhiteSpace(ele)).Select(ele => new JsonObjectNode
        {
            { "id", ele },
        }));
        node.SetValue(key, arr);
    }

    public static void SetDate(JsonObjectNode node, string key, DateTime value)
        => node.SetValue(key, WebFormat.ParseDate(value));

    public static void SetDate(JsonObjectNode node, string key, DateTime? value)
    {
        if (value.HasValue) SetDate(node, key, value.Value);
    }

    public static void SetLink(JsonObjectNode node, string key, string url, string? title = null)
    {
        if (string.IsNullOrWhiteSpace(url)) return;
        var obj = new JsonObjectNode
        {
            { "link", url }
        };
        if (!string.IsNullOrWhiteSpace(title)) obj.SetValue("text", title);
        node.SetValue(key, obj);
    }

    public static void SetLink(JsonObjectNode node, string key, Uri uri, string? title = null)
    {
        if (uri is null) return;
        var obj = new JsonObjectNode
        {
            { "link", uri?.OriginalString }
        };
        if (!string.IsNullOrWhiteSpace(title)) obj.SetValue("text", title);
        node.SetValue(key, obj);
    }

    public static void SetFile(JsonObjectNode node, string key, string fileToken)
    {
        if (string.IsNullOrWhiteSpace(fileToken)) return;
        node.SetValue(key, new JsonArrayNode
        {
            new JsonObjectNode
            {
                { "file_token", fileToken },
            }
        });
    }

    public static void SetFile(JsonObjectNode node, string key, IEnumerable<string> fileToken)
    {
        if (fileToken is null) return;
        var arr = fileToken.Where(ele => !string.IsNullOrWhiteSpace(ele)).Select(ele => new JsonObjectNode
        {
            { "file_token", fileToken },
        });
        node.SetValue(key, arr);
    }
}
