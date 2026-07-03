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
    internal static string? GetName(JsonObjectNode? json)
    {
        if (json is null) return null;
        return json.TryGetStringTrimmedValue("zh_cn", true) ?? json.TryGetStringTrimmedValue("en_us", true);
    }
}
