using LarkSuite;
using LarkSuite.Docs;
using LarkSuite.OapiModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Collection;
using Trivial.CommandLine;
using Trivial.Tasks;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.CommandLine;

public class LarkDocsCommandVerb : BaseCommandVerb
{
    public static string Description => "Access Lark Docs and Base tables";

    protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        var verb = Arguments.Verb;
        var s = verb.Count > 0 ? verb[0]?.Trim()?.ToLowerInvariant() : null;
        while (true)
        {
            if (string.IsNullOrEmpty(s))
            {
                LarkCliUtils.WriteOrderedLine(console,
                [
                    new("Get the information of the specific space.", "space"),
                    new("Get the text content of the specific doc ID.", "open"),
                    new("Search docs.", "search"),
                ]);
                s = LarkCliUtils.ReadLine(console, "Docs")?.Trim()?.ToLowerInvariant();
            }

            console.WriteLine();
            if (string.IsNullOrEmpty(s) || s == ".." || s == "exit" || s == "quit") return;
            switch (s)
            {
                case "space":
                case "1":
                    await ShowSpaceAsync(cancellationToken);
                    break;
                case "doc":
                case "open":
                case "2":
                    await ShowDocAsync(cancellationToken);
                    break;
                case "search":
                case "3":
                    await SearchWikiAsync(cancellationToken);
                    break;
                default:
                    console.WriteLine(ConsoleColor.Red, "Not supported.");
                    console.WriteLine();
                    break;
            }

            s = null;
        }
    }

    public async Task ShowDocAsync(CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        var lark = LarkApi.DefaultInstance;

        //var files = await lark.ListDocsDriveFilesAsync(cancellationToken);
        //if (files?.Data is not null && !files.IsError && files.Count > 0)
        //{
        //    var list = new List<SelectionItem<string>>();
        //    foreach (var file in files.Data)
        //    {
        //        if (string.IsNullOrWhiteSpace(file?.Token)) continue;
        //        list.Add(new($"{file.Name ?? "?"} \t{file.NodeType}", file.Token));
        //    }

        //    LarkCliUtils.WriteOrderedLine(console, list);
        //}

        var id = LarkCliUtils.ReadLine(console, "Docs\\Doc");
        cancellationToken.ThrowIfCancellationRequested();
        await ShowDocAsync(id, cancellationToken);
    }

    public async Task ShowDocAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token)) return;
        var console = CurrentConsole;
        var lark = LarkApi.DefaultInstance;
        var node = await lark.GetWikiNodeAsync(token, cancellationToken);
        if (string.IsNullOrWhiteSpace(node?.Data?.NodeToken))
        {
            await ResolveDocAsync(token, cancellationToken);
            return;
        }

        console.WriteLine(node.Data);
        console.WriteLine();
        await ShowDocAsync(node.Data);
        var space = string.IsNullOrWhiteSpace(node.Data.SpaceId) ? null : await lark.GetWikiSpaceInfoAsync(node.Data.SpaceId, cancellationToken);
        await ShowSpaceAsync([node.Data], space?.Data, cancellationToken);
    }

    public Task ShowSpaceAsync(CancellationToken cancellationToken = default)
        => ShowSpaceAsync(null, null, cancellationToken);

    public async Task ShowSpaceAsync(List<LarkDocsNodeInfo>? path, LarkWikiSpaceInfo? space, CancellationToken cancellationToken = default)
    {
        path ??= new();
        var console = CurrentConsole;
        var lark = LarkApi.DefaultInstance;
        while (true)
        {
            if (string.IsNullOrWhiteSpace(space?.Id))
            {
                var spaces = await lark.GetWikiSpacesAsync(cancellationToken);
                if (LarkCliUtils.WriteEmpty(console, spaces)) return;
                path.Clear();
                var list = spaces.Data.ToSelectionStringItems().ToList();
                LarkCliUtils.WriteOrderedLine(console, list);
                console.WriteLine();
                console.WriteLine("Please type the index or the space ID.");
                var spaceId = LarkCliUtils.ReadId(console, "Docs\\Space", list)!;
                if (LarkCliUtils.IsToExit(spaceId) || spaceId == "..") return;
                if (spaceId == "." || spaceId == "~") continue;
                cancellationToken.ThrowIfCancellationRequested();
                var info = await lark.GetWikiSpaceInfoAsync(spaceId, cancellationToken);
                if (string.IsNullOrWhiteSpace(info?.Data?.Id))
                {
                    console.WriteLine(ConsoleColor.Red, "Not found.");
                    spaceId = LarkCliUtils.ReadId(console, "Docs\\Space", list)!;
                    if (LarkCliUtils.IsToExit(spaceId)) return;
                    cancellationToken.ThrowIfCancellationRequested();
                    info = await lark.GetWikiSpaceInfoAsync(spaceId, cancellationToken);
                    if (info?.Data is null)
                    {
                        console.WriteLine(ConsoleColor.Red, "Not found.");
                        continue;
                    }
                }

                space = info.Data;
            }
            else
            {
                var node = path.LastOrDefault();
                LarkResponsePagingBody<LarkDocsNodeInfo> nodes;
                string prefix;
                if (string.IsNullOrWhiteSpace(node?.NodeToken))
                {
                    console.WriteLine(space);
                    nodes = await lark.GetWikiSpaceNodesAsync(space.Id, cancellationToken);
                    prefix = string.IsNullOrWhiteSpace(space.Name) ? "Docs\\Doc" : LarkCliUtils.ToPrefix("Docs\\Space\\", space.Name);
                }
                else
                {
                    console.WriteLine(node);
                    prefix = LarkCliUtils.ToPrefix("Docs\\Doc\\", node.Name);
                    nodes = await lark.GetWikiSpaceNodesAsync(node, cancellationToken);
                }

                console.WriteLine();
                var items = nodes.Data.ToSelectionStringItems().ToList();
                if (items.Count < 1)
                {
                    console.WriteLine("No child node");
                }
                else
                {
                    console.WriteLine(LarkCliUtils.ItalicText(), "Child nodes");
                    LarkCliUtils.WriteOrderedLine(console, items, true);
                    console.WriteLine();
                    console.WriteLine("Please type the index or node token to get the child details;");
                    if (path.Count > 0)
                    {
                        console.Write("Or, type: ");
                        console.Write(ConsoleColor.Yellow, ".");
                        console.Write(" to read content; ");
                        console.Write(ConsoleColor.Yellow, "..");
                        console.WriteLine(" to turn back parent node.");
                    }
                }

                var sub = LarkCliUtils.ReadId(console, prefix, items);
                if (LarkCliUtils.IsToExit(sub)) return;
                cancellationToken.ThrowIfCancellationRequested();
                switch (sub)
                {
                    case ".":
                        await ShowDocAsync(node, cancellationToken);
                        break;
                    case "..":
                        if (node is null) break;
                        path.Remove(node);
                        if (path.Count > 0 || string.IsNullOrWhiteSpace(node.ParentNodeToken) || node.ParentNodeToken == node.DocToken) break;
                        var parent = await lark.GetWikiNodeAsync(node.ParentNodeToken, cancellationToken);
                        if (!string.IsNullOrWhiteSpace(parent?.Data?.NodeToken)) path.Add(parent.Data);
                        break;
                    default:
                        var select = await lark.GetWikiNodeAsync(sub!, cancellationToken);
                        if (string.IsNullOrWhiteSpace(select?.Data?.NodeToken))
                        {
                            console.WriteLine(ConsoleColor.Red, "Not found");
                            break;
                        }

                        path.Add(select.Data);
                        break;
                }
            }

            console.WriteLine();
        }
    }

    public async Task ShowDocAsync(LarkDocsNodeInfo node, CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        if (string.IsNullOrWhiteSpace(node?.DocToken))
        {
            console.Write(ConsoleColor.Red, "Error");
            console.WriteLine(node is null ? " \tNo node given." : "Unknown type.");
            return;
        }

        var pressKey = true;
        switch (node.DocType)
        {
            case "doc":
            case "docs":
            case "docx":
                await ResolveDocAsync(node.DocToken, cancellationToken);
                break;
            case "bitable":
                await ResolveBaseTable(node.DocToken, cancellationToken);
                break;
            case "file":
                await ResolveFileAsync(node.NodeToken, node.Name, cancellationToken);
                break;
            default:
                pressKey = false;
                LarkCliUtils.WritePropertyLine(console, "Doc Type", node.DocType);
                break;
        }

        if (!pressKey) return;
        console.Write(ConsoleColor.DarkGray, "--- * THE END * ---");
        console.Write("  (Press any key to continue...)  ");
        console.ReadKey(true);
        console.Clear(StyleConsole.RelativeAreas.Line);
        console.BackspaceToBeginning();
        console.WriteLine();
    }

    public Task<JsonObjectNode?> SearchWikiAsync(CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        console.WriteLine("Please type keyword to search.");
        var q = LarkCliUtils.ReadLine(console, string.Empty)!;
        return SearchWikiAsync(q, cancellationToken);
    }

    public async Task<JsonObjectNode?> SearchWikiAsync(string q, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(q)) return null;
        cancellationToken.ThrowIfCancellationRequested();
        var json = await LarkApi.DefaultInstance.SearchWikiAsync(q, cancellationToken);
        CurrentConsole.WriteLine(json.Data);
        return json.Data;
    }

    public async Task<LarkResponsePagingBody<LarkContentBlock>?> ResolveDocAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token)) return null;
        var console = CurrentConsole;
        var larkApi = LarkApi.DefaultInstance;
        console.WriteLine();
        var task = larkApi.GetDocsBlocksAsync(token, cancellationToken);
        var writer = new InternalLarkDocsContentCliWriter();
        var blocks = await LarkCliUtils.WritePagesAsync(
            console,
            larkApi.GetDocsBlocksAsync,
            larkApi.GetDocsBlocksAsync,
            token,
            writer.WriteLine,
            null,
            cancellationToken);
        console.WriteLine();
        return blocks;
    }

    public async Task<IReadOnlyList<LarkDocsBaseTableTableInfo>?> ResolveBaseTable(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token)) return null;
        var console = CurrentConsole;
        var lark = LarkApi.DefaultInstance;
        var t = await lark.GetBaseTableAsync(token, cancellationToken);
        console.WriteLine(t.Data.Name);
        console.WriteLine(ConsoleColor.Yellow, t.Data.Token);
        console.WriteLine();
        console.WriteLine(LarkCliUtils.ItalicText(), "Tables");
        var tables = await lark.ListBaseTableTablesAsync(token, new(50), cancellationToken);
        if (LarkCliUtils.WriteEmpty(console, tables)) return null;
        var col = tables.Data.ToSelectionStringItems().ToList();
        if (col.Count < 1)
        {
            LarkCliUtils.WriteEmpty(console);
            return tables.Data;
        }

        LarkCliUtils.WriteOrderedLine(console, col);
        var id = LarkCliUtils.ReadId(console, $"Docs\\Doc\\{t.Data.Name}", col);
        if (LarkCliUtils.IsToExit(id) || string.IsNullOrWhiteSpace(id)) return tables.Data;

        var fields = await lark.ListBaseTableFieldsAsync(token, id, true, cancellationToken);
        if (fields?.Data is null || fields.IsError || fields.Data.Count < 1)
        {
            // No additional views.
        }
        else
        {
            console.WriteLine(LarkCliUtils.ItalicText(), "Fields");
            foreach (var field in fields.Data)
            {
                console.Write(ConsoleColor.Blue, "· ");
                console.Write(field.Name ?? "?");
                console.Write(" \t");
                console.WriteLine(field.FieldType.ToString());
            }

            console.WriteLine();
        }

        var views = await lark.ListBaseTableViewsAsync(token, id, new LarkPageTokenInfo(50), cancellationToken);
        if (views?.Data is null || views.IsError || views.Data.Count < 2)
        {
            // No additional views.
        }
        else
        {
            console.WriteLine(LarkCliUtils.ItalicText(), "Views");
            foreach (var view in views.Data)
            {
                console.Write(ConsoleColor.Blue, "· ");
                console.Write(view.Name ?? "?");
                console.Write(" \t");
                console.Write(view.ViewType ?? "?");
                console.Write(" \t");
                console.WriteLine(ConsoleColor.DarkGray, view.Id);
            }

            console.WriteLine();
        }

        console.WriteLine(LarkCliUtils.ItalicText(), "Records");
        var records = await lark.ReadBaseTableAsync(token, id, new LarkPageTokenInfo(100), cancellationToken);
        if (records?.Data is null || records.IsError || records.Data.Count < 1)
        {
            LarkCliUtils.WriteEmpty(console);
            return tables.Data;
        }

        var recordItems = new List<SelectionItem<string>>();
        foreach (var record in records.Data)
        {
            var json = record.Simplify();
            if (json is null) continue;
            recordItems.Add(new(json.ToString()));
        }

        if (records.Data.Count >= 100) console.WriteLine("Return 100 records at most.");
        LarkCliUtils.WriteOrderedLine(console, recordItems, true);
        return tables.Data;
    }

    private async Task ResolveFileAsync(string node, string name, CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        var lark = LarkApi.DefaultInstance;
        if (string.IsNullOrWhiteSpace(name) || !name.Contains('.'))
        {
            console.WriteLine("This is an online file.");
            return;
        }

        name = name.Trim().ToLowerInvariant();
        if (name.EndsWith(".md") || name.EndsWith(".markdown") || name.EndsWith(".txt") || name.EndsWith(".yaml") || name.EndsWith(".jsonl") || name.EndsWith(".xml") || name.EndsWith(".log"))
        {
            var file = await lark.ReadDocsTextFileAsync(node, cancellationToken);
            if (file?.Data is null || file.IsError)
            {
                console.WriteLine("Load file failed.");
                if (!string.IsNullOrWhiteSpace(file?.Message)) console.WriteLine(file.Message);
            }
            else if (string.IsNullOrWhiteSpace(file.Data.Value))
            {
                LarkCliUtils.WriteEmpty(console);
            }
            else
            {
                console.WriteLine(file.Data.Value);
            }
        }
        else if (name.EndsWith(".json"))
        {
            var file = await lark.ReadDocsTextFileAsync(node, cancellationToken);
            if (file?.Data is null || file.IsError)
            {
                console.WriteLine("Load file failed.");
                if (!string.IsNullOrWhiteSpace(file?.Message)) console.WriteLine(file.Message);
            }
            else if (string.IsNullOrWhiteSpace(file.Data.Value))
            {
                LarkCliUtils.WriteEmpty(console);
            }
            else
            {
                var json = JsonObjectNode.TryParse(file.Data.Value);
                if (json is null) console.WriteLine(file.Data.Value);
                else console.WriteLine(json);
            }
        }
        else
        {
            console.WriteLine("This is an online file.");
        }
    }
}

