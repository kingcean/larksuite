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
using Trivial.Collection;
using Trivial.CommandLine;
using Trivial.Data;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.CommandLine;

public partial class LarkUsersCommandVerb : BaseCommandVerb
{
    private static List<string> employeeCoreFields = ["person_info.email_address", "person_info.gender", "person_info.legal_name", "person_info.phone_number", "person_info.preferred_name", "person_info.preferred_english_full_name", "person_info.preferred_local_full_name", "person_info.person_id", "person_info.date_of_birth", "person_info.talent_id", "avatar_url", "work_location_id", "employee_number", "job.id", "job.name", "job.job_title", "job_level.level_order", "job_level.name", "job_level.id", "compensation_type", "employment_status", "employment_type", "pay_group_id", "expiration_date", "effective_date", "contract_start_date", "contract_end_date", "contract_expected_end_date", "regular_employee_start_date", "department.department_name", "department.id", "department.id_v2"];
    private static List<string> employeeLimitFields = ["person_info.email_address", "person_info.gender", "person_info.legal_name", "person_info.phone_number", "person_info.preferred_name", "person_info.preferred_english_full_name", "person_info.preferred_local_full_name", "person_info.person_id", "person_info.date_of_birth", "person_info.talent_id", "avatar_url", "work_location_id", "employee_number", "job.id", "job.name", "job.job_title", "job_level.level_order", "job_level.name", "job_level.id", "compensation_type", "employment_status", "employment_type", "pay_group_id", "expiration_date", "effective_date", "contract_start_date", "contract_end_date", "contract_expected_end_date", "regular_employee_start_date", "department.department_name", "department.id", "department.id_v2", "custom_fields"];

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
        var item = resp.Data.FirstOrDefault();
        return item;
    }

    public static async Task<LarkResponsePagingBody> GetEmployeesAsync(LarkApi? larkApi, List<string> ids, bool withCustomFields = false, CancellationToken cancellationToken = default)
    {
        if (ids is null) return new(true, "No employee identifier provided.");
        larkApi ??= LarkApi.DefaultInstance;
        return await larkApi.GetEmployeesAsync(new LarkEmployeeResolveRequest("employment", ids, withCustomFields ? employeeLimitFields : employeeCoreFields), cancellationToken);
    }

    public static async Task<LarkEmployeeInfo?> SelectEmployeeAsync(LarkApi? larkApi, StyleConsole console, LarkResponsePagingBody resp, DataCacheCollection<LarkEmployeeInfo>? cache = null, CancellationToken cancellationToken = default)
    {
        if (resp?.Data is null || resp.IsError)
        {
            LarkCliUtils.WriteEmpty(console, resp);
            return null;
        }

        return await SelectEmployeeAsync(larkApi, console, resp.Data, cache, cancellationToken);
    }

    public static async Task<LarkEmployeeInfo?> SelectEmployeeAsync(LarkApi? larkApi, StyleConsole console, IEnumerable<JsonObjectNode> col, DataCacheCollection<LarkEmployeeInfo>? cache = null, CancellationToken cancellationToken = default)
    {
        console ??= StyleConsole.Default;
        larkApi ??= LarkApi.DefaultInstance;
        var selection = new SelectionData<LarkEmployeeInfo>();
        foreach (var item in col)
        {
            if (item is null) continue;
            var employee = SimplifyEmployee(item);
            selection.Add(new($"{employee.Name}\t{employee.Job?.Title}", employee));
        }

        if (selection.Count < 1)
        {
            console.WriteLine(ConsoleColor.Red, "No employee found.");
            return null;
        }

        if (selection.Count == 1)
        {
            var onlyOne = selection.FirstOrDefault()!.Data;
            LarkCliUtils.WriteLine(console, onlyOne);
            return onlyOne;
        }

        var select = console.Select(selection, LarkCliUtils.GetItemSelectionOptions());
        var model = select.Data;
        if (model is not null)
        {
            console.WriteLine();
            LarkCliUtils.WriteLine(console, model);
            if (cache is not null && !string.IsNullOrWhiteSpace(model.Id)) cache[model.Id] = select.Data;
            return model;
        }

        var id = select.Value;
        if (string.IsNullOrWhiteSpace(id) || LarkCliUtils.IsToExit(id)) return null;
        if (cache is not null && cache.TryGet(id, out model) && model is not null)
        {
            console.WriteLine();
            LarkCliUtils.WriteLine(console, model);
            return model;
        }

        var resp = await GetEmployeeAsync(larkApi, id, false, cancellationToken);
        if (resp is null)
        {
            console.WriteLine(ConsoleColor.Red, "Not found.");
            return null;
        }

        model = SimplifyEmployee(resp);
        if (cache is not null && !string.IsNullOrWhiteSpace(model.Id)) cache[model.Id] = model;
        console.WriteLine();
        LarkCliUtils.WriteLine(console, model);
        return model;
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

            var id = GetEmployeeId(employee);
            if (id is null)
            {
                console.WriteLine();
                continue;
            }

            console.Write(" \t");
            console.WriteLine(ConsoleColor.DarkGray, id);
        }
    }

    public static async Task<LarkEmployeeInfo?> WriteEmployeeAsync(StyleConsole console, string id, CancellationToken cancellationToken = default)
    {
        console ??= StyleConsole.Default;
        if (string.IsNullOrWhiteSpace(id)) return null;
        var resp = await GetEmployeeAsync(null, id, false, cancellationToken);
        if (resp is null)
        {
            console.WriteLine(ConsoleColor.Red, "Not found.");
            return null;
        }

        var model = SimplifyEmployee(resp);
        LarkCliUtils.WriteLine(console, model);
        return model;
    }

    public static async Task<LarkEmployeeInfo?> WriteEmployeeInPropertiesAsync(StyleConsole console, LarkApi? larkApi, string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        console ??= StyleConsole.Default;
        larkApi ??= LarkApi.DefaultInstance;
        var resp = await GetEmployeeAsync(larkApi, id, false, cancellationToken);
        if (resp is null)
        {
            LarkCliUtils.WritePropertyLine(console, "ID", id);
            return null;
        }

        var info = SimplifyEmployee(resp);
        LarkCliUtils.WritePropertyLine(console, "ID", info.Id);
        LarkCliUtils.WritePropertyLine(console, "Name", info.Name);
        LarkCliUtils.WritePropertyLine(console, "Title", info.Job?.Title);
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Email", info.Info?.Email);
        return info;
    }

    public static async Task<IReadOnlyList<JsonObjectNode>> WriteEmployeesAsync(StyleConsole console, DateTime effectiveStartDate, DataCacheCollection<LarkEmployeeInfo>? cache, CancellationToken cancellationToken = default)
    {
        var larkApi = LarkApi.DefaultInstance;
        console ??= StyleConsole.Default;
        var resp = await ListEmployeesAsync(null, effectiveStartDate, false, cancellationToken);
        await SelectEmployeeAsync(larkApi, console, resp, cache, cancellationToken);
        return resp?.Data ?? [];
    }

    public static Task<IReadOnlyList<JsonObjectNode>> WriteEmployeesAsync(StyleConsole console, DateTime effectiveStartDate, CancellationToken cancellationToken = default)
        => WriteEmployeesAsync(console, effectiveStartDate, null, cancellationToken);

    public static async Task<IReadOnlyList<JsonObjectNode>> WriteEmployeesAsync(StyleConsole console, string q, DataCacheCollection<LarkEmployeeInfo>? cache, CancellationToken cancellationToken = default)
    {
        var larkApi = LarkApi.DefaultInstance;
        console ??= StyleConsole.Default;
        var resp = await SearchEmployeesAsync(null, q, false, cancellationToken);
        await SelectEmployeeAsync(larkApi, console, resp, cache, cancellationToken);
        return resp?.Data ?? [];
    }

    public static Task<IReadOnlyList<JsonObjectNode>> WriteEmployeesAsync(StyleConsole console, string q, CancellationToken cancellationToken = default)
        => WriteEmployeesAsync(console, q, null, cancellationToken);

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
        var info = employee.TryGetObjectValue("person_info") ?? [];
        var gender = LarkApiUtils.ParseGender(info.TryGetObjectValue("gender")?.TryGetStringValue("enum_name"));
        return new()
        {
            Id = GetEmployeeId(employee),
            Name = GetEmployeeName(employee),
            AvatarUrl = employee.TryGetStringTrimmedValue("avatar_url", true),
            Info = new()
            {
                LegalName = info.TryGetStringTrimmedValue("legal_name", true),
                Email = info.TryGetStringTrimmedValue("email_address", true),
                Phone = info.TryGetStringTrimmedValue("phone_number", true),
                Birthday = info.TryGetStringTrimmedValue("date_of_birth", true),
                Gender = gender,
            },
            Job = new()
            {
                Title = job.Name,
                JobId = job.Id,
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
            },
            Employment = new()
            {
                Status = employee.TryGetObjectValue("employment_status")?.TryGetStringValue("enum_name"),
                EmploymentType = employee.TryGetObjectValue("employment_type")?.TryGetStringValue("enum_name"),
                EffectiveDate = employee.TryGetStringTrimmedValue("effective_date", true),
                ContractStartDate = employee.TryGetStringTrimmedValue("contract_start_date", true),
                ContractEndDate = employee.TryGetStringTrimmedValue("contract_end_date", true),
                EmployeeNumber = employee.TryGetStringTrimmedValue("employee_number", true),
                TalentId = info.TryGetStringTrimmedValue("talent_id", true),
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

public static partial class LarkCliUtils
{
    public static void WriteLine(this StyleConsole console, LarkEmployeeInfo employee)
    {
        if (employee is null) return;
        console ??= StyleConsole.Default;
        console.WriteLine(BoldText(), employee.Name ?? "?");
        console.WriteLine(ConsoleColor.Yellow, employee.Id ?? "?");

        var jobInfo = employee.Job;
        if (jobInfo is not null)
        {
            console.WriteLine();
            console.WriteLine(ItalicText(), "Job Info");
            WritePropertyLineIfNotEmpty(console, "Department", jobInfo.Department?.Name, jobInfo.Department?.Id);
            WritePropertyLineIfNotEmpty(console, "Title", jobInfo.Title, jobInfo.JobId);
            WritePropertyLineIfNotEmpty(console, "Level", jobInfo.Level?.Name, jobInfo.Level?.Id);
        }

        var info = employee.Info;
        if (info is not null)
        {
            console.WriteLine();
            console.WriteLine(ItalicText(), "Contact and Basic Info");
            var legalName = info.LegalName;
            if (legalName is not null && legalName != employee.Name) WritePropertyLineIfNotEmpty(console, "Name", legalName);
            WritePropertyLineIfNotEmpty(console, "Email", info.Email);
            WritePropertyLineIfNotEmpty(console, "Phone", info.Phone);
            WritePropertyLineIfNotEmpty(console, "Birthday", info.Birthday);
            WritePropertyLineIfNotEmpty(console, "Gender", LarkApiUtils.ToString(info.Gender));
        }
    }
}
