using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Collection;
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
    /// <summary>
    /// Gets or sets the list of work emails.
    /// </summary>
    public IList<string>? Emails { get; set; }

    /// <summary>
    /// Gets or sets the list of work phones.
    /// </summary>
    public IList<string>? Phones { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include resigned users.
    /// </summary>
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

/// <summary>
/// The request options of user info.
/// </summary>
public class LarkUserInfoRequest : LarkUserIdTypeRequestOptions
{
    /// <summary>
    /// Gets or sets the list of user identifiers.
    /// </summary>
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

/// <summary>
/// The request options of owner info.
/// </summary>
public class LarkOwnerInfoRequest
{
    /// <summary>
    /// Initializes a new instance of the LarkOwnerInfoRequest class.
    /// </summary>
    public LarkOwnerInfoRequest()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkOwnerInfoRequest class.
    /// </summary>
    /// <param name="type">The type of the owner.</param>
    /// <param name="id">The identifier of the owner.</param>
    public LarkOwnerInfoRequest(string type, string id)
    {
        OwnerType = type;
        UserId = id;
    }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the owner type.
    /// </summary>
    [JsonPropertyName("owner_type")]
    public string OwnerType { get; set; }
}

/// <summary>
/// The request options of employee resolve.
/// </summary>
public class LarkEmployeeResolveRequest : LarkUserIdTypeRequestOptions
{
    /// <summary>
    /// Initializes a new instance of the LarkEmployeeResolveRequest class.
    /// </summary>
    public LarkEmployeeResolveRequest()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkEmployeeResolveRequest class.
    /// </summary>
    /// <param name="id">The employment identifier or email.</param>
    /// <param name="fields">The list of fields.</param>
    public LarkEmployeeResolveRequest(string id, List<string>? fields = null)
    {
        Fields = fields;
        if (string.IsNullOrWhiteSpace(id)) return;
        if (id.Contains('@')) Emails = [id];
        else EmploymentIds = [id];
    }

    /// <summary>
    /// Initializes a new instance of the LarkEmployeeResolveRequest class.
    /// </summary>
    /// <param name="idType">The type of the identifier: employment, person or email.</param>
    /// <param name="ids">The list of identifiers.</param>
    /// <param name="fields">The list of fields.</param>
    public LarkEmployeeResolveRequest(string idType, List<string> ids, List<string>? fields = null)
    {
        idType ??= string.Empty;
        Fields = fields;
        if (ids is null) return;
        var col = ids.WhereNotNullOrWhiteSpace().Distinct().ToList();
        if (col.Count < 1) return;
        switch (idType.Trim().ToLowerInvariant().Replace(" ", string.Empty).Replace("_", string.Empty).Replace("-", string.Empty))
        {
            case "employment":
            case "employmentid":
            case "":
                EmploymentIds = col;
                break;
            case "person":
            case "personid":
                PersonIds = col;
                break;
            case "email":
            case "workemail":
            case "workemails":
                Emails = col;
                break;
            default:
                var first = col.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(first)) break;
                if (first.Contains('@')) Emails = col;
                else EmploymentIds = col;
                break;
        }
    }

    /// <summary>
    /// Gets or sets the list of fields to resolve.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("fields")]
    public List<string>? Fields { get; set; }

    /// <summary>
    /// Gets or sets the list of employment identifiers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employment_ids")]
    public List<string>? EmploymentIds { get; set; }

    /// <summary>
    /// Gets or sets the list of person identifiers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("person_ids")]
    public List<string>? PersonIds { get; set; }

    /// <summary>
    /// Gets or sets the list of work emails.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("work_emails")]
    public List<string>? Emails { get; set; }
}

/// <summary>
/// The request for searching Lark employees.
/// </summary>
public class LarkEmployeeSearchRequest : LarkUserIdTypeRequestOptions
{
    /// <summary>
    /// Gets or sets the list of fields to resolve.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("fields")]
    public List<string>? Fields { get; set; }

    /// <summary>
    /// Gets or sets the list of employment identifiers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employment_id_list")]
    public List<string>? EmploymentIds { get; set; }

    /// <summary>
    /// Gets or sets the list of employee numbers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employee_number_list")]
    public List<string>? EmploymentNos { get; set; }
    
    /// <summary>
    /// Gets or sets the work email.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("work_email")]
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("phone_number")]
    public string? Phone { get; set; }
    
    /// <summary>
    /// Gets or sets the keyword.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("key_word")]
    public string? Keyword { get; set; }
    
    /// <summary>
    /// Gets or sets the employment status.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employment_status")]
    public string? EmploymentStatus { get; set; }
    
    /// <summary>
    /// Gets or sets the employee type identifier.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employee_type_id")]
    public string? EmployeeTypeId { get; set; }
    
    /// <summary>
    /// Gets or sets the list of department identifiers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("department_id_list")]
    public List<string>? DepartmentIds { get; set; }
    
    /// <summary>
    /// Gets or sets the list of direct manager identifiers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("direct_manager_id_list")]
    public List<string>? DirectManagerIds { get; set; }
    
    /// <summary>
    /// Gets or sets the list of dotted line manager identifiers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("dotted_line_manager_id_list")]
    public List<string>? DottedLineManagerIds { get; set; }
    
    /// <summary>
    /// Gets or sets the regular employee start date.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("regular_employee_start_date_start")]
    public string? RegularStartDate { get; set; }
    
    /// <summary>
    /// Gets or sets the regular employee end date.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("regular_employee_start_date_end")]
    public string? RegularEndDate { get; set; }
    
    /// <summary>
    /// Gets or sets the effective start date.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("effective_time_start")]
    public string? EffectiveStartDate { get; set; }

    /// <summary>
    /// Gets or sets the effective end date.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("effective_time_end")]
    public string? EffectiveEndDate { get; set; }
    
    /// <summary>
    /// Gets or sets the list of work location identifiers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("work_location_id_list_include_sub")]
    public List<string>? LocationIds { get; set; }
    
    /// <summary>
    /// Gets or sets the list of preferred English full names.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("preferred_english_full_name_list")]
    public List<string>? PreferredEnglishNames { get; set; }
    
    /// <summary>
    /// Gets or sets the list of preferred local full names.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("preferred_local_full_name_list")]
    public List<string>? PreferredNames { get; set; }
    
    /// <summary>
    /// Gets or sets the list of national ID numbers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("national_id_number_list")]
    public List<string>? NationalIds { get; set; }
    
    /// <summary>
    /// Gets or sets the list of phone numbers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("phone_number_list")]
    public List<string>? Phones { get; set; }
    
    /// <summary>
    /// Gets or sets the list of email addresses.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("email_address_list")]
    public List<string>? Emails { get; set; }
    
    /// <summary>
    /// Gets or sets the list of department identifiers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("department_id_list_include_sub")]
    public List<string>? DepartmentIdsDeeply { get; set; }
    
    /// <summary>
    /// Gets or sets the list of additional national ID numbers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("additional_national_id_number_list")]
    public List<string>? AdditionalNationalIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("citizenship_status_list")]
    public List<string>? CitizenshipStatus { get; set; }

    /// <summary>
    /// Gets or sets the list of cost center identifiers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cost_center_id_list")]
    public List<string>? CostCenterIds { get; set; }
    
    /// <summary>
    /// Gets or sets the list of service company identifiers.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the list of job level identifiers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("job_level_id_list")]
    public List<string>? JobLevelIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("job_grade_id_list")]
    public List<string>? JobGradeIds { get; set; }

    /// <summary>
    /// Gets or sets the list of job identifiers.
    /// </summary>
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

