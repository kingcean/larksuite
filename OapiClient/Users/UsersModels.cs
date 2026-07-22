using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Net;
using Trivial.Text;

namespace LarkSuite.OapiModels;

/// <summary>
/// Gender.
/// </summary>
public enum LarkGender : byte
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Male.
    /// </summary>
    Male = 1,

    /// <summary>
    /// Female.
    /// </summary>
    Female = 2,

    /// <summary>
    /// Other kind of gender.
    /// </summary>
    Others = 3,
}

/// <summary>
/// The request options of user identifer.
/// </summary>
public class LarkUserIdRequestOptions : LarkUserIdTypeRequestOptions, IJsonObjectHost
{
    public IList<string>? Emails { get; set; }

    public IList<string>? Phones { get; set; }

    public bool IncludeResigned { get; set; }

    /// <inheritdoc />
    public JsonObjectNode ToJson()
    {
        var json = new JsonObjectNode();
        json.SetValueIfNotEmpty("emails", Emails);
        json.SetValueIfNotEmpty("mobiles", Phones);
        if (IncludeResigned) json.SetValue("include_resigned", IncludeResigned);
        return json;
    }
}

public class LarkUserInfoRequest : LarkUserIdTypeRequestOptions
{
    public IList<string> UserIds { get; set; }

    /// <inheritdoc />
    protected override void OnQueryDataFill(QueryData q)
    {
        base.OnQueryDataFill(q);
        var users = UserIds;
        if (users is null) return;
        foreach (var user in users)
        {
            if (string.IsNullOrWhiteSpace(user)) continue;
            q.Add("user_ids", user);
        }
    }
}

public class LarkOwnerInfoRequest
{
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("owner_type")]
    public string OwnerType { get; set; }
}

public class LarkEmployeeResolveRequest : LarkUserIdTypeRequestOptions
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("fields")]
    public List<string>? Fields { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employment_id_list")]
    public List<string>? EmploymentIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("person_ids")]
    public List<string>? PersonIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("work_emails")]
    public List<string>? Emails { get; set; }
}

public class LarkEmployeeSearchRequest : LarkUserIdTypeRequestOptions
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("fields")]
    public List<string>? Fields { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employment_id_list")]
    public List<string>? EmploymentIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employee_number_list")]
    public List<string>? EmploymentNos { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("work_email")]
    public string? Email { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("phone_number")]
    public string? Phone { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("key_word")]
    public string? Keyword { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employment_status")]
    public string? EmploymentStatus { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employee_type_id")]
    public string? EmployeeTypeId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("department_id_list")]
    public List<string>? DepartmentIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("direct_manager_id_list")]
    public List<string>? DirectManagerIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("dotted_line_manager_id_list")]
    public List<string>? DottedLineManagerIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("regular_employee_start_date_start")]
    public string? RegularStartDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("regular_employee_start_date_end")]
    public string? RegularEndDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("effective_time_start")]
    public string? EffectiveStartDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("effective_time_end")]
    public string? EffectiveEndDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("work_location_id_list_include_sub")]
    public List<string>? LocationIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("preferred_english_full_name_list")]
    public List<string>? PreferredEnglishNames { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("preferred_local_full_name_list")]
    public List<string>? PreferredNames { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("national_id_number_list")]
    public List<string>? NationalIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("phone_number_list")]
    public List<string>? Phones { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("email_address_list")]
    public List<string>? Emails { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("department_id_list_include_sub")]
    public List<string>? DepartmentIdsDeeply { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("additional_national_id_number_list")]
    public List<string>? AdditionalNationalIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("citizenship_status_list")]
    public List<string>? CitizenshipStatus { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cost_center_id_list")]
    public List<string>? CostCenterIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("service_company_list")]
    public List<string>? ServiceCompanies { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("service_company_list_include_sub")]
    public List<string>? ServiceCompaniesDeeply { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("job_family_id_list")]
    public List<string>? JobFamilyIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("job_family_id_list_include_sub")]
    public List<string>? JobFamilyIdsDeeply { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("job_level_id_list")]
    public List<string>? JobLevelIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("job_grade_id_list")]
    public List<string>? JobGradeIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("job_id_list")]
    public List<string>? JobIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("position_id_list")]
    public List<string>? PositionIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("position_id_list_include_sub")]
    public List<string>? PositionIdsDeeply { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("working_hours_type_id_list")]
    public List<string>? WorkingHourTypes { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("nationality_id_list")]
    public List<string>? NationalityIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("pay_group_id_list")]
    public List<string>? PayGroupIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("assignment_pay_group_id_list")]
    public List<string>? AssignmentPayGroupIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("contract_type_list")]
    public List<string>? ContractTypes { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("archive_cpst_plan_id_list")]
    public List<string>? ArchiveCpstPlanIds { get; set; }
}
