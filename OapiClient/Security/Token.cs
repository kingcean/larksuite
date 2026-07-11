using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Security;
using Trivial.Text;

namespace LarkSuite.Security;

/// <summary>
/// The tenant token of Lark API.
/// </summary>
public class LarkTenantToken
{
    /// <summary>
    /// Gets or sets the status code.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// Gets or sets the response message.
    /// </summary>
    [JsonPropertyName("msg")]
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    [JsonPropertyName("tenant_access_token")]
    public string Value { get; set; }

    /// <summary>
    /// Gets a value indicating whether the access token is empty.
    /// </summary>
    [JsonIgnore]
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// Gets or sets the expiration.
    /// </summary>
    [JsonIgnore]
    public TimeSpan Expired { get; set; }

    /// <summary>
    /// Gets or sets the expiration in second.
    /// </summary>
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
