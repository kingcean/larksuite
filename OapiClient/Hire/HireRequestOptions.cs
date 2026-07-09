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

/// <summary>
/// The request of to search talents.
/// </summary>
public class LarkTalentSearchOptions : BaseQueryRequestInfo
{
    /// <summary>
    /// Initializes a new instance of the LarkTalentSearchOptions class.
    /// </summary>
    public LarkTalentSearchOptions()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkTalentSearchOptions class.
    /// </summary>
    /// <param name="keyword">The name or keyword used to search.</param>
    public LarkTalentSearchOptions(string keyword)
    {
        Keyword = keyword;
    }

    /// <summary>
    /// Gets or sets the name or keyword to search.
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// Gets or sets the start date of update.
    /// </summary>
    public DateTime? StartUpdateDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of update.
    /// </summary>
    public DateTime? EndUpdateDate { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        q.SetIfNotEmpty("keyword", Keyword);
        if (StartUpdateDate.HasValue) q.Add("update_start_time", WebFormat.ParseDate(StartUpdateDate.Value));
        if (EndUpdateDate.HasValue) q.Add("update_end_time", WebFormat.ParseDate(EndUpdateDate.Value));
        q.Add("sort_by", 1);
        //q.Add("user_id_type", "people_admin_id");
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
