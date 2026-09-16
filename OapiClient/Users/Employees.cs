using LarkSuite;
using LarkSuite.OapiModels;
using LarkSuite.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text;
using System.Text.Json;
using Trivial.CommandLine;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.CommandLine;

public partial class LarkUsersCommandVerb : BaseCommandVerb
{
    private static List<string> employeeCoreFields = ["person_info.email_address", "person_info.gender", "person_info.legal_name", "person_info.phone_number", "person_info.preferred_name", "person_info.preferred_english_full_name", "person_info.preferred_local_full_name", "person_info.person_id", "person_info.date_of_birth", "person_info.talent_id", "avatar_url", "work_location_id", "employee_number", "job.id", "job.name", "job.job_title", "job_level.level_order", "job_level.name", "job_level.id", "compensation_type", "pay_group_id", "expiration_date", "effective_date", "contract_start_date", "contract_end_date", "contract_expected_end_date", "regular_employee_start_date", "department.department_name", "department.id", "department.id_v2"];
    private static List<string> employeeLimitFields = ["person_info.email_address", "person_info.gender", "person_info.legal_name", "person_info.phone_number", "person_info.preferred_name", "person_info.preferred_english_full_name", "person_info.preferred_local_full_name", "person_info.person_id", "person_info.date_of_birth", "person_info.talent_id", "avatar_url", "work_location_id", "employee_number", "job.id", "job.name", "job.job_title", "job_level.level_order", "job_level.name", "job_level.id", "compensation_type", "pay_group_id", "expiration_date", "effective_date", "contract_start_date", "contract_end_date", "contract_expected_end_date", "regular_employee_start_date", "department.department_name", "department.id", "department.id_v2", "custom_fields"];

    public static async Task<LarkResponsePagingBody> ListEmployeesAsync(LarkApi? larkApi, DateTime effectiveStartDate, bool withCustomFields = false, CancellationToken cancellationToken = default)
    {
        larkApi ??= LarkApi.DefaultInstance;
        var resp = await larkApi.SearchEmployeesAsync(new LarkEmployeeSearchRequest()
        {
            Fields = withCustomFields ? employeeLimitFields : employeeCoreFields,
            EffectiveStartDate = effectiveStartDate.ToString("yyyy-MM-dd"),
            EmploymentStatus = "hired",
        }, new(50), cancellationToken);
        if (resp is null) return new(true, "No response.");
        if (resp.Data is null || resp.IsError) return resp;
        await LarkApiUtils.LoadAllPagesAsync(resp, 50, larkApi.SearchEmployeesAsync, cancellationToken).CountAsync(cancellationToken);
        return resp;
    }

    public static async Task<LarkResponsePagingBody> SearchEmployeesAsync(LarkApi? larkApi, string q, bool withCustomFields = false, CancellationToken cancellationToken = default)
    {
        larkApi ??= LarkApi.DefaultInstance;
        var resp = await larkApi.SearchEmployeesAsync(q, withCustomFields ? employeeLimitFields : employeeCoreFields, cancellationToken);
        if (resp is null) return new(true, "No response.");
        if (resp.Data is null || resp.IsError) return resp;
        await LarkApiUtils.LoadAllPagesAsync(resp, 50, larkApi.SearchEmployeesAsync, cancellationToken).CountAsync(cancellationToken);
        return resp;
    }

    public static async Task<LarkResponsePagingBody> GetDepartmentEmployeesAsync(LarkApi? larkApi, List<string> ids, bool withCustomFields = false, CancellationToken cancellationToken = default)
    {
        larkApi ??= LarkApi.DefaultInstance;
        var resp = await larkApi.SearchEmployeesAsync(new LarkEmployeeSearchRequest()
        {
            Fields = withCustomFields ? employeeLimitFields : employeeCoreFields,
            DepartmentIds = ids,
            EmploymentStatus = "hired",
        }, new(50), cancellationToken);
        if (resp is null) return new(true, "No response.");
        if (resp.Data is null || resp.IsError) return resp;
        await LarkApiUtils.LoadAllPagesAsync(resp, 50, larkApi.SearchEmployeesAsync, cancellationToken).CountAsync(cancellationToken);
        return resp;
    }

    public static async Task<JsonObjectNode?> GetEmployeeAsync(LarkApi? larkApi, string id, bool withCustomFields = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        larkApi ??= LarkApi.DefaultInstance;
        var resp = await larkApi.GetEmployeesAsync(new LarkEmployeeResolveRequest(id, withCustomFields ? employeeLimitFields : employeeCoreFields), cancellationToken);
        if (resp?.Data is null || resp.IsError) return null;
        return resp.Data.FirstOrDefault();
    }

