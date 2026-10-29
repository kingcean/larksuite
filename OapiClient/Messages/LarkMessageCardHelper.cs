using LarkSuite.Docs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Text;
using static System.Net.Mime.MediaTypeNames;

namespace LarkSuite.OapiModels;

public enum LarkMessageCardButtonType : byte
{
    Default = 0, // default
    Primary = 1, // primary
    Danger = 2, // danger
    PrimaryFilled = 3, // primary_filled
    DangerFilled = 4, // danger_filled
    PrimaryText = 5, // primary_text
    DangerText = 6, // danger_text
}

public static class LarkMessageCardHelper
{
    public static JsonObjectNode CreateSeparator()
        => new()
        {
            { "tag", "hr" },
        };

    public static void AddSeparator(IList<JsonObjectNode> blocks)
    {
        var json = CreateSeparator();
        blocks.Add(json);
    }

    public static JsonObjectNode CreateText(string text, string? size = null, string? iconToken = null, string? iconColor = null, string? align = null)
    {
        var element = new JsonObjectNode
        {
            { "tag", "plain_text" },
            { "content", text },
        };
        if (!string.IsNullOrWhiteSpace(size)) element.SetValue("text_size", size);
        if (!string.IsNullOrWhiteSpace(align)) element.SetValue("text_align", align);
        var json = new JsonObjectNode()
        {
            { "tag", "div" },
            { "text", element },
        };
        AddStandardIcon(json, iconToken, iconColor);
        return json;
    }

    public static void AddText(IList<JsonObjectNode> blocks, string text, string? size = null, string? iconToken = null, string? iconColor = null, string? align = null)
    {
        var json = CreateText(text, size, iconToken, iconColor, align);
        blocks.Add(json);
    }

    public static void AddStandardIcon(JsonObjectNode json, string? token, string? color = null)
    {
        if (string.IsNullOrWhiteSpace(token)) return;
        var element = new JsonObjectNode
        {
            { "tag", "standard_icon" },
            { "token", token },
        };
        if (!string.IsNullOrWhiteSpace(color)) element.SetValue("color", color);
        json.SetValue("icon", element);
    }

