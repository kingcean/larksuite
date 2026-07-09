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
    public Task<LarkResponsePagingBody> GetInterviews(LarkInterviewOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.Interviews, options, paging, cancellationToken);

    public Task<IReadOnlyList<JsonObjectNode>> GetInterviews(LarkResponsePagingBody response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.Interviews, response, pageSize, cancellationToken);

    public Task<LarkResponsePagingBody> GetInterviews(LarkInterviewByTelentOptions options, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.InterviewByTalent, options, null, cancellationToken);

    public Task<LarkResponsePagingBody<LarkInterviewMinuteInfo>> GetInterviewMinutes(LarkInterviewOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.InterviewMinutes, options, paging, LarkInterviewMinuteInfo.Deserialize, obj =>
        {
            return obj.TryGetObjectValue("minutes")?.TryGetObjectListValue("sentences");
        }, cancellationToken);

    public Task<LarkResponsePagingBody<LarkInterviewMinuteInfo>> GetInterviewMinutes(string id, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetInterviewMinutes(new LarkInterviewOptions
        {
            Id = id
        }, paging, cancellationToken);

    public Task<IReadOnlyList<LarkInterviewMinuteInfo>> GetInterviewMinutes(LarkResponsePagingBody<LarkInterviewMinuteInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.InterviewMinutes, response, obj =>
        {
            return obj.TryGetObjectValue("minutes")?.TryGetObjectListValue("sentences");
        }, pageSize, cancellationToken);

    public Task<LarkResponseBody<LarkHireTalentInfo>> GetHireTalent(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkHireTalentInfo>(string.Concat(LarkUrls.HireTalent, id), cancellationToken);

    public Task<LarkResponsePagingBody<LarkHireTalentInfo>> SearchHireTalents(string keyword, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkHireTalentInfo>(LarkUrls.HireTalents, new LarkTalentSearchOptions(keyword), paging, cancellationToken);

    public Task<LarkResponsePagingBody<LarkHireTalentInfo>> SearchHireTalents(LarkTalentSearchOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkHireTalentInfo>(LarkUrls.HireTalents, options, paging, cancellationToken);

    public Task<IReadOnlyList<LarkHireTalentInfo>> SearchHireTalents(LarkResponsePagingBody<LarkHireTalentInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.HireTalents, response, pageSize, cancellationToken);

    public Task<LarkResponseBody> GetHireApplicationInfo(string id, CancellationToken cancellationToken = default)
        => GetAsync(string.Concat(LarkUrls.Applications, id), cancellationToken);

    public Task<LarkResponseBody<LarkHireTalentInfo>> GetHireApplicationDetails(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkHireTalentInfo>(LarkUrls.ToUrl(LarkUrls.ApplicationDetails, id), cancellationToken);

    public Task<LarkResponsePagingBody> GetHireApplications(LarkHireApplicationOptions options, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.Applications, options, paging, cancellationToken);

    public Task<IReadOnlyList<JsonObjectNode>> GetHireApplications(LarkResponsePagingBody response, int? pageSize, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.Applications, response, pageSize, cancellationToken);
}
