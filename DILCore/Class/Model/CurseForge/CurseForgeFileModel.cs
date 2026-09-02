using System.Text.Json.Serialization;

namespace DILCore.Class.Model.CurseForge;

public class CurseForgeFileModel
{
    [JsonPropertyName("projectID")] public long ProjectId { get; set; }

    [JsonPropertyName("fileID")] public long FileId { get; set; }

    [JsonPropertyName("required")] public bool Required { get; set; }
}