using System;
using DILCore.Class.Model;

namespace DILCore.Event;

public class DownloadFileChangedEventArgs : EventArgs
{
    public double Speed { get; init; }
    public ProgressValue ProgressPercentage { get; init; }
    public long BytesReceived { get; init; }
    public long? TotalBytes { get; init; }
}