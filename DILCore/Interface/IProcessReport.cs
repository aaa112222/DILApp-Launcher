using System;
using DILCore.Event;

namespace DILCore.Interface;

public interface IProgressReport
{
    event EventHandler<StageChangedEventArgs> StageChangedEventDelegate;
}