/// <summary>
/// The request options of company department resolve.
/// </summary>
public abstract class BaseLarkCompanyDepartmentResolveRequest : LarkUserIdTypeRequestOptions
{
    /// <summary>
    /// <para>Gets or sets the list of fields to resolve.</para>
    /// <list type="bullet">
    /// <item>version_id</item>
    /// <item>sub_type</item>
    /// <item>manager</item>
    /// <item>is_root</item>
    /// <item>is_confidential</item>
    /// <item>effective_date</item>
    /// <item>expiration_date</item>
    /// <item>department_name</item>
    /// <item>parent_department_id</item>
    /// <item>tree_order</item>
    /// <item>list_order</item>
    /// <item>code</item>
    /// <item>active</item>
    /// <item>description</item>
    /// <item>custom_fields</item>
    /// <item>staffing_model</item>
    /// <item>cost_center_id</item>
    /// <item>created_time</item>
    /// <item>updated_time</item>
    /// <item>created_by</item>
    /// <item>updated_by</item>
    /// <item>record_created_time</item>
    /// <item>record_updated_time</item>
    /// <item>record_created_by</item>
    /// <item>record_updated_by</item>
    /// </list>
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("fields")]
    public List<string>? Fields { get; set; }

    /// <summary>
    /// Gets or sets the list of department identifiers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("department_id_list")]
    public List<string>? Ids { get; set; }
}

/// <summary>
/// The request options of company department resolve.
/// </summary>
public class LarkCompanyDepartmentResolveRequest : BaseLarkCompanyDepartmentResolveRequest
{
    /// <summary>
    /// Initializes a new instance of the LarkCompanyDepartmentResolveRequest class.
    /// </summary>
    public LarkCompanyDepartmentResolveRequest()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkCompanyDepartmentResolveRequest class.
    /// </summary>
    /// <param name="ids">The list of identifiers.</param>
    /// <param name="areNames">A value indicating whether the identifiers are names.</param>
    /// <param name="fields">The list of fields.</param>
    public LarkCompanyDepartmentResolveRequest(List<string> ids, bool areNames, List<string>? fields = null)
    {
        Fields = fields;
        if (ids is null) return;
        var col = ids.WhereNotNullOrWhiteSpace().Distinct().ToList();
        if (col.Count < 1) return;
        if (areNames) Names = col;
        else Ids = col;
    }

