using AVS.Replace.Enums;
using System.Diagnostics;
using System.Text;

namespace AVS.Replace;

[DebuggerDisplay("{ToString()}")]
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

    public override string ToString()
    {
		var sb = new StringBuilder();
        if (UserMode != UserMode.Default)
			sb.Append($"mode:{UserMode} ");

        if (CaseSensitive)
            sb.Append("Case Sensitive ");

        if (Regexp)
            sb.Append("RegExp ");

        sb.Append($"depth:{Depth} ");
        sb.Append($"file types:{FileTypes} ");
        sb.Append($"exclude:{Exclude} ");

        return sb.ToString();
    }
}

public enum UserMode
{
    Default = 0,
    Verbose,
    Silent
}
