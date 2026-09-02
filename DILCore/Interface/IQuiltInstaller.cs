using System.Threading.Tasks;
using DILCore.Class.Model.Quilt;

namespace DILCore.Interface;

public interface IQuiltInstaller : IInstaller
{
    QuiltLoaderModel LoaderArtifact { get; init; }
    string Install();
    Task<string> InstallTaskAsync();
}