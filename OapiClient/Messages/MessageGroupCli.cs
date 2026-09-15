using LarkSuite.OapiModels;
using LarkSuite.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Collection;
using Trivial.CommandLine;
using Trivial.Text;

namespace LarkSuite.CommandLine;

/// <summary>
/// The message group command verb.
/// </summary>
public class LarkMessageGroupCommandVerb : BaseCommandVerb
{
    /// <summary>
    /// Gets the command description.
    /// </summary>
    public static string Description => "Get message group joined";

    /// <inherticdot />
    protected async override Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        var api = LarkApi.DefaultInstance;
        var console = CurrentConsole;
        var items = await api.ListMessageGroupAsync(new()
        {
            SortType = "ByActiveTimeDesc",
        }, new(100), cancellationToken);
        if (LarkCliUtils.WriteEmpty(console, items)) return;
        var selection = new SelectionData<BaseLarkMessageGroupInfo>();
        foreach (var item in items.Data)
        {
            selection.Add(new(item.Name, item));
        }

        var select = console.Select(selection);
        console.WriteLine();
        if (select.Data is null)
        {
            if (string.IsNullOrWhiteSpace(select.Value) || LarkCliUtils.IsToExit(select.Value)) return;
            console.WriteLine(ConsoleColor.Red, "Not found.");
            return;
        }

        console.WriteLine(LarkCliUtils.BoldText(), select.Data.Name ?? "?");
        console.WriteLine(ConsoleColor.Yellow, select.Data.Id ?? "?");
        console.WriteLine();
        var desc = select.Data.Description?.Trim();
        if (!string.IsNullOrWhiteSpace(desc))
        {
            if (desc.Length > 20)
            {
                console.WriteLine(select.Data.Description);
                console.WriteLine();
            }
            else
            {
                LarkCliUtils.WritePropertyLine(console, "Description", desc);
            }
        }

        LarkCliUtils.WritePropertyLine(console, "External", select.Data.IsExternal ? "Yes" : "No");
        if (string.IsNullOrWhiteSpace(select.Data.OwnerId))
        {
            LarkCliUtils.WritePropertyLine(console, "Owner", "None");
            return;
        }

        var users = await api.GetUserInfoAsync([select.Data.OwnerId], cancellationToken);
        var userName = users.Data?.FirstOrDefault()?.TryGetStringTrimmedValue("name");
        LarkCliUtils.WritePropertyLine(console, "Owner", string.IsNullOrWhiteSpace(userName) ? "?" : userName, select.Data.OwnerId);
    }
}