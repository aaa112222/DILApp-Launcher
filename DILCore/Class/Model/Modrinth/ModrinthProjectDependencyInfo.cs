using System.Text.Json.Serialization;

namespace DILCore.Class.Model.Modrinth;

public class ModrinthProjectDependencyInfo
{
    [JsonPropertyName("projects")] public ModrinthProjectInfo[] Projects { get; set; } = [];

    [JsonPropertyName("versions")] public ModrinthVersionInfo[] Versions { get; set; } = [];
}