using LarkSuite.OapiModels;
using LarkSuite.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Collection;
using Trivial.Net;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite;

public class LarkInterviewOptions : BaseQueryRequestInfo
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public string? Id { get; set; }

    public string? ApplicationId { get; set; }

    public DateTime? Start { get; set; }

    public DateTime? End { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        q.SetIfNotEmpty("interview_id", Id);
        q.SetIfNotEmpty("application_id", ApplicationId);
        if (Start.HasValue) q["start_time"] = Trivial.Web.WebFormat.ParseDate(Start.Value).ToString();
        if (End.HasValue) q["end_time"] = Trivial.Web.WebFormat.ParseDate(End.Value).ToString();
    }
}

public class LarkInterviewByTelentOptions : BaseQueryRequestInfo
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the job level identifier type.
    /// </summary>
    public string? Level { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        q["talent_id"] = Id;
        q.SetIfNotEmpty("job_level_id_type", Level);
    }
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

public class LarkHireApplicationOptions : BaseQueryRequestInfo
{
    public string? ProcessId { get; set; }

    public string? StageId { get; set; }

    public string? TalentId { get; set; }

    public int? ActiveStatus { get; set; }

    public string? JobId { get; set; }

    //public List<int>? LockStatus { get; set; }

    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime? UpdateStartDate { get; set; }

    [JsonConverter(typeof(JsonDateTimeTickStringConverter))]
    public DateTime? UpdateEndDate { get; set; }

    protected override void OnQueryDataFill(QueryData q)
    {
        q.SetIfNotEmpty("process_id", ProcessId);
        q.SetIfNotEmpty("stage_id", StageId);
        q.SetIfNotEmpty("talent_id", TalentId);
        if (ActiveStatus.HasValue) q.Add("active_status", ActiveStatus.Value);
        q.SetIfNotEmpty("job_id", JobId);
        //q.Set("lock_status", LockStatus);
        q.Set("update_start_time", WebFormat.ParseDate(UpdateStartDate)?.ToString("G"));
        q.Set("update_end_time", WebFormat.ParseDate(UpdateEndDate)?.ToString("G"));
    }
}
