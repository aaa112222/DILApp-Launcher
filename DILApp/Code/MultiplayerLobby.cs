using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace DILApp;

public class MultiplayerLobbyResult
{
    public string RoomCode { get; set; } = "";
    public string HostName { get; set; } = "";
    public int PlayerCount { get; set; } = 1;
}

public static class MultiplayerLobby
{
    private const string GitHubLatestReleaseApi = "https://api.github.com/repos/burningtnt/Terracotta/releases/latest";
    private const string GitHubLatestReleasePage = "https://github.com/burningtnt/Terracotta/releases/latest";
    private const string GhProxyLatestReleaseApi = "https://ghfast.top/https://api.github.com/repos/burningtnt/Terracotta/releases/latest";
    private const string TerracottaDirName = "terracotta";
    private const string TerracottaExe = "terracotta.exe";
    private const string HandoffPrefix = "dil-terracotta-";
    private const string FallbackVersion = "0.4.2";

    private static readonly HttpClient _http;
    private static readonly HttpClient _downloadHttp;

    private static Process? _terracottaProcess;
    private static int _terracottaPort;
    private static string _currentRoomCode = "";
    private static readonly object _lock = new object();

    static MultiplayerLobby()
    {
        _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("DIL/1.1.0");

        _downloadHttp = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(10)
        };
        _downloadHttp.DefaultRequestHeaders.UserAgent.ParseAdd("DIL/1.1.0");
    }

    private static string TerracottaBaseDir => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "tools", TerracottaDirName);

    private static string CpuArchitecture => RuntimeInformation.ProcessArchitecture switch
    {
        System.Runtime.InteropServices.Architecture.X64 => "x86_64",
        System.Runtime.InteropServices.Architecture.Arm64 => "arm64",
        _ => "x86_64"
    };

    public static bool IsLobbyActive
    {
        get { lock (_lock) { return !string.IsNullOrEmpty(_currentRoomCode); } }
    }

    public static async Task<MultiplayerLobbyResult> CreateLobbyAsync(string hostName)
    {
        if (IsLobbyActive)
            throw new InvalidOperationException("A lobby is already active.");

        var exePath = await EnsureTerracottaAvailableAsync();
        var endpoint = await StartOrGetTerracottaAsync(exePath);

        await EnsureWaitingStateAsync(endpoint.Port);

        var normalizedHostName = NormalizeProfileText(hostName, "Player", 64);
        await SendCommandAsync(endpoint.Port, $"/state/scanning?player={Uri.EscapeDataString(normalizedHostName)}");

        var result = await WaitForHostAsync(endpoint.Port);
        _currentRoomCode = result.RoomCode;
        return result;
    }

    public static async Task<MultiplayerLobbyResult> JoinLobbyAsync(string roomCode, string playerName)
    {
        if (IsLobbyActive)
            throw new InvalidOperationException("A lobby is already active.");

        var normalizedRoomCode = NormalizeProfileText(roomCode, "", 256);
        if (normalizedRoomCode.Length == 0)
            throw new ArgumentException("Room code cannot be empty.");

        var exePath = await EnsureTerracottaAvailableAsync();
        var endpoint = await StartOrGetTerracottaAsync(exePath);

        await EnsureWaitingStateAsync(endpoint.Port);

        var normalizedPlayerName = NormalizeProfileText(playerName, "Player", 64);
        await SendGuestCommandAsync(endpoint.Port,
            $"/state/guesting?room={Uri.EscapeDataString(normalizedRoomCode)}&player={Uri.EscapeDataString(normalizedPlayerName)}");

        var result = await WaitForGuestAsync(endpoint.Port, normalizedRoomCode);
        _currentRoomCode = result.RoomCode;
        return result;
    }

    public static async Task LeaveLobbyAsync()
    {
        var process = _terracottaProcess;
        var port = _terracottaPort;
        _currentRoomCode = "";

        if (process == null || process.HasExited)
        {
            _terracottaProcess = null;
            return;
        }

        try
        {
            await SendCommandAsync(port, "/state/ide");
            await WaitForWaitingStateAsync(port);
        }
        catch { }
    }

    private static async Task<string> EnsureTerracottaAvailableAsync()
    {
        var exePath = FindTerracottaExecutable();
        if (exePath != null)
            return exePath;

        var releaseInfo = await FetchLatestReleaseAsync();
        if (releaseInfo == null)
            throw new InvalidOperationException("Failed to fetch Terracotta release info. Please check your network connection.");

        var version = releaseInfo.Version;
        var arch = CpuArchitecture;
        var assetName = $"terracotta-{version}-windows-{arch}-pkg.tar.gz";
        var installDir = Path.Combine(TerracottaBaseDir, version, $"terracotta-windows-{arch}");

        if (File.Exists(Path.Combine(installDir, TerracottaExe)))
            return Path.Combine(installDir, TerracottaExe);

        string? downloadUrl = null;
        foreach (var asset in releaseInfo.Assets)
        {
            if (string.Equals(asset.Name, assetName, StringComparison.OrdinalIgnoreCase))
            {
                downloadUrl = asset.DownloadUrl;
                break;
            }
        }

        if (string.IsNullOrEmpty(downloadUrl))
        {
            downloadUrl = $"https://github.com/burningtnt/Terracotta/releases/download/v{version}/{assetName}";
        }

        Directory.CreateDirectory(installDir);

        var archivePath = Path.Combine(TerracottaBaseDir, $".download-{Guid.NewGuid():N}.tar.gz");
        try
        {
            byte[]? bytes = null;

            try
            {
                bytes = await _downloadHttp.GetByteArrayAsync(downloadUrl);
            }
            catch { }

            if (bytes == null)
            {
                var proxyUrl = $"https://ghfast.top/{downloadUrl}";
                try
                {
                    bytes = await _downloadHttp.GetByteArrayAsync(proxyUrl);
                }
                catch { }
            }

            if (bytes == null)
                throw new InvalidOperationException($"Failed to download Terracotta v{version}. Please check your network connection.");

            var expectedExeName = $"terracotta-{version}-windows-{arch}.exe";
            await Task.Run(() =>
            {
                File.WriteAllBytes(archivePath, bytes);
                ExtractTarGz(archivePath, installDir, expectedExeName);
            });
        }
        finally
        {
            TryDeleteFile(archivePath);
        }

        exePath = Path.Combine(installDir, TerracottaExe);
        if (!File.Exists(exePath))
            throw new InvalidOperationException("Terracotta executable not found after extraction.");

        return exePath;
    }

    private static void ExtractTarGz(string archivePath, string destinationDir, string expectedExeName)
    {
        var gzipPath = Path.Combine(TerracottaBaseDir, $".temp-{Guid.NewGuid():N}.tar");
        try
        {
            using (var gzipStream = File.OpenRead(archivePath))
            using (var decompressStream = new GZipStream(gzipStream, CompressionMode.Decompress))
            using (var tarStream = File.Create(gzipPath))
            {
                decompressStream.CopyTo(tarStream);
            }

            ExtractTar(gzipPath, destinationDir, expectedExeName);
        }
        finally
        {
            TryDeleteFile(gzipPath);
        }
    }

    private static void ExtractTar(string tarPath, string destinationDir, string expectedExeName)
    {
        using var stream = File.OpenRead(tarPath);
        var buffer = new byte[512];

        while (true)
        {
            if (stream.Read(buffer, 0, 512) < 512)
                break;

            var nameBytes = new byte[100];
            Array.Copy(buffer, 0, nameBytes, 0, 100);
            var name = TrimNulls(nameBytes);
            if (string.IsNullOrEmpty(name))
                break;

            var sizeBytes = new byte[12];
            Array.Copy(buffer, 124, sizeBytes, 0, 12);
            var sizeStr = TrimNulls(sizeBytes);
            var size = string.IsNullOrEmpty(sizeStr) ? 0 : Convert.ToInt64(sizeStr, 8);

            var typeFlag = (char)buffer[156];

            if (typeFlag == '5')
            {
                continue;
            }
            else if (typeFlag == '0' || typeFlag == '\0')
            {
                var normalizedName = name.Replace('\\', '/');
                if (normalizedName.Contains('/') || normalizedName is "." or "..")
                {
                    if (size > 0)
                    {
                        var blocks = (size + 511) / 512;
                        stream.Position += blocks * 512;
                    }
                    continue;
                }

                var destName = normalizedName;
                if (string.Equals(normalizedName, expectedExeName, StringComparison.OrdinalIgnoreCase))
                {
                    destName = TerracottaExe;
                }

                var filePath = Path.Combine(destinationDir, destName);

                using (var fileStream = File.Create(filePath))
                {
                    var remaining = size;
                    var readBuffer = new byte[81920];
                    while (remaining > 0)
                    {
                        var toRead = (int)Math.Min(remaining, readBuffer.Length);
                        var read = stream.Read(readBuffer, 0, toRead);
                        if (read == 0) break;
                        fileStream.Write(readBuffer, 0, read);
                        remaining -= read;
                    }
                }

                var padding = (512 - (size % 512)) % 512;
                if (padding > 0)
                    stream.Position += padding;
            }
            else
            {
                if (size > 0)
                {
                    var blocks = (size + 511) / 512;
                    stream.Position += blocks * 512;
                }
            }
        }
    }

    private static string TrimNulls(byte[] bytes)
    {
        var length = 0;
        for (var i = 0; i < bytes.Length; i++)
        {
            if (bytes[i] == 0) break;
            length++;
        }
        return System.Text.Encoding.ASCII.GetString(bytes, 0, length);
    }

    private static string? FindTerracottaExecutable()
    {
        if (!Directory.Exists(TerracottaBaseDir))
            return null;

        try
        {
            var files = Directory.GetFiles(TerracottaBaseDir, TerracottaExe, SearchOption.AllDirectories);
            if (files.Length > 0)
                return files[0];
        }
        catch { }

        return null;
    }

    private static async Task<TerracottaRelease?> FetchLatestReleaseAsync()
    {
        try
        {
            var json = await _http.GetStringAsync(GitHubLatestReleaseApi);
            var release = ParseRelease(json);
            if (release != null) return release;
        }
        catch { }

        try
        {
            var json = await _http.GetStringAsync(GhProxyLatestReleaseApi);
            var release = ParseRelease(json);
            if (release != null) return release;
        }
        catch { }

        try
        {
            var response = await _http.GetAsync(GitHubLatestReleasePage);
            var finalUrl = response.RequestMessage?.RequestUri?.ToString() ?? "";
            var match = System.Text.RegularExpressions.Regex.Match(finalUrl, @"/releases/tag/v(\d+\.\d+\.\d+)");
            if (match.Success)
            {
                return new TerracottaRelease
                {
                    Version = match.Groups[1].Value,
                    Assets = []
                };
            }
        }
        catch { }

        return new TerracottaRelease
        {
            Version = FallbackVersion,
            Assets = []
        };
    }

    private static TerracottaRelease? ParseRelease(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        string? version = null;
        if (root.TryGetProperty("tag_name", out var tagProp))
            version = tagProp.GetString()?.TrimStart('v');
        else if (root.TryGetProperty("name", out var nameProp))
            version = nameProp.GetString();

        if (string.IsNullOrEmpty(version))
            return null;

        if (root.TryGetProperty("prerelease", out var prerelease) && prerelease.GetBoolean())
            return null;
        if (root.TryGetProperty("draft", out var draft) && draft.GetBoolean())
            return null;

        var assets = new List<TerracottaReleaseAsset>();
        if (root.TryGetProperty("assets", out var assetsElement))
        {
            foreach (var asset in assetsElement.EnumerateArray())
            {
                string? name = null;
                if (asset.TryGetProperty("name", out var nameEl))
                    name = nameEl.GetString();

                string? downloadUrl = null;
                if (asset.TryGetProperty("browser_download_url", out var urlEl))
                    downloadUrl = urlEl.GetString();

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(downloadUrl))
                {
                    assets.Add(new TerracottaReleaseAsset
                    {
                        Name = name,
                        DownloadUrl = downloadUrl
                    });
                }
            }
        }

        return new TerracottaRelease
        {
            Version = version!,
            Assets = assets
        };
    }

    private static async Task<(int Port, bool Owned)> StartOrGetTerracottaAsync(string exePath)
    {
        var existingPort = TryReadExistingPort();
        if (existingPort > 0)
        {
            try
            {
                var state = await GetStateAsync(existingPort);
                if (state.Kind != TerracottaStateKind.Unknown)
                    return (existingPort, false);
            }
            catch { }
        }

        var handoffPath = Path.Combine(Path.GetTempPath(), $"{HandoffPrefix}{Guid.NewGuid():N}.json");
        var workingDir = Path.GetDirectoryName(exePath) ?? TerracottaBaseDir;
        var startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            WorkingDirectory = workingDir,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        startInfo.ArgumentList.Add("--hmcl2");
        startInfo.ArgumentList.Add(handoffPath);

        var process = new Process { StartInfo = startInfo };
        if (!process.Start())
            throw new InvalidOperationException("Failed to start Terracotta.");

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        var port = await WaitForHandoffAsync(process, handoffPath);
        _terracottaProcess = process;
        _terracottaPort = port;

        TryDeleteFile(handoffPath);
        return (port, true);
    }

    private static int TryReadExistingPort()
    {
        var lockPath = Path.Combine(Path.GetTempPath(), "terracotta", "terracotta.lock");
        if (!File.Exists(lockPath))
            return 0;

        try
        {
            using var stream = new FileStream(lockPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            Span<byte> buffer = stackalloc byte[2];
            if (stream.Read(buffer) != buffer.Length)
                return 0;
            var port = (buffer[0] << 8) | buffer[1];
            return port is > 0 and <= 65535 ? port : 0;
        }
        catch { }

        return 0;
    }

    private static async Task<int> WaitForHandoffAsync(Process process, string handoffPath)
    {
        var deadline = DateTime.Now.AddSeconds(20);
        while (DateTime.Now < deadline)
        {
            if (process.HasExited)
                throw new InvalidOperationException("Terracotta exited unexpectedly.");

            if (File.Exists(handoffPath))
            {
                try
                {
                    var json = await Task.Run(() => File.ReadAllText(handoffPath));
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("port", out var portProp))
                    {
                        var port = portProp.GetInt32();
                        if (port is > 0 and <= 65535)
                            return port;
                    }
                }
                catch (IOException) { }
            }

            await Task.Delay(200);
        }

        throw new TimeoutException("Terracotta handoff timed out.");
    }

    private static async Task<TerracottaStateInfo> GetStateAsync(int port)
    {
        try
        {
            var json = await _http.GetStringAsync($"http://127.0.0.1:{port}/state");
            return ParseState(json);
        }
        catch
        {
            return new TerracottaStateInfo { Kind = TerracottaStateKind.Unknown };
        }
    }

    internal static TerracottaStateInfo ParseState(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Object
            || !root.TryGetProperty("state", out var stateElement)
            || stateElement.ValueKind != JsonValueKind.String)
        {
            return new TerracottaStateInfo { Kind = TerracottaStateKind.Unknown };
        }

        var stateStr = stateElement.GetString() ?? "";
        var kind = stateStr switch
        {
            "waiting" => TerracottaStateKind.Waiting,
            "host-scanning" => TerracottaStateKind.HostScanning,
            "host-starting" => TerracottaStateKind.HostStarting,
            "host-ok" => TerracottaStateKind.HostOk,
            "guest-connecting" => TerracottaStateKind.GuestConnecting,
            "guest-starting" => TerracottaStateKind.GuestStarting,
            "guest-ok" => TerracottaStateKind.GuestOk,
            "exception" => TerracottaStateKind.Exception,
            _ => TerracottaStateKind.Unknown
        };

        var roomCode = root.TryGetProperty("room", out var roomElement)
            && roomElement.ValueKind == JsonValueKind.String
                ? NormalizeProfileText(roomElement.GetString(), "", 64)
                : "";

        var players = new List<TerracottaPlayer>();
        if (root.TryGetProperty("profiles", out var profilesElement)
            && profilesElement.ValueKind == JsonValueKind.Array)
        {
            var machineIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var profile in profilesElement.EnumerateArray())
            {
                if (profile.ValueKind != JsonValueKind.Object)
                    continue;

                var machineId = GetStringValue(profile, "machine_id", 128);
                if (machineId.Length == 0 || !machineIds.Add(machineId))
                    continue;

                var name = NormalizeProfileText(GetStringValue(profile, "name", 64), "Player", 64);
                var profileKind = GetStringValue(profile, "kind", 16);
                var isHost = string.Equals(profileKind, "HOST", StringComparison.OrdinalIgnoreCase);

                players.Add(new TerracottaPlayer
                {
                    Name = name,
                    MachineId = machineId,
                    IsHost = isHost
                });
            }
        }

        int? exceptionType = null;
        if (root.TryGetProperty("exception_type", out var exTypeElement)
            && exTypeElement.ValueKind == JsonValueKind.Number)
        {
            exceptionType = exTypeElement.GetInt32();
        }

        return new TerracottaStateInfo
        {
            Kind = kind,
            RoomCode = roomCode,
            ExceptionType = exceptionType,
            Players = players
        };
    }

    private static string GetStringValue(JsonElement element, string propertyName, int maxLength)
    {
        if (!element.TryGetProperty(propertyName, out var value)
            || value.ValueKind != JsonValueKind.String)
        {
            return "";
        }
        return NormalizeProfileText(value.GetString(), "", maxLength);
    }

    private static string NormalizeProfileText(string? value, string fallback, int maxLength)
    {
        var normalized = new string((value ?? "")
            .Trim()
            .Where(c => !char.IsControl(c))
            .ToArray());
        if (normalized.Length == 0)
            normalized = fallback;
        return normalized.Length <= maxLength ? normalized : normalized[..maxLength];
    }

    private static async Task SendCommandAsync(int port, string path)
    {
        var response = await _http.GetAsync($"http://127.0.0.1:{port}{path}");
        response.EnsureSuccessStatusCode();
    }

    private static async Task SendGuestCommandAsync(int port, string path)
    {
        var response = await _http.GetAsync($"http://127.0.0.1:{port}{path}");
        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            throw new InvalidOperationException("Invalid room code. Terracotta rejected the room code.");
        }
        response.EnsureSuccessStatusCode();
    }

    private static async Task EnsureWaitingStateAsync(int port)
    {
        var state = await GetStateAsync(port);
        if (state.Kind == TerracottaStateKind.Waiting)
            return;

        try
        {
            await SendCommandAsync(port, "/state/ide");
            await WaitForWaitingStateAsync(port);
        }
        catch { }

        state = await GetStateAsync(port);
        if (state.Kind != TerracottaStateKind.Waiting)
            throw new InvalidOperationException("Terracotta is busy. Another lobby may be active.");
    }

    private static async Task WaitForWaitingStateAsync(int port)
    {
        var deadline = DateTime.Now.AddSeconds(5);
        while (DateTime.Now < deadline)
        {
            try
            {
                var state = await GetStateAsync(port);
                if (state.Kind == TerracottaStateKind.Waiting)
                    return;
            }
            catch { }
            await Task.Delay(100);
        }
    }

    private static async Task<MultiplayerLobbyResult> WaitForHostAsync(int port)
    {
        var deadline = DateTime.Now.AddSeconds(60);
        var consecutiveFailures = 0;
        TerracottaStateKind? lastKind = null;
        while (DateTime.Now < deadline)
        {
            try
            {
                var state = await GetStateAsync(port);
                consecutiveFailures = 0;
                lastKind = state.Kind;
                switch (state.Kind)
                {
                    case TerracottaStateKind.HostOk:
                        if (string.IsNullOrWhiteSpace(state.RoomCode))
                            throw new InvalidOperationException("Terracotta returned an invalid room code.");
                        return BuildResult(state);
                    case TerracottaStateKind.Exception:
                        var exType = state.ExceptionType;
                        throw new InvalidOperationException(
                            exType == 3 ? "Terracotta's EasyTier host stopped during creation." :
                            exType == 4 ? "The Minecraft LAN world is no longer available. Please open Minecraft to LAN first." :
                            "Terracotta reported a host creation error.");
                    case TerracottaStateKind.Waiting:
                        throw new InvalidOperationException("Terracotta returned to the waiting state during creation. Please ensure Minecraft is open to LAN.");
                    case TerracottaStateKind.HostScanning:
                    case TerracottaStateKind.HostStarting:
                        break;
                    case TerracottaStateKind.Unknown:
                        break;
                }
            }
            catch (InvalidOperationException) { throw; }
            catch
            {
                consecutiveFailures++;
                if (consecutiveFailures > 10)
                    throw new InvalidOperationException("Cannot communicate with Terracotta. The process may have crashed.");
            }

            await Task.Delay(500);
        }

        var hint = lastKind == TerracottaStateKind.HostScanning
            ? " Terracotta is still scanning for a LAN world — please ensure Minecraft is open to LAN."
            : lastKind == TerracottaStateKind.Unknown
                ? " Terracotta state is unknown — please check if Terracotta is running."
                : "";
        throw new TimeoutException($"Timed out waiting for lobby creation.{hint}");
    }

    private static async Task<MultiplayerLobbyResult> WaitForGuestAsync(int port, string expectedRoomCode)
    {
        var canonicalRoomCode = expectedRoomCode;
        var deadline = DateTime.Now.AddSeconds(60);
        var consecutiveFailures = 0;
        while (DateTime.Now < deadline)
        {
            try
            {
                var state = await GetStateAsync(port);
                consecutiveFailures = 0;
                if (state.Kind is TerracottaStateKind.GuestConnecting or TerracottaStateKind.GuestStarting
                    && !string.IsNullOrWhiteSpace(state.RoomCode))
                {
                    canonicalRoomCode = state.RoomCode;
                }
                switch (state.Kind)
                {
                    case TerracottaStateKind.GuestOk:
                        return BuildResult(state, canonicalRoomCode);
                    case TerracottaStateKind.Exception:
                        var exType = state.ExceptionType;
                        throw new InvalidOperationException(
                            exType == 0 || exType == 1 ? "Terracotta could not reach the room host." :
                            exType == 2 ? "Terracotta's EasyTier guest stopped while joining." :
                            "Terracotta reported a guest connection error.");
                    case TerracottaStateKind.Waiting:
                        throw new InvalidOperationException("Terracotta returned to the waiting state while joining.");
                    case TerracottaStateKind.HostScanning:
                    case TerracottaStateKind.HostStarting:
                    case TerracottaStateKind.HostOk:
                        throw new InvalidOperationException("Terracotta returned an incompatible guest state. Another lobby may be active on this machine.");
                    case TerracottaStateKind.GuestConnecting:
                    case TerracottaStateKind.GuestStarting:
                        break;
                    case TerracottaStateKind.Unknown:
                        break;
                }
            }
            catch (InvalidOperationException) { throw; }
            catch
            {
                consecutiveFailures++;
                if (consecutiveFailures > 10)
                    throw new InvalidOperationException("Cannot communicate with Terracotta. The process may have crashed.");
            }

            await Task.Delay(500);
        }

        throw new TimeoutException("Timed out waiting to join lobby.");
    }

    private static MultiplayerLobbyResult BuildResult(TerracottaStateInfo state, string? overrideRoomCode = null)
    {
        var roomCode = overrideRoomCode ?? state.RoomCode;
        var hostName = state.Players.FirstOrDefault(p => p.IsHost)?.Name ?? "";
        if (string.IsNullOrEmpty(hostName) && state.Players.Count > 0)
            hostName = state.Players[0].Name;
        return new MultiplayerLobbyResult
        {
            RoomCode = roomCode,
            HostName = hostName,
            PlayerCount = state.Players.Count > 0 ? state.Players.Count : 1
        };
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch { }
    }

    private class TerracottaRelease
    {
        public string Version { get; set; } = "";
        public List<TerracottaReleaseAsset> Assets { get; set; } = [];
    }

    private class TerracottaReleaseAsset
    {
        public string Name { get; set; } = "";
        public string DownloadUrl { get; set; } = "";
    }
}

internal enum TerracottaStateKind
{
    Unknown,
    Waiting,
    HostScanning,
    HostStarting,
    HostOk,
    GuestConnecting,
    GuestStarting,
    GuestOk,
    Exception
}

internal class TerracottaStateInfo
{
    public TerracottaStateKind Kind { get; set; } = TerracottaStateKind.Unknown;
    public string RoomCode { get; set; } = "";
    public int? ExceptionType { get; set; }
    public List<TerracottaPlayer> Players { get; set; } = [];
}

internal class TerracottaPlayer
{
    public string Name { get; set; } = "";
    public string MachineId { get; set; } = "";
    public bool IsHost { get; set; }
}