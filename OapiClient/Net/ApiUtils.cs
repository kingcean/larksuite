using LarkSuite.Docs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Net;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.OapiModels;

/// <summary>
/// The utilities of Lark doc content.
/// </summary>
public static partial class LarkApiUtils
{
    /// <summary>
    /// Gets the locale name.
    /// </summary>
    /// <param name="json">The object with strings.</param>
    /// <returns>The locale name.</returns>
    public static string? GetName(JsonObjectNode? json)
    {
        if (json is null) return null;
        return json.TryGetStringTrimmedValue("zh_cn", true) ?? json.TryGetStringTrimmedValue("en_us", true);
    }

    /// <summary>
    /// Sets the default instance of Lark API.
    /// </summary>
    /// <param name="instance">The new instance.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    public static bool ReplaceDefaultInstance(LarkApi instance)
    {
        if (instance is null || instance.IsAppKeyEmpty) return false;
        LarkApi.DefaultInstance = instance;
        return true;
    }
}
