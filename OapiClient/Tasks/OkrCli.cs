using LarkSuite.OapiModels;
using LarkSuite.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.CommandLine;
using Trivial.Text;

namespace LarkSuite.CommandLine;

public class LarkOkrCommandVerb : BaseCommandVerb
{
    protected async override Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        var larkApi = LarkApi.DefaultInstance;
        console.WriteLine("Please type the cycle ID.");
        var id = LarkCliUtils.ReadLine(console, "Okr\\Cycle");
        if (LarkCliUtils.IsToExit(id)) return;
        id = GetCycleId(id);
        if (id is null)
        {
            console.WriteLine(ConsoleColor.Red, "Failed to get the cycle ID.");
            return;
        }

        var okr = larkApi.GetOkrsAsync(id, cancellationToken);
        var col = await LarkCliUtils.WriteLineAsync(console, okr);
        console.Write($"Total objective: ");
        console.Write(ConsoleColor.Green, col.Count);
        console.WriteLine('.');
        if (col.Count < 1) return;
        console.Write("Please type O?KR? to get details: ");
        var line = console.ReadLine();
        if (string.IsNullOrWhiteSpace(line) || LarkCliUtils.IsToExit(line)) return;
        var item = LarkApiUtils.Get(col, line);
        if (item is null) LarkCliUtils.WriteEmpty(console);
        else if (item is LarkOkrObjectiveInfo objectiveInfo) await ProcessAsync(console, larkApi, objectiveInfo, cancellationToken);
        else if (item is LarkOkrKeyResultInfo keyResultInfo) await ProcessAsync(console, larkApi, keyResultInfo, cancellationToken);
    }

    public static async Task<LarkOkrObjectiveItem?> ProcessAsync(StyleConsole console, LarkApi larkApi, LarkOkrObjectiveInfo info, CancellationToken cancellationToken = default)
    {
        console ??= StyleConsole.Default;
        larkApi ??= LarkApi.DefaultInstance;
        await LarkCliUtils.WriteLineAsync(console, info, larkApi, cancellationToken);
        console.WriteLine();
        var id = info.Id;
        var item = await larkApi.GetOkrObjectiveAsync(info);
        if (item is not null)
        {
            console.WriteLine(LarkCliUtils.ItalicText(), "JSON");
            console.WriteLine(JsonObjectNode.ConvertFrom(item));
            console.WriteLine();
            if (!string.IsNullOrWhiteSpace(item.Id)) id = item.Id;
        }

        var progress = await larkApi.GetOkrKeyResultProgressAsync(id, new(50), cancellationToken);
        WriteLine(console, progress);
        return item;
    }

    public static async Task<LarkOkrKeyResultItem?> ProcessAsync(StyleConsole console, LarkApi larkApi, LarkOkrKeyResultInfo info, CancellationToken cancellationToken = default)
    {
        console ??= StyleConsole.Default;
        larkApi ??= LarkApi.DefaultInstance;
        await LarkCliUtils.WriteLineAsync(console, info, larkApi, cancellationToken);
        console.WriteLine();
        var id = info.Id;
        var item = await larkApi.GetOkrKeyResultAsync(info);
        if (item is not null)
        {
            console.WriteLine(LarkCliUtils.ItalicText(), "JSON");
            console.WriteLine(JsonObjectNode.ConvertFrom(item));
            console.WriteLine();
            if (!string.IsNullOrWhiteSpace(item.Id)) id = item.Id;
        }

        var progress = await larkApi.GetOkrKeyResultProgressAsync(id, new(50), cancellationToken);
        WriteLine(console, progress);
        return item;
    }

    public static string? GetCycleId(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (!s.Contains("://")) return s;
        var q = Trivial.Net.QueryData.Parse(s);
        s = q.GetFirstValue("okrId");
        if (string.IsNullOrWhiteSpace(s)) return null;
        return s;
    }

    private static void WriteLine(StyleConsole console, LarkResponsePagingBody<LarkOkrProgressItem> progress)
    {
        if (progress?.Data is null || progress.IsError || progress.Data.Count < 1) return;
        console.WriteLine(LarkCliUtils.ItalicText(), "Progress");
        foreach (var item in progress.Data)
        {
            if (item is null) continue;
            console.Write(ConsoleColor.Blue, "· ");
            console.Write(item.Progress?.Value.ToString("P0") ?? "-");
            console.Write(" \t");
            console.WriteLine(ConsoleColor.DarkGray, item.LastModificationDate.ToShortDateString());
        }
    }
}

