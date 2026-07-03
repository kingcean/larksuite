using LarkSuite.OapiModels;
using LarkSuite.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using Trivial.Net;
using Trivial.Security;
using Trivial.Text;

namespace LarkSuite;

/// <summary>
/// The API to access resources in Lark.
/// </summary>
public partial class LarkApi : TokenContainer
{
    private HttpClient? httpClient;

    /// <summary>
    /// Gets the default instance of Lark API.
    /// </summary>
    public static LarkApi DefaultInstance { get; } = new();

    /// <summary>
    /// Initializes a new instance of the LarkApi class.
    /// </summary>
    public LarkApi()
    {
        AppKey = new(LarkUrls.GetAppKeyId(), LarkUrls.GetAppKeySecret());
    }

    /// <summary>
    /// Initializes a new instance of the LarkApi class.
    /// </summary>
    /// <param name="appId">The app key identifer.</param>
    /// <param name="appSecret">The app key secret.</param>
    public LarkApi(string appId, string appSecret)
    {
        AppKey = new(appId ?? LarkUrls.GetAppKeyId(), appSecret ?? LarkUrls.GetAppKeySecret());
    }

    /// <summary>
    /// Initializes a new instance of the LarkApi class.
    /// </summary>
    /// <param name="appKey">The app key with secret.</param>
    public LarkApi(AppAccessingKey appKey)
    {
        AppKey = appKey ?? new(LarkUrls.GetAppKeyId(), LarkUrls.GetAppKeySecret());
    }

    /// <summary>
    /// Gets the app key to access resource.
    /// </summary>
    protected AppAccessingKey AppKey { get; }

    /// <summary>
    /// The HTTP client.
    /// </summary>
    public HttpClient HttpClient
    {
        get
        {
            httpClient ??= new HttpClient();
            return httpClient;
        }
    }

    /// <summary>
    /// Gets the date time when the token is resolved.
    /// </summary>
    public DateTime TokenResolved { get; protected set; } = DateTime.Now;

    /// <summary>
    /// Gets a value indicating whether the token is expired.
    /// </summary>
    public bool IsTokenExpired
    {
        get
        {
            var expired = Token?.ExpiredAfter;
            return expired is null || Token!.IsEmpty || (TokenResolved + expired.Value) >= DateTime.Now;
        }
    }

    /// <summary>
    /// Creates a JSON HTTP client.
    /// </summary>
    /// <typeparam name="T">The type of response.</typeparam>
    /// <returns>A JSON HTTP client.</returns>
    public JsonHttpClient<T> CreateJsonHttpClient<T>()
        => new(HttpClient);

    /// <summary>
    /// Creates a JSON HTTP client.
    /// </summary>
    /// <returns>A JSON HTTP client.</returns>
    public JsonHttpClient<JsonObjectNode> CreateJsonHttpClient()
    {
        var http = CreateJsonHttpClient<JsonObjectNode>();
        http.SerializeEvenIfFailed = true;
        return http;
    }

    /// <summary>
    /// Sends a request message by GET to get response result.
    /// </summary>
    /// <typeparam name="T">The type of response.</typeparam>
    /// <param name="uri">The URI the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result deserialized.</returns>
    public async Task<T> GetAsync<T>(Uri uri, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient<T>();
        var resp = await http.GetAsync(uri, cancellationToken);
        return resp;
    }

