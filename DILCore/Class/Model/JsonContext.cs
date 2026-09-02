using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using DILCore.Class.Model.CurseForge;
using DILCore.Class.Model.CurseForge.API;
using DILCore.Class.Model.Fabric;
using DILCore.Class.Model.Forge;
using DILCore.Class.Model.GameResource;
using DILCore.Class.Model.GameResource.ResolvedInfo;
using DILCore.Class.Model.LauncherAccount;
using DILCore.Class.Model.LauncherProfile;
using DILCore.Class.Model.LiteLoader;
using DILCore.Class.Model.Microsoft.Graph;
using DILCore.Class.Model.MicrosoftAuth;
using DILCore.Class.Model.Modrinth;
using DILCore.Class.Model.Mojang;
using DILCore.Class.Model.NeoForge;
using DILCore.Class.Model.Optifine;
using DILCore.Class.Model.Quilt;
using DILCore.Class.Model.ServerPing;
using DILCore.Class.Model.YggdrasilAuth;
using DILCore.DefaultComponent.Authenticator;
using DILCore.Services;
using AuthTokenRequestModel = DILCore.Class.Model.YggdrasilAuth.AuthTokenRequestModel;

namespace DILCore.Class.Model;

// Metadata also supports JsonContent's asynchronous write path. Only models serialized synchronously opt into
// the fast path below.
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(string[]),
    GenerationMode = JsonSourceGenerationMode.Metadata | JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(JsonElement[]))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(Dictionary<string, string[]>), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(AssetObjectModel))]
[JsonSerializable(typeof(RawVersionModel),
    GenerationMode = JsonSourceGenerationMode.Metadata | JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(GameRules[]))]
[JsonSerializable(typeof(JvmRules[]))]
[JsonSerializable(typeof(CurseForgeManifestModel),
    GenerationMode = JsonSourceGenerationMode.Metadata | JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(FeaturedQueryOptions))]
[JsonSerializable(typeof(FabricLoaderArtifactModel))]
[JsonSerializable(typeof(FabricModInfoModel))]
[JsonSerializable(typeof(ForgeInstallProfile))]
[JsonSerializable(typeof(LegacyForgeInstallProfile))]
[JsonSerializable(typeof(GameModInfoModel[]))]
[JsonSerializable(typeof(GameResourcePackModel))]
[JsonSerializable(typeof(ObjectResourcePackDescription[]))]
[JsonSerializable(typeof(NativeReplaceModel))]
[JsonSerializable(typeof(LauncherAccountModel),
    GenerationMode = JsonSourceGenerationMode.Metadata | JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(LauncherProfileModel),
    GenerationMode = JsonSourceGenerationMode.Metadata | JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(LiteLoaderDownloadVersionModel))]
[JsonSerializable(typeof(DeviceIdResponseModel))]
[JsonSerializable(typeof(GraphAuthResultModel))]
[JsonSerializable(typeof(GraphResponseErrorModel))]
[JsonSerializable(typeof(AuthMojangResponseModel))]
[JsonSerializable(typeof(AuthXBLRequestModel))]
[JsonSerializable(typeof(AuthXSTSErrorModel))]
[JsonSerializable(typeof(AuthXSTSRequestModel))]
[JsonSerializable(typeof(AuthXSTSResponseModel))]
[JsonSerializable(typeof(MojangErrorResponseModel))]
[JsonSerializable(typeof(MojangOwnershipResponseModel))]
[JsonSerializable(typeof(MojangProfileResponseModel))]
[JsonSerializable(typeof(ModrinthCategoryInfo[]))]
[JsonSerializable(typeof(ModrinthModPackIndexModel),
    GenerationMode = JsonSourceGenerationMode.Metadata | JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(ModrinthProjectDependencyInfo))]
[JsonSerializable(typeof(ModrinthProjectInfo[]))]
[JsonSerializable(typeof(ModrinthSearchResult))]
[JsonSerializable(typeof(ModrinthVersionInfo[]))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, ModrinthVersionInfo>))]
[JsonSerializable(typeof(UserProfile))]
[JsonSerializable(typeof(UserProfilePropertyValue))]
[JsonSerializable(typeof(VersionManifest))]
[JsonSerializable(typeof(NeoForgeVersionModel))]
[JsonSerializable(typeof(OptifineDownloadVersionModel))]
[JsonSerializable(typeof(QuiltLoaderModel))]
[JsonSerializable(typeof(QuiltSupportGameModel[]))]
[JsonSerializable(typeof(PingPayload))]
[JsonSerializable(typeof(AuthRefreshRequestModel))]
[JsonSerializable(typeof(AuthRequestModel))]
[JsonSerializable(typeof(AuthResponseModel))]
[JsonSerializable(typeof(AuthTokenRequestModel))]
[JsonSerializable(typeof(ErrorModel))]
[JsonSerializable(typeof(SignOutRequestModel))]
[JsonSerializable(typeof(McReqModel))]
[JsonSerializable(typeof(AddonInfoReqModel))]
[JsonSerializable(typeof(FileInfoReqModel))]
[JsonSerializable(typeof(FuzzyFingerPrintReqModel))]
[JsonSerializable(typeof(DataModelWithPagination<CurseForgeAddonInfo[]>))]
[JsonSerializable(typeof(DataModel<CurseForgeAddonInfo>))]
[JsonSerializable(typeof(DataModel<CurseForgeAddonInfo[]>))]
[JsonSerializable(typeof(DataModel<CurseForgeLatestFileModel[]>))]
[JsonSerializable(typeof(DataModelWithPagination<CurseForgeLatestFileModel[]>))]
[JsonSerializable(typeof(DataModel<CurseForgeSearchCategoryModel[]>))]
[JsonSerializable(typeof(DataModel<CurseForgeFeaturedAddonModel>))]
[JsonSerializable(typeof(DataModel<CurseForgeFuzzySearchResponseModel>))]
[JsonSerializable(typeof(DataModel<string>))]
public sealed partial class SerializerContext : JsonSerializerContext;
