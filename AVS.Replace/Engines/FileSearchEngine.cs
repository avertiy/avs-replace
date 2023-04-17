using AVS.CoreLib.PowerConsole;
using AVS.Replace.Enums;
using AVS.Replace.Services;

namespace AVS.Replace.Engines;

public class FileSearchEngine : AbstractEngine
{
	private readonly IFileSystemService _fileSystemService;
	private readonly IPrinter _printer;

	public FileSearchEngine(IFileSystemService fileSystemService, IPrinter printer)
	{
		_fileSystemService = fileSystemService;
		_printer = printer;
	}

	public override async Task ExecuteAsync(SearchContext context)
	{
		await ScanFileSystem(context);
		_printer.PrintSearchResults(context);
	}

	private async Task ScanFileSystem(SearchContext context)
	{
		var path = context.WorkingDirectory;
		var scanOptions = GetScanOptions(context);

		ReportProgress("Scanning file system..", 0);

		var entries = await _fileSystemService.ScanAsync(path, scanOptions);

		ReportProgress($"Scanning file system.. #{entries.Length} entries to process", 10);

		var searchText = context.SearchText;
		var files = new List<FileInfo>();
		var directories = new List<string>();
		for (var i = 0; i < entries.Length; i++)
		{
			var entry = entries[i];
			var name = Path.GetFileName(entry);
			if (!name.Contains(searchText))
				continue;

			var progress = i * 100 / entries.Length;
			if (progress > 10 && progress % 10 == 0)
			{
				ReportProgress($"Scanning file system.. #{entries.Length - i} entries to process", progress);
			}
			// get the file attributes for file or directory
			//var attr = File.GetAttributes(entry);
			var fi = new FileInfo(entry);
			if (fi.Attributes.HasFlag(FileAttributes.Directory))
				directories.Add(entry);
			else
				files.Add(fi);
		}

		ReportProgress($"Scan completed: files #{files.Count}; directories #{directories.Count}", 100);
		context.Files = files;
		context.Directories = directories;
	}

	private ScanOptions GetScanOptions(SearchContext context)
	{
		var scanOptions = new ScanOptions()
		{
			TargetEntry = TargetEntry.FilesAndDirs,
			Depth = context.Options.Depth,
			MatchCasing = context.Options.CaseSensitive ? MatchCasing.CaseSensitive : MatchCasing.CaseInsensitive,
		};

		var arr = context.Options.Exclude.Split(';', StringSplitOptions.RemoveEmptyEntries);
		var excludeDirs = arr.Where(x => !x.StartsWith('*')).ToArray();
		scanOptions.ExcludeDirectories.AddRange(excludeDirs);
		

		if(arr.Length > excludeDirs.Length)
			scanOptions.ExcludeFileTypes.AddRange(arr.Where(x => x.StartsWith('*')).Select(x => x.Substring(1)));

		var excludeFileTypes = context.Options.ExcludeFileClasses.GetFileTypes();
		scanOptions.ExcludeFileTypes.AddRange(excludeFileTypes);

		return scanOptions;
	}

}
