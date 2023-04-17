using AVS.CoreLib.Console.ColorFormatting.Extensions;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole;
using AVS.CoreLib.PowerConsole.Printers;

namespace AVS.Replace.Services;

public interface IPrinter
{
	void PrintSearchResults(SearchContext context);
	void PrintFiles(SearchContext context);
	void PrintDirectories(SearchContext context);
}
public class ConsolePrinter : IPrinter
{
	public void PrintSearchResults(SearchContext context)
	{
		var path = context.WorkingDirectory;
		path = path.Contains("..") ? Path.GetFullPath(context.WorkingDirectory): path;

		PowerConsole.Print($"Search results: found {context.Files?.Count} file(s) and {context.Directories?.Count} directories.");
		PowerConsole.Print($"Search path: {path}");


		PrintDirectories(context);
		PrintFiles(context);
	}

	public void PrintFiles(SearchContext context)
	{
		if (context.Files == null)
			return;
		PowerConsole.WriteLine();
		//core-lib does not support CTags yet in switch color mode, SwitchColorOutputWriter needs to be completed
		//PowerConsole.Print($"Found #{context.Files.Count} files matching <Cyan>`{context.SearchText}`</Cyan>:", PrintOptions.CTags());
		PowerConsole.Write($"Found #{context.Files.Count} files matching``:");
		PowerConsole.Write(context.SearchText, ConsoleColor.Cyan);
		PowerConsole.WriteLine("`:");

		var searchText = context.SearchText;
		var len = searchText.Length;

		var di = new DirectoryInfo(context.WorkingDirectory);
		var rootPath = di.FullName;
		var dirName = rootPath;// Path.GetFileName(rootPath);
		for (var i = 0; i < context.Files.Count; i++)
		{
			var fileInfo = context.Files[i];
			var dir = fileInfo.DirectoryName;
			if (dirName != dir)
			{
				dirName = dir;
				var currentDir = dir.Replace(rootPath, ".");
				PowerConsole.Print($"\r\n{currentDir}", CTag.DarkYellow);
			}

			var message = $"{i+1}) {Highlight(fileInfo.Name, searchText, CTag.Cyan)}";
			PowerConsole.Print(message, PrintOptions.CTags());
		}
	}

	public void PrintDirectories(SearchContext context)
	{
		if (context.Directories == null)
			return;

		PowerConsole.Print($"Found #{context.Directories.Count} directories matching <Cyan>`{context.SearchText}`</Cyan>:", PrintOptions.CTags());
		var searchText = context.SearchText;
		var len = searchText.Length;
		foreach (var dir in context.Directories)
		{
			var message = Highlight(dir, searchText, CTag.Cyan);
			PowerConsole.Print(message, PrintOptions.CTags());			
		}
	}

	private string Highlight(string str, string matchText, CTag tag)
	{
		return str.Replace(matchText, matchText.WrapInTag(tag));
	}
}