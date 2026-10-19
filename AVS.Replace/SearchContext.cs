using System.Diagnostics;

namespace AVS.Replace;

[DebuggerDisplay("{ToString()}")]
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

    public override string ToString()
    {
		if (Replace == null)
		{
			return $"search text:\"{SearchText}\", dir:{WorkingDirectory}\r\noptions:{Options}";
        }
		else
		{
            return $"search text:\"{SearchText}\" replace with:\"{Replace}\", dir:\"{WorkingDirectory}\"\r\noptions:{Options}";
        }
    }
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