using System.Text.Json.Serialization;

namespace DILCore.Class.Model.ServerPing;

public class Player
{
    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("id")] public string? Id { get; set; }
}