using System;

namespace DILCore.Event;

public class LaunchLogEventArgs : EventArgs
{
    public required string SourceGameId { get; init; }
    public required string Item { get; init; }
    public required TimeSpan ItemRunTime { get; init; }
}