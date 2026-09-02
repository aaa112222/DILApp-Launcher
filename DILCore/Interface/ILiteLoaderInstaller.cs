using System.Threading.Tasks;

namespace DILCore.Interface;

public interface ILiteLoaderInstaller : IInstaller
{
    string Install();
    Task<string> InstallTaskAsync();
}