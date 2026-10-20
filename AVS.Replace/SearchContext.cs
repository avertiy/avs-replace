using AVS.CoreLib.PowerConsole;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AVS.Replace;

//[DebuggerDisplay("{ToString()}")]
public class SearchContext
{
	public string WorkingDirectory { get; set; } = ".";
	public string SearchText { get; set; } = "";
	public string? Replace { get; set; }
	public SearchMode Mode { get; set; }

	public SearchOptions Options { get; set; } = new SearchOptions();

	/// <summary>
	/// holds file paths (file name or content match search criteria)
	/// </summary>
	public List<FileInfo>? Files { get; set; }

	/// <summary>
	/// holds dir paths that match search criteria
	/// </summary>
	public List<string>? Directories { get; set; }
	
	public required Regex Regexp { get; set; }

    public void Print()
	{
        string text = Mode switch
        {
            SearchMode.QuickScan => $"quick scan <Green>\"{SearchText}\"</Green>",
            SearchMode.Replace => $"<Green>\"{SearchText}\"</Green> replace with <Cyan>\"{Replace}\"</Cyan>",
            _ => $"search <Green>\"{SearchText}\"</Green>",
        };
        PowerConsole.Print(text);
        //PowerConsole.Print("Search Options:");
        PowerConsole.PrintF($"Working dir:<Blue>\"{WorkingDirectory}\"</Blue> depth:<Blue>{Options.Depth}</Blue>");
        PowerConsole.PrintF($"CaseSensitive:<Blue>{Options.CaseSensitive}</Blue> Regexp:<Blue>{Options.Regexp}</Blue>");
        PowerConsole.PrintF($"File types:<Cyan>{Options.FileTypes}</Cyan> <DarkGray>(exclude:{Options.Exclude})</DarkGray>");

    }
}

public enum SearchMode
{
    /// <summary>
    /// scan filesystem and file contents
    /// </summary>
    Default = 0,
	/// <summary>
	/// scan only filesystem without file contents
	/// </summary>
	QuickScan = 1,
	/// <summary>
	/// rename files / directories, replace text in file contents
	/// </summary>
	Replace =3,


	//CopyPaste
}