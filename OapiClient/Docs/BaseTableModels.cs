using LarkSuite.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Trivial.Maths;
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

    /// <inheritdoc />
    public override string ToString()
        => $"{Name ?? "?"} (Token = {Token} & Rev = {Revision})";
}

public class LarkDocsBaseTableFullInfo(LarkDocsBaseTableInfo info, List<LarkDocsBaseTableTableInfo>? tables)
{
    public LarkDocsBaseTableInfo Info { get; } = info;

    public List<LarkDocsBaseTableTableInfo> Tables { get; } = tables ?? [];
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

    public LarkDocsBaseTableFilter(string baseId, string tableId, LarkDocsFilter filter)
        : this(baseId, tableId)
    {
        Filter = filter;
    }

    public LarkDocsBaseTableFilter(string baseId, string tableId, LarkDocsFilterCondition condition)
        : this(baseId, tableId)
    {
        Filter = new(CriteriaBooleanOperator.Or, condition);
    }

    public LarkDocsBaseTableFilter(string baseId, string tableId, LarkDocsFilter filter, List<LarkDocsSortItem> sort)
        : this(baseId, tableId, filter)
    {
        Sort = sort;
    }

    public LarkDocsBaseTableFilter(string baseId, string tableId, LarkDocsFilter filter, LarkDocsSortItem sort)
        : this(baseId, tableId, filter)
    {
        Sort = [sort];
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

    public void SetOrder(string name, bool isDesc)
        => Sort = [new(name, isDesc)];

    public void AddOrder(string name, bool isDesc)
    {
        Sort ??= [];
        Sort.Add(new(name, isDesc));
    }

    public void SetFilter(CriteriaBooleanOperator conjunction, params IEnumerable<LarkDocsFilterCondition> conditions)
        => Filter = new(conjunction, conditions);

    public void SetFilter(LarkDocsFilterCondition condition)
        => Filter = new(CriteriaBooleanOperator.Or, [condition]);

    public void SetFilter(string name, string op, string? value)
        => Filter = new(CriteriaBooleanOperator.Or, [new(name, op, value)]);

    public void SetFilter(string name, string op, List<string> value)
        => Filter = new(CriteriaBooleanOperator.Or, [new(name, op, value)]);

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

    /// <inheritdoc />
    public override string ToString()
        => $"{Name ?? "?"} (Table ID = {Id} & Rev = {Revision})";
}

public class LarkDocsBaseTableViewInfo
{
    [JsonPropertyName("view_id")]
    public string Id { get; set; }

    [JsonPropertyName("view_name")]
    public string Name { get; set; }

    [JsonPropertyName("view_type")]
    public string ViewType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("view_public_level")]
    public string Visibility { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("view_private_owner_id")]
    public string OwnerId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("property")]
    public JsonObjectNode Properties { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{Name ?? "?"} (View ID = {Id} & Type = {ViewType})";
}

/// <summary>
/// The table record of Lark Base.
/// </summary>
public class LarkDocsBaseTableRecord
{
    /// <summary>
    /// Initializes a new instance of the LarkDocsBaseTableRecord class.
    /// </summary>
    public LarkDocsBaseTableRecord()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkDocsBaseTableRecord class.
    /// </summary>
    /// <param name="json">The raw JSON.</param>
    public LarkDocsBaseTableRecord(JsonObjectNode json)
    {
        if (json is null) return;
        Fields = json.TryGetObjectValue("fields");
        Id = json.TryGetStringTrimmedValue("record_id", true);
        Creator = new(json.TryGetObjectValue("created_by"));
        CreateDate = json.TryGetDateTimeValue("created_time") ?? DateTime.Now;
        LastModifier = new(json.TryGetObjectValue("last_modified_by"));
        LastModificationDate = json.TryGetDateTimeValue("last_modified_time") ?? DateTime.Now;
        SharedUrl = json.TryGetStringTrimmedValue("shared_url");
        RecordUrl = json.TryGetStringTrimmedValue("record_url");
    }

    /// <summary>
    /// Gets or sets all the fields of the record.
    /// The JSON property key is the field name; the value is the value and properties of the field.
    /// </summary>
    [Description("All the fields of the record. The property key is the field name; the value is the value and properties of the field.")]
    [JsonPropertyName("fields")]
    public JsonObjectNode Fields { get; set; } = new();

