using LarkSuite;
using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text;
using System.Text.Json;
using Trivial.CommandLine;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.CommandLine;

public class LarkUsersCommandVerb : BaseCommandVerb
{
    public static string Description => "Get user info and org info.";

    protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }
}
