using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Maths;
using Trivial.Net;
using Trivial.Text;

namespace LarkSuite.OapiModels;

/// <summary>
/// The user identifier type.
/// </summary>
public enum LarkUserIdType
{
    /// <summary>
    /// The default value, which is the same as OpenId.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The ID of department in app scope. For the same department, this is different for the different app.
    /// </summary>
    OpenId = 1,

    /// <summary>
    /// The ID of department in app publisher scope. For the same department, this is same for the different app of the same publisher, but is different between different publishers.
    /// </summary>
    UnionId = 2,

    /// <summary>
    /// The unique ID of department. It keeps the same for different apps.
    /// </summary>
    UniqueId = 3,
}

/// <summary>
/// THe department identifier type.
/// </summary>
public enum LarkDepartmentIdType
{
    /// <summary>
    /// The default value, which is the same as OpenId.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The ID of department in app scope. For the same department, this is different for the different app.
    /// </summary>
    OpenId = 1,

    /// <summary>
    /// The unique ID of department. It keeps the same for different apps.
    /// </summary>
    UniqueId = 3,
}

/// <summary>
/// The request info with paging.
/// </summary>
public class LarkPageTokenInfo : BaseQueryRequestInfo
{
    /// <summary>
    /// Initializes a new instance of the LarkPageTokenInfo class.
    /// </summary>
    public LarkPageTokenInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkPageTokenInfo class.
    /// </summary>
    /// <param name="size">The page size.</param>
    /// <param name="token">The page token.</param>
    public LarkPageTokenInfo(int size, string? token = null)
    {
        Size = size;
        Token = token;
    }

    /// <summary>
    /// Initializes a new instance of the LarkPageTokenInfo class.
    /// </summary>
    /// <param name="token">The page token.</param>
    public LarkPageTokenInfo(string? token)
    {
        Token = token;
    }

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int? Size { get; set; }

    /// <summary>
    /// Gets or sets the last page token used to identify the next page.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Sets next page token.
    /// </summary>
    /// <param name="response">The current response body.</param>
    /// <returns>true if has next page; otherwise, false.</returns>
    public bool SetPageToken(LarkResponsePagingBody response)
    {
        if (response is null || response.PageToken == Token || !response.HasNextPage) return false;
        Token = response.PageToken;
        return !string.IsNullOrWhiteSpace(Token);
    }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        if (Size.HasValue && Size.Value > 0) q.Add("page_size", Size.Value);
        q.SetIfNotEmpty("page_token", Token);
    }
}

/// <summary>
/// The request info with paging.
/// </summary>
public class LarkSearchOptions : LarkPageTokenInfo
{
    /// <summary>
    /// Gets or sets the last page token used to identify the next page.
    /// </summary>
    public string? Query { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        base.OnQueryDataFill(q);
        q.SetIfNotEmpty("query", Query);
    }
}

/// <summary>
/// The request options to resolve resource.
/// </summary>
public class LarkResourceRequestOptions : BaseQueryRequestInfo
{
    /// <summary>
    /// Gets or sets the language.
    /// </summary>
    public string? Language { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        q.SetIfNotEmpty("lang", Language);
    }
}

/// <summary>
/// The sort options.
/// </summary>
public class LarkDocsSortItem : IJsonObjectHost
{
    /// <summary>
    /// Initializes a new instance of the LarkDocsSortItem class.
    /// </summary>
    public LarkDocsSortItem()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkDocsSortItem class.
    /// </summary>
    /// <param name="name">The field name.</param>
    /// <param name="isDesc">true if ordered by desc; otherwise, false. The default is false, to by asc.</param>
    public LarkDocsSortItem(string name, bool isDesc = false)
    {
        Name = name;
        IsDesc = isDesc;
    }

