using LarkSuite;
using LarkSuite.OapiModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text;
using System.Text.Json;
using Trivial.Collection;
using Trivial.CommandLine;
using Trivial.Text;
using Trivial.Web;
using static System.Collections.Specialized.BitVector32;

namespace LarkSuite.CommandLine;

public partial class LarkUsersCommandVerb : BaseCommandVerb
{
    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    public static string Description => "Get employee info and org info.";

    /// <summary>
    /// Additional capabilities.
    /// </summary>
    private readonly Dictionary<string, string> caps = new();

    /// <summary>
    /// Registers the additional command.
    /// </summary>
    /// <param name="key">The command key.</param>
    /// <param name="description">The description.</param>
    protected void Register(string key, string description)
    {
        if (key is null) return;
        key = key.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(key)) return;
        var method = GetType().GetMethod($"Process{key.ToSpecificCaseInvariant(Cases.Capitalize)}Async", [typeof(CancellationToken)]);
        if (method is not null && !method.IsStatic) caps[key] = description;
    }

    /// <inheritdoc />
    protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        console.WriteLine(ConsoleColor.Magenta, Description);
        var verb = Arguments.Verb;
        var command = verb.Count > 0 ? verb[0]?.Trim()?.ToLowerInvariant() : null;

        if (string.IsNullOrWhiteSpace(command))
        {
            var selection = new SelectionData<string>
            {
                new('r', "recent\tGet the recent employees.", "recent"),
                new('e', "employee\tGet details of a specific employee.", "employee"),
                new('d', "department\tGet details of a specific department.", "department"),
            };
            foreach (var item in caps)
            {
                if (string.IsNullOrWhiteSpace(item.Key)) continue;
                selection.Add(new($"{item.Key}\t{item.Value}", item.Key));
            }

            var result = console.Select(selection, LarkCliUtils.GetMenuSelectionOptions());
            command = result.Data ?? result.Value;
        }

        if (string.IsNullOrWhiteSpace(command) || LarkCliUtils.IsToExit(command)) return;
        console.WriteLine();
        if (caps.ContainsKey(command))
        {
            var method = GetType().GetMethod($"Process{command.ToSpecificCaseInvariant(Cases.Capitalize)}Async", [typeof(CancellationToken)]);
            if (method is null || method.IsStatic)
            {
                console.WriteLine("Not supported command.");
                console.WriteLine();
                return;
            }

            var task = method.Invoke(this, [cancellationToken]) as Task;
            if (task is not null) await task;
            return;
        }

        switch (command)
        {
            case "r":
            case "recent":
            case "近期":
                {
                    var today = DateTime.Today;
                    await WriteEmployeesAsync(console, today.Day < 15 ? today.AddMonths(-1) : today, cancellationToken);
                    break;
                }
            case "e":
            case "employee":
            case "员工":
                {
                    console.WriteLine("Please type the employee ID or email:  ");
                    var s = LarkCliUtils.ReadLine(console, "employee");
                    if (string.IsNullOrWhiteSpace(s) || LarkCliUtils.IsToExit(s)) break;
                    await WriteEmployeeAsync(console, s, cancellationToken);
                    break;
                }
            case "d":
            case "department":
            case "dept":
            case "部门":
                {
                    console.WriteLine("Please type the department ID:  ");
                    var s = LarkCliUtils.ReadLine(console, "dept");
                    if (string.IsNullOrWhiteSpace(s) || LarkCliUtils.IsToExit(s)) break;
                    await WriteDepartmentAsync(console, s, cancellationToken);
                    break;
                }
            default:
                {
                    console.WriteLine("Not supported command.");
                    console.WriteLine();
                    break;
                }
        }
    }

    public async Task<JsonObjectNode> GetUserInfoAsync()
    {
        var q = LarkCliUtils.ReadLine(CurrentConsole, "User");
        if (string.IsNullOrEmpty(q)) return [];
        var col = await LarkApi.DefaultInstance.GetUserIdAsync(new LarkUserIdRequestOptions
        {
            Emails = [q],
            Phones = [q],
        });
        var users = col.Data.TryGetObjectListValue("user_list");
        if (users is null) return [];
        var i = 0;
        foreach (var user in users)
        {
            if (user is null) continue;
            i++;
            DefaultConsole.WriteLine(user);
        }

        if (i < 1) DefaultConsole.WriteLine(ConsoleColor.Red, "Empty");
        return col.Data;
    }

    private static string? GetName(JsonObjectNode? json, string? key = null)
    {
        var arr = json?.TryGetObjectListValue(key ?? "name", true);
        if (arr is null) return null;
        string? nameZh = null;
        string? nameEn = null;
        foreach (var info in arr)
        {
            if (info.TryGetStringTrimmedValue("lang") == "zh-CN") nameZh = info.TryGetStringTrimmedValue("value", true);
            else if (info.TryGetStringTrimmedValue("lang") == "en-US") nameEn = info.TryGetStringTrimmedValue("value", true);
        }

        return LarkApiUtils.UseChinese ? (nameZh ?? nameEn) : (nameEn ?? nameZh);
    }
}
