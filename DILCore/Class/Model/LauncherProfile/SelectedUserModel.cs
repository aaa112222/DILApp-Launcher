using System.Text.Json.Serialization;

namespace DILCore.Class.Model.LauncherProfile;

public class SelectedUserModel
{
    [JsonPropertyName("account")] public string? Account { get; set; }

    [JsonPropertyName("profile")] public string? Profile { get; set; }
}