    /// <summary>
    /// Sends a request message by GET to get response result.
    /// </summary>
    /// <typeparam name="T">The type of resposne.</typeparam>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result deserialized.</returns>
    public Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default)
        => GetAsync<T>(new Uri(url), cancellationToken);

    /// <summary>
    /// Sends a request message by GET to get response result.
    /// </summary>
    /// <typeparam name="T">The type of resposne.</typeparam>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result deserialized.</returns>
    public async Task<LarkResponseBody<T>> GetAsync<T>(string url, Func<JsonObjectNode, T> converter, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var json = await http.GetAsync(url, cancellationToken);
        return new(json, converter);
    }

    /// <summary>
    /// Sends a request message by GET to get response result.
    /// </summary>
    /// <typeparam name="T">The type of resposne.</typeparam>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="key">The property key to resolve result col.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result deserialized.</returns>
    public async Task<LarkResponseBody<T>> GetAsync<T>(string url, string key, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var json = await http.GetAsync(url, cancellationToken);
        return new(json, key);
    }

    /// <summary>
    /// Sends a request message by GET to get response result.
    /// </summary>
    /// <param name="uri">The URI the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<LarkResponseBody> GetAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.GetAsync(uri, cancellationToken);
        return new(resp);
    }

    /// <summary>
    /// Sends a request message by GET to get response result.
    /// </summary>
    /// <param name="uri">The URI the request is sent to.</param>
    /// <param name="key">The property key to resolve result col.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<LarkResponseBody> GetAsync(Uri uri, string key, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.GetAsync(uri, cancellationToken);
        return new(resp, key);
    }

    /// <summary>
    /// Sends a request message by GET to get response result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public Task<LarkResponseBody> GetAsync(string url, CancellationToken cancellationToken = default)
        => GetAsync(new Uri(url), cancellationToken);

    /// <summary>
    /// Sends a request message by GET to get response result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="key">The property key to resolve result col.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public Task<LarkResponseBody> GetAsync(string url, string key, CancellationToken cancellationToken = default)
        => GetAsync(new Uri(url), key, cancellationToken);

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="uri">The URI the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<LarkResponsePagingBody> GetItemsAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.GetAsync(uri, cancellationToken);
        return new(null, resp);
    }

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public Task<LarkResponsePagingBody> GetItemsAsync(string url, CancellationToken cancellationToken = default)
        => GetItemsAsync(new Uri(url), cancellationToken);

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="page">The page size and page token.</param>
    /// <param name="q">The query info.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<LarkResponsePagingBody> GetItemsAsync(string url, BaseQueryRequestInfo? q, LarkPageTokenInfo? page, CancellationToken cancellationToken = default)
        => new(q, await GetJsonObjectAsync(url, q, page, cancellationToken));

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="response">The response of the previous or the first page.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<IReadOnlyList<JsonObjectNode>> GetItemsAsync(string url, LarkResponsePagingBody response, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        if (!response.HasNextPage || string.IsNullOrWhiteSpace(response.PageToken)) return [];
        var resp = await GetJsonObjectAsync(url, response, pageSize, cancellationToken);
        return response.AddRange(resp);
    }

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="page">The page size and page token.</param>
    /// <param name="q">The query info.</param>
    /// <param name="items">The handler to get items.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<LarkResponsePagingBody> GetItemsAsync(string url, BaseQueryRequestInfo? q, LarkPageTokenInfo? page, Func<JsonObjectNode, List<JsonObjectNode>?> items, CancellationToken cancellationToken = default)
        => new(q, await GetJsonObjectAsync(url, q, page, cancellationToken), items);

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="response">The response of the previous or the first page.</param>
    /// <param name="items">The handler to get items.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<IReadOnlyList<JsonObjectNode>> GetItemsAsync(string url, LarkResponsePagingBody response, Func<JsonObjectNode, List<JsonObjectNode>?> items, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        if (!response.HasNextPage || string.IsNullOrWhiteSpace(response.PageToken)) return [];
        var resp = await GetJsonObjectAsync(url, response, pageSize, cancellationToken);
        return response.AddRange(resp, items);
    }

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="page">The page size and page token.</param>
    /// <param name="q">The query info.</param>
    /// <param name="key">The property key of items.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<LarkResponsePagingBody> GetItemsAsync(string url, BaseQueryRequestInfo? q, LarkPageTokenInfo? page, string key, CancellationToken cancellationToken = default)
        => new(q, await GetJsonObjectAsync(url, q, page, cancellationToken), key);

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="response">The response of the previous or the first page.</param>
    /// <param name="key">The property key of items.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<IReadOnlyList<JsonObjectNode>> GetItemsAsync(string url, LarkResponsePagingBody response, string key, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        if (!response.HasNextPage || string.IsNullOrWhiteSpace(response.PageToken)) return [];
        var resp = await GetJsonObjectAsync(url, response, pageSize, cancellationToken);
        return response.AddRange(resp, key);
    }

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="uri">The URI the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<LarkResponsePagingBody<T>> GetItemsAsync<T>(Uri uri, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.GetAsync(uri, cancellationToken);
        return new(null, resp);
    }

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="uri">The URI the request is sent to.</param>
    /// <param name="converter">The converter of result col.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<LarkResponsePagingBody<T>> GetItemsAsync<T>(Uri uri, Func<JsonObjectNode, T>? converter, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.GetAsync(uri, cancellationToken);
        return new(null, resp, converter);
    }

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="uri">The URI the request is sent to.</param>
    /// <param name="converter">The converter of result col.</param>
    /// <param name="items">The handler to get items.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<LarkResponsePagingBody<T>> GetItemsAsync<T>(Uri uri, Func<JsonObjectNode, T>? converter, Func<JsonObjectNode, List<JsonObjectNode>?> items, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.GetAsync(uri, cancellationToken);
        return items is null ? new(null, resp, converter) : new(null, resp, items, converter);
    }

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public Task<LarkResponsePagingBody<T>> GetItemsAsync<T>(string url, CancellationToken cancellationToken = default)
        => GetItemsAsync<T>(new Uri(url), cancellationToken);

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <param name="converter">The converter of result col.</param>
    /// <returns>The response result.</returns>
    public Task<LarkResponsePagingBody<T>> GetItemsAsync<T>(string url, Func<JsonObjectNode, T>? converter, CancellationToken cancellationToken = default)
        => GetItemsAsync(new Uri(url), converter, cancellationToken);

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <param name="converter">The converter of result col.</param>
    /// <param name="items">The handler to get items.</param>
    /// <returns>The response result.</returns>
    public Task<LarkResponsePagingBody<T>> GetItemsAsync<T>(string url, Func<JsonObjectNode, T>? converter, Func<JsonObjectNode, List<JsonObjectNode>?> items, CancellationToken cancellationToken = default)
        => GetItemsAsync(new Uri(url), converter, items, cancellationToken);

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="page">The page size and page token.</param>
    /// <param name="q">The query info.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<LarkResponsePagingBody<T>> GetItemsAsync<T>(string url, BaseQueryRequestInfo? q, LarkPageTokenInfo? page, CancellationToken cancellationToken = default)
        => new(q, await GetJsonObjectAsync(url, q, page, cancellationToken));

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="response">The response of the previous or the first page.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<IReadOnlyList<T>> GetItemsAsync<T>(string url, LarkResponsePagingBody<T> response, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        if (!response.HasNextPage || string.IsNullOrWhiteSpace(response.PageToken)) return [];
        var resp = await GetJsonObjectAsync(url, response, pageSize, cancellationToken);
        return response.AddRange(resp);
    }

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="page">The page size and page token.</param>
    /// <param name="q">The query info.</param>
    /// <param name="converter">The converter of result col.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<LarkResponsePagingBody<T>> GetItemsAsync<T>(string url, BaseQueryRequestInfo? q, LarkPageTokenInfo? page, Func<JsonObjectNode, T?> converter, CancellationToken cancellationToken = default)
        => new(q, await GetJsonObjectAsync(url, q, page, cancellationToken), converter);

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="page">The page size and page token.</param>
    /// <param name="q">The query info.</param>
    /// <param name="converter">The converter of result col.</param>
    /// <param name="items">The handler to get items.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<LarkResponsePagingBody<T>> GetItemsAsync<T>(string url, BaseQueryRequestInfo? q, LarkPageTokenInfo? page, Func<JsonObjectNode, T?> converter, Func<JsonObjectNode, List<JsonObjectNode>?> items, CancellationToken cancellationToken = default)
    {
        var resp = await GetJsonObjectAsync(url, q, page, cancellationToken);
        return new(q, resp, items, converter);
    }

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="response">The response of the previous or the first page.</param>
    /// <param name="items">The handler to get items.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<IReadOnlyList<T>> GetItemsAsync<T>(string url, LarkResponsePagingBody<T> response, Func<JsonObjectNode, List<JsonObjectNode>?> items, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        if (!response.HasNextPage || string.IsNullOrWhiteSpace(response.PageToken)) return [];
        var resp = await GetJsonObjectAsync(url, response, pageSize, cancellationToken);
        return response.AddRange(resp, items);
    }

    /// <summary>
    /// Sends a request message by POST to get response result.
    /// </summary>
    /// <typeparam name="T">The type of resposne.</typeparam>
    /// <param name="uri">The Uri the request is sent to.</param>
    /// <param name="request">The request body.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public async Task<T> PostAsync<T>(Uri uri, JsonObjectNode request, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient<T>();
        var resp = await http.PostAsync(uri, request, cancellationToken);
        return resp;
    }

    /// <summary>
    /// Sends a request message by POST to get response result.
    /// </summary>
    /// <typeparam name="T">The type of resposne.</typeparam>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="request">The request body.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result deserialized.</returns>
    public Task<T> PostAsync<T>(string url, JsonObjectNode request, CancellationToken cancellationToken = default)
        => PostAsync<T>(new Uri(url), request, cancellationToken);

    /// <summary>
    /// Sends a request message by POST to get response result.
    /// </summary>
    /// <param name="uri">The Uri the request is sent to.</param>
    /// <param name="request">The request body.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result deserialized.</returns>
    public async Task<LarkResponseBody> PostAsync(Uri uri, JsonObjectNode request, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.PostAsync(uri, request, cancellationToken);
        return new(resp);
    }

    /// <summary>
    /// Sends a request message by POST to get response result.
    /// </summary>
    /// <param name="uri">The Uri the request is sent to.</param>
    /// <param name="request">The request body.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    public Task<LarkResponseBody> PostAsync(string uri, JsonObjectNode request, CancellationToken cancellationToken = default)
        => PostAsync(new Uri(uri), request, cancellationToken);

    /// <summary>
    /// Tests if the token will expire soon (less than a quarter of duration) or is expired.
    /// </summary>
    /// <returns>true if the token will expire soon or is expired; otherwise, false.</returns>
    public bool WillTokenExpireSoon()
    {
        var expired = Token?.ExpiredAfter;
        if (expired is null || Token!.IsEmpty) return true;
        var diff = DateTime.Now - TokenResolved;
        return diff.TotalMilliseconds > expired.Value.TotalMilliseconds / 4 * 3;
    }

    /// <summary>
    /// Gets and refreshes the token of tenant.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The token resolved.</returns>
    public async Task<LarkTenantToken?> GetTenantTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!WillTokenExpireSoon() && Token is LarkTenantTokenInfo token) return token.OriginalToken;
        var http = CreateJsonHttpClient<LarkTenantToken>();
        var resp = await http.PostAsync(LarkUrls.tenantTokenUri, new JsonObjectNode
        {
            { "app_id", AppKey.Id },
            { "app_secret", AppKey.Secret },
        }, cancellationToken);
        if (string.IsNullOrWhiteSpace(resp?.Value))
        {
            if (IsTokenExpired) Token = null;
            return resp;
        }

        TokenResolved = DateTime.Now;
        Token = new LarkTenantTokenInfo(resp);
        HttpClient.DefaultRequestHeaders.Authorization = Token.ToAuthenticationHeaderValue();
        return resp;
    }

    /// <summary>
    /// Gets and refreshes the token of user.
    /// </summary>
    /// <param name="request">The code token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The token resolved.</returns>
    public async Task<TokenInfo?> GetUserTokenAsync(CodeTokenRequestBody request, CancellationToken cancellationToken = default)
    {
        if (!WillTokenExpireSoon() && Token is not LarkTenantTokenInfo) return Token;
        var http = CreateJsonHttpClient<TokenInfo>();
        var req = new CodeTokenRequest(request, AppKey);
        var resp = await http.PostAsync(LarkUrls.userTokenUri, req.ToJson(), cancellationToken);
        if (string.IsNullOrWhiteSpace(resp?.AccessToken))
        {
            if (IsTokenExpired) Token = null;
            return resp;
        }

        TokenResolved = DateTime.Now;
        Token = resp;
        HttpClient.DefaultRequestHeaders.Authorization = Token.ToAuthenticationHeaderValue();
        return resp;
    }

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="page">The page size and page token.</param>
    /// <param name="q">The query info.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    private async Task<JsonObjectNode> GetJsonObjectAsync(string url, BaseQueryRequestInfo? q, LarkPageTokenInfo? page, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        return await http.GetAsync(new Uri(LarkUrls.ToUrl(url, q, page)), cancellationToken);
    }

    /// <summary>
    /// Sends a request message by GET to get response with collection result.
    /// </summary>
    /// <param name="url">The URL the request is sent to.</param>
    /// <param name="response">The response of the previous or the first page.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    private async Task<JsonObjectNode> GetJsonObjectAsync(string url, LarkResponsePagingBody response, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        return await http.GetAsync(new Uri(response.ToUrl(url, pageSize)), cancellationToken);
    }
}
