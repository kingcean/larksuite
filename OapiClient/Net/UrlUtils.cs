using LarkSuite.Docs;
using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Net;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite;

internal static partial class LarkUrls
{
    public static string? GetId(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        if (url.IndexOf(".feishu.cn/") < 0) return url;
        var info = new LarkUrlInfo(url);
        return info.Id;
    }

    public static string ToUrl(string url, string? arg)
        => url.Replace("{0}", arg);

    public static string? ToUrl(string url, string? arg, bool testArg)
        => testArg && string.IsNullOrWhiteSpace(arg) ? null : url.Replace("{0}", arg);

    public static string ToUrl(string url, string? arg0, string? arg1, string? arg2 = null, string? arg3 = null)
        => url.Replace("{0}", arg0).Replace("{1}", arg1).Replace("{2}", arg2).Replace("{3}", arg3);

    public static string ToUrl(string url, BaseQueryRequestInfo? q)
        => q is null ? url : q.ToQueryUrlString(url);

    public static string ToUrl(string url, BaseQueryRequestInfo? q, string? arg)
        => q is null ? url.Replace("{0}", arg) : q.ToQueryUrlString(url.Replace("{0}", arg));

    public static string ToUrl(string url, BaseQueryRequestInfo? q, string? arg0, string? arg1, string? arg2 = null)
        => ToUrl(url.Replace("{0}", arg0).Replace("{1}", arg1).Replace("{2}", arg2), q);

    public static string ToUrl(string url, BaseQueryRequestInfo? q, string? arg0, string? arg1, string? arg2, string? arg3, string? arg4 = null, string? arg5 = null)
        => ToUrl(url.Replace("{0}", arg0).Replace("{1}", arg1).Replace("{2}", arg2).Replace("{3}", arg3).Replace("{4}", arg4).Replace("{5}", arg5), q);

    public static string ToUrl(string url, QueryData? q)
        => q is null ? url : q.ToString(url);

    public static string ToUrl(string url, QueryData? q, string? arg)
        => q is null ? url.Replace("{0}", arg) : q.ToString(url.Replace("{0}", arg));

    public static string ToUrl(string url, QueryData? q, string? arg0, string? arg1, string? arg2 = null)
        => ToUrl(url.Replace("{0}", arg0).Replace("{1}", arg1).Replace("{2}", arg2), q);

    public static string ToUrl(string url, QueryData? q, string? arg0, string? arg1, string? arg2, string? arg3, string? arg4 = null, string? arg5 = null)
        => ToUrl(url.Replace("{0}", arg0).Replace("{1}", arg1).Replace("{2}", arg2).Replace("{3}", arg3).Replace("{4}", arg4).Replace("{5}", arg5), q);

    public static string ToUrl(string url, BaseQueryRequestInfo? q, LarkPageTokenInfo? page)
    {
        var data = new QueryData();
        if (q is not null) q.ToQueryData(data);
        if (page is not null) page.ToQueryData(data);
        return data.ToString(url);
    }

    public static DateTime? TryGetDateTime(JsonObjectNode json, string key)
    {
        var s = json.TryGetStringTrimmedValue(key, true);
        if (s == null || !long.TryParse(s, out var i) || i < 0) return null;
        return WebFormat.ParseDate(i);
    }
}
