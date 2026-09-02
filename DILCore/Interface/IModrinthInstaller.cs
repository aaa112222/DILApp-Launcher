using System.Threading.Tasks;
using DILCore.Class.Model.Modrinth;

namespace DILCore.Interface;

public interface IModrinthInstaller : IInstaller
{
    string? GameId { get; init; }
    string ModPackPath { get; init; }
    Task<ModrinthModPackIndexModel?> ReadIndexTask();
    void Install();
    Task InstallTaskAsync();
}