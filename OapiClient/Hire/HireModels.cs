using LarkSuite.OapiModels;
using LarkSuite.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Trivial.Collection;
using Trivial.Net;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite;

public class LarkInterviewMinuteInfo
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("speaker")]
    public string? SpeakerName { get; set; }

    [JsonPropertyName("speakerRole")]
    [JsonConverter(typeof(JsonStringEnumConverter<LarkInterviewRole>))]
    public LarkInterviewRole SpeakerRole { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    public static LarkInterviewMinuteInfo? Deserialize(JsonObjectNode? record)
    {
        var message = record?.TryGetStringValue("content");
        if (string.IsNullOrWhiteSpace(message)) return null;
        var time = LarkUrls.TryGetDateTime(record!, "speak_time");
        if (!time.HasValue) return null;
        var item = new LarkInterviewMinuteInfo
        {
            Message = message,
            Time = time.Value,
        };
        var speaker = record!.TryGetObjectValue("speaker_name");
        if (speaker is not null)
            item.SpeakerName = LarkApiUtils.GetName(speaker);
        var speakerType = record.TryGetInt32Value("user_type") ?? 0;
        item.SpeakerRole = speakerType switch
        {
            1 => LarkInterviewRole.Interviewer,
            2 => LarkInterviewRole.Interviewee,
            _ => LarkInterviewRole.Unknown,
        };
        return item;
    }
}

public class LarkHireTalentInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("talent_id")]
    public string Id { get; set; }

    [JsonPropertyName("is_onboarded")]
    public bool IsOnboard { get; set; }

    [JsonPropertyName("basic_info")]
    public LarkHireTalentBasicInfo BasicInfo { get; set; }

    [JsonPropertyName("education_list")]
    public List<LarkHireTalentEducationInfo> EducationInfo { get; set; }

    [JsonPropertyName("career_list")]
    public List<LarkHireTalentWorkingInfo> WorkingInfo { get; set; }

    [JsonPropertyName("project_list")]
    public List<LarkHireTalentProjectInfo> ProjectInfo { get; set; }

    [JsonPropertyName("resume_attachment_id_list")]
    public List<string> ResumeAttachments { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}

public class LarkHireTalentBasicInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mobile_number")]
    public string PhoneNumber { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mobile_code")]
    public string PhoneNumberRegionCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("experience_years")]
    public int? ExperienceYears { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("gender")]
    public LarkGender Gender { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("identification_number")]
    public string IdNumber { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("nationality_code")]
    public string RegionCode { get; set; }

    [JsonConverter(typeof(JsonDateTimeSecondNumberConverter))]
    [JsonPropertyName("birthday")]
    public DateTime? Birthday { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}

public class LarkHireTalentEducationInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("school_name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("major")]
    public string Major { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("start_time")]
    public string StartDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("end_time")]
    public string EndDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("degree")]
    public int Degree { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}

public class LarkHireTalentWorkingInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("company_name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("start_time")]
    public string StartDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("end_time")]
    public string EndDate { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}

public class LarkHireTalentProjectInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}
