using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using DILCore.Class.Helper.Download;
using DILCore.Interface;
using DILCore.Interface.Services;
using DILCore.Services;

namespace DILCore.Class.Helper;

public static class InitHelper
{
    public static IServiceCollection UseDILCore(
        this IServiceCollection services,
        Func<ILauncherCoreSettingsProvider> coreSettingsProvider)
    {
        var coreSettings = coreSettingsProvider();
        var userAgent = coreSettings.DefaultUserAgent ?? DownloadHelper.DefaultUserAgent;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        services.AddSingleton(_ => coreSettings);

        services.AddHttpClient(DownloadHelper.DefaultDownloadClientName,
            client => { client.DefaultRequestHeaders.Add("User-Agent", userAgent); });
        services.AddHttpClient(DownloadHelper.DefaultCurseForgeDownloadClientName,
            client =>
            {
                client.DefaultRequestHeaders.Add("x-api-key", coreSettings.CurseForgeApiKey);
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            });

        services.AddHttpClient<IModrinthApiService, ModrinthApiService>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        });

        services.AddHttpClient<ICurseForgeApiService, CurseForgeApiService>(client =>
        {
            client.DefaultRequestHeaders.Add("x-api-key", coreSettings.CurseForgeApiKey);
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        });

        return services;
    }
}