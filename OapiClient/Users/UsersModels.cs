using System;
using System.Collections.Generic;
using System.Text;
using Trivial.Net;
using Trivial.Text;

namespace LarkSuite.OapiModels;

/// <summary>
/// Gender.
/// </summary>
public enum LarkGender : byte
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Male.
    /// </summary>
    Male = 1,

    /// <summary>
    /// Female.
    /// </summary>
    Female = 2,

    /// <summary>
    /// Other kind of gender.
    /// </summary>
    Others = 3,
}

/// <summary>
/// The request options of user identifer.
/// </summary>
public class LarkUserIdRequestOptions : LarkUserIdTypeRequestOptions, IJsonObjectHost
{
    public IList<string>? Emails { get; set; }

    public IList<string>? Phones { get; set; }

    public bool IncludeResigned { get; set; }

    /// <inheritdoc />
    public JsonObjectNode ToJson()
    {
        var json = new JsonObjectNode();
        json.SetValueIfNotEmpty("emails", Emails);
        json.SetValueIfNotEmpty("mobiles", Phones);
        if (IncludeResigned) json.SetValue("include_resigned", IncludeResigned);
        return json;
    }
}

public class LarkUserInfoRequest : LarkUserIdTypeRequestOptions
{
    public IList<string> UserIds { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        base.OnQueryDataFill(q);
        var users = UserIds;
        if (users is null) return;
        foreach (var user in users)
        {
            if (string.IsNullOrWhiteSpace(user)) continue;
            q.Add("user_ids", user);
        }
    }
}
