using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Data;
using Trivial.Maths;
using Trivial.Net;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.OapiModels;

/// <summary>
/// The utilities of Lark doc content.
/// </summary>
public static partial class LarkApiUtils
{
    /// <summary>
    /// Gets or sets a value indicating whether to use Chinese instead of English as the first language.
    /// </summary>
    public static bool UseChinese { get; set; } = true;

    /// <summary>
    /// Gets the locale name.
    /// </summary>
    /// <param name="json">The object with strings.</param>
    /// <returns>The locale name.</returns>
    public static string? GetName(JsonObjectNode? json)
    {
        if (json is null) return null;
        if (UseChinese) return json.TryGetStringTrimmedValue("zh_cn", true) ?? json.TryGetStringTrimmedValue("en_us", true);
        return json.TryGetStringTrimmedValue("en_us", true) ?? json.TryGetStringTrimmedValue("zh_cn", true);
    }

    /// <summary>
    /// Gets the locale name.
    /// </summary>
    /// <param name="names">The collection of name.</param>
    /// <returns>The locale name.</returns>
    public static string? GetName(this IEnumerable<LarkLocaleValueItemInfo> names)
    {
        if (names is null) return null;
        var name = GetName(names, UseChinese ? "zh-CN" : "en-US");
        if (string.IsNullOrEmpty(name)) name = GetName(names, UseChinese ? "en-US" : "zh-CN");
        return name;
    }

    /// <summary>
    /// Gets the locale name.
    /// </summary>
    /// <param name="names">The collection of name.</param>
    /// <param name="langCode">The language code.</param>
    /// <returns>The locale name.</returns>
    public static string? GetName(IEnumerable<LarkLocaleValueItemInfo> names, string langCode)
    {
        if (names is null || string.IsNullOrWhiteSpace(langCode)) return null;
        foreach (var name in names)
        {
            if (name?.LanguageCode == langCode) return name.Value;
        }

        return null;
    }

    /// <summary>
    /// Gets the item with the specific identifier.
    /// </summary>
    /// <param name="col">The collection.</param>
    /// <param name="id">The identifier of the item.</param>
    /// <returns>The instance with the identifier.</returns>
    public static LarkIdNameInfo? GetById(this IEnumerable<LarkIdNameInfo> col, string id)
    {
        if (col is null || string.IsNullOrWhiteSpace(id)) return null;
        foreach (var item in col)
        {
            if (item?.Id == id) return item;
        }

        return null;
    }

    /// <summary>
    /// Sets the default instance of Lark API.
    /// </summary>
    /// <param name="instance">The new instance.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    public static bool ReplaceDefaultInstance(LarkApi instance)
    {
        if (instance is null || instance.IsAppKeyEmpty) return false;
        LarkApi.DefaultInstance = instance;
        return true;
    }

    /// <summary>
    /// Loads all rest pages.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="response">The response.</param>
    /// <param name="pageSize">The page size for each.</param>
    /// <param name="resolver">The function to load next page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The paging result.</returns>
    public static async IAsyncEnumerable<IReadOnlyList<T>> LoadAllPagesAsync<T>(LarkResponsePagingBody<T> response, int? pageSize, Func<LarkResponsePagingBody<T>, int?, CancellationToken, Task<IReadOnlyList<T>>> resolver, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (response is null || response.IsError || resolver is null) yield break;
        while (response.HasNextPage)
        {
            var col = await resolver(response, pageSize, cancellationToken);
            if (col is null) break;
            yield return col;
        }
    }

    /// <summary>
    /// Loads all rest pages.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="response">The response.</param>
    /// <param name="resolver">The function to load next page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The paging result.</returns>
    public static IAsyncEnumerable<IReadOnlyList<T>> LoadAllPagesAsync<T>(LarkResponsePagingBody<T> response, Func<LarkResponsePagingBody<T>, int?, CancellationToken, Task<IReadOnlyList<T>>> resolver, CancellationToken cancellationToken = default)
        => LoadAllPagesAsync(response, 50, resolver, cancellationToken);

