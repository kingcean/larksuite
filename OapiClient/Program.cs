using LarkSuite;
using LarkSuite.CommandLine;
using LarkSuite.OapiModels;
using System.Buffers.Text;
using System.Drawing;
using System.Text;
using Trivial.Collection;
using Trivial.CommandLine;

var console = StyleConsole.Default;
LinearGradientConsoleStyle linear = new(ConsoleColor.Cyan, Color.FromArgb(0x36, 0x70, 0xfa), Color.FromArgb(0x3d, 0xd4, 0xb9))
{
    Bold = true
};
console.WriteLine(linear, "LarkSuite");
console.WriteLine();

var larkApi = LarkApi.DefaultInstance;
if (larkApi.IsAppKeyEmpty)
{
    console.WriteLine("Need use an app key to access the resource.");
    console.Write("App ID: \t");
    var s = console.ReadLine();
    if (string.IsNullOrWhiteSpace(s))
    {
        console.Write("App ID: \t");
        s = console.ReadLine();
        if (string.IsNullOrWhiteSpace(s)) return;
    }

    console.Write("App Secret: \t");
    var secret = console.ReadPassword('*');
    if (string.IsNullOrWhiteSpace(s))
    {
        console.Write("App Secret: \t");
        secret = console.ReadPassword();
        if (string.IsNullOrWhiteSpace(s)) return;
    }

    larkApi = new(s, secret);
    LarkApiUtils.ReplaceDefaultInstance(larkApi);
    console.WriteLine();
}

console.Write(ConsoleColor.DarkGray, "Loading…");
var token = await larkApi.GetTenantTokenAsync();
console.Clear(StyleConsole.RelativeAreas.Line);
console.BackspaceToBeginning();
if (token is null || token.IsEmpty)
{
    console.Write(ConsoleColor.Red, "Login failed.");
    console.Write(" \t");
    console.WriteLine(token?.Message);
    console.WriteLine();
    return;
}

var info = await larkApi.GetBotInfoAsync();
if (!(info?.Data?.Name is null || info.IsError))
{
    console.Append(info.Data.Name);
    console.Append(" \t");
    console.WriteLine(ConsoleColor.DarkGray, info.Data.OpenId);
    console.WriteLine();
}

var dispatcher = new CommandDispatcher();
dispatcher.Register<LarkDocsCommandVerb>("docs");
dispatcher.Register<LarkHireCommandVerb>("hire");
dispatcher.Register<LarkUsersCommandVerb>("users");
dispatcher.Register<LarkOkrCommandVerb>("okr");

await dispatcher.ProcessOrSelectAsync(LarkCliUtils.GetMenuSelectionOptions());
