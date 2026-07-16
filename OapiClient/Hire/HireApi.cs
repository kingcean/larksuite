using LarkSuite.OapiModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Net;
using Trivial.Text;

namespace LarkSuite;

public partial class LarkApi
{
    public async Task<LarkResponseBody> GetInterviewAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) return new(true, "The interview ID should not be null or empty.");
        var list = await GetInterviewsAsync(new LarkInterviewOptions
        {
            Id = id,
        }, new LarkPageTokenInfo(), cancellationToken);
        if (list is null) return new(true, "Access resource failed.");
        if (list.IsError || list.Data is null) return new(true, list.Message);
        foreach (var item in list.Data)
        {
            if (item?.TryGetStringTrimmedValue("id", true) == id) return new(list.Code, list.Message, item);
        }

        if (list.Count == 1 && list.Data[0] is not null) return new(list.Code, list.Message, list.Data[0]);
        return new(true, list.Message);
    }

    public Task<LarkResponsePagingBody> GetInterviewsAsync(LarkInterviewOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.Interviews, options, paging, cancellationToken);

    public Task<IReadOnlyList<JsonObjectNode>> GetInterviewsAsync(LarkResponsePagingBody response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.Interviews, response, pageSize, cancellationToken);

    public Task<LarkResponsePagingBody> GetInterviewsAsync(LarkInterviewByTelentOptions options, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.InterviewByTalent, options, null, cancellationToken);

    public Task<LarkResponsePagingBody<LarkInterviewMinuteInfo>> GetInterviewMinutesAsync(LarkInterviewOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.InterviewMinutes, options, paging, LarkInterviewMinuteInfo.Deserialize, obj =>
        {
            return obj.TryGetObjectValue("minutes")?.TryGetObjectListValue("sentences");
        }, cancellationToken);

    public Task<LarkResponsePagingBody<LarkInterviewMinuteInfo>> GetInterviewMinutesAsync(string id, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetInterviewMinutesAsync(new LarkInterviewOptions
        {
            Id = id
        }, paging, cancellationToken);

    public Task<IReadOnlyList<LarkInterviewMinuteInfo>> GetInterviewMinutesAsync(LarkResponsePagingBody<LarkInterviewMinuteInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.InterviewMinutes, response, obj =>
        {
            return obj.TryGetObjectValue("minutes")?.TryGetObjectListValue("sentences");
        }, pageSize, cancellationToken);

    public Task<LarkResponseBody<LarkHireTalentInfo>> GetHireTalentAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkHireTalentInfo>(string.Concat(LarkUrls.HireTalent, id), cancellationToken);

    public Task<LarkResponsePagingBody<LarkHireTalentInfo>> SearchHireTalentsAsync(string keyword, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkHireTalentInfo>(LarkUrls.HireTalents, new LarkTalentSearchOptions(keyword), paging, cancellationToken);

    public Task<LarkResponsePagingBody<LarkHireTalentInfo>> SearchHireTalentsAsync(LarkTalentSearchOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkHireTalentInfo>(LarkUrls.HireTalents, options, paging, cancellationToken);

    public Task<IReadOnlyList<LarkHireTalentInfo>> SearchHireTalentsAsync(LarkResponsePagingBody<LarkHireTalentInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.HireTalents, response, pageSize, cancellationToken);

    public Task<LarkResponseBody> GetHireApplicationInfoAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync(string.Concat(LarkUrls.Applications, id), cancellationToken);

    public Task<LarkResponseBody> GetHireApplicationDetailsAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.ApplicationDetails, id), "application_detail", cancellationToken);

    public Task<LarkResponsePagingBody> GetHireApplicationsAsync(LarkHireApplicationOptions options, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.Applications, options, paging, cancellationToken);

    public Task<IReadOnlyList<JsonObjectNode>> GetHireApplicationsAsync(LarkResponsePagingBody response, int? pageSize, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.Applications, response, pageSize, cancellationToken);

    public Task<LarkResponseBody> GetHireJobAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.HireJob, id), "job_detail", cancellationToken);

    public Task<LarkResponseBody> GetHireJobAsync(string id, LarkUserIdTypeRequestOptions options, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.HireJob, options, id), "job_detail", cancellationToken);
}
