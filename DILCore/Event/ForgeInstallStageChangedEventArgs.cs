using System;
using DILCore.Class.Model;

namespace DILCore.Event;

public class ForgeInstallStageChangedEventArgs : EventArgs
{
    public required string CurrentStage { get; init; }
    public ProgressValue Progress { get; init; }
}