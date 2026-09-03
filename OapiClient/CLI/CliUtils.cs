using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Linq;
using Trivial.Collection;
using Trivial.CommandLine;
using Trivial.Net;
using Trivial.Text;

namespace LarkSuite.CommandLine;

public static partial class LarkCliUtils
{
    public static SelectionConsoleOptions MenuSelectionOptions { get; set; }
    public static SelectionConsoleOptions ItemSelectionOptions { get; set; }

    public static async Task<LarkResponsePagingBody<TItem>> WritePagesAsync<TItem>(
        StyleConsole console,
        Task<LarkResponsePagingBody<TItem>> firstPage,
        Func<LarkResponsePagingBody<TItem>, int?, CancellationToken, Task<IReadOnlyList<TItem>>> restPages,
        Action<StyleConsole, IReadOnlyList<TItem>>? writeLine = null,
        int? restPageSize = null,
        CancellationToken cancellationToken = default)
    {
        var resp = await firstPage;
        if (WriteEmpty(console, resp)) return new(true);
        writeLine ??= (console, list) =>
        {
            foreach (var item in list)
            {
                if (item is null) continue;
                console.WriteLine(item.ToString());
            }
        };
        writeLine(console, resp.Data);
        if (!resp.HasNextPage) return resp;
        var loadAll = false;
        while (resp.HasNextPage)
        {
            if (!loadAll)
            {
                console.Write("Load next page? [Y]es, [N]o or [A]ll. ");
                var c = DefaultConsole.ReadKey();
                if (c.Key == ConsoleKey.N || c.Key == ConsoleKey.Q || c.Key == ConsoleKey.Escape || c.Key == ConsoleKey.BrowserStop)
                {
                    console.WriteLine();
                    console.WriteLine("Aborted to list rest.");
                    return resp;
                }
                if (c.Key == ConsoleKey.Y || c.Key == ConsoleKey.C || c.Key == ConsoleKey.Enter || c.Key == ConsoleKey.Spacebar)
                {
                    console.Clear(StyleConsole.RelativeAreas.Line);
                    console.BackspaceToBeginning();
                }
                else if (c.Key == ConsoleKey.A)
                {
                    console.Clear(StyleConsole.RelativeAreas.Line);
                    console.BackspaceToBeginning();
                    loadAll = true;
                }
                else
                {
                    console.Write(" [Y/N/A] ");
                    c = DefaultConsole.ReadKey();
                    if (c.Key == ConsoleKey.N || c.Key == ConsoleKey.Q || c.Key == ConsoleKey.Escape || c.Key == ConsoleKey.BrowserStop)
                    {
                        console.WriteLine();
                        console.WriteLine("Aborted to list rest.");
                        return resp;
                    }
                    if (c.Key == ConsoleKey.Y || c.Key == ConsoleKey.C || c.Key == ConsoleKey.Enter || c.Key == ConsoleKey.Spacebar)
                    {
                        console.Clear(StyleConsole.RelativeAreas.Line);
                        console.BackspaceToBeginning();
                    }
                    else if (c.Key == ConsoleKey.A)
                    {
                        console.Clear(StyleConsole.RelativeAreas.Line);
                        console.BackspaceToBeginning();
                        loadAll = true;
                    }
                    else
                    {
                        console.WriteLine();
                        console.WriteLine("Aborted to list rest.");
                        return resp;
                    }
                }
            }

            var items = await restPages(resp, restPageSize, cancellationToken);
            if (items.Count < 1) break;
            writeLine(console, items);
        }

        return resp;
    }

    public static Task<LarkResponsePagingBody<TItem>> WritePagesAsync<TItem>(
        StyleConsole console,
        Func<CancellationToken, Task<LarkResponsePagingBody<TItem>>> firstPage,
        Func<LarkResponsePagingBody<TItem>, int?, CancellationToken, Task<IReadOnlyList<TItem>>> restPages,
        Action<StyleConsole, IReadOnlyList<TItem>>? writeLine = null,
        int? restPageSize = null,
        CancellationToken cancellationToken = default)
        => WritePagesAsync(console, firstPage(cancellationToken), restPages, writeLine, restPageSize, cancellationToken);

    public static Task<LarkResponsePagingBody<TItem>> WritePagesAsync<TItem>(
        StyleConsole console,
        Func<string, CancellationToken, Task<LarkResponsePagingBody<TItem>>> firstPage,
        Func<LarkResponsePagingBody<TItem>, int?, CancellationToken, Task<IReadOnlyList<TItem>>> restPages,
        string arg,
        Action<StyleConsole, IReadOnlyList<TItem>>? writeLine = null,
        int? restPageSize = null,
        CancellationToken cancellationToken = default)
        => WritePagesAsync(console, firstPage(arg, cancellationToken), (response, pageSize, cancellationToken) =>
        {
            return restPages(response, restPageSize, cancellationToken);
        }, writeLine, restPageSize, cancellationToken);

