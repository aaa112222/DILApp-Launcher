using System;
using DILCore.Event;
using DILCore.Interface;

namespace DILCore.Class.Model;

public class ProgressReportBase : IProgressReport
{
    public event EventHandler<StageChangedEventArgs>? StageChangedEventDelegate;

    protected void InvokeStatusChangedEvent(string currentStage, ProgressValue progress)
    {
        this.StageChangedEventDelegate?.Invoke(this, new StageChangedEventArgs
        {
            CurrentStage = currentStage,
            Progress = progress
        });
    }
}