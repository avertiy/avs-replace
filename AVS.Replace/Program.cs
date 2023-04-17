/*
 search&replace command util find and replace text analogue of Ctrl+Shift+H

 User stories:
 - I'd like to search in files (skip node_modules, bin, .git etc. dirs and not relevant file types)
 - I'd like to search & replace text in files (including replace in file & dir names i.e. rename in file system)
 - I'd like to copy-paste + replace (i.e. when i need to do something by analogy with existing just renaming class name for example)
 - I'd like to have a quick search by default (limit depth i.e. i don't want to wait 10 mins while all sub folders will be scanned, 
algorithm might prioritize scan small folders & small files first, don't waste time on scanning big files or run a deep scan in background showing user quickly fast search results)
 
 */

using AVS.CoreLib.PowerConsole;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.Replace;
using AVS.Replace.Services;

//args
//-w - working directory
//-f - find text
//-r - replace with
//-i - interactive means confirm y/n each replacement
//-s - skip files/folders, the defaults are ".git;.vs;bin;obj;"
//-t - files types pattern e.g. "*.cs;*.html"
//-l - file size limit, by default skip files whose size > 100kb
//-r - treat find text as regexp

//>search Text -w .\test -T text|code

//args
//args = new[] { "-w", ".\\test", "-v","-s" };

//ansi colors strangely not working when using the tool as intalled cmd tool
PowerConsole.SwitchColorMode(ColorMode.AnsiCodes);

PowerConsole.WriteLine("test standard color");
PowerConsole.Print("<Cyan>test colors output</Cyan>", PrintOptions.CTags());
PowerConsole.WriteLine("test standard color");

// switch colors printer work but it does not support ctags at the moment..
PowerConsole.SwitchColorMode(ColorMode.Default);
PowerConsole.WriteLine("test standard color");
PowerConsole.Print("<Cyan>test colors output</Cyan>", PrintOptions.CTags());
PowerConsole.Foreground = ConsoleColor.DarkRed;
PowerConsole.Print("test dark red");
PowerConsole.ColorSchemeReset();
PowerConsole.WriteLine("test standard color");
Console.WriteLine("");

var input = args.Length > 0 ? Input.ParseArgs(args) : Input.CommandLine("AVS");

if (input == null)
	return;

SearchContext context = SearchContext.FromInputArgs(input);

var factory = new DefaultFactory();
var engine = factory.GetEngine(context.SearchMode);

await engine.ExecuteAsync(context);

