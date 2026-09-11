using System;
using System.Collections.Generic;
using System.Text;
using Trivial.Net;

namespace LarkSuite;

internal static partial class LarkUrls
{
    public const string UserInfo = "https://open.feishu.cn/open-apis/contact/v3/users/batch";
    public const string SearchUser = "https://open.feishu.cn/open-apis/search/v1/user";
    public const string UserId = "https://open.feishu.cn/open-apis/contact/v3/users/batch_get_id";
    public const string SearchCompanyDepartments = "https://open.feishu.cn/open-apis/directory/v1/departments/search";
    public const string ListCompanyDepartments = "https://open.feishu.cn/open-apis/directory/v1/departments/filter";
    public const string GetCompanyDepartments = "https://open.feishu.cn/open-apis/directory/v1/departments/mget";
    public const string SearchEmployees = "https://open.feishu.cn/open-apis/corehr/v2/employees/search";
    public const string ListEmployees = "https://open.feishu.cn/open-apis/directory/v1/employees/filter";
    public const string GetEmployees = "https://open.feishu.cn/open-apis/corehr/v2/employees/batch_get";
}