public class InternalLarkDocsContentCliWriter
{
    private LarkContentBlock? root;

    public void WriteLine(StyleConsole console, IEnumerable<LarkContentBlock?>? col)
    {
        console ??= StyleConsole.Default;
        var tree = col?.ToTree(root);
        if (tree is null)
        {
            LarkCliUtils.WriteEmpty(console);
            return;
        }

        root ??= col!.GetPageOrFirst();
        LarkCliUtils.WriteLine(console, tree);
    }
}

public static partial class LarkCliUtils
{
    public static LarkContentBlockTree? WriteLine(this StyleConsole console, IEnumerable<LarkContentBlock?>? col)
    {
        console ??= StyleConsole.Default;
        var tree = col?.ToTree();
        if (tree is null) WriteEmpty(console);
        else WriteLine(console, tree);
        return tree;
    }

    public static void WriteLine(this StyleConsole console, LarkWikiSpaceInfo space)
    {
        console ??= StyleConsole.Default;
        console.WriteLine(BoldText(), space.Name);
        console.WriteLine(ConsoleColor.Yellow, space.Id);
        console.WriteLine();
        var style = new ConsoleTextStyle(Color.FromArgb(0xCE, 0x91, 0x78), ConsoleColor.Green, null, null);
        if (string.IsNullOrWhiteSpace(space.Description))
        {
        }
        else if (space.Description.Length > 20)
        {
            console.WriteLine(space.Description);
            console.WriteLine();
        }
        else
        {
            WritePropertyLine(console, "Desc", space.Description);
        }

        WritePropertyLine(console, "Type", space.SpaceType);
        WritePropertyLine(console, "Share", space.ShareState);
        WritePropertyLine(console, "Visibility", space.Visibility);
    }

