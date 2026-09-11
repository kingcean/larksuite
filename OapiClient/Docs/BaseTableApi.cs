using LarkSuite.Docs;
using LarkSuite.OapiModels;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
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
    /// Gets the information of a Lark Base (bitable) instance.
    /// </summary>
    /// <param name="baseId">The Lark Base app token or URL.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The Lark Base instance information.</returns>
    public Task<LarkResponseBody<LarkDocsBaseTableInfo>> GetBaseTableAsync(string baseId, CancellationToken cancellationToken = default)
        => GetAsync<LarkDocsBaseTableInfo>(string.Concat(LarkUrls.GetBaseTable, LarkUrls.GetId(baseId)), "app", cancellationToken);

    /// <summary>
    /// Gets the tables (sheets) of the specific Lark Base (bitable) instance.
    /// </summary>
    /// <param name="baseId">The node token of Lark Base instance.</param>
    /// <param name="paging">The paging options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The tables of Lark Base.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableTableInfo>> ListBaseTableTablesAsync(string baseId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsBaseTableTableInfo>(LarkUrls.ToUrl(LarkUrls.ListBaseTableTables, LarkUrls.GetId(baseId)), new LarkResourceIdRequest(LarkUrls.GetId(baseId)), paging, cancellationToken);

    /// <summary>
    /// Gets the tables (sheets) of the specific Lark Base (bitable) instance.
    /// </summary>
    /// <param name="baseId">The node token of Lark Base instance.</param>
    /// <param name="loadAllPages">true if load all tables; otherwise, false, to load the ones in the first page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The tables of Lark Base.</returns>
    public async Task<LarkResponsePagingBody<LarkDocsBaseTableTableInfo>> ListBaseTableTablesAsync([Description("The node token or URL of Lark Base instance.")] string baseId, [Description("A value indicating whether load all tables: true if return all; otherwise, false, to load the ones in the first page.")] bool loadAllPages, CancellationToken cancellationToken = default)
    {
        var paging = new LarkPageTokenInfo(50);
        if (!loadAllPages) return await ListBaseTableTablesAsync(baseId, paging, cancellationToken);
        var resp = await ListBaseTableTablesAsync(baseId, paging, cancellationToken);
        await LarkApiUtils.LoadAllPagesAsync(resp, paging.Size, ListBaseTableTablesAsync, cancellationToken).CountAsync(cancellationToken);
        return resp;
    }

    /// <summary>
    /// Gets the tables (sheets) of the specific Lark Base (bitable) instance.
    /// </summary>
    /// <param name="response">The response of previous (or the first) page.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The tables of Lark Base.</returns>
    public Task<IReadOnlyList<LarkDocsBaseTableTableInfo>> ListBaseTableTablesAsync(LarkResponsePagingBody<LarkDocsBaseTableTableInfo> response, int? pageSize, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.ListBaseTableTables, LarkUrls.GetId((response.Query as LarkResourceIdRequest)?.Id)), response, pageSize, cancellationToken);

    /// <summary>
    /// Lists a page of views of a table in Lark Base.
    /// </summary>
    /// <param name="baseId">The Lark Base app token or URL.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The table views for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableViewInfo>> ListBaseTableViewsAsync(string baseId, string tableId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsBaseTableViewInfo>(LarkUrls.ToUrl(LarkUrls.ListBaseTableViews, paging, LarkUrls.GetId(baseId), tableId), cancellationToken);

    /// <summary>
    /// Requests the information of a specific table view in Lark Base.
    /// </summary>
    /// <param name="baseId">The Lark Base app token or URL.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="viewId">The view identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The view query result wrapped in a paging response.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableViewInfo>> GetBaseTableViewAsync(string baseId, string tableId, string viewId, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsBaseTableViewInfo>(LarkUrls.ToUrl(LarkUrls.GetBaseTableView, LarkUrls.GetId(baseId), tableId, viewId), cancellationToken);

    /// <summary>
    /// Lists a page of fields of a table in Lark Base.
    /// </summary>
    /// <param name="baseId">The Lark Base app token or URL.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The table fields for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableFieldInfo>> ListBaseTableFieldsAsync(string baseId, string tableId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsBaseTableFieldInfo>(LarkUrls.ToUrl(LarkUrls.GetBaseTableFields, LarkUrls.GetId(baseId), tableId), new LarkResourceIdRequest(LarkUrls.GetId(baseId), tableId), paging, cancellationToken);

    /// <summary>
    /// Loads the next page of table fields and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the Lark Base and table identifiers.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The fields loaded from the next page, or an empty list if the request identifiers or next page are unavailable.</returns>
    public Task<IReadOnlyList<LarkDocsBaseTableFieldInfo>> ListBaseTableFieldsAsync(LarkResponsePagingBody<LarkDocsBaseTableFieldInfo> response, int? pageSize, CancellationToken cancellationToken = default)
    {
        if (response?.Query is not LarkResourceIdRequest info || string.IsNullOrWhiteSpace(info?.Id) || string.IsNullOrWhiteSpace(info.Text)) return Task.FromResult<IReadOnlyList<LarkDocsBaseTableFieldInfo>>([]);
        return GetItemsAsync(LarkUrls.ToUrl(LarkUrls.GetBaseTableFields, LarkUrls.GetId(info.Id), info.Text), response, pageSize, cancellationToken);
    }

    /// <summary>
    /// Lists the fields of a table in Lark Base, optionally loading all pages.
    /// </summary>
    /// <param name="baseId">The Lark Base app token or URL.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="loadAllPages">true to load all pages; otherwise, false, to load only the first page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The table fields from the requested pages.</returns>
    public async Task<LarkResponsePagingBody<LarkDocsBaseTableFieldInfo>> ListBaseTableFieldsAsync(string baseId, string tableId, bool loadAllPages, CancellationToken cancellationToken = default)
    {
        var paging = new LarkPageTokenInfo(50);
        if (!loadAllPages) return await ListBaseTableFieldsAsync(baseId, tableId, paging, cancellationToken);
        var resp = await ListBaseTableFieldsAsync(baseId, tableId, paging, cancellationToken);
        await LarkApiUtils.LoadAllPagesAsync(resp, paging.Size, ListBaseTableFieldsAsync, cancellationToken).CountAsync(cancellationToken);
        return resp;
    }

    /// <summary>
    /// Gets specific table records by their identifiers, requesting automatic fields and shared URLs.
    /// </summary>
    /// <param name="baseId">The Lark Base app token or URL.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordIds">The record identifiers to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the requested records.</returns>
    public Task<LarkResponseBody<LarkDocsBaseTableRecordsInfo>> GetBaseTableRecordsAsync(string baseId, string tableId, IEnumerable<string> recordIds, CancellationToken cancellationToken = default)
        => PostAsync<LarkDocsBaseTableRecordsInfo>(LarkUrls.ToUrl(LarkUrls.GetBaseTableRecords, LarkUrls.GetId(baseId), tableId), new JsonObjectNode()
        {
            { "record_ids", recordIds },
            { "automatic_fields", true },
            { "with_shared_url", true }
        }, cancellationToken);

    /// <summary>
    /// Gets a specific record from a table in Lark Base.
    /// </summary>
    /// <param name="baseId">The Lark Base app token or URL.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordId">The record identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The requested record, or an error response if an identifier is missing or the record cannot be retrieved.</returns>
    public async Task<LarkResponseBody<LarkDocsBaseTableRecord>> GetBaseTableRecordAsync(string baseId, string tableId, string recordId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(baseId)) return new(true, "The Lark Base identifier (node token) should not be empty.");
        if (string.IsNullOrWhiteSpace(tableId)) return new(true, "The table identifier should not be empty.");
        if (string.IsNullOrWhiteSpace(recordId)) return new(true, "The record identifier should not be empty.");
        var records = await GetBaseTableRecordsAsync(baseId, tableId, [recordId], cancellationToken);
        if (records is null) return new(true, "Resolve data failed.");
        if (records.Data is null || records.IsError) return new(true, records.Message ?? "No data resolved.");
        var record = records.GetById(recordId);
        if (record is null) return new(true, "Cannot find the record by the specific identifier");
        return new(records.Code, records.Message, record, JsonObjectNode.ConvertFrom(record));
    }

    /// <summary>
    /// Requests renaming a table in Lark Base.
    /// </summary>
    /// <param name="baseId">The Lark Base app token.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="title">The new table name.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response of the table rename request.</returns>
    public Task<LarkResponseBody> RenameBaseTableAsync(string baseId, string tableId, string title, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.RenameBaseTable, baseId, tableId), new JsonObjectNode
        {
            { "name", title },
        }, cancellationToken);

    /// <summary>
    /// Lists the first page of records of a table in Lark Base.
    /// </summary>
    /// <param name="baseId">The Lark Base app token (doc token).</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The first page of table records.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableRecord>> ReadBaseTableAsync(string baseId, string tableId, CancellationToken cancellationToken = default)
        => ReadBaseTableAsync(new LarkDocsBaseTableFilter(baseId, tableId), null, cancellationToken);

    /// <summary>
    /// Queries a page of table records using the specified filter options.
    /// </summary>
    /// <param name="options">The Lark Base and table identifiers, filters, and sort options.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching records for the requested page, or an error response if the Lark Base or table identifier is missing.</returns>
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

    /// <summary>
    /// Creates a record in a table in Lark Base.
    /// </summary>
    /// <param name="baseId">The Lark Base app token.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="fields">The field names and values for the new record.</param>
    /// <param name="options">The optional record request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the created record.</returns>
    public async Task<LarkResponseBody<LarkDocsBaseTableRecord>> InsertBaseTableRecordAsync(string baseId, string tableId, JsonObjectNode fields, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.PostAsync(LarkUrls.ToUrl(LarkUrls.InsertBaseTableRecord, options, baseId, tableId), new JsonObjectNode()
        {
            { "fields", fields }
        }, cancellationToken);
        return new(resp, "record");
    }

    /// <summary>
    /// Requests creating multiple records in a table in Lark Base.
    /// </summary>
    /// <param name="baseId">The Lark Base app token.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="fields">The collection of field name and value objects for the new records.</param>
    /// <param name="options">The optional record request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The batch creation response containing the returned records.</returns>
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

    /// <summary>
    /// Updates the fields of a record in a table in Lark Base.
    /// </summary>
    /// <param name="baseId">The Lark Base app token.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordId">The record identifier.</param>
    /// <param name="fields">The field names and values to update.</param>
    /// <param name="options">The optional record request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the updated record.</returns>
    public async Task<LarkResponseBody<LarkDocsBaseTableRecord>> UpdateBaseTableRecordAsync(string baseId, string tableId, string recordId, JsonObjectNode fields, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.PutAsync(LarkUrls.ToUrl(LarkUrls.UpdateBaseTableRecord, options, baseId, tableId, recordId), new JsonObjectNode()
        {
            { "fields", fields }
        }, cancellationToken);
        return new(resp, "record");
    }

    /// <summary>
    /// Deletes a record from a table in Lark Base.
    /// </summary>
    /// <param name="baseId">The Lark Base app token.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordId">The record identifier.</param>
    /// <param name="options">The optional record request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The record deletion result.</returns>
    public async Task<LarkResponseBody<LarkDocsBaseTableRecordDeletionInfo>> DeleteBaseTableRecordAsync(string baseId, string tableId, string recordId, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.SendAsync(HttpMethod.Delete, LarkUrls.ToUrl(LarkUrls.UpdateBaseTableRecord, options, baseId, tableId, recordId), cancellationToken);
        return new(resp);
    }

    /// <summary>
    /// Requests deleting a table record with a document revision identifier and optional client token.
    /// </summary>
    /// <param name="baseId">The Lark Base app token.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordId">The record identifier.</param>
    /// <param name="revision">The document revision identifier sent with the deletion request.</param>
    /// <param name="clientToken">The optional client token sent with the deletion request.</param>
    /// <param name="options">The optional record request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The record deletion result.</returns>
    public async Task<LarkResponseBody<LarkDocsBaseTableRecordDeletionInfo>> DeleteBaseTableRecordAsync(string baseId, string tableId, string recordId, int revision, string? clientToken = null, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var q = new QueryData
        {
            { "document_revision_id", revision },
        };
        q.SetIfNotEmpty("client_token", clientToken);
        var resp = await http.SendAsync(HttpMethod.Delete, LarkUrls.ToUrl(q.ToString(LarkUrls.UpdateBaseTableRecord), options, baseId, tableId, recordId), cancellationToken);
        return new(resp);
    }

    /// <summary>
    /// Deletes multiple records from a table in Lark Base.
    /// </summary>
    /// <param name="baseId">The Lark Base app token.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordId">The identifiers of the records to delete.</param>
    /// <param name="options">The optional record request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The deletion results for the requested records.</returns>
    public async Task<LarkResponseBody<List<LarkDocsBaseTableRecordDeletionInfo>>> DeleteBaseTableRecordAsync(string baseId, string tableId, IEnumerable<string> recordId, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.PostAsync(LarkUrls.ToUrl(LarkUrls.DeleteBaseTableRecords, options, baseId, tableId), new JsonObjectNode
        {
            { "records", recordId },
        }, cancellationToken);
        return new(resp, "records");
    }
}
