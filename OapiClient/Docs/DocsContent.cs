using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Text;

namespace LarkSuite.OapiModels;

public class BaseLarkContentBlock
{
    [JsonPropertyName("block_id")]
    public string Id { get; set; }

    [JsonPropertyName("parent_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentId { get; set; }

    [JsonPropertyName("children")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? ChildIds { get; set; }

    [JsonPropertyName("block_type")]
    public LarkContentBlockType BlockType { get; set; }
}

public class LarkContentBlock : BaseLarkContentBlock
{
    public LarkContentBlock()
    {
    }

    public LarkContentBlock(JsonObjectNode item)
    {
        if (item is null)
        {
            BlockType = LarkContentBlockType.Unsupported;
            return;
        }

        Id = item.TryGetStringTrimmedValue("block_id") ?? string.Empty;
        ParentId = item.TryGetStringTrimmedValue("parent_id", true);
        ChildIds = item.TryGetStringListValue("children");
        BlockType = item.TryGetEnumValue<LarkContentBlockType>("block_type") ?? LarkContentBlockType.Unsupported;

        if (BlockType switch
        {
            LarkContentBlockType.Page => SetElements(item, "page"),
            LarkContentBlockType.Text => SetElements(item, "text"),
            LarkContentBlockType.Heading1 => SetElements(item, "heading1"),
            LarkContentBlockType.Heading2 => SetElements(item, "heading2"),
            LarkContentBlockType.Heading3 => SetElements(item, "heading3"),
            LarkContentBlockType.Heading4 => SetElements(item, "heading4"),
            LarkContentBlockType.Heading5 => SetElements(item, "heading5"),
            LarkContentBlockType.Heading6 => SetElements(item, "heading6"),
            LarkContentBlockType.Heading7 => SetElements(item, "heading7"),
            LarkContentBlockType.Heading8 => SetElements(item, "heading8"),
            LarkContentBlockType.Heading9 => SetElements(item, "heading9"),
            LarkContentBlockType.Bullet => SetElements(item, "bullet"),
            LarkContentBlockType.Ordered => SetElements(item, "ordered"),
            LarkContentBlockType.Code => SetElements(item, "code"),
            LarkContentBlockType.Quote => SetElements(item, "quote"),
            LarkContentBlockType.Equation => SetElements(item, "equation"),
            LarkContentBlockType.ToDo => SetElements(item, "todo"),
            LarkContentBlockType.BaseTable => SetResourceToken(item, "bitable"),
            LarkContentBlockType.Highlight => SetDataContent(item, "callout"),
            LarkContentBlockType.Conversation => SetDataContent(item, "chat_card"),
            LarkContentBlockType.Uml => SetDataContent(item, "diagram"),
            LarkContentBlockType.File => SetDataContent(item, "file"),
            LarkContentBlockType.Columns => SetOptionsContent(item, "grid"),
            LarkContentBlockType.Column => SetOptionsContent(item, "grid_column"),
            LarkContentBlockType.WebPage => SetDataDeeplyContent(item, "iframe", "component"),
            LarkContentBlockType.Image => SetDataContent(item, "image"),
            LarkContentBlockType.Widget => SetDataContent(item, "isv"),
            LarkContentBlockType.DocsWidget => SetDataContent(item, "add_ons"),
            LarkContentBlockType.Mind => SetResourceToken(item, "mindnote"),
            LarkContentBlockType.SheetTable => SetResourceToken(item, "sheet"),
            LarkContentBlockType.GridTable => SetDataContent(item, "table"),
            LarkContentBlockType.View => SetDataContent(item, "view"),
            LarkContentBlockType.Task => SetDataContent(item, "task"),
            LarkContentBlockType.OkrBlock => SetDataContent(item, "ork"),
            LarkContentBlockType.JiraIssue => SetDataContent(item, "jira_issue"),
            LarkContentBlockType.LinkPreview => SetDataContent(item, "link_preview"),
            LarkContentBlockType.ReferenceSyncBlock => SetDataContent(item, "reference_synced"),
            LarkContentBlockType.Unsupported or LarkContentBlockType.TableCell or LarkContentBlockType.Quote or LarkContentBlockType.OkrProgress => true,
            _ => false
        }) return;

        switch (BlockType)
        {
            case LarkContentBlockType.OkrObjective:
                {
                    var obj = item.TryGetObjectValue("okr_objective");
                    if (obj is null) break;
                    SetElements(obj, "content");
                    var id = obj.TryGetStringTrimmedValue("objective_id", true);
                    ResourceToken = id;
                    Options = new JsonObjectNode
                    {
                        { "objective_id", id },
                        { "confidential", obj.TryGetBooleanValue("confidential") },
                        { "position", obj.TryGetInt32Value("position") },
                        { "score", obj.TryGetInt32Value("score") },
                        { "visible", obj.TryGetBooleanValue("visible") },
                        { "weight", obj.TryGetDoubleValue("weight") },
                        { "progress_rate", obj.TryGetObjectValue("progress_rate") },
                    };
                    break;
                }
            case LarkContentBlockType.OkrKeyResult:
                {
                    var obj = item.TryGetObjectValue("okr_key_result");
                    if (obj is null) break;
                    SetElements(obj, "content");
                    var id = obj.TryGetStringTrimmedValue("kr_id", true);
                    ResourceToken = id;
                    Options = new JsonObjectNode
                    {
                        { "kr_id", id },
                        { "confidential", obj.TryGetBooleanValue("confidential") },
                        { "position", obj.TryGetInt32Value("position") },
                        { "score", obj.TryGetInt32Value("score") },
                        { "visible", obj.TryGetBooleanValue("visible") },
                        { "weight", obj.TryGetDoubleValue("weight") },
                        { "progress_rate", obj.TryGetObjectValue("progress_rate") },
                    };
                    break;
                }
            case LarkContentBlockType.Whiteboard:
                {
                    var obj = item.TryGetObjectValue("board");
                    if (obj is null) break;
                    ResourceToken = obj.TryGetStringTrimmedValue("token", true);
                    Options = new JsonObjectNode
                    {
                        { "align", obj.TryGetInt32Value("align") },
                        { "width", obj.TryGetInt32Value("width") },
                        { "height", obj.TryGetInt32Value("height") },
                    };
                    break;
                }
            case LarkContentBlockType.AgendaSubject:
                {
                    var obj = item.TryGetObjectValue("agenda_item_title");
                    if (obj is null) break;
                    SetElements(item, "agenda_item_title");
                    Options = new JsonObjectNode
                    {
                        { "align", obj.TryGetInt32Value("align") },
                    };
                    break;
                }
            case LarkContentBlockType.SyncBlock:
                {
                    var obj = item.TryGetObjectValue("source_synced");
                    if (obj is null) break;
                    SetElements(item, "source_synced");
                    Options = new JsonObjectNode
                    {
                        { "align", obj.TryGetInt32Value("align") },
                    };
                    break;
                }
            case LarkContentBlockType.WikiContents1:
                ResourceToken = item.TryGetObjectValue("sub_page_list")?.TryGetStringTrimmedValue("wiki_token", true);
                break;
            case LarkContentBlockType.WikiContents2:
                ResourceToken = item.TryGetObjectValue("wiki_catalog")?.TryGetStringTrimmedValue("wiki_token", true);
                break;
        }
    }

    public LarkContentTextBlockStyle Style { get; set; }

    public List<LarkContentTextElement> Elements { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObjectNode? Data { get; set; }

    [JsonPropertyName("token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ResourceToken { get; set; }

    [JsonPropertyName("options")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObjectNode? Options { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder($"Type = {BlockType.ToString()}");
        var eles = Elements;
        if (eles is not null && eles.Count > 0)
        {
            sb.Append($"; Count = {eles.Count}");
        }
        if (!string.IsNullOrWhiteSpace(ResourceToken))
        {
            sb.Append($"; Ref = {ResourceToken}");
        }

        return sb.ToString();
    }

    private bool SetElements(JsonObjectNode item, string? elements, string? options = null)
    {
        if (string.IsNullOrWhiteSpace(options)) Options = item.TryGetObjectValue(options);
        if (string.IsNullOrWhiteSpace(elements)) return false;
        var content = item.TryGetObjectValue(elements);
        if (content is null || content.GetValueKind("elements") != JsonValueKind.Array)
            return false;
        try
        {
            Elements = content.TryGetArrayValue("elements").Deserialize<List<LarkContentTextElement>>();
            var style = content.TryGetObjectValue("style");
            Style = style?.Deserialize<LarkContentTextBlockStyle>() ?? new();
            return true;
        }
        catch (JsonException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (InvalidCastException)
        {
        }

        return false;
    }

    private bool SetDataContent(JsonObjectNode item, string? data = null, string? options = null)
    {
        if (string.IsNullOrWhiteSpace(options)) Options = item.TryGetObjectValue(options);
        if (string.IsNullOrWhiteSpace(data)) return false;
        Data = item.TryGetObjectValue(data);
        if (Data is null) return false;
        ResourceToken = Data.TryGetStringTrimmedValue("token", true);
        return true;
    }

    private bool SetDataDeeplyContent(JsonObjectNode item, string? data, string? subKey, string? options = null)
    {
        if (string.IsNullOrWhiteSpace(options)) Options = item.TryGetObjectValue(options);
        if (string.IsNullOrWhiteSpace(data)) return false;
        Data = item.TryGetObjectValue(data);
        if (!string.IsNullOrWhiteSpace(subKey)) Data = Data?.TryGetObjectValue(subKey);
        return true;
    }

    private bool SetResourceToken(JsonObjectNode item, string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;
        ResourceToken = item.TryGetObjectValue(key)?.TryGetStringTrimmedValue("token", true);
        return ResourceToken is not null;
    }

    private bool SetOptionsContent(JsonObjectNode item, string options)
    {
        if (string.IsNullOrWhiteSpace(options)) return false;
        Options = item.TryGetObjectValue(options);
        return true;
    }
}

public class LarkContentTextInfo
{
    [JsonPropertyName("style")]
    public LarkContentTextBlockStyle Style { get; set; }

    [JsonPropertyName("elements")]
    public List<LarkContentTextElement> Elements { get; set; }
}

public class LarkContentTextElement
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("text_run")]
    public LarkContentTextRun? Text { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mention_user")]
    public LarkContentUserInfo? UserMentioned { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mention_doc")]
    public LarkContentDocInfo? DocMentioned { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("reminder")]
    public JsonObjectNode? Reminder { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("file")]
    public JsonObjectNode? File { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("inline_block")]
    public JsonObjectNode? InlineBlock { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("equation")]
    public JsonObjectNode? Equation { get; set; }
}

public class LarkContentTextRun
{
    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("text_element_style")]
    public LarkContentTextElementStyle Style { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => Content ?? string.Empty;
}

public class LarkContentTextBlockStyle
{
    [JsonPropertyName("align")]
    public LarkContentTextAlign Align { get; set; }

    [JsonPropertyName("done")]
    public bool HasDone { get; set; }

    [JsonPropertyName("folded")]
    public bool IsFolded { get; set; }

    [JsonPropertyName("language")]
    public int ProgramLanguage { get; set; }

    [JsonPropertyName("wrap")]
    public bool Wrap { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter<LarkContentTextIndentationLevel>))]
    [JsonPropertyName("indentation_level")]
    public LarkContentTextIndentationLevel Indentation { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("sequence")]
    public string? Sequence { get; set; }
}

public class LarkContentTextElementStyle
{
    [JsonPropertyName("bold")]
    public bool IsBold { get; set; }

    [JsonPropertyName("italic")]
    public bool IsItalic { get; set; }

    [JsonPropertyName("strikethrough")]
    public bool Strikethrough { get; set; }

    [JsonPropertyName("underline")]
    public bool Underline { get; set; }

    [JsonPropertyName("inline_code")]
    public bool IsInlineCode { get; set; }

    [JsonPropertyName("background_color")]
    public int BackgroundColor { get; set; }

    [JsonPropertyName("text_color")]
    public int TextColor { get; set; }

    [JsonPropertyName("link")]
    public LarkContentLinkInfo Link { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("comment_ids")]
    public List<string>? CommentIds { get; set; }
}

public class LarkContentLinkInfo
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public class LarkContentUserInfo
{
    [JsonPropertyName("user_id")]
    public string Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("text_element_style")]
    public LarkContentTextElementStyle? Style { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => Id ?? string.Empty;
}

public class LarkContentDocInfo
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("obj_type")]
    public int DocReferenceType { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("title")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("text_element_style")]
    public LarkContentTextElementStyle? Style { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => string.IsNullOrWhiteSpace(Name) ? (Url ?? Token ?? string.Empty) : $"[{Name}]({Url ?? Token})";
}

public class LarkDocsAccessUserInfo
{
    public LarkDocsAccessUserInfo()
    {
    }

    public LarkDocsAccessUserInfo(JsonObjectNode json)
    {
        if (json is null) return;
        Id = json.TryGetStringTrimmedValue("id");
        Name = json.TryGetStringTrimmedValue("name");
        EnglishName = json.TryGetStringTrimmedValue("en_name");
        Email = json.TryGetStringTrimmedValue("email");
        AvatarUrl = json.TryGetStringTrimmedValue("avatar_url");
    }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("en_name")]
    public string? EnglishName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var name = Name ?? EnglishName ?? Email;
        return string.IsNullOrWhiteSpace(name) ? (Id ?? string.Empty) : $"{name} ({Id})";
    }
}