    public static JsonObjectNode? CreateBulletLines(IEnumerable<string>? content)
    {
        if (content is null) return null;
        var lines = content
            .Where(ele => !string.IsNullOrWhiteSpace(ele))
            .Select(ele => ele.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\t", " ")).ToList() ?? [];
        if (lines.Count < 1) return null;
        return new()
        {
            { "tag", "markdown" },
            { "text_align", "left" },
            { "content", string.Concat("- ", string.Join(string.Concat(Environment.NewLine, "- "), lines)) },
        };
    }

    public static int AddBulletLines(IList<JsonObjectNode> blocks, IEnumerable<string>? content)
    {
        if (content is null) return 0;
        var lines = content
            .Where(ele => !string.IsNullOrWhiteSpace(ele))
            .Select(ele => ele.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\t", " ")).ToList() ?? [];
        if (lines.Count < 1) return 0;
        blocks.Add(new()
        {
            { "tag", "markdown" },
            { "text_align", "left" },
            { "content", string.Concat("- ", string.Join(string.Concat(Environment.NewLine, "- "), lines)) },
        });
        return lines.Count;
    }

    public static JsonObjectNode CreatePersonList(IEnumerable<string> users, string? size, bool showAvatar, bool showName)
    {
        var persons = new List<JsonObjectNode>();
        if (users is not null)
        {
            foreach (var userId in users)
            {
                if (string.IsNullOrWhiteSpace(userId)) continue;
                persons.Add(new()
                {
                    { "id", userId }
                });
            }
        }

        return new()
        {
            { "tag", "person_list" },
            { "persons", persons },
            { "size", size ?? "small" },
            { "show_avatar", showAvatar },
            { "show_name", showName },
        };
    }

    public static JsonObjectNode CreatePersonList(IEnumerable<string> users, string? size = null)
        => CreatePersonList(users, size, true, true);

    public static JsonObjectNode CreatePersonList(IEnumerable<LarkDocsAccessUserInfo> users, string? size, bool showAvatar, bool showName)
    {
        var persons = new List<JsonObjectNode>();
        if (users is not null)
        {
            foreach (var person in users)
            {
                var userId = person?.Id;
                if (string.IsNullOrWhiteSpace(userId)) continue;
                persons.Add(new()
                {
                    { "id", userId }
                });
            }
        }

        return new()
        {
            { "tag", "person_list" },
            { "persons", persons },
            { "size", size ?? "small" },
            { "show_avatar", showAvatar },
            { "show_name", showName },
        };
    }

    public static JsonObjectNode CreatePersonList(IEnumerable<LarkDocsAccessUserInfo> users, string? size = null)
        => CreatePersonList(users, size, true, true);

    public static void AddPersonList(IList<JsonObjectNode> blocks, IEnumerable<string> users, string? size, bool showAvatar, bool showName)
    {
        var json = CreatePersonList(users, size, showAvatar, showName);
        blocks.Add(json);
    }

    public static void AddPersonList(IList<JsonObjectNode> blocks, IEnumerable<string> users, string? size = null)
    {
        var json = CreatePersonList(users, size, true, true);
        blocks.Add(json);
    }

    public static void AddPersonList(IList<JsonObjectNode> blocks, IEnumerable<LarkDocsAccessUserInfo> users, string? size, bool showAvatar, bool showName)
    {
        var json = CreatePersonList(users, size, showAvatar, showName);
        blocks.Add(json);
    }

    public static void AddPersonList(IList<JsonObjectNode> blocks, IEnumerable<LarkDocsAccessUserInfo> users, string? size = null)
    {
        var json = CreatePersonList(users, size, true, true);
        blocks.Add(json);
    }

    public static JsonObjectNode CreateButton(string buttonType, string text, string url, string? iconToken = null, string? iconColor = null)
    {
        var json = new JsonObjectNode
        {
            { "tag", "button" },
            { "text", new JsonObjectNode
            {
                { "tag", "plain_text" },
                { "content", text },
            }
            },
            { "type", buttonType },
            { "width", "default" },
            { "size", "medium" },
        };
        if (!string.IsNullOrWhiteSpace(iconToken)) AddStandardIcon(json, iconToken, iconColor);
        if (!string.IsNullOrWhiteSpace(url)) json.SetValue("behaviors", new JsonArrayNode
        {
            new JsonObjectNode
            {
                { "type", "open_url" },
                { "default_url", url },
            },
        });
        return json;
    }

    public static JsonObjectNode CreateButton(LarkMessageCardButtonType buttonType, string text, string url, string? iconToken = null)
        => CreateButton(ToString(buttonType), text, url, iconToken);

    public static void AddButton(IList<JsonObjectNode> blocks, string buttonType, string text, string url, string? iconToken = null)
    {
        var json = CreateButton(buttonType, text, url, iconToken);
        blocks.Add(json);
    }

    public static void AddButton(IList<JsonObjectNode> blocks, LarkMessageCardButtonType buttonType, string text, string url, string? iconToken = null)
    {
        var json = CreateButton(buttonType, text, url, iconToken);
        blocks.Add(json);
    }

    public static JsonObjectNode CreateOverflowButton(string? width = null, IEnumerable<JsonObjectNode>? options = null)
        => new ()
        {
            { "tag", "overflow" },
            { "width", width ?? "default" },
            { "options", options?.Where(ele => ele is not null) ?? [] },
        };

    public static JsonObjectNode? CreateOverflowButton(ICollection<JsonObjectNode> options, bool checkOptionCount = false, string? width = null)
        => checkOptionCount && (options is null || options.Count < 1) ? null : CreateOverflowButton(width, options);

    public static JsonObjectNode? CreateOverflowButton(IEnumerable<LarkDocsItemInfo> options, bool checkOptionCount = false, string? width = null)
    {
        if (options is null) return checkOptionCount ? null : CreateOverflowButton(width);
        var col = new List<JsonObjectNode>();
        foreach (var option in options)
        {
            if (string.IsNullOrWhiteSpace(option?.Title)) continue;
            col.Add(CreateOverflowButtonOption(option.Title, option.Url));
        }

        if (checkOptionCount && col.Count < 1) return null;
        return CreateOverflowButton(width, col);
    }

    public static void AddOverflowButton(IList<JsonObjectNode> blocks, string? width = null, IEnumerable<JsonObjectNode>? options = null)
    {
        var json = CreateOverflowButton(width, options);
        blocks.Add(json);
    }

    public static void AddOverflowButton(IList<JsonObjectNode> blocks, ICollection<JsonObjectNode> options, bool checkOptionCount = false, string? width = null)
    {
        if (checkOptionCount && (options is null || options.Count < 1)) return;
        AddOverflowButton(blocks, width, options);
    }

    public static void AddOverflowButton(IList<JsonObjectNode> blocks, ICollection<LarkDocsItemInfo> options, bool checkOptionCount = false, string? width = null)
    {
        var json = CreateOverflowButton(options, checkOptionCount, width);
        blocks.Add(json);
    }

    public static JsonObjectNode CreateOverflowButtonOption(string title, string? url = null)
    {
        var json = new JsonObjectNode
        {
            { "text", new JsonObjectNode
            {
                { "tag", "plain_text" },
                { "content", title },
            } },
        };
        if (!string.IsNullOrWhiteSpace(url)) json.SetValue("multi_url", new JsonObjectNode
        {
            { "url", url }
        });
        return json;
    }

    public static void AddOverflowButtonOption(JsonObjectNode button, string title, string? url = null)
    {
        if (button is null) return;
        var options = button.TryGetArrayValue("options");
        if (options is null)
        {
            options = [];
            button.SetValue("options", options);
        }

        var option = CreateOverflowButtonOption(title, url);
        options.Add(option);
    }

    public static JsonObjectNode CreateColumn(string? width, string? verticalAlign, string? horizontalAlign, string? background = null, IEnumerable<JsonObjectNode>? elements = null)
    {
        var json = new JsonObjectNode
        {
            { "tag", "column" }
        };
        if (string.IsNullOrWhiteSpace(width))
        {
            json.SetValue("width", "weighted");
            json.SetValue("weight", 1);
        }
        else
        {
            json.SetValue("width", width);
        };

        json.SetValueIfNotEmpty("vertical_align", verticalAlign);
        json.SetValueIfNotEmpty("horizontal_align", horizontalAlign);
        json.SetValueIfNotEmpty("background_style", background);
        if (elements is not null) json.SetValue("elements", elements.Where(ele => ele is not null));
        return json;
    }

    public static JsonObjectNode CreateColumn(int width, bool isWeighted, string? verticalAlign, string? horizontalAlign, string? background = null, IEnumerable<JsonObjectNode>? elements = null)
    {
        var json = new JsonObjectNode
        {
            { "tag", "column" }
        };
        if (isWeighted)
        {
            json.SetValue("width", "weighted");
            json.SetValue("weight", width);
        }
        else
        {
            json.SetValue("width", $"{width}px");
        }

        json.SetValueIfNotEmpty("vertical_align", verticalAlign);
        json.SetValueIfNotEmpty("horizontal_align", horizontalAlign);
        json.SetValueIfNotEmpty("background_style", background);
        if (elements is not null) json.SetValue("elements", elements.Where(ele => ele is not null));
        return json;
    }

    public static JsonObjectNode CreateColumn(string? width, string? verticalAlign, string? horizontalAlign, string? background, string text, string? textSize = null)
        => CreateColumn(width, verticalAlign, horizontalAlign, background, [CreateText(text, textSize)]);

    public static JsonObjectNode CreateColumn(int width, bool isWeighted, string? verticalAlign, string? horizontalAlign, string? background, string text, string? textSize = null)
        => CreateColumn(width, isWeighted, verticalAlign, horizontalAlign, background, [CreateText(text, textSize)]);

    public static JsonObjectNode CreateColumn(string? width, IEnumerable<JsonObjectNode>? elements = null)
        => CreateColumn(width, null, null, null, elements);

    public static JsonObjectNode CreateColumn(int width, bool isWeighted, IEnumerable<JsonObjectNode>? elements = null)
        => CreateColumn(width, isWeighted, null, null, null, elements);

    public static JsonObjectNode CreateColumn(string? width, string text)
        => CreateColumn(width, null, null, null, [CreateText(text)]);

    public static JsonObjectNode CreateColumn(int width, bool isWeighted, string text)
        => CreateColumn(width, isWeighted, null, null, null, [CreateText(text)]);

    public static void AddColumn(IList<JsonObjectNode> blocks, string? width, string? verticalAlign, string? horizontalAlign, string? background = null, IEnumerable<JsonObjectNode>? elements = null)
    {
        if (blocks is null) return;
        blocks.Add(CreateColumn(width, verticalAlign, horizontalAlign, background, elements));
    }

    public static void AddColumn(IList<JsonObjectNode> blocks, int width, bool isWeighted, string? verticalAlign, string? horizontalAlign, string? background = null, IEnumerable<JsonObjectNode>? elements = null)
    {
        if (blocks is null) return;
        blocks.Add(CreateColumn(width, isWeighted, verticalAlign, horizontalAlign, background, elements));
    }

    public static void AddColumn(IList<JsonObjectNode> blocks, string? width, string? verticalAlign, string? horizontalAlign, string? background, string text, string? textSize = null)
    {
        if (blocks is null) return;
        blocks.Add(CreateColumn(width, verticalAlign, horizontalAlign, background, text, textSize));
    }

    public static void AddColumn(IList<JsonObjectNode> blocks, int width, bool isWeighted, string? verticalAlign, string? horizontalAlign, string? background, string text, string? textSize = null)
    {
        if (blocks is null) return;
        blocks.Add(CreateColumn(width, isWeighted, verticalAlign, horizontalAlign, background, text, textSize));
    }

    public static void AddColumn(IList<JsonObjectNode> blocks, string? width, IEnumerable<JsonObjectNode>? elements = null)
    {
        if (blocks is null) return;
        blocks.Add(CreateColumn(width, elements));
    }

    public static void AddColumn(IList<JsonObjectNode> blocks, int width, bool isWeighted, IEnumerable<JsonObjectNode>? elements = null)
    {
        if (blocks is null) return;
        blocks.Add(CreateColumn(width, isWeighted, elements));
    }

    public static void AddColumn(IList<JsonObjectNode> blocks, string? width, string text)
    {
        if (blocks is null) return;
        blocks.Add(CreateColumn(width, text));
    }

    public static void AddColumn(IList<JsonObjectNode> blocks, int width, bool isWeighted, string text)
    {
        if (blocks is null) return;
        blocks.Add(CreateColumn(width, isWeighted, text));
    }

    public static JsonObjectNode CreateColumnSet(string? horizontalAlign, IEnumerable<JsonObjectNode>? columns = null)
    {
        var json = new JsonObjectNode
        {
            { "tag", "column_set" }
        };
        json.SetValueIfNotEmpty("horizontal_align", horizontalAlign);
        if (columns is not null) json.SetValue("columns", columns);
        return json;
    }

    public static JsonObjectNode CreateColumnSet(IEnumerable<JsonObjectNode>? columns = null)
        => CreateColumnSet(null, columns);

    public static void AddColumnSet(IList<JsonObjectNode> blocks, string? horizontalAlign, IEnumerable<JsonObjectNode>? columns = null)
    {
        if (blocks is null) return;
        blocks.Add(CreateColumnSet(horizontalAlign, columns));
    }

    public static void AddColumnSet(IList<JsonObjectNode> blocks, IEnumerable<JsonObjectNode>? columns = null)
    {
        if (blocks is null) return;
        blocks.Add(CreateColumnSet(columns));
    }

    public static IEnumerable<JsonObjectNode> Create(IEnumerable<BaseSimpleLarkMessageBlock?> blocks)
    {
        if (blocks is null) yield break;
        foreach (var block in blocks)
        {
            if (block is null)
            {
                yield return CreateSeparator();
                continue;
            }

            var blockType = block.BlockType?.Trim();
            if (string.IsNullOrEmpty(block.BlockType)) continue;
            switch (blockType!.ToLowerInvariant())
            {
                case "hr":
                case "separator":
                    {
                        yield return CreateSeparator();
                        break;
                    }
                case "text":
                case "txt":
                case "plain":
                    {
                        if (block is SimpleLarkTextMessageBlock textBlock)
                        {
                            var text = CreateText(textBlock.Text, textBlock.FontSize, textBlock.Icon, textBlock.IconColor, textBlock.Align);
                            if (!string.IsNullOrWhiteSpace(textBlock.Color)) text.SetValue("text_color", textBlock.Color); 
                            yield return text;
                        }
                        else if (block is BaseSimpleLarkTextMessageBlock textBlock2)
                        {
                            yield return CreateText(textBlock2.Text, null, textBlock2.Icon);
                        }

                        break;
                    }
                case "markdown":
                case "md":
                    {
                        if (block is not BaseSimpleLarkTextMessageBlock textBlock) break;
                        var md = new JsonObjectNode()
                        {
                            { "tag", "markdown" },
                            { "content", textBlock.Text },
                        };
                        AddStandardIcon(md, textBlock.Icon, textBlock.IconColor);
                        yield return md;
                        break;
                    }
                case "bullet":
                case "list":
                    {
                        if (block is not SimpleLarkListMessageBlock bullet) break;
                        var lines = CreateBulletLines(bullet.Lines)!;
                        AddStandardIcon(lines, bullet.Icon, bullet.IconColor);
                        yield return lines;
                        break;
                    }
                case "button":
                    {
                        if (block is not SimpleLarkButtonMessageBlock button) break;
                        yield return CreateButton(button.BlockType, button.Text, button.Url, button.Icon, button.IconColor);
                        break;
                    }
                case "buttons":
                    {
                        if (block is not SimpleLarkButtonsMessageBlock buttonsBlock) break;
                        var buttons = new List<JsonObjectNode>();
                        foreach (var button in buttonsBlock.Items)
                        {
                            if (button is null) continue;
                            buttons.Add(CreateColumn("auto", [
                                CreateButton(button.BlockType, button.Text, button.Url, button.Icon, button.IconColor)
                            ]));
                        }

                        if (buttons.Count > 0)
                        {
                            buttons.Add(CreateColumn(1, true));
                            yield return CreateColumnSet(buttons);
                        }
                        else if (buttonsBlock.Empty is not null)
                        {
                            var empty = CreateText(buttonsBlock.Empty.Text, buttonsBlock.Empty.FontSize, buttonsBlock.Empty.Icon, buttonsBlock.Empty.IconColor, buttonsBlock.Empty.Align);
                            if (!string.IsNullOrWhiteSpace(buttonsBlock.Empty.Color)) empty.SetValue("text_color", buttonsBlock.Empty.Color);
                            yield return empty;
                        }

                        break;
                    }
            }
        }
    }

    public static int AddRange(IList<JsonObjectNode> blocks, IEnumerable<BaseSimpleLarkMessageBlock?> add)
    {
        var i = 0;
        if (add is null) return i;
        var list = Create(add);
        foreach (var block in list)
        {
            blocks.Add(block);
            i++;
        }

        return i;
    }

    public static T? Deserialize<T>(string answer)
    {
        if (answer is null) return default;
        answer = answer.Trim().Trim('`');
        if (answer.StartsWith("json")) answer = answer[4..].Trim();
        if (string.IsNullOrEmpty(answer)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(answer);
        }
        catch (JsonException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (InvalidCastException)
        {
        }

        return default;
    }

    public static List<BaseSimpleLarkMessageBlock?> ToSimpleBlock(IEnumerable<JsonObjectNode> blocks)
    {
        var list = new List<BaseSimpleLarkMessageBlock?>();
        if (blocks is null) return list;
        foreach (var block in blocks)
        {
            if (block is null)
            {
                list.Add(null);
                continue;
            }

            var blockType = (block.TryGetStringTrimmedValue("blockType", true) ?? block.TryGetStringTrimmedValue("block_type", true))?.ToLowerInvariant();
            if (string.IsNullOrEmpty(blockType)) continue;
            var icon = block.TryGetStringTrimmedValue("icon", true);
            var iconColor = block.TryGetStringTrimmedValue("iconColor", true) ?? block.TryGetStringTrimmedValue("icon_color", true);
            switch (blockType)
            {
                case "hr":
                case "separator":
                    {
                        list.Add(null);
                        break;
                    }
                case "text":
                case "plain":
                case "txt":
                    {
                        list.Add(new SimpleLarkTextMessageBlock(block.TryGetStringValue("text"))
                        {
                            FontSize = block.TryGetStringTrimmedValue("fontSize", true) ?? block.TryGetStringTrimmedValue("font_size", true),
                            Color = block.TryGetStringTrimmedValue("color", true),
                            Icon = icon,
                            IconColor = iconColor,
                            Align = block.TryGetStringTrimmedValue("align", true)
                        });
                        break;
                    }
                case "md":
                case "markdown":
                    {
                        list.Add(new SimpleLarkMarkdownMessageBlock(block.TryGetStringValue("text"))
                        {
                            Icon = icon,
                            IconColor = iconColor
                        });
                        break;
                    }
                case "bullet":
                case "list":
                    {
                        var lines = block.TryGetStringListValue("lines", true);
                        list.Add(new SimpleLarkListMessageBlock(lines)
                        {
                            Icon = icon,
                            IconColor = iconColor
                        });
                        break;
                    }
                case "button":
                    {
                        var text = block.TryGetStringTrimmedValue("text");
                        var url = block.TryGetStringTrimmedValue("url");
                        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(url)) continue;
                        var buttonTypeStr = block.TryGetStringValue("buttonType") ?? block.TryGetStringValue("button_type") ?? "default";
                        list.Add(new SimpleLarkButtonMessageBlock(text, url, ToButtonType(buttonTypeStr))
                        {
                            Icon = icon,
                            IconColor = iconColor,
                        });
                        break;
                    }
                case "buttons":
                    {
                        var items = block.TryGetObjectListValue("items", true) ?? block.TryGetObjectListValue("buttons", true);
                        var arr = new List<SimpleLarkButtonMessageBlock>();
                        if (items is not null)
                        {
                            foreach (var button in items)
                            {
                                var text = button.TryGetStringTrimmedValue("text");
                                var url = button.TryGetStringTrimmedValue("url");
                                if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(url)) continue;
                                var buttonTypeStr = button.TryGetStringValue("buttonType") ?? button.TryGetStringValue("button_type") ?? "default";
                                arr.Add(new SimpleLarkButtonMessageBlock(text, url, ToButtonType(buttonTypeStr))
                                {
                                    Icon = icon,
                                    IconColor = iconColor,
                                });
                            }
                        }

                        var empty = block.TryGetObjectValue("empty");
                        list.Add(new SimpleLarkButtonsMessageBlock
                        {
                            Items = arr,
                            Icon = icon,
                            IconColor = iconColor,
                            Empty = empty is null ? null : new(empty.TryGetStringValue("text"))
                            {
                                FontSize = empty.TryGetStringTrimmedValue("fontSize", true) ?? empty.TryGetStringTrimmedValue("font_size", true),
                                Color = empty.TryGetStringTrimmedValue("color", true),
                                Icon = empty.TryGetStringTrimmedValue("icon", true),
                                IconColor = empty.TryGetStringTrimmedValue("iconColor", true) ?? empty.TryGetStringTrimmedValue("icon_color", true),
                                Align = empty.TryGetStringTrimmedValue("align", true),
                            },
                        });
                        break;
                    }
            }
        }

        return list;
    }

    internal static string ToString(LarkMessageCardButtonType type)
        => type switch
        {
            LarkMessageCardButtonType.Default => "default",
            LarkMessageCardButtonType.Primary => "primary",
            LarkMessageCardButtonType.Danger => "danger",
            LarkMessageCardButtonType.PrimaryFilled => "primary_filled",
            LarkMessageCardButtonType.DangerFilled => "danger_filled",
            LarkMessageCardButtonType.PrimaryText => "primary_text",
            LarkMessageCardButtonType.DangerText => "danger_text",
            _ => "default"
        };

    internal static LarkMessageCardButtonType ToButtonType(string s)
        => s is null ? LarkMessageCardButtonType.Default : s.Trim().ToLowerInvariant().Replace("-", string.Empty).Replace("_", string.Empty) switch
        {
            "primary" or "1" => LarkMessageCardButtonType.Primary,
            "danger" or "2" => LarkMessageCardButtonType.Danger,
            "primaryfilled" or "3" => LarkMessageCardButtonType.PrimaryFilled,
            "dangerfilled" or "4" => LarkMessageCardButtonType.DangerFilled,
            "primarytext" or "5" => LarkMessageCardButtonType.PrimaryText,
            "dangertext" or "6" => LarkMessageCardButtonType.DangerText,
            _ => LarkMessageCardButtonType.Default
        };
}

public abstract class BaseSimpleLarkMessageBlock
{
    [JsonPropertyName("blockType")]
    public abstract string BlockType { get; }

