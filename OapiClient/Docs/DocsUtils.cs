using LarkSuite.Docs;
using System;
using System.Buffers.Text;
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
                else if (!string.IsNullOrWhiteSpace(element.Text?.Style?.Link?.Url))
                    content.Information = new LarkContentBlockLinkReference
                    {
                        Url = element.Text.Style.Link.Url,
                        Title = text,
                    };
                tree.Content ??= [];
                tree.Content.Add(content);
            }
        }

        if (root.ChildIds is not null)
        {
            foreach (var id in root.ChildIds)
            {
                var child = GetById(col!, id);
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

    public static async Task<List<LarkDocsBaseTableRecord<JsonObjectNode>>> SimplifyAsync(LarkApi? larkApi, string baseId, string tableId, LarkPageTokenInfo paging, Dictionary<string, string> mapping, CancellationToken cancellationToken = default)
    {
        var records = await (larkApi ?? LarkApi.DefaultInstance).ReadBaseTableAsync(baseId, tableId, paging, cancellationToken);
        return Simplify(records, mapping).ToList();
    }

    public static async Task<List<LarkDocsBaseTableRecord<T>>> SimplifyAsync<T>(LarkApi? larkApi, string baseId, string tableId, LarkPageTokenInfo paging, Dictionary<string, string> mapping, CancellationToken cancellationToken = default)
    {
        var records = await (larkApi ?? LarkApi.DefaultInstance).ReadBaseTableAsync(baseId, tableId, paging, cancellationToken);
        return Simplify<T>(records, mapping).ToList();
    }

    public static async Task<List<LarkDocsBaseTableRecord<JsonObjectNode>>> SimplifyAsync(LarkApi? larkApi, string baseId, string tableId, Dictionary<string, string> mapping, CancellationToken cancellationToken = default)
    {
        var records = await (larkApi ?? LarkApi.DefaultInstance).ReadBaseTableAsync(baseId, tableId, cancellationToken);
        return Simplify(records, mapping).ToList();
    }

    public static async Task<List<LarkDocsBaseTableRecord<T>>> SimplifyAsync<T>(LarkApi? larkApi, string baseId, string tableId, Dictionary<string, string> mapping, CancellationToken cancellationToken = default)
    {
        var records = await (larkApi ?? LarkApi.DefaultInstance).ReadBaseTableAsync(baseId, tableId, cancellationToken);
        return Simplify<T>(records, mapping).ToList();
    }

    public static async Task<List<LarkDocsBaseTableRecord<JsonObjectNode>>> SimplifyAsync(LarkApi? larkApi, string baseId, string tableId, bool all, Dictionary<string, string> mapping, CancellationToken cancellationToken = default)
    {
        larkApi ??= LarkApi.DefaultInstance;
        var records = await larkApi.ReadBaseTableAsync(baseId, tableId, cancellationToken);
        if (records?.Data is null || records.IsError) return [];
        if (all)
        {
            while (records.HasNextPage)
            {
                var col = larkApi.ReadBaseTableAsync(records, 500, cancellationToken);
                if (col is null) break;
            }
        }

        return Simplify(records, mapping).ToList();
    }

    public static async Task<List<LarkDocsBaseTableRecord<T>>> SimplifyAsync<T>(LarkApi? larkApi, string baseId, string tableId, bool all, Dictionary<string, string> mapping, CancellationToken cancellationToken = default)
    {
        larkApi ??= LarkApi.DefaultInstance;
        var records = await larkApi.ReadBaseTableAsync(baseId, tableId, cancellationToken);
        if (records?.Data is null || records.IsError) return [];
        if (all)
        {
            while (records.HasNextPage)
            {
                var col = larkApi.ReadBaseTableAsync(records, 500, cancellationToken);
                if (col is null) break;
            }
        }

        return Simplify<T>(records, mapping).ToList();
    }

    public static async Task<List<LarkDocsBaseTableRecord<JsonObjectNode>>> SimplifyAsync(this Task<LarkResponsePagingBody<LarkDocsBaseTableRecord>> records, Dictionary<string, string> mapping)
        => Simplify(await records, mapping).ToList();

    public static async Task<List<LarkDocsBaseTableRecord<T>>> SimplifyAsync<T>(this Task<LarkResponsePagingBody<LarkDocsBaseTableRecord>> records, Dictionary<string, string> mapping)
        => Simplify<T>(await records, mapping).ToList();

    public static IEnumerable<LarkDocsBaseTableRecord<T>> Simplify<T>(this LarkResponsePagingBody<LarkDocsBaseTableRecord> records, Dictionary<string, string> mapping)
    {
        foreach (var fields in Simplify(records, mapping))
        {
            var info = fields.Deserialize<T>();
            if (info is not null && info.Data is not null) yield return info;
        }
    }

    public static IEnumerable<LarkDocsBaseTableRecord<JsonObjectNode>> Simplify(this LarkResponsePagingBody<LarkDocsBaseTableRecord> records, Dictionary<string, string> mapping)
    {
        if (records?.Data is null || records.IsError) yield break;
        foreach (var record in records.Data)
        {
            var fields = record.Simplify(mapping);
            if (fields is null) continue;
            yield return new(record, fields);
        }
    }

    public static async Task<LarkDocsBaseTableRecord<T>> SimplifyAsync<T>(LarkApi? larkApi, string baseId, string tableId, string recordId, Dictionary<string, string> mapping, CancellationToken cancellationToken = default)
    {
        var json = await SimplifyAsync(larkApi, baseId, tableId, recordId, mapping, cancellationToken);
        if (json is null) return new(null, default);
        return json.Deserialize<T>();
    }

    public static async Task<LarkDocsBaseTableRecord<JsonObjectNode>> SimplifyAsync(LarkApi? larkApi, string baseId, string tableId, string recordId, Dictionary<string, string> mapping, CancellationToken cancellationToken = default)
    {
        larkApi ??= LarkApi.DefaultInstance;
        var resp = await larkApi.GetBaseTableRecordsAsync(baseId, tableId, [recordId], cancellationToken);
        var record = resp?.Data?.Get(recordId);
        if (string.IsNullOrWhiteSpace(record?.Id)) return new(null, null);
        return new(record, record.Simplify(mapping));
    }

    public static IEnumerable<T?> ListDataOrNull<T>(this IEnumerable<LarkDocsBaseTableRecord<T>> records)
    {
        if (records is null) yield break;
        foreach (var fields in records)
        {
            yield return fields.Data;
        }
    }

    public static IEnumerable<T> ListData<T>(this IEnumerable<LarkDocsBaseTableRecord<T>> records)
    {
        if (records is null) yield break;
        foreach (var fields in records)
        {
            if (fields.Data is not null) yield return fields.Data;
        }
    }

    public static async IAsyncEnumerable<T?> ListDataOrNullAsync<T>(this IAsyncEnumerable<LarkDocsBaseTableRecord<T>> records)
    {
        if (records is null) yield break;
        await foreach (var fields in records)
        {
            yield return fields.Data;
        }
    }

    public static async IAsyncEnumerable<T> ListDataAsync<T>(this IAsyncEnumerable<LarkDocsBaseTableRecord<T>> records)
    {
        if (records is null) yield break;
        await foreach (var fields in records)
        {
            if (fields.Data is not null) yield return fields.Data;
        }
    }

    public static LarkDocsBaseTableRecord<T> Deserialize<T>(this LarkDocsBaseTableRecord<JsonObjectNode> record)
    {
        if (record?.Data is null) return new(record?.Source, default);
        return new(record.Source, record.Data.Deserialize<T>());
    }

    public static async Task<T?> GetDataAsync<T>(this Task<LarkDocsBaseTableRecord<T>> task)
    {
        if (task is null) return default;
        var result = await task;
        if (result is null) return default;
        return result.Data ?? default;
    }
}
