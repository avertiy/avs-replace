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

        if (Directory.Exists(path))
		{
            var counter = ProcessDirectory(path, context);
        }
		else
		{
            PowerConsole.Print($"Path `{path}` does not exist", ConsoleColor.Red);
        }
		return Task.CompletedTask;
	}

	private int ProcessDirectory(string path, SearchContext context, int depth = 0)
	{		
        PowerConsole.WriteLine(false);
        PowerConsole.PrintF($"=========== Start processing <Blue>{path}</Blue> ============");

        var counter = ProcessFiles(path, context);
		if (counter > 0)
			PowerConsole.PrintF($"<Green>{path} <Blue>#{counter}</Blue> files have been processed</Green>");

        foreach (var directory in _searchFileService.EnumerateTopDirectories(path))
		{
            if (context.Options.Depth > 0 && depth < context.Options.Depth)
                counter += ProcessDirectory(directory, context, depth + 1);
        }

        RenameDir(path, context);
        
        PowerConsole.WriteLine(false);
        return counter;
	}

	private string RenameDir(string path, SearchContext context)
	{
        try
		{
            var dirName = Path.GetFileName(path);

			if (!_matchService.MatchFileName(dirName))
				return path;

            var len = context.SearchText.Length;
			var rootPath = Path.GetDirectoryName(path);

			if (rootPath == null)
			{
				PowerConsole.Print($"Unable to get directory name from {path} - skipped", ConsoleColor.Red);
				return path;
			}

			var newName = dirName.Replace(context.SearchText, context.Replace);

			if (dirName == newName)
				return path;

			var newPath = Path.Combine(rootPath, newName);

			if (Directory.Exists(newPath))
			{
				PowerConsole.Print($"Directory `{newName}` already exists at {rootPath}.", ConsoleColor.Red);
				return path;
			}

			Directory.Move(path, newPath);
			PowerConsole.PrintF($"Directory `{rootPath}\\<Green>{dirName}</Green>` has been renamed to `{rootPath}\\<Cyan>{newName}</Cyan>`");
			return newPath;
		}
		catch (Exception ex)
		{
			PowerConsole.PrintError(ex, $"Unable to rename directory: {path}", false);
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