    [JsonPropertyName("icon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Icon { get; set; }

    [JsonPropertyName("iconColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? IconColor { get; set; }
}

public abstract class BaseSimpleLarkTextMessageBlock : BaseSimpleLarkMessageBlock
{
    public BaseSimpleLarkTextMessageBlock()
    {
    }

    public BaseSimpleLarkTextMessageBlock(string text)
    {
        Text = text;
    }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}

public class SimpleLarkTextMessageBlock : BaseSimpleLarkTextMessageBlock
{
    public SimpleLarkTextMessageBlock()
        : base()
    {
    }

    public SimpleLarkTextMessageBlock(string text)
        : base(text)
    {
    }

    public override string BlockType => "text";

    [JsonPropertyName("color")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Color { get; set; }

    [JsonPropertyName("fontSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FontSize { get; set; }

    [JsonPropertyName("align")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Align { get; set; }
}

public class SimpleLarkMarkdownMessageBlock : BaseSimpleLarkTextMessageBlock
{
    public SimpleLarkMarkdownMessageBlock()
        : base()
    {
    }

    public SimpleLarkMarkdownMessageBlock(string text)
        : base(text)
    {
    }

    public override string BlockType => "markdown";
}

public class SimpleLarkButtonMessageBlock : BaseSimpleLarkTextMessageBlock
{
    public SimpleLarkButtonMessageBlock()
        : base()
    {
    }

    public SimpleLarkButtonMessageBlock(string text, string url, LarkMessageCardButtonType type = LarkMessageCardButtonType.Primary)
        : base(text)
    {
        Url = url;
        ButtonType = type;
    }

    public override string BlockType => "button";

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("buttonType")]
    [JsonConverter(typeof(LarkMessageCardButtonTypeJsonConverter))]
    public LarkMessageCardButtonType ButtonType { get; set; }
}

public class SimpleLarkButtonsMessageBlock : BaseSimpleLarkMessageBlock
{
    public override string BlockType => "buttons";

    [JsonPropertyName("items")]
    public List<SimpleLarkButtonMessageBlock> Items { get; set; }

    [JsonPropertyName("empty")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SimpleLarkTextMessageBlock? Empty { get; set; }
}

public class SimpleLarkListMessageBlock : BaseSimpleLarkMessageBlock
{
    public SimpleLarkListMessageBlock()
    {
        Lines = [];
    }

    public SimpleLarkListMessageBlock(List<string> lines)
    {
        Lines = lines ?? [];
    }

    public override string BlockType => "bullet";

    public List<string> Lines { get; set; }
}

public class LarkMessageCardButtonTypeJsonConverter : System.Text.Json.Serialization.JsonConverter<LarkMessageCardButtonType>
{
    public override LarkMessageCardButtonType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return LarkMessageCardHelper.ToButtonType(reader.GetString());
            case JsonTokenType.Null:
                return LarkMessageCardButtonType.Default;
            case JsonTokenType.Number:
                var i = reader.GetInt32();
                return (LarkMessageCardButtonType)i;
            default:
                throw new JsonException($"Unexpected token parsing LarkMessageCardButtonType. Expected String or Null, got {reader.TokenType}.");
        }
    }
    public override void Write(Utf8JsonWriter writer, LarkMessageCardButtonType value, JsonSerializerOptions options)
    {
        var str = LarkMessageCardHelper.ToString(value);
        writer.WriteStringValue(str);
    }
}
