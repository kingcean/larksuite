using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Security;
using Trivial.Text;

namespace LarkSuite.Docs;

/// <summary>
/// The item info of the docs.
/// </summary>
public class LarkDocsItemInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// The block content, key properties and child blocks.
/// </summary>
public class LarkContentBlockTree
{
    /// <summary>
    /// Gets or sets the block identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [Description("The block identifier.")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the block type.
    /// </summary>
    [JsonPropertyName("type")]
    [Description("The block type.")]
    [JsonConverter(typeof(JsonStringEnumConverter<LarkContentBlockType>))]
    public LarkContentBlockType BlockType { get; set; }

    /// <summary>
    /// Gets or sets the block content.
    /// </summary>
    [JsonPropertyName("content")]
    [Description("The block content.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<LarkContentBlockTreeContent> Content { get; set; }

    /// <summary>
    /// Gets or sets the child blocks.
    /// </summary>
    [JsonPropertyName("children")]
    [Description("The child blocks.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<LarkContentBlockTree>? Children { get; set; }

    /// <summary>
    /// Gets or sets the resource token if the block is a kind of other node reference.
    /// </summary>
    [JsonPropertyName("resouce_token")]
    [Description("The resource token if the block is a kind of other node reference.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ResourceToken { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(BlockType.ToString());
        var children = Children?.Count ?? 0;
        if (children > 0)
        {
            sb.Append(", Children = ");
            sb.Append(children);
        }

        var list = Content;
        if (list is not null && list.Count > 0)
        {
            sb.Append(" | ");
            foreach (var item in list)
            {
                if (sb.Length > 500) break;
                var text = item?.Text?.Trim();
                if (string.IsNullOrEmpty(text)) text = item?.Information?.DisplayName?.Trim();
                if (string.IsNullOrEmpty(text)) continue;
                sb.Append(text);
                sb.Append(' ');
            }
        }

        return sb.ToString().TrimEnd();
    }
}

public class LarkContentBlockTreeContent
{
    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; set; }

    [JsonPropertyName("info")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BaseLarkContentBlockTreeContentReference? Information { get; set; }

    public override string? ToString()
    {
        var text = Text?.Trim();
        if (string.IsNullOrEmpty(text)) text = Information?.DisplayName?.Trim();
        return string.IsNullOrEmpty(text) ? base.ToString() : text;
    }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(LarkContentBlockUserReference), "user")]
[JsonDerivedType(typeof(LarkContentBlockLinkReference), "link")]
public abstract class BaseLarkContentBlockTreeContentReference
{
    [JsonIgnore]
    public virtual string ReferenceType { get; }

    [JsonIgnore]
    public virtual string DisplayName { get; }
}

public class LarkContentBlockUserReference : BaseLarkContentBlockTreeContentReference
{
    [JsonIgnore]
    public override string ReferenceType => "user";

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonIgnore]
    public override string DisplayName => $"@({Id})";
}

public class LarkContentBlockLinkReference : BaseLarkContentBlockTreeContentReference
{
    public LarkContentBlockLinkReference()
    {
    }

    public LarkContentBlockLinkReference(string url, string? title = null)
    {
        Url = url;
        Title = title;
    }

    public LarkContentBlockLinkReference(JsonObjectNode json)
    {
        if (json is null) return;
        Url = json.TryGetStringValue("url");
        Title = json.TryGetStringValue("title");
    }

    [JsonIgnore]
    public override string ReferenceType => "link";

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Title { get; set; }

    [JsonIgnore]
    public override string DisplayName => string.IsNullOrWhiteSpace(Title) ? Url : $"{Title} ({Url})";
}

public class LarkDocLinkItem(string nodeToken, string? name)
{
    /// <summary>
    /// Gets the doc title.
    /// </summary>
    [JsonPropertyName("name")]
    [Description("The doc title.")]
    public string? Name { get; } = name;

    /// <summary>
    /// Gets the node token. The node token is a kind of identifier to a doc node used to get its information, content and child nodes.
    /// </summary>
    [JsonPropertyName("nodeToken")]
    [Description("The node token is a kind of identifier to a doc node used to get its information, content and child nodes.")]
    public string NodeToken { get; } = nodeToken;
}

public class LarkDocContent(string nodeToken, string? name, string? docToken, string docType, object content)
    : LarkDocLinkItem(nodeToken, name)
{
    public LarkDocContent(LarkDocsNodeInfo info, object content)
        : this(info.NodeToken, info.Name, info.DocToken, info.DocToken, content)
    {
    }

    /// <summary>
    /// Gets the doc token. The doc token is a kind of identifier to a doc object used to get its content.
    /// </summary>
    [JsonPropertyName("docToken")]
    [Description("The doc token is a kind of identifier to a doc object used to get its content.")]
    public string DocToken { get; } = docToken;

    /// <summary>
    /// Gets the doc type. The doc type is the type of the node resource, e.g. docx (online document), file (online file), bitable (Base Table, a kind of digital rich sheets), etc.
    /// </summary>
    [JsonPropertyName("type")]
    [Description("The doc type is the type of the node resource, e.g. docx (online document), file (online file), bitable (Base Table, a kind of digital rich sheets), etc.")]
    public string DocType { get; } = docType;

    /// <summary>
    /// Gets the content.
    /// </summary>
    [JsonPropertyName("content")]
    [Description("The content.")]
    public object Content { get; } = content;

    public virtual Type? GetContentType()
        => Content?.GetType();

    public bool SetToProperty(JsonObjectNode json, string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;
        if (Content is null)
        {
            json.SetNullValue(key);
            return true;
        }

        if (Content is string s)
        {
            json.SetValue(key, s);
            return true;
        }

        if (Content is JsonObjectNode jsonObj)
        {
            json.SetValue(key, jsonObj);
            return true;
        }

        if (Content is JsonArrayNode jsonArr)
        {
            json.SetValue(key, jsonArr);
            return true;
        }

        if (Content is StringBuilder sb)
        {
            json.SetValue(key, sb);
            return true;
        }

        if (Content is SecureString ss)
        {
            json.SetValue(key, ss);
            return true;
        }

        try
        {
            var obj = JsonObjectNode.ConvertFrom(Content);
            if (obj is not null) json.SetValue(key, obj);
        }
        catch (JsonException)
        {
        }
        catch (NotSupportedException)
        {
        }

        if (!Content.GetType().IsValueType)
        {
            if (Content is Uri uri) json.SetValue(key, uri.OriginalString);
            else if (Content is Guid guid) json.SetValue(key, guid);
            else if (Content is IEnumerable<string> strArr) json.SetValue(key, strArr);
            else return false;
        }

        if (Content is int i1) json.SetValue(key, i1);
        else if (Content is long i2) json.SetValue(key, i2);
        else if (Content is float i5) json.SetValue(key, i5);
        else if (Content is double i6) json.SetValue(key, i6);
        else if (Content is decimal i7) json.SetValue(key, i7);
        else if (Content is bool b) json.SetValue(key, b);
        else if (Content is DateTime dt) json.SetValue(key, dt);
        else if (Content is DateTimeOffset dto) json.SetValue(key, dto);
        else return false;
        return true;
    }
}

public class LarkDocContent<T>(string nodeToken, string? name, string? docToken, string docType, T content)
    : LarkDocContent(nodeToken, name, docToken, docType, content)
{
    public LarkDocContent(LarkDocsNodeInfo info, T content)
        : this(info.NodeToken, info.Name, info.DocToken, info.DocType, content)
    {
    }

    /// <summary>
    /// Gets the content.
    /// </summary>
    [JsonPropertyName("content")]
    [Description("The content.")]
    public new T Content { get; } = content;

    public override Type GetContentType()
        => Content?.GetType() ?? typeof(T);
}

public class LarkDocContentError
{
    public LarkDocContentError()
    {
    }

    public LarkDocContentError(string message)
    {
        Message = message;
    }

    [JsonPropertyName("error")]
    public bool IsError { get; set; } = true;

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
