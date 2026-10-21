using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Collections;
using AVS.CoreLib.PowerConsole;
using AVS.CoreLib.Text.Extensions;
using AVS.Replace.Helpers;
using System.Text;
using System.Text.RegularExpressions;

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
        var options = context.Options;

        string output;
        var oldValue = context.SearchText;
        var newValue = context.Replace!;

        var lowerCaseOldValue = oldValue.ToLower();
        var lowerCaseNewValue = newValue.ToLower();

        var regex = context.Regexp;
        var stringComparison = options.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

        if (context.Options.UserMode == UserMode.Silent)
        {
            output = content.Replace(oldValue, newValue);
            PowerConsole.Print($"{path} - DONE", ConsoleColor.Cyan);
            return output;
        }

        PowerConsole.WriteLine();
        PowerConsole.PrintF($"<Cyan>\"{path}\"</Cyan>:");

        var lines = content.Split(["\r\n","\n"]);
        var lineIndexes = new List<int>();

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var ind = line.IndexOf(oldValue, stringComparison);

            if (ind == -1)
                continue;

            lineIndexes.Add(i);
            line = line.TrimLine(oldValue, maxLength: 1000);
            lines[i] = line;

            line = line.Replace(oldValue, $"<Green>{oldValue}</Green>");

            if (!options.CaseSensitive)
                line = line.Replace(lowerCaseOldValue, $"<Green>{lowerCaseOldValue}</Green>");

            PowerConsole.PrintF($"Line[{i}] {line}");
        }

        PowerConsole.WriteLine(false);
        PowerConsole.PrintF($"Replacing <Green>`{oldValue}`</Green> => <Cyan>`{newValue}`</Cyan>..");
        PowerConsole.WriteLine(false);

        output = content.Replace(oldValue, newValue);

        if (!options.CaseSensitive)
            output = output.Replace(lowerCaseOldValue, lowerCaseNewValue);

        for (var i = 0; i < lineIndexes.Count; i++)
        {
            var index = lineIndexes[i];
            var line = lines[index];
            line = line.Replace(oldValue, $"<Cyan>{newValue}</Cyan>");

            if (!options.CaseSensitive)
                line = line.Replace(lowerCaseOldValue, $"<Cyan>{lowerCaseNewValue}</Cyan>");

            PowerConsole.PrintF($"Line[{index}] {line}");
        }

        PowerConsole.WriteLine(false);
        PowerConsole.PrintF($"<Cyan>\"{path}\"</Cyan> - DONE");
        PowerConsole.WriteLine(false);

        return output;
    }
}