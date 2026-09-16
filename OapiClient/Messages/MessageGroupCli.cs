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
            if (select.Value.Contains(' '))
            {
                console.WriteLine(ConsoleColor.Red, "Not found.");
                return;
            }

            var chat = await api.GetMessageHistoryAsync(select.Value, 50, cancellationToken);
            if (chat?.Data is null || chat.IsError)
            {
                console.WriteLine(ConsoleColor.Red, "Not found.");
                return;
            }

            if (chat.Data.Count < 1)
            {
                LarkCliUtils.WriteEmpty(console);
                return;
            }

            WriteLine(console, chat.Data, true);
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

        if (string.IsNullOrWhiteSpace(select.Data.OwnerId))
        {
            LarkCliUtils.WritePropertyLine(console, "Owner", "None");
        }
        else
        {
            var users = await api.GetUserInfoAsync([select.Data.OwnerId], cancellationToken);
            var userName = users.Data?.FirstOrDefault()?.TryGetStringTrimmedValue("name");
            LarkCliUtils.WritePropertyLine(console, "Owner", string.IsNullOrWhiteSpace(userName) ? "?" : userName, select.Data.OwnerId);
        }

        LarkCliUtils.WritePropertyLine(console, "External", select.Data.IsExternal ? "Yes" : "No");
        console.WriteLine();

        var messages = await api.GetMessageHistoryAsync(select.Data.Id, 50, cancellationToken);
        if (messages?.Data is null || messages.IsError || messages.Data.Count < 1) return;
        console.WriteLine(LarkCliUtils.ItalicText(), "Recent history");
        console.WriteLine();
        WriteLine(console, messages.Data, true);
    }

    public static void WriteLine(StyleConsole console, IEnumerable<LarkMessageResponse> col, bool revert = false)
    {
        if (col is null) return;
        console ??= StyleConsole.Default;
        if (revert) col = col.Reverse();
        var sep = string.Concat(Environment.NewLine, "---", Environment.NewLine);
        foreach (var message in col)
        {
            if (message is null) continue;
            var text = message.GetContentString()?.Trim();
            var empty = string.IsNullOrEmpty(text) || text == "---";
            var senderName = message.Sender?.SenderName;
            if (empty && string.IsNullOrEmpty(senderName)) continue;
            console.Append(ConsoleColor.Blue, "· ");
            console.Append(senderName ?? "?");
            console.Append(" \t");
            console.Append(ConsoleColor.DarkGray, message.CreationDate.ToString("f"));
            console.WriteLine();
            if (!empty)
            {
                var offset = text.IndexOf(sep);
                if (offset >= 0) text = text[(offset + sep.Length)..].Trim();
                if (!string.IsNullOrEmpty(text)) console.WriteLine(text);
            }

            console.WriteLine();
        }
    }
}