using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Security;
using Trivial.Text;

namespace LarkSuite.Security;

public class LarkTenantToken
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string Message { get; set; }

    [JsonPropertyName("tenant_access_token")]
    public string Value { get; set; }

    [JsonIgnore]
    public TimeSpan Expired { get; set; }

    [JsonPropertyName("expire")]
    public int ExpireInSeconds
    {
        get => (int)Math.Floor(Expired.TotalSeconds);
        set => Expired = TimeSpan.FromSeconds(value);
    }
}

public class LarkTenantTokenInfo : TokenInfo
{
    public LarkTenantTokenInfo(LarkTenantToken token)
    {
        if (token is null) return;
        TokenType = BearerTokenType;
        AccessToken = token.Value;
        ExpiredAfter = token.Expired;
        OriginalToken = token;
    }

    public LarkTenantToken OriginalToken { get; }
}

internal class CodeTokenRequest : TokenRequest<CodeTokenRequestBody>
{
    public CodeTokenRequest(CodeTokenRequestBody body, AppAccessingKey appKey, IEnumerable<string> scope = null)
        : base(body, appKey, scope)
    {
    }

    public JsonObjectNode ToJson()
        => base.ToJsonObject();
}
