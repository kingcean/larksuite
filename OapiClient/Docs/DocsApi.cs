using LarkSuite.Docs;
using LarkSuite.OapiModels;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Trivial.Net;
using Trivial.Security;
using Trivial.Text;

namespace LarkSuite;

public partial class LarkApi
{
    /// <summary>
    /// Gets the wiki (and docs) spaces that current account can access.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki space information</returns>
    [Description("Get the wiki (and docs) spaces that current account can access.")]
    public Task<LarkResponsePagingBody<LarkWikiSpaceInfo>> GetWikiSpacesAsync(CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.WikiSpaces, LarkWikiSpaceInfo.Convert, cancellationToken);

    /// <summary>
    /// Gets a page of wiki spaces that the current account can access.
    /// </summary>
    /// <param name="options">The page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki space information for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkWikiSpaceInfo>> GetWikiSpacesAsync(LarkPageTokenInfo options, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.WikiSpaces, null, options, LarkWikiSpaceInfo.Convert, cancellationToken);

    /// <summary>
    /// Loads the next page of wiki spaces and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response to update.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki spaces loaded from the next page.</returns>
    public Task<IReadOnlyList<LarkWikiSpaceInfo>> GetWikiSpacesAsync(LarkResponsePagingBody<LarkWikiSpaceInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.WikiSpaces, response, pageSize, cancellationToken);

