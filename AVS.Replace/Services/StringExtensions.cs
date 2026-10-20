using AVS.CoreLib.Extensions;
using System.Text;
using System.Text.RegularExpressions;
namespace AVS.Replace.Services;

public static class StringExtensions
{
	public static string GetLineByOffset(this StringBuilder sb, int index, int maxLength = 100)
	{
		if (sb.Length < index)
			return string.Empty;

		var start = sb.LastIndexOf(Environment.NewLine, index, false);

		if (start == -1)
			start = 0;
		else
			start += Environment.NewLine.Length;
			
		var end = sb.IndexOf(Environment.NewLine, index, false);

		if (end == -1)
			end = sb.Length;

		var length = end - start;

		if (length > maxLength)
		{
            length = maxLength;
			if (index - start > maxLength / 2)
				start = index - maxLength / 2;
        }

		var line = sb.ToString(start, length);
		return line;
	}
	public static IEnumerable<int> IndexOfAllOccurrences(this string str, string value)
	{
		var ind = str.IndexOf(value, StringComparison.InvariantCulture);
		while (ind > -1)
		{
			yield return ind;
			ind += value.Length;
			ind = str.IndexOf(value, ind, StringComparison.InvariantCulture);
		}
	}

	public static string TrimLine(this string line, string subString, int maxLength = 200)
	{
		if (line.Length <= maxLength)
			return line;

		var index = line.IndexOf(subString);

		if (index < maxLength / 2)
			return line.Substring(0, maxLength) + "..";

		var startIndex = index - maxLength / 2;
		var endIndex = index + subString.Length;

		if (startIndex + maxLength > endIndex)
			endIndex = startIndex + maxLength;

        var length = Math.Min(endIndex - startIndex, line.Length - startIndex);
		return ".." + line.Substring(startIndex, length) + "..";
    }
}