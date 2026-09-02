using System.Threading.Tasks;
using DILCore.Class.Model.CurseForge;

namespace DILCore.Interface;

public interface ICurseForgeInstaller : IInstaller
{
    string? GameId { get; init; }
    string ModPackPath { get; init; }
    static abstract Task<CurseForgeManifestModel?> ReadManifestTask(string modPackPath);
    void Install();
    Task InstallTaskAsync();
}