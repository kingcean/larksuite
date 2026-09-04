using LarkSuite.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Trivial.Collection;
using Trivial.Net;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.OapiModels;

public enum LarkAcademyDegree : byte
{
    Unknown = 0,
    PrimarySchool = 1,
    JuniorMiddleSchool = 2,
    TechnicalSecondarySchool = 3,
    HighSchool = 4,
    JuniorCollege = 5,
    Bachelor = 6,
    Master = 7,
    Doctor = 8,
    Others = 9,
}

public enum LarkHireInterviewConclusionStatus : byte
{
    Unknown = 0,
    Pass = 1,
    Failure = 2,
    NotStart = 3,
    WaitingToSubmit = 4,
    WaitingComing = 5,
    WaitingResult = 6
}

public enum LarkHireInterviewType : byte
{
    Unknown = 0,
    OnSite = 1,
    Phone = 2,
    Video = 3,
    Others = 15,
}

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

public class LarkHireInterviewInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    [JsonPropertyName("begin_time")]
    public DateTime? BeginDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    [JsonPropertyName("end_time")]
    public DateTime? EndDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    [JsonPropertyName("biz_create_time")]
    public DateTime? CreationDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    [JsonPropertyName("biz_modify_time")]
    public DateTime? LastModificationDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("round")]
    public int Round { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("interview_record_list")]
    public List<LarkHireInterviewRecordInfo>? Records { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    [JsonPropertyName("feedback_submit_time")]
    public DateTime? SubmitDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("stage_id")]
    public string? StageId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("application_id")]
    public string? ApplicationId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("stage")]
    public LarkIdNameInfo? Stage { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("creator")]
    public LarkIdNameInfo? Creator { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("interview_round_summary")]
    public int State { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("interview_arrangement_id")]
    public string? ArrangementId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("interview_type")]
    public LarkHireInterviewType InterviewType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("talent_time_zone")]
    public JsonObjectNode? TalentTimeZone { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("contact_user")]
    public LarkIdNameInfo? ContactUser { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("contact_mobile")]
    public string? ContactMobile { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("address")]
    public JsonObjectNode? Address { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("video_type")]
    public int VideoApp { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("arrangement_status")]
    public int ArrangementStatus { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("arrangement_type")]
    public int ArrangementType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("arrangement_appointment_kind")]
    public int ArrangementAppointmentKind { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("meeting_room_list")]
    public List<LarkInterviewMeetingRoomInfo>? MeetingRooms { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("interview_round_type")]
    public LarkIdNameInfo? RoundType { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }

    public IReadOnlyList<LarkIdNameInfo> GetInterviewers()
    {
        var records = Records;
        var list = new List<LarkIdNameInfo>();
        if (records is null) return list;
        foreach (var record in records)
        {
            LarkApiUtils.Add(list, record?.Interviewer);
        }

        return list;
    }

    public string GetInterviewers(Func<LarkIdNameInfo, string?>? template, string? separator)
    {
        template ??= ele => ele.GetName();
        var col = GetInterviewers().Select(template).Where(ele => !string.IsNullOrEmpty(ele));
        return string.Join(separator ?? string.Empty, col);
    }

    public string GetInterviewers(Func<LarkIdNameInfo, string?>? template, string? separator, string? prefix, string? suffix = null)
    {
        var interviewers = GetInterviewers();
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(prefix)) sb.Append(prefix);
        sb.Append(GetInterviewers(template, separator));
        if (!string.IsNullOrEmpty(suffix)) sb.Append(suffix);
        return sb.ToString();
    }

    public bool GetBeginEndDateString(StringBuilder sb, string? prefix = null, string? suffix = null)
    {
        var start = BeginDate;
        if (!start.HasValue) return false;
        if (sb is null) return true;
        if (!string.IsNullOrEmpty(prefix)) sb.Append(prefix);
        sb.Append(start.Value.ToString("f"));
        var end = EndDate;
        if (!end.HasValue) return true;
        sb.Append(" → ");
        sb.Append(start.Value.Date == end.Value.Date
            ? end.Value.ToShortTimeString()
            : end.Value.ToString("f"));
        if (!string.IsNullOrEmpty(suffix)) sb.Append(suffix);
        return true;
    }

    public string? GetBeginEndDateString()
    {
        var sb = new StringBuilder();
        return GetBeginEndDateString(sb) ? sb.ToString() : null;
    }

    public string? GetBeginEndDateString(string prefix, string? suffix = null)
    {
        var sb = new StringBuilder();
        return GetBeginEndDateString(sb, prefix, suffix) ? sb.ToString() : null;
    }

    public string GetStateString()
    {
        return State switch
        {
            2 => "Pending",
            3 => "Evaluating",
            4 => "Pass",
            5 => "Fail",
            7 => "Evaluating and pass",
            8 => "Partial evaluated",
            9 => "Evaluating and fail",
            10 => "Evaluated",
            11 or 12 or 13 or 14 => "Partial evaluated",
            15 => "Evaluated",
            16 => "Partial fail",
            17 => "No decision",
            18 => "Evaluated",
            _ => "Unknown state"
        };
    }

    public override string ToString()
        => $"Id = {Id} & Appication Id = {ApplicationId} & State = {GetStateString()} & Date = {GetBeginEndDateString()}";
}

