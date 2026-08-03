using LarkSuite.Docs;
using LarkSuite.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Collection;
using Trivial.Text;

namespace LarkSuite.OapiModels;

/// <summary>
/// The utilities of Lark doc content.
/// </summary>
public static partial class LarkApiUtils
{
    /// <summary>
    /// Gets the content block by identifier.
    /// </summary>
    /// <param name="col">The content block collection.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>The content block with the specific identifier; or null, if not found.</returns>
    public static BaseLarkOkrItemInfo? Get(this IList<LarkOkrObjectiveInfo> col, string id)
    {
        if (col is null || string.IsNullOrWhiteSpace(id)) return null;
        id = id.Trim().ToUpperInvariant();
        if (id.StartsWith('O')) id = id[1..];
        var sep = "KR";
        var i = id.IndexOf(sep);
        if (i < 0)
        {
            sep = "-";
            i = id.IndexOf(sep);
            if (i < 0)
            {
                if (!int.TryParse(id, out var o) || o < 1 || o > col.Count) return null;
                return col[o - 1];
            }
        }

        var oStr = id[..i].Trim().TrimEnd('-').TrimEnd();
        id = id[(i + sep.Length)..].Trim();
        if (!int.TryParse(oStr, out var oIndex) || oIndex < 1 || oIndex > col.Count) return null;
        var oItem = col[oIndex - 1];
        if (oItem?.KeyResults is null || !int.TryParse(id, out var kr) || kr < 1 || kr > oItem.KeyResults.Count) return null;
        return oItem.KeyResults[kr - 1];
    }

    public static LarkOkrCycleItem? GetById(this IEnumerable<LarkOkrCycleItem> col, string id)
    {
        if (col is null || string.IsNullOrWhiteSpace(id)) return null;
        foreach (var item in col)
        {
            if (item.Id == id) return item;
        }

        return null;
    }

    public static Task<LarkOkrCycleItem?> GetOkrCycleAsync(LarkApi larkApi, LarkUserOkrInfo okr, CancellationToken cancellationToken = default)
        => okr is null ? Task.FromResult<LarkOkrCycleItem?>(null) : GetOkrCycleAsync(larkApi, okr.UserId, okr.CycleId, cancellationToken);

    public static async Task<LarkOkrCycleItem?> GetOkrCycleAsync(LarkApi larkApi, string userId, string cycleId, CancellationToken cancellationToken = default)
    {
        larkApi ??= LarkApi.DefaultInstance;
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(cycleId)) return null;
        var response = await larkApi.ListOkrPeriodsAsync(userId, new(50), cancellationToken);
        if (response?.Data is null || response.IsError) return null;
        var item = GetById(response.Data, cycleId);
        if (item is not null) return item;
        while (response.HasNextPage)
        {
            var col = await larkApi.ListOkrPeriodsAsync(response, 50, cancellationToken);
            if (col is null) break;
            item = GetById(response.Data, cycleId);
            if (item is not null) return item;
        }

        return null;
    }

    public static LarkOkrCycleItem? GetOkrCycleByTenantCycle(this IEnumerable<LarkOkrCycleItem> col, string tenantCycleId)
    {
        if (col is null || string.IsNullOrWhiteSpace(tenantCycleId)) return null;
        foreach (var item in col)
        {
            if (item.TenantCycleId == tenantCycleId) return item;
        }

        return null;
    }

    public static async Task<LarkOkrCycleItem?> GetOkrCycleByTenantCycleAsync(LarkApi larkApi, string userId, string tenantCycleId, CancellationToken cancellationToken = default)
    {
        var periods = await larkApi.ListOkrPeriodsAsync(userId, new(50), cancellationToken);
        return await GetOkrCycleByTenantCycleAsync(larkApi, periods, tenantCycleId);
    }

    public static async Task<LarkOkrCycleItem?> GetOkrCycleByTenantCycleAsync(LarkApi larkApi, LarkResponsePagingBody<LarkOkrCycleItem> response, string tenantCycleId, CancellationToken cancellationToken = default)
    {
        if (response?.Data is null || response.IsError || string.IsNullOrWhiteSpace(tenantCycleId)) return null;
        larkApi ??= LarkApi.DefaultInstance;
        var item = GetOkrCycleByTenantCycle(response.Data, tenantCycleId);
        if (item is not null) return item;
        while (response.HasNextPage)
        {
            var col = await larkApi.ListOkrPeriodsAsync(response, 50, cancellationToken);
            if (col is null) break;
            item = GetOkrCycleByTenantCycle(response.Data, tenantCycleId);
            if (item is not null) return item;
        }

        return null;
    }

    public static async Task<LarkUserOkrInfo?> GetOkrByTenantCycleAsync(LarkApi larkApi, string userId, string tenantCycleId, CancellationToken cancellationToken = default)
    {
        larkApi ??= LarkApi.DefaultInstance;
        var response = await larkApi.ListOkrPeriodsAsync(userId, new(50), cancellationToken);
        var cycle = await GetOkrCycleByTenantCycleAsync(larkApi, response, tenantCycleId, cancellationToken);
        if (string.IsNullOrWhiteSpace(cycle?.Id)) return null;
        return await larkApi.GetOkrsAsync(cycle.Id, cancellationToken);
    }

    public static IEnumerable<LarkOkrKeyResultInfo> ListOkrKeyResults(this IEnumerable<LarkOkrObjectiveInfo> objectives)
    {
        foreach (var objective in objectives)
        {
            foreach (var kr in objective.KeyResults)
            {
                yield return kr;
            }
        }
    }

    public static async IAsyncEnumerable<LarkOkrKeyResultInfo> ListOkrKeyResultsAsync(this IAsyncEnumerable<LarkOkrObjectiveInfo> objectives)
    {
        await foreach (var objective in objectives)
        {
            foreach (var kr in objective.KeyResults)
            {
                yield return kr;
            }
        }
    }
}
