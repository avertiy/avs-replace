using AVS.CoreLib.Console.ColorFormatting.Extensions;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole;
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
        //var path = context.WorkingDirectory;
        //path = path.Contains("..") ? Path.GetFullPath(context.WorkingDirectory): path;
        //PowerConsole.Print($"Found {context.Files?.Count} file(s) and {context.Directories?.Count} folder(s) at {path}");
        PowerConsole.WriteLine();
        PrintDirectories(context);
        PowerConsole.WriteLine();
        PrintFiles(context);
	}

	public void PrintFiles(SearchContext context)
	{
        if (context.Files == null || context.Files.Count == 0)
		{
            PowerConsole.Print($"No files matching <Cyan>`{context.SearchText}`</Cyan>:");
            return;
        }

        PowerConsole.Print($"Found #{context.Files.Count} files matching <Cyan>`{context.SearchText}`</Cyan>:");

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
				PowerConsole.Print($"\r\n{currentDir}", ConsoleColor.DarkYellow);
			}

			var message = $"{i+1}) {Highlight(fileInfo.Name, searchText, CTag.Cyan)}";
			PowerConsole.Print(message);
		}
	}

	public void PrintDirectories(SearchContext context)
	{
        if (context.Directories == null || context.Directories.Count == 0)
        {
            PowerConsole.Print($"No directories matching <Cyan>`{context.SearchText}`</Cyan>:");
            return;
        }

		PowerConsole.Print($"Found #{context.Directories.Count} directories matching <Cyan>`{context.SearchText}`</Cyan>:");
		var searchText = context.SearchText;
		var len = searchText.Length;
		foreach (var dir in context.Directories)
		{
			var message = Highlight(dir, searchText, CTag.Cyan);
			PowerConsole.Print(message);			
		}
	}

	private string Highlight(string str, string matchText, CTag tag)
	{
        return str.Replace(matchText, matchText.WrapInTag(tag));
	}
}