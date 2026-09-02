using System;
using DILCore.Class.Model;

namespace DILCore.Event;

public class GameResourceInfoResolveEventArgs : EventArgs
{
    public required ProgressValue Progress { get; init; }
    public string? Status { get; init; }
}