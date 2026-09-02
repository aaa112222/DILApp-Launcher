using System.Threading.Tasks;
using DILCore.Class;
using DILCore.Class.Model;

namespace DILCore.Interface;

public interface IForgeInstaller : IInstaller
{
    string ForgeExecutablePath { get; init; }
    VersionLocatorBase VersionLocator { get; init; }

    ForgeInstallResult InstallForge();
    Task<ForgeInstallResult> InstallForgeTaskAsync();
}