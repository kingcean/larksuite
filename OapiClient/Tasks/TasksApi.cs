using LarkSuite.OapiModels;
using LarkSuite.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using Trivial.Text;

namespace LarkSuite;

public partial class LarkApi
{
    /// <summary>
    /// Gets OKR periods owned by a user.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="request">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The OKR periods for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkOkrCycleItem>> ListOkrPeriodsAsync(string id, LarkPageTokenInfo? request = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrCycleItem>(LarkUrls.OkrPeriods, new LarkUserOwnedResourcesRequest(id), request, cancellationToken);

    /// <summary>
    /// Gets OKR periods using the specified user resource request options.
    /// </summary>
    /// <param name="options">The user identifier and resource request options.</param>
    /// <param name="request">The page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The OKR periods for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkOkrCycleItem>> ListOkrPeriodsAsync(LarkUserOwnedResourcesRequest options, LarkPageTokenInfo request, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrCycleItem>(LarkUrls.OkrPeriods, options, request, cancellationToken);

    /// <summary>
    /// Loads the next page of OKR periods and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the period request options.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The OKR periods loaded from the next page.</returns>
    public Task<IReadOnlyList<LarkOkrCycleItem>> ListOkrPeriodsAsync(LarkResponsePagingBody<LarkOkrCycleItem> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.OkrPeriods, response, pageSize, cancellationToken);

    /// <summary>
    /// Gets OKR objectives for a target resource.
    /// </summary>
    /// <param name="id">The target resource identifier.</param>
    /// <param name="request">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The OKR objectives for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkOkrObjectiveItem>> ListOkrObjectivesAsync(string id, LarkPageTokenInfo? request = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrObjectiveItem>(LarkUrls.ToUrl(LarkUrls.OkrObjectives, id), new LarkTargetResourcesRequest(id), request, cancellationToken);

    /// <summary>
    /// Gets OKR objectives for a target resource with user identifier type options.
    /// </summary>
    /// <param name="id">The target resource identifier.</param>
    /// <param name="options">The user identifier type request options.</param>
    /// <param name="request">The page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The OKR objectives for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkOkrObjectiveItem>> ListOkrObjectivesAsync(string id, LarkUserIdTypeRequestOptions options, LarkPageTokenInfo request, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrObjectiveItem>(LarkUrls.ToUrl(LarkUrls.OkrObjectives, id), new LarkTargetResourcesRequest(options, id), request, cancellationToken);

