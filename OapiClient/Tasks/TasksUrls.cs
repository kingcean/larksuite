using System;
using System.Collections.Generic;
using System.Text;

namespace LarkSuite;

internal static partial class LarkUrls
{
    public const string OkrPeriods = "https://open.feishu.cn/open-apis/okr/v2/cycles";
    public const string OkrObjectives = "https://open.feishu.cn/open-apis/okr/v2/cycles/{0}/objectives";
    public const string OkrObjectiveDetails = "https://open.feishu.cn/open-apis/okr/v2/objectives/{0}";
    public const string OkrObjectiveProgress = "https://open.feishu.cn/open-apis/okr/v2/objectives/{0}/progresses";
    public const string OkrKeyResults = "https://open.feishu.cn/open-apis/okr/v2/objectives/{0}/key_results";
    public const string OkrKeyResultDetails = "https://open.feishu.cn/open-apis/okr/v2/key_results/{0}";
    public const string OkrKeyResultProgress = "https://open.feishu.cn/open-apis/okr/v2/key_results/{0}/progresses";
}
