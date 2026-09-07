using System;
using System.Collections.Generic;
using System.Text;
using Trivial.Net;

namespace LarkSuite;

internal static partial class LarkUrls
{
    public const string SendMessage = "https://open.feishu.cn/open-apis/im/v1/messages";
    public const string CreateMessageCard = "https://open.feishu.cn/open-apis/cardkit/v1/cards/";
    public const string UpdateMessageCard = "https://open.feishu.cn/open-apis/cardkit/v1/cards/{0}/elements/{1}/content";
    public const string UpdateMessageCardSettings = "https://open.feishu.cn/open-apis/cardkit/v1/cards/{0}/settings";
    public const string DownloadMessageImageUploaded = "https://open.feishu.cn/open-apis/im/v1/images/";
    public const string DownloadMessageFileUploaded = "https://open.feishu.cn/open-apis/im/v1/files/";
    public const string DownloadMessageFile = "https://open.feishu.cn/open-apis/im/v1/messages/{0}/resources/{1}";
}