    /// <summary>
    /// Loads the next page of OKR objectives and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the target resource request options.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The OKR objectives loaded from the next page.</returns>
    public Task<IReadOnlyList<LarkOkrObjectiveItem>> ListOkrObjectivesAsync(LarkResponsePagingBody<LarkOkrObjectiveItem> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.OkrObjectives, (response.Query as LarkTargetResourcesRequest)?.Id), response, pageSize, cancellationToken);

    /// <summary>
    /// Gets the details of an OKR objective.
    /// </summary>
    /// <param name="id">The objective identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The OKR objective details.</returns>
    public Task<LarkResponseBody<LarkOkrObjectiveItem>> GetOkrObjectiveAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkOkrObjectiveItem>(LarkUrls.ToUrl(LarkUrls.OkrObjectiveDetails, id), cancellationToken);

    /// <summary>
    /// Gets the details of an OKR objective with user identifier type options.
    /// </summary>
    /// <param name="id">The objective identifier.</param>
    /// <param name="options">The user identifier type request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The OKR objective details.</returns>
    public Task<LarkResponseBody<LarkOkrObjectiveItem>> GetOkrObjectiveAsync(string id, LarkUserIdTypeRequestOptions options, CancellationToken cancellationToken = default)
        => GetAsync<LarkOkrObjectiveItem>(LarkUrls.ToUrl(LarkUrls.OkrObjectiveDetails, options, id), cancellationToken);

    /// <summary>
    /// Gets an OKR objective from its source item or retrieves its details when needed.
    /// </summary>
    /// <param name="info">The objective information containing a source item or objective identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The source objective when available; otherwise, the retrieved objective; or null if it cannot be resolved.</returns>
    public async Task<LarkOkrObjectiveItem?> GetOkrObjectiveAsync(LarkOkrObjectiveInfo info, CancellationToken cancellationToken = default)
    {
        if (info?.Source is not null) return info.Source;
        if (string.IsNullOrWhiteSpace(info?.Id)) return null;
        var resp = await GetOkrObjectiveAsync(info.Id, cancellationToken);
        if (resp?.Data is null || resp.IsError) return null;
        return resp.Data;
    }

    /// <summary>
    /// Gets a page of progress records for an OKR objective.
    /// </summary>
    /// <param name="id">The objective identifier.</param>
    /// <param name="request">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The objective progress records for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkOkrProgressItem>> GetOkrObjectiveProgressAsync(string id, LarkPageTokenInfo? request = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrProgressItem>(LarkUrls.ToUrl(LarkUrls.OkrObjectiveProgress, id), new LarkTargetResourcesRequest(id), request, cancellationToken);

    /// <summary>
    /// Gets a page of progress records for an OKR objective with user identifier type options.
    /// </summary>
    /// <param name="id">The objective identifier.</param>
    /// <param name="options">The user identifier type request options.</param>
    /// <param name="request">The page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The objective progress records for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkOkrProgressItem>> GetOkrObjectiveProgressAsync(string id, LarkUserIdTypeRequestOptions options, LarkPageTokenInfo request, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrProgressItem>(LarkUrls.ToUrl(LarkUrls.OkrObjectiveProgress, id), new LarkTargetResourcesRequest(options, id), request, cancellationToken);

    /// <summary>
    /// Loads the next page of objective progress records and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the target resource request options.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The objective progress records loaded from the next page.</returns>
    public Task<IReadOnlyList<LarkOkrProgressItem>> GetOkrObjectiveProgressAsync(LarkResponsePagingBody<LarkOkrProgressItem> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.OkrObjectiveProgress, (response.Query as LarkTargetResourcesRequest)?.Id), response, pageSize, cancellationToken);

    /// <summary>
    /// Gets key results for an OKR objective.
    /// </summary>
    /// <param name="id">The objective identifier.</param>
    /// <param name="request">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The key results for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkOkrKeyResultItem>> ListOkrKeyResultsAsync(string id, LarkPageTokenInfo? request = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrKeyResultItem>(LarkUrls.ToUrl(LarkUrls.OkrKeyResults, id), new LarkTargetResourcesRequest(id), request, cancellationToken);

    /// <summary>
    /// Gets key results for an OKR objective with user identifier type options.
    /// </summary>
    /// <param name="id">The objective identifier.</param>
    /// <param name="options">The user identifier type request options.</param>
    /// <param name="request">The page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The key results for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkOkrKeyResultItem>> ListOkrKeyResultsAsync(string id, LarkUserIdTypeRequestOptions options, LarkPageTokenInfo request, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrKeyResultItem>(LarkUrls.ToUrl(LarkUrls.OkrKeyResults, id), new LarkTargetResourcesRequest(options, id), request, cancellationToken);

    /// <summary>
    /// Loads the next page of key results and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the target resource request options.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The key results loaded from the next page.</returns>
    public Task<IReadOnlyList<LarkOkrKeyResultItem>> ListOkrKeyResultsAsync(LarkResponsePagingBody<LarkOkrKeyResultItem> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.OkrKeyResults, (response.Query as LarkTargetResourcesRequest)?.Id), response, pageSize, cancellationToken);

    /// <summary>
    /// Gets the details of an OKR key result.
    /// </summary>
    /// <param name="id">The key result identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The OKR key result details.</returns>
    public Task<LarkResponseBody<LarkOkrKeyResultItem>> GetOkrKeyResultAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkOkrKeyResultItem>(LarkUrls.ToUrl(LarkUrls.OkrKeyResultDetails, id), cancellationToken);

    /// <summary>
    /// Gets the details of an OKR key result with user identifier type options.
    /// </summary>
    /// <param name="id">The key result identifier.</param>
    /// <param name="options">The user identifier type request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The OKR key result details.</returns>
    public Task<LarkResponseBody<LarkOkrKeyResultItem>> GetOkrKeyResultAsync(string id, LarkUserIdTypeRequestOptions options, CancellationToken cancellationToken = default)
        => GetAsync<LarkOkrKeyResultItem>(LarkUrls.ToUrl(LarkUrls.OkrKeyResultDetails, options, id), cancellationToken);

    /// <summary>
    /// Gets an OKR key result from its source item or retrieves its details when needed.
    /// </summary>
    /// <param name="info">The key result information containing a source item or key result identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The source key result when available; otherwise, the retrieved key result; or null if it cannot be resolved.</returns>
    public async Task<LarkOkrKeyResultItem?> GetOkrKeyResultAsync(LarkOkrKeyResultInfo info, CancellationToken cancellationToken = default)
    {
        if (info?.Source is not null) return info.Source;
        if (string.IsNullOrWhiteSpace(info?.Id)) return null;
        var resp = await GetOkrKeyResultAsync(info.Id, cancellationToken);
        if (resp?.Data is null || resp.IsError) return null;
        return resp.Data;
    }

    /// <summary>
    /// Gets a page of progress records for an OKR key result.
    /// </summary>
    /// <param name="id">The key result identifier.</param>
    /// <param name="request">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The key result progress records for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkOkrProgressItem>> GetOkrKeyResultProgressAsync(string id, LarkPageTokenInfo? request = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrProgressItem>(LarkUrls.ToUrl(LarkUrls.OkrKeyResultProgress, id), new LarkTargetResourcesRequest(id), request, cancellationToken);

    /// <summary>
    /// Gets a page of progress records for an OKR key result with user identifier type options.
    /// </summary>
    /// <param name="id">The key result identifier.</param>
    /// <param name="options">The user identifier type request options.</param>
    /// <param name="request">The page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The key result progress records for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkOkrProgressItem>> GetOkrKeyResultProgressAsync(string id, LarkUserIdTypeRequestOptions options, LarkPageTokenInfo request, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrProgressItem>(LarkUrls.ToUrl(LarkUrls.OkrKeyResultProgress, id), new LarkTargetResourcesRequest(options, id), request, cancellationToken);

    /// <summary>
    /// Loads the next page of key result progress records and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the target resource request options.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The key result progress records loaded from the next page.</returns>
    public Task<IReadOnlyList<LarkOkrProgressItem>> GetOkrKeyResultProgressAsync(LarkResponsePagingBody<LarkOkrProgressItem> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.OkrKeyResultProgress, (response.Query as LarkTargetResourcesRequest)?.Id), response, pageSize, cancellationToken);

    /// <summary>
    /// Combines objectives and their key results into a user's OKR information.
    /// </summary>
    /// <param name="objectives">The objective items to enrich with their key results.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The combined user OKR information, or null if <paramref name="objectives"/> is null.</returns>
    /// <remarks>Key result pages are loaded until no next page is available.</remarks>
    public async Task<LarkUserOkrInfo?> GetOkrsAsync(IEnumerable<LarkOkrObjectiveItem> objectives, CancellationToken cancellationToken = default)
    {
        if (objectives is null) return null;
        var col = objectives is IList<LarkOkrObjectiveItem> arr ? arr : new List<LarkOkrObjectiveItem>(objectives);
        var list = await GetOkrsInternalAsync(objectives, cancellationToken).OrderBy(ele => ele.Position).ToListAsync(cancellationToken);
        var first = col.FirstOrDefault();
        return new(first?.Owner?.UserId!, first?.CycleId!, list);
    }

    /// <summary>
    /// Gets OKR information for the specified cycle.
    /// </summary>
    /// <param name="cycleId">The OKR cycle identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The combined user OKR information for the objectives in the first cycle page, or null if it cannot be resolved.</returns>
    public async Task<LarkUserOkrInfo?> GetOkrsAsync(string cycleId, CancellationToken cancellationToken = default)
    {
        var objectives = await ListOkrObjectivesAsync(cycleId, new(50), cancellationToken);
        return await GetOkrsAsync(objectives, cancellationToken);
    }

    /// <summary>
    /// Loads all objective pages and combines the objectives with their key results into user OKR information.
    /// </summary>
    /// <param name="objectives">The first or previous objective paging response.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The combined user OKR information, or null if the objective response is invalid or an error response.</returns>
    public async Task<LarkUserOkrInfo?> GetOkrsAsync(LarkResponsePagingBody<LarkOkrObjectiveItem> objectives, CancellationToken cancellationToken = default)
    {
        if (objectives?.Data is null || objectives.IsError) return null;
        await LarkApiUtils.LoadAllPagesAsync(objectives, 50, ListOkrObjectivesAsync, cancellationToken).CountAsync(cancellationToken);
        return await GetOkrsAsync(objectives.Data, cancellationToken);
    }

    private async IAsyncEnumerable<LarkOkrObjectiveInfo> GetOkrsInternalAsync(IEnumerable<LarkOkrObjectiveItem> objectives, CancellationToken cancellationToken = default)
    {
        foreach (var objective in objectives)
        {
            var objectiveId = objective?.Id;
            if (string.IsNullOrWhiteSpace(objectiveId)) continue;
            var keyResults = await ListOkrKeyResultsAsync(objectiveId, new(50), cancellationToken);
            if (keyResults?.Data is null || keyResults.IsError)
            {
                yield return new(objective!);
                continue;
            }

            while (keyResults.HasNextPage)
            {
                var list = await ListOkrKeyResultsAsync(keyResults, 50, cancellationToken);
                if (list is null || list.Count < 1) break;
            }

            var col = new List<LarkOkrKeyResultInfo>();
            var hasPosition = true;
            foreach (var keyResult in keyResults.Data)
            {
                if (keyResult.Position > 0) continue;
                hasPosition = false;
                break;
            }

            var list2 = hasPosition ? keyResults.Data.OrderBy(item => item.Position).ToList() : keyResults.Data;
            foreach (var keyResult in list2)
            {
                var keyResultId = keyResult?.Id;
                if (string.IsNullOrWhiteSpace(keyResultId)) continue;
                col.Add(new(keyResult!));
            }

            yield return new(objective!)
            {
                KeyResults = col,
            };
        }
    }
}
