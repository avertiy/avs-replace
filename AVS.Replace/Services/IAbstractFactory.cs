using AVS.Replace.Engines;

namespace AVS.Replace.Services;

public interface IAbstractFactory
{
	ISearchFileService CreateSearchFileService();
	IMatchService CreateMatchService();
	IReplaceService CreateReplaceService();
	IEngine GetEngine(SearchMode searchMode);
}

public class DefaultFactory : IAbstractFactory
{

	public ISearchFileService CreateSearchFileService()
	{
		return new SearchFileService();
	}

	public IMatchService CreateMatchService()
	{
		return new MatchService();
	}

	public IReplaceService CreateReplaceService()
	{
		return new ReplaceService();
	}

	public IEngine GetEngine(SearchMode searchMode)
	{
		IEngine engine = null;
		switch (searchMode)
		{
			case SearchMode.FileSearch:
				engine = new FileSearchEngine(new FileSystemService(), new ConsolePrinter());
				break;
			default:
				engine = new ReplaceEngine(this);
				break;
		}
		return engine;
	}
}