using LarkSuite.Docs;
using LarkSuite.OapiModels;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
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
    /// Gets the metadata of a drive folder.
    /// </summary>
    /// <param name="token">The folder token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The drive folder metadata.</returns>
    public Task<LarkResponseBody<LarkDocsFolderMetaInfo>> GetDocsDriveMetaAsync(string token, CancellationToken cancellationToken = default)
        => GetAsync<LarkDocsFolderMetaInfo>(LarkUrls.ToUrl(LarkUrls.GetDriveFolder, token), cancellationToken);

    /// <summary>
    /// Gets the metadata of the drive root.
    /// </summary>
    /// <param name="options">The drive file request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The drive root metadata.</returns>
    public Task<LarkResponseBody<LarkDocsDriveMetaInfo>> GetDocsDriveMetaAsync(LarkDocsDriveFilesRequest options, CancellationToken cancellationToken = default)
        => GetAsync<LarkDocsDriveMetaInfo>(LarkUrls.GetDriveRoot, cancellationToken);

    /// <summary>
    /// Lists a page of files in a drive folder using the specified request options.
    /// </summary>
    /// <param name="options">The drive folder token and file listing request options.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The drive files for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkDocsDriveNodeInfo>> ListDocsDriveFilesAsync(LarkDocsDriveFilesRequest options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsDriveNodeInfo>(LarkUrls.GetDriveFiles, options, "files", paging, null, cancellationToken);

    /// <summary>
    /// Lists a page of files in a drive folder.
    /// </summary>
    /// <param name="token">The drive folder token.</param>
    /// <param name="paging">The optional page size and page token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The drive files for the requested page.</returns>
    public Task<LarkResponsePagingBody<LarkDocsDriveNodeInfo>> ListDocsDriveFilesAsync(string token, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => ListDocsDriveFilesAsync(new LarkDocsDriveFilesRequest
        {
            Token = token,
        }, paging, cancellationToken);

    /// <summary>
    /// Lists the first page of files in the drive root.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The first page of drive root files.</returns>
    public Task<LarkResponsePagingBody<LarkDocsDriveNodeInfo>> ListDocsDriveFilesAsync(CancellationToken cancellationToken = default)
        => ListDocsDriveFilesAsync(new LarkDocsDriveFilesRequest(), new LarkPageTokenInfo(), cancellationToken);

    /// <summary>
    /// Loads the next page of drive files and adds it to the previous response.
    /// </summary>
    /// <param name="response">The previous paging response containing the drive file request options.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The drive files loaded from the next page.</returns>
    public Task<IReadOnlyList<LarkDocsDriveNodeInfo>> ListDocsDriveFilesAsync(LarkResponsePagingBody<LarkDocsDriveNodeInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.GetDriveFiles, response, "files", pageSize, cancellationToken);

    /// <summary>
    /// Requests moving a drive file to another location in drive storage.
    /// </summary>
    /// <param name="options">The source file token and move destination options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The move task information.</returns>
    public Task<LarkResponseBody<BaseLarkTaskInfo>> MoveDocsDriveFileAsync(LarkDocsDriveFileMoveRequest options, CancellationToken cancellationToken = default)
        => PostAsync<BaseLarkTaskInfo>(LarkUrls.ToUrl(LarkUrls.MoveDriveFile, options.Token), JsonObjectNode.ConvertFrom(options), cancellationToken);

    /// <summary>
    /// Requests moving a drive file into a wiki space.
    /// </summary>
    /// <param name="options">The source drive file, destination wiki space, and target node options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The drive-to-wiki move task information.</returns>
    public Task<LarkResponseBody<LarkDocsDriveFileMoveTaskInfo>> MoveDocsDriveFileToWikiAsync(LarkDocsDriveFileMoveToWikiRequest options, CancellationToken cancellationToken = default)
        => PostAsync<LarkDocsDriveFileMoveTaskInfo>(LarkUrls.ToUrl(LarkUrls.MoveDriveFileToWiki, options.SpaceId), JsonObjectNode.ConvertFrom(options), cancellationToken);

    /// <summary>
    /// Requests moving a wiki node to drive storage.
    /// </summary>
    /// <param name="sourceNodeToken">The source wiki node token.</param>
    /// <param name="destinationFolderToken">The optional destination drive folder token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The move task information.</returns>
    public Task<LarkResponseBody<BaseLarkTaskInfo>> MoveDocsDriveFileFromWikiAsync(string sourceNodeToken, string? destinationFolderToken = null, CancellationToken cancellationToken = default)
        => PostAsync<BaseLarkTaskInfo>(LarkUrls.ToUrl(LarkUrls.MoveWikiNodeToDrive, sourceNodeToken), string.IsNullOrWhiteSpace(destinationFolderToken) ? [] : new JsonObjectNode()
        {
            { "folder_token", destinationFolderToken },
        }, cancellationToken);

    /// <summary>
    /// Deletes a drive file or folder.
    /// </summary>
    /// <param name="type">The resource type, or null to use <c>file</c>.</param>
    /// <param name="token">The file or folder token.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The deletion task response, or an error response if <paramref name="token"/> is blank.</returns>
    public async Task<LarkResponseBody<BaseLarkTaskInfo>> DeleteDocsDriveFileAsync(string? type, string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token)) return new(true, "The node token should not be empty.");
        var http = CreateJsonHttpClient();
        var q = new QueryData
        {
            { "type", type ?? "file" },
        };
        var url = q.ToString(string.Concat(LarkUrls.GetDriveFiles, token));
        var resp = await http.SendAsync(HttpMethod.Delete, url, cancellationToken);
        return new(resp);
    }
}
