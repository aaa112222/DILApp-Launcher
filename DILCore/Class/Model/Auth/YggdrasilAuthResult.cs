using DILCore.Class.Model.YggdrasilAuth;

namespace DILCore.Class.Model.Auth;

/// <summary>
///     验证结果类
/// </summary>
public class YggdrasilAuthResult : AuthResultBase
{
    /// <summary>
    ///     可用的Profiles
    /// </summary>
    public ProfileInfoModel[]? Profiles { get; set; }

    public string? LocalId { get; set; }
    public string? RemoteId { get; set; }
}