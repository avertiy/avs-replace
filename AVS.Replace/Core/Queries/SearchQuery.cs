namespace AVS.Replace.Core.Queries;

public abstract class SearchQuery
{
	public string SearchText { get; set; } = "";
	public string WorkingDirectory { get; set; } = ".";
	public SearchOptions Options { get; set; } = new SearchOptions();
}