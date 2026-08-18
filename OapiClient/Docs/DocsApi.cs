using LarkSuite.Docs;
using LarkSuite.OapiModels;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Trivial.Net;
using Trivial.Security;
using Trivial.Text;

namespace LarkSuite;

public partial class LarkApi
{
    /// <summary>
    /// Gets the wiki (and docs) spaces that current account can access.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki space information</returns>
    [Description("Get the wiki (and docs) spaces that current account can access.")]
    public Task<LarkResponsePagingBody<LarkWikiSpaceInfo>> GetWikiSpacesAsync(CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.WikiSpaces, LarkWikiSpaceInfo.Convert, cancellationToken);

    public Task<LarkResponsePagingBody<LarkWikiSpaceInfo>> GetWikiSpacesAsync(LarkPageTokenInfo options, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.WikiSpaces, null, options, LarkWikiSpaceInfo.Convert, cancellationToken);

    public Task<IReadOnlyList<LarkWikiSpaceInfo>> GetWikiSpacesAsync(LarkResponsePagingBody<LarkWikiSpaceInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.WikiSpaces, response, pageSize, cancellationToken);

    /// <summary>
    /// Gets the information of the specific wiki (and docs) space.
    /// </summary>
    /// <param name="id">The wiki space identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki space information.</returns>
    [Description("Get the information of the specific wiki (and docs) space.")]
    public Task<LarkResponseBody<LarkWikiSpaceInfo>> GetWikiSpaceInfoAsync([Description("The wiki space identifier.")] string id, CancellationToken cancellationToken = default)
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

    /// <summary>
    /// Lists the top level nodes of the wiki space.
    /// </summary>
    /// <param name="id">The wiki space identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The doc node info.</returns>
    [Description("List the top level nodes of the wiki space.")]
    public Task<LarkResponsePagingBody<LarkDocsNodeInfo>> GetWikiSpaceNodesAsync([Description("The wiki space identifier.")] string id, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsNodeInfo>(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, LarkUrls.GetId(id)), cancellationToken);

    public Task<LarkResponsePagingBody<LarkDocsNodeInfo>> GetWikiSpaceNodesAsync(LarkWikiNodesRequestOptions options, LarkPageTokenInfo? page = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsNodeInfo>(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, options.SpaceId), options, page, cancellationToken);

