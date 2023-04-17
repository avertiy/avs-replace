using AVS.Replace.Enums;
using AVS.Replace.Services;

namespace AVS.Replace.Core.Queries.SearchFiles
{
	public class SearchFilesQueryHandler : BaseHandler //: IRequestHandler<SearchFilesQuery, SearchFilesViewModel>
	{
		private readonly IFileSystemService _fileSystemService;

		public SearchFilesQueryHandler(IFileSystemService fileSystemService)
		{
			_fileSystemService = fileSystemService;
		}

		public async Task<SearchFilesResult> Handle(SearchFilesQuery query, CancellationToken cancellationToken)
		{
			var result = await ScanFileSystem(query, cancellationToken);
			return result;
		}

		private async Task<SearchFilesResult> ScanFileSystem(SearchFilesQuery query, CancellationToken cancellationToken)
		{
			var searchText = query.SearchText;
			var path = query.WorkingDirectory;

			var scanOptions = GetScanOptions(query);

			ReportProgress("Scanning file system..", 0);

			var entries = await _fileSystemService.ScanAsync(path, scanOptions, cancellationToken);

			ReportProgress($"Scanning file system.. #{entries.Length} entries to process", 10);

			
			var files = new List<FileInfo>();
			var directories = new List<string>();
			for (var i = 0; i < entries.Length; i++)
			{
				if(cancellationToken.IsCancellationRequested)
					break;

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

			var result = new SearchFilesResult()
			{
				Files = files,
				Directories = directories
			};

			return result;

		}

		private ScanOptions GetScanOptions(SearchFilesQuery query)
		{
			var scanOptions = new ScanOptions()
			{
				TargetEntry = TargetEntry.FilesAndDirs,
				Depth = query.Options.Depth,
				MatchCasing = query.Options.CaseSensitive ? MatchCasing.CaseSensitive : MatchCasing.CaseInsensitive,
			};

			var arr = query.Options.Exclude.Split(';', StringSplitOptions.RemoveEmptyEntries);
			var excludeDirs = arr.Where(x => !x.StartsWith('*')).ToArray();
			scanOptions.ExcludeDirectories.AddRange(excludeDirs);


			if (arr.Length > excludeDirs.Length)
				scanOptions.ExcludeFileTypes.AddRange(arr.Where(x => x.StartsWith('*')).Select(x => x.Substring(1)));

			var excludeFileTypes = query.Options.ExcludeFileClasses.GetFileTypes();
			scanOptions.ExcludeFileTypes.AddRange(excludeFileTypes);

			return scanOptions;
		}
	}
}
