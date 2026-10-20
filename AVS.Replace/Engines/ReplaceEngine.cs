using AVS.CoreLib.PowerConsole;
using AVS.CoreLib.PowerConsole.Printers2;
using AVS.Replace.Services;
using ConsoleColor = System.ConsoleColor;

namespace AVS.Replace.Engines;

public class ReplaceEngine : AbstractEngine
{
	private readonly ISearchFileService _searchFileService;
	private readonly IMatchService _matchService;
	private readonly IReplaceService _replaceService;

	public ReplaceEngine(IAbstractFactory factory)
	{
		_searchFileService = factory.CreateSearchFileService();
		_matchService = factory.CreateMatchService();
		_replaceService = factory.CreateReplaceService();
	}

	public override Task ExecuteAsync(SearchContext context)
	{
		_searchFileService.Init(context);
		_matchService.Init(context);
		var path = context.WorkingDirectory;
		var counter = ProcessDirectory(path, context);
		return Task.CompletedTask;
	}

	private int ProcessDirectory(string path, SearchContext context, int depth = 0)
	{
		var counter = ProcessFiles(path, context);
		foreach (var directory in _searchFileService.EnumerateTopDirectories(path))
		{
			var dir = directory;
			if (_matchService.MatchFileName(directory))
			{
				dir = RenameDirectory(directory, context);
			}

			if (context.Options.Depth > 0 && depth < context.Options.Depth)
                counter += ProcessDirectory(dir, context, depth + 1);
		}

		if (counter > 0)
            PowerConsole.PrintF($"<Cyan>{path}</Cyan> - #{counter} files have been processed");
        return counter;
	}

	private string RenameDirectory(string path, SearchContext context)
	{
		try
		{
			var len = context.SearchText.Length;
			var rootPath = Path.GetDirectoryName(path);

			if (rootPath == null)
			{
				PowerConsole.Print($"Unable to get directory name from {path} - skipped", ConsoleColor.Red);
				return path;
			}

			var name = Path.GetFileName(path);

			var newName = name.Replace(context.SearchText, context.Replace);
			var fullPath = Path.Combine(rootPath, newName);

			if (Directory.Exists(fullPath))
			{
				PowerConsole.Print($"Directory `{newName}` already exists at {rootPath}.");
			}

			Directory.Move(path, fullPath);
			PowerConsole.Print($"{rootPath}: directory {name} => {newName} - done", ConsoleColor.Green);
			return fullPath;
		}
		catch (Exception ex)
		{
			PowerConsole.PrintError(ex, $"Unable to rename directory: {path} - skipped", false, PrintOptions2.Inline);
			return path;
		}
	}

	private int ProcessFiles(string path, SearchContext context)
	{
        var counter = 0;
        try
		{
			var files = _searchFileService.EnumerateFiles(path);
			foreach (var file in files)
			{
				var fileInfo = new FileInfo(file);
				if (fileInfo.Length > context.Options.MaxLength || fileInfo.IsReadOnly)
					continue;

				_replaceService.ProcessFile(file, context);
				counter++;
                if (_matchService.MatchFileName(fileInfo.Name))
				{
					RenameFile(fileInfo, context);
				}
			}
		}
		catch (Exception ex)
		{
			PowerConsole.PrintError(ex, $"Processing files at `{path}` failed", false, PrintOptions2.Inline);
		}
		return counter;
	}

	private void RenameFile(FileInfo fileInfo, SearchContext context)
	{
		var name = fileInfo.Name;
		try
		{
			var len = context.SearchText.Length;
			var newname = name.Replace(context.SearchText, context.Replace);

			if (context.Options.UserMode == UserMode.Verbose)
			{
				var confirmation = PowerConsole.PromptYesNo($"Confirm file renaming \"{name}\" => \"{newname}\"", 0);
				if (!confirmation)
					return;
			}

			var fullPath = Path.Combine(fileInfo.DirectoryName, newname);
			if (File.Exists(fullPath))
			{
				var confirmation = PowerConsole
					.PromptYesNo($"file with name {newname} already exists, do you want to overwrite it?", 0);
				if (!confirmation)
					return;
			}

			fileInfo.MoveTo(fullPath, true);
			PowerConsole.Print($"File \"{name}\" has been renamed \"{newname}\" successfully", ConsoleColor.Green);
		}
		catch (Exception ex)
		{
			PowerConsole.PrintError(ex, $"Unable to rename file: {name} - skipped", false, PrintOptions2.Inline);			
		}
	}
}