    public static async Task<LarkResponsePagingBody> GetEmployeesAsync(LarkApi? larkApi, List<string> ids, bool withCustomFields = false, CancellationToken cancellationToken = default)
    {
        if (ids is null) return new(true, "No employee identifier provided.");
        larkApi ??= LarkApi.DefaultInstance;
        return await larkApi.GetEmployeesAsync(new LarkEmployeeResolveRequest("employment", ids, withCustomFields ? employeeLimitFields : employeeCoreFields), cancellationToken);
    }

    public static void WriteEmployees(StyleConsole console, IEnumerable<JsonObjectNode> col)
    {
        console ??= StyleConsole.Default;
        foreach (var employee in col)
        {
            console.Append(ConsoleColor.Blue, "· ");
            var name = GetEmployeeName(employee);
            console.Append(name ?? "?");
            var jobTitle = GetEmployeeJobTitle(employee);
            if (!string.IsNullOrWhiteSpace(jobTitle))
            {
                console.Append(" \t");
                console.Append(jobTitle);
            }

            var id = employee.TryGetStringTrimmedValue("employment_id_v2", true) ?? employee.TryGetStringTrimmedValue("employment_id", true);
            if (id is null)
            {
                console.WriteLine();
                continue;
            }

            console.Write(" \t");
            console.WriteLine(ConsoleColor.DarkGray, id);
        }
    }

    public static void WriteEmployee(StyleConsole console, JsonObjectNode employee)
    {
        if (employee is null) return;
        console ??= StyleConsole.Default;
        var employeeName = GetEmployeeName(employee, out var info);
        if (info is null) return;
        console.WriteLine(LarkCliUtils.BoldText(), employeeName ?? "?");
        console.WriteLine(ConsoleColor.Yellow, GetEmployeeId(employee) ?? "?");

        console.WriteLine();
        console.WriteLine(LarkCliUtils.ItalicText(), "Job Info");
        var deptInfo = employee.TryGetObjectValue("department");
        var deptName = GetName(deptInfo, "department_name");
        var deptId = deptInfo?.TryGetStringTrimmedValue("id_v2", true) ?? deptInfo?.TryGetStringTrimmedValue("id", true) ?? employee.TryGetStringTrimmedValue("department_id_v2", true) ?? employee.TryGetStringTrimmedValue("department_id", true);
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Department", deptName, deptId);
        var jobTitle = GetEmployeeJobTitle(employee, out var jobInfo);
        var jobId = jobInfo?.TryGetStringTrimmedValue("id", true) ?? employee.TryGetStringTrimmedValue("job_id", true);
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Title", jobTitle, jobId);
        var jobLevel = employee.TryGetObjectValue("job_level");
        if (jobLevel is not null) LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Level", GetName(jobLevel), jobLevel.TryGetStringTrimmedValue("id", true) ?? employee.TryGetStringTrimmedValue("job_level_id", true));

        console.WriteLine();
        console.WriteLine(LarkCliUtils.ItalicText(), "Contact and Basic Info");
        var legalName = info.TryGetStringTrimmedValue("legal_name", true);
        if (legalName is not null && legalName != employeeName) LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Name", legalName);
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Email", info.TryGetStringValue("email_address"));
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Phone", info.TryGetStringValue("phone_number"));
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Birthday", info.TryGetStringValue("date_of_birth"));
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Gender", info.TryGetObjectValue("gender")?.TryGetStringValue("enum_name")?.ToSpecificCase(Cases.Capitalize));
    }

    public static async Task<JsonObjectNode?> WriteEmployeeAsync(StyleConsole console, string id, CancellationToken cancellationToken = default)
    {
        console ??= StyleConsole.Default;
        if (string.IsNullOrWhiteSpace(id)) return null;
        var resp = await GetEmployeeAsync(null, id, false, cancellationToken);
        if (resp is null)
        {
            console.WriteLine(ConsoleColor.Red, "Not found.");
            return null;
        }

        WriteEmployee(console, resp);
        return resp;
    }

    public static async Task<JsonObjectNode?> WriteEmployeeInPropertiesAsync(StyleConsole console, LarkApi? larkApi, string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        console ??= StyleConsole.Default;
        larkApi ??= LarkApi.DefaultInstance;
        var info = await GetEmployeeAsync(larkApi, id, false, cancellationToken);
        if (info is null)
        {
            LarkCliUtils.WritePropertyLine(console, "ID", id);
            return null;
        }

        LarkCliUtils.WritePropertyLine(console, "ID", GetEmployeeId(info));
        LarkCliUtils.WritePropertyLine(console, "Name", GetEmployeeName(info));
        LarkCliUtils.WritePropertyLine(console, "Title", GetEmployeeJobTitle(info));
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Email", info.TryGetStringValue("email_address"));
        return info;
    }

    public static async Task<IReadOnlyList<JsonObjectNode>> WriteEmployeesAsync(StyleConsole console, DateTime effectiveStartDate, CancellationToken cancellationToken = default)
    {
        var larkApi = LarkApi.DefaultInstance;
        console ??= StyleConsole.Default;
        var resp = await ListEmployeesAsync(null, effectiveStartDate, false, cancellationToken);
        if (resp?.Data is null || resp.IsError)
        {
            LarkCliUtils.WriteEmpty(console, resp);
            return [];
        }

        WriteEmployees(console, resp.Data);
        return resp.Data;
    }

