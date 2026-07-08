using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
        BiTableToken = item.TryGetObjectValue("bitable")?.TryGetStringTrimmedValue("token");
        Callout = item.TryGetObjectValue("callout");
        ChatCard = item.TryGetObjectValue("chat_card");
        Diagram = item.TryGetObjectValue("diagram");
        File = item.TryGetObjectValue("file");
        Grid = item.TryGetObjectValue("grid");
        GridColumn = item.TryGetObjectValue("grid_column");
        WebPage = item.TryGetObjectValue("iframe");
        Image = item.TryGetObjectValue("image");
        Isv = item.TryGetObjectValue("isv");
        var content = BlockType switch
        {
            LarkContentBlockType.Page => item.TryGetObjectValue("page"),
            LarkContentBlockType.Text => item.TryGetObjectValue("text"),
            LarkContentBlockType.Heading1 => item.TryGetObjectValue("heading1"),
            LarkContentBlockType.Heading2 => item.TryGetObjectValue("heading2"),
            LarkContentBlockType.Heading3 => item.TryGetObjectValue("heading3"),
            LarkContentBlockType.Heading4 => item.TryGetObjectValue("heading4"),
            LarkContentBlockType.Heading5 => item.TryGetObjectValue("heading5"),
            LarkContentBlockType.Heading6 => item.TryGetObjectValue("heading6"),
            LarkContentBlockType.Heading7 => item.TryGetObjectValue("heading7"),
            LarkContentBlockType.Heading8 => item.TryGetObjectValue("heading8"),
            LarkContentBlockType.Heading9 => item.TryGetObjectValue("heading9"),
            LarkContentBlockType.Bullet => item.TryGetObjectValue("bullet"),
            LarkContentBlockType.Ordered => item.TryGetObjectValue("ordered"),
            LarkContentBlockType.Code => item.TryGetObjectValue("code"),
            LarkContentBlockType.Quote => item.TryGetObjectValue("quote"),
            LarkContentBlockType.Equation => item.TryGetObjectValue("equation"),
            LarkContentBlockType.ToDo => item.TryGetObjectValue("todo"),
            _ => null
        };
        if (content is null || content.GetValueKind("elements") != System.Text.Json.JsonValueKind.Array)
            return;
        Elements = content.TryGetArrayValue("elements").Deserialize<List<LarkContentTextElement>>();
        var style = content.TryGetObjectValue("style");
        Style = style?.Deserialize<LarkContentTextBlockStyle>() ?? new();
    }

    public LarkContentTextBlockStyle Style { get; set; }

    public List<LarkContentTextElement> Elements { get; set; }

    public string? BiTableToken { get; set; }

    [JsonPropertyName("callout")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObjectNode? Callout { get; set; }

    [JsonPropertyName("chat_card")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObjectNode? ChatCard { get; set; }

    [JsonPropertyName("diagram")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObjectNode? Diagram { get; set; }

    [JsonPropertyName("file")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObjectNode? File { get; set; }

    [JsonPropertyName("grid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObjectNode? Grid { get; set; }

    [JsonPropertyName("grid_column")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObjectNode? GridColumn { get; set; }

    [JsonPropertyName("iframe")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObjectNode? WebPage { get; set; }

    [JsonPropertyName("image")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObjectNode? Image { get; set; }

    [JsonPropertyName("isv")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObjectNode? Isv { get; set; }
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
    [JsonPropertyName("text_run")]
    public LarkContentTextRun Text { get; set; }

    [JsonPropertyName("mention_user")]
    public LarkContentUserInfo UserMentioned { get; set; }

    [JsonPropertyName("mention_doc")]
    public LarkContentDocInfo DocMentioned { get; set; }

    [JsonPropertyName("reminder")]
    public JsonObjectNode Reminder { get; set; }

    [JsonPropertyName("file")]
    public JsonObjectNode File { get; set; }

    [JsonPropertyName("inline_block")]
    public JsonObjectNode InlineBlock { get; set; }

    [JsonPropertyName("equation")]
    public JsonObjectNode Equation { get; set; }
}

public class LarkContentTextRun
{
    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("text_element_style")]
    public LarkContentTextElementStyle Style { get; set; }
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

    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter<LarkContentTextIndentationLevel>))]
    [JsonPropertyName("indentation_level")]
    public LarkContentTextIndentationLevel Indentation { get; set; }

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

    [JsonPropertyName("comment_ids")]
    public List<string> CommentIds { get; set; }
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

    [JsonPropertyName("text_element_style")]
    public LarkContentTextElementStyle Style { get; set; }
}

public class LarkContentDocInfo
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("obj_type")]
    public int DocReferenceType { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("title")]
    public string? Name { get; set; }

    [JsonPropertyName("text_element_style")]
    public LarkContentTextElementStyle Style { get; set; }
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

    [JsonPropertyName("en_name")]
    public string? EnglishName { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }
}

public class LarkDocsBaseTableRecord
{
    public LarkDocsBaseTableRecord()
    {
    }

    public LarkDocsBaseTableRecord(JsonObjectNode json)
    {
        if (json is null) return;
        Fields = json.TryGetObjectValue("fields");
        Id = json.TryGetStringTrimmedValue("record_id");
        Creator = new(json.TryGetObjectValue("created_by"));
        CreateDate = json.TryGetDateTimeValue("created_time") ?? DateTime.Now;
        LastModifier = new(json.TryGetObjectValue("last_modified_by"));
        LastModificationDate = json.TryGetDateTimeValue("last_modified_time") ?? DateTime.Now;
        SharedUrl = json.TryGetStringTrimmedValue("shared_url");
        RecordUrl = json.TryGetStringTrimmedValue("record_url");
    }

    [JsonPropertyName("fields")]
    public JsonObjectNode Fields { get; set; } = new();

    [JsonPropertyName("record_id")]
    public string Id { get; set; }

    [JsonPropertyName("created_by")]
    public LarkDocsAccessUserInfo Creator { get; set; }

    [JsonPropertyName("created_time")]
    public DateTime CreateDate { get; set; }

    [JsonPropertyName("last_modified_by")]
    public LarkDocsAccessUserInfo LastModifier { get; set; }

    [JsonPropertyName("last_modified_time")]
    public DateTime LastModificationDate { get; set; }

    [JsonPropertyName("shared_url")]
    public string SharedUrl { get; set; }

    [JsonPropertyName("record_url")]
    public string RecordUrl { get; set; }
}
