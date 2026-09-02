using System.Threading.Tasks;
using DILCore.Class.Model.Fabric;

namespace DILCore.Interface;

public interface IFabricInstaller : IInstaller
{
    FabricLoaderArtifactModel LoaderArtifact { get; init; }
    string Install();
    Task<string> InstallTaskAsync();
}