using LarkSuite;
using LarkSuite.CommandLine;
using LarkSuite.OapiModels;
using System.Buffers.Text;
using System.Drawing;
using Trivial.CommandLine;

LinearGradientConsoleStyle linear = new(ConsoleColor.Cyan, Color.FromArgb(0x36, 0x70, 0xfa), Color.FromArgb(0x3d, 0xd4, 0xb9));
DefaultConsole.WriteLine(linear, "LarkSuite");
DefaultConsole.WriteLine();

if (LarkApi.DefaultInstance.IsAppKeyEmpty)
{
    DefaultConsole.WriteLine("Need use an app key to access the resource.");
    DefaultConsole.Write("App ID: \t");
    var s = DefaultConsole.ReadLine();
    if (string.IsNullOrWhiteSpace(s))
    {
        DefaultConsole.Write("App ID: \t");
        s = DefaultConsole.ReadLine();
        if (string.IsNullOrWhiteSpace(s)) return;
    }

    DefaultConsole.Write("App Secret: \t");
    var secret = DefaultConsole.ReadPassword('*');
    if (string.IsNullOrWhiteSpace(s))
    {
        DefaultConsole.Write("App Secret: \t");
        secret = DefaultConsole.ReadPassword();
        if (string.IsNullOrWhiteSpace(s)) return;
    }

    LarkApiUtils.ReplaceDefaultInstance(new(s, secret));
    DefaultConsole.WriteLine();
}

DefaultConsole.Write(ConsoleColor.DarkGray, "Loading…");
var token = await LarkApi.DefaultInstance.GetTenantTokenAsync();
DefaultConsole.Clear(StyleConsole.RelativeAreas.Line);
DefaultConsole.BackspaceToBeginning();
if (token is null || token.IsEmpty)
{
    DefaultConsole.Write(ConsoleColor.Red, "Login failed.");
    DefaultConsole.Write(" \t");
    DefaultConsole.WriteLine(token?.Message);
    DefaultConsole.WriteLine();
    return;
}

var dispatcher = new CommandDispatcher();
dispatcher.Register<LarkDocsCommandVerb>("docs");
dispatcher.Register<LarkHireCommandVerb>("hire");
dispatcher.Register<LarkUsersCommandVerb>("users");
dispatcher.Register<LarkOkrCommandVerb>("okr");

await dispatcher.ProcessAsync();
