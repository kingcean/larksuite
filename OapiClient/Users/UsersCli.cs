using LarkSuite;
using LarkSuite.OapiModels;
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

public class LarkUsersCommandVerb : BaseCommandVerb
{
    public static string Description => "Get user info and org info.";

    protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        await GetUserInfoAsync();
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

    public static void WriteLine(StyleConsole console, IEnumerable<JsonObjectNode> col)
    {
        console ??= StyleConsole.Default;
        foreach (var user in col)
        {
            console.Write(ConsoleColor.Blue, "· ");
            var nickname = user.TryGetStringValue("nickname");
            var name = user.TryGetStringValue("name");
            var enName = user.TryGetStringValue("en_name");
            if (string.IsNullOrWhiteSpace(nickname) || nickname == name)
            {
                console.Write(name);
                if (!string.IsNullOrWhiteSpace(enName) && enName != name)
                    console.Write(ConsoleColor.DarkGray, $"  ({enName})");
            }
            else
            {
                console.Write(nickname);
                if (!string.IsNullOrWhiteSpace(enName) && enName != name && enName != nickname)
                    console.Write(ConsoleColor.DarkGray, $"  ({name} | {nickname})");
                else
                    console.Write(ConsoleColor.DarkGray, $"  ({name})");
            }

            console.Write(" \t");
            console.WriteLine(ConsoleColor.DarkGray, user.TryGetStringTrimmedValue("open_id", true) ?? user.TryGetStringTrimmedValue("user_id", true));
        }
    }

    public static void WriteEmployee(StyleConsole console, JsonObjectNode employee)
    {
        if (employee is null) return;
        console ??= StyleConsole.Default;
        var info = employee.TryGetObjectValue("person_info");
        if (info is null) return;
        console.WriteLine(LarkCliUtils.BoldText(), employee.TryGetStringTrimmedValue("preferred_name", true)
            ?? info.TryGetStringTrimmedValue("preferred_local_full_name", true)
            ?? info.TryGetStringTrimmedValue("legal_name", true)
            ?? info.TryGetStringTrimmedValue("preferred_english_full_name", true)
            ?? "?");
        var jobTitle = employee.TryGetObjectValue("job")?.TryGetObjectListValue("name", true);
        if (jobTitle is not null)
        {
            string? nameZh = null;
            string? nameEn = null;
            foreach (var jobInfo in jobTitle)
            {
                if (jobInfo.TryGetStringTrimmedValue("lang") == "zh-CN") nameZh = jobInfo.TryGetStringTrimmedValue("value", true);
                else if (jobInfo.TryGetStringTrimmedValue("lang") == "en-US") nameEn = jobInfo.TryGetStringTrimmedValue("value", true);
            }

            var name = nameZh ?? nameEn;
            if (name is not null) console.WriteLine(name);
        }

        console.WriteLine();
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Email", info.TryGetStringValue("email_address"));
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Phone", info.TryGetStringValue("phone_number"));
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Birthday", info.TryGetStringValue("date_of_birth"));
        LarkCliUtils.WritePropertyLineIfNotEmpty(console, "Gender", info.TryGetObjectValue("gender")?.TryGetStringValue("enum_name")?.ToSpecificCase(Cases.Capitalize));
    }
}
