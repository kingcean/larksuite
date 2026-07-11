using LarkSuite.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Net;
using Trivial.Security;
using Trivial.Text;

namespace LarkSuite.OapiModels;

public class LarkDocsBaseTableInfo
{
    [JsonPropertyName("app_token")]
    public string Token { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("revision")]
    public int Revision { get; set; }

    [JsonPropertyName("is_advanced")]
    public bool HasAdvancedPermission { get; set; }

    [JsonPropertyName("time_zone")]
    public string TimeZone { get; set; }

    [JsonPropertyName("formula_type")]
    public int FormulaType { get; set; }

    [JsonPropertyName("advance_version")]
    public string AdvanceVersion { get; set; }
}

public class LarkDocsBaseTableFilter : BaseQueryRequestInfo, IJsonObjectHost
{
    public LarkDocsBaseTableFilter()
    {
    }

    public LarkDocsBaseTableFilter(string baseId, string tableId)
    {
        BaseId = LarkUrls.GetId(baseId);
        TableId = tableId;
    }

    public LarkDocsBaseTableFilter(string baseId, string tableId, LarkDocsFilter filter, List<LarkDocsSortItem>? sort = null)
        : this(baseId, tableId)
    {
        Filter = filter;
        Sort = sort;
    }

    public LarkDocsBaseTableFilter(string baseId, string tableId, string viewId)
        : this(baseId, tableId)
    {
        ViewId = viewId;
    }

    public string BaseId { get; set; }

    public string TableId { get; set; }

    public string? ViewId { get; set; }

    public LarkDocsFilter? Filter { get; set; }

    public List<LarkDocsSortItem>? Sort { get; set; }

    /// <inheritdoc />
    public JsonObjectNode ToJson()
    {
        var json = new JsonObjectNode()
        {
            { "automatic_fields", true },
        };
        json.SetValueIfNotEmpty("view_id", ViewId);
        if (Filter is not null) json.SetValue("filter", Filter.ToJson());
        if (Sort is not null)
        {
            var arr = new JsonArrayNode();
            foreach (var item in Sort)
            {
                var sortJson = item.ToJson();
                if (sortJson is not null) arr.Add(sortJson);
            }

            if (arr.Count > 0) json.SetValue("sort", arr);
        }

        return json;
    }

    protected override void OnQueryDataFill(QueryData q)
    {
    }
}

public class LarkDocsBaseTableTableInfo
{
    [JsonPropertyName("table_id")]
    public string Id { get; set; }

    [JsonPropertyName("revision")]
    public int Revision { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
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

    [JsonConverter(typeof(JsonDateTimeTickNumberConverter))]
    [JsonPropertyName("created_time")]
    public DateTime CreateDate { get; set; }

    [JsonPropertyName("last_modified_by")]
    public LarkDocsAccessUserInfo LastModifier { get; set; }

    [JsonConverter(typeof(JsonDateTimeTickNumberConverter))]
    [JsonPropertyName("last_modified_time")]
    public DateTime LastModificationDate { get; set; }

    [JsonPropertyName("shared_url")]
    public string SharedUrl { get; set; }

    [JsonPropertyName("record_url")]
    public string RecordUrl { get; set; }

    public JsonObjectNode? Simplify(Dictionary<string, string> mapping)
    {
        var fields = Fields;
        if (fields is null) return null;
        var json = new JsonObjectNode();
        foreach (var kvp in mapping)
        {
            Simplify(kvp.Value, json, kvp.Key);
        }

        return json;
    }

    public void Simplify(string sourceKey, JsonObjectNode target, string targetKey)
    {
        var fields = Fields;
        var kind = fields.GetValueKind(sourceKey);
        switch (kind)
        {
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return;
            case JsonValueKind.String:
                target.SetValue(targetKey, fields.TryGetStringValue(sourceKey));
                return;
            case JsonValueKind.Number:
                target.SetValue(targetKey, fields.TryGetDoubleValue(sourceKey));
                return;
            case JsonValueKind.True:
                target.SetValue(targetKey, true);
                return;
            case JsonValueKind.False:
                target.SetValue(targetKey, false);
                return;
            case JsonValueKind.Array:
                var arr = fields.TryGetArrayValue(sourceKey);
                if (arr.Length > 1)
                {
                    if (arr[0].ValueKind != JsonValueKind.Object)
                    {
                        target.SetValue(targetKey, arr);
                        return;
                    }

                    var strings = new List<string>();
                    foreach (var item in arr)
                    {
                        if (item is null) continue;
                        if (item is not JsonObjectNode itemJson || itemJson.TryGetStringValue("type") != "text")
                        {
                            strings.Clear();
                            break;
                        }

                        var s = itemJson.TryGetStringValue("text");
                        if (string.IsNullOrWhiteSpace(s)) continue;
                        strings.Add(s);
                    }

                    if (strings.Count > 0)
                        target.SetValue(targetKey, string.Join(Environment.NewLine, strings));
                    else
                        target.SetValue(targetKey, arr);
                    return;
                }

                var first = arr.TryGetObjectValue(0);
                GetBaseTableFieldValueFromJsonValue(first, target, targetKey);
                return;
            case JsonValueKind.Object:
                GetBaseTableFieldValueFromJsonValue(fields.TryGetObjectValue(sourceKey), target, targetKey);
                return;
        }
    }

    private static void GetBaseTableFieldValueFromJsonValue(JsonObjectNode value, JsonObjectNode target, string key)
    {
        var type = value.TryGetStringTrimmedValue("type");
        if (string.IsNullOrEmpty(type))
        {
            target.SetValue(key, value);
            return;
        }

        switch (type)
        {
            case "text":
                {
                    var s = value.TryGetStringValue("text");
                    if (string.IsNullOrWhiteSpace(s)) break;
                    target.SetValue(key, s);
                    return;
                }
            case "url":
                {
                    var s = value.TryGetStringValue("link") ?? value.TryGetStringValue("text");
                    if (string.IsNullOrWhiteSpace(s)) break;
                    target.SetValue(key, s);
                    return;
                }
            case "mention":
                {
                    if (value.TryGetStringValue("mentionType") == "Wiki")
                    {
                        var s = value.TryGetStringValue("link") ?? value.TryGetStringValue("text");
                        if (string.IsNullOrWhiteSpace(s)) break;
                        target.SetValue(key, s);
                        return;
                    }

                    break;
                }
        }

        target.SetValue(key, value);
    }
}

public class LarkDocsBaseTableRecordsInfo
{
    [JsonPropertyName("records")]
    public List<LarkDocsBaseTableRecord> Records { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("forbidden_record_ids")]
    public List<string>? ForbiddenRecords { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("absent_record_ids")]
    public List<string>? AbsentRecords { get; set; }

    public LarkDocsBaseTableRecord? Get(string id)
    {
        var col = Records;
        if (string.IsNullOrWhiteSpace(id) || col is null) return null;
        foreach (var record in col)
        {
            if (record?.Id == id) return record;
        }

        return null;
    }
}

public class LarkDocsBaseTableRecordDeletionInfo
{
    [JsonPropertyName("deleted")]
    public bool HasDeleted { get; set; }

    [JsonPropertyName("record_id")]
    public string Id { get; set; }
}
