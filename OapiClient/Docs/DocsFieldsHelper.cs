using System;
using System.Collections.Generic;
using System.Text;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.OapiModels;

public static class LarkDocsFieldsHelper
{
    public static void SetUser(JsonObjectNode node, string key, string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        node.SetValue(key, new JsonArrayNode
        {
            new JsonObjectNode
            {
                { "id", id },
            }
        });
    }

    public static void SetUser(JsonObjectNode node, string key, IEnumerable<string> id)
    {
        if (id is null) return;
        var arr = new JsonArrayNode();
        arr.AddRange(id.Where(ele => !string.IsNullOrWhiteSpace(ele)).Select(ele => new JsonObjectNode
        {
            { "id", ele },
        }));
        node.SetValue(key, arr);
    }

    public static void SetDate(JsonObjectNode node, string key, DateTime value)
        => node.SetValue(key, WebFormat.ParseDate(value));

    public static void SetDate(JsonObjectNode node, string key, DateTime? value)
    {
        if (value.HasValue) SetDate(node, key, value.Value);
    }

    public static void SetLink(JsonObjectNode node, string key, string url, string? title = null)
    {
        if (string.IsNullOrWhiteSpace(url)) return;
        var obj = new JsonObjectNode
        {
            { "link", url }
        };
        if (!string.IsNullOrWhiteSpace(title)) obj.SetValue("text", title);
        node.SetValue(key, obj);
    }

    public static void SetLink(JsonObjectNode node, string key, Uri uri, string? title = null)
    {
        if (uri is null) return;
        var obj = new JsonObjectNode
        {
            { "link", uri?.OriginalString }
        };
        if (!string.IsNullOrWhiteSpace(title)) obj.SetValue("text", title);
        node.SetValue(key, obj);
    }

    public static void SetFile(JsonObjectNode node, string key, string fileToken)
    {
        if (string.IsNullOrWhiteSpace(fileToken)) return;
        node.SetValue(key, new JsonArrayNode
        {
            new JsonObjectNode
            {
                { "file_token", fileToken },
            }
        });
    }

    public static void SetFile(JsonObjectNode node, string key, IEnumerable<string> fileToken)
    {
        if (fileToken is null) return;
        var arr = fileToken.Where(ele => !string.IsNullOrWhiteSpace(ele)).Select(ele => new JsonObjectNode
        {
            { "file_token", fileToken },
        });
        node.SetValue(key, arr);
    }
}
