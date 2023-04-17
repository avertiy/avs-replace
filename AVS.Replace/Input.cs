using CommandLine;
using CommandLine.Text;

namespace AVS.Replace;

/// <summary>
/// args:
/// [0] - search text
/// [1] - replace text [optional]
///-Q - quick scan mode
///-C - copy-paste mode
///
///-c - case sensitive
///-d - depth subdirectories
///-e - exclude files/folders, the defaults are ".git;.vs;bin;obj;"
///-f - files types pattern e.g. "*.cs;*.html"
///-i - interactive means confirm y/n each replacement
///-l - file size limit, by default skip files whose size > 100kb
///-r - regexp
///-s - silent  
///-v - verbose 
///-w - working directory
/// etc.
/// </summary>
public class Input
{
	[Option('C', "copy-paste mode", Required = false, Default = false, HelpText = "Set copy-paste mode to make copy-paste+replace operations.")]
	public bool CopyPaste { get; set; }

	[Option('Q', "quick scan mode", Required = false, Default = false, HelpText = "Set to quick scan.")]
	public bool QuickScan { get; set; }


	[Value(0, Required = true, HelpText = "Set search text.")]
	public string SearchText { get; set; } = string.Empty;

	[Value(1, Required = false, HelpText = "Set search text.")]
	public string? ReplaceText { get; set; }


	[Option('c', "case sensitive", Required = false, Default = false, HelpText = "Set case sensitive search, default case insensitive")]
	public bool CaseSensitive { get; set; }
	[Option('d', "depth", Required = false, Default = 3, HelpText = "Set depth to search in subdirectories, default is 3")]
	public int Depth { get; set; } = 3;

	[Option('e', "exclude", Required = false, Default = ".git;.vs;bin;obj;node_modules;*.exe;*.pdb;*.dll", HelpText = "Set exclude to skip files & directories, default `.git;.vs;bin;obj;*.exe;*.pdb;*.dll`.")]
	public string Exclude { get; set; } = ".git;.vs;bin;obj;node_modules;*.exe;*.pdb;*.dll";
	[Option('f', "file types", Required = false, Default = "*.*", HelpText = "Set file types pattern to search, default is `*.*`.")]
	public string FileTypes { get; set; } = "*.*";

	[Option('i', "interactive (dialogue)", Required = false, Default = false, HelpText = "Set interactive(dialogue) mode to confirm each replacement.")]
	public bool InteractiveMode { get; set; }

	[Option('w', "working directory", Required = false, Default = ".", HelpText = "Set working directory.")]
	public string WorkingDirectory { get; set; } = ".";

	[Option('v', "verbose mode", Required = false, Default = false, HelpText = "Set to process in verbose/interactive mode.")]
	public bool Verbose { get; set; }

	[Option('s', "silent mode", Required = false, Default = false, HelpText = "Set to process in silent mode.")]
	public bool Silent { get; set; } 

	[Option('r', "regexp", Required = false, Default = false, HelpText = "Set to treat search text as regexp.")]
	public bool Regexp { get; set; }

	[Option('m', "media and images files", Required = false, Default = false, HelpText = "Set to search among media and image files.")]
	public bool MediaFiles { get; set; }

	[Option('l', "max length", Required = false, Default = 102_400, HelpText = "Set file size limit, by default files with size > 100kb are skipped")]
	public int MaxLength { get; set; } = 102_400;

	public static Input? CommandLine(string args)
	{
		return ParseArgs(args.Split(' ', StringSplitOptions.RemoveEmptyEntries));
	}

	public static Input? ParseArgs(params string[] args)
	{
		var parserRes = Parser.Default.ParseArguments<Input>(args);
		return parserRes.Value;
	}
}