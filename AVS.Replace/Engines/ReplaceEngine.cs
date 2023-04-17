using System.IO;
using System.Text;
using AVS.CoreLib.PowerConsole;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.Replace.Helpers;
using AVS.Replace.Services;
using ConsoleColor = System.ConsoleColor;

namespace AVS.Replace.Engines;

public class ReplaceEngine : AbstractEngine
{
	private readonly ISearchFileService _searchFileService;
	private readonly IMatchService _matchService;
	private readonly IReplaceService _replaceService;
	private SearchContext? _context;

	public ReplaceEngine(IAbstractFactory factory)
	{
		_searchFileService = factory.CreateSearchFileService();
		_matchService = factory.CreateMatchService();
		_replaceService = factory.CreateReplaceService();
	}

	public override Task ExecuteAsync(SearchContext context)
	{
		_context = context;
		_searchFileService.Init(context);
		_matchService.Init(context);
		var path = _context.WorkingDirectory;
		ProcessDirectory(path);
		return Task.CompletedTask;
	}

	private void ProcessDirectory(string path, int depth = 0)
	{
		ProcessFiles(path);
		foreach (var directory in _searchFileService.EnumerateTopDirectories(path))
		{
			var dir = directory;
			if (_matchService.MatchFileName(directory))
			{
				dir = RenameDirectory(directory);
			}

			if (_context.Options.Depth > 0 && depth < _context.Options.Depth)
				ProcessDirectory(dir, depth + 1);
		}
	}

	private string RenameDirectory(string path)
	{
		try
		{
			var len = _context.SearchText.Length;
			var rootPath = Path.GetDirectoryName(path);

			if (rootPath == null)
			{
				PowerConsole.Print($"Unable to get directory name from {path} - skipped", ConsoleColor.Red);
				return path;
			}

			var name = Path.GetFileName(path);

			var newName = name.Replace(_context.SearchText, _context.Replace);
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
			PowerConsole.PrintError(ex, $"Unable to rename directory: {path} - skipped", false, PrintOptions.NoTimestamp);
			return path;
		}
	}

	private void ProcessFiles(string path)
	{
		try
		{
			var files = _searchFileService.EnumerateFiles(path);
			foreach (var file in files)
			{
				var fileInfo = new FileInfo(file);
				if (fileInfo.Length > _context.Options.MaxLength || fileInfo.IsReadOnly)
					continue;

				_replaceService.ProcessFile(file, _context);

				if (_matchService.MatchFileName(fileInfo.Name))
				{
					RenameFile(fileInfo);
				}
			}
		}
		catch (Exception ex)
		{
			PowerConsole.PrintError(ex, $"Processing files at `{path}` failed", false, PrintOptions.NoTimestamp);
		}
	}

	private void RenameFile(FileInfo fileInfo)
	{
		var name = fileInfo.Name;
		try
		{
			var len = _context.SearchText.Length;
			var newname = name.Replace(_context.SearchText, _context.Replace);

			if (_context.Options.UserMode == UserMode.Verbose)
			{
				//for now no colorizing
				//PowerConsole.Write(name.Substring(0, index), ConsoleColor.White);
				//PowerConsole.Write(name.Substring(index, len), ConsoleColor.Yellow);
				//PowerConsole.Print(name.Substring(index + len), ConsoleColor.Yellow);
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
			PowerConsole.Print($"File \"{name}\" => \"{newname}\" - done", ConsoleColor.Green);
		}
		catch (Exception ex)
		{
			PowerConsole.PrintError(ex, $"Unable to rename file: {name} - skipped", false, PrintOptions.NoTimestamp);			
		}
	}
}
