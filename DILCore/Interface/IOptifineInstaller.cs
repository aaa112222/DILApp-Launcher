using System.Threading.Tasks;
using DILCore.Class.Model.Optifine;

namespace DILCore.Interface;

public interface IOptifineInstaller : IInstaller
{
    string JavaExecutablePath { get; init; }
    string OptifineJarPath { get; init; }
    OptifineDownloadVersionModel OptifineDownloadVersion { get; init; }
    string Install();
    Task<string> InstallTaskAsync();
}