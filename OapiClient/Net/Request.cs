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
    Defaut = 0,

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
/// The filter of Lark Base table.
/// </summary>
public class LarkBaseFilter : List<LarkBaseFilterCondition>
{
    /// <summary>
    /// Initializes a new instance of the LarkBaseFilter class.
    /// </summary>
    /// <param name="conjunction"></param>
    /// <param name="conditions"></param>
    public LarkBaseFilter(CriteriaBooleanOperator conjunction, params IEnumerable<LarkBaseFilterCondition> conditions)
    {
        Conjunction = conjunction;
        if (conditions is not null) AddRange(conditions);
    }

    /// <summary>
    /// Gets or sets the conjunction operator to combine the filter conditions.
    /// </summary>
    public CriteriaBooleanOperator Conjunction { get; set; }
}

/// <summary>
/// The filter condition of Lark Base table.
/// </summary>
public class LarkBaseFilterCondition
{
    /// <summary>
    /// Initializes a new instance of the LarkBaseFilterCondition class.
    /// </summary>
    public LarkBaseFilterCondition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkBaseFilterCondition class.
    /// </summary>
    /// <param name="name">The name of field.</param>
    /// <param name="op">The operation.</param>
    /// <param name="value">The value of field.</param>
    public LarkBaseFilterCondition(string name, string op, string value)
        : this(name, op, [value])
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkBaseFilterCondition class.
    /// </summary>
    /// <param name="name">The name of field.</param>
    /// <param name="op">The operation.</param>
    /// <param name="value">The value of field.</param>
    public LarkBaseFilterCondition(string name, string op, List<string> value)
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

internal class LarkResourceIdRequest : BaseQueryRequestInfo
{
    public LarkResourceIdRequest(string id, string? text = null)
    {
        Id = id;
        Text = text;
    }

    public string Id { get; set; }

    public string? Text { get; set; }

    protected override void OnQueryDataFill(QueryData q)
    {
    }
}
