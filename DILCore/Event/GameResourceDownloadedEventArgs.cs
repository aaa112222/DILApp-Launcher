using System;

namespace DILCore.Event;

public class GameResourceDownloadedEventArgs : EventArgs
{
    public ulong TotalNeedToDownload { get; init; }
    public required DownloadFileCompletedEventArgs DownloadEventArgs { get; init; }
}