public class LarkHireApplicationInterviewInfo
{
    [JsonPropertyName("application_id")]
    public string Id { get; set; }

    [JsonPropertyName("interview_list")]
    public List<LarkHireInterviewInfo> List { get; set; }
}

public class LarkHireInterviewRecordInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("commit_status")]
    public int CommitStatus { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("conclusion")]
    public LarkHireInterviewConclusionStatus Conclusion { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("interview_score")]
    public JsonObjectNode? Score { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("interviewer")]
    public LarkIdNameInfo? Interviewer { get; set; }
}

public class LarkInterviewMeetingRoomInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("room_id")]
    public string Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("room_name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("building_name")]
    public string? Building { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("reserved_status")]
    public int ReservedStatus { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("floor_name")]
    public string? Floor { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}

public class LarkHireResumeSourceInfo : LarkIdNameInfo
{
    [JsonPropertyName("resume_source_type")]
    public int SourceType { get; set; }
}

public class LarkHireWebResumeSourceInfo
{
    [JsonPropertyName("website_id")]
    public string Id { get; set; }

    [JsonPropertyName("website_name")]
    public JsonObjectNode Name { get; set; }

    [JsonPropertyName("channel")]
    public JsonObjectNode Channel { get; set; }
}

public class LarkHireApplicationInfo
{
    [JsonPropertyName("basic_info")]
    public LarkHireApplicationBasicInfo Info { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("job")]
    public JsonObjectNode? Job { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("talent")]
    public LarkHireTalentBasicInfo? Talent { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("evaluations")]
    public List<LarkHireApplicationEvaluationInfo>? Evaluations { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("interview_aggregation")]
    public JsonObjectNode? InterviewAggregation { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("offer")]
    public JsonObjectNode? Offer { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employee")]
    public JsonObjectNode? Employee { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("agency")]
    public JsonObjectNode? Agency { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("portal")]
    public LarkHireApplicationPortalInfo? Portal { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("referral")]
    public JsonObjectNode? Referral { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }

    [JsonIgnore]
    public string Id => Info?.Id;

    [JsonIgnore]
    public string JobId => Info?.JobId ?? Job?.TryGetStringTrimmedValue("id");

    [JsonIgnore]
    public string TalentId => Info?.TalentId ?? Talent?.Id;
}

public class LarkHireApplicationStageTimingInfo
{
    [JsonPropertyName("stage_id")]
    public string Id { get; set; }

    [JsonPropertyName("enter_time")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    public DateTime EnterDate { get; set; }

    [JsonPropertyName("exit_time")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    public DateTime ExitDate { get; set; }
}

public class LarkHireApplicationTerminationInfo : LarkIdNameInfo
{
    [JsonPropertyName("children")]
    public List<LarkIdNameInfo> Children { get; set; }
}

public class LarkHireApplicationEvaluationInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("application_id")]
    public string ApplicationId { get; set; }

    [JsonPropertyName("stage_id")]
    public string StageId { get; set; }

    [JsonPropertyName("creator_id")]
    public string CreatorId { get; set; }

    [JsonPropertyName("evaluator_id")]
    public string EvaluatorId { get; set; }

    [JsonPropertyName("commit_status")]
    public int CommitStatus { get; set; }

    [JsonPropertyName("conclusion")]
    public int Conclusion { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("create_time")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    public DateTime CreationDate { get; set; }

    [JsonPropertyName("update_time")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    public DateTime LastModificationDate { get; set; }
}

public class LarkHireApplicationCampusVolunteerInfo
{
    [JsonPropertyName("volunteer_seq")]
    public int Index { get; set; }
}

public class LarkHireApplicationPortalInfo
{
    [JsonPropertyName("campus_volunteer_info")]
    public LarkHireApplicationCampusVolunteerInfo CampusVolunteer { get; set; }
}

public class LarkHireApplicationStageInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("zh_name")]
    public string ChineseName { get; set; }

    [JsonPropertyName("en_name")]
    public string EnglishName { get; set; }

    [JsonPropertyName("type")]
    public int StageType { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}

public class LarkHireApplicationBasicInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("job_id")]
    public string JobId { get; set; }

    [JsonPropertyName("talent_id")]
    public string TalentId { get; set; }

    [JsonPropertyName("stage")]
    public LarkHireApplicationStageInfo Stage { get; set; }

    [JsonPropertyName("active_status")]
    public int ActiveState { get; set; }

    [JsonPropertyName("delivery_type")]
    public int DeliveryType { get; set; }

    [JsonPropertyName("resume_source_info")]
    public LarkHireResumeSourceInfo ResumeSource { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("website_resume_source")]
    public LarkHireWebResumeSourceInfo? WebsiteResumeSource { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("talent_attachment_resume_id")]
    public string? TalentAttachmentResumeId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("stage_time_list")]
    public List<LarkHireApplicationStageTimingInfo>? StageTiming { get; set; }

    [JsonPropertyName("onboard_status")]
    public int OnboardStatus { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("application_preferred_city_list")]
    public List<LarkCodeAndNameInfo>? PreferredCity { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("termination_reason")]
    public LarkHireApplicationTerminationInfo? TerminationReason { get; set; }

    [JsonPropertyName("creator_id")]
    public string CreatorId { get; set; }

    [JsonPropertyName("owner_id")]
    public string OwnerId { get; set; }

    [JsonPropertyName("terminator_id")]
    public string TerminatorId { get; set; }

    [JsonPropertyName("create_time")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    public DateTime CreationDate { get; set; }

    [JsonPropertyName("modify_time")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    public DateTime LastModificationDate { get; set; }

    [JsonPropertyName("lock_status")]
    public int LockStatus { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("termination_reason_note")]
    public string? TerminationReasonNote { get; set; }
}

public class LarkHireTalentInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("talent_id")]
    public string Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public string IdInOldVersion { get; set; }

    [JsonPropertyName("is_onboarded")]
    public bool IsOnboard { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("basic_info")]
    public LarkHireTalentBasicInfo BasicInfo { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("education_list")]
    public List<LarkHireTalentEducationInfo> EducationInfo { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("career_list")]
    public List<LarkHireTalentWorkingInfo> WorkingInfo { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("internship_list")]
    public List<LarkHireTalentWorkingInfo> InternInfo { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("project_list")]
    public List<LarkHireTalentProjectInfo> Projects { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("works_list")]
    public List<LarkHireTalentWorkInfo> Works { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("award_list")]
    public List<LarkHireTalentAwardInfo> Award { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("sns_list")]
    public List<LarkHireTalentWorkInfo> Sns { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("resume_attachment_id_list")]
    public List<string> ResumeAttachments { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }

    /// <summary>
    /// Simplifies to a JSON object.
    /// </summary>
    /// <returns>The JSON object with the key information.</returns>
    public JsonObjectNode Simplify()
    {
        var json = new JsonObjectNode
        {
            { "id", Id },
        };
        if (BasicInfo is not null) json.SetValue("basic", BasicInfo.Simplify());
        if (WorkingInfo is not null && WorkingInfo.Count > 0)
        {
            var arr = new JsonArrayNode();
            foreach (var item in WorkingInfo)
            {
                var simple = item?.Simplify();
                if (simple is not null) arr.Add(simple);
            }

            if (arr.Count > 0) json.SetValue("working", arr);
        }

        if (EducationInfo is not null && EducationInfo.Count > 0)
        {
            var arr = new JsonArrayNode();
            foreach (var item in EducationInfo)
            {
                var simple = item?.Simplify();
                if (simple is not null) arr.Add(simple);
            }

            if (arr.Count > 0) json.SetValue("education", arr);
        }

        if (Projects is not null && Projects.Count > 0)
        {
            var arr = new JsonArrayNode();
            foreach (var item in Projects)
            {
                var simple = item?.Simplify();
                if (simple is not null) arr.Add(simple);
            }

            if (arr.Count > 0) json.SetValue("projects", arr);
        }

        return json;
    }
}

