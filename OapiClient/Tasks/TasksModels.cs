using LarkSuite.Docs;
using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Text;

namespace LarkSuite.Tasks;

public abstract class BaseLarkOkrItemInfo
{
    public BaseLarkOkrItemInfo()
    {
    }

    public BaseLarkOkrItemInfo(string id, string text, double weight = double.NaN, List<string>? mentionedUserIds = null, List<LarkContentBlockLinkReference>? refLinks = null)
    {
        Id = id;
        Text = [text];
        if (double.IsNaN(weight)) Weight = weight;
        if (mentionedUserIds is not null) MentionedUserIds = mentionedUserIds;
        if (refLinks is not null) ReferenceLinks = refLinks;
    }

    public BaseLarkOkrItemInfo(string id, List<string> text, double weight = double.NaN, List<string>? mentionedUserIds = null, List<LarkContentBlockLinkReference>? refLinks = null)
    {
        Id = id;
        Text = text;
        if (double.IsNaN(weight)) Weight = weight;
        if (mentionedUserIds is not null) MentionedUserIds = mentionedUserIds;
        if (refLinks is not null) ReferenceLinks = refLinks;
    }

    public BaseLarkOkrItemInfo(JsonObjectNode json)
    {
        if (json is null) return;
        Id = json.TryGetStringTrimmedValue("id", true);
        Weight = json.TryGetDoubleValue("weight", false);
        var blocks = json.TryGetObjectValue("content")?.TryGetObjectListValue("blocks", true);
        if (blocks is null) return;
        Text = new();
        var users = new List<string>();
        var links = new List<LarkContentBlockLinkReference>();
        foreach (var block in blocks)
        {
            if (block?.TryGetStringTrimmedValue("block_element_type") != "paragraph") continue;
            var content = block.TryGetObjectValue("paragraph")?.TryGetObjectListValue("elements", true);
            if (content is null || content.Count < 1) continue;
            var item = new StringBuilder();
            foreach (var text in content)
            {
                var type = text.TryGetStringTrimmedValue("paragraph_element_type", true);
                if (type is null) continue;
                switch (type)
                {
                    case "textRun":
                        {
                            var s = text.TryGetObjectValue("text_run")?.TryGetStringValue("text");
                            if (!string.IsNullOrEmpty(s)) item.Append(s);
                            break;
                        }
                    case "mention":
                        {
                            var userId = text.TryGetObjectValue("mention")?.TryGetStringValue("user_id");
                            if (!string.IsNullOrWhiteSpace(userId)) users.Add(userId);
                            break;
                        }
                    case "docsLink":
                        {
                            var link = new LarkContentBlockLinkReference(text.TryGetObjectValue("docs_link"));
                            if (string.IsNullOrWhiteSpace(link.Url)) continue;
                            item.Append(' ');
                            item.Append(link.DisplayName);
                            item.Append(' ');
                            break;
                        }
                }
            }

            if (item.Length > 0) Text.Add(item.ToString());
        }

        if (users.Count > 0) MentionedUserIds = users;
        if (links.Count > 0) ReferenceLinks = links;
    }

    public BaseLarkOkrItemInfo(BaseLarkOkrRecordItem item)
    {
        if (item is null) return;
        Id = item.Id;
        var blocks = item.Content?.Blocks;
        if (blocks is null) return;
        Text = new();
        var users = new List<string>();
        var links = new List<LarkContentBlockLinkReference>();
        foreach (var block in blocks)
        {
            if (block?.BlockType != "paragraph") continue;
            var content = block.Paragraph?.TryGetObjectListValue("elements", true);
            if (content is null || content.Count < 1) continue;
            var sb = new StringBuilder();
            foreach (var text in content)
            {
                var type = text.TryGetStringTrimmedValue("paragraph_element_type", true);
                if (type is null) continue;
                switch (type)
                {
                    case "textRun":
                        {
                            var s = text.TryGetObjectValue("text_run")?.TryGetStringValue("text");
                            if (!string.IsNullOrEmpty(s)) sb.Append(s);
                            break;
                        }
                    case "mention":
                        {
                            var userId = text.TryGetObjectValue("mention")?.TryGetStringValue("user_id");
                            if (!string.IsNullOrWhiteSpace(userId)) users.Add(userId);
                            break;
                        }
                    case "docsLink":
                        {
                            var link = new LarkContentBlockLinkReference(text.TryGetObjectValue("docs_link"));
                            if (string.IsNullOrWhiteSpace(link.Url)) continue;
                            sb.Append(' ');
                            sb.Append(link.DisplayName);
                            sb.Append(' ');
                            break;
                        }
                }
            }

            if (sb.Length > 0) Text.Add(sb.ToString());
        }

        if (users.Count > 0) MentionedUserIds = users;
        if (links.Count > 0) ReferenceLinks = links;
        if (item is not BaseLarkOkrItem item2) return;
        Weight = item2.Weight;
    }

    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("text")]
    public List<string> Text { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mentionedUsers")]
    public List<string> MentionedUserIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("refLinks")]
    public List<LarkContentBlockLinkReference> ReferenceLinks { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("weight")]
    public double Weight { get; set; }
}

public class LarkOkrObjectiveInfo : BaseLarkOkrItemInfo
{
    public LarkOkrObjectiveInfo()
        : base()
    {
    }

    public LarkOkrObjectiveInfo(string id, string text, double weight = double.NaN, List<string>? mentionedUserIds = null, List<LarkContentBlockLinkReference>? refLinks = null)
        : base(id, text, weight, mentionedUserIds, refLinks)
    {
    }

    public LarkOkrObjectiveInfo(string id, List<string> text, double weight = double.NaN, List<string>? mentionedUserIds = null, List<LarkContentBlockLinkReference>? refLinks = null)
        : base(id, text, weight, mentionedUserIds, refLinks)
    {
    }

    public LarkOkrObjectiveInfo(JsonObjectNode json)
        : base(json)
    {
        if (json is null) return;
        var position = json.TryGetInt32Value("position");
        if (position.HasValue) Position = position.Value;
    }

    public LarkOkrObjectiveInfo(LarkOkrObjectiveItem item)
        : base(item)
    {
        Source = item;
        Position = item.Position;
    }

    [JsonPropertyName("keyResults")]
    public List<LarkOkrKeyResultInfo> KeyResults { get; set; }

    [JsonIgnore]
    internal int Position { get; set; }

    [JsonIgnore]
    internal LarkOkrObjectiveItem Source { get; }
}

public class LarkOkrKeyResultInfo : BaseLarkOkrItemInfo
{
    public LarkOkrKeyResultInfo()
        : base()
    {
    }

    public LarkOkrKeyResultInfo(string id, string text, double weight = double.NaN, List<string>? mentionedUserIds = null, List<LarkContentBlockLinkReference>? refLinks = null)
        : base(id, text, weight, mentionedUserIds, refLinks)
    {
    }

    public LarkOkrKeyResultInfo(string id, List<string> text, double weight = double.NaN, List<string>? mentionedUserIds = null, List<LarkContentBlockLinkReference>? refLinks = null)
        : base(id, text, weight, mentionedUserIds, refLinks)
    {
    }

    public LarkOkrKeyResultInfo(JsonObjectNode json)
        : base(json)
    {
    }

    public LarkOkrKeyResultInfo(LarkOkrKeyResultItem item)
        : base(item)
    {
        Source = item;
    }

    [JsonIgnore]
    internal LarkOkrKeyResultItem Source { get; }
}
