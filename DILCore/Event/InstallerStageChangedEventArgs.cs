using System;
using DILCore.Class.Model;

namespace DILCore.Event;

public class StageChangedEventArgs : EventArgs
{
    public required string CurrentStage { get; init; }
    public required ProgressValue Progress { get; init; }
}