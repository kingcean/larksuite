using LarkSuite;
using LarkSuite.CommandLine;
using System.Drawing;
using Trivial.CommandLine;

LinearGradientConsoleStyle linear = new(ConsoleColor.Cyan, Color.FromArgb(0x36, 0x70, 0xfa), Color.FromArgb(0x3d, 0xd4, 0xb9));
DefaultConsole.WriteLine(linear, "LarkSuite");
DefaultConsole.WriteLine();
DefaultConsole.Write(ConsoleColor.DarkGray, "Loading…");
await LarkApi.DefaultInstance.GetTenantTokenAsync();
DefaultConsole.Clear(StyleConsole.RelativeAreas.Line);
DefaultConsole.BackspaceToBeginning();
var dispatcher = new CommandDispatcher();

dispatcher.Register<LarkDocsCommandVerb>("docs");
dispatcher.Register<LarkHireCommandVerb>("hire");
dispatcher.Register<LarkUsersCommandVerb>("users");

await dispatcher.ProcessAsync();
