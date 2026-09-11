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
    /// <summary>
    /// Gets interview information by querying the interview list with the specified identifier.
    /// </summary>
    /// <param name="id">The interview identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching interview, or an error response if the identifier is missing or no interview can be resolved.</returns>
    /// <remarks>If no identifier matches but the query returns exactly one interview, that interview is returned.</remarks>
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

    /// <summary>
    /// Lists a page of interviews matching the specified query options.
    /// </summary>
    /// <param name="options">The interview identifier, application identifier, and time range query options.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The interviews for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkHireInterviewInfo>> ListInterviewsAsync(LarkInterviewOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkHireInterviewInfo>(LarkUrls.Interviews, options, paging, cancellationToken);

    /// <summary>
    /// Loads the next page of interviews and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the interview query options.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The interviews loaded from the next page.</returns>
    public Task<IReadOnlyList<LarkHireInterviewInfo>> ListInterviewsAsync(LarkResponsePagingBody<LarkHireInterviewInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.Interviews, response, pageSize, cancellationToken);

    /// <summary>
    /// Queries interviews associated with a talent.
    /// </summary>
    /// <param name="options">The talent identifier and optional job level identifier type.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The application interview information returned for the talent.</returns>
    public Task<LarkResponsePagingBody<LarkHireApplicationInterviewInfo>> ListInterviewsAsync(LarkInterviewByTelentOptions options, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkHireApplicationInterviewInfo>(LarkUrls.InterviewByTalent, options, null, cancellationToken);

    /// <summary>
    /// Gets a page of interview minute sentences using the specified query options.
    /// </summary>
    /// <param name="options">The interview query options.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The interview minute sentences for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkInterviewMinuteInfo>> GetInterviewMinutesAsync(LarkInterviewOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.InterviewMinutes, options, paging, LarkInterviewMinuteInfo.Deserialize, obj =>
        {
            return obj.TryGetObjectValue("minutes")?.TryGetObjectListValue("sentences");
        }, cancellationToken);

    /// <summary>
    /// Gets a page of minute sentences for a specific interview.
    /// </summary>
    /// <param name="id">The interview identifier.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The interview minute sentences for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkInterviewMinuteInfo>> GetInterviewMinutesAsync(string id, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetInterviewMinutesAsync(new LarkInterviewOptions
        {
            Id = id
        }, paging, cancellationToken);

    /// <summary>
    /// Loads the next page of interview minute sentences and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the interview query options.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The interview minute sentences loaded from the next page.</returns>
    public Task<IReadOnlyList<LarkInterviewMinuteInfo>> GetInterviewMinutesAsync(LarkResponsePagingBody<LarkInterviewMinuteInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.InterviewMinutes, response, obj =>
        {
            return obj.TryGetObjectValue("minutes")?.TryGetObjectListValue("sentences");
        }, pageSize, cancellationToken);

    /// <summary>
    /// Gets the information of a recruitment attachment.
    /// </summary>
    /// <param name="id">The attachment identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The attachment information.</returns>
    public Task<LarkResponseBody<LarkAttachmentInfo>> GetAttachmentAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkAttachmentInfo>(string.Concat(LarkUrls.Attachments, id), "attachment", cancellationToken);

    /// <summary>
    /// Gets the information of a recruitment talent.
    /// </summary>
    /// <param name="id">The talent identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The talent information.</returns>
    public Task<LarkResponseBody<LarkHireTalentInfo>> GetHireTalentAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkHireTalentInfo>(string.Concat(LarkUrls.HireTalent, id), cancellationToken);

    /// <summary>
    /// Gets the talent associated with a job application.
    /// </summary>
    /// <param name="application">The job application containing the talent identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The talent information, or an error response if the application or talent identifier is unavailable.</returns>
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

    /// <summary>
    /// Searches a page of recruitment talents by name or keyword.
    /// </summary>
    /// <param name="keyword">The name or keyword to search.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching talents for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkHireTalentInfo>> SearchHireTalentsAsync(string keyword, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkHireTalentInfo>(LarkUrls.HireTalents, new LarkTalentSearchOptions(keyword), paging, cancellationToken);

    /// <summary>
    /// Searches a page of recruitment talents using the specified query options.
    /// </summary>
    /// <param name="options">The keyword and update date range search options.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The matching talents for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkHireTalentInfo>> SearchHireTalentsAsync(LarkTalentSearchOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkHireTalentInfo>(LarkUrls.HireTalents, options, paging, cancellationToken);

    /// <summary>
    /// Loads the next page of talent search results and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the talent search options.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The talents loaded from the next page.</returns>
    public Task<IReadOnlyList<LarkHireTalentInfo>> SearchHireTalentsAsync(LarkResponsePagingBody<LarkHireTalentInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.HireTalents, response, pageSize, cancellationToken);

    /// <summary>
    /// Gets the details of a job application.
    /// </summary>
    /// <param name="id">The job application identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The job application details.</returns>
    public Task<LarkResponseBody<LarkHireApplicationInfo>> GetHireApplicationAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync<LarkHireApplicationInfo>(LarkUrls.ToUrl(LarkUrls.ApplicationDetails, id), "application_detail", cancellationToken);

    /// <summary>
    /// Gets the details of a job application with additional request options.
    /// </summary>
    /// <param name="id">The job application identifier.</param>
    /// <param name="options">The application detail request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The job application details.</returns>
    public Task<LarkResponseBody<LarkHireApplicationInfo>> GetHireApplicationAsync(string id, LarkHireApplicationDetailsOptions options, CancellationToken cancellationToken = default)
        => GetAsync<LarkHireApplicationInfo>(LarkUrls.ToUrl(LarkUrls.ApplicationDetails, options, id), "application_detail", cancellationToken);

    /// <summary>
    /// Lists a page of job application identifiers matching the specified query options.
    /// </summary>
    /// <param name="options">The job application search options.</param>
    /// <param name="paging">The page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The job application identifiers for the requested page.</returns>
    public Task<LarkResponsePagingBody<string>> ListHireApplicationsAsync(LarkHireApplicationSearchOptions options, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<string>(LarkUrls.Applications, options, paging, cancellationToken);

    /// <summary>
    /// Loads the next page of job application identifiers and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the application search options.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The job application identifiers loaded from the next page.</returns>
    public Task<IReadOnlyList<string>> ListHireApplicationsAsync(LarkResponsePagingBody<string> response, int? pageSize, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.Applications, response, pageSize, cancellationToken);

    /// <summary>
    /// Gets the job associated with a job application.
    /// </summary>
    /// <param name="application">The job application containing the job identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The job details, or an error response if the application or job identifier is unavailable.</returns>
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

    /// <summary>
    /// Gets the details of a recruitment job.
    /// </summary>
    /// <param name="id">The job identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The job details.</returns>
    public Task<LarkResponseBody> GetHireJobAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.HireJob, id), "job_detail", cancellationToken);

    /// <summary>
    /// Gets the details of a recruitment job with user identifier type options.
    /// </summary>
    /// <param name="id">The job identifier.</param>
    /// <param name="options">The user identifier type request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The job details.</returns>
    public Task<LarkResponseBody> GetHireJobAsync(string id, LarkUserIdTypeRequestOptions options, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.HireJob, options, id), "job_detail", cancellationToken);

    /// <summary>
    /// Gets the offer associated with a job application.
    /// </summary>
    /// <param name="id">The job application identifier.</param>
    /// <param name="options">The optional recruitment resource identifier type options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The offer information.</returns>
    public Task<LarkResponseBody> GetHireOfferByApplicationAsync(string id, LarkHireBasicResourceOptions? options = null, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.HireOfferByApplication, options, id), "offer", cancellationToken);

    /// <summary>
    /// Gets the details of a recruitment offer.
    /// </summary>
    /// <param name="id">The offer identifier.</param>
    /// <param name="options">The optional recruitment resource identifier type options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The offer details.</returns>
    public Task<LarkResponseBody> GetHireOfferAsync(string id, LarkHireBasicResourceOptions? options = null, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.HireOfferDetails, options, id), "offer", cancellationToken);
}
