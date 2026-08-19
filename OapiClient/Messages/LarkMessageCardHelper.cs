using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
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

    public static int AddBulletLines(IList<JsonObjectNode> blocks, IEnumerable<string> content)
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

    public static JsonObjectNode CreateOverflowButton(string? width = null)
        => new()
        {
            { "tag", "overflow" },
            { "width", width ?? "default" },
            { "options", new JsonArrayNode() },
        };

    public static void AddOverflowButton(IList<JsonObjectNode> blocks, string? width = null)
    {
        var json = CreateOverflowButton(width);
        blocks.Add(json);
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

        var option = new JsonObjectNode
        {
            { "text", new JsonObjectNode
            {
                { "tag", "plain_text" },
                { "content", title },
            } },
        };
        if (!string.IsNullOrWhiteSpace(url)) option.SetValue("multi_url", new JsonObjectNode
        {
            { "url", url }
        });
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
