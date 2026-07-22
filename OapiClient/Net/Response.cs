using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Trivial.Net;
using Trivial.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Trivial.Reflection.ExceptionHandler;

namespace LarkSuite.OapiModels;

/// <summary>
/// The response body from Lark API.
/// </summary>
public class LarkResponseBody
{
    /// <summary>
    /// Initializes a new instance of the LarkResponseBody class.
    /// </summary>
    public LarkResponseBody()
        : this(true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponseBody class.
    /// </summary>
    /// <param name="isError">true if error; otherwise, false.</param>
    /// <param name="message">The message.</param>
    public LarkResponseBody(bool isError, string? message = null)
    {
        IsError = isError;
        Code = -1;
        Data = [];
        Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponseBody class.
    /// </summary>
    /// <param name="raw">The raw package col in JSON.</param>
    public LarkResponseBody(JsonObjectNode raw)
        : this(raw, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponseBody class.
    /// </summary>
    /// <param name="code">The response code.</param>
    /// <param name="message">The response message.</param>
    /// <param name="data">The data.</param>
    public LarkResponseBody(int code, string? message, JsonObjectNode data)
    {
        Code = code;
        Message = message;
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponseBody class.
    /// </summary>
    /// <param name="raw">The raw package col in JSON.</param>
    /// <param name="key">The property key to resolve result col.</param>
    public LarkResponseBody(JsonObjectNode raw, string? key)
    {
        if (raw is null)
        {
            Code = -1;
            Data = [];
            return;
        }

        Message = raw.TryGetStringTrimmedValue("msg");
        var data = raw.TryGetObjectValue("data");
        if (!string.IsNullOrWhiteSpace(key) && data is not null) data = data.TryGetObjectValue(key);
        Data = data ?? [];
        Code = raw.TryGetInt32Value("code") ?? (data is null ? -1 : 0);
    }

    /// <summary>
    /// Gets a value indicating whether the response is in error.
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// Gets the response message.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Gets the response code. 0 means success, others means failure.
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Gets the result col.
    /// </summary>
    public JsonObjectNode Data { get; }

    /// <summary>
    /// Gets a value indicating whether the message is null, empty or white space only.
    /// </summary>
    public bool IsMessageEmpty()
        => string.IsNullOrWhiteSpace(Message);

    /// <summary>
    /// Throws if the response is in error.
    /// </summary>
    /// <exception cref="InvalidOperationException">The response is in error.</exception>
    public void ThrowIfError()
    {
        if (!IsError) return;
        throw new InvalidOperationException(Message ?? "Unknown error.");
    }
}

/// <summary>
/// The response body from Lark API.
/// </summary>
public class LarkResponseBody<T> : LarkResponseBody
{
    /// <summary>
    /// Initializes a new instance of the LarkResponseBody class.
    /// </summary>
    public LarkResponseBody()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponseBody class.
    /// </summary>
    /// <param name="code">The response code.</param>
    /// <param name="message">The response message.</param>
    /// <param name="data">The data.</param>
    /// <param name="sourceJsonData">The source data in JSON.</param>
    public LarkResponseBody(int code, string? message, T data, JsonObjectNode sourceJsonData)
        : base(code, message, sourceJsonData)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponseBody class.
    /// </summary>
    /// <param name="isError">true if error; otherwise, false.</param>
    /// <param name="message">The message.</param>
    public LarkResponseBody(bool isError, string? message = null)
        : base(isError, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponseBody class.
    /// </summary>
    /// <param name="raw">The raw package col in JSON.</param>
    /// <param name="key">The property key to resolve result col.</param>
    public LarkResponseBody(JsonObjectNode raw, string key)
        : base(raw)
    {
        if (raw is null) return;
        var data = raw.TryGetObjectValue("data");
        if (data is null) return;
        Data = data.DeserializeValue<T>(key);
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponseBody class.
    /// </summary>
    /// <param name="raw">The raw package col in JSON.</param>
    /// <param name="converter">The converter of result col.</param>
    public LarkResponseBody(JsonObjectNode raw, Func<JsonObjectNode, T>? converter = null)
        : base(raw)
    {
        if (raw is null) return;
        var data = raw.TryGetObjectValue("data");
        if (data is null)
        {
        }
        else if (converter is not null)
        {
            Data = converter(data);
        }
        else if (typeof(T) == typeof(string))
        {
            var str = data.TryGetStringTrimmedValue("content");
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Contains(Environment.NewLine))
                    str = str.Replace(string.Concat(Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine), Environment.NewLine)
                        .Replace(string.Concat(Environment.NewLine, Environment.NewLine, Environment.NewLine), Environment.NewLine)
                        .Replace(string.Concat(Environment.NewLine, Environment.NewLine), Environment.NewLine);
                else if (str.Contains('\n'))
                    str = str.Replace("\n\n\n\n", Environment.NewLine)
                        .Replace("\n\n\n", Environment.NewLine)
                        .Replace("\n\n", Environment.NewLine);
                Data = (T)(object)str;
            }
        }
        else if (typeof(T) == typeof(JsonObjectNode) || typeof(T) == typeof(object))
        {
            Data = (T)(object)data;
        }
        else
        {
            Data = data.Deserialize<T>();
        }
    }

    /// <summary>
    /// Gets the result col.
    /// </summary>
    public new T? Data { get; }
}

/// <summary>
/// The status and additional information for each paging request.
/// </summary>
/// <param name="response">The response date time.</param>
/// <param name="message">The message.</param>
/// <param name="count">The item count.</param>
public class LarkResponsePagingStatusInfo(DateTime response, string? message, int count)
{
    /// <summary>
    /// Gets the request receiving date time.
    /// </summary>
    public DateTime ResponseTime { get; } = response;

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string? Message { get; } = message;

    /// <summary>
    /// Gets the count of item.
    /// </summary>
    public int Count { get; } = count;
}

/// <summary>
/// The response body with paging from Lark API.
/// </summary>
public class LarkResponsePagingBody : LarkResponseBody
{
    private readonly List<JsonObjectNode> col;
    private readonly List<LarkResponsePagingStatusInfo> records = new();

    /// <summary>
    /// Initializes a new instance of the LarkResponsePagingBody class.
    /// </summary>
    public LarkResponsePagingBody()
        : base()
    {
        col = [];
        TotalCount = 0;
        PagingRecords = records.AsReadOnly();
        Data = col.AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponsePagingBody class.
    /// </summary>
    /// <param name="isError">true if error; otherwise, false.</param>
    /// <param name="message">The message.</param>
    public LarkResponsePagingBody(bool isError, string? message = null)
        : base(isError, message)
    {
        col = [];
        TotalCount = 0;
        PagingRecords = records.AsReadOnly();
        Data = col.AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponsePagingBody class.
    /// </summary>
    /// <param name="query">The query info without page token to list current list.</param>
    /// <param name="raw">The raw package col in JSON.</param>
    /// <param name="key">The property key of items.</param>
    public LarkResponsePagingBody(BaseQueryRequestInfo? query, JsonObjectNode raw, string? key = null)
        : base(raw)
    {
        Query = query;
        PagingRecords = records.AsReadOnly();
        var data = raw?.TryGetObjectValue("data");
        if (data is null)
        {
            col = [];
            Data = col.AsReadOnly();
            TotalCount = 0;
            return;
        }

        TotalCount = raw?.TryGetInt32Value("total");
        col = data.TryGetObjectListValue(string.IsNullOrWhiteSpace(key) ? "items" : key, true) ?? [];
        Data = col.AsReadOnly();
        PageToken = data.TryGetStringValue("page_token");
        HasNextPage = data.TryGetBooleanValue("has_more") ?? false;
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponsePagingBody class.
    /// </summary>
    /// <param name="query">The query info without page token to list current list.</param>
    /// <param name="raw">The raw package col in JSON.</param>
    /// <param name="items">The handler to get items.</param>
    public LarkResponsePagingBody(BaseQueryRequestInfo? query, JsonObjectNode raw, Func<JsonObjectNode, List<JsonObjectNode>?> items)
        : base(raw)
    {
        Query = query;
        PagingRecords = records.AsReadOnly();
        var data = raw?.TryGetObjectValue("data");
        if (data is null)
        {
            col = [];
            Data = col.AsReadOnly();
            TotalCount = 0;
            return;
        }

        TotalCount = raw?.TryGetInt32Value("total");
        col = (items is null ? data.TryGetObjectListValue("items") : items(data)) ?? [];
        Data = col.AsReadOnly();
        PageToken = data.TryGetStringValue("page_token");
        HasNextPage = data.TryGetBooleanValue("has_more") ?? false;
    }

    /// <summary>
    /// Gets the query info used to resolve the items.
    /// </summary>
    public BaseQueryRequestInfo? Query { get; }

    /// <summary>
    /// Gets the result col.
    /// </summary>
    public new IReadOnlyList<JsonObjectNode> Data { get; }

    /// <summary>
    /// Gets the token used to get next page.
    /// </summary>
    public string? PageToken { get; private set; }

    /// <summary>
    /// Gets a value indicating whether has next page.
    /// </summary>
    public bool HasNextPage { get; private set; }

    /// <summary>
    /// Gets the total count; or null, if not provided.
    /// </summary>
    public int? TotalCount { get; private set; }

    /// <summary>
    /// Gets the count of items.
    /// </summary>
    public virtual int Count => Data.Count;

    /// <summary>
    /// Gets the action records of paging.
    /// </summary>
    public IReadOnlyList<LarkResponsePagingStatusInfo> PagingRecords { get; }

    /// <summary>
    /// Gets the next page token bag.
    /// </summary>
    /// <param name="size">The page size.</param>
    /// <returns>A page token info instance for next page.</returns>
    public LarkPageTokenInfo? NextPageInfo(int? size = null)
        => HasNextPage && !string.IsNullOrWhiteSpace(PageToken) ? new()
        {
            Token = PageToken,
            Size = size,
        } : null;

    /// <summary>
    /// Converts to query data of next page.
    /// </summary>
    /// <returns>The query data instance to list next page.</returns>
    public QueryData ToNextPageQueryData()
    {
        var page = new LarkPageTokenInfo(PageToken);
        var q = page.ToQueryData();
        Query?.ToQueryData(q);
        return q;
    }

    /// <summary>
    /// Converts to query data of next page.
    /// </summary>
    /// <param name="size">The page size.</param>
    /// <returns>The query data instance to list next page.</returns>
    public QueryData ToNextPageQueryData(int size)
    {
        var page = new LarkPageTokenInfo(size, PageToken);
        var q = page.ToQueryData();
        Query?.ToQueryData(q);
        return q;
    }

    /// <summary>
    /// Converts to URL with next page token.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="size">The optional page size.</param>
    /// <returns>The URL.</returns>
    public string ToUrl(string url, int? size = null)
    {
        var q = size.HasValue ? ToNextPageQueryData(size.Value) : ToNextPageQueryData();
        return q.ToString(url);
    }

    /// <summary>
    /// Adds items.
    /// </summary>
    /// <param name="raw">The raw package col in JSON.</param>
    /// <param name="key">The property key of items.</param>
    /// <param name="jsonResult">The result collection in JSON.</param>
    /// <param name="objectResult">The result collection in customized object.</param>
    /// <returns>true if the raw package contains any item; otherwise, false.</returns>
    protected bool AddRange(JsonObjectNode raw, string? key, out List<JsonObjectNode> jsonResult, out List<object> objectResult)
    {
        jsonResult = [];
        objectResult = [];
        var data = raw?.TryGetObjectValue("data");
        if (data is null) return false;
        var list = data.TryGetObjectListValue(string.IsNullOrWhiteSpace(key) ? "items" : key, true);
        AddRangeInternal(raw!, list.Count);
        if (list is null) return false;
        foreach (var item in list)
        {
            AddItem(item, jsonResult, objectResult);
        }

        var total = raw.TryGetInt32Value("total");
        if (total.HasValue) TotalCount = total;
        return true;
    }

    internal List<JsonObjectNode> AddRange(JsonObjectNode raw, string? key = null)
        => AddRange(raw, key, out var result, out _) ? result : [];

    /// <summary>
    /// Adds items.
    /// </summary>
    /// <param name="raw">The raw package col in JSON.</param>
    /// <param name="items">The handler to get items.</param>
    /// <param name="jsonResult">The result collection in JSON.</param>
    /// <param name="objectResult">The result collection in customized object.</param>
    /// <returns>true if the raw package contains any item; otherwise, false.</returns>
    protected bool AddRange(JsonObjectNode raw, Func<JsonObjectNode, List<JsonObjectNode>?> items, out List<JsonObjectNode> jsonResult, out List<object> objectResult)
    {
        jsonResult = [];
        objectResult = [];
        var data = raw?.TryGetObjectValue("data");
        if (data is null) return false;
        var list = (items is null ? data.TryGetObjectListValue("items") : items(data)) ?? [];
        AddRangeInternal(raw!, list.Count);
        foreach (var item in list)
        {
            AddItem(item, jsonResult, objectResult);
        }

        var total = raw.TryGetInt32Value("total");
        if (total.HasValue) TotalCount = total;
        return true;
    }

    internal List<JsonObjectNode> AddRange(JsonObjectNode raw, Func<JsonObjectNode, List<JsonObjectNode>?> items)
        => AddRange(raw, items, out var result, out _) ? result : [];

    /// <summary>
    /// Occurs on an item is added.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>The item; or null, if need skip.</returns>
    protected virtual object? OnAddItem(JsonObjectNode item)
        => item;

    private bool AddItem(JsonObjectNode? item, List<JsonObjectNode> jsonResult, List<object> objectResult)
    {
        if (item is null) return false;
        var r = OnAddItem(item);
        if (r is null) return false;
        col.Add(item);
        jsonResult.Add(item);
        objectResult.Add(r);
        return true;
    }

    private void AddRangeInternal(JsonObjectNode raw, int count)
    {
        var data = raw.TryGetObjectValue("data") ?? [];
        PageToken = raw.TryGetStringValue("page_token") ?? data.TryGetStringValue("page_token");
        HasNextPage = raw.TryGetBooleanValue("has_more") ?? data.TryGetBooleanValue("has_more") ?? false;
        var record = new LarkResponsePagingStatusInfo(DateTime.Now, raw.TryGetStringValue("msg") ?? data.TryGetStringValue("msg"), count);
    }
}

/// <summary>
/// The response body with paging from Lark API.
/// </summary>
public sealed class LarkResponsePagingBody<T> : LarkResponsePagingBody
{
    private readonly List<T> col;
    private readonly Func<JsonObjectNode, T?>? itemConverter;

    /// <summary>
    /// Initializes a new instance of the LarkResponsePagingBody class.
    /// </summary>
    public LarkResponsePagingBody()
        : base()
    {
        col = [];
        Data = col.AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponsePagingBody class.
    /// </summary>
    /// <param name="isError">true if error; otherwise, false.</param>
    /// <param name="message">The message.</param>
    public LarkResponsePagingBody(bool isError, string? message = null)
        : base(isError, message)
    {
        col = [];
        Data = col.AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponsePagingBody class.
    /// </summary>
    /// <param name="query">The query info without page token to list current list.</param>
    /// <param name="raw">The raw package col in JSON.</param>
    /// <param name="items">The handler to get items.</param>
    /// <param name="converter">The converter of result col.</param>
    public LarkResponsePagingBody(BaseQueryRequestInfo? query, JsonObjectNode raw, Func<JsonObjectNode, List<JsonObjectNode>?> items, Func<JsonObjectNode, T?>? converter = null)
        : base(query, raw, items)
    {
        col = [];
        Data = col.AsReadOnly();
        itemConverter = converter;
        foreach (var item in base.Data)
        {
            AddItem(item, out _);
        }
    }

    /// <summary>
    /// Initializes a new instance of the LarkResponsePagingBody class.
    /// </summary>
    /// <param name="query">The query info without page token to list current list.</param>
    /// <param name="raw">The raw package col in JSON.</param>
    /// <param name="converter">The converter of result col.</param>
    public LarkResponsePagingBody(BaseQueryRequestInfo? query, JsonObjectNode raw, Func<JsonObjectNode, T?>? converter = null)
        : base(query, raw)
    {
        col = [];
        Data = col.AsReadOnly();
        itemConverter = converter;
        if (raw is null) return;
        foreach (var item in base.Data)
        {
            AddItem(item, out _);
        }
    }

    /// <summary>
    /// Gets the result col.
    /// </summary>
    public new IReadOnlyList<T> Data { get; }

    /// <summary>
    /// Gets the count of items.
    /// </summary>
    public override int Count => Data.Count;

    internal new List<T> AddRange(JsonObjectNode raw, string? key = null)
        => AddRange(raw, key, out _, out var result) ? result.Cast<T>().ToList() : [];

    internal new List<T> AddRange(JsonObjectNode raw, Func<JsonObjectNode, List<JsonObjectNode>?> items)
        => AddRange(raw, items, out _, out var result) ? result.Cast<T>().ToList() : [];

    /// <inheritdoc />
    protected override object? OnAddItem(JsonObjectNode item)
    {
        if (base.OnAddItem(item) is null) return null; ;
        return AddItem(item, out var result) ? result : null;
    }

    private bool AddItem(JsonObjectNode? item, out T? result)
    {
        if (item is null)
        {
            result = default;
            return false;
        }
        var converter = itemConverter;
        if (converter is not null)
        {
            result = converter(item);
            if (result is null) return false;
            col.Add(result);
            return true;
        }
        
        if (typeof(T) == typeof(string))
        {
            var str = item.TryGetStringTrimmedValue("content", true);
            if (str is null)
            {
                result = default;
                return false;
            }

            result = (T)(object)str;
            col.Add(result);
            return true;
        }
        
        if (typeof(T) == typeof(JsonObjectNode) || typeof(T) == typeof(object))
        {
            result = (T)(object)item;
            col.Add(result);
        }
        else
        {
            result = item.Deserialize<T>();
            if (result is null) return false;
            col.Add(result);
        }

        return true;
    }
}
