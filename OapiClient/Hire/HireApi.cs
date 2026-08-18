using LarkSuite.OapiModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Data;
using Trivial.Net;
using Trivial.Text;

namespace LarkSuite;

public partial class LarkApi
{
    public async Task<LarkResponseBody<LarkHireInterviewInfo>> GetInterviewAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) return new(true, "The interview ID should not be null or empty.");
        var list = await ListInterviewsAsync(new LarkInterviewOptions
        {
            Id = id,
        }, new LarkPageTokenInfo(), cancellationToken);
        if (list is null) return new(true, "Access resource failed.");
        if (list.IsError || list.Data is null) return new(true, list.Message);
        foreach (var item in list.Data)
        {
            if (item.Id == id) return new(list.Code, list.Message, item, JsonObjectNode.ConvertFrom(item));
        }

        if (list.Count == 1 && list.Data[0] is not null) return new(list.Code, list.Message, list.Data[0], JsonObjectNode.ConvertFrom(list.Data[0]));
        return new(true, list.Message);
    }

    public Task<LarkResponsePagingBody<LarkHireInterviewInfo>> ListInterviewsAsync(LarkInterviewOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkHireInterviewInfo>(LarkUrls.Interviews, options, paging, cancellationToken);

    public Task<IReadOnlyList<LarkHireInterviewInfo>> ListInterviewsAsync(LarkResponsePagingBody<LarkHireInterviewInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.Interviews, response, pageSize, cancellationToken);

    public Task<LarkResponsePagingBody<LarkHireApplicationInterviewInfo>> ListInterviewsAsync(LarkInterviewByTelentOptions options, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkHireApplicationInterviewInfo>(LarkUrls.InterviewByTalent, options, null, cancellationToken);

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

    public Task<LarkResponseBody<LarkAttachmentInfo>> GetAttachmentAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkAttachmentInfo>(string.Concat(LarkUrls.Attachments, id), "attachment", cancellationToken);

    public Task<LarkResponseBody<LarkHireTalentInfo>> GetHireTalentAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkHireTalentInfo>(string.Concat(LarkUrls.HireTalent, id), cancellationToken);

    public Task<LarkResponseBody<LarkHireTalentInfo>> GetHireTalentAsync(LarkHireApplicationInfo application, CancellationToken cancellationToken = default)
    {
        if (application is null) return Task.FromResult(new LarkResponseBody<LarkHireTalentInfo>(true, "The job application should not be null."));
        var id = application.Info?.TalentId ?? application.Talent?.Id;
        if (string.IsNullOrWhiteSpace(id)) return Task.FromResult(new LarkResponseBody<LarkHireTalentInfo>(true, "Cannot get the job identifier from the application info."));
        return GetHireTalentAsync(id, cancellationToken);
    }

    public async Task<LarkResponseBody<LarkHireTalentInfo>> GetHireTalentAsync(LarkHireInterviewInfo interview, CancellationToken cancellationToken = default)
    {
        if (interview is null) return new(true, "The hire interview instance should not be null.");
        if (string.IsNullOrWhiteSpace(interview.Id)) return new(true, "The hire interview indentifier should not be null.");
        var application = await GetHireApplicationAsync(interview.Id, cancellationToken);
        if (application?.Data is null || application.IsError) return new(true, application?.Message ?? "Get job application failed.");
        return await GetHireTalentAsync(application.Data, cancellationToken);
    }

    public Task<LarkResponsePagingBody<LarkHireTalentInfo>> SearchHireTalentsAsync(string keyword, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkHireTalentInfo>(LarkUrls.HireTalents, new LarkTalentSearchOptions(keyword), paging, cancellationToken);

    public Task<LarkResponsePagingBody<LarkHireTalentInfo>> SearchHireTalentsAsync(LarkTalentSearchOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkHireTalentInfo>(LarkUrls.HireTalents, options, paging, cancellationToken);

    public Task<IReadOnlyList<LarkHireTalentInfo>> SearchHireTalentsAsync(LarkResponsePagingBody<LarkHireTalentInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.HireTalents, response, pageSize, cancellationToken);

    public Task<LarkResponseBody<LarkHireApplicationInfo>> GetHireApplicationAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkHireApplicationInfo>(LarkUrls.ToUrl(LarkUrls.ApplicationDetails, id), "application_detail", cancellationToken);

    public Task<LarkResponseBody<LarkHireApplicationInfo>> GetHireApplicationAsync(string id, LarkHireApplicationDetailsOptions options, CancellationToken cancellationToken = default)
        => GetAsync<LarkHireApplicationInfo>(LarkUrls.ToUrl(LarkUrls.ApplicationDetails, options, id), "application_detail", cancellationToken);

    public Task<LarkResponsePagingBody<string>> ListHireApplicationsAsync(LarkHireApplicationSearchOptions options, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<string>(LarkUrls.Applications, options, paging, cancellationToken);

    public Task<IReadOnlyList<string>> ListHireApplicationsAsync(LarkResponsePagingBody<string> response, int? pageSize, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.Applications, response, pageSize, cancellationToken);

    public Task<LarkResponseBody> GetHireJobAsync(LarkHireApplicationInfo application, CancellationToken cancellationToken = default)
    {
        if (application is null) return Task.FromResult(new LarkResponseBody(true, "The job application should not be null."));
        var id = application.Info?.JobId ?? application.Job?.TryGetStringTrimmedValue("id");
        if (string.IsNullOrWhiteSpace(id)) return Task.FromResult(new LarkResponseBody(true, "Cannot get the job identifier from the application info."));
        return GetHireJobAsync(id, cancellationToken);
    }

    public async Task<LarkResponseBody> GetHireJobAsync(LarkHireInterviewInfo interview, CancellationToken cancellationToken = default)
    {
        if (interview is null) return new(true, "The hire interview instance should not be null.");
        if (string.IsNullOrWhiteSpace(interview.Id)) return new(true, "The hire interview indentifier should not be null.");
        var application = await GetHireApplicationAsync(interview.Id, cancellationToken);
        if (application?.Data is null || application.IsError) return new(true, application?.Message ?? "Get job application failed.");
        return await GetHireJobAsync(application.Data, cancellationToken);
    }

    public Task<LarkResponseBody> GetHireJobAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.HireJob, id), "job_detail", cancellationToken);

    public Task<LarkResponseBody> GetHireJobAsync(string id, LarkUserIdTypeRequestOptions options, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.HireJob, options, id), "job_detail", cancellationToken);

    public Task<LarkResponseBody> GetHireOfferByApplicationAsync(string id, LarkHireBasicResourceOptions? options = null, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.HireOfferByApplication, options, id), "offer", cancellationToken);

    public Task<LarkResponseBody> GetHireOfferAsync(string id, LarkHireBasicResourceOptions? options = null, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.HireOfferDetails, options, id), "offer", cancellationToken);
}
