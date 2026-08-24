using System;
using System.Collections.Generic;
using System.Text;
using Trivial.Net;

namespace LarkSuite;

internal static partial class LarkUrls
{
    public const string SearchWiki = "https://open.feishu.cn/open-apis/wiki/v2/nodes/search";
    public const string WikiSpaces = "https://open.feishu.cn/open-apis/wiki/v2/spaces";
    public const string WikiSpaceInfo = "https://open.feishu.cn/open-apis/wiki/v2/spaces/{0}";
    public const string WikiSpaceNodes = "https://open.feishu.cn/open-apis/wiki/v2/spaces/{0}/nodes";
    public const string WikiSpaceMembers = "https://open.feishu.cn/open-apis/wiki/v2/spaces/{0}/members";
    public const string GetWikiNode = "https://open.feishu.cn/open-apis/wiki/v2/spaces/get_node";
    public const string DocsInfo = "https://open.feishu.cn/open-apis/docx/v1/documents/";
    public const string DocsText = "https://open.feishu.cn/open-apis/docx/v1/documents/{0}/raw_content";
    public const string DocsBlocks = "https://open.feishu.cn/open-apis/docx/v1/documents/{0}/blocks";
    public const string DocsMarkdown = "https://open.feishu.cn/open-apis/docs/v1/content";
    public const string DocsBoardNodes = "https://open.feishu.cn/open-apis/board/v1/whiteboards/{0}/nodes";
    public const string DownloadFile = "https://open.feishu.cn/open-apis/drive/v1/files/{0}/download";
    public const string GetBaseTable = "https://open.feishu.cn/open-apis/bitable/v1/apps/";
    public const string ListBaseTableDashboards = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/dashboards";
    public const string ListBaseTableTables = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables";
    public const string ListBaseTableViews = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables/{1}/views";
    public const string GetBaseTableView = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables/{1}/views/{2}";
    public const string GetBaseTableRecords = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables/{1}/records/batch_get";
    public const string RenameBaseTable = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables/{1}";
    public const string GetBaseTableFields = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables/{1}/fields";
    public const string ReadBaseTable = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables/{1}/records/search";
    public const string InsertBaseTableRecord = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables/{1}/records";
    public const string InsertBaseTableRecords = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables/{1}/records/batch_create";
    public const string UpdateBaseTableRecord = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables/{1}/records/{2}";
    public const string DeleteBaseTableRecords = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables/{1}/records/batch_delete";
    public const string UploadFile = "https://open.feishu.cn/open-apis/drive/v1/files/upload_all";
    public const string ConvertDocsFileFormat = "https://open.feishu.cn/open-apis/drive/v1/import_tasks";
    public const string ConvertDocsFileFormatState = "https://open.feishu.cn/open-apis/drive/v1/import_tasks/";
    public const string ConvertDocsBlocks = "https://open.feishu.cn/open-apis/docx/v1/documents/blocks/convert";
    public const string AddDocsBlocks = "https://open.feishu.cn/open-apis/docx/v1/documents/{0}/blocks/{1}/descendant";
    public const string DeleteDocsBlocks = "https://open.feishu.cn/open-apis/docx/v1/documents/{0}/blocks/{1}/children/batch_delete";
    public const string GetDriveRoot = "https://open.feishu.cn/open-apis/drive/explorer/v2/root_folder/meta";
    public const string GetDriveFolder = "https://open.feishu.cn/open-apis/drive/explorer/v2/folder/{0}/meta";
    public const string GetDriveFiles = "https://open.feishu.cn/open-apis/drive/v1/files";
    public const string MoveDriveFile = "https://open.feishu.cn/open-apis/drive/v1/files/{0}/move";
    public const string DocsComments = "https://open.feishu.cn/open-apis/drive/v1/files/{0}/comments";
    public const string DocsReplyComment = "https://open.feishu.cn/open-apis/drive/v1/files/{0}/comments/{1}/replies";
    public const string DocsWhiteboardNodes = "https://open.feishu.cn/open-apis/board/v1/whiteboards/{0}/nodes";
    public const string DocsWhiteboardImage = "https://open.feishu.cn/open-apis/board/v1/whiteboards/{0}/download_as_image";
}
