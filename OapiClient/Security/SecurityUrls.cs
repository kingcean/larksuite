using System;
using System.Collections.Generic;
using System.Text;
using Trivial.Net;

namespace LarkSuite;

internal partial class LarkUrls
{
    public static readonly Uri tenantTokenUri = new("https://open.feishu.cn/open-apis/auth/v3/tenant_access_token/internal");
    public static readonly Uri userTokenUri = new("https://open.feishu.cn/open-apis/authen/v2/oauth/token");

    public static string GetAppKeyId()
        => Environment.GetEnvironmentVariable("LARK_OAPI_APP_ID");

    public static string GetAppKeySecret()
        => Environment.GetEnvironmentVariable("LARK_OAPI_APP_SECRET");
}
