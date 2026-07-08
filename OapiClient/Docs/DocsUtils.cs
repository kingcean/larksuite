using LarkSuite.Docs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Collection;
using Trivial.Text;

namespace LarkSuite.OapiModels;

/// <summary>
/// The utilities of Lark doc content.
/// </summary>
public static partial class LarkApiUtils
{
    /// <summary>
    /// Gets the content block by identifier.
    /// </summary>
    /// <param name="col">The content block collection.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>The content block with the specific identifier; or null, if not found.</returns>
    public static LarkContentBlock? GetById(this IEnumerable<LarkContentBlock> col, string id)
    {
        if (col is null) return null;
        foreach (var item in col)
        {
            if (item?.Id == id) return item;
        }

        return null;
    }

    /// <summary>
    /// Gets the content block of page.
    /// </summary>
    /// <param name="col">The content block collection.</param>
    /// <returns>The content block of page; or null, if not found.</returns>
    public static LarkContentBlock? GetPage(this IEnumerable<LarkContentBlock?> col)
    {
        if (col is null) return null;
        foreach (var item in col)
        {
            if (item?.BlockType == LarkContentBlockType.Page) return item;
        }

        return null;
    }

    /// <summary>
    /// Gets the content block of page; or the first one if the page does not exist.
    /// </summary>
    /// <param name="col">The content block collection.</param>
    /// <returns>The content block of page; or the first one if the page does not exist; or null, if nothing.</returns>
    public static LarkContentBlock? GetPageOrFirst(this IEnumerable<LarkContentBlock?> col)
    {
        if (col is null) return null;
        LarkContentBlock? first = null;
        foreach (var item in col)
        {
            if (item is null) continue;
            if (item.BlockType == LarkContentBlockType.Page) return item;
            first ??= item;
        }

        return first;
    }

    /// <summary>
    /// Simplifies the content block collection to a tree with key information.
    /// </summary>
    /// <param name="col">The content block collection.</param>
    /// <param name="root">The optional root content block to build the tree.</param>
    /// <returns>The content block tree.</returns>
    public static LarkContentBlockTree ToTree(this IEnumerable<LarkContentBlock?> col, LarkContentBlock? root = null)
    {
        root ??= GetPageOrFirst(col);
        if (root is null) return new()
        {
            BlockType = LarkContentBlockType.Unsupported,
        };
        var tree = new LarkContentBlockTree
        {
            Id = root.Id,
            BlockType = root.BlockType,
        };
        if (root.Elements is not null)
        {
            foreach (var element in root.Elements)
            {
                if (element is null) continue;
                var text = element.Text?.Content;
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (string.IsNullOrWhiteSpace(element.DocMentioned?.Url)
                        && string.IsNullOrWhiteSpace(element.UserMentioned?.Id))
                        continue;
                }

                var content = new LarkContentBlockTreeContent
                {
                    Text = text,
                };
                if (!string.IsNullOrWhiteSpace(element.DocMentioned?.Url))
                    content.Information = new LarkContentBlockLinkReference
                    {
                        Url = element.DocMentioned.Url,
                        Title = element.DocMentioned.Name,
                    };
                else if (!string.IsNullOrWhiteSpace(element.UserMentioned?.Id))
                    content.Information = new LarkContentBlockUserReference
                    {
                        Id = element.UserMentioned.Id
                    };
                tree.Content ??= [];
                tree.Content.Add(content);
            }
        }

        if (root.ChildIds is not null)
        {
            foreach (var id in root.ChildIds)
            {
                var child = GetById(col, id);
                if (child is null) continue;
                var sub = ToTree(col, child);
                if (sub is null) continue;
                if (sub.Content is null
                    && sub.BlockType == LarkContentBlockType.TableCell
                    && sub.Children is not null
                    && sub.Children.Count == 1
                    && sub.Children[0].BlockType == LarkContentBlockType.Text)
                {
                    var first = sub.Children[0];
                    sub.Content = first.Content;
                    sub.Children = first.Children;
                }

                tree.Children ??= [];
                tree.Children.Add(sub);
            }
        }

        return tree;
    }

    public static IEnumerable<SelectionItem<string>> ToSelectionStringItems(this IEnumerable<LarkWikiSpaceInfo> col)
    {
        if (col is null) yield break;
        foreach (var space in col)
        {
            yield return new(space.Name, space.Id);
        }
    }

    public static IEnumerable<SelectionItem<string>> ToSelectionStringItems(this IEnumerable<LarkDocsNodeInfo> col)
    {
        if (col is null) yield break;
        foreach (var space in col)
        {
            yield return new(space.Name, space.NodeToken);
        }
    }

    public static IEnumerable<SelectionItem<string>> ToSelectionStringItems(this IEnumerable<LarkDocsBaseTableTableInfo> col)
    {
        if (col is null) yield break;
        foreach (var table in col)
        {
            yield return new(table.Name, table.Id);
        }
    }
}
