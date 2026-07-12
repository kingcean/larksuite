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

    public Task<LarkResponseBody<LarkDocsBaseTableInfo>> GetDocsInfoAsync(string token, CancellationToken cancellationToken = default)
        => GetAsync<LarkDocsBaseTableInfo>(string.Concat(LarkUrls.DocsInfo, LarkUrls.GetId(token)), "document", cancellationToken);

    public async Task<LarkResponseBody<string>> GetDocsTextAsync(string token, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.GetAsync(LarkUrls.ToUrl(LarkUrls.DocsText, LarkUrls.GetId(token)), cancellationToken);
        return new(resp);
    }

    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public Task<LarkResponsePagingBody<LarkContentBlock>> GetDocsBlocksAsync(string token, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkContentBlock>(LarkUrls.ToUrl(LarkUrls.DocsBlocks, LarkUrls.GetId(token)), new LarkResourceIdRequest(LarkUrls.GetId(token)), null, json => new(json), cancellationToken);

    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public Task<LarkResponsePagingBody<LarkContentBlock>> GetDocsBlocksAsync(string token, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkContentBlock>(LarkUrls.ToUrl(LarkUrls.DocsBlocks, LarkUrls.GetId(token)), new LarkResourceIdRequest(LarkUrls.GetId(token)), paging, json => new(json), cancellationToken);

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

    public Task<LarkResponseBody<LarkDocsBaseTableInfo>> GetBaseTableAsync(string baseId, CancellationToken cancellationToken = default)
        => GetAsync<LarkDocsBaseTableInfo>(string.Concat(LarkUrls.GetBaseTable, LarkUrls.GetId(baseId)), "app", cancellationToken);

    public Task<LarkResponsePagingBody<LarkDocsBaseTableTableInfo>> ListBaseTableTablesAsync(string baseId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsBaseTableTableInfo>(LarkUrls.ToUrl(LarkUrls.ListBaseTableTables, LarkUrls.GetId(baseId)), new LarkResourceIdRequest(LarkUrls.GetId(baseId)), paging, cancellationToken);

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

    public Task<LarkResponsePagingBody<LarkDocsBaseTableRecord>> ReadBaseTableAsync(string baseId, string tableId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => ReadBaseTableAsync(new LarkDocsBaseTableFilter(baseId, tableId), paging, cancellationToken);

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
}