    /// <summary>
    /// Gets or sets the fieled name.
    /// </summary>
    [JsonPropertyName("field_name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets if orders by desc. The default is asc.
    /// </summary>
    [JsonPropertyName("desc")]
    public bool IsDesc { get; set; }

    /// <inheritdoc />
    public JsonObjectNode? ToJson()
    {
        if (string.IsNullOrWhiteSpace(Name)) return null;
        return new()
        {
            { "field_name", Name },
            { "desc", IsDesc },
        };
    }
}

public abstract class BaseLarkDocsFilter : IJsonObjectHost
{
    internal BaseLarkDocsFilter()
    {
    }

    /// <inheritdoc />
    public abstract JsonObjectNode? ToJson();
}

/// <summary>
/// The filter of Lark Base table.
/// </summary>
public class LarkDocsFilter : BaseLarkDocsFilter
{
    private readonly List<BaseLarkDocsFilter> list = [];

    /// <summary>
    /// Initializes a new instance of the LarkDocsFilter class.
    /// </summary>
    /// <param name="conjunction"></param>
    /// <param name="conditions"></param>
    public LarkDocsFilter(CriteriaBooleanOperator conjunction, params IEnumerable<LarkDocsFilterCondition> conditions)
    {
        Conjunction = conjunction;
        if (conditions is not null) list.AddRange(conditions);
    }

    /// <summary>
    /// Gets or sets the conjunction operator to combine the filter conditions.
    /// </summary>
    public CriteriaBooleanOperator Conjunction { get; set; }

    public void Add(string name, string op, string value)
        => list.Add(new LarkDocsFilterCondition(name, op, value));

    public void Add(string name, string op, List<string> value)
        => list.Add(new LarkDocsFilterCondition(name, op, value));

    public void Add(LarkDocsFilterCondition item)
        => list.Add(item);

    public void Add(LarkDocsFilter item)
        => list.Add(item);

    /// <inheritdoc />
    public override JsonObjectNode? ToJson()
    {
        var arr = new JsonArrayNode();
        foreach (var item in list)
        {
            var json = item?.ToJson();
            if (json is null) continue;
            arr.Add(json);
        }

        return arr.Count > 0 ? new()
        {
            { "conjunction", Conjunction.ToString().ToLowerInvariant() },
            { "conditions", arr },
        } : null;
    }
}

/// <summary>
/// The filter condition of Lark Base table.
/// </summary>
public class LarkDocsFilterCondition: BaseLarkDocsFilter
{
    /// <summary>
    /// Initializes a new instance of the LarkDocsFilterCondition class.
    /// </summary>
    public LarkDocsFilterCondition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkDocsFilterCondition class.
    /// </summary>
    /// <param name="name">The name of field.</param>
    /// <param name="op">The operation.</param>
    /// <param name="value">The value of field.</param>
    public LarkDocsFilterCondition(string name, string op, string? value)
        : this(name, op, value is null ? [] : [value])
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkDocsFilterCondition class.
    /// </summary>
    /// <param name="name">The name of field.</param>
    /// <param name="op">The operation.</param>
    /// <param name="value">The value of field.</param>
    public LarkDocsFilterCondition(string name, string op, List<string> value)
    {
        Name = name;
        Operation = op;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the name of the field to filter.
    /// </summary>
    [JsonPropertyName("field_name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the operation.
    /// </summary>
    [JsonPropertyName("operator")]
    public string Operation { get; set; }

    /// <summary>
    /// Gets or sets the value of the field to filter.
    /// </summary>
    [JsonPropertyName("value")]
    public List<string> Value { get; set; }

    /// <inheritdoc />
    public override JsonObjectNode? ToJson()
    {
        if (string.IsNullOrWhiteSpace(Name)) return null;
        return new()
        {
            { "field_name", Name },
            { "operator", Operation },
            { "value", Value },
        };
    }
}

/// <summary>
/// The request options with user identifier to resolve resources.
/// </summary>
public class LarkUserIdTypeRequestOptions : BaseQueryRequestInfo
{
    /// <summary>
    /// Gets or sets type of user identifier.
    /// </summary>
    [JsonPropertyName("user_id_type")]
    public LarkUserIdType UserIdType { get; set; }

    /// <summary>
    /// Gets or sets type of department identifier.
    /// </summary>
    [JsonPropertyName("department_id_type")]
    public LarkDepartmentIdType DepartmentIdType { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        switch (UserIdType)
        {
            case LarkUserIdType.OpenId:
                q.SetIfNotEmpty("user_id_type", "open_id");
                break;
            case LarkUserIdType.UnionId:
                q.SetIfNotEmpty("user_id_type", "union_id");
                break;
            case LarkUserIdType.UniqueId:
                q.SetIfNotEmpty("user_id_type", "user_id");
                break;
        }

        switch (DepartmentIdType)
        {
            case LarkDepartmentIdType.OpenId:
                q.SetIfNotEmpty("department_id_type", "open_department_id");
                break;
            case LarkDepartmentIdType.UniqueId:
                q.SetIfNotEmpty("department_id_type", "department_id");
                break;
        }
    }
}

/// <summary>
/// The request options with user identifer to get the related resources.
/// </summary>
public class LarkUserOwnedResourcesRequest : LarkUserIdTypeRequestOptions
{
    /// <summary>
    /// Initializes a new instance of the LarkUserOwnedResourcesRequest class.
    /// </summary>
    public LarkUserOwnedResourcesRequest()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkUserOwnedResourcesRequest class.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    public LarkUserOwnedResourcesRequest(string userId)
    {
        UserId = userId;
    }

    /// <summary>
    /// Initializes a new instance of the LarkUserOwnedResourcesRequest class.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="userIdType">The type of user identifier.</param>
    public LarkUserOwnedResourcesRequest(string userId, LarkUserIdType userIdType)
        : this(userId)
    {
        UserIdType = userIdType;
    }

    /// <summary>
    /// Gets or sets the user identifer.
    /// </summary>
    public string UserId { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        base.OnQueryDataFill(q);
        q["user_id"] = UserId;
    }
}

/// <summary>
/// The base request options to resolve resource by identifier.
/// </summary>
internal class LarkResourceIdRequest : BaseQueryRequestInfo
{
    /// <summary>
    /// Intiailizes a new instance of the LarkResourceIdRequest class.
    /// </summary>
    /// <param name="id">The resource identifier.</param>
    /// <param name="text">The additional text.</param>
    public LarkResourceIdRequest(string id, string? text = null)
    {
        Id = id;
        Text = text;
    }

    /// <summary>
    /// Gets or sets the resource identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the additional text.
    /// </summary>
    public string? Text { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
    }
}

/// <summary>
/// The base request options to resolve resources by identifier.
/// </summary>
internal class LarkTargetResourcesRequest : LarkUserIdTypeRequestOptions
{
    /// <summary>
    /// Intiailizes a new instance of the LarkResourceIdRequest class.
    /// </summary>
    /// <param name="id">The resource identifier.</param>
    /// <param name="text">The additional text.</param>
    public LarkTargetResourcesRequest(string id, string? text = null)
    {
        Id = id;
        Text = text;
    }

    /// <summary>
    /// Intiailizes a new instance of the LarkResourceIdRequest class.
    /// </summary>
    /// <param name="options">The base options.</param>
    /// <param name="id">The resource identifier.</param>
    /// <param name="text">The additional text.</param>
    public LarkTargetResourcesRequest(LarkUserIdTypeRequestOptions options, string id, string? text = null)
        : this(id, text)
    {
        if (options is null) return;
        UserIdType = options.UserIdType;
        DepartmentIdType = options.DepartmentIdType;
    }

    /// <summary>
    /// Gets or sets the resource identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the additional text.
    /// </summary>
    public string? Text { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        base.OnQueryDataFill(q);
    }
}

internal class QueryDataContainer : BaseQueryRequestInfo
{
    public QueryDataContainer(QueryData q)
    {
        Query = q;
    }

    public QueryData Query { get; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        foreach (var prop in Query)
        {
            Query.Add(prop.Key, prop.Value);
        }
    }
}
