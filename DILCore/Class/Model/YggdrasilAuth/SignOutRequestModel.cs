using System.Text.Json.Serialization;

namespace DILCore.Class.Model.YggdrasilAuth;

public class SignOutRequestModel
{
    [JsonPropertyName("username")] public required string Username { get; init; }

    [JsonPropertyName("password")] public required string Password { get; init; }
}