    /// <summary>
    /// Gets or sets the list of department names.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("department_name_list")]
    public List<string>? Names { get; set; }
}

/// <summary>
/// The request options of company department search.
/// </summary>
public class LarkCompanyDepartmentsSearchRequest : BaseLarkCompanyDepartmentResolveRequest
{
    /// <summary>
    /// Gets or sets the list of department names.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name_list")]
    public List<string>? Names { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to get active state of the departments: true for active departments only; false for inactive departments only; null for all.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("active")]
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to get all children departments when search by parent department identifier.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("get_all_children")]
    public bool GetAllChildren { get; set; }

    /// <summary>
    /// Gets or sets the list of managers.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("manager_list")]
    public List<string>? ManagerList { get; set; }

    /// <summary>
    /// Gets or sets the parent department identifier.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("parent_department_id")]
    public string? ParentDepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the list of department codes.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("code_list")]
    public List<string>? CodeList { get; set; }
}

/// <summary>
/// Represents the parent information of a company department.
/// </summary>
public class LarkCompanyDepartmentParentInfo
{
    /// <summary>
    /// Gets or sets the department identifier.
    /// </summary>
    [JsonPropertyName("department_id")]
    public string DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the list of department names in different locales.
    /// </summary>
    [JsonPropertyName("department_name")]
    public List<LarkLocaleNameItemInfo> Name { get; set; }

    /// <summary>
    /// Gets or sets the parent department identifier.
    /// </summary>
    [JsonPropertyName("parent_department_id")]
    public string ParentDepartmentId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the department is active.
    /// </summary>
    [JsonPropertyName("active")]
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the department is the root department.
    /// </summary>
    [JsonPropertyName("is_root")]
    public bool IsRoot { get; set; }

    /// <summary>
    /// Gets the name of the department in the preferred locale.
    /// </summary>
    /// <returns>The name of the department.</returns>
    public string GetName()
        => LarkApiUtils.GetName(Name);

    /// <summary>
    /// Returns a string representation of the department parent information.
    /// </summary>
    /// <returns>A string representation of the department parent information.</returns>
    public override string ToString()
    {
        var name = GetName() ?? "?";
        if (IsRoot) return IsActive ? $"Root department {name} ({DepartmentId})" : $"Inactive root department {name} ({DepartmentId})";
        return IsActive ? $"Department {name} ({DepartmentId}) - Parent {ParentDepartmentId}" : $"Inactive department {name} ({DepartmentId}) - Parent {ParentDepartmentId}";
    }
}

/// <summary>
/// Represents the response item containing department parent information.
/// </summary>
public class LarkCompanyDepartmentParentResponseItem
{
    /// <summary>
    /// Gets or sets the department identifier.
    /// </summary>
    [JsonPropertyName("department_id")]
    public string DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the list of parent department information.
    /// </summary>
    [JsonPropertyName("parent_department_list")]
    public List<LarkCompanyDepartmentParentInfo> Parents { get; set; }

    /// <summary>
    /// Returns a string representation of the department parent response item.
    /// </summary>
    /// <returns>A string representation of the department parent response item.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder(DepartmentId);
        foreach (var parent in Parents)
        {
            if (parent?.DepartmentId is null) continue;
            sb.Append(" ← ");
            sb.Append(parent.DepartmentId);
        }

        return sb.ToString();
    }
}

/// <summary>
/// Represents the response containing department parent information.
/// </summary>
public class LarkCompanyDepartmentParentResponseInfo
{
    /// <summary>
    /// Gets or sets the list of department parent information.
    /// </summary>
    [JsonPropertyName("items")]
    public List<LarkCompanyDepartmentParentResponseItem> Items { get; set; }

    /// <summary>
    /// Gets the department parent information by department identifier.
    /// </summary>
    /// <param name="id">The department identifier.</param>
    /// <returns>The department parent information if found; otherwise, null.</returns>
    public LarkCompanyDepartmentParentResponseItem? Get(string id)
        => Items?.FirstOrDefault(item => item.DepartmentId == id);

    /// <summary>
    /// Returns a string representation of the department parent response information.
    /// </summary>
    /// <returns>A string representation of the department parent response information.</returns>
    public override string ToString()
        => Items is null ? "null" : string.Concat("Count = ", Items.Count, "; Ids = ", string.Join(", ", Items.Select(ele => ele.DepartmentId)));
}