    public Task<IReadOnlyList<LarkDocsNodeInfo>> GetWikiSpaceNodesAsync(LarkResponsePagingBody<LarkDocsNodeInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, (response?.Query as LarkWikiNodesRequestOptions)?.SpaceId), response!, pageSize, cancellationToken);

    public Task<LarkResponsePagingBody> GetWikiSpaceMembersAsync(string token, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceMembers, token), cancellationToken);

    public Task<LarkResponsePagingBody> GetWikiSpaceNodesAsync(string token, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, token), new LarkResourceIdRequest(token), paging, cancellationToken);

    public Task<IReadOnlyList<JsonObjectNode>> GetWikiSpaceNodesAsync(LarkResponsePagingBody response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, (response?.Query as LarkResourceIdRequest)?.Id), response!, pageSize, cancellationToken);

    public Task<LarkResponseBody<LarkDocsNodeInfo>> GetWikiNodeAsync(string token, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(token) ? Task.FromResult(new LarkResponseBody<LarkDocsNodeInfo>(true, "The node token is not given.")) : GetAsync<LarkDocsNodeInfo>(new QueryData()
        {
            { "token", LarkUrls.GetId(token) },
        }.ToString(LarkUrls.GetWikiNode), "node", cancellationToken);

    public Task<LarkResponseBody<LarkDocsNodeInfo>> GetWikiNodeAsync(string token, string objType, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(token) ? Task.FromResult(new LarkResponseBody<LarkDocsNodeInfo>(true, "The node token is not given.")) : GetAsync<LarkDocsNodeInfo>(new QueryData()
        {
            { "token", LarkUrls.GetId(token) },
            { "obj_type", objType },
        }.ToString(LarkUrls.GetWikiNode), "node", cancellationToken);

    public Task<LarkResponseBody<LarkDocsDocInfo>> GetDocsInfoAsync(string token, CancellationToken cancellationToken = default)
        => GetAsync<LarkDocsDocInfo>(string.Concat(LarkUrls.DocsInfo, LarkUrls.GetId(token)), "document", cancellationToken);

    public async Task<LarkResponseBody<string>> GetDocsTextAsync(string token, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.GetAsync(LarkUrls.ToUrl(LarkUrls.DocsText, LarkUrls.GetId(token)), cancellationToken);
        return new(resp);
    }

    /// <summary>
    /// Gets the content of the specific doc.
    /// </summary>
    /// <param name="token">The node token of the online doc.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The content of the doc.</returns>
    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public Task<LarkResponsePagingBody<LarkContentBlock>> GetDocsBlocksAsync([Description("The URL or node token of the online doc.")] string token, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkContentBlock>(LarkUrls.ToUrl(LarkUrls.DocsBlocks, LarkUrls.GetId(token), true), new LarkResourceIdRequest(LarkUrls.GetId(token)), null, json => new(json), cancellationToken);

    /// <summary>
    /// Gets the content of the specific doc.
    /// </summary>
    /// <param name="token">The node token of the online doc.</param>
    /// <param name="paging">The paging information.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The content of the doc.</returns>
    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public Task<LarkResponsePagingBody<LarkContentBlock>> GetDocsBlocksAsync([Description("The URL or node token of the online doc.")] string token, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkContentBlock>(LarkUrls.ToUrl(LarkUrls.DocsBlocks, LarkUrls.GetId(token), true), new LarkResourceIdRequest(LarkUrls.GetId(token)), paging, json => new(json), cancellationToken);

    /// <summary>
    /// Gets the content of the specific doc.
    /// </summary>
    /// <param name="token">The node token of the online doc.</param>
    /// <param name="loadAllPages">true if load all the blocks once, otherwise, false, to load the ones of the first page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The content of the doc.</returns>
    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public async Task<LarkResponsePagingBody<LarkContentBlock>> GetDocsBlocksAsync([Description("The URL or node token of the online doc.")] string token, [Description("A flag to control if need load all the blocks once; or false, if load the ones of the first page only.")] bool loadAllPages, CancellationToken cancellationToken = default)
    {
        var resp = await GetDocsBlocksAsync(token, new LarkPageTokenInfo(50), cancellationToken);
        if (loadAllPages) await LarkApiUtils.LoadAllPagesAsync(resp, 50, GetDocsBlocksAsync, cancellationToken).CountAsync(cancellationToken);
        return resp;
    }

    /// <summary>
    /// Gets the content of the specific doc.
    /// </summary>
    /// <param name="response">The previous response to list the resources.</param>
    /// <param name="pageSize">The page size to load each page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The content of the doc.</returns>
    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public Task<IReadOnlyList<LarkContentBlock>> GetDocsBlocksAsync(LarkResponsePagingBody<LarkContentBlock> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.DocsBlocks, (response?.Query as LarkResourceIdRequest)?.Id), response!, pageSize, cancellationToken);

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

    public Task<LarkResponseBody<LarkDocsDocInfo>> CreateDocsNodeAsync(string containerToken, string title, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.DocsInfo, new()
        {
            { "folder_token", containerToken },
            { "title", title },
        }, json => json?.DeserializeValue<LarkDocsDocInfo>("document")!, cancellationToken);

    public Task<LarkResponseBody<LarkDocsNodeInfo>> CreateDocsNodeAsync(LarkWikiNodesCreateRequestOptions options, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, options.SpaceId), JsonObjectNode.ConvertFrom(options), json => json?.DeserializeValue<LarkDocsNodeInfo>("node")!, cancellationToken);

    public Task<LarkResponseBody<LarkDocsBaseTableInfo>> GetBaseTableAsync(string baseId, CancellationToken cancellationToken = default)
        => GetAsync<LarkDocsBaseTableInfo>(string.Concat(LarkUrls.GetBaseTable, LarkUrls.GetId(baseId)), "app", cancellationToken);

    public Task<LarkResponsePagingBody<LarkDocsBaseTableTableInfo>> ListBaseTableTablesAsync(string baseId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsBaseTableTableInfo>(LarkUrls.ToUrl(LarkUrls.ListBaseTableTables, LarkUrls.GetId(baseId)), new LarkResourceIdRequest(LarkUrls.GetId(baseId)), paging, cancellationToken);

    public Task<IReadOnlyList<LarkDocsBaseTableTableInfo>> ListBaseTableTablesAsync(LarkResponsePagingBody<LarkDocsBaseTableTableInfo> response, int? pageSize, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.ListBaseTableTables, LarkUrls.GetId((response.Query as LarkResourceIdRequest)?.Id)), response, pageSize, cancellationToken);

    public Task<LarkResponsePagingBody<LarkDocsBaseTableViewInfo>> ListBaseTableViewsAsync(string baseId, string tableId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsBaseTableViewInfo>(LarkUrls.ToUrl(LarkUrls.ListBaseTableViews, paging, LarkUrls.GetId(baseId), tableId), cancellationToken);

    public Task<LarkResponsePagingBody<LarkDocsBaseTableViewInfo>> GetBaseTableViewAsync(string baseId, string tableId, string viewId, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsBaseTableViewInfo>(LarkUrls.ToUrl(LarkUrls.GetBaseTableView, LarkUrls.GetId(baseId), tableId, viewId), cancellationToken);

    public Task<LarkResponsePagingBody<LarkDocsBaseTableFieldInfo>> ListBaseTableFieldsAsync(string baseId, string tableId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsBaseTableFieldInfo>(LarkUrls.ToUrl(LarkUrls.ListBaseTableTables, LarkUrls.GetId(baseId), tableId), new LarkResourceIdRequest(LarkUrls.GetId(baseId), tableId), paging, cancellationToken);

    public Task<IReadOnlyList<LarkDocsBaseTableFieldInfo>> ListBaseTableFieldsAsync(LarkResponsePagingBody<LarkDocsBaseTableFieldInfo> response, int? pageSize, CancellationToken cancellationToken = default)
    {
        if (response?.Query is not LarkResourceIdRequest info || string.IsNullOrWhiteSpace(info?.Id) || string.IsNullOrWhiteSpace(info.Text)) return Task.FromResult<IReadOnlyList<LarkDocsBaseTableFieldInfo>>([]);
        return GetItemsAsync(LarkUrls.ToUrl(LarkUrls.ListBaseTableTables, LarkUrls.GetId(info.Id), info.Text), response, pageSize, cancellationToken);
    }

    public Task<LarkResponseBody<LarkDocsBaseTableRecordsInfo>> GetBaseTableRecordsAsync(string baseId, string tableId, IEnumerable<string> recordIds, CancellationToken cancellationToken = default)
        => PostAsync<LarkDocsBaseTableRecordsInfo>(LarkUrls.ToUrl(LarkUrls.GetBaseTableRecords, LarkUrls.GetId(baseId), tableId), new JsonObjectNode()
        {
            { "record_ids", recordIds },
            { "automatic_fields", true },
            { "with_shared_url", true }
        }, cancellationToken);

    public Task<LarkResponseBody> RenameBaseTableAsync(string baseId, string tableId, string title, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.RenameBaseTable, baseId, tableId), new JsonObjectNode
        {
            { "name", title },
        }, cancellationToken);

    public Task<LarkResponsePagingBody<LarkDocsBaseTableRecord>> ReadBaseTableAsync(string baseId, string tableId, CancellationToken cancellationToken = default)
        => ReadBaseTableAsync(new LarkDocsBaseTableFilter(baseId, tableId), null, cancellationToken);

    public async Task<LarkResponsePagingBody<LarkDocsBaseTableRecord>> ReadBaseTableAsync(LarkDocsBaseTableFilter options, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(options?.BaseId) || string.IsNullOrWhiteSpace(options.TableId)) return new(true);
        var http = CreateJsonHttpClient();
        var resp = await http.PostAsync(LarkUrls.ToUrl(LarkUrls.ReadBaseTable, paging, options.BaseId, options.TableId), options.ToJson(), cancellationToken);
        return new(options, resp, json => new(json));
    }

    /// <summary>
    /// Lists the records of a specific table in Lark Base (former named Bitable).
    /// </summary>
    /// <param name="baseId">The Lark Base app token (doc token).</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="paging">The paging options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The records of the base table.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableRecord>> ReadBaseTableAsync(string baseId, string tableId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => ReadBaseTableAsync(new LarkDocsBaseTableFilter(baseId, tableId), paging, cancellationToken);

    /// <summary>
    /// Lists the records of a specific table in Lark Base (former named Bitable).
    /// </summary>
    /// <param name="response">The response of previous page.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The records of the base table.</returns>
    public async Task<IReadOnlyList<LarkDocsBaseTableRecord>> ReadBaseTableAsync(LarkResponsePagingBody<LarkDocsBaseTableRecord> response, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var query = response.Query as LarkDocsBaseTableFilter;
        if (string.IsNullOrWhiteSpace(query?.BaseId) || string.IsNullOrWhiteSpace(query.TableId)) return [];
        var paging = response.NextPageInfo(pageSize);
        if (paging is null) return [];
        var resp = await http.PostAsync(LarkUrls.ToUrl(LarkUrls.ReadBaseTable, paging, query.BaseId, query.TableId), query.ToJson(), cancellationToken);
        return response.AddRange(resp);
    }

    /// <summary>
    /// Lists the records of a specific table in Lark Base (former named Bitable).
    /// </summary>
    /// <param name="baseId">The Lark Base app token (doc token).</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="filter">The optional simple filter.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The records of the base table.</returns>
    [Description("List the records in Lark Base (former named Bitable). Return 100 records at most.")]
    public async Task<LarkResponsePagingBody<LarkDocsBaseTableRecord>> ReadBaseTableAsync(
    [Description("The Lark Base instance identifier (token node).")] string baseId,
    [Description("The table (sheet) identifier. A Lark Base instance may include one or more table instance.")] string tableId,
    [Description("The optional filter and sort options.")] LarkBaseTableSimpleFilter? filter,
    CancellationToken cancellationToken = default)
    {
        LarkResponsePagingBody<LarkDocsBaseTableRecord> resp;
        if (string.IsNullOrWhiteSpace(filter?.FilterPropertyValue) && string.IsNullOrWhiteSpace(filter?.SortPropertyName))
        {
            resp = await ReadBaseTableAsync(baseId, tableId, new LarkPageTokenInfo(100), cancellationToken);
        }
        else
        {
            var filter2 = new LarkDocsBaseTableFilter(baseId, tableId);
            if (!string.IsNullOrWhiteSpace(filter.FilterPropertyName))
                filter2.SetFilter(filter.FilterPropertyName, filter.FilterOperator ?? "is", filter.FilterPropertyValue);
            if (!string.IsNullOrWhiteSpace(filter.SortPropertyName))
                filter2.SetOrder(filter.SortPropertyName, filter.SortByDesc);
            resp = await ReadBaseTableAsync(filter2, new LarkPageTokenInfo(100), cancellationToken);
        }

        return resp;
    }

    public async Task<LarkResponseBody<LarkDocsBaseTableRecord>> InsertBaseTableRecordAsync(string baseId, string tableId, JsonObjectNode fields, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.PostAsync(LarkUrls.ToUrl(LarkUrls.InsertBaseTableRecord, options, baseId, tableId), new JsonObjectNode()
        {
            { "fields", fields }
        }, cancellationToken);
        return new(resp, "record");
    }

    public async Task<LarkResponseBody<List<LarkDocsBaseTableRecord>>> InsertBaseTableRecordAsync(string baseId, string tableId, IEnumerable<JsonObjectNode> fields, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.PostAsync(LarkUrls.ToUrl(LarkUrls.InsertBaseTableRecords, options, baseId, tableId), new JsonObjectNode()
        {
            { "records", fields.Select(ele => new JsonObjectNode
            {
            { "fields", fields },
            }) },
        }, cancellationToken);
        return new(resp, "records");
    }

    public async Task<LarkResponseBody<LarkDocsBaseTableRecord>> UpdateBaseTableRecordAsync(string baseId, string tableId, string recordId, JsonObjectNode fields, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.PutAsync(LarkUrls.ToUrl(LarkUrls.UpdateBaseTableRecord, options, baseId, tableId, recordId), new JsonObjectNode()
        {
            { "fields", fields }
        }, cancellationToken);
        return new(resp, "record");
    }

    public async Task<LarkResponseBody<LarkDocsBaseTableRecordDeletionInfo>> DeleteBaseTableRecordAsync(string baseId, string tableId, string recordId, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.SendAsync(HttpMethod.Delete, LarkUrls.ToUrl(LarkUrls.UpdateBaseTableRecord, options, baseId, tableId, recordId), cancellationToken);
        return new(resp);
    }

    public async Task<LarkResponseBody<List<LarkDocsBaseTableRecordDeletionInfo>>> DeleteBaseTableRecordAsync(string baseId, string tableId, IEnumerable<string> recordId, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.PostAsync(LarkUrls.ToUrl(LarkUrls.DeleteBaseTableRecords, options, baseId, tableId), new JsonObjectNode
        {
            { "records", recordId },
        }, cancellationToken);
        return new(resp, "records");
    }

    public Task<LarkResponsePagingBody> ListDocsComments(LarkDocsCommentListOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.DocsComments, options.DocToken), options, paging, cancellationToken);

    public Task<IReadOnlyList<JsonObjectNode>> ListDocsComments(LarkResponsePagingBody response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.DocsComments, (response.Query as LarkDocsCommentListOptions)?.DocToken), response, pageSize, cancellationToken);

    public Task<LarkResponseBody> ReplyDocsComment(LarkDocsCommentReplyOptions options, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.DocsReplyComment, options, options.DocToken, options.CommentId), options.ToJson(), cancellationToken);

    public async Task<LarkResponseBody<string>> UploadDocsFileAsync(string name, FileInfo file, string parentToken, string? mime = null, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        content.Add("file_name", name);
        content.Add("parent_type", "wiki");
        content.Add("parent_node", LarkUrls.GetId(parentToken));
        content.Add("size", file.Length);
        content.Add("file", file, name, mime);
        var resp = await HttpClient.PostAsync(LarkUrls.UploadFile, content, cancellationToken);
        var json = await HttpClientExtensions.DeserializeJsonAsync<JsonObjectNode>(resp.Content, cancellationToken);
        return new(json, json =>
        {
            return json.TryGetStringTrimmedValue("file_token");
        });
    }

    public async Task<LarkResponseBody<string>> UploadDocsFileAsync(string token, string name, FileInfo file, string parentToken, string? mime = null, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        content.Add("file_token", token);
        content.Add("file_name", name);
        content.Add("parent_type", "wiki");
        content.Add("parent_node", LarkUrls.GetId(parentToken));
        content.Add("size", file.Length);
        content.Add("file", file, name, mime);
        var resp = await HttpClient.PostAsync(LarkUrls.UploadFile, content, cancellationToken);
        var json = await HttpClientExtensions.DeserializeJsonAsync<JsonObjectNode>(resp.Content, cancellationToken);
        return new(json, json =>
        {
            return json.TryGetStringTrimmedValue("file_token");
        });
    }

    public async Task<Stream> DownloadFileAsync(string token, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient<Stream>();
        var resp = await http.GetAsync(LarkUrls.ToUrl(LarkUrls.DownloadFile, token), cancellationToken);
        return resp;
    }

    public async Task<LarkResponseBody<LarkDocsFileTextResponse>> ReadDocsTextFileAsync(LarkResponseBody<LarkDocsNodeInfo> node, CancellationToken cancellationToken = default)
    {
        if (node?.Data is null || node.IsError) return new(true, node?.Message);
        if (string.IsNullOrWhiteSpace(node.Data.DocToken)) return new(true, "Cannot find file identifier.");
        var file = await DownloadFileAsync(node.Data.DocToken, cancellationToken);
        if (file is null) return new(true, "Empty file.");
        if (!file.CanRead) return new(true, "Cannot read the stream.");
        using var reader = new StreamReader(file, Encoding.UTF8);
        var s = await reader.ReadToEndAsync();
        return new(new JsonObjectNode()
        {
            { "code", 0 },
            { "data", new JsonObjectNode()
            {
                { "value", s },
                { "node", (node as LarkResponseBody).Data },
            } },
            { "msg", "OK" },
        }, raw => new(node.Data, s));
    }

    /// <summary>
    /// Gets the text content of an onine file in wiki.
    /// </summary>
    /// <param name="token">The node token of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The file text and doc node information.</returns>
    [Description("Get the text content of an onine file in wiki.")]
    public async Task<LarkResponseBody<LarkDocsFileTextResponse>> ReadDocsTextFileAsync([Description("The node token of the file.")] string token, CancellationToken cancellationToken = default)
    {
        var node = await GetWikiNodeAsync(token, cancellationToken);
        return await ReadDocsTextFileAsync(node, cancellationToken);
    }

    public Task<LarkResponseBody<string>> ConvertDocsFileFormatAsync(string token, string ext, string docType, string parentToken, string? name = null, CancellationToken cancellationToken = default)
    {
        var json = new JsonObjectNode()
        {
            { "file_extension", ext },
            { "file_token", token },
            { "type", docType },
            { "point", new JsonObjectNode
            {
                { "mount_type", 1 },
                { "mount_key", parentToken },
            } }
        };
        json.SetValueIfNotEmpty("file_name", name);
        return PostAsync(LarkUrls.ConvertDocsFileFormat, json, data =>
        {
            return data.TryGetStringValue("ticket");
        }, cancellationToken);
    }

    public Task<LarkResponseBody> ConvertDocsFileFormatStateAsync(string ticket, CancellationToken cancellationToken = default)
        => GetAsync(string.Concat(LarkUrls.ConvertDocsFileFormatState, ticket), cancellationToken);

    public Task<LarkResponseBody> ConvertDocsBlocksAsync(string? mime, string content, LarkUserIdTypeRequestOptions? options = null, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.ConvertDocsBlocks, options), new()
        {
            { "content_type", mime ?? "markdown" },
            { "content", content },
        }, cancellationToken);

    /// <summary>
    /// Gets the whiteboard nodes.
    /// </summary>
    /// <param name="id">The whiteboard identifier, or the whiteboard reference token in docs tree block.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The whiteboard nodes.</returns>
    [Description("Get the whiteboard nodes.")]
    public Task<LarkResponseBody> GetDocsWhiteboardNodes([Description("The whiteboard identifier, or the whiteboard reference token in docs tree block.")] string id, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.DocsWhiteboardNodes, id), cancellationToken);

    /// <summary>
    /// Gets the whiteboard nodes.
    /// </summary>
    /// <param name="id">The whiteboard identifier, or the whiteboard reference token in docs tree block.</param>
    /// <param name="options">The additional options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The whiteboard nodes.</returns>
    public Task<LarkResponseBody> GetDocsWhiteboardNodes(string id, LarkUserIdTypeRequestOptions options, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.DocsWhiteboardNodes, options, id), cancellationToken);

    /// <summary>
    /// Gets the whiteboard as image.
    /// </summary>
    /// <param name="id">The whiteboard identifier, or the whiteboard reference token in docs tree block.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The whiteboard screenshot.</returns>
    [Description("Get the whiteboard as an image.")]
    public async Task<Stream> GetDocsWhiteboardAsImage([Description("The whiteboard identifier, or the whiteboard reference token in docs tree block..")] string id, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient<Stream>();
        var resp = await http.GetAsync(LarkUrls.ToUrl(LarkUrls.DocsWhiteboardImage, id), cancellationToken);
        return resp;
    }

    public Task<LarkResponseBody> AddDocsBlocksAsync(string documentId, string blockId, List<string> blockChildrenIds, int blockIndex, List<JsonObjectNode> descendants, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.AddDocsBlocks, documentId, blockId), new()
        {
            { "children_id", blockChildrenIds },
            { "index", blockIndex },
            { "descendants", descendants },
        }, cancellationToken);
    
    public async Task<LarkResponseBody> UpdateDocsAsync(LarkApi larkApi, string nodeToken, string md, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(md)) return new(true, "The markdown is empty.");
        var blocksTask = larkApi.ConvertDocsBlocksAsync("markdown", md, null, cancellationToken);
        var nodeInfo = await larkApi.GetWikiNodeAsync(nodeToken, cancellationToken);
        var blocks = await blocksTask;
        if (string.IsNullOrWhiteSpace(nodeInfo?.Data?.DocToken) || nodeInfo.IsError)
            return new(true, nodeInfo?.Message);
        if (blocks?.Data is null || blocks.IsError)
            return new(true, blocks?.Message);
        var resp = await larkApi.AddDocsBlocksAsync(nodeInfo.Data.DocToken, nodeInfo.Data.DocToken, blocks.Data.TryGetStringListValue("first_level_block_ids", true), 0, blocks.Data.TryGetObjectListValue("blocks", true), cancellationToken);
        return resp;
    }

    /// <summary>
    /// Reads the content of the specific online doc.
    /// </summary>
    /// <param name="token">The node token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The doc content.</returns>
    [Description("Read the content of the specific online doc.")]
    public async Task<LarkDocContent> GetDocsNodeContentAsync([Description("The URL or node token of the online doc.")] string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token)) return LarkApiUtils.ErrorLarkDocContent(null, "The node token is null.");
        token = LarkUrls.GetId(token)!;
        var info = await GetWikiNodeAsync(token, cancellationToken);
        if (string.IsNullOrWhiteSpace(info?.Data?.DocType) || info.IsError) return LarkApiUtils.ErrorLarkDocContent(token, info?.Message ?? "Get node failed.");
        switch (info.Data.DocType)
        {
            case "doc":
            case "docx":
            case "docs":
                {
                    var doc = await GetDocsBlocksAsync(token, true, cancellationToken);
                    if (doc?.Data is null || doc.IsError) return LarkApiUtils.ErrorLarkDocContent(token, doc?.Message ?? "Get doc content failed.");
                    var tree = doc.Data.ToTree();
                    return new LarkDocContent<LarkContentBlockTree>(token, info.Data.Name, info.Data.DocToken, "docx", tree);
                }
            case "file":
                {
                    var file = await ReadDocsTextFileAsync(info, cancellationToken);
                    return LarkApiUtils.ToDocContent(file, info.Data, "Load file text error.");
                }
            case "bitable":
                {
                    var table = await GetBaseTableAsync(token, cancellationToken);
                    if (table?.Data is null || table.IsError) return LarkApiUtils.ErrorLarkDocContent(token, table?.Message ?? "Get base table info failed.");
                    var tables = await ListBaseTableTablesAsync(token, new(50), cancellationToken);
                    await LarkApiUtils.LoadAllPagesAsync(tables, 50, ListBaseTableTablesAsync, cancellationToken).CountAsync(cancellationToken);
                    return new LarkDocContent<LarkDocsBaseTableFullInfo>(info.Data, new(table.Data, tables?.Data?.ToList()));
                }
            default:
                return new LarkDocContent<string>(info.Data, "Unsupported format.");
        }
    }
}
