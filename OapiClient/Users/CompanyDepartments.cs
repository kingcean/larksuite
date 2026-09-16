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
using System.Xml.Serialization;
using Trivial.CommandLine;
using Trivial.Data;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.CommandLine;

public partial class LarkUsersCommandVerb : BaseCommandVerb
{
    private static List<string> departmentFields = ["version_id", "sub_type", "manager", "is_root", "is_confidential", "effective_date", "expiration_date", "department_name", "parent_department_id", "tree_order", "list_order", "code", "active", "description", "custom_fields", "staffing_model", "cost_center_id", "created_time", "updated_time", "created_by", "updated_by", "record_created_time", "record_updated_time", "record_created_by", "record_updated_by"];

    public static async Task<JsonObjectNode?> GetDepartmentAsync(LarkApi? larkApi, string id, bool withCustomFields = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        larkApi ??= LarkApi.DefaultInstance;
        var resp = await larkApi.GetCompanyDepartmentsAsync(new LarkCompanyDepartmentResolveRequest([id], false, departmentFields), cancellationToken);
        if (resp?.Data is null || resp.IsError) return null;
        return resp.Data.FirstOrDefault();
    }

    public static void WriteDepartments(StyleConsole console, IEnumerable<JsonObjectNode> col)
    {
        console ??= StyleConsole.Default;
        foreach (var department in col)
        {
            if (department.TryGetBooleanValue("active") == false)
                console.Append(ConsoleColor.Red, "× ");
            else
                console.Append(ConsoleColor.Blue, "· ");

            var name = GetDepartmentName(department);
            console.Append(name ?? "?");

            var id = department.TryGetStringTrimmedValue("id", true);
            if (id is null)
            {
                console.WriteLine();
                continue;
            }

            console.Write(" \t");
            console.WriteLine(ConsoleColor.DarkGray, id);
        }
    }

    public static void WriteDepartment(StyleConsole console, JsonObjectNode department)
        => WriteDepartment(console, department, false);

    public static async Task<JsonObjectNode?> WriteDepartmentAsync(StyleConsole console, string id, CancellationToken cancellationToken = default)
    {
        var larkApi = LarkApi.DefaultInstance;
        console ??= StyleConsole.Default;
        if (string.IsNullOrWhiteSpace(id)) return null;
        var resp = await GetDepartmentAsync(larkApi, id, false, cancellationToken);
        if (resp is null)
        {
            console.WriteLine(ConsoleColor.Red, "Not found.");
            return null;
        }

        WriteDepartment(console, resp, true);

        var childrenTask = GetSubordinateDepartmentsAsync(larkApi, id, true, cancellationToken);
        var managerId = resp.TryGetStringTrimmedValue("manager", true);
        if (managerId is not null)
        {
            console.WriteLine();
            console.WriteLine(LarkCliUtils.ItalicText(), "Manager");
            await WriteEmployeeInPropertiesAsync(console, larkApi, managerId, cancellationToken);
        }

        var employeesTask = GetDepartmentEmployeesAsync(larkApi, [id], false, cancellationToken);
        var parentId = resp.TryGetStringTrimmedValue("parent_department_id", true);
        if (parentId is not null)
        {
            console.WriteLine();
            console.WriteLine(LarkCliUtils.ItalicText(), "Parent Department");
            var parentPathTask = larkApi.GetCompanyDepartmentsParentAsync([parentId], cancellationToken);
            await WriteDepartmentInPropertiesAsync(console, larkApi, parentId, false, cancellationToken);
            var parentPath = (await parentPathTask)?.Data?.Get(parentId);
            if (parentPath?.Parents is not null && parentPath.Parents.Count > 0)
            {
                console.WriteLine();
                console.WriteLine(LarkCliUtils.ItalicText(), "Higher-Level Department");
                var i = 1;
                foreach (var p in parentPath.Parents)
                {
                    i++;
                    if (p is null) continue;
                    console.Append(ConsoleColor.Blue, '+');
                    console.Append(ConsoleColor.Blue, i);
                    if (!p.IsActive) console.Append(ConsoleColor.Red, " × ");
                    else console.Append(ConsoleColor.DarkGray, " | ");
                    console.Append(p.GetName());
                    console.Append(" \t");
                    console.WriteLine(ConsoleColor.DarkGray, p.DepartmentId);
                }
            }
        }

        var children = await childrenTask;
        if (children.Data is null || children.IsError) return resp;
        if (children.Data.Count > 0)
        {
            console.WriteLine();
            console.WriteLine(LarkCliUtils.ItalicText(), "Subordinate Departments");
            WriteDepartments(console, children.Data);
        }

        var employees = await employeesTask;
        if (employees.Data is not null && employees.Data.Count > 0)
        {
            console.WriteLine();
            console.WriteLine(LarkCliUtils.ItalicText(), "Members");
            WriteEmployees(console, employees.Data);
            console.WriteLine(ConsoleColor.DarkGray, "* Includeing only members of this department, excluding ones of its sub-departments.");
        }

        return resp;
    }

