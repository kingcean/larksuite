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
