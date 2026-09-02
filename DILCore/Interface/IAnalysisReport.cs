using System.Collections.Generic;
using DILCore.DefaultComponent.LogAnalysis;

namespace DILCore.Interface;

public interface IAnalysisReport
{
    CrashCauses Cause { get; }
    IReadOnlyCollection<string>? Details { get; }
    string? From { get; set; }
    bool HasDetails { get; }
}