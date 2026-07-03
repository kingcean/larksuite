using LarkSuite;
using LarkSuite.Docs;
using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using Trivial.Collection;
using Trivial.CommandLine;
using Trivial.Tasks;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.CommandLine;

internal class LarkDocsCommandVerb : BaseCommandVerb
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
                    new("Get the text content of the specific doc ID.", "doc"),
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
                    await GetWikiSpaceNodesAsync(cancellationToken);
                    break;
                case "doc":
                case "2":
                    await ResolveDocAsync(cancellationToken);
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

    public async Task<IReadOnlyList<LarkDocsNodeInfo>?> GetWikiSpaceNodesAsync(CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        var spaces = await LarkApi.DefaultInstance.GetWikiSpacesAsync(cancellationToken);
        if (LarkCliUtils.WriteEmpty(console, spaces)) return null;
        var list = new List<SelectionItem<string>>();
        foreach (var space in spaces.Data)
        {
            if (string.IsNullOrWhiteSpace(space?.Id)) continue;
            list.Add(new(space.Name, space.Id));
        }

        LarkCliUtils.WriteOrderedLine(console, list, true);
        console.WriteLine();
        console.WriteLine("Please type the index or the space ID.");
        var id = LarkCliUtils.ReadId(console, "Docs\\Space", list)!;
        console.WriteLine();
        return await GetWikiSpaceNodesAsync(id);
    }

    public async Task<IReadOnlyList<LarkDocsNodeInfo>?> GetWikiSpaceNodesAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id)) return null;
        var info = await LarkApi.DefaultInstance.GetWikiSpaceInfoAsync(id, cancellationToken);
        var console = CurrentConsole;
        if (info?.Data is null)
        {
            LarkCliUtils.WriteEmpty(console);
            return null;
        }

        console.WriteLine(info.Data);
        var nodes = await LarkApi.DefaultInstance.GetWikiSpaceNodesAsync(id, cancellationToken);
        console.WriteLine();
        await GetWikiSpaceNodesAsync(id, nodes, async sub =>
        {
            switch (sub)
            {
                case ".":
                    return true;
                case "..":
                    console.WriteLine();
                    await GetWikiSpaceNodesAsync(cancellationToken);
                    return true;
                default:
                    return false;
            }
        }, cancellationToken);
        return nodes.Data;
    }

    public async Task<IReadOnlyList<LarkDocsNodeInfo>?> GetWikiSpaceNodesAsync(string id, string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id)) return null;
        var info = await LarkApi.DefaultInstance.GetWikiNodeAsync(token, cancellationToken);
        var console = CurrentConsole;
        if (info?.Data is null)
        {
            LarkCliUtils.WriteEmpty(console);
            return null;
        }

        console.WriteLine(info.Data);
        var nodes = await LarkApi.DefaultInstance.GetWikiSpaceNodesAsync(id, new LarkWikiNodesRequestOptions()
        {
            ParentNodeToken = token,
        }, null, cancellationToken);
        console.WriteLine();
        await GetWikiSpaceNodesAsync(id, nodes, async sub =>
        {
            switch (sub)
            {
                case ".":
                    console.WriteLine();
                    await ResolveDocAsync(token, cancellationToken);
                    console.Write(ConsoleColor.DarkGray, "--- * THE END * ---");
                    console.Write("  (Press any key to continue...)  ");
                    console.ReadKey();
                    console.WriteLine();
                    return true;
                case "..":
                    console.WriteLine();
                    if (string.IsNullOrWhiteSpace(info.Data.ParentNodeToken) || sub == info.Data.ParentNodeToken)
                    {
                        console.WriteLine("Current node is the top one.");
                        return false;
                    }

                    await GetWikiSpaceNodesAsync(info.Data.ParentNodeToken, cancellationToken);
                    return true;
                default:
                    return false;
            }
        }, cancellationToken);
        return nodes.Data;
    }

    public async Task<string?> GetWikiSpaceNodesAsync(string id, LarkResponsePagingBody<LarkDocsNodeInfo> nodes, Func<string, Task<bool>>? command = null, CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        console.WriteLine(LarkCliUtils.ItalicText(), "Contents");
        var items = new List<SelectionItem<string>>();
        foreach (var node in nodes.Data)
        {
            if (node is null) continue;
            items.Add(new(node.Title, node.NodeToken));
        }

        if (items.Count < 1)
        {
            LarkCliUtils.WriteEmpty(console);
            return null;
        }

        LarkCliUtils.WriteOrderedLine(console, items, true);
        console.WriteLine();
        console.WriteLine("Please type the index or node token to get the child details; or press ENTER to turn back.");
        var sub = LarkCliUtils.ReadId(console, "Docs\\Doc", items);
        if (!string.IsNullOrWhiteSpace(sub))
        {
            if (command is null || !await command(sub))
                await GetWikiSpaceNodesAsync(id, sub, cancellationToken);
            return sub;
        }

        return null;
    }

    public Task<LarkResponsePagingBody<LarkContentBlock>?> ResolveDocAsync(CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        var id = LarkCliUtils.ReadLine(console, "Docs\\Doc");
        cancellationToken.ThrowIfCancellationRequested();
        return ResolveDocAsync(id, cancellationToken);
    }

    public async Task<LarkResponsePagingBody<LarkContentBlock>?> ResolveDocAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token)) return null;
        var console = CurrentConsole;
        var info = await LarkApi.DefaultInstance.GetDocsInfoAsync(token, cancellationToken);
        console.WriteLine(info.Data);
        console.WriteLine();
        var task = LarkApi.DefaultInstance.GetDocsBlocksAsync(token, cancellationToken);
        var writer = new InternalLarkDocsContentCliWriter();
        var blocks = await LarkCliUtils.WritePagesAsync(
            console,
            LarkApi.DefaultInstance.GetDocsBlocksAsync,
            LarkApi.DefaultInstance.GetDocsBlocksAsync,
            token,
            writer.WriteLine,
            null,
            cancellationToken);
        console.WriteLine();
        return blocks;
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
        if (!string.IsNullOrWhiteSpace(node.Title))
            console.WriteLine(BoldText(), node.Title);
        if (!string.IsNullOrWhiteSpace(node.Url))
            console.WriteLine(node.Url);
        console.WriteLine();
        console.WriteLine(ItalicText(), "Node information");
        console.WriteLine(ConsoleColor.Yellow, node.NodeToken);
        WritePropertyLine(console, "Type", node.NodeType);
        WritePropertyLine(console, "Creation", node.NodeCreationTime.ToString("D"));
        console.WriteLine(node.HasChild ? "Has child nodes" : "Leaf node");
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
