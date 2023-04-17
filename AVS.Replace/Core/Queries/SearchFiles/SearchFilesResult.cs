namespace AVS.Replace.Core.Queries.SearchFiles
{
	public class SearchFilesResult
	{
		/// <summary>
		/// holds file paths (file name or content match search criteria)
		/// </summary>
		public List<FileInfo>? Files { get; set; }

		/// <summary>
		/// holds dir paths that match search criteria
		/// </summary>
		public List<string>? Directories { get; set; }
	}
}
