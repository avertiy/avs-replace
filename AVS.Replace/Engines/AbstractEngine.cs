namespace AVS.Replace.Engines;

public interface IEngine
{
	Task ExecuteAsync(SearchContext context);
}

public abstract class AbstractEngine : IEngine
{
	public abstract Task ExecuteAsync(SearchContext context);

	public event Action<(string, int)>? ProgressChange;

	protected void ReportProgress(string status, int progress)
	{
		ProgressChange?.Invoke((status, progress));
	}
}