using AVS.CoreLib.PowerConsole.Utilities;
using AVS.Replace.Enums;
using CommandLine;

namespace AVS.Replace;


public class SearchContext
{
	public string WorkingDirectory { get; set; } = ".";
	public string SearchText { get; set; } = "";
	public string? Replace { get; set; }
	public SearchMode SearchMode { get; set; }

	public SearchOptions Options { get; set; } = new SearchOptions();

	/// <summary>
	/// holds file paths (file name or content match search criteria)
	/// </summary>
	public List<FileInfo>? Files { get; set; }

	/// <summary>
	/// holds dir paths that match search criteria
	/// </summary>
	public List<string>? Directories { get; set; }

	public static SearchContext FromInputArgs(Input args)
	{
		var ctx = new SearchContext
		{
			SearchText = args.SearchText,
			WorkingDirectory = args.WorkingDirectory,
			Options =
			{
				CaseSensitive = args.CaseSensitive,
				Regexp = args.Regexp,
				FileTypes = args.FileTypes,
				Exclude = args.Exclude,
				Depth = args.Depth
			},
			Replace = args.ReplaceText
		};
		//etc.
		return ctx;
	}
}

public class SearchOptions
{
	public string FileTypes { get; set; } = "*.*";
	public string Exclude { get; set; } = ".git";
	public FileTypeClass ExcludeFileClasses { get; set; } = FileTypeClass.AllNonText;
	public int MaxLength { get; set; } = 102_400;
	public bool Regexp { get; set; }
	public bool CaseSensitive { get; set; } = true;
	public bool ImageFiles { get; set; } = false;
	public bool MediaFiles { get; set; } = false;
	
	public UserMode UserMode { get; set; }
	public int Depth { get; set; } = 3;
}

public enum SearchMode
{
	/// <summary>
	/// quick scan 
	/// </summary>
	FileSearch = 0,
	TextSearch = 1,
	Replace,
	CopyPaste
}

public enum UserMode
{
	Default = 0,
	Verbose,
	Silent
}