    public static Task<LarkResponsePagingBody<TItem>> WritePagesAsync<TRequestOptions, TItem>(
        StyleConsole console,
        TRequestOptions? options,
        Func<TRequestOptions?, LarkPageTokenInfo?, CancellationToken, Task<LarkResponsePagingBody<TItem>>> firstPage,
        Func<LarkResponsePagingBody<TItem>, int?, CancellationToken, Task<IReadOnlyList<TItem>>> restPages,
        Action<StyleConsole, IReadOnlyList<TItem>>? writeLine = null,
        int? firstPageSize = null,
        int? restPageSize = null,
        CancellationToken cancellationToken = default)
    where TRequestOptions : BaseQueryRequestInfo
    {
        var resp = firstPage(options, firstPageSize.HasValue ? new(firstPageSize.Value) : null, cancellationToken);
        return WritePagesAsync(console, resp, restPages, writeLine, restPageSize, cancellationToken);
    }

    public static bool IsToExit(string? command)
    {
        command = command?.Trim()?.ToLowerInvariant();
        if (string.IsNullOrEmpty(command)) return false;
        return command == "exit" || command == "quit" || command == "close" || command == "esc" || command == "bye" || command == "goodbye" || command == "tuichu" || command == "退出" || command == "关闭" || command == "再见" || command == "结束";
    }

