namespace HearthWatcher.LogReader
{
	public class LogWatcherInfo
	{
		public bool HasFilters => StartsWithFilters != null || ContainsFilters != null;

		public string Name { get; set; } = string.Empty;
		public string[] StartsWithFilters { get; set; } = Array.Empty<string>();
		public string[] ContainsFilters { get; set; } = Array.Empty<string>();
		public bool Reset { get; set; } = true;
	}
}
