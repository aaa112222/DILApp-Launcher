using System;
using System.Text.Json;

namespace DILCore.Class.Model.MicrosoftAuth;

public class AuthXSTSResponseModel
{
    public DateTime IssueInstant { get; set; }
    public DateTime NotAfter { get; set; }
    public required string Token { get; set; }
    public JsonElement DisplayClaims { get; set; }
}