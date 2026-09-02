using System.Collections.Generic;
using DILCore.Class.Model.Downloading;

namespace DILCore.Class.Model;

public class ResourceCompleterCheckResult
{
    public bool IsLibDownloadFailed { get; init; }
    public required IReadOnlyCollection<MultiSourceDownloadFile> FailedFiles { get; init; }
}