using System;
using System.Diagnostics;
using DILCore.Class.Model.YggdrasilAuth;

namespace DILCore.Class.Model;

public class LaunchResult
{
    public LaunchErrorType ErrorType { get; init; }
    public LaunchSettings? LaunchSettings { get; init; }
    public ErrorModel? Error { get; init; }
    public TimeSpan RunTime { get; init; }
    public Process? GameProcess { get; init; }
}