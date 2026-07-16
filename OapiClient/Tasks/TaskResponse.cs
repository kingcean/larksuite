using LarkSuite.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Text;

namespace LarkSuite.OapiModels;

public abstract class BaseLarkOkrRecordItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("create_time")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime CreationDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("update_time")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime LastModificationDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("owner")]
    public LarkOwnerInfoRequest Owner { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("content")]
    public LarkOkrItemContent Content { get; set; }
}

public class BaseLarkOkrItem : BaseLarkOkrRecordItem
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("score")]
    public double Score { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("weight")]
    public double Weight { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("deadline")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime? Deadline { get; set; }
}

public class LarkOkrCycleItem : BaseLarkOkrRecordItem
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("tenant_cycle_id")]
    public string TenantCycleId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("score")]
    public double Score { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("start_time")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime StartDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("end_time")]
    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime EndDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("cycle_status")]
    public int Status { get; set; }
}

public class LarkOkrObjectiveItem : BaseLarkOkrItem
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cycle_id")]
    public string CycleId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("notes")]
    public LarkOkrItemContent Notes { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("category_id")]
    public string CategoryId { get; set; }
}

public class LarkOkrKeyResultItem : BaseLarkOkrItem
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("objective_id")]
    public string ObjectiveId { get; set; }
}

public class LarkOkrProgressItem : BaseLarkOkrRecordItem
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("entity_type")]
    public int TargetType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("entity_id")]
    public string TargetId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("progress_rate")]
    public LarkOkrProgressValue Progress { get; set; }
}

public class LarkOkrProgressValue
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("progress_percent")]
    public double Value { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("progress_status")]
    public int Status { get; set; }
}

public class LarkOkrItemContent
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("blocks")]
    public List<LarkOkrItemBlockInfo> Blocks { get; set; }
}

public class LarkOkrItemBlockInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("block_element_type")]
    public string BlockType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("paragraph")]
    public JsonObjectNode Paragraph { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("gallery")]
    public LarkOkrGalleryInfo Gallery { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}

public class LarkOkrGalleryInfo
{
    [JsonPropertyName("images")]
    public List<LarkOkrGalleryItemInfo> Items { get; set; }
}

public class LarkOkrGalleryItemInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("file_token")]
    public string FileToken { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("src")]
    public string Url { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("width")]
    public double Width { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("height")]
    public double Height { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}
