using AVS.Replace;

namespace AVS.Replace.Services;

public interface IMatchService
{
	void Init(SearchContext context);
	bool MatchFileName(string fileName, out int index);
	bool MatchFileName(string fileName);
}

public class MatchService : IMatchService
{
	private string Text { get; set; } = string.Empty;
	private bool CaseSensitive { get; set; }
	
	public void Init(SearchContext context)
	{
		Text = context.SearchText;
		CaseSensitive = context.Options.CaseSensitive;
	}

	public bool MatchFileName(string fileName, out int index)
	{
		var strComparison = CaseSensitive
			? StringComparison.InvariantCulture
			: StringComparison.InvariantCultureIgnoreCase;
		index = fileName.IndexOf(Text, strComparison);
		return index >= 0;
	}

	public bool MatchFileName(string fileName)
	{
		var strComparison = CaseSensitive
			? StringComparison.InvariantCulture
			: StringComparison.InvariantCultureIgnoreCase;

		var index = fileName.IndexOf(Text, strComparison);
		return index >= 0;
	}
}