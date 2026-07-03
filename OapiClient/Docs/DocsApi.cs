using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Net;
using Trivial.Security;
using Trivial.Text;

namespace LarkSuite;

public partial class LarkApi
{
    public Task<LarkResponsePagingBody<LarkWikiSpaceInfo>> GetWikiSpacesAsync(CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.WikiSpaces, LarkWikiSpaceInfo.Convert, cancellationToken);

    public Task<LarkResponsePagingBody<LarkWikiSpaceInfo>> GetWikiSpacesAsync(LarkPageTokenInfo options, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.WikiSpaces, null, options, LarkWikiSpaceInfo.Convert, cancellationToken);

    public Task<IReadOnlyList<LarkWikiSpaceInfo>> GetWikiSpacesAsync(LarkResponsePagingBody<LarkWikiSpaceInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.WikiSpaces, response, pageSize, cancellationToken);

    public Task<LarkResponseBody<LarkWikiSpaceInfo>> GetWikiSpaceInfoAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceInfo, LarkUrls.GetId(id)), LarkWikiSpaceInfo.Convert, cancellationToken);

    public Task<LarkResponseBody<LarkWikiSpaceInfo>> GetWikiSpaceInfoAsync(string id, LarkResourceRequestOptions options, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceInfo, options, LarkUrls.GetId(id)), LarkWikiSpaceInfo.Convert, cancellationToken);

    /// <summary>
    /// Searches wiki by given query string.
    /// </summary>
    /// <param name="q">The query.</param>
    /// <param name="cancellationToken">A cancellation id to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    /// <remarks>User id only.</remarks>
    public Task<LarkResponseBody> SearchWikiAsync(string q, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.SearchWiki, new JsonObjectNode
        {
            { "query", q }
        }, cancellationToken);

    /// <summary>
    /// Searches wiki by given query string.
    /// </summary>
    /// <param name="options">The search options.</param>
    /// <param name="cancellationToken">A cancellation id to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    /// <remarks>User id only.</remarks>
    public Task<LarkResponseBody> SearchWikiAsync(LarkWikiSearchOptions options, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.SearchWiki, options), options?.ToJson() ?? [], cancellationToken);
    
    public Task<LarkResponsePagingBody<LarkDocsNodeInfo>> GetWikiSpaceNodesAsync(string id, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsNodeInfo>(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, id), cancellationToken);

    public Task<LarkResponsePagingBody<LarkDocsNodeInfo>> GetWikiSpaceNodesAsync(string id, LarkWikiNodesRequestOptions options, LarkPageTokenInfo? page = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsNodeInfo>(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, id), options, page, cancellationToken);

    public Task<IReadOnlyList<LarkDocsNodeInfo>> GetWikiSpaceNodesAsync(string id, LarkResponsePagingBody<LarkDocsNodeInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, id), response, pageSize, cancellationToken);

    public Task<LarkResponsePagingBody> GetWikiSpaceMembersAsync(string token, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceMembers, token), cancellationToken);

    public Task<LarkResponsePagingBody> GetWikiSpaceNodesAsync(string token, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, token), null, paging, cancellationToken);

    public Task<IReadOnlyList<JsonObjectNode>> GetWikiSpaceNodesAsync(string token, LarkResponsePagingBody response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, token), response, pageSize, cancellationToken);

    public Task<LarkResponseBody<LarkDocsNodeInfo>> GetWikiNodeAsync(string token, CancellationToken cancellationToken = default)
        => GetAsync<LarkDocsNodeInfo>(new QueryData()
        {
            { "token", LarkUrls.GetId(token) },
        }.ToString(LarkUrls.GetWikiNode), "node", cancellationToken);

    public Task<LarkResponseBody<LarkDocsNodeInfo>> GetWikiNodeAsync(string token, string objType, CancellationToken cancellationToken = default)
        => GetAsync<LarkDocsNodeInfo>(new QueryData()
        {
            { "token", LarkUrls.GetId(token) },
            { "obj_type", objType },
        }.ToString(LarkUrls.GetWikiNode), "node", cancellationToken);

    public Task<LarkResponseBody> GetDocsInfoAsync(string token, CancellationToken cancellationToken = default)
        => GetAsync(string.Concat(LarkUrls.DocsInfo, LarkUrls.GetId(token)), "document", cancellationToken);

    public async Task<LarkResponseBody<string>> GetDocsTextAsync(string token, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.GetAsync(LarkUrls.ToUrl(LarkUrls.DocsText, LarkUrls.GetId(token)), cancellationToken);
        return new(resp);
    }

    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public Task<LarkResponsePagingBody<LarkContentBlock>> GetDocsBlocksAsync(string token, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkContentBlock>(LarkUrls.ToUrl(LarkUrls.DocsBlocks, LarkUrls.GetId(token)), null, null, json => new(json), cancellationToken);

    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public Task<LarkResponsePagingBody<LarkContentBlock>> GetDocsBlocksAsync(string token, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkContentBlock>(LarkUrls.ToUrl(LarkUrls.DocsBlocks, LarkUrls.GetId(token)), null, paging, json => new(json), cancellationToken);

    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public Task<IReadOnlyList<LarkContentBlock>> GetDocsBlocksAsync(string token, LarkResponsePagingBody<LarkContentBlock> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.DocsBlocks, LarkUrls.GetId(token)), response, pageSize, cancellationToken);

    public Task<LarkResponseBody> GetDocsMarkdownAsync(LarkWikiDocMarkdownOptions options, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.DocsMarkdown, options), cancellationToken);

    public Task<LarkResponseBody> GetDocsMarkdownAsync(string token, CancellationToken cancellationToken = default)
    => GetAsync(LarkUrls.ToUrl(LarkUrls.DocsMarkdown, new LarkWikiDocMarkdownOptions()
    {
        Id = token,
    }), cancellationToken);

    public Task<LarkResponseBody> GetDocsBoardNodesAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.DocsBoardNodes, LarkUrls.GetId(id)), cancellationToken);

    public Task<LarkResponseBody> GetDocsBoardNodesAsync(string id, LarkUserIdTypeRequestOptions options, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.DocsBoardNodes, options, LarkUrls.GetId(id)), cancellationToken);

    public Task<LarkResponseBody> ListBaseTableAsync(string baseId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.ListBaseTable, paging, LarkUrls.GetId(baseId)), cancellationToken);

    public Task<LarkResponseBody> RenameBaseTableAsync(string baseId, string tableId, string title, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.RenameBaseTable, baseId, tableId), new JsonObjectNode
        {
            { "name", title },
        }, cancellationToken);

    public Task<LarkResponseBody> ReadBaseTableAsync(string baseId, string tableId, CancellationToken cancellationToken = default)
        => ReadBaseTableAsync(baseId, tableId, null, cancellationToken);

    public Task<LarkResponseBody> ReadBaseTableAsync(string baseId, string tableId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.ReadBaseTable, paging, baseId, tableId), new JsonObjectNode
        {
            { "automatic_fields", true },
        }, cancellationToken);
}
