using System.Collections.Generic;

namespace DILCore.Interface;

public interface ILogAnalyzer
{
    string? RootPath { get; }
    string? GameId { get; }
    bool VersionIsolation { get; }
    IReadOnlyList<string>? CustomLogFiles { get; }
    IEnumerable<IAnalysisReport> GenerateReport();
}