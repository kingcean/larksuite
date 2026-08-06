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

public class LarkWikiNodesCreateRequestOptions
{
    [JsonIgnore]
    public string SpaceId { get; set; }

    [JsonPropertyName("obj_type")]
    public string DocType { get; set; } = "docx";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("parent_node_token")]
    public string? ParentNodeToken { get; set; }

    [JsonPropertyName("node_type")]
    public string NodeType { get; set; } = "origin";

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("origin_node_token")]
    public string? OriginNodeToken { get; set; }
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

public class LarkDocsBaseTableRecordOptions : LarkUserIdTypeRequestOptions
{
    public bool? IgnoreConsistencyCheck { get; set; }

    protected override void OnQueryDataFill(QueryData q)
    {
        base.OnQueryDataFill(q);
        if (IgnoreConsistencyCheck.HasValue) q["ignore_consistency_check"] = IgnoreConsistencyCheck.Value ? JsonBooleanNode.TrueString : JsonBooleanNode.FalseString;
    }
}

public class LarkDocsCommentListOptions : LarkUserIdTypeRequestOptions
{
    public string DocToken { get; set; }

    public string DocType { get; set; }

    public bool IsWhole { get; set; }

    public bool IsSolved { get; set; }

    public bool NeedReaction { get; set; }

    protected override void OnQueryDataFill(QueryData q)
    {
        base.OnQueryDataFill(q);
        q["file_type"] = DocType ?? "docx";
        if (IsWhole) q["is_whole"] = JsonBooleanNode.TrueString;
        if (IsSolved) q["is_solved"] = JsonBooleanNode.TrueString;
        if (NeedReaction) q["need_reaction"] = JsonBooleanNode.TrueString;
    }
}
