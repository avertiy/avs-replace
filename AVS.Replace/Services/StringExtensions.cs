using System.Text;
using AVS.CoreLib.Extensions;
namespace AVS.Replace.Services;

public static class StringExtensions
{
	public static string GetLineByOffset(this StringBuilder sb, int index)
	{
		if (sb.Length < index)
			return string.Empty;

		//var start = index > 100 ? index - 100 : 0;

		var start = sb.LastIndexOf(Environment.NewLine, index, false)+ Environment.NewLine.Length;

		if (start == -1)
			return String.Empty;

		var end = sb.IndexOf(Environment.NewLine, index, false);
		if (end == -1)
			end = index + 20 < sb.Length ? index + 20 : sb.Length;

		var line = sb.ToString(start, end - start);
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
}