public static partial class LarkCliUtils
{
    public static async Task<List<LarkOkrObjectiveInfo>> WriteLineAsync(this StyleConsole console, IAsyncEnumerable<LarkOkrObjectiveInfo> objectives)
    {
        console ??= StyleConsole.Default;
        if (objectives is null) return [];
        var col = new List<LarkOkrObjectiveInfo>();
        await foreach (var objective in objectives)
        {
            if (objective?.Text is null) continue;
            col.Add(objective);
            WriteLine(console, objective, col.Count);
            console.WriteLine();
        }

        return col;
    }

    public static int WriteLine(this StyleConsole console, IEnumerable<LarkOkrObjectiveInfo> objectives)
    {
        console ??= StyleConsole.Default;
        if (objectives is null) return 0;
        var i = 0;
        foreach (var objective in objectives)
        {
            if (objective?.Text is null && string.IsNullOrWhiteSpace(objective?.Id)) continue;
            i++;
            WriteLine(console, objective, i);
            console.WriteLine();
        }

        return i;
    }

    public static void WriteLine(this StyleConsole console, LarkOkrObjectiveInfo objective, int i)
    {
        console ??= StyleConsole.Default;
        if (objective?.Text is null) return;
        console.Write(ConsoleColor.Blue, $"O{i}. ");
        console.WriteLine(BoldText(), string.Join(Environment.NewLine, objective.Text));
        var keyResults = objective.KeyResults;
        if (keyResults is null) return;
        var j = 0;
        foreach (var keyResult in keyResults)
        {
            if (keyResult?.Text == null && string.IsNullOrWhiteSpace(keyResult?.Id)) continue;
            j++;
            console.WriteLine();
            WriteLine(console, keyResult, j);
        }
    }

    public static void WriteLine(this StyleConsole console, LarkOkrKeyResultInfo objective, int i)
    {
        console ??= StyleConsole.Default;
        if (objective?.Text is null) return;
        console.Write(ConsoleColor.Blue, $"KR{i}. ");
        console.WriteLine(string.Join(Environment.NewLine, objective.Text));
    }

    public static void WriteLine(this StyleConsole console, BaseLarkOkrItemInfo info)
    {
        if (info is null) return;
        WriteLineWithoutUsers(console, info);
        var userIds = info.MentionedUserIds;
        WritePropertyLine(console, "Users mentioned", (userIds is null ? 0 : userIds.Count).ToString());
    }

    public static async Task WriteLineAsync(this StyleConsole console, BaseLarkOkrItemInfo info, LarkApi? larkApi = null, CancellationToken cancellationToken = default)
    {
        if (info is null) return;
        WriteLineWithoutUsers(console, info);
        var userIds = info.MentionedUserIds;
        if (userIds is null || userIds.Count < 1)
        {
            WritePropertyLine(console, "Users mentioned", "0  (Empty)");
            return;
        }

        larkApi ??= LarkApi.DefaultInstance;
        var response = await larkApi.GetUserInfoAsync(userIds, cancellationToken);
        if (response?.Data is null || response.IsError)
        {
            WritePropertyLine(console, "Users mentioned", userIds.Count.ToString());
            return;
        }

        console.WriteLine();
        console.WriteLine(ItalicText(), "Users mentioned");
        foreach (var user in response.Data)
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

    private static void WriteLineWithoutUsers(this StyleConsole console, BaseLarkOkrItemInfo info)
    {
        if (info is null) return;
        console ??= StyleConsole.Default;
        if (!string.IsNullOrWhiteSpace(info.Id)) console.WriteLine(ConsoleColor.Yellow, info.Id);
        if (info.Text is not null)
        {
            console.WriteLine(string.Join(Environment.NewLine, info.Text));
            console.WriteLine();
        }

        if (info.Weight > 0 && !double.IsNaN(info.Weight)) WritePropertyLine(console, "Weight", info.Weight.ToString("P0"));
    }
}