using LarkSuite.Docs;
using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Text;
using Trivial.Net;
using Trivial.Text;

namespace LarkSuite;

public partial class LarkApi
{
    [Description("Get the users information.")]
    public async Task<LarkResponseBody<List<JsonObjectNode>>> GetUserInfoAsync([Description("The user identifier list to get information.")] IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        if (ids is null) return new(true, "ids should not be null or empty.");
        var col = ids.ToList();
        var paging = col.Count > 50;
        var first = await GetAsync<List<JsonObjectNode>>(LarkUrls.ToUrl(LarkUrls.UserInfo, new LarkUserInfoRequest()
        {
            UserIds = paging ? col.Take(50).ToList() : col,
        }), "items", cancellationToken);
        if (first is null) return new(true, "Get users info failed because of no response.");
        if (first.Data is null || first.IsError) return first;
        if (!paging) return first;
        for (var i = 50; i < col.Count; i += 50)
        {
            var next = await GetAsync<List<JsonObjectNode>>(LarkUrls.ToUrl(LarkUrls.UserInfo, new LarkUserInfoRequest()
            {
                UserIds = col.Skip(i).Take(50).ToList(),
            }), "items", cancellationToken);
            if (next?.Data is null || next.IsError) break;
            first.Data.AddRange(next.Data);
        }

        return first;
    }

    public Task<LarkResponsePagingBody> GetUserInfoAsync(LarkUserInfoRequest options, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.UserInfo, options), cancellationToken);

    public async Task<LarkResponseBody<List<JsonObjectNode>>> GetUserInfoAsync(IEnumerable<string> ids, LarkContentBlockTree docs, CancellationToken cancellationToken = default)
    {
        var users = await GetUserInfoAsync(ids, cancellationToken);
        if (users is null) return new(true, "Get users info failed.");
        if (users.Data is null || users.IsError) return users;
        docs.Resources ??= new()
        {
            Users = [],
            Whiteboards = [],
        };
        foreach (var user in users!.Data!)
        {
            if (user is null) continue;
            var userId = user.TryGetStringTrimmedValue("open_id", true) ?? user.TryGetStringTrimmedValue("user_id", true);
            var userName = user.TryGetStringValue("name");
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(userName)) continue;
            docs.Resources.Users![userId] = userName;
        }

        return users;
    }

    public Task<LarkResponsePagingBody> SearchUserAsync(LarkSearchOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.SearchUser, options, paging, "users", cancellationToken);

    public Task<LarkResponsePagingBody> SearchUserAsync(string q, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => SearchUserAsync(new LarkSearchOptions
        {
            Query = q,
        }, paging, cancellationToken);

    public Task<IReadOnlyList<JsonObjectNode>> SearchUserAsync(LarkResponsePagingBody response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.SearchUser, response, "users", pageSize, cancellationToken);

    public async Task<LarkResponseBody> GetUserIdAsync(LarkUserIdRequestOptions options, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient<string>();
        return await PostAsync(LarkUrls.ToUrl(LarkUrls.UserId, options), options?.ToJson() ?? [], cancellationToken); ;
    }

    public Task<LarkResponsePagingBody> GetEmployeesAsync(LarkEmployeeResolveRequest options, CancellationToken cancellationToken = default)
        => PostItemsAsync(LarkUrls.GetEmployees, options, null, cancellationToken);

    public Task<LarkResponsePagingBody> SearchEmployeesAsync(LarkEmployeeSearchRequest options, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => PostItemsAsync(LarkUrls.SearchEmployees, options, paging, cancellationToken);

    public Task<IReadOnlyList<JsonObjectNode>> SearchEmployeesAsync(LarkResponsePagingBody response, int? pageSize, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.SearchEmployees, response, pageSize, cancellationToken);
}