    public static async Task<LarkResponsePagingBody> GetSubordinateDepartmentsAsync(LarkApi? larkApi, string id, bool? isActive, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) return new(true, "The department identifier should not be empty.");
        larkApi ??= LarkApi.DefaultInstance;
        var childrenReq = new LarkCompanyDepartmentsSearchRequest()
        {
            ParentDepartmentId = id,
            Fields = departmentFields,
            IsActive = isActive,
        };
        var resp = await larkApi.SearchCompanyDepartmentsAsync(childrenReq, new(50), cancellationToken);
        if (resp is null) return new(true, "No response.");
        if (resp.Data is null || resp.IsError) return resp;
        await LarkApiUtils.LoadAllPagesAsync(resp, 50, larkApi.SearchCompanyDepartmentsAsync, cancellationToken).CountAsync(cancellationToken);
        return resp;
    }

    public static string? GetDepartmentName(JsonObjectNode json)
        => GetName(json, "department_name");

    public static string? GetDepartmentDescription(JsonObjectNode json)
        => GetName(json, "description");

    public static Task<JsonObjectNode?> WriteDepartmentInPropertiesAsync(StyleConsole? console, LarkApi? larkApi, string id, CancellationToken cancellationToken = default)
        => WriteDepartmentInPropertiesAsync(console, larkApi, id, true, cancellationToken);

    public static LarkCompanyDepartmentInfo SimplifyDepartment(JsonObjectNode department, DataCacheCollection<LarkCompanyDepartmentInfo> cache)
    {
        var id = department.TryGetStringTrimmedValue("id", true);
        var parentId = department.TryGetStringTrimmedValue("parent_department_id", true);
        var info = new LarkCompanyDepartmentInfo()
        {
            Id = id,
            IsActive = department.TryGetBooleanValue("active"),
            Info = new()
            {
                Name = GetDepartmentName(department),
                Code = department.TryGetStringValue("code"),
                Description = GetDepartmentDescription(department),
                CostCenterId = department.TryGetStringValue("cost_center_id"),
            },
            Manager = new()
            {
                Id = department.TryGetStringTrimmedValue("manager", true)
            },
            ParentDepartment = new()
            {
                Id = department.TryGetStringTrimmedValue("parent_department_id", true)
            },
            IsRoot = department.TryGetBooleanValue("is_root"),
        };
        if (cache is not null)
        {
            if (id is not null) cache[id] = info;
            if (parentId is not null && cache.TryGet(parentId, out var parent) && parent?.Info?.Name is not null)
                info.ParentDepartment.Name = parent.Info.Name;
        }

        return info;
    }

    public static LarkCompanyDepartmentInfo SimplifyDepartment(JsonObjectNode department)
        => SimplifyDepartment(department, null);

    private static async Task<JsonObjectNode?> WriteDepartmentInPropertiesAsync(StyleConsole? console, LarkApi? larkApi, string id, bool containParentId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        console ??= StyleConsole.Default;
        larkApi ??= LarkApi.DefaultInstance;
        var info = await GetDepartmentAsync(larkApi, id, false, cancellationToken);
        if (info is null)
        {
            LarkCliUtils.WritePropertyLine(console, "ID", id);
            return null;
        }

        LarkCliUtils.WritePropertyLine(console, "ID", info.TryGetStringTrimmedValue("id", true) ?? id);
        LarkCliUtils.WritePropertyLine(console, "Name", GetDepartmentName(info));
        var activeState = info.TryGetBooleanValue("active");
        if (activeState.HasValue) LarkCliUtils.WritePropertyLine(console, "State", activeState.Value ? "Active" : "Inactive");
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Code", info.TryGetStringValue("code"));
        if (info.TryGetBooleanValue("is_root") == true) LarkCliUtils.WritePropertyLine(console, "Level", "Root");
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Description", GetDepartmentDescription(info));
        if (containParentId) LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Parent ID", info.TryGetStringTrimmedValue("parent_department_id", true));
        return info;
    }

    private static void WriteDepartment(StyleConsole console, JsonObjectNode department, bool half)
    {
        if (department is null) return;
        console ??= StyleConsole.Default;
        var name = GetDepartmentName(department);
        console.WriteLine(LarkCliUtils.BoldText(), name ?? "?");
        var id = department.TryGetStringTrimmedValue("id", true);
        console.WriteLine(ConsoleColor.Yellow, id ?? "?");
        console.WriteLine();
        console.WriteLine(LarkCliUtils.ItalicText(), "Properties");
        var desc = GetDepartmentDescription(department)?.Trim();
        if (!string.IsNullOrEmpty(desc))
        {
            if (desc.Length > 20)
            {
                console.WriteLine(desc);
                console.WriteLine();
            }
            else
            {
                LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Description", desc);
            }
        }

        var activeState = department.TryGetBooleanValue("active");
        if (activeState.HasValue) LarkCliUtils.WritePropertyLine(console, "State", activeState.Value ? "Active" : "Inactive");
        LarkCliUtils.WritePropertyLine(console, "Code", department.TryGetStringValue("code"));
        if (department.TryGetBooleanValue("is_root") == true) LarkCliUtils.WritePropertyLine(console, "Level", "Root");

        if (half)
        {
            LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Cost center", department.TryGetStringValue("cost_center_id"));
            return;
        }

        console.WriteLine();
        console.WriteLine(LarkCliUtils.ItalicText(), "Related IDs");
        var parent = department.TryGetStringTrimmedValue("parent_department_id", true);
        LarkCliUtils.WritePropertyLine(console, "Manager", department.TryGetStringValue("manager"));
        LarkCliUtils.WritePropertyLine(console, "Parent", parent);
        LarkCliUtils.WritePropertyLine(console, "Cost center", department.TryGetStringValue("cost_center_id"));
    }
}
