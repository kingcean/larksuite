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
}
