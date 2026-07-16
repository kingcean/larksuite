using LarkSuite.OapiModels;
using LarkSuite.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using Trivial.Text;

namespace LarkSuite;

public partial class LarkApi
{
    public Task<LarkResponsePagingBody<LarkOkrCycleItem>> ListOkrPeriodsAsync(string id, LarkPageTokenInfo? request = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrCycleItem>(LarkUrls.OkrPeriods, new LarkUserOwnedResourcesRequest(id), request, cancellationToken);

    public Task<LarkResponsePagingBody<LarkOkrCycleItem>> ListOkrPeriodsAsync(LarkUserOwnedResourcesRequest options, LarkPageTokenInfo request, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrCycleItem>(LarkUrls.OkrPeriods, options, request, cancellationToken);

    public Task<IReadOnlyList<LarkOkrCycleItem>> ListOkrPeriodsAsync(LarkResponsePagingBody<LarkOkrCycleItem> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.OkrPeriods, response, pageSize, cancellationToken);

    public Task<LarkResponsePagingBody<LarkOkrObjectiveItem>> ListOkrObjectivesAsync(string id, LarkPageTokenInfo? request = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrObjectiveItem>(LarkUrls.ToUrl(LarkUrls.OkrObjectives, id), new LarkTargetResourcesRequest(id), request, cancellationToken);

    public Task<LarkResponsePagingBody<LarkOkrObjectiveItem>> ListOkrObjectivesAsync(string id, LarkUserIdTypeRequestOptions options, LarkPageTokenInfo request, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrObjectiveItem>(LarkUrls.ToUrl(LarkUrls.OkrObjectives, id), new LarkTargetResourcesRequest(options, id), request, cancellationToken);

    public Task<IReadOnlyList<LarkOkrObjectiveItem>> ListOkrObjectivesAsync(LarkResponsePagingBody<LarkOkrObjectiveItem> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.OkrObjectives, (response.Query as LarkTargetResourcesRequest)?.Id), response, pageSize, cancellationToken);

    public Task<LarkResponseBody<LarkOkrObjectiveItem>> GetOkrObjectiveAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkOkrObjectiveItem>(LarkUrls.ToUrl(LarkUrls.OkrObjectiveDetails, id), cancellationToken);

    public Task<LarkResponseBody<LarkOkrObjectiveItem>> GetOkrObjectiveAsync(string id, LarkUserIdTypeRequestOptions options, CancellationToken cancellationToken = default)
        => GetAsync<LarkOkrObjectiveItem>(LarkUrls.ToUrl(LarkUrls.OkrObjectiveDetails, options, id), cancellationToken);

    public async Task<LarkOkrObjectiveItem?> GetOkrObjectiveAsync(LarkOkrObjectiveInfo info, CancellationToken cancellationToken = default)
    {
        if (info?.Source is not null) return info.Source;
        if (string.IsNullOrWhiteSpace(info?.Id)) return null;
        var resp = await GetOkrObjectiveAsync(info.Id, cancellationToken);
        if (resp?.Data is null || resp.IsError) return null;
        return resp.Data;
    }

    public Task<LarkResponsePagingBody<LarkOkrProgressItem>> GetOkrObjectiveProgressAsync(string id, LarkPageTokenInfo? request = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrProgressItem>(LarkUrls.ToUrl(LarkUrls.OkrObjectiveProgress, id), new LarkTargetResourcesRequest(id), request, cancellationToken);

    public Task<LarkResponsePagingBody<LarkOkrProgressItem>> GetOkrObjectiveProgressAsync(string id, LarkUserIdTypeRequestOptions options, LarkPageTokenInfo request, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrProgressItem>(LarkUrls.ToUrl(LarkUrls.OkrObjectiveProgress, id), new LarkTargetResourcesRequest(options, id), request, cancellationToken);

    public Task<IReadOnlyList<LarkOkrProgressItem>> GetOkrObjectiveProgressAsync(LarkResponsePagingBody<LarkOkrProgressItem> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.OkrObjectiveProgress, (response.Query as LarkTargetResourcesRequest)?.Id), response, pageSize, cancellationToken);

    public Task<LarkResponsePagingBody<LarkOkrKeyResultItem>> ListOkrKeyResultsAsync(string id, LarkPageTokenInfo? request = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrKeyResultItem>(LarkUrls.ToUrl(LarkUrls.OkrKeyResults, id), new LarkTargetResourcesRequest(id), request, cancellationToken);

    public Task<LarkResponsePagingBody<LarkOkrKeyResultItem>> ListOkrKeyResultsAsync(string id, LarkUserIdTypeRequestOptions options, LarkPageTokenInfo request, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrKeyResultItem>(LarkUrls.ToUrl(LarkUrls.OkrKeyResults, id), new LarkTargetResourcesRequest(options, id), request, cancellationToken);

    public Task<IReadOnlyList<LarkOkrKeyResultItem>> ListOkrKeyResultsAsync(LarkResponsePagingBody<LarkOkrKeyResultItem> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.OkrKeyResults, (response.Query as LarkTargetResourcesRequest)?.Id), response, pageSize, cancellationToken);

    public Task<LarkResponseBody<LarkOkrKeyResultItem>> GetOkrKeyResultAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkOkrKeyResultItem>(LarkUrls.ToUrl(LarkUrls.OkrKeyResultDetails, id), cancellationToken);

    public Task<LarkResponseBody<LarkOkrKeyResultItem>> GetOkrKeyResultAsync(string id, LarkUserIdTypeRequestOptions options, CancellationToken cancellationToken = default)
        => GetAsync<LarkOkrKeyResultItem>(LarkUrls.ToUrl(LarkUrls.OkrKeyResultDetails, options, id), cancellationToken);

    public async Task<LarkOkrKeyResultItem?> GetOkrKeyResultAsync(LarkOkrKeyResultInfo info, CancellationToken cancellationToken = default)
    {
        if (info?.Source is not null) return info.Source;
        if (string.IsNullOrWhiteSpace(info?.Id)) return null;
        var resp = await GetOkrKeyResultAsync(info.Id, cancellationToken);
        if (resp?.Data is null || resp.IsError) return null;
        return resp.Data;
    }

    public Task<LarkResponsePagingBody<LarkOkrProgressItem>> GetOkrKeyResultProgressAsync(string id, LarkPageTokenInfo? request = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrProgressItem>(LarkUrls.ToUrl(LarkUrls.OkrKeyResultProgress, id), new LarkTargetResourcesRequest(id), request, cancellationToken);

    public Task<LarkResponsePagingBody<LarkOkrProgressItem>> GetOkrKeyResultProgressAsync(string id, LarkUserIdTypeRequestOptions options, LarkPageTokenInfo request, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkOkrProgressItem>(LarkUrls.ToUrl(LarkUrls.OkrKeyResultProgress, id), new LarkTargetResourcesRequest(options, id), request, cancellationToken);

    public Task<IReadOnlyList<LarkOkrProgressItem>> GetOkrKeyResultProgressAsync(LarkResponsePagingBody<LarkOkrProgressItem> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.OkrKeyResultProgress, (response.Query as LarkTargetResourcesRequest)?.Id), response, pageSize, cancellationToken);

    public async IAsyncEnumerable<LarkOkrObjectiveInfo> GetOkrsAsync(IEnumerable<LarkOkrObjectiveItem> objectives, CancellationToken cancellationToken = default)
    {
        var list = GetOkrsInternalAsync(objectives, cancellationToken).OrderBy(ele => ele.Position);
        await foreach (var item in list)
        {
            yield return item;
        }
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

    public async IAsyncEnumerable<LarkOkrObjectiveInfo> GetOkrsAsync(string cycleId, CancellationToken cancellationToken = default)
    {
        var objectives = await ListOkrObjectivesAsync(cycleId, new(50), cancellationToken);
        var result = GetOkrsAsync(objectives, cancellationToken);
        await foreach (var item in result)
        {
            yield return item;
        }
    }

    public async IAsyncEnumerable<LarkOkrObjectiveInfo> GetOkrsAsync(LarkResponsePagingBody<LarkOkrObjectiveItem> objectives, CancellationToken cancellationToken = default)
    {
        if (objectives?.Data is null || objectives.IsError) yield break;
        while (objectives.HasNextPage)
        {
            var list = await ListOkrObjectivesAsync(objectives, 50, cancellationToken);
            if (list is null || list.Count < 1) break;
        }

        var result = GetOkrsAsync(objectives.Data, cancellationToken);
        await foreach (var item in result)
        {
            yield return item;
        }
    }
}
