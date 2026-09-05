using System.Threading;

namespace DILApp;

public class DownloadTask
{
	private static int _nextId = 1;

	public int Id { get; }

	public string Name { get; set; } = "";

	public string VersionId { get; set; } = "";

	public string LoaderName { get; set; } = "";

	public double Progress { get; set; }

	public double Speed { get; set; }

	public DownloadStep Step { get; set; }

	public string StepText { get; set; } = "";

	public int CurrentFileIndex { get; set; }

	public int TotalFiles { get; set; }

	public long TotalBytes { get; set; }

	public long DownloadedBytes { get; set; }

	internal CancellationTokenSource CancellationTokenSource { get; }

	internal long CurrentBytes;

	internal long LastBytes;

	internal DateTime LastSpeedUpdate;

	public bool IsActive => Step != DownloadStep.Completed && Step != DownloadStep.Failed && Step != DownloadStep.Cancelled;

	public DownloadTask()
	{
		Id = Interlocked.Increment(ref _nextId);
		CancellationTokenSource = new CancellationTokenSource();
		LastSpeedUpdate = DateTime.Now;
	}
}