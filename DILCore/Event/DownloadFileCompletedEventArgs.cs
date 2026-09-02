using System;

namespace DILCore.Event;

public class DownloadFileCompletedEventArgs(bool success, Exception? ex, double averageSpeed) : EventArgs
{
    public double AverageSpeed { get; } = averageSpeed;
    public bool Success { get; } = success;
    public Exception? Error { get; } = ex;
}