using System;
using System.Collections.Generic;
using System.Text;
using Trivial.Net;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.OapiModels;

/// <summary>
/// The resource identifier info from page URL.
/// </summary>
public class LarkUrlInfo
{
    /// <summary>
    /// Initializes a new instance of the LarkUrlInfo class.
    /// </summary>
    public LarkUrlInfo()
    {
        Query = new();
    }

    /// <summary>
    /// Initializes a new instance of the LarkUrlInfo class.
    /// </summary>
    /// <param name="url">The page URL.</param>
    public LarkUrlInfo(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            Query = [];
            return;
        }

        url = url.Trim().Replace("//", "/");
        var i = url.IndexOf(".feishu.cn/");
        if (i >= 0)
        {
            url = url[(i + 11)..];
        }
        else if (!url.Contains('/') && !url.Contains('?'))
        {
            Id = url;
            return;
        }
        
        i = url.IndexOf('/');
        Product = i > 0 ? url[..i] : null;
        if (i > 0) url = url[(i + 1)..];
        i = url.IndexOf('?');
        if (i == 0)
        {
            Query = QueryData.Parse(url);
            return;
        }

        if (i < 0)
        {
            i = url.IndexOf('/');
            if (i > 0) Id = url[..i];
            else if (i < 0) Id = url;
            Query = [];
            return;
        }

        url = url[(i + 1)..];
        var id = url[..i];
        i = id.IndexOf('/');
        if (i > 0) id = id[..i];
        else if (i == 0) id = null;
        Id = id;
        Query = QueryData.Parse(url);
    }

    /// <summary>
    /// Initializes a new instance of the LarkUrlInfo class.
    /// </summary>
    /// <param name="uri">The page URI.</param>
    public LarkUrlInfo(Uri uri)
        : this(uri?.OriginalString ?? string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkUrlInfo class.
    /// </summary>
    /// <param name="product">The product code.</param>
    /// <param name="id">The resource identifier.</param>
    /// <param name="query">The optional query.</param>
    public LarkUrlInfo(string? product, string? id, QueryData? query = null)
    {
        Product = product;
        Id = id;
        Query = query ?? new();
    }

    /// <summary>
    /// Gets the product code.
    /// </summary>
    public string? Product { get; }

    /// <summary>
    /// Gets the resource identifier.
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// Gets the optional query col.
    /// </summary>
    public QueryData Query { get; }
}
