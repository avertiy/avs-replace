namespace AVS.Replace.Core;

public abstract class BaseHandler
{
	public event Action<(string, int)>? ProgressChange;

	protected void ReportProgress(string status, int progress)
	{
		ProgressChange?.Invoke((status, progress));
	}
}