public class LarkHireTalentBasicInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

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

    [JsonConverter(typeof(JsonUnixTimestampConverter))]
    [JsonPropertyName("birthday")]
    public DateTime? Birthday { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }

    /// <summary>
    /// Simplifies to a JSON object.
    /// </summary>
    /// <returns>The JSON object with the key information.</returns>
    public JsonObjectNode Simplify()
    {
        var json = new JsonObjectNode
        {
            { "name", Name }
        };
        json.SetValueIfNotEmpty("phone", PhoneNumber);
        json.SetValueIfNotEmpty("phone_region", PhoneNumberRegionCode);
        json.SetValue("gender", Gender.ToString());
        json.SetValueIfNotEmpty("region", RegionCode);
        if (Birthday.HasValue) json.SetValue("birthday", Birthday.Value.ToShortDateString());
        return json;
    }
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
    public LarkAcademyDegree Degree { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }

    /// <summary>
    /// Simplifies to a JSON object.
    /// </summary>
    /// <returns>The JSON object with the key information.</returns>
    public JsonObjectNode Simplify()
    {
        var json = new JsonObjectNode
        {
            { "id", Id },
            { "school", Name },
        };
        json.SetValueIfNotEmpty("major", Major);
        json.SetValueIfNotEmpty("start",StartDate);
        json.SetValueIfNotEmpty("end", EndDate);
        json.SetValue("degree", Degree.ToString());
        return json;
    }
}

public class LarkHireTalentWorkingInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("company_name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("title")]
    public string JobTitle { get; set; }

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

    /// <summary>
    /// Simplifies to a JSON object.
    /// </summary>
    /// <returns>The JSON object with the key information.</returns>
    public JsonObjectNode Simplify()
    {
        var json = new JsonObjectNode
        {
            { "id", Id },
            { "company", Name },
        };
        json.SetValueIfNotEmpty("title", JobTitle);
        json.SetValueIfNotEmpty("start", StartDate);
        json.SetValueIfNotEmpty("end", EndDate);
        json.SetValueIfNotEmpty("description", Description);
        return json;
    }
}

public class LarkHireTalentProjectInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("project_name")]
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

    /// <summary>
    /// Simplifies to a JSON object.
    /// </summary>
    /// <returns>The JSON object with the key information.</returns>
    public JsonObjectNode Simplify()
    {
        var json = new JsonObjectNode
        {
            { "project", Name },
        };
        json.SetValueIfNotEmpty("role", Role);
        json.SetValueIfNotEmpty("description", Description);
        return json;
    }
}

public class LarkHireTalentWorkInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}

public class LarkHireTalentAwardInfo
{
    [JsonPropertyName("award_name")]
    public string Name { get; set; }

    [JsonPropertyName("award_time")]
    public string Date { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}

public class LarkHireTalentSnsInfo
{
    [JsonPropertyName("sns_type")]
    public string Provider { get; set; }

    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}

/// <summary>
/// The information with identifier and name.
/// </summary>
public class LarkIdNameInfo
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets name in each languages.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public JsonObjectNode Name { get; set; }

    /// <summary>
    /// Gets name.
    /// </summary>
    /// <returns>The name.</returns>
    public string GetName()
        => LarkApiUtils.GetName(Name);

    /// <inhertidoc />
    public override string ToString()
    {
        var name = LarkApiUtils.GetName(Name);
        return string.IsNullOrWhiteSpace(name) ? Id : $"{name} ({Id})";
    }
}

/// <summary>
/// The information with identifier and name.
/// </summary>
public class LarkCodeAndNameInfo
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("code")]
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets name in each languages.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public JsonObjectNode Name { get; set; }

    /// <summary>
    /// Gets name.
    /// </summary>
    /// <returns>The name.</returns>
    public string GetName()
        => LarkApiUtils.GetName(Name);

    /// <inhertidoc />
    public override string ToString()
    {
        var name = LarkApiUtils.GetName(Name);
        return string.IsNullOrWhiteSpace(name) ? Code : $"{name} ({Code})";
    }
}

public class LarkAttachmentInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mime")]
    public string ContentType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    [JsonPropertyName("create_time")]
    public DateTime CreationDate { get; set; }
}
