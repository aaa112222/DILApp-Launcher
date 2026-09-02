using System.Net.Http;

namespace DILCore.Interface;

public interface IInstaller
{
    string? CustomId { get; init; }
    string RootPath { get; init; }
    string? InheritsFrom { get; init; }
    IHttpClientFactory HttpClientFactory { get; init; }
}