    public static async Task<IReadOnlyList<JsonObjectNode>> WriteEmployeesAsync(StyleConsole console, string q, CancellationToken cancellationToken = default)
    {
        var larkApi = LarkApi.DefaultInstance;
        console ??= StyleConsole.Default;
        var resp = await SearchEmployeesAsync(null, q, false, cancellationToken);
        if (resp?.Data is null || resp.IsError)
        {
            LarkCliUtils.WriteEmpty(console, resp);
            return [];
        }

        WriteEmployees(console, resp.Data);
        return resp.Data;
    }

    public static string? GetEmployeeId(JsonObjectNode employee)
        => employee.TryGetStringTrimmedValue("employment_id_v2", true) ?? employee.TryGetStringTrimmedValue("employment_id", true);

    public static LarkIdNameStaticInfo GetDepartmentByEmployee(JsonObjectNode employee)
    {
        if (employee is null) return new();
        var deptInfo = employee.TryGetObjectValue("department");
        var deptName = GetName(deptInfo, "department_name");
        var deptId = deptInfo?.TryGetStringTrimmedValue("id_v2", true) ?? deptInfo?.TryGetStringTrimmedValue("id", true) ?? employee.TryGetStringTrimmedValue("department_id_v2", true) ?? employee.TryGetStringTrimmedValue("department_id", true);
        return new(deptId, deptName);
    }

    public static LarkIdNameStaticInfo GetJobByEmployee(JsonObjectNode employee)
    {
        if (employee is null) return new();
        var jobTitle = GetEmployeeJobTitle(employee, out var jobInfo);
        var jobId = jobInfo?.TryGetStringTrimmedValue("id", true) ?? employee.TryGetStringTrimmedValue("job_id", true);
        return new(jobId, jobTitle);
    }

    public static LarkIdNameStaticInfo GetJobLevelByEmployee(JsonObjectNode employee)
    {
        var jobLevel = employee?.TryGetObjectValue("job_level");
        if (jobLevel is null) return new();
        return new(jobLevel.TryGetStringTrimmedValue("id", true) ?? employee!.TryGetStringTrimmedValue("job_level_id", true), GetName(jobLevel));
    }

    public static string? GetEmployeeName(JsonObjectNode employee)
        => GetEmployeeName(employee, out _);

    public static LarkEmployeeInfo SimplifyEmployee(JsonObjectNode employee)
    {
        var department = GetDepartmentByEmployee(employee);
        var job = GetJobByEmployee(employee);
        var jobLevel = GetJobLevelByEmployee(employee);
        return new()
        {
            Id = GetEmployeeId(employee),
            Name = GetEmployeeName(employee),
            AvatarUrl = employee.TryGetStringTrimmedValue("avatar_url", true),
            Info = new()
            {
                LegalName = employee.TryGetStringTrimmedValue("legal_name", true),
                Email = employee.TryGetStringTrimmedValue("email_address", true),
                Phone = employee.TryGetStringTrimmedValue("phone_number", true),
                Birthday = employee.TryGetStringTrimmedValue("date_of_birth", true),
                Gender = employee.TryGetObjectValue("gender")?.TryGetStringValue("enum_name")?.ToSpecificCase(Cases.Capitalize),
            },
            Job = new()
            {
                Title = job?.Name,
                JobId = job?.Id,
                EmployeeNumber = employee.TryGetStringTrimmedValue("employee_number", true),
                Level = new()
                {
                    Id = jobLevel?.Id,
                    Name = jobLevel?.Name,
                },
                Department = new()
                {
                    Id = department?.Id,
                    Name = department?.Name,
                },
            }
        };
    }

    private static string? GetEmployeeName(JsonObjectNode employee, out JsonObjectNode? personInfo)
    {
        var info = employee?.TryGetObjectValue("person_info");
        personInfo = info;
        info ??= employee;
        if (info is null) return null;
        return employee!.TryGetStringTrimmedValue("preferred_name", true)
            ?? info.TryGetStringTrimmedValue("preferred_name", true)
            ?? info.TryGetStringTrimmedValue("preferred_local_full_name", true)
            ?? info.TryGetStringTrimmedValue("legal_name", true)
            ?? info.TryGetStringTrimmedValue("preferred_english_full_name", true);
    }

    private static string? GetEmployeeJobTitle(JsonObjectNode employee, out JsonObjectNode? jobInfo)
    {
        jobInfo = employee?.TryGetObjectValue("job");
        return GetName(jobInfo ?? employee);
    }

    private static string? GetEmployeeJobTitle(JsonObjectNode employee)
        => GetEmployeeJobTitle(employee, out _);
}
