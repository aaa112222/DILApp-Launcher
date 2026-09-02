using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DILCore.Class.Model;
using DILCore.Class.Model.Auth;
using DILCore.Class.Model.LauncherAccount;
using DILCore.Class.Model.Microsoft.Graph;
using DILCore.Class.Model.MicrosoftAuth;
using DILCore.DefaultComponent.Authenticator;
using DILCore.DefaultComponent.Launch;

namespace DILApp;

public static class MicrosoftAuthService
{
	private static readonly string MsAuthCachePath = Path.Combine(
		DownloadManager.GetExeDirectory(), "ms_auth.json");

	private static readonly string LogFilePath = Path.Combine(
		DownloadManager.GetExeDirectory(), "ms_auth_debug.log");

	private static readonly MicrosoftAuthenticatorAPISettings ApiSettings = new MicrosoftAuthenticatorAPISettings
	{
		ClientId = "00000000402b5328",
		TenentId = "consumers",
		Scopes = new[] { "XboxLive.signin", "offline_access", "openid", "email", "profile" }
	};

	public static bool IsLoggedIn { get; private set; }
	public static string? PlayerName { get; private set; }
	public static string? PlayerUuid { get; private set; }
	public static string? AccessToken { get; private set; }
	public static string? RefreshToken { get; private set; }
	public static DateTime TokenExpiry { get; private set; }
	public static string? LastError { get; private set; }

	public static event Action? LoginStateChanged;

	static MicrosoftAuthService()
	{
		MicrosoftAuthenticator.Configure(ApiSettings);
		LoadCachedLogin();
	}

	private static void Log(string message)
	{
		try
		{
			var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
			File.AppendAllText(LogFilePath, line);
		}
		catch { }
	}

	public static async Task<MsLoginResult> LoginAsync(Action<DeviceCodeInfo> onDeviceCodeReceived, IProgress<string>? progress = null)
	{
		try
		{
			LastError = null;
			Log("LoginAsync started");
			var factory = new SimpleHttpClientFactory();

			progress?.Report(LanguageManager.Get("MsAuthRequestingToken"));
			GraphAuthResultModel? graphResult = null;
			DeviceCodeInfo? deviceCodeInfo = null;

			Log("Calling GetMSAuthResult...");
			graphResult = await MicrosoftAuthenticator.GetMSAuthResult(factory, code =>
			{
				deviceCodeInfo = new DeviceCodeInfo
				{
					UserCode = code.UserCode,
					VerificationUrl = code.VerificationUri
				};
				onDeviceCodeReceived(deviceCodeInfo);
			});

			if (graphResult == null)
			{
				var detail = MicrosoftAuthenticator.LastGetMSAuthResultError ?? "unknown";
				LastError = $"GetMSAuthResult returned null: {detail}";
				Log($"GetMSAuthResult failed: {detail}");
				progress?.Report(LanguageManager.Get("MsAuthExpired"));
				return MsLoginResult.Failed(LanguageManager.Get("MsAuthExpired"));
			}

			Log($"GetMSAuthResult succeeded, access_token length={graphResult.AccessToken?.Length ?? 0}, has_refresh={graphResult.RefreshToken != null}, has_id_token={graphResult.IdToken != null}");

			progress?.Report(LanguageManager.Get("MsAuthAuthenticating"));

			DefaultLauncherAccountParser accountParser;
			try
			{
				accountParser = CreateAccountParser();
				Log("CreateAccountParser succeeded");
			}
			catch (Exception ex)
			{
				LastError = $"CreateAccountParser failed: {ex.Message}";
				Log($"CreateAccountParser failed: {ex}");
				return MsLoginResult.Failed($"Account parser error: {ex.Message}");
			}

			var authenticator = new MicrosoftAuthenticator
			{
				HttpClientFactory = factory,
				LauncherAccountParser = accountParser,
				CacheTokenProvider = _ => ValueTask.FromResult(new CacheTokenProviderResult(
					true, false, null, graphResult))
			};

			AuthResultBase authResult;
			try
			{
				Log("Calling AuthTaskAsync...");
				authResult = await authenticator.AuthTaskAsync(false);
				Log($"AuthTaskAsync returned: status={authResult.AuthStatus}, hasError={authResult.Error != null}");
			}
			catch (Exception ex)
			{
				LastError = $"AuthTaskAsync exception: {ex.Message}";
				Log($"AuthTaskAsync exception: {ex}");
				return MsLoginResult.Failed(ex.Message);
			}

			if (authResult is MicrosoftAuthResult msResult && msResult.AuthStatus == AuthStatus.Succeeded)
			{
				PlayerName = msResult.SelectedProfile?.Name ?? msResult.User?.UserName;
				PlayerUuid = msResult.SelectedProfile?.Id.ToString("N");
				AccessToken = msResult.AccessToken;
				RefreshToken = msResult.RefreshToken;
				TokenExpiry = DateTime.Now.AddSeconds(msResult.ExpiresIn);
				IsLoggedIn = true;
				SaveCachedLogin();
				LoginStateChanged?.Invoke();
				Log($"Login succeeded: player={PlayerName}, uuid={PlayerUuid}");
				progress?.Report(LanguageManager.Get("MsAuthSuccess"));
				return MsLoginResult.Succeeded();
			}

			var error = authResult.Error;
			string errorMsg = error?.ErrorMessage ?? error?.Error ?? "Unknown auth error";
			string errorCause = error?.Cause ?? "";
			LastError = $"Auth failed: [{errorCause}] {errorMsg}";
			Log($"Auth failed: cause={errorCause}, error={error?.Error}, msg={errorMsg}");
			progress?.Report(errorMsg);
			return MsLoginResult.Failed(errorMsg);
		}
		catch (Exception ex)
		{
			LastError = $"LoginAsync exception: {ex}";
			Log($"LoginAsync exception: {ex}");
			progress?.Report(ex.Message);
			return MsLoginResult.Failed(ex.Message);
		}
	}

