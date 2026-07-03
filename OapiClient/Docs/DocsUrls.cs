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
    public const string ListBaseTable = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables";
    public const string RenameBaseTable = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables/{1}";
    public const string ReadBaseTable = "https://open.feishu.cn/open-apis/bitable/v1/apps/{0}/tables/{1}/records/search";
}
