using LarkSuite.Docs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Trivial.Text;

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

    public static JsonObjectNode CreateButton(string buttonType, string text, string url, string? iconToken = null)
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
        if (!string.IsNullOrWhiteSpace(iconToken)) AddStandardIcon(json, iconToken);
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
        => CreateButton(buttonType switch
        {
            LarkMessageCardButtonType.Default => "default",
            LarkMessageCardButtonType.Primary => "primary",
            LarkMessageCardButtonType.Danger => "danger",
            LarkMessageCardButtonType.PrimaryFilled => "primary_filled",
            LarkMessageCardButtonType.DangerFilled => "danger_filled",
            LarkMessageCardButtonType.PrimaryText => "primary_text",
            LarkMessageCardButtonType.DangerText => "danger_text",
            _ => "default"
        }, text, url, iconToken);

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
}
