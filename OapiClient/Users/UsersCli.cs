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
}
