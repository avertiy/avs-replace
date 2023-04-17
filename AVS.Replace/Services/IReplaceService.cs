using System.Text;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.PowerConsole;
using AVS.CoreLib.Text.Extensions;
using AVS.Replace.Helpers;

namespace AVS.Replace.Services;

public interface IReplaceService
{
	void ProcessFile(string path, SearchContext context);
}


public class ReplaceService : IReplaceService
{
	public void ProcessFile(string path, SearchContext context)
	{
		var (content, encoding) = FileHelper.ReadToEnd(path);

		if (!content.Contains(context.SearchText))
			return;

		var output = ProcessContent(content, path, context);
		File.WriteAllText(path, output, encoding);
	}

	private string ProcessContent(string content, string path, SearchContext context)
	{
		if (context.Options.Regexp)
		{
			PowerConsole.Print("Regexp is not supported yet", ConsoleColor.DarkRed);
			return content;
		}
		else
		{
			if (context.Options.UserMode == UserMode.Silent)
			{
				var output = content.Replace(context.SearchText, context.Replace);
				PowerConsole.Print($"{path} - done", ConsoleColor.Cyan);
				return output;
			}

			var indexes = content.IndexOfAllOccurrences(context.SearchText).ToArray();
			PowerConsole.Print($"Processing {path} found #{indexes.Length} occurrences of `{context.SearchText}`");


			var sb = new StringBuilder(content);
			var oldValue = context.SearchText;
			var newValue = context.Replace;
			
			foreach (var index in indexes)
			{
				var line = sb.GetLineByOffset(index);
				var lineIndex = sb.GetLineIndexOf(index);
				var ind = line.IndexOf(oldValue);
				PowerConsole.Print($"[{lineIndex}]{line} contains `{oldValue}` at {ind} => `{newValue}`", ConsoleColor.Yellow);
				sb.Replace(oldValue, newValue, index, newValue.Length);
				var newLine = sb.GetLineAt(lineIndex);
				PowerConsole.Print($"[{lineIndex}]`{line}` => `{newLine}` - done", ConsoleColor.Green);
			}

			return sb.ToString();
		}
	}
}