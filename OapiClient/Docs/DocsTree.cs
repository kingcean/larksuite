using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
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

public class LarkContentBlockTree
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter<LarkContentBlockType>))]
    public LarkContentBlockType BlockType { get; set; }

    [JsonPropertyName("content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<LarkContentBlockTreeContent> Content { get; set; }

    [JsonPropertyName("children")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<LarkContentBlockTree>? Children { get; set; }

    [JsonPropertyName("resouce_token")]
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
