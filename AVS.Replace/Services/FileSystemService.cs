namespace AVS.Replace.Services;


public class ScanOptions
{
	public string SearchPattern { get; set; } = "*.*";
	public FileAttributes SkipAttributes { get; set; } = FileAttributes.Hidden |
	                                                     FileAttributes.ReadOnly |
	                                                     FileAttributes.Encrypted |
	                                                     FileAttributes.Offline |
	                                                     FileAttributes.SparseFile |
	                                                     FileAttributes.System |
	                                                     FileAttributes.Temporary;
	public int Depth { get; set; } = 1;
	public bool IgnoreInaccessible { get; set; } = true;
	public MatchCasing MatchCasing { get; set; } = MatchCasing.PlatformDefault;
	public MatchType MatchType { get; set; } = MatchType.Win32;
	public List<string> ExcludeDirectories { get; set; } = new List<string>() { ".", ".." };
	public List<string> ExcludeFileTypes { get; set; } = new List<string>();
	public TargetEntry TargetEntry { get; set; } = TargetEntry.FilesAndDirs;
	public EnumerationOptions GetEnumerationOptions()
	{
		var options = new EnumerationOptions()
		{
			MaxRecursionDepth = Depth,
			RecurseSubdirectories = Depth > 1,
			IgnoreInaccessible = IgnoreInaccessible,
			MatchCasing = MatchCasing,
			MatchType = MatchType,
			AttributesToSkip = SkipAttributes,

		};

		return options;
	}
}

[Flags]
public enum TargetEntry
{
	File = 1,
	Directory = 2,
	FilesAndDirs = File | Directory
}

public interface IFileSystemService
{
	IEnumerable<string> Scan(string path, ScanOptions options);
	Task<string[]> ScanAsync(string path, ScanOptions options, CancellationToken cancellationToken = default);
}

public class FileSystemService : IFileSystemService
{
	public IEnumerable<string> Scan(string path, ScanOptions options)
	{
		foreach (var entry in Directory.EnumerateFileSystemEntries(
			         path, options.SearchPattern, options.GetEnumerationOptions()))
		{
			//skip "node_modules" or "\\node_modules" or "\\node_modules\\";  
			if (options.ExcludeDirectories.Any(x => entry == x || entry.EndsWith("\\"+x) || entry.Contains($"\\{x}\\")))
				continue;

			var isDir = Directory.Exists(entry);

			if (isDir)
			{
				if (!options.TargetEntry.HasFlag(TargetEntry.Directory))
					continue;

				var dirName = Path.GetFileName(entry);
				if (options.ExcludeDirectories.Contains(dirName))
					continue;
			}
			else
			{
				if (!options.TargetEntry.HasFlag(TargetEntry.File))
					continue;

				if (entry.Contains('.'))
				{
					var ext = Path.GetExtension(entry).ToLower();

					if (options.ExcludeFileTypes.Contains(ext))
						continue;
				}
			}

			yield return entry;
		}
	}

	public Task<string[]> ScanAsync(string path, ScanOptions options, CancellationToken cancellationToken = default)
	{
		if (options.Depth == 1)
		{
			return Task.FromResult(Scan(path, options).ToArray());
		}

		var task = Task.Run(() => Scan(path, options).ToArray(), cancellationToken);
		return task;
	}
}