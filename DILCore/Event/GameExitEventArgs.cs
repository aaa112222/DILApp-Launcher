using System;

namespace DILCore.Event;

public class GameExitEventArgs : EventArgs
{
    public required string SourceGameId { get; init; }
    public Exception? Exception { get; init; }

    public int ExitCode { get; init; }
}