    /// <summary>
    /// Loads all rest pages.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="pageSize">The page size for each.</param>
    /// <param name="resolver">The function to load next page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The paging result.</returns>
    public static async IAsyncEnumerable<IReadOnlyList<JsonObjectNode>> LoadAllPagesAsync(LarkResponsePagingBody response, int? pageSize, Func<LarkResponsePagingBody, int?, CancellationToken, Task<IReadOnlyList<JsonObjectNode>>> resolver, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (response is null || response.IsError || resolver is null) yield break;
        while (response.HasNextPage)
        {
            var col = await resolver(response, pageSize, cancellationToken);
            if (col is null) break;
            yield return col;
        }
    }

    /// <summary>
    /// Loads all rest pages.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="resolver">The function to load next page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The paging result.</returns>
    public static IAsyncEnumerable<IReadOnlyList<JsonObjectNode>> LoadAllPagesAsync(LarkResponsePagingBody response, Func<LarkResponsePagingBody, int?, CancellationToken, Task<IReadOnlyList<JsonObjectNode>>> resolver, CancellationToken cancellationToken = default)
        => LoadAllPagesAsync(response, 50, resolver, cancellationToken);

    /// <summary>
    /// Loads all rest pages.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="response">The response.</param>
    /// <param name="pageSize">The page size for each.</param>
    /// <param name="resolver">The function to load next page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The paging result.</returns>
    public static async IAsyncEnumerable<T> ForEachAsync<T>(LarkResponsePagingBody<T> response, int? pageSize, Func<LarkResponsePagingBody<T>, int?, CancellationToken, Task<IReadOnlyList<T>>>? resolver, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (response?.Data is null || response.IsError) yield break;
        foreach (var item in response.Data)
        {
            yield return item;
        }

        if (resolver is null) yield break;
        while (response.HasNextPage)
        {
            var col = await resolver(response, pageSize, cancellationToken);
            if (col is null) break;
            foreach (var item in col)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Loads all rest pages.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="response">The response.</param>
    /// <param name="resolver">The function to load next page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The paging result.</returns>
    public static IAsyncEnumerable<T> ForEachAsync<T>(LarkResponsePagingBody<T> response, Func<LarkResponsePagingBody<T>, int?, CancellationToken, Task<IReadOnlyList<T>>> resolver, CancellationToken cancellationToken = default)
        => ForEachAsync(response, 50, resolver, cancellationToken);

    /// <summary>
    /// Loads all rest pages.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="response">The response.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The paging result.</returns>
    public static IAsyncEnumerable<T> ForEachAsync<T>(LarkResponsePagingBody<T> response, CancellationToken cancellationToken = default)
        => ForEachAsync(response, null, null, cancellationToken);

    /// <summary>
    /// Loads all rest pages.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="pageSize">The page size for each.</param>
    /// <param name="resolver">The function to load next page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The paging result.</returns>
    public static async IAsyncEnumerable<JsonObjectNode> ForEachAsync(LarkResponsePagingBody response, int? pageSize, Func<LarkResponsePagingBody, int?, CancellationToken, Task<IReadOnlyList<JsonObjectNode>>>? resolver, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (response?.Data is null || response.IsError) yield break;
        foreach (var item in response.Data)
        {
            yield return item;
        }

        if (resolver is null) yield break;
        while (response.HasNextPage)
        {
            var col = await resolver(response, pageSize, cancellationToken);
            if (col is null) break;
            foreach (var item in col)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Loads all rest pages.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="resolver">The function to load next page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The paging result.</returns>
    public static IAsyncEnumerable<JsonObjectNode> ForEachAsync(LarkResponsePagingBody response, Func<LarkResponsePagingBody, int?, CancellationToken, Task<IReadOnlyList<JsonObjectNode>>> resolver, CancellationToken cancellationToken = default)
        => ForEachAsync(response, 50, resolver, cancellationToken);

    /// <summary>
    /// Loads all rest pages.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The paging result.</returns>
    public static IAsyncEnumerable<JsonObjectNode> ForEachAsync(LarkResponsePagingBody response, CancellationToken cancellationToken = default)
        => ForEachAsync(response, null, null, cancellationToken);

    /// <summary>
    /// Gets the resource.
    /// </summary>
    /// <typeparam name="T">The type of the resource.</typeparam>
    /// <param name="id">The resource identifier.</param>
    /// <param name="resolver">The function to resolve resource.</param>
    /// <param name="cache">The data cache.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The resource.</returns>
    public static async Task<T?> GetResourceAsync<T>(string id, Func<string, CancellationToken, Task<LarkResponseBody<T>>> resolver, DataCacheCollection<T>? cache, CancellationToken cancellationToken = default)
    {
        if (cache is not null && cache.TryGet(id, out var data) && data is not null) return data;
        if (resolver is null) return default;
        var resp = await resolver(id, cancellationToken);
        if (resp is null || resp.Data is null || resp.IsError) return default;
        if (cache is not null) cache[id] = resp.Data!;
        return resp.Data;
    }

    /// <summary>
    /// Gets the resource.
    /// </summary>
    /// <typeparam name="T">The type of the resource.</typeparam>
    /// <param name="id">The resource identifier.</param>
    /// <param name="resolver">The function to resolve resource.</param>
    /// <param name="cache">The data cache.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The resource.</returns>
    public static async Task<JsonObjectNode?> GetResourceAsync(string id, Func<string, CancellationToken, Task<LarkResponseBody>> resolver, DataCacheCollection<JsonObjectNode>? cache, CancellationToken cancellationToken = default)
    {
        if (cache is not null && cache.TryGet(id, out var data) && data is not null) return data;
        if (resolver is null) return null;
        var resp = await resolver(id, cancellationToken);
        if (resp is null || resp.Data is null || resp.IsError) return default;
        if (cache is not null) cache[id] = resp.Data!;
        return resp.Data;
    }

    /// <summary>
    /// Parses gender.
    /// </summary>
    /// <param name="s">The string representation of gender.</param>
    /// <returns>The parsed <see cref="LarkGender"/> value.</returns>
    public static LarkGender ParseGender(string? s)
    {
        if (s == null) return LarkGender.Unknown;
        return s.Trim().ToLowerInvariant() switch
        {
            "male" or "m" or "男" or "1" => LarkGender.Male,
            "female" or "f" or "女" or "2" => LarkGender.Female,
            "" or "unknown" or "0" or "?" => LarkGender.Unknown,
            _ => LarkGender.Others,
        };
    }

    /// <summary>
    /// Converts gender to string.
    /// </summary>
    /// <param name="gender">The gender value.</param>
    /// <returns>The string representation of the gender.</returns>
    public static string ToString(LarkGender gender)
        => UseChinese ? gender switch
        {
            LarkGender.Male => "男",
            LarkGender.Female => "女",
            LarkGender.Unknown => "未知",
            _ => "其它",
        } : gender switch
        {
            LarkGender.Male => "Male",
            LarkGender.Female => "Female",
            LarkGender.Unknown => "Unknown",
            _ => "Others",
        };

    internal static string ToDocsFilterString(BasicCompareOperator op)
        => op switch
        {
            BasicCompareOperator.Equal => "is",
            BasicCompareOperator.NotEqual => "isNot",
            BasicCompareOperator.Greater => "isGreater",
            BasicCompareOperator.GreaterOrEqual => "isGreaterEqual",
            BasicCompareOperator.Less => "isLess",
            BasicCompareOperator.LessOrEqual => "isLessEqual",
            _ => "is",
        };
}

/// <summary>
/// The source kind of Lark access token.
/// </summary>
public enum LarkApiTokenSourceKind : byte
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Tenant token used by app.
    /// </summary>
    Tenant = 1,

    /// <summary>
    /// User OpenID token.
    /// </summary>
    User = 2,

    /// <summary>
    /// The static token without information.
    /// </summary>
    Static = 3,

    /// <summary>
    /// Invalid or empty.
    /// </summary>
    Empty = 14,

    /// <summary>
    /// Other kind of token.
    /// </summary>
    Others = 15,
}
