using System.Text.Json;
using System.Text.Json.Serialization;

namespace DILCore.Class.Helper;

public static class JsonHelper
{
    public static JsonSerializerOptions CamelCasePropertyNamesSettings()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            MaxDepth = 100
        };
    }
}