    public static async Task WriteChatMessagesAsync(StyleConsole console, string chatId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(chatId)) return;
        var backward = await LarkApi.DefaultInstance.GetMessageHistoryAsync(chatId, 50, cancellationToken);
        if (backward?.Data is null || backward.IsError) return;
        var col = backward.Data.Reverse();
        var yesterday = DateTime.Today.AddDays(-1);
        console ??= StyleConsole.Default;
        foreach (var item in col)
        {
            WriteChatMessage(console, item);
        }
    }

    public static void WriteChatMessage(StyleConsole console, LarkMessageResponse item)
    {
        if (string.IsNullOrWhiteSpace(item?.MessageType) || item.IsDeleted) return;
        var text = item.GetContentString();
        if (text is null) return;
        var author = item.Sender.SenderName ?? string.Concat(item.Sender.SenderType, ' ', item.Sender.Id);
        console.Write(ConsoleColor.Blue, "· ");
        console.Write(BoldText(), author);
        console.Write(" \t");
        console.WriteLine(item.CreationDate.ToString());
        console.WriteLine();
        console.WriteLine(text);
        console.WriteLine();
    }

    internal static string? GetName(JsonObjectNode? json)
        => LarkApiUtils.GetName(json);

    internal static string? ReadLine(StyleConsole console, string prefix, ConsoleColor? foreground = null)
    {
        return (console ?? StyleConsole.Default).ReadLine(new ReadLineConsoleOptions
        {
            Prefix = string.Concat(prefix, '>'),
            PrefixForegroundColor = foreground,
            AgainIfEmpty = true,
            NeedTrim = true,
        });
    }

    internal static string ReadLine(StyleConsole console, string prefix, string fallback, ConsoleColor? foreground = null)
    {
        console ??= StyleConsole.Default;
        var s = console.ReadLine(new ReadLineConsoleOptions
        {
            Prefix = string.Concat(prefix, '>'),
            PrefixForegroundColor = foreground,
            NeedTrim = true,
        });
        if (!string.IsNullOrEmpty(s)) return s;
        console.Write("Use default value ");
        console.WriteLine(ConsoleColor.Yellow, fallback);
        return fallback;
    }

    internal static string ReadLine(StyleConsole console, char prefix, string fallback, ConsoleColor? foreground = null)
    {
        console ??= StyleConsole.Default;
        var s = console.ReadLine(new ReadLineConsoleOptions
        {
            Prefix = string.Concat(prefix, ' ', '>'),
            PrefixForegroundColor = foreground,
            NeedTrim = true,
        });
        if (!string.IsNullOrEmpty(s)) return s;
        console.Write(ConsoleColor.Green, "Use default value ");
        console.WriteLine(fallback);
        return fallback;
    }

    internal static string? ReadId(StyleConsole console, string prefix, IList<string> ids)
    {
        console ??= StyleConsole.Default;
        var id = ReadLine(console, prefix)!;
        if (string.IsNullOrEmpty(id)) return null;
        if (ids.Contains(id)) return id;
        if (int.TryParse(id, out var index) && index > 0 && index <= ids.Count)
            return ids[index - 1];
        return id;
    }

    internal static string? ReadId(StyleConsole console, string prefix, IList<SelectionItem<string>> ids)
    {
        console ??= StyleConsole.Default;
        var id = ReadLine(console, prefix)!;
        if (string.IsNullOrEmpty(id)) return null;
        foreach (var item in ids)
        {
            if (item?.Data == id) return id;
        }

        if (int.TryParse(id, out var index) && index > 0 && index <= ids.Count)
            return ids[index - 1]?.Data;
        return id;
    }

    internal static void WritePropertyLine(StyleConsole console, string label, string? value)
    {
        console ??= StyleConsole.Default;
        var style = new ConsoleTextStyle(Color.FromArgb(0xCE, 0x91, 0x78), ConsoleColor.Green, null, null);
        if (value is null)
        {
            console.WriteLine(style, label);
            return;
        }

        console.Write(style, label);
        console.Write(label.Length < 8 ? "\t\t" : " \t");
        console.WriteLine(value);
    }

    internal static void WritePropertyLineIfNotEmpty(StyleConsole console, string label, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value)) WritePropertyLine(console, label, value);
    }

    internal static void WriteEmpty(StyleConsole console)
        => (console ?? StyleConsole.Default).WriteLine(ConsoleColor.Red, "Empty");

    internal static bool WriteEmpty(StyleConsole console, LarkResponsePagingBody resp)
    {
        if (resp is not null && !resp.IsError && resp.Count > 0) return false;
        console ??= StyleConsole.Default;
        console.WriteLine(ConsoleColor.Red, "Empty.");
        if (!resp.IsMessageEmpty()) console.WriteLine(resp.Message);
        return true;
    }

    internal static void WriteOrderedLine(StyleConsole console, IList<SelectionItem<string>> selection, bool revert = false)
    {
        for (var i = 0; i < selection.Count; i++)
        {
            var item = selection[i];
            WriteOrderedLine(console, i, item.Data, item.Title, revert);
        }
    }

    internal static void WriteOrderedLine(StyleConsole console, int i, string code, string? desc, bool revert = false)
    {
        console ??= StyleConsole.Default;
        console.Write(ConsoleColor.Blue, i + 1);
        console.Write(ConsoleColor.Blue, i < 9 ? ".  " : ". ");
        if (revert)
        {
            console.Write(desc);
            console.Write(" \t");
            console.WriteLine(ConsoleColor.DarkGray, code);
        }
        else
        {
            console.Write(ConsoleColor.Yellow, code);
            console.Write(" \t");
            console.WriteLine(desc);
        }
    }

    internal static ConsoleTextStyle BoldText(bool blackBackground = false)
        => blackBackground
        ? new()
        {
            BackgroundRgbColor = Color.FromArgb(12, 12, 12),
            BackgroundConsoleColor = ConsoleColor.Black,
            Bold = true,
        }
        : new()
        {
            Bold = true,
        };

    internal static ConsoleTextStyle ItalicText(bool blackBackground = false)
        => new()
        {
            Italic = true,
        };

    internal static ConsoleTextStyle BoldText(ConsoleColor foreground, int r, int g, int b, bool blackBackground = false)
        => blackBackground
        ? new(Color.FromArgb(r, g, b), foreground, Color.FromArgb(8, 8, 8), ConsoleColor.Black)
        {
            Bold = true,
        }
        : new(foreground)
        {
            ForegroundRgbColor = Color.FromArgb(r, g, b),
            Bold = true,
        };

    internal static string ToPrefix(string parent, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return parent;
        if (name.Length < 21) return string.Concat(parent, name);
        return string.Concat(parent, name[0..19], "…");
    }

    internal static SelectionConsoleOptions GetMenuSelectionOptions()
        => MenuSelectionOptions ?? GetDefaultSelectionOptions();

    internal static SelectionConsoleOptions GetItemSelectionOptions()
        => ItemSelectionOptions ?? GetDefaultSelectionOptions();

    private static SelectionConsoleOptions GetDefaultSelectionOptions()
        => new()
        {
            Prefix = "· ",
            SelectedPrefix = "→ ",
            SelectedForegroundConsoleColor = ConsoleColor.Cyan,
            SelectedForegroundRgbColor = Color.FromArgb(0x3d, 0xd4, 0xb9),
            SelectedBackgroundConsoleColor = null,
            SelectedBackgroundRgbColor = null,
            MaxRow = 20,
        };
}
