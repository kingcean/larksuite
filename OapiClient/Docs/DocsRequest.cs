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

/// <summary>
/// The request options of lark wiki nodes.
/// </summary>
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

/// <summary>
/// The node search options of wiki space.
/// </summary>
[Description("The node search options of wiki space.")]
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        base.OnQueryDataFill(q);
        if (IgnoreConsistencyCheck.HasValue) q["ignore_consistency_check"] = IgnoreConsistencyCheck.Value ? JsonBooleanNode.TrueString : JsonBooleanNode.FalseString;
    }
}

/// <summary>
/// The simple filter of Lark Base.
/// </summary>
[Description("The filter and sort options.")]
public class LarkBaseTableSimpleFilter
{
    /// <summary>
    /// Gets or sets the optional property name used to filter.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("filterPropName")]
    [Description("The optional property name used to filter.")]
    public string? FilterPropertyName { get; set; }

    /// <summary>
    /// Gets or sets the optional property value used to filter.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("filterPropValue")]
    [Description("The optional property value used to filter.")]
    public string? FilterPropertyValue { get; set; }

    /// <summary>
    /// <para>
    /// Gets or sets the optional filter operator.
    /// </para>
    /// <list type="bullet">
    /// <item>"is" means to equal the filter property value;</item>
    /// <item>"isNot" means not to equal;</item>
    /// <item>"contains" means to include the filter property value as substring;</item>
    /// <item>"doesNotContain" means not include;</item>
    /// <item>"isEmpty" means the property should be empty;</item>
    /// <item>"isNotEmpty" means should not be empty.</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("filterOperator")]
    [Description("The optional filter operator: `is` means to equal the filter property value; `isNot` means not to equal; `contains` means to include the filter property value as substring; `doesNotContain` means not include; `isEmpty` means the property should be empty; `isNotEmpty` means should not be empty.")]
    public string? FilterOperator { get; set; }

    /// <summary>
    /// Gets or sets the optional property name used to sort.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("sortPropertyName")]
    [Description("The optional property name used to sort.")]
    public string? SortPropertyName { get; set; }

    /// <summary>
    /// Gets or sets the optional value indicating whether need order by desc (available only when sort property name is given).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("sortDesc")]
    [Description("The optional value indicating whether need order by desc (available only when sort property name is given).")]
    public bool SortByDesc { get; set; }
}

/// <summary>
/// The comment list options of docs.
/// </summary>
public class LarkDocsCommentListOptions : LarkUserIdTypeRequestOptions
{
    /// <summary>
    /// Gets or sets the doc token.
    /// </summary>
    public string DocToken { get; set; }

    /// <summary>
    /// Gets or sets the doc type.
    /// </summary>
    public string DocType { get; set; }

    public bool IsWhole { get; set; }

    public bool IsSolved { get; set; }

    public bool NeedReaction { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        base.OnQueryDataFill(q);
        q["file_type"] = DocType ?? "docx";
        if (IsWhole) q["is_whole"] = JsonBooleanNode.TrueString;
        if (IsSolved) q["is_solved"] = JsonBooleanNode.TrueString;
        if (NeedReaction) q["need_reaction"] = JsonBooleanNode.TrueString;
    }
}

public class LarkDocsCommentReplyOptions : LarkUserIdTypeRequestOptions, IJsonObjectHost
{
    private readonly List<JsonObjectNode> content = new();

    /// <summary>
    /// Initializes a new instance of the LarkDocsCommentReplyOptions class.
    /// </summary>
    public LarkDocsCommentReplyOptions()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkDocsCommentReplyOptions class.
    /// </summary>
    /// <param name="docType">The doc type.</param>
    /// <param name="docToken">The doc token.</param>
    /// <param name="commentId">The comment identifier.</param>
    public LarkDocsCommentReplyOptions(string docType, string docToken, string commentId)
    {
        DocToken = docToken;
        CommentId = commentId;
        DocType = docType;
    }

    /// <summary>
    /// Gets or sets the doc type.
    /// </summary>
    public string DocToken { get; set; }

    /// <summary>
    /// Gets or sets the comment identifier.
    /// </summary>
    public string CommentId { get; set; }

    /// <summary>
    /// Gets or sets the doc type.
    /// </summary>
    public string DocType { get; set; }

    /// <summary>
    /// Adds the comment content.
    /// </summary>
    /// <param name="type">The content type: text_run, docs_link, person.</param>
    /// <param name="value">The content.</param>
    public void AddContent(string type, string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        switch (type ?? "text_run")
        {
            case "text_run":
            case "text":
            case "string":
            case "":
                content.Add(new()
                {
                    { "type", "text_run" },
                    { "text_run", new JsonObjectNode
                    {
                        { "text", value }
                    }
                    }
                });
                break;
            case "docs_link":
            case "docs":
            case "link":
            case "url":
                content.Add(new()
                {
                    { "type", "docs_link" },
                    { "docs_link", new JsonObjectNode
                    {
                        { "url", value }
                    }
                    }
                });
                break;
            case "person":
            case "user":
            case "at":
            case "@":
                content.Add(new()
                {
                    { "type", "person" },
                    { "person", new JsonObjectNode
                    {
                        { "user_id", value }
                    }
                    }
                });
                break;
        }
    }

    /// <summary>
    /// Clears the comment content.
    /// </summary>
    public void ClearContent()
        => content.Clear();

    /// <inheritdoc />
    public JsonObjectNode ToJson()
    {
        var arr = new JsonArrayNode();
        arr.AddRange(content);
        return new JsonObjectNode
        {
            { "content", new JsonObjectNode
            {
                { "elements", arr },
            }
            },
        };
    }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        base.OnQueryDataFill(q);
        q["file_type"] = DocType;
    }
}

public class LarkDocsDriveFilesRequest : LarkUserIdTypeRequestOptions
{
    public string? Token { get; set; }

    public string? OrderBy { get; set; }

    public bool OrderByDesc { get; set; }

    public void SetOrder(string name, bool desc = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            OrderBy = null;
            OrderByDesc = false;
        }
        else
        {
            OrderBy = name;
            OrderByDesc = desc;
        }
    }

    protected override void OnQueryDataFill(QueryData q)
    {
        base.OnQueryDataFill(q);
        q.SetIfNotEmpty("folder_token", Token);
        q.SetIfNotEmpty("order_by", OrderBy);
        q["direction"] = OrderByDesc ? "DESC" : "ASC";
    }
}

public class LarkDocsDriveFileMoveRequest
{
    [JsonIgnore]
    public string Token { get; set; }

    [JsonPropertyName("type")]
    public string NodeType { get; set; }

    [JsonPropertyName("folder_token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string DestinationToken { get; set; }
}

public class LarkDocsDriveFileMoveToWikiRequest
{
    [JsonIgnore]
    public string SpaceId { get; set; }

    [JsonPropertyName("obj_token")]
    public string DocToken { get; set; }

    [JsonPropertyName("obj_type")]
    public string DocType { get; set; }

    [JsonPropertyName("parent_wiki_token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string DestinationParentToken { get; set; }

    [JsonPropertyName("apply")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string ApplyPermission { get; set; }
}

public class LarkDocsNodeMoveRequest
{
    [JsonIgnore]
    public string SourceSpaceId { get; set; }

    [JsonIgnore]
    public string SourceToken { get; set; }

    [JsonPropertyName("target_space_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DestinationSpaceId { get; set; }

    [JsonPropertyName("target_parent_token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DestinationToken { get; set; }
}
