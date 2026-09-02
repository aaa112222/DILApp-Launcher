using System.Text.Json.Serialization;

namespace DILCore.Class.Model.LauncherProfile;

public class LauncherVersionModel
{
    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("format")] public int Format { get; set; }
}