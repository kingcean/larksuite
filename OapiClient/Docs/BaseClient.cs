using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using Trivial.Text;

namespace LarkSuite.Docs;

/// <summary>
/// The client of Lark Base (bitable) instance.
/// The instance has no OpenAPI instance stored.
/// </summary>
public class LarkBaseClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LarkBaseClient"/> class with the specified base identifier.
    /// </summary>
    /// <param name="baseId">The Lark Base (bitable) instance identifier.</param>
    public LarkBaseClient(string baseId)
    {
        BaseId = baseId;
    }

    /// <summary>
    /// Gets the Lark Base (bitable) instance identifier. It is also the node token in wiki space.
    /// </summary>
    public string BaseId { get; }

    public LarkDocsBaseTableFilter CreateFilter(string tableId)
        => new(BaseId, tableId);

    public LarkDocsBaseTableFilter CreateFilter(string tableId, LarkDocsFilterCondition condition)
        => new(BaseId, tableId, condition);

    public LarkDocsBaseTableFilter CreateFilter(string tableId, LarkDocsFilter filter, List<LarkDocsSortItem> sort)
        => new(BaseId, tableId, filter, sort);

    public LarkDocsBaseTableFilter CreateFilter(string tableId, LarkDocsFilter filter, LarkDocsSortItem sort)
        => new(BaseId, tableId, filter, sort);

    public LarkDocsBaseTableFilter CreateFilter(string tableId, string viewId)
        => new(BaseId, tableId)
        {
            ViewId = viewId,
        };

    /// <summary>
    /// Gets the tables (sheets) of the specific Lark Base (bitable) instance.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="paging">The paging options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The tables of Lark Base.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableTableInfo>> ListTablesAsync(LarkApi? larkApi, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).ListBaseTableTablesAsync(BaseId, paging, cancellationToken);

    /// <summary>
    /// Gets views of a table in Lark Base.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The table views for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableViewInfo>> ListViewsAsync(LarkApi? larkApi, string tableId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).ListBaseTableViewsAsync(BaseId, tableId, paging, cancellationToken);

    /// <summary>
    /// Requests the information of a specific table view in Lark Base.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="viewId">The view identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The view query result wrapped in a paging response.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableViewInfo>> GetViewAsync(LarkApi? larkApi, string tableId, string viewId, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).GetBaseTableViewAsync(BaseId, tableId, viewId, cancellationToken);

    /// <summary>
    /// Gets fields of a table in Lark Base.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The table fields for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableFieldInfo>> ListFieldsAsync(LarkApi? larkApi, string tableId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).ListBaseTableFieldsAsync(BaseId, tableId, paging, cancellationToken);

    /// <summary>
    /// Gets fields of a table in Lark Base.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="loadAllPages">true to load all pages; otherwise, false, to load only the first page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The table fields for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableFieldInfo>> ListFieldsAsync(LarkApi? larkApi, string tableId, bool loadAllPages, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).ListBaseTableFieldsAsync(BaseId, tableId, loadAllPages, cancellationToken);

    /// <summary>
    /// Gets a specific record from a table in Lark Base.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordId">The record identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The requested record, or an error response if an identifier is missing or the record cannot be retrieved.</returns>
    public Task<LarkResponseBody<LarkDocsBaseTableRecord>> GetRecordAsync(LarkApi? larkApi, string tableId, string recordId, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).GetBaseTableRecordAsync(BaseId, tableId, recordId, cancellationToken);

    /// <summary>
    /// Gets specific table records by their identifiers, requesting automatic fields and shared URLs.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordId">The record identifier to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the requested records.</returns>
    public Task<LarkResponseBody<LarkDocsBaseTableRecordsInfo>> GetRecordsAsync(LarkApi? larkApi, string tableId, string recordId, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).GetBaseTableRecordsAsync(BaseId, tableId, [recordId], cancellationToken);

    /// <summary>
    /// Gets specific table records by their identifiers, requesting automatic fields and shared URLs.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordIds">The record identifiers to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the requested records.</returns>
    public Task<LarkResponseBody<LarkDocsBaseTableRecordsInfo>> GetRecordsAsync(LarkApi? larkApi, string tableId, IEnumerable<string> recordIds, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).GetBaseTableRecordsAsync(BaseId, tableId, recordIds, cancellationToken);

    /// <summary>
    /// Requests renaming a table in Lark Base.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="title">The new table name.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response of the table rename request.</returns>
    public Task<LarkResponseBody> RenameTableAsync(LarkApi? larkApi, string tableId, string title, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).RenameBaseTableAsync(BaseId, tableId, title, cancellationToken);

    /// <summary>
    /// Lists the first page of records of a table in Lark Base.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The first page of table records.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableRecord>> ReadAsync(LarkApi? larkApi, string tableId, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).ReadBaseTableAsync(BaseId, tableId, new LarkPageTokenInfo(500), cancellationToken);

    /// <summary>
    /// Lists the records of a specific table in Lark Base (former named Bitable).
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching records for the requested page, or an error response if the Lark Base or table identifier is missing.</returns>
    public Task<LarkResponsePagingBody<LarkDocsBaseTableRecord>> ReadAsync(LarkApi? larkApi, string tableId, LarkPageTokenInfo? paging, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).ReadBaseTableAsync(BaseId, tableId, paging, cancellationToken);

    /// <summary>
    /// Creates a record in a table in Lark Base.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="fields">The field names and values for the new record.</param>
    /// <param name="options">The optional record request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the created record.</returns>
    public Task<LarkResponseBody<LarkDocsBaseTableRecord>> InsertRecordAsync(LarkApi? larkApi, string tableId, JsonObjectNode fields, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).InsertBaseTableRecordAsync(BaseId, tableId, fields, options, cancellationToken);

    /// <summary>
    /// Requests creating multiple records in a table in Lark Base.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="fields">The collection of field name and value objects for the new records.</param>
    /// <param name="options">The optional record request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The batch creation response containing the returned records.</returns>
    public Task<LarkResponseBody<List<LarkDocsBaseTableRecord>>> InsertRecordAsync(LarkApi? larkApi, string tableId, IEnumerable<JsonObjectNode> fields, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).InsertBaseTableRecordAsync(BaseId, tableId, fields, options, cancellationToken);

    /// <summary>
    /// Updates the fields of a record in a table in Lark Base.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordId">The record identifier.</param>
    /// <param name="fields">The field names and values to update.</param>
    /// <param name="options">The optional record request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the updated record.</returns>
    public Task<LarkResponseBody<LarkDocsBaseTableRecord>> UpdateRecordAsync(LarkApi? larkApi, string tableId, string recordId, JsonObjectNode fields, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).UpdateBaseTableRecordAsync(BaseId, tableId, recordId, fields, options, cancellationToken);

    /// <summary>
    /// Deletes a record from a table in Lark Base.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordId">The record identifier.</param>
    /// <param name="options">The optional record request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The record deletion result.</returns>
    public Task<LarkResponseBody<LarkDocsBaseTableRecordDeletionInfo>> DeleteRecordAsync(LarkApi? larkApi, string tableId, string recordId, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).DeleteBaseTableRecordAsync(BaseId, tableId, recordId, options, cancellationToken);

    /// <summary>
    /// Requests deleting a table record with a document revision identifier and optional client token.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordId">The record identifier.</param>
    /// <param name="revision">The document revision identifier sent with the deletion request.</param>
    /// <param name="clientToken">The optional client token sent with the deletion request.</param>
    /// <param name="options">The optional record request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The record deletion result.</returns>
    public Task<LarkResponseBody<LarkDocsBaseTableRecordDeletionInfo>> DeleteRecordAsync(LarkApi? larkApi, string tableId, string recordId, int revision, string? clientToken = null, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).DeleteBaseTableRecordAsync(BaseId, tableId, recordId, revision, clientToken, options, cancellationToken);

    /// <summary>
    /// Deletes multiple records from a table in Lark Base.
    /// </summary>
    /// <param name="larkApi">The Lark OpenAPI instance.</param>
    /// <param name="tableId">The table identifier.</param>
    /// <param name="recordId">The identifiers of the records to delete.</param>
    /// <param name="options">The optional record request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The deletion results for the requested records.</returns>
    public Task<LarkResponseBody<List<LarkDocsBaseTableRecordDeletionInfo>>> DeleteRecordAsync(LarkApi? larkApi, string tableId, IEnumerable<string> recordId, LarkDocsBaseTableRecordOptions? options = null, CancellationToken cancellationToken = default)
        => (larkApi ?? LarkApi.DefaultInstance).DeleteBaseTableRecordAsync(BaseId, tableId, recordId, options, cancellationToken);

    public Task<LarkDocsBaseTableRecord<T>> GetAndSimplifyAsync<T>(LarkApi? larkApi, string tableId, string recordId, Dictionary<string, string> mapping, CancellationToken cancellationToken = default)
        => LarkApiUtils.SimplifyAsync<T>(larkApi, BaseId, tableId, recordId, mapping, cancellationToken);

    public Task<LarkDocsBaseTableRecord<JsonObjectNode>> GetAndSimplifyAsync(LarkApi? larkApi, string tableId, string recordId, Dictionary<string, string> mapping, CancellationToken cancellationToken = default)
        => LarkApiUtils.SimplifyAsync(larkApi, BaseId, tableId, recordId, mapping, cancellationToken);
}