    public static void WriteLine(this StyleConsole console, LarkDocsNodeInfo node)
    {
        if (node is null) return;
        console ??= StyleConsole.Default;
        if (!string.IsNullOrWhiteSpace(node.Name))
            console.WriteLine(BoldText(), node.Name);
        if (!string.IsNullOrWhiteSpace(node.Url))
            console.WriteLine(node.Url);
        console.WriteLine();
        console.WriteLine(ItalicText(), "Node information");
        console.WriteLine(ConsoleColor.Yellow, node.NodeToken);
        WritePropertyLine(console, "Type", node.NodeType);
        WritePropertyLine(console, "Creation", node.NodeCreationTime.ToString("D"));
        console.WriteLine();
        console.WriteLine(ItalicText(), "Doc information");
        console.WriteLine(ConsoleColor.Yellow, node.DocToken);
        WritePropertyLine(console, "Type", node.DocType);
        WritePropertyLine(console, "Creation", node.DocCreationTime.ToString("D"));
        WritePropertyLine(console, "Modification", node.DocLastModificationTime.ToString("D"));
    }

    public static void WriteLine(this StyleConsole console, LarkContentBlockTree tree)
    {
        console ??= StyleConsole.Default;
        if (tree.Content is not null)
        {
            var i = 0;
            foreach (var content in tree.Content)
            {
                var text = content.Text ?? content.Information?.DisplayName;
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (tree.BlockType == LarkContentBlockType.Separator)
                        console.Write(ConsoleColor.DarkGray, "----------");
                    continue;
                }

                switch (tree.BlockType)
                {
                    case LarkContentBlockType.Bullet:
                        console.Write(ConsoleColor.Blue, "· ");
                        console.Write(text);
                        break;
                    case LarkContentBlockType.Ordered:
                        console.Write(ConsoleColor.Blue, "· ");
                        console.Write(text);
                        break;
                    case LarkContentBlockType.Heading1:
                        console.WriteLine();
                        console.Write(ConsoleColor.DarkGray, "#  ");
                        console.Write(BoldText(ConsoleColor.Yellow, 240, 240, 48, true), text);
                        break;
                    case LarkContentBlockType.Heading2:
                        console.WriteLine();
                        console.Write(ConsoleColor.DarkGray, "## ");
                        console.Write(BoldText(ConsoleColor.Yellow, 200, 200, 48, true), text);
                        break;
                    case LarkContentBlockType.Heading3:
                        console.WriteLine();
                        console.Write(ConsoleColor.DarkGray, "### ");
                        console.Write(BoldText(ConsoleColor.Yellow, 180, 180, 32, true), text);
                        break;
                    case LarkContentBlockType.Heading4:
                        console.WriteLine();
                        console.Write(ConsoleColor.DarkGray, "#### ");
                        console.Write(BoldText(ConsoleColor.Green, 48, 240, 48, true), text);
                        break;
                    case LarkContentBlockType.Heading5:
                        console.WriteLine();
                        console.Write(ConsoleColor.DarkGray, "##### ");
                        console.Write(BoldText(ConsoleColor.Green, 48, 200, 48, true), text);
                        break;
                    case LarkContentBlockType.Heading6:
                        console.WriteLine();
                        console.Write(ConsoleColor.DarkGray, "###### ");
                        console.Write(BoldText(ConsoleColor.Green, 32, 180, 32, true), text);
                        break;
                    case LarkContentBlockType.Heading7:
                        console.WriteLine();
                        console.Write(ConsoleColor.DarkGray, "####### ");
                        console.Write(BoldText(), text);
                        break;
                    case LarkContentBlockType.Heading8:
                        console.WriteLine();
                        console.Write(ConsoleColor.DarkGray, "######## ");
                        console.Write(BoldText(), text);
                        break;
                    case LarkContentBlockType.Heading9:
                        console.WriteLine();
                        console.Write(ConsoleColor.DarkGray, "######### ");
                        console.Write(BoldText(), text);
                        break;
                    default:
                        console.Write(text);
                        break;
                }

                i++;
            }

            if (i > 0) console.WriteLine();
        }

        if (tree.Children is not null)
        {
            foreach (var child in tree.Children)
            {
                WriteLine(console, child);
            }
        }
    }
}
