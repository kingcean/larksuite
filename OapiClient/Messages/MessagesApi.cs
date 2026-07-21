using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Trivial.Text;

namespace LarkSuite;

public partial class LarkApi
{
    public Task<LarkResponseBody<LarkMessageResponse>> SendMessage(LarkMessageRequest options, CancellationToken cancellationToken = default)
        => PostAsync<LarkMessageResponse>(LarkUrls.ToUrl(LarkUrls.SendMessage, options), JsonObjectNode.ConvertFrom(options), cancellationToken);
}
