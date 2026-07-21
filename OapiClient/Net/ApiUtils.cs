using LarkSuite.Docs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
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
    /// Gets the locale name.
    /// </summary>
    /// <param name="json">The object with strings.</param>
    /// <returns>The locale name.</returns>
    public static string? GetName(JsonObjectNode? json)
    {
        if (json is null) return null;
        return json.TryGetStringTrimmedValue("zh_cn", true) ?? json.TryGetStringTrimmedValue("en_us", true);
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
