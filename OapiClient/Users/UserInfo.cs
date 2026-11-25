using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Data;

namespace LarkSuite.OapiModels;

/// <summary>
/// User information in the contact directory.
/// </summary>
[Description("User information in the contact directory.")]
public class LarkContactUserInfo : INamePropertyModel
{
    /// <summary>
    /// Gets or sets the unique user identifier within an app developer.
    /// </summary>
    [Description("The unique user identifier within an app developer.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("union_id")]
    public string? UnionId { get; set; }

    /// <summary>
    /// Gets or sets the unique user identifier within a tenant.
    /// </summary>
    [Description("The unique user identifier within a tenant.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the unique user identifier within an app.
    /// </summary>
    [Description("The unique user identifier within an app.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("open_id")]
    public string? OpenId { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    [Description("The user name.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the user name in English.
    /// </summary>
    [Description("The user name in English.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("en_name")]
    public string? EnglishName { get; set; }

    /// <summary>
    /// Gets or sets the user nickname.
    /// </summary>
    [Description("The user nickname.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>
    /// Gets or sets the user email address.
    /// </summary>
    [Description("The user email address.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the user mobile phone number.
    /// </summary>
    [Description("The user mobile phone number.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>
    /// Gets or sets whether the mobile phone number is visible to users in the organization.
    /// </summary>
    [Description("Whether the mobile phone number is visible to users in the organization.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("mobile_visible")]
    public bool? MobileVisible { get; set; }

    /// <summary>
    /// Gets or sets the user gender: 0 for unspecified, 1 for male, 2 for female, or 3 for other.
    /// </summary>
    [Description("The user gender: 0 for unspecified, 1 for male, 2 for female, or 3 for other.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("gender")]
    public LarkGender Gender { get; set; } = LarkGender.Unknown;

    /// <summary>
    /// Gets or sets the user avatar information.
    /// </summary>
    [Description("The user avatar information.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("avatar")]
    public LarkUserAvatarInfo? Avatar { get; set; }

    /// <summary>
    /// Gets or sets the user status information.
    /// </summary>
    [Description("The user status information.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("status")]
    public LarkContactUserStatusInfo? Status { get; set; }

    /// <summary>
    /// Gets or sets the list of department identifiers the user belongs to.
    /// </summary>
    [Description("The list of department identifiers the user belongs to.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("department_ids")]
    public List<string>? DepartmentIds { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user’s direct manager.
    /// </summary>
    [Description("The identifier of the user’s direct manager.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("leader_user_id")]
    public string? LeaderUserId { get; set; }

    /// <summary>
    /// Gets or sets the user’s city.
    /// </summary>
    [Description("The user’s city.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the user’s country or region code.
    /// </summary>
    [Description("The user’s country or region code.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the user’s workstation.
    /// </summary>
    [Description("The user’s workstation.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("work_station")]
    public string? WorkStation { get; set; }

    /// <summary>
    /// Gets or sets the user’s join time as a Unix timestamp in seconds.
    /// </summary>
    [Description("The user’s join time as a Unix timestamp in seconds.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("join_time")]
    public long? JoinTime { get; set; }

    /// <summary>
    /// Gets or sets whether the user is a tenant administrator.
    /// </summary>
    [Description("Whether the user is a tenant administrator.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("is_tenant_manager")]
    public bool? IsTenantManager { get; set; }

    /// <summary>
    /// Gets or sets the user’s employee number.
    /// </summary>
    [Description("The user’s employee number.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("employee_no")]
    public string? EmployeeNo { get; set; }

    /// <summary>
    /// Gets or sets the employee type identifier.
    /// </summary>
    [Description("The employee type identifier.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("employee_type")]
    public int? EmployeeType { get; set; }

    /// <summary>
    /// Gets or sets the user’s ordering information within their departments.
    /// </summary>
    [Description("The user’s ordering information within their departments.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("orders")]
    public List<LarkUserOrderInfo>? Orders { get; set; }

    /// <summary>
    /// Gets or sets the list of custom user attributes.
    /// </summary>
    [Description("The list of custom user attributes.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("custom_attrs")]
    public List<LarkUserCustomAttributeInfo>? CustomAttributes { get; set; }

    /// <summary>
    /// Gets or sets the user’s enterprise email address.
    /// </summary>
    [Description("The user’s enterprise email address.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("enterprise_email")]
    public string? EnterpriseEmail { get; set; }

    /// <summary>
    /// Gets or sets the user’s job title.
    /// </summary>
    [Description("The user’s job title.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("job_title")]
    public string? JobTitle { get; set; }

    /// <summary>
    /// Gets or sets whether the user is frozen.
    /// </summary>
    [Description("Whether the user is frozen.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("is_frozen")]
    public bool? IsFrozen { get; set; }

    /// <summary>
    /// Gets or sets the user’s data residency region.
    /// </summary>
    [Description("The user’s data residency region.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("geo")]
    public string? Geo { get; set; }

    /// <summary>
    /// Gets or sets the user’s job level identifier.
    /// </summary>
    [Description("The user’s job level identifier.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("job_level_id")]
    public string? JobLevelId { get; set; }

    /// <summary>
    /// Gets or sets the user’s job family identifier.
    /// </summary>
    [Description("The user’s job family identifier.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("job_family_id")]
    public string? JobFamilyId { get; set; }

    /// <summary>
    /// Gets or sets the list of subscription identifiers assigned to the user.
    /// </summary>
    [Description("The list of subscription identifiers assigned to the user.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("subscription_ids")]
    public List<string>? SubscriptionIds { get; set; }

    /// <summary>
    /// Gets or sets the list of subscriptions assigned to the user.
    /// </summary>
    [Description("The list of subscriptions assigned to the user.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("assign_info")]
    public List<LarkUserAssignInfo>? AssignInfo { get; set; }

    /// <summary>
    /// Gets or sets the user’s department path.
    /// </summary>
    [Description("The user’s department path.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("department_path")]
    public List<LarkUserDepartmentPathInfo>? DepartmentPath { get; set; }

    /// <summary>
    /// Gets or sets the list of the user’s dotted-line manager identifiers.
    /// </summary>
    [Description("The list of the user’s dotted-line manager identifiers.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("dotted_line_leader_user_ids")]
    public List<string>? DottedLineLeaderUserIds { get; set; }

    /// <summary>
    /// Gets or sets whether the user is an agent.
    /// </summary>
    [Description("Whether the user is an agent.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("is_agent")]
    public bool? IsAgent { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }

    public string GetId()
        => OpenId ?? UserId ?? UnionId ?? string.Empty;

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => $"{Nickname ?? Name} ({EnglishName}) - {GetId()}";
}

/// <summary>
/// User avatar information.
/// </summary>
[Description("User avatar information.")]
public class LarkUserAvatarInfo
{
    /// <summary>
    /// Gets or sets the 72-by-72-pixel avatar URL.
    /// </summary>
    [Description("The 72-by-72-pixel avatar URL.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("avatar_72")]
    public string? Avatar72 { get; set; }

    /// <summary>
    /// Gets or sets the 240-by-240-pixel avatar URL.
    /// </summary>
    [Description("The 240-by-240-pixel avatar URL.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("avatar_240")]
    public string? Avatar240 { get; set; }

    /// <summary>
    /// Gets or sets the 640-by-640-pixel avatar URL.
    /// </summary>
    [Description("The 640-by-640-pixel avatar URL.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("avatar_640")]
    public string? Avatar640 { get; set; }

    /// <summary>
    /// Gets or sets the original avatar URL.
    /// </summary>
    [Description("The original avatar URL.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("avatar_origin")]
    public string? AvatarOrigin { get; set; }
}

/// <summary>
/// User status information.
/// </summary>
[Description("User status information.")]
public class LarkContactUserStatusInfo
{
    /// <summary>
    /// Gets or sets whether the user is frozen.
    /// </summary>
    [Description("Whether the user is frozen.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("is_frozen")]
    public bool? IsFrozen { get; set; }

    /// <summary>
    /// Gets or sets whether the user has resigned.
    /// </summary>
    [Description("Whether the user has resigned.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("is_resigned")]
    public bool? IsResigned { get; set; }

    /// <summary>
    /// Gets or sets whether the user is activated.
    /// </summary>
    [Description("Whether the user is activated.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("is_activated")]
    public bool? IsActivated { get; set; }

    /// <summary>
    /// Gets or sets whether the user has voluntarily left.
    /// </summary>
    [Description("Whether the user has voluntarily left.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("is_exited")]
    public bool? IsExited { get; set; }

    /// <summary>
    /// Gets or sets whether the user has not yet joined the organization or team.
    /// </summary>
    [Description("Whether the user has not yet joined the organization or team.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("is_unjoin")]
    public bool? IsUnjoin { get; set; }
}

/// <summary>
/// User ordering information within a department.
/// </summary>
[Description("User ordering information within a department.")]
public class LarkUserOrderInfo
{
    /// <summary>
    /// Gets or sets the department identifier for this ordering information.
    /// </summary>
    [Description("The department identifier for this ordering information.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("department_id")]
    public string? DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the user’s order within their direct department; higher values appear first.
    /// </summary>
    [Description("The user’s order within their direct department; higher values appear first.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("user_order")]
    public int? UserOrder { get; set; }

    /// <summary>
    /// Gets or sets the order among the user’s departments; higher values appear first.
    /// </summary>
    [Description("The order among the user’s departments; higher values appear first.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("department_order")]
    public int? DepartmentOrder { get; set; }

    /// <summary>
    /// Gets or sets whether this is the user’s primary department.
    /// </summary>
    [Description("Whether this is the user’s primary department.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("is_primary_dept")]
    public bool? IsPrimaryDepartment { get; set; }
}

/// <summary>
/// A custom user attribute.
/// </summary>
[Description("A custom user attribute.")]
public class LarkUserCustomAttributeInfo
{
    /// <summary>
    /// Gets or sets the custom attribute type.
    /// </summary>
    [Description("The custom attribute type.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the custom attribute identifier.
    /// </summary>
    [Description("The custom attribute identifier.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the custom attribute value.
    /// </summary>
    [Description("The custom attribute value.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("value")]
    public LarkUserCustomAttributeValueInfo? Value { get; set; }
}

/// <summary>
/// The value of a custom user attribute.
/// </summary>
[Description("The value of a custom user attribute.")]
public class LarkUserCustomAttributeValueInfo
{
    /// <summary>
    /// Gets or sets the text value or the title of a URL attribute.
    /// </summary>
    [Description("The text value or the title of a URL attribute.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the default URL of a URL attribute.
    /// </summary>
    [Description("The default URL of a URL attribute.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the PC URL of a URL attribute.
    /// </summary>
    [Description("The PC URL of a URL attribute.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("pc_url")]
    public string? PcUrl { get; set; }

    /// <summary>
    /// Gets or sets the option identifier of an enumeration attribute.
    /// </summary>
    [Description("The option identifier of an enumeration attribute.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("option_id")]
    public string? OptionId { get; set; }

    /// <summary>
    /// Gets or sets the option value of an enumeration attribute.
    /// </summary>
    [Description("The option value of an enumeration attribute.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("option_value")]
    public string? OptionValue { get; set; }

    /// <summary>
    /// Gets or sets the name of an image attribute option.
    /// </summary>
    [Description("The name of an image attribute option.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the URL of an image attribute option.
    /// </summary>
    [Description("The URL of an image attribute option.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("picture_url")]
    public string? PictureUrl { get; set; }

    /// <summary>
    /// Gets or sets the referenced user information.
    /// </summary>
    [Description("The referenced user information.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("generic_user")]
    public LarkUserCustomAttributeGenericUserInfo? GenericUser { get; set; }
}

/// <summary>
/// Referenced user information in a custom user attribute.
/// </summary>
[Description("Referenced user information in a custom user attribute.")]
public class LarkUserCustomAttributeGenericUserInfo
{
    /// <summary>
    /// Gets or sets the identifier of the referenced user.
    /// </summary>
    [Description("The identifier of the referenced user.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the referenced user type, currently fixed at 1.
    /// </summary>
    [Description("The referenced user type, currently fixed at 1.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("type")]
    public int? Type { get; set; }
}

/// <summary>
/// User subscription information.
/// </summary>
[Description("User subscription information.")]
public class LarkUserAssignInfo
{
    /// <summary>
    /// Gets or sets the subscription identifier.
    /// </summary>
    [Description("The subscription identifier.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("subscription_id")]
    public string? SubscriptionId { get; set; }

    /// <summary>
    /// Gets or sets the subscription license plan key.
    /// </summary>
    [Description("The subscription license plan key.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("license_plan_key")]
    public string? LicensePlanKey { get; set; }

    /// <summary>
    /// Gets or sets the subscription name.
    /// </summary>
    [Description("The subscription name.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    /// <summary>
    /// Gets or sets the localized subscription name.
    /// </summary>
    [Description("The localized subscription name.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("i18n_name")]
    public LarkUserI18nNameInfo? I18nName { get; set; }

    /// <summary>
    /// Gets or sets the subscription start time as a Unix timestamp in seconds.
    /// </summary>
    [Description("The subscription start time as a Unix timestamp in seconds.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>
    /// Gets or sets the subscription end time as a Unix timestamp in seconds.
    /// </summary>
    [Description("The subscription end time as a Unix timestamp in seconds.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }
}

/// <summary>
/// Localized subscription or department names.
/// </summary>
[Description("Localized subscription or department names.")]
public class LarkUserI18nNameInfo
{
    /// <summary>
    /// Gets or sets the Chinese name.
    /// </summary>
    [Description("The Chinese name.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("zh_cn")]
    public string? ZhCn { get; set; }

    /// <summary>
    /// Gets or sets the Japanese name.
    /// </summary>
    [Description("The Japanese name.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("ja_jp")]
    public string? JaJp { get; set; }

    /// <summary>
    /// Gets or sets the English name.
    /// </summary>
    [Description("The English name.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("en_us")]
    public string? EnUs { get; set; }
}

/// <summary>
/// User department path information.
/// </summary>
[Description("User department path information.")]
public class LarkUserDepartmentPathInfo
{
    /// <summary>
    /// Gets or sets the department identifier.
    /// </summary>
    [Description("The department identifier.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("department_id")]
    public string? DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the department name information.
    /// </summary>
    [Description("The department name information.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("department_name")]
    public LarkUserDepartmentNameInfo? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets the department path and its name information.
    /// </summary>
    [Description("The department path and its name information.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("department_path")]
    public LarkUserDepartmentPathDetailInfo? DepartmentPath { get; set; }
}

/// <summary>
/// Department name and localized names.
/// </summary>
[Description("Department name and localized names.")]
public class LarkUserDepartmentNameInfo
{
    /// <summary>
    /// Gets or sets the department name.
    /// </summary>
    [Description("The department name.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the localized department name.
    /// </summary>
    [Description("The localized department name.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("i18n_name")]
    public LarkUserI18nNameInfo? I18nName { get; set; }
}

/// <summary>
/// Department path and its name information.
/// </summary>
[Description("Department path and its name information.")]
public class LarkUserDepartmentPathDetailInfo
{
    /// <summary>
    /// Gets or sets the list of department identifiers in the path.
    /// </summary>
    [Description("The list of department identifiers in the path.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("department_ids")]
    public List<string>? DepartmentIds { get; set; }

    /// <summary>
    /// Gets or sets the department path name information.
    /// </summary>
    [Description("The department path name information.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("department_path_name")]
    public LarkUserDepartmentNameInfo? DepartmentPathName { get; set; }
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
