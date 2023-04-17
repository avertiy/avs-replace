using System.Dynamic;
using AVS.Replace;

namespace AVS.Replace.Services;

public interface ISearchFileService
{
	void Init(SearchContext context);
	IEnumerable<string> EnumerateFiles(string path);
	IEnumerable<string> EnumerateTopDirectories(string path);
}

public class SearchFileService : ISearchFileService
{
	//private SearchContext _context;
	private string[] Skip;
	private string[] SkipFileTypes;
	private string[] AllowedFileTypes;
	private bool AllFiles;
	private long FileSizeLimit;
	private MatchCasing CaseSensitive = MatchCasing.CaseInsensitive;

	public void Init(SearchContext context)
	{
		CaseSensitive = context.Options.CaseSensitive ? MatchCasing.CaseSensitive : MatchCasing.CaseInsensitive;
		var arr = context.Options.Exclude.Split(';', StringSplitOptions.RemoveEmptyEntries);
		Skip = arr.Where(x => !x.StartsWith('*')).ToArray();

		var skipFileTypes = new List<string>(arr.Where(x => x.StartsWith('*')).Select(x => x.Substring(1)));
		if (!context.Options.ImageFiles)
			skipFileTypes.AddRange(GetImageFileTypes());

		if (!context.Options.MediaFiles)
			skipFileTypes.AddRange(GetMediaFileTypes());

		SkipFileTypes = skipFileTypes.ToArray();
		if (context.Options.FileTypes == "*.*")
		{
			AllFiles = SkipFileTypes.Length == 0;
			AllowedFileTypes = Array.Empty<string>();
		}
		else
			AllowedFileTypes = context.Options.FileTypes.Split(';', StringSplitOptions.RemoveEmptyEntries);


		FileSizeLimit = context.Options.MaxLength;
	}

	public IEnumerable<string> EnumerateTopDirectories(string path)
	{
		//var options = new EnumerationOptions()
		//{
		//	MaxRecursionDepth = 1,
		//	IgnoreInaccessible = true,
		//	MatchCasing = _options.CaseSensitive ? MatchCasing.CaseSensitive : MatchCasing.CaseInsensitive,
		//	MatchType = MatchType.Win32,
		//	AttributesToSkip = GetAttributesToSkip(false)
		//};
		//return Directory.EnumerateDirectories(path,"*", options);
		return Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
	}

	public IEnumerable<string> EnumerateFiles(string path)
	{
		var options = new EnumerationOptions()
		{
			MaxRecursionDepth = 1,
			IgnoreInaccessible = true,
			MatchCasing = CaseSensitive,
			MatchType = MatchType.Win32,
			AttributesToSkip = GetAttributesToSkip(true)
		};

		foreach (var entry in Directory.EnumerateFileSystemEntries(path, "*.*", options))
		{
			if(Skip.Contains(entry) || entry =="." || entry =="..")
				continue;
			
			if (!AllFiles && entry.Contains('.'))
			{
				var ext = Path.GetExtension(entry);

				if(SkipFileTypes.Contains(ext))
					continue;
				
				//*.cs or .cs
				if(AllowedFileTypes.Length >0 && AllowedFileTypes.All(x => x.StartsWith('*') ? x.Substring(1) != ext : x != ext))
					continue;
			}

			yield return entry;
		} 
	}

	private static FileAttributes GetAttributesToSkip(bool skipDirectory = true)
	{
		var attrs = FileAttributes.Hidden |
		            FileAttributes.ReadOnly |
		            FileAttributes.Encrypted |
		            FileAttributes.Offline |
		            FileAttributes.SparseFile |
		            FileAttributes.System |
		            FileAttributes.Temporary;
		if (skipDirectory)
			return attrs | FileAttributes.Directory;
		return attrs;
	}

	private static string[] GetImageFileTypes()
	{
		return new[] { ".jpg", ".jpeg", ".tiff", ".png", ".ico", ".bmp", ".png", ".gif" };
	}

	private static string[] GetMediaFileTypes()
	{
		return new[] { ".mp3", ".mp4" };
	}
}