    /// <summary>
    /// Gets the information of the specific wiki (and docs) space.
    /// </summary>
    /// <param name="id">The wiki space identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki space information.</returns>
    [Description("Get the information of the specific wiki (and docs) space.")]
    public Task<LarkResponseBody<LarkWikiSpaceInfo>> GetWikiSpaceInfoAsync([Description("The wiki space identifier.")] string id, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceInfo, LarkUrls.GetId(id)), LarkWikiSpaceInfo.Convert, cancellationToken);

    /// <summary>
    /// Gets the information of the specific wiki space with additional request options.
    /// </summary>
    /// <param name="id">The wiki space identifier.</param>
    /// <param name="options">The additional resource request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki space information.</returns>
    public Task<LarkResponseBody<LarkWikiSpaceInfo>> GetWikiSpaceInfoAsync(string id, LarkResourceRequestOptions options, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceInfo, options, LarkUrls.GetId(id)), LarkWikiSpaceInfo.Convert, cancellationToken);

    /// <summary>
    /// Searches wiki by given query string.
    /// </summary>
    /// <param name="q">The query.</param>
    /// <param name="cancellationToken">A cancellation id to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    /// <remarks>User id only.</remarks>
    public Task<LarkResponseBody> SearchWikiAsync(string q, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.SearchWiki, new JsonObjectNode
        {
            { "query", q }
        }, cancellationToken);

    /// <summary>
    /// Searches wiki by given query string.
    /// </summary>
    /// <param name="options">The search options.</param>
    /// <param name="cancellationToken">A cancellation id to observe while waiting for the task to complete.</param>
    /// <returns>The response result.</returns>
    /// <remarks>User id only.</remarks>
    public Task<LarkResponseBody> SearchWikiAsync(LarkWikiSearchOptions options, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.SearchWiki, options), options?.ToJson() ?? [], cancellationToken);

    /// <summary>
    /// Lists the top level nodes of the wiki space.
    /// </summary>
    /// <param name="id">The wiki space identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The doc node info.</returns>
    public Task<LarkResponsePagingBody<LarkDocsNodeInfo>> GetWikiSpaceNodesAsync(string id, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsNodeInfo>(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, LarkUrls.GetId(id)), cancellationToken);

    /// <summary>
    /// Lists a page of nodes in a wiki space, optionally under a parent node.
    /// </summary>
    /// <param name="options">The wiki space identifier and optional parent node token.</param>
    /// <param name="page">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki nodes for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkDocsNodeInfo>> GetWikiSpaceNodesAsync(LarkWikiNodesRequestOptions options, LarkPageTokenInfo? page = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsNodeInfo>(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, options.SpaceId), options, page, cancellationToken);

    /// <summary>
    /// Loads the next page of wiki nodes and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the wiki node request options.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki nodes loaded from the next page.</returns>
    public Task<IReadOnlyList<LarkDocsNodeInfo>> GetWikiSpaceNodesAsync(LarkResponsePagingBody<LarkDocsNodeInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, (response?.Query as LarkWikiNodesRequestOptions)?.SpaceId), response!, pageSize, cancellationToken);

    /// <summary>
    /// Lists the members of a wiki space.
    /// </summary>
    /// <param name="token">The wiki space identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A page of wiki space members.</returns>
    public Task<LarkResponsePagingBody> GetWikiSpaceMembersAsync(string token, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceMembers, token), cancellationToken);

    /// <summary>
    /// Lists a page of top level nodes in a wiki space.
    /// </summary>
    /// <param name="id">The wiki space identifier.</param>
    /// <param name="paging">The page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki nodes for the requested page.</returns>
    public Task<LarkResponsePagingBody> GetWikiSpaceNodesAsync(string id, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, id), new LarkResourceIdRequest(id), paging, cancellationToken);

    /// <summary>
    /// Lists the top level nodes of the wiki space.
    /// </summary>
    /// <param name="id">The wiki space identifier.</param>
    /// <param name="loadAll">true if load all pages; otherwise, false, to load the first page of list.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The doc node info.</returns>
    [Description("List the top level nodes of the wiki space.")]
    public async Task<LarkResponsePagingBody> GetWikiSpaceNodesAsync([Description("The wiki space identifier.")] string id, [Description("A value indicating whether load all items instead of the first page of list.")] bool loadAll, CancellationToken cancellationToken = default)
    {
        if (!loadAll) return await GetWikiSpaceNodesAsync(id, cancellationToken);
        var resp = await GetWikiSpaceNodesAsync(id, new LarkPageTokenInfo(50), cancellationToken);
        await LarkApiUtils.LoadAllPagesAsync(resp, 50, GetWikiSpaceNodesAsync, cancellationToken).CountAsync(cancellationToken);
        return resp;
    }

    /// <summary>
    /// Lists nodes in a wiki space, optionally loading all pages.
    /// </summary>
    /// <param name="options">The wiki space identifier and optional parent node token.</param>
    /// <param name="loadAll">true to load all pages; otherwise, false, to load only the first page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki nodes matching the request options.</returns>
    public async Task<LarkResponsePagingBody<LarkDocsNodeInfo>> GetWikiSpaceNodesAsync(LarkWikiNodesRequestOptions options, bool loadAll, CancellationToken cancellationToken = default)
    {
        var paging = new LarkPageTokenInfo(50);
        if (!loadAll) return await GetWikiSpaceNodesAsync(options, paging, cancellationToken);
        var resp = await GetWikiSpaceNodesAsync(options, paging, cancellationToken);
        await LarkApiUtils.LoadAllPagesAsync(resp, paging.Size, GetWikiSpaceNodesAsync, cancellationToken).CountAsync(cancellationToken);
        return resp;
    }

    /// <summary>
    /// Lists all pages of child nodes of a wiki node.
    /// </summary>
    /// <param name="node">The parent wiki node.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The child nodes, or an error response if the space identifier is missing.</returns>
    /// <remarks>For a shortcut with no returned children, the child nodes of its original node are requested.</remarks>
    public async Task<LarkResponsePagingBody<LarkDocsNodeInfo>> GetWikiSpaceNodesAsync(LarkDocsNodeInfo node, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(node?.SpaceId)) return new(true, "The space identifier is required");
        var resp = await GetWikiSpaceNodesAsync(new LarkWikiNodesRequestOptions
        {
            SpaceId = node.SpaceId,
            ParentNodeToken = node.NodeToken,
        }, true, cancellationToken);
        if (node.NodeType == "shortcut")
        {
            if (resp is null || resp.Count < 1)
            {
                return await GetWikiSpaceNodesAsync(new LarkWikiNodesRequestOptions()
                {
                    SpaceId = node.OriginSpaceId,
                    ParentNodeToken = node.OriginNodeToken,
                }, true, cancellationToken);
            }
        }

        return resp;
    }

    /// <summary>
    /// Loads the next page of wiki nodes and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the wiki space identifier request.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki nodes loaded from the next page.</returns>
    public Task<IReadOnlyList<JsonObjectNode>> GetWikiSpaceNodesAsync(LarkResponsePagingBody response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, (response?.Query as LarkResourceIdRequest)?.Id), response!, pageSize, cancellationToken);

    /// <summary>
    /// Gets the information of a wiki node.
    /// </summary>
    /// <param name="token">The wiki node token or URL.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki node information.</returns>
    public Task<LarkResponseBody<LarkDocsNodeInfo>> GetWikiNodeAsync(string token, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(token) ? Task.FromResult(new LarkResponseBody<LarkDocsNodeInfo>(true, "The node token is not given.")) : GetAsync<LarkDocsNodeInfo>(new QueryData()
        {
            { "token", LarkUrls.GetId(token) },
        }.ToString(LarkUrls.GetWikiNode), "node", cancellationToken);

    /// <summary>
    /// Gets the wiki node information for a token of the specified object type.
    /// </summary>
    /// <param name="token">The token or URL of the object to look up.</param>
    /// <param name="objType">The object type associated with the token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The wiki node information.</returns>
    public Task<LarkResponseBody<LarkDocsNodeInfo>> GetWikiNodeAsync(string token, string objType, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(token) ? Task.FromResult(new LarkResponseBody<LarkDocsNodeInfo>(true, "The node token is not given.")) : GetAsync<LarkDocsNodeInfo>(new QueryData()
        {
            { "token", LarkUrls.GetId(token) },
            { "obj_type", objType },
        }.ToString(LarkUrls.GetWikiNode), "node", cancellationToken);

    /// <summary>
    /// Gets the information of an online document.
    /// </summary>
    /// <param name="token">The document token or URL.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The document information.</returns>
    public Task<LarkResponseBody<LarkDocsDocInfo>> GetDocsInfoAsync(string token, CancellationToken cancellationToken = default)
        => GetAsync<LarkDocsDocInfo>(string.Concat(LarkUrls.DocsInfo, LarkUrls.GetId(token)), "document", cancellationToken);

    /// <summary>
    /// Gets the file text.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The text content.</returns>
    public async Task<LarkResponseBody<string>> GetDocsTextAsync(string id, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.GetAsync(LarkUrls.ToUrl(LarkUrls.DocsText, LarkUrls.GetId(id)), cancellationToken);
        return new(resp);
    }

    /// <summary>
    /// Gets the content of the specific doc.
    /// </summary>
    /// <param name="token">The node token of the online doc.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The content of the doc.</returns>
    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public Task<LarkResponsePagingBody<LarkContentBlock>> GetDocsBlocksAsync([Description("The URL or node token (not the doc token nor file token) of the online doc.")] string token, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkContentBlock>(LarkUrls.ToUrl(LarkUrls.DocsBlocks, LarkUrls.GetId(token), true), new LarkResourceIdRequest(LarkUrls.GetId(token)), null, json => new(json), cancellationToken);

    /// <summary>
    /// Gets the content of the specific doc.
    /// </summary>
    /// <param name="token">The node token of the online doc.</param>
    /// <param name="paging">The paging information.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The content of the doc.</returns>
    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public Task<LarkResponsePagingBody<LarkContentBlock>> GetDocsBlocksAsync([Description("The URL or node token (not the doc token nor file token) of the online doc.")] string token, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkContentBlock>(LarkUrls.ToUrl(LarkUrls.DocsBlocks, LarkUrls.GetId(token), true), new LarkResourceIdRequest(LarkUrls.GetId(token)), paging, json => new(json), cancellationToken);

    /// <summary>
    /// Gets the content of the specific doc.
    /// </summary>
    /// <param name="token">The node token of the online doc.</param>
    /// <param name="loadAllPages">true if load all the blocks once, otherwise, false, to load the ones of the first page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The content of the doc.</returns>
    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public async Task<LarkResponsePagingBody<LarkContentBlock>> GetDocsBlocksAsync([Description("The URL or node token (not the doc token nor file token) of the online doc.")] string token, [Description("A flag to control if need load all the blocks once; or false, if load the ones of the first page only.")] bool loadAllPages, CancellationToken cancellationToken = default)
    {
        var resp = await GetDocsBlocksAsync(token, new LarkPageTokenInfo(50), cancellationToken);
        if (loadAllPages) await LarkApiUtils.LoadAllPagesAsync(resp, 50, GetDocsBlocksAsync, cancellationToken).CountAsync(cancellationToken);
        return resp;
    }

    /// <summary>
    /// Gets the content of the specific doc.
    /// </summary>
    /// <param name="response">The previous response to list the resources.</param>
    /// <param name="pageSize">The page size to load each page.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The content of the doc.</returns>
    [Description("Get the content of the specific identifier. The result is in a collection of block.")]
    public Task<IReadOnlyList<LarkContentBlock>> GetDocsBlocksAsync(LarkResponsePagingBody<LarkContentBlock> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.DocsBlocks, (response?.Query as LarkResourceIdRequest)?.Id), response!, pageSize, cancellationToken);

    /// <summary>
    /// Gets the Markdown content of an online document.
    /// </summary>
    /// <param name="options">The document token and additional request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the document content in Markdown format.</returns>
    public Task<LarkResponseBody> GetDocsMarkdownAsync(LarkWikiDocMarkdownOptions options, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.DocsMarkdown, options), cancellationToken);

    /// <summary>
    /// Gets the Markdown content of an online document.
    /// </summary>
    /// <param name="token">The document token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the document content in Markdown format.</returns>
    public Task<LarkResponseBody> GetDocsMarkdownAsync(string token, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.DocsMarkdown, new LarkWikiDocMarkdownOptions()
        {
            Id = token,
        }), cancellationToken);

    /// <summary>
    /// Gets the nodes of a whiteboard.
    /// </summary>
    /// <param name="id">The whiteboard identifier or URL.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the whiteboard nodes.</returns>
    public Task<LarkResponseBody> GetDocsBoardNodesAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.DocsBoardNodes, LarkUrls.GetId(id)), cancellationToken);

    /// <summary>
    /// Gets the nodes of a whiteboard with additional request options.
    /// </summary>
    /// <param name="id">The whiteboard identifier or URL.</param>
    /// <param name="options">The user identifier type request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the whiteboard nodes.</returns>
    public Task<LarkResponseBody> GetDocsBoardNodesAsync(string id, LarkUserIdTypeRequestOptions options, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.DocsBoardNodes, options, LarkUrls.GetId(id)), cancellationToken);

    /// <summary>
    /// Creates an online document in a folder.
    /// </summary>
    /// <param name="containerToken">The destination folder token.</param>
    /// <param name="title">The document title.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The created document information.</returns>
    public Task<LarkResponseBody<LarkDocsDocInfo>> CreateDocsNodeAsync(string containerToken, string title, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.DocsInfo, new()
        {
            { "folder_token", containerToken },
            { "title", title },
        }, json => json?.DeserializeValue<LarkDocsDocInfo>("document")!, cancellationToken);

    /// <summary>
    /// Creates a node in a wiki space.
    /// </summary>
    /// <param name="options">The destination wiki space, parent node, title, and node creation options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The created wiki node information.</returns>
    public Task<LarkResponseBody<LarkDocsNodeInfo>> CreateDocsNodeAsync(LarkWikiNodesCreateRequestOptions options, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.WikiSpaceNodes, options.SpaceId), JsonObjectNode.ConvertFrom(options), json => json?.DeserializeValue<LarkDocsNodeInfo>("node")!, cancellationToken);

    /// <summary>
    /// Requests moving a wiki node to the specified destination.
    /// </summary>
    /// <param name="options">The source wiki node and destination space and parent node options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The move task information.</returns>
    public Task<LarkResponseBody<BaseLarkTaskInfo>> MoveDocsNodeAsync(LarkDocsNodeMoveRequest options, CancellationToken cancellationToken = default)
        => PostAsync<BaseLarkTaskInfo>(LarkUrls.ToUrl(LarkUrls.MoveWikiNode, options.SourceSpaceId, options.SourceToken), JsonObjectNode.ConvertFrom(options), cancellationToken);

    /// <summary>
    /// Lists a page of comments on a document.
    /// </summary>
    /// <param name="options">The document token and comment query options.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The comments for the requested page.</returns>
    public Task<LarkResponsePagingBody> ListDocsCommentsAsync(LarkDocsCommentListOptions options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.DocsComments, options.DocToken), options, paging, cancellationToken);

    /// <summary>
    /// Loads the next page of document comments and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the comment query options.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The comments loaded from the next page.</returns>
    public Task<IReadOnlyList<JsonObjectNode>> ListDocsCommentsAsync(LarkResponsePagingBody response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.DocsComments, (response.Query as LarkDocsCommentListOptions)?.DocToken), response, pageSize, cancellationToken);

    /// <summary>
    /// Adds a comment to a document.
    /// </summary>
    /// <param name="options">The target document and comment content options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response of the comment creation request.</returns>
    public Task<LarkResponseBody> AddDocsCommentAsync(LarkDocsCommentAddOptions options, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.DocsAddComment, options, options.DocToken), options.ToJson(), cancellationToken);

    /// <summary>
    /// Adds a reply to a document comment.
    /// </summary>
    /// <param name="options">The target document, comment identifier, and reply content options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response of the comment reply request.</returns>
    public Task<LarkResponseBody> ReplyDocsCommentAsync(LarkDocsCommentReplyOptions options, CancellationToken cancellationToken = default)
        => PostAsync(LarkUrls.ToUrl(LarkUrls.DocsReplyComment, options, options.DocToken, options.CommentId), options.ToJson(), cancellationToken);

    /// <summary>
    /// Uploads a local file under a wiki node.
    /// </summary>
    /// <param name="name">The uploaded file name.</param>
    /// <param name="file">The local file to upload.</param>
    /// <param name="parentToken">The parent wiki node token or URL.</param>
    /// <param name="mime">The optional MIME type of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the uploaded file token.</returns>
    public Task<LarkResponseBody<string>> UploadDocsFileAsync(string name, FileInfo file, string parentToken, string? mime = null, CancellationToken cancellationToken = default)
        => UploadDocsFileAsync(name, file, false, parentToken, mime, cancellationToken);

    /// <summary>
    /// Uploads a file stream under a wiki node.
    /// </summary>
    /// <param name="name">The uploaded file name.</param>
    /// <param name="file">The file stream to upload.</param>
    /// <param name="parentToken">The parent wiki node token or URL.</param>
    /// <param name="mime">The optional MIME type of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the uploaded file token.</returns>
    public Task<LarkResponseBody<string>> UploadDocsFileAsync(string name, Stream file, string parentToken, string? mime = null, CancellationToken cancellationToken = default)
        => UploadDocsFileAsync(name, file, false, parentToken, mime, cancellationToken);

    /// <summary>
    /// Uploads a local file under a wiki node with a file token included in the request.
    /// </summary>
    /// <param name="token">The file token to include in the upload request.</param>
    /// <param name="name">The uploaded file name.</param>
    /// <param name="file">The local file to upload.</param>
    /// <param name="parentToken">The parent wiki node token or URL.</param>
    /// <param name="mime">The optional MIME type of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the uploaded file token.</returns>
    public Task<LarkResponseBody<string>> UploadDocsFileAsync(string token, string name, FileInfo file, string parentToken, string? mime = null, CancellationToken cancellationToken = default)
        => UploadDocsFileAsync(token, name, file, false, parentToken, mime, cancellationToken);

    /// <summary>
    /// Uploads a file stream under a wiki node with a file token included in the request.
    /// </summary>
    /// <param name="token">The file token to include in the upload request.</param>
    /// <param name="name">The uploaded file name.</param>
    /// <param name="file">The file stream to upload.</param>
    /// <param name="parentToken">The parent wiki node token or URL.</param>
    /// <param name="mime">The optional MIME type of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the uploaded file token.</returns>
    public Task<LarkResponseBody<string>> UploadDocsFileAsync(string token, string name, Stream file, string parentToken, string? mime = null, CancellationToken cancellationToken = default)
        => UploadDocsFileAsync(token, name, file, false, parentToken, mime, cancellationToken);

    /// <summary>
    /// Uploads a local file to a drive folder or wiki node.
    /// </summary>
    /// <param name="name">The uploaded file name.</param>
    /// <param name="file">The local file to upload.</param>
    /// <param name="isDriver">true to upload to a drive folder; otherwise, false, to upload under a wiki node.</param>
    /// <param name="parentToken">The parent folder or wiki node token or URL.</param>
    /// <param name="mime">The optional MIME type of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the uploaded file token.</returns>
    public async Task<LarkResponseBody<string>> UploadDocsFileAsync(string name, FileInfo file, bool isDriver, string parentToken, string? mime = null, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        content.Add("file_name", name);
        content.Add("parent_type", isDriver ? "explorer" : "wiki");
        content.Add("parent_node", LarkUrls.GetId(parentToken));
        content.Add("size", file.Length);
        content.Add("file", file, name, mime);
        var resp = await HttpClient.PostAsync(LarkUrls.UploadFile, content, cancellationToken);
        var json = await HttpClientExtensions.DeserializeJsonAsync<JsonObjectNode>(resp.Content, cancellationToken);
        return new(json, json =>
        {
            return json.TryGetStringTrimmedValue("file_token");
        });
    }

    /// <summary>
    /// Uploads a file stream to a drive folder or wiki node.
    /// </summary>
    /// <param name="name">The uploaded file name.</param>
    /// <param name="file">The file stream to upload.</param>
    /// <param name="isDriver">true to upload to a drive folder; otherwise, false, to upload under a wiki node.</param>
    /// <param name="parentToken">The parent folder or wiki node token or URL.</param>
    /// <param name="mime">The optional MIME type of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the uploaded file token.</returns>
    public async Task<LarkResponseBody<string>> UploadDocsFileAsync(string name, Stream file, bool isDriver, string parentToken, string? mime = null, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        content.Add("file_name", name);
        content.Add("parent_type", isDriver ? "explorer" : "wiki");
        content.Add("parent_node", LarkUrls.GetId(parentToken));
        content.Add("size", file.Length);
        content.Add("file", file, name, mime);
        var resp = await HttpClient.PostAsync(LarkUrls.UploadFile, content, cancellationToken);
        var json = await HttpClientExtensions.DeserializeJsonAsync<JsonObjectNode>(resp.Content, cancellationToken);
        return new(json, json =>
        {
            return json.TryGetStringTrimmedValue("file_token");
        });
    }

    /// <summary>
    /// Uploads a local file to a drive folder or wiki node with a file token included in the request.
    /// </summary>
    /// <param name="token">The file token to include in the upload request.</param>
    /// <param name="name">The uploaded file name.</param>
    /// <param name="file">The local file to upload.</param>
    /// <param name="isDriver">true to upload to a drive folder; otherwise, false, to upload under a wiki node.</param>
    /// <param name="parentToken">The parent folder or wiki node token or URL.</param>
    /// <param name="mime">The optional MIME type of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the uploaded file token.</returns>
    public async Task<LarkResponseBody<string>> UploadDocsFileAsync(string token, string name, FileInfo file, bool isDriver, string parentToken, string? mime = null, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        content.Add("file_token", token);
        content.Add("file_name", name);
        content.Add("parent_type", isDriver ? "explorer" : "wiki");
        content.Add("parent_node", LarkUrls.GetId(parentToken));
        content.Add("size", file.Length);
        content.Add("file", file, name, mime);
        var resp = await HttpClient.PostAsync(LarkUrls.UploadFile, content, cancellationToken);
        var json = await HttpClientExtensions.DeserializeJsonAsync<JsonObjectNode>(resp.Content, cancellationToken);
        return new(json, json =>
        {
            return json.TryGetStringTrimmedValue("file_token");
        });
    }

    /// <summary>
    /// Uploads a file stream to a drive folder or wiki node with a file token included in the request.
    /// </summary>
    /// <param name="token">The file token to include in the upload request.</param>
    /// <param name="name">The uploaded file name.</param>
    /// <param name="file">The file stream to upload.</param>
    /// <param name="isDriver">true to upload to a drive folder; otherwise, false, to upload under a wiki node.</param>
    /// <param name="parentToken">The parent folder or wiki node token or URL.</param>
    /// <param name="mime">The optional MIME type of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the uploaded file token.</returns>
    public async Task<LarkResponseBody<string>> UploadDocsFileAsync(string token, string name, Stream file, bool isDriver, string parentToken, string? mime = null, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        content.Add("file_token", token);
        content.Add("file_name", name);
        content.Add("parent_type", isDriver ? "explorer" : "wiki");
        content.Add("parent_node", LarkUrls.GetId(parentToken));
        content.Add("size", file.Length);
        content.Add("file", file, name, mime);
        var resp = await HttpClient.PostAsync(LarkUrls.UploadFile, content, cancellationToken);
        var json = await HttpClientExtensions.DeserializeJsonAsync<JsonObjectNode>(resp.Content, cancellationToken);
        return new(json, json =>
        {
            return json.TryGetStringTrimmedValue("file_token");
        });
    }

    /// <summary>
    /// Creates a wiki document from Markdown, falling back to uploading a Markdown file.
    /// </summary>
    /// <param name="name">The document or file name. A timestamp-based name is used if blank.</param>
    /// <param name="markdown">The Markdown content, or blank content to create an empty document.</param>
    /// <param name="container">The parent wiki node containing the node token and space identifier.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the created document or uploaded file's wiki node token.</returns>
    public Task<LarkResponseBody<string>> UploadDocsMarkdownAsync(string name, string markdown, LarkDocsNodeInfo container, CancellationToken cancellationToken = default)
        => UploadDocsMarkdownAsync(name, markdown, container?.NodeToken, container?.SpaceId, cancellationToken);

    /// <summary>
    /// Creates a wiki document from Markdown when a space identifier is provided, otherwise uploading a Markdown file.
    /// </summary>
    /// <param name="name">The document or file name. A timestamp-based name is used if blank.</param>
    /// <param name="markdown">The Markdown content. Blank content requires a space identifier to create an empty document.</param>
    /// <param name="containerToken">The required parent wiki node token.</param>
    /// <param name="spaceId">The optional wiki space identifier used to create a document.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the created document or uploaded file's wiki node token.</returns>
    /// <remarks>If creating or populating the document fails, the method attempts to upload the content as a Markdown file.</remarks>
    public async Task<LarkResponseBody<string>> UploadDocsMarkdownAsync(string name, string? markdown, string? containerToken, string? spaceId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(containerToken)) return new(true, "The contaner (parent) should not be empty.");
        string? docToken = null;
        string? docNodeToken = null;
        if (string.IsNullOrWhiteSpace(name)) name = DateTime.Now.ToString("Doc yyMMdd HHmmss");
        if (string.IsNullOrWhiteSpace(markdown))
        {
            if (string.IsNullOrWhiteSpace(spaceId)) return new(true, "The space identifier or the markdown content should not be null");
            var doc = await CreateDocsNodeAsync(new LarkWikiNodesCreateRequestOptions
            {
                ParentNodeToken = containerToken,
                SpaceId = spaceId,
                DocType = "docx",
                NodeType = "origin",
                Title = name,
            }, cancellationToken);
            if (doc is null) return new(true, "Create empty node failed.");
            if (doc.Data?.NodeToken is null || doc.IsError) return new(true, doc.Message ?? "Create empty node failed.");
            return new(0, "OK", doc.Data.NodeToken, new()
            {
                { "node_token", doc.Data.NodeToken }
            });
        }

        if (!string.IsNullOrWhiteSpace(spaceId))
        {
            try
            {
                var convert = await ConvertDocsBlocksAsync(null, markdown, null, cancellationToken);
                var isError = convert?.Data is null || convert.IsError;
                if (!isError)
                {
                    var doc = await CreateDocsNodeAsync(new LarkWikiNodesCreateRequestOptions
                    {
                        ParentNodeToken = containerToken,
                        SpaceId = spaceId,
                        DocType = "docx",
                        NodeType = "origin",
                        Title = name,
                    }, cancellationToken);
                    isError = string.IsNullOrWhiteSpace(doc?.Data?.DocToken) || doc.IsError;
                    if (!isError)
                    {
                        docToken = doc!.Data!.DocToken;
                        docNodeToken = doc.Data.NodeToken;
                        var blocks = await AddDocsBlocksAsync(docToken, null, convert!.Data!, 0, cancellationToken);
                        isError = blocks?.Data is null || blocks.IsError;
                        if (!isError) return new(0, "OK", docNodeToken, new()
                        {
                            { "node_token", docNodeToken }
                        });
                    }
                }
            }
            catch (ArgumentException)
            {
            }
            catch (FailedHttpException)
            {
            }
            catch (JsonException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (SecurityException)
            {
            }
        }

        using var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(markdown);
        writer.Flush();
        stream.Seek(0, SeekOrigin.Begin);
        var resp = await UploadDocsFileAsync(
            name.EndsWith(".md") ? name : string.Concat(name, ".md"),
            stream,
            containerToken,
            null,
            cancellationToken);
        if (resp is null) return new(true, "Upload file failed.");
        if (string.IsNullOrWhiteSpace(resp.Data) || resp.IsError) return resp;
        if (docToken is not null)
        {
            var blockId = Guid.NewGuid().ToString();
            var block = new JsonObjectNode
            {
                { "block_id", blockId },
                { "block_type", 2 },
                { "text", new JsonObjectNode()
                {
                    { "elements", new JsonArrayNode
                    {
                        new JsonObjectNode
                        {
                            { "text_run", new JsonObjectNode
                            {
                                { "content", name },
                            }
                            }
                        },
                        new JsonObjectNode
                        {
                            { "mention_doc", new JsonObjectNode
                            {
                                { "token", resp.Data },
                                { "obj_type", 12 },
                            }
                            }
                        },
                    }
                    }
                }
                }
            };
            await AddDocsBlocksAsync(docToken, null, [blockId], -1, [block], cancellationToken);
        }

        var fileNode = await GetWikiNodeAsync(resp.Data, "file", cancellationToken);
        if (string.IsNullOrWhiteSpace(fileNode?.Data?.NodeToken) || fileNode.IsError) return new(true, fileNode?.Message ?? "Cannot get the node info of the new markdown file uploaded.");
        if (docToken is not null && !string.IsNullOrWhiteSpace(spaceId) && !string.IsNullOrWhiteSpace(docNodeToken))
        {
            try
            {
                await MoveDocsNodeAsync(new()
                {
                    SourceSpaceId = spaceId,
                    SourceToken = docNodeToken,
                    DestinationSpaceId = spaceId,
                    DestinationToken = fileNode.Data.NodeToken,
                }, cancellationToken);
            }
            catch (ArgumentException)
            {
            }
            catch (FailedHttpException)
            {
            }
            catch (JsonException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (SecurityException)
            {
            }
        }

        return new(0, "OK", fileNode.Data.NodeToken, new()
        {
            { "node_token", fileNode.Data.NodeToken }
        });
    }

    /// <summary>
    /// Downloads a file as a stream.
    /// </summary>
    /// <param name="token">The file token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The downloaded file stream.</returns>
    public async Task<Stream> DownloadFileAsync(string token, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient<Stream>();
        var resp = await http.GetAsync(LarkUrls.ToUrl(LarkUrls.DownloadFile, token), cancellationToken);
        return resp;
    }

    /// <summary>
    /// Downloads and reads a wiki file as text using UTF-8 by default.
    /// </summary>
    /// <param name="node">The response containing the wiki node information and file token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The file text and wiki node information, or an error response if the file cannot be read.</returns>
    public async Task<LarkResponseBody<LarkDocsFileTextResponse>> ReadDocsTextFileAsync(LarkResponseBody<LarkDocsNodeInfo> node, CancellationToken cancellationToken = default)
    {
        if (node?.Data is null || node.IsError) return new(true, node?.Message);
        if (string.IsNullOrWhiteSpace(node.Data.DocToken)) return new(true, "Cannot find file identifier.");
        var file = await DownloadFileAsync(node.Data.DocToken, cancellationToken);
        if (file is null) return new(true, "Empty file.");
        if (!file.CanRead) return new(true, "Cannot read the stream.");
        using var reader = new StreamReader(file, Encoding.UTF8);
        var s = await reader.ReadToEndAsync();
        return new(new JsonObjectNode()
        {
            { "code", 0 },
            { "data", new JsonObjectNode()
            {
                { "value", s },
                { "node", (node as LarkResponseBody).Data },
            } },
            { "msg", "OK" },
        }, raw => new(node.Data, s));
    }

    /// <summary>
    /// Gets the text content of an onine file in wiki.
    /// </summary>
    /// <param name="token">The node token of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The file text and doc node information.</returns>
    [Description("Get the text content of an onine file in wiki.")]
    public async Task<LarkResponseBody<LarkDocsFileTextResponse>> ReadDocsTextFileAsync([Description("The node token (not the doc token nor file token) of the file.")] string token, CancellationToken cancellationToken = default)
    {
        var node = await GetWikiNodeAsync(token, cancellationToken);
        return await ReadDocsTextFileAsync(node, cancellationToken);
    }

    /// <summary>
    /// Creates an import task to convert an uploaded file to an online document.
    /// </summary>
    /// <param name="token">The uploaded file token.</param>
    /// <param name="ext">The source file extension.</param>
    /// <param name="docType">The target online document type.</param>
    /// <param name="parentToken">The destination folder token.</param>
    /// <param name="name">The optional name of the imported document.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the import task ticket.</returns>
    public Task<LarkResponseBody<string>> ConvertDocsFileFormatAsync(string token, string ext, string docType, string parentToken, string? name = null, CancellationToken cancellationToken = default)
    {
        var json = new JsonObjectNode()
        {
            { "file_extension", ext },
            { "file_token", token },
            { "type", docType },
            { "point", new JsonObjectNode
            {
                { "mount_type", 1 },
                { "mount_key", parentToken },
            } }
        };
        json.SetValueIfNotEmpty("file_name", name);
        return PostAsync(LarkUrls.ConvertDocsFileFormat, json, data =>
        {
            return data.TryGetStringValue("ticket");
        }, cancellationToken);
    }

    /// <summary>
    /// Gets the status and result of a file import task.
    /// </summary>
    /// <param name="ticket">The import task ticket returned when creating the task.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response containing the import task status and result.</returns>
    public Task<LarkResponseBody> ConvertDocsFileFormatStateAsync(string ticket, CancellationToken cancellationToken = default)
        => GetAsync(string.Concat(LarkUrls.ConvertDocsFileFormatState, ticket), cancellationToken);

    /// <summary>
    /// Converts content into document blocks without inserting them into a document.
    /// </summary>
    /// <param name="mime">The content type accepted by the conversion API, or null to use Markdown.</param>
    /// <param name="content">The source content to convert.</param>
    /// <param name="options">The optional user identifier type request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The converted blocks and their top level block identifiers.</returns>
    public Task<LarkResponseBody<LarkDocsMarkdownConvertResponse>> ConvertDocsBlocksAsync(string? mime, string content, LarkUserIdTypeRequestOptions? options = null, CancellationToken cancellationToken = default)
        => PostAsync<LarkDocsMarkdownConvertResponse>(LarkUrls.ToUrl(LarkUrls.ConvertDocsBlocks, options), new()
        {
            { "content_type", mime ?? "markdown" },
            { "content", content },
        }, cancellationToken);

    /// <summary>
    /// Gets the whiteboard nodes.
    /// </summary>
    /// <param name="id">The whiteboard identifier, or the whiteboard reference token in docs tree block.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The whiteboard nodes.</returns>
    [Description("Get the whiteboard nodes.")]
    public Task<LarkResponseBody> GetDocsWhiteboardNodesAsync([Description("The whiteboard identifier, or the whiteboard reference token in docs tree block.")] string id, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.DocsWhiteboardNodes, id), cancellationToken);

    /// <summary>
    /// Gets the whiteboard nodes.
    /// </summary>
    /// <param name="id">The whiteboard identifier, or the whiteboard reference token in docs tree block.</param>
    /// <param name="options">The additional options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The whiteboard nodes.</returns>
    public Task<LarkResponseBody> GetDocsWhiteboardNodesAsync(string id, LarkUserIdTypeRequestOptions? options, CancellationToken cancellationToken = default)
        => GetAsync(LarkUrls.ToUrl(LarkUrls.DocsWhiteboardNodes, options, id), cancellationToken);

    /// <summary>
    /// Gets the whiteboard nodes.
    /// </summary>
    /// <param name="ids">The whiteboard identifiers, or the whiteboard reference tokens in docs tree block.</param>
    /// <param name="doc">The docs tree block.</param>
    /// <param name="options">The additional options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The whiteboard nodes.</returns>
    public async IAsyncEnumerable<LarkDocWhiteboardInstanceInfo> GetDocsWhiteboardNodesAsync(IEnumerable<string> ids, LarkContentBlockTree? doc, LarkUserIdTypeRequestOptions? options, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (ids is null) yield break;
        var dict = doc?.Resources?.Whiteboards;
        if (dict is null)
        {
            dict = [];
            if (doc is not null)
            {
                doc.Resources ??= new();
                doc.Resources.Whiteboards = dict;
            }
        }

        foreach (var id in ids)
        {
            if (string.IsNullOrWhiteSpace(id) || dict.ContainsKey(id)) continue;
            var nodes = await GetDocsWhiteboardNodesAsync(id, options, cancellationToken);
            if (nodes?.Data is null || nodes.IsError) continue;
            var col = nodes.Data.TryGetObjectListValue("nodes", true);
            if (col is null || col.Count < 1) continue;
            var info = new LarkDocWhiteboardInstanceInfo
            {
                Id = id,
                Nodes = [],
            };
            foreach (var item in col)
            {
                info.Nodes.Add(LarkApiUtils.SimplifyWhiteboard(item));
            }

            dict[id] = info;
            yield return info;
        }
    }

    /// <summary>
    /// Gets the whiteboard nodes.
    /// </summary>
    /// <param name="ids">The whiteboard identifiers, or the whiteboard reference tokens in docs tree block.</param>
    /// <param name="options">The additional options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The whiteboard nodes.</returns>
    public IAsyncEnumerable<LarkDocWhiteboardInstanceInfo> GetDocsWhiteboardNodesAsync(IEnumerable<string> ids, LarkUserIdTypeRequestOptions? options, CancellationToken cancellationToken = default)
        => GetDocsWhiteboardNodesAsync(ids, null, options, cancellationToken);

    /// <summary>
    /// Gets the whiteboard as image.
    /// </summary>
    /// <param name="id">The whiteboard identifier, or the whiteboard reference token in docs tree block.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The whiteboard screenshot.</returns>
    [Description("Get the whiteboard as an image.")]
    public async Task<Stream> GetDocsWhiteboardAsImageAsync([Description("The whiteboard identifier, or the whiteboard reference token in docs tree block..")] string id, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient<Stream>();
        var resp = await http.GetAsync(LarkUrls.ToUrl(LarkUrls.DocsWhiteboardImage, id), cancellationToken);
        return resp;
    }

    /// <summary>
    /// Adds child blocks and their descendants under a document block.
    /// </summary>
    /// <param name="documentId">The document identifier (doc token).</param>
    /// <param name="blockId">The parent block identifier, or blank to use the document root block.</param>
    /// <param name="blockChildrenIds">The ordered identifiers of the direct child blocks to insert.</param>
    /// <param name="blockIndex">The insertion index, or -1 to append. Batched requests always append.</param>
    /// <param name="descendants">The block definitions for the child blocks and their descendants.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The insertion response, or the last batch response when multiple requests are required.</returns>
    /// <remarks>When more than 1,000 block definitions are supplied, the method splits the insertion into batches.</remarks>
    public async Task<LarkResponseBody> AddDocsBlocksAsync(string documentId, string? blockId, List<string> blockChildrenIds, int blockIndex, List<JsonObjectNode> descendants, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(documentId)) return new(true, "The document identifier (doc token) should not be empty.");
        if (blockChildrenIds is null) return new(true, "The block children identifiers should not be null.");
        if (descendants is null) return new(true, "The block items should not be null.");
        if (string.IsNullOrWhiteSpace(blockId)) blockId = documentId;
        if (descendants.Count <= 1000) return await PostAsync(LarkUrls.ToUrl(LarkUrls.AddDocsBlocks, documentId, blockId), new()
        {
            { "children_id", blockChildrenIds },
            { "index", blockIndex },
            { "descendants", descendants },
        }, cancellationToken);

        var added = new List<string>();
        var size = 200;
        LarkResponseBody? resp = null;
        for (var i = 0; i < blockChildrenIds.Count; i += size)
        {
            var blockIds = blockChildrenIds.Skip(i).Take(size).Where(ele => !string.IsNullOrWhiteSpace(ele)).ToList();
            var part = new List<JsonObjectNode>();
            var childrenIds = new List<string>(blockIds);
            added.AddRange(blockIds);
            while (childrenIds.Count > 0)
            {
                var col = new List<string>(childrenIds);
                childrenIds.Clear();
                foreach (var id in col)
                {
                    foreach (var json in descendants)
                    {
                        var block = json?.TryGetStringTrimmedValue("block_id", true);
                        if (block != id) continue;
                        part.Add(json!);
                        var children = json!.TryGetStringListValue("children", true);
                        if (children is not null && children.Count > 0)
                        {
                            foreach (var child in children)
                            {
                                if (added.Contains(child)) continue;
                                childrenIds.Add(child);
                            }
                        }

                        var parent = json.TryGetStringTrimmedValue("parent_id", true);
                        if (parent is not null && !added.Contains(parent))
                        {
                            childrenIds.Add(parent);
                        }

                        break;
                    }
                }

                added.AddRange(childrenIds);
            }

            resp = await PostAsync(LarkUrls.ToUrl(LarkUrls.AddDocsBlocks, documentId, blockId), new()
            {
                { "children_id", blockIds },
                { "index", -1 },
                { "descendants", part },
            }, cancellationToken);
            if (resp?.Data is null || resp.IsError) return resp ?? new(true, "The count of descendants is out of range.");
        }

        return resp ?? new(true, "The count of descendants is out of range.");
    }

    /// <summary>
    /// Appends child blocks and their descendants under a document block.
    /// </summary>
    /// <param name="documentId">The document identifier (doc token).</param>
    /// <param name="blockId">The parent block identifier, or blank to use the document root block.</param>
    /// <param name="blockChildrenIds">The ordered identifiers of the direct child blocks to append.</param>
    /// <param name="descendants">The block definitions for the child blocks and their descendants.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The insertion response, or the last batch response when multiple requests are required.</returns>
    public Task<LarkResponseBody> AddDocsBlocksAsync(string documentId, string? blockId, List<string> blockChildrenIds, List<JsonObjectNode> descendants, CancellationToken cancellationToken = default)
        => AddDocsBlocksAsync(documentId, blockId, blockChildrenIds, -1, descendants, cancellationToken);

    /// <summary>
    /// Inserts converted content blocks under a document block.
    /// </summary>
    /// <param name="documentId">The document identifier (doc token).</param>
    /// <param name="blockId">The parent block identifier, or blank to use the document root block.</param>
    /// <param name="markdown">The converted content containing block definitions and top level block identifiers.</param>
    /// <param name="blockIndex">The insertion index, or -1 to append. Batched requests always append.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The insertion response, or the last batch response when multiple requests are required.</returns>
    public Task<LarkResponseBody> AddDocsBlocksAsync(string documentId, string? blockId, LarkDocsMarkdownConvertResponse markdown, int blockIndex, CancellationToken cancellationToken = default)
        => AddDocsBlocksAsync(documentId, blockId, markdown.TopBlockIds, blockIndex, markdown.Blocks, cancellationToken);
 
    /// <summary>
    /// Appends converted content blocks under a document block.
    /// </summary>
    /// <param name="documentId">The document identifier (doc token).</param>
    /// <param name="blockId">The parent block identifier, or blank to use the document root block.</param>
    /// <param name="markdown">The converted content containing block definitions and top level block identifiers.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The insertion response, or the last batch response when multiple requests are required.</returns>
    public Task<LarkResponseBody> AddDocsBlocksAsync(string documentId, string? blockId, LarkDocsMarkdownConvertResponse markdown, CancellationToken cancellationToken = default)
        => AddDocsBlocksAsync(documentId, blockId, markdown.TopBlockIds, -1, markdown.Blocks, cancellationToken);

    /// <summary>
    /// Converts Markdown to blocks and inserts them into a wiki document without replacing existing content.
    /// </summary>
    /// <param name="nodeToken">The wiki node token or URL of the document.</param>
    /// <param name="markdown">The nonempty Markdown content to insert.</param>
    /// <param name="blockIndex">The insertion index in the document root block, or -1 to append. Batched requests always append.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The block insertion response, or an error response if conversion or node lookup fails.</returns>
    public async Task<LarkResponseBody> UpdateDocsAsync(string nodeToken, string markdown, int blockIndex, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(markdown)) return new(true, "The markdown is empty.");
        var blocksTask = ConvertDocsBlocksAsync("markdown", markdown, null, cancellationToken);
        var nodeInfo = await GetWikiNodeAsync(nodeToken, cancellationToken);
        var blocks = await blocksTask;
        if (string.IsNullOrWhiteSpace(nodeInfo?.Data?.DocToken) || nodeInfo.IsError)
            return new(true, nodeInfo?.Message);
        if (blocks?.Data is null || blocks.IsError)
            return new(true, blocks?.Message);
        var resp = await AddDocsBlocksAsync(nodeInfo.Data.DocToken, nodeInfo.Data.DocToken, blocks.Data.TopBlockIds, blockIndex, blocks.Data.Blocks, cancellationToken);
        return resp;
    }

    /// <summary>
    /// Converts Markdown to blocks and appends them to a wiki document without replacing existing content.
    /// </summary>
    /// <param name="nodeToken">The wiki node token or URL of the document.</param>
    /// <param name="markdown">The nonempty Markdown content to append.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The block insertion response, or an error response if conversion or node lookup fails.</returns>
    public Task<LarkResponseBody> UpdateDocsAsync(string nodeToken, string markdown, CancellationToken cancellationToken = default)
        => UpdateDocsAsync(nodeToken, markdown, -1, cancellationToken);

    /// <summary>
    /// Deletes a range of child blocks from a document block.
    /// </summary>
    /// <param name="documentId">The document identifier (doc token).</param>
    /// <param name="blockId">The parent block identifier.</param>
    /// <param name="start">The zero-based inclusive start index of the child blocks to delete.</param>
    /// <param name="end">The zero-based exclusive end index of the child blocks to delete.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The block deletion response.</returns>
    public Task<LarkResponseBody> DeleteDocsBlocksAsync(string documentId, string blockId, int start, int end, CancellationToken cancellationToken = default)
        => DeleteDocsBlocksAsync(documentId, blockId, null, start, end, cancellationToken);

    /// <summary>
    /// Deletes a range of child blocks from a document block with an optional client token.
    /// </summary>
    /// <param name="documentId">The document identifier (doc token).</param>
    /// <param name="blockId">The parent block identifier.</param>
    /// <param name="clientToken">The optional client token sent with the deletion request.</param>
    /// <param name="start">The zero-based inclusive start index of the child blocks to delete.</param>
    /// <param name="end">The zero-based exclusive end index of the child blocks to delete.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The block deletion response.</returns>
    public async Task<LarkResponseBody> DeleteDocsBlocksAsync(string documentId, string blockId, string? clientToken, int start, int end, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var query = string.IsNullOrEmpty(clientToken) ? null : new QueryData
        {
            { "client_token", clientToken },
        };
        var resp = await http.SendJsonAsync(HttpMethod.Delete, LarkUrls.ToUrl(LarkUrls.DeleteDocsBlocks, query, documentId, blockId), new JsonObjectNode
        {
            { "start_index", start },
            { "end_index", end },
        }, cancellationToken);
        return new(resp);
    }

    /// <summary>
    /// Clears the content of a wiki document by deleting the child blocks of its page block.
    /// </summary>
    /// <param name="token">The wiki node token or URL of the document.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The deletion response, a successful empty response if there are no child blocks, or a lookup error response.</returns>
    public async Task<LarkResponseBody> DeleteDocsContentAsync(string token, CancellationToken cancellationToken = default)
    {
        var nodeToken = GetWikiNodeAsync(token, cancellationToken);
        var blocks = await GetDocsBlocksAsync(token, true, cancellationToken);
        var node = await nodeToken;
        if (blocks?.Data is null || blocks.IsError) return blocks ?? new(true, "Cannot get the blocks of the doc.");
        if (string.IsNullOrWhiteSpace(node?.Data?.DocToken) || node.IsError) return node ?? new(true, "Cannot get the doc node info.");
        var pageBlock = blocks.Data.GetPageOrFirst();
        if (string.IsNullOrWhiteSpace(pageBlock?.Id)) return new(true, "Cannot get the page block.");
        if (pageBlock.ChildIds is null || pageBlock.ChildIds.Count < 1) return new(false, "Empty.");
        var resp = await DeleteDocsBlocksAsync(node.Data.DocToken, pageBlock.Id, 0, pageBlock.ChildIds.Count, cancellationToken);
        return resp;
    }

    /// <summary>
    /// Reads the content of the specific online doc.
    /// </summary>
    /// <param name="token">The node token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The doc content.</returns>
    [Description("Read the content of the specific online doc.")]
    public async Task<LarkDocContent> GetDocsNodeContentAsync([Description("The URL or node token (not the doc token nor file token) of the online doc.")] string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token)) return LarkApiUtils.ErrorLarkDocContent(null, "The node token is null.");
        token = LarkUrls.GetId(token)!;
        var info = await GetWikiNodeAsync(token, cancellationToken);
        if (string.IsNullOrWhiteSpace(info?.Data?.NodeToken) || info.IsError) return LarkApiUtils.ErrorLarkDocContent(token, info?.Message ?? "Get node failed.");
        return await LarkApiUtils.GetDocsNodeContentAsync(this, info, cancellationToken);
    }

    /// <summary>
    /// Creates a version of a document.
    /// </summary>
    /// <param name="docToken">The document token.</param>
    /// <param name="docType">The document object type.</param>
    /// <param name="name">The optional version name.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The response of the version creation request.</returns>
    public Task<LarkResponseBody> AddDocsFileVersion(string docToken, string docType, string? name = null, CancellationToken cancellationToken = default)
    {
        var json = new JsonObjectNode
        {
            { "obj_type", docType }
        };
        json.SetValueIfNotEmpty("name", name);
        return PostAsync(LarkUrls.ToUrl(LarkUrls.DocsVersions, docToken), json, cancellationToken);
    }

    /// <summary>
    /// Lists a page of document versions.
    /// </summary>
    /// <param name="options">The document token, object type, and additional request options.</param>
    /// <param name="paging">The page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The document versions for the requested page.</returns>
    public Task<LarkResponsePagingBody> ListDocsFileVersion(LarkDocsDocTokenRequest options, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.DocsVersions, options.DocToken), options, paging, cancellationToken);

    /// <summary>
    /// Lists a page of versions of the specified document.
    /// </summary>
    /// <param name="docToken">The document token.</param>
    /// <param name="docType">The document object type.</param>
    /// <param name="paging">The page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The document versions for the requested page.</returns>
    public Task<LarkResponsePagingBody> ListDocsFileVersion(string docToken, string docType, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.DocsVersions, docToken), new LarkDocsDocTokenRequest(docToken, docType), paging, cancellationToken);

    /// <summary>
    /// Loads the next page of document versions and adds it to the previous response.
    /// </summary>
    /// <param name="resp">The previous paging response containing the document token request options.</param>
    /// <param name="pageSize">The page size for the next request.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The document versions loaded from the next page.</returns>
    public Task<IReadOnlyList<JsonObjectNode>> ListDocsFileVersion(LarkResponsePagingBody resp, int pageSize, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.ToUrl(LarkUrls.DocsVersions, (resp.Query as LarkDocsDocTokenRequest)?.DocToken), resp, pageSize, cancellationToken);
}