	public static async Task<bool> TrySilentLoginAsync()
	{
		if (string.IsNullOrEmpty(RefreshToken)) return false;

		try
		{
			Log("TrySilentLoginAsync started");
			var factory = new SimpleHttpClientFactory();
			var graphResult = await RefreshTokenAsync(factory);
			if (graphResult == null)
			{
				Log("RefreshTokenAsync returned null");
				return false;
			}

			var accountParser = CreateAccountParser();
			var authenticator = new MicrosoftAuthenticator
			{
				HttpClientFactory = factory,
				LauncherAccountParser = accountParser,
				CacheTokenProvider = _ => ValueTask.FromResult(new CacheTokenProviderResult(
					true, false, null, graphResult))
			};

			var authResult = await authenticator.AuthTaskAsync(false);

			if (authResult is MicrosoftAuthResult msResult && msResult.AuthStatus == AuthStatus.Succeeded)
			{
				PlayerName = msResult.SelectedProfile?.Name ?? msResult.User?.UserName;
				PlayerUuid = msResult.SelectedProfile?.Id.ToString("N");
				AccessToken = msResult.AccessToken;
				RefreshToken = msResult.RefreshToken ?? RefreshToken;
				TokenExpiry = DateTime.Now.AddSeconds(msResult.ExpiresIn);
				IsLoggedIn = true;
				SaveCachedLogin();
				LoginStateChanged?.Invoke();
				Log($"Silent login succeeded: player={PlayerName}");
				return true;
			}

			Log($"Silent login failed: {authResult.Error?.ErrorMessage ?? "unknown"}");
			return false;
		}
		catch (Exception ex)
		{
			Log($"TrySilentLoginAsync exception: {ex.Message}");
			return false;
		}
	}

	public static void Logout()
	{
		IsLoggedIn = false;
		PlayerName = null;
		PlayerUuid = null;
		AccessToken = null;
		RefreshToken = null;
		TokenExpiry = DateTime.MinValue;
		try { File.Delete(MsAuthCachePath); } catch { }
		LoginStateChanged?.Invoke();
	}

	private static async Task<GraphAuthResultModel?> RefreshTokenAsync(SimpleHttpClientFactory factory)
	{
		try
		{
			var client = factory.CreateClient();
			var dic = new[]
			{
				new KeyValuePair<string, string>("client_id", ApiSettings.ClientId),
				new KeyValuePair<string, string>("grant_type", "refresh_token"),
				new KeyValuePair<string, string>("refresh_token", RefreshToken!),
				new KeyValuePair<string, string>("scope", string.Join(' ', ApiSettings.Scopes))
			};

			var url = $"https://login.microsoftonline.com/{ApiSettings.TenentId}/oauth2/v2.0/token";
			using var req = new HttpRequestMessage(HttpMethod.Post, url);
			req.Content = new FormUrlEncodedContent(dic);
			using var res = await client.SendAsync(req);
			var content = await res.Content.ReadAsStringAsync();

			var parsed = MicrosoftAuthenticator.ResolveMSGraphResult(
				content,
				SerializerContext.Default.GraphAuthResultModel);

			return parsed as GraphAuthResultModel;
		}
		catch
		{
			return null;
		}
	}

	private static DefaultLauncherAccountParser CreateAccountParser()
	{
		var clientToken = Guid.NewGuid();
		var parser = new DefaultLauncherAccountParser(
			DownloadManager.MinecraftPath, clientToken)
		{
			LauncherAccount = new LauncherAccountModel
			{
				MojangClientToken = clientToken.ToString("N"),
				Accounts = new Dictionary<string, AccountModel>()
			}
		};
		return parser;
	}

	private static void SaveCachedLogin()
	{
		try
		{
			var data = new MsAuthCache
			{
				PlayerName = PlayerName,
				PlayerUuid = PlayerUuid,
				AccessToken = AccessToken,
				RefreshToken = RefreshToken,
				TokenExpiry = TokenExpiry
			};
			var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(MsAuthCachePath, json);
		}
		catch { }
	}

	private static void LoadCachedLogin()
	{
		try
		{
			if (!File.Exists(MsAuthCachePath)) return;
			var json = File.ReadAllText(MsAuthCachePath);
			var data = JsonSerializer.Deserialize<MsAuthCache>(json);
			if (data == null) return;

			PlayerName = data.PlayerName;
			PlayerUuid = data.PlayerUuid;
			AccessToken = data.AccessToken;
			RefreshToken = data.RefreshToken;
			TokenExpiry = data.TokenExpiry;

			if (!string.IsNullOrEmpty(PlayerName) && !string.IsNullOrEmpty(RefreshToken))
			{
				IsLoggedIn = true;
			}
		}
		catch { }
	}

	private class MsAuthCache
	{
		public string? PlayerName { get; set; }
		public string? PlayerUuid { get; set; }
		public string? AccessToken { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime TokenExpiry { get; set; }
	}

	public class DeviceCodeInfo
	{
		public string UserCode { get; set; } = "";
		public string VerificationUrl { get; set; } = "";
	}

	public readonly struct MsLoginResult
	{
		public bool Success { get; init; }
		public string? ErrorMessage { get; init; }

		public static MsLoginResult Succeeded() => new() { Success = true };
		public static MsLoginResult Failed(string error) => new() { Success = false, ErrorMessage = error };
	}

	private class SimpleHttpClientFactory : IHttpClientFactory
	{
		public HttpClient CreateClient(string name = "") => new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
	}
}