    /// <summary>
    /// Gets or sets the record identifier.
    /// </summary>
    [Description("The record identifier.")]
    [JsonPropertyName("record_id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the user created the record.
    /// </summary>
    [Description("The user created the record.")]
    [JsonPropertyName("created_by")]
    public LarkDocsAccessUserInfo Creator { get; set; }

    /// <summary>
    /// Gets or sets the creation date time.
    /// </summary>
    [Description("The creation date time.")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    [JsonPropertyName("created_time")]
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// Gets or sets the user modified the record.
    /// </summary>
    [Description("The user created the record.")]
    [JsonPropertyName("last_modified_by")]
    public LarkDocsAccessUserInfo LastModifier { get; set; }

    /// <summary>
    /// Gets or sets the last modification date time.
    /// </summary>
    [Description("The creation date time.")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    [JsonPropertyName("last_modified_time")]
    public DateTime LastModificationDate { get; set; }

    /// <summary>
    /// Gets or sets the URL to share.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The URL to share.")]
    [JsonPropertyName("shared_url")]
    public string? SharedUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL of the record.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The URL of the record.")]
    [JsonPropertyName("record_url")]
    public string? RecordUrl { get; set; }

    /// <summary>
    /// Simplifies the data of all the record fields.
    /// </summary>
    /// <returns>A JSON object with fields data.</returns>
    public JsonObjectNode? Simplify()
    {
        var fields = Fields;
        if (fields is null) return null;
        var json = new JsonObjectNode();
        foreach (var kvp in fields)
        {
            Simplify(kvp.Key, json, kvp.Key);
        }

        return json;
    }

    /// <summary>
    /// Simplifies the data of all the record fields.
    /// </summary>
    /// <param name="keys">The field keys.</param>
    /// <returns>A JSON object with fields data.</returns>
    public JsonObjectNode? Simplify(IEnumerable<string> keys)
    {
        var fields = Fields;
        if (fields is null) return null;
        var json = new JsonObjectNode();
        foreach (var key in keys ?? fields.Keys)
        {
            Simplify(key, json, key);
        }

        return json;
    }

    /// <summary>
    /// Simplifies the data of all the record fields.
    /// </summary>
    /// <param name="mapping">A mapping of field key.</param>
    /// <returns>A JSON object with fields data.</returns>
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

    /// <summary>
    /// Simplifies the data of all the record fields.
    /// </summary>
    /// <param name="sourceKey">The original field key.</param>
    /// <param name="target">The JSON object target to save the field.</param>
    /// <param name="targetKey">The field key to save into the target.</param>
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
                {
                    var i1 = fields.TryGetInt64Value(sourceKey);
                    var i2 = fields.TryGetDoubleValue(sourceKey, false);
                    if (i1.HasValue && i1.Value == i2) target.SetValue(targetKey, i1);
                    else target.SetValue(targetKey, i2);
                    return;
                }
            case JsonValueKind.True:
                target.SetValue(targetKey, true);
                return;
            case JsonValueKind.False:
                target.SetValue(targetKey, false);
                return;
            case JsonValueKind.Array:
                var arr = fields.TryGetArrayValue(sourceKey);
                if (arr.Length < 1) return;
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
                if (first is null)
                {
                    switch (arr.GetValueKind(0))
                    {
                        case JsonValueKind.Null:
                        case JsonValueKind.Undefined:
                            break;
                        case JsonValueKind.String:
                            target.SetValue(targetKey, arr.TryGetStringValue(0));
                            break;
                        case JsonValueKind.True:
                            target.SetValue(targetKey, true);
                            break;
                        case JsonValueKind.False:
                            target.SetValue(targetKey, false);
                            break;
                        default:
                            target.SetValue(targetKey, arr);
                            break;
                    }
                }
                else
                {
                    GetBaseTableFieldValueFromJsonValue(first, target, targetKey);
                }

                return;
            case JsonValueKind.Object:
                GetBaseTableFieldValueFromJsonValue(fields.TryGetObjectValue(sourceKey), target, targetKey);
                return;
        }
    }

    /// <inheritdoc />
    public override string ToString()
        => $"Record ID = {Id}; Fields Count = {Fields?.Count ?? 0}";

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
                    var mentionType = value.TryGetStringValue("mentionType");
                    if (mentionType == "Wiki" || mentionType == "Docx")
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

/// <summary>
/// Initializes a new instance of the LarkDocsBaseTableRecord class.
/// </summary>
/// <param name="source">The source record.</param>
/// <param name="data">The data.</param>
public class LarkDocsBaseTableRecord<T>(LarkDocsBaseTableRecord? source, T? data)
{
    /// <summary>
    /// Gets the record identifier.
    /// </summary>
    public string? Id => Source?.Id;

    /// <summary>
    /// Gets the source record.
    /// </summary>
    public LarkDocsBaseTableRecord Source { get; } = source;

    /// <summary>
    /// Gets the data.
    /// </summary>
    public T? Data { get; } = data;
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

/// <summary>
/// The table record deletion information of Lark Base.
/// </summary>
[Description("The table record deletion information of Lark Base.")]
public class LarkDocsBaseTableRecordDeletionInfo
{
    /// <summary>
    /// Gets or sets a value indicating whether the record has deleted.
    /// </summary>
    [Description("A value indicating whether the record has deleted.")]
    [JsonPropertyName("deleted")]
    public bool HasDeleted { get; set; }

    /// <summary>
    /// Gets or sets the record identifier.
    /// </summary>
    [Description("The record identifier.")]
    [JsonPropertyName("record_id")]
    public string Id { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => HasDeleted ? $"{Id ?? "?"} (Deleted)" : (Id ?? string.Empty);
}
