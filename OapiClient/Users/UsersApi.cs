using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.Text;
using Trivial.Net;
using Trivial.Text;

namespace LarkSuite;

public partial class LarkApi
{
    public Task<LarkResponsePagingBody> GetUserInfoAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        => ids is null ? Task.FromResult(new LarkResponsePagingBody(true, "ids should not be null or empty.")) : GetItemsAsync(LarkUrls.ToUrl(LarkUrls.UserInfo, new LarkUserInfoRequest()
        {
            UserIds = ids.ToList(),
        }), cancellationToken);

    public Task<LarkResponsePagingBody> GetUserInfoAsync(LarkUserInfoRequest options, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.UserInfo, options), cancellationToken);

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
        //=> PostAsync(LarkUrls.ToUrl(LarkUrls.UserInfo, options), options?.ToJson() ?? [], cancellationToken);
}
