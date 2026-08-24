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
    public Task<LarkResponseBody<LarkDocsFolderMetaInfo>> GetDocsDriveMetaAsync(string token, CancellationToken cancellationToken = default)
        => GetAsync<LarkDocsFolderMetaInfo>(LarkUrls.ToUrl(LarkUrls.GetDriveFolder, token), cancellationToken);

    public Task<LarkResponseBody<LarkDocsDriveMetaInfo>> GetDocsDriveMetaAsync(DocsDriveFilesRequest options, CancellationToken cancellationToken = default)
        => GetAsync<LarkDocsDriveMetaInfo>(LarkUrls.GetDriveRoot, cancellationToken);

    public Task<LarkResponsePagingBody<LarkDocsDriveNodeInfo>> ListDocsDriveFilesAsync(DocsDriveFilesRequest options, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkDocsDriveNodeInfo>(LarkUrls.GetDriveFiles, options, "files", paging, null, cancellationToken);

    public Task<LarkResponsePagingBody<LarkDocsDriveNodeInfo>> ListDocsDriveFilesAsync(string token, LarkPageTokenInfo? paging = null, CancellationToken cancellationToken = default)
        => ListDocsDriveFilesAsync(new DocsDriveFilesRequest
        {
            Token = token,
        }, paging, cancellationToken);

    public Task<LarkResponsePagingBody<LarkDocsDriveNodeInfo>> ListDocsDriveFilesAsync(CancellationToken cancellationToken = default)
        => ListDocsDriveFilesAsync(new DocsDriveFilesRequest(), new LarkPageTokenInfo(), cancellationToken);

    public Task<IReadOnlyList<LarkDocsDriveNodeInfo>> ListDocsDriveNodesAsync(LarkResponsePagingBody<LarkDocsDriveNodeInfo> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.GetDriveFiles, response, "files", pageSize, cancellationToken);

    public Task<LarkResponseBody<BaseLarkTaskInfo>> MoveDocsDriveFileAsync(DocsDriveFileMoveRequest options, CancellationToken cancellationToken = default)
        => PostAsync<BaseLarkTaskInfo>(LarkUrls.ToUrl(LarkUrls.GetDriveFolder, options.Token), JsonObjectNode.ConvertFrom(options), cancellationToken);
}
