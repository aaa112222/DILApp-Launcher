# DIL

A Minecraft launcher built with WPF and .NET 10.

## Project Structure

```
DIL/
├── DILApp/          # WPF application (launcher UI)
├── DILCore/         # Core library (launcher engine)
├── DIL.sln          # Visual Studio solution file
├── LICENSE          # MIT License
└── README.md        # This file
```

## Build

```bash
dotnet restore DIL.sln
dotnet build DIL.sln
```

## Publish

```bash
dotnet publish DILApp/DILApp.csproj -c Release -r win-x64
```

## Disclaimer / 免责声明

**English:**

This software (DIL) is provided "as is", without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose, and non-infringement. In no event shall the authors or copyright holders be liable for any claim, damages, or other liability, whether in an action of contract, tort, or otherwise, arising from, out of, or in connection with the software or the use or other dealings in the software.

1. **Minecraft Ownership**: This launcher is a third-party tool and is NOT affiliated with, endorsed by, or connected to Mojang Studios, Microsoft, or any of their subsidiaries or affiliates. Minecraft is a registered trademark of Mojang Studios. All Minecraft-related assets, names, and trademarks remain the exclusive property of their respective owners.

2. **Account Security**: This launcher uses the official Microsoft authentication flow (OAuth 2.0 with device code) to log in to your Minecraft account. Your credentials are processed through Microsoft's official authentication servers and are never stored, transmitted, or accessible to the developers of this software. However, use at your own risk. The developers are not responsible for any account-related issues, including but not limited to account suspension, data breaches, or unauthorized access.

3. **Data Safety**: While this launcher strives to handle your data responsibly, the developers make no guarantees regarding data integrity, loss, or corruption. Users are encouraged to maintain backups of their Minecraft installations, worlds, and configurations.

4. **Legal Compliance**: Users are solely responsible for ensuring that their use of this software complies with the Minecraft End User License Agreement (EULA), Mojang's Terms of Service, and all applicable local, national, and international laws. The developers do not encourage, endorse, or support any violation of these agreements or laws.

5. **No Support Obligation**: This software is provided as a community project. There is no guarantee of updates, bug fixes, feature additions, or any form of technical support. The developers reserve the right to discontinue development at any time without prior notice.

6. **Third-Party Dependencies**: This software relies on third-party libraries and services. The developers are not responsible for any issues arising from these dependencies, including but not limited to security vulnerabilities, service outages, or breaking changes.

7. **Age Restriction**: Minecraft is rated for players of certain age groups depending on jurisdiction. Users and guardians are responsible for ensuring age-appropriate use. The developers of this launcher assume no responsibility for age-related compliance.

8. **Modding and Modified Versions**: This launcher may support launching modded versions of Minecraft (Forge, Fabric, Quilt, etc.). The developers are not responsible for the content, safety, legality, or functionality of any mods, mod loaders, or modpacks. Users should exercise caution and only install mods from trusted sources.

9. **System Impact**: The developers are not responsible for any damage to your computer system, data loss, performance degradation, or other technical issues that may result from the installation, use, or removal of this software.

10. **Jurisdictional Limitations**: This disclaimer shall be interpreted and enforced in accordance with applicable law. If any provision of this disclaimer is found to be unenforceable or invalid, that provision shall be limited or eliminated to the minimum extent necessary so that the remaining provisions remain in full force and effect.

---

**中文：**

本软件（DIL）按"原样"提供，不附带任何明示或暗示的保证，包括但不限于适销性保证、特定用途适用性保证和非侵权保证。在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论该责任是基于合同、侵权或其他方式产生，且无论是否与本软件或本软件的使用或其他交易有关。

1. **Minecraft 所有权**：本启动器为第三方工具，与 Mojang Studios、Microsoft 或其任何子公司或关联公司无关，未获得其认可或授权。Minecraft 为 Mojang Studios 的注册商标。所有与 Minecraft 相关的资产、名称和商标均属于其各自所有者的专有财产。

2. **账户安全**：本启动器使用微软官方认证流程（OAuth 2.0 设备码授权）登录您的 Minecraft 账户。您的凭据通过微软官方认证服务器处理，不会被本软件的开发者存储、传输或访问。但仍需自行承担使用风险。开发者不对任何账户相关问题负责，包括但不限于账户暂停、数据泄露或未授权访问。

3. **数据安全**：尽管本启动器努力负责任地处理您的数据，开发者不对数据的完整性、丢失或损坏做出任何保证。建议用户对 Minecraft 安装、存档和配置进行备份。

4. **法律合规**：用户有责任确保其对本软件的使用符合 Minecraft 最终用户许可协议（EULA）、Mojang 服务条款以及所有适用的地方法律、国家法律和国际法律。开发者不鼓励、不支持任何违反上述协议或法律的行为。

5. **无支持义务**：本软件作为社区项目提供。不保证提供更新、错误修复、功能添加或任何形式的技术支持。开发者保留随时终止开发的权利，恕不另行通知。

6. **第三方依赖**：本软件依赖第三方库和服务。开发者不对因这些依赖产生的任何问题负责，包括但不限于安全漏洞、服务中断或破坏性变更。

7. **年龄限制**：Minecraft 根据不同司法管辖区设有年龄分级。用户和监护人应确保适龄使用。本启动器的开发者不承担与年龄合规相关的任何责任。

8. **Mod 和修改版本**：本启动器可能支持启动 Minecraft 的修改版本（Forge、Fabric、Quilt 等）。开发者不对任何 Mod、Mod 加载器或整合包的内容、安全性、合法性或功能性负责。用户应谨慎行事，仅从可信来源安装 Mod。

9. **系统影响**：开发者不对因本软件的安装、使用或卸载而可能导致的计算机系统损坏、数据丢失、性能下降或其他技术问题承担任何责任。

10. **司法管辖限制**：本免责声明应根据适用法律进行解释和执行。如果本免责声明中的任何条款被认定为不可执行或无效，则应在该必要最小范围内限制或消除该条款，以使剩余条款保持完全有效。

## Privacy Policy / 隐私政策

**English:**

Your privacy is important to us. This privacy policy explains what information DIL collects, how it is used, and what choices you have.

### 1. Information We Collect

**1.1 Information You Provide:**
- **Offline Player Name**: If you choose offline mode, the player name you enter is stored locally on your device for launcher configuration purposes. This name is not transmitted to any server except the Minecraft game server you connect to during gameplay.
- **Microsoft Account Token**: If you choose Microsoft login, the authentication tokens (access token and refresh token) obtained through Microsoft's official OAuth 2.0 device code flow are stored locally on your device. These tokens are used to authenticate with Minecraft services and are never sent to any third-party server other than Microsoft's and Mojang's official servers.

**1.2 Information Collected Automatically:**
- **Launcher Configuration**: Your preferences (selected Minecraft version, Java path, memory allocation, game directory, language setting, etc.) are stored locally in configuration files on your device.
- **Minecraft Version Metadata**: The launcher downloads and caches Minecraft version manifests from Mojang's official servers (piston-meta.mojang.com) to display available versions. This metadata is cached locally and refreshed periodically.
- **Game Installation Data**: Downloaded Minecraft game files, libraries, and assets are stored in the game directory you specify. These files are fetched from Mojang's official content delivery network and mod loader distribution servers (e.g., Forge, Fabric, Quilt).

**1.3 Information We Do NOT Collect:**
- We do **NOT** collect, store, or transmit your Microsoft account password at any time. The login process is handled entirely by Microsoft's authentication servers through a browser-based or device code flow.
- We do **NOT** collect personal identification information (real name, email address, phone number, etc.).
- We do **NOT** collect device hardware identifiers or unique device fingerprints.
- We do **NOT** collect usage analytics, telemetry, or tracking data.
- We do **NOT** collect your IP address.
- We do **NOT** use cookies or any tracking technologies.
- We do **NOT** share any data with third parties for advertising, marketing, or any other purpose.

### 2. How Information Is Used

- **Authentication tokens** are used solely to log in to your Minecraft account and launch the game. They are refreshed automatically when expired and are never shared with any party other than Microsoft and Mojang official services.
- **Launcher configuration** is used to remember your preferences and provide a personalized experience within the launcher.
- **Player name** is used to set your in-game display name and is only transmitted to the Minecraft server you connect to during gameplay.
- **Cached version metadata** is used to display available Minecraft versions and manage installations.

### 3. Data Storage and Security

- All data is stored **exclusively on your local device** in plain text or JSON format within the launcher's working directory or the Minecraft game directory.
- Authentication tokens are stored in a local file (`ms_auth.json`). While we do not encrypt these tokens locally, they are short-lived and can be revoked at any time through your Microsoft account security settings.
- We implement standard security practices in our code, but no method of electronic storage is 100% secure. You are responsible for maintaining the security of your device and the data stored on it.

### 4. Data Retention and Deletion

- All data is retained on your local device for as long as you use the launcher. You have full control over this data at all times.
- To delete all launcher data, you can:
  - Delete the launcher's configuration and authentication files in its working directory.
  - Delete the Minecraft game directory if you wish to remove all game-related data.
  - Uninstall the launcher application.
- Revoking Microsoft account access: You can revoke the launcher's access to your Microsoft account at any time by visiting [Microsoft Account Privacy Settings](https://account.microsoft.com/privacy) and removing the app's permission under "Apps and services".

### 5. Third-Party Services

This launcher interacts with the following third-party services. Each service has its own privacy policy:

| Service | Purpose | Privacy Policy |
|---------|---------|----------------|
| Microsoft Authentication (login.microsoftonline.com) | Account login via OAuth 2.0 | [Microsoft Privacy Statement](https://privacy.microsoft.com/) |
| Mojang API (piston-meta.mojang.com) | Minecraft version manifests | [Mojang Privacy Policy](https://www.minecraft.net/en-us/privacy) |
| Modrinth API (api.modrinth.com) | Mod browsing and downloading | [Modrinth Privacy Policy](https://modrinth.com/privacy) |
| Forge/Fabric/Quilt distribution servers | Mod loader installation | Respective project privacy policies |

The launcher does not control and is not responsible for the privacy practices of these third-party services. We encourage you to review their privacy policies.

### 6. Children's Privacy

This launcher does not knowingly collect personal information from children under the age of 13 (or the applicable age of consent in your jurisdiction). Since we do not collect personal information from any user regardless of age, our practices are inherently compliant with children's privacy regulations including COPPA and GDPR-K. Parents or guardians who have concerns should contact us through the project's GitHub repository.

### 7. International Users

This launcher is available globally. All data is stored locally on your device. No data is transmitted across international borders by the launcher itself. However, when you authenticate with Microsoft or download game files, those services may transfer data internationally according to their own privacy policies.

### 8. Your Rights Under GDPR and Other Regulations

If you are located in the European Economic Area (EEA), United Kingdom, or any region with data protection laws, you have the following rights regarding your locally stored data:
- **Right to Access**: You can view all data stored by the launcher in its local configuration files at any time.
- **Right to Rectification**: You can modify any locally stored data directly through the launcher's interface or by editing configuration files.
- **Right to Erasure**: You can delete all locally stored data at any time by deleting the launcher's files as described in Section 4.
- **Right to Data Portability**: Since all data is stored locally in standard formats (JSON, plain text), you can copy and transfer it freely.
- **Right to Object**: You can stop using the launcher at any time and delete all associated data.

Since we do not collect or process personal data on our servers, these rights can be exercised directly by you without needing to contact us.

### 9. Changes to This Privacy Policy

We may update this privacy policy from time to time. Any changes will be reflected in this document with an updated revision date. Continued use of the launcher after changes constitutes acceptance of the revised policy. We encourage you to review this policy periodically.

### 10. Contact

If you have questions or concerns about this privacy policy, please open an issue on the project's [GitHub repository](https://github.com/DIL/DIL/issues).

---

**中文：**

您的隐私对我们很重要。本隐私政策说明了 DIL 收集哪些信息、如何使用这些信息以及您有哪些选择。

### 1. 我们收集的信息

**1.1 您提供的信息：**
- **离线玩家名称**：如果您选择离线模式，您输入的玩家名称将存储在本地设备上，用于启动器配置。该名称不会传输到任何服务器，仅在游戏过程中连接 Minecraft 游戏服务器时使用。
- **微软账户令牌**：如果您选择微软登录，通过微软官方 OAuth 2.0 设备码授权流程获取的身份验证令牌（访问令牌和刷新令牌）将存储在本地设备上。这些令牌用于与 Minecraft 服务进行身份验证，绝不会发送到微软和 Mojang 官方服务器以外的任何第三方服务器。

**1.2 自动收集的信息：**
- **启动器配置**：您的偏好设置（选择的 Minecraft 版本、Java 路径、内存分配、游戏目录、语言设置等）以配置文件形式存储在本地设备上。
- **Minecraft 版本元数据**：启动器从 Mojang 官方服务器（piston-meta.mojang.com）下载并缓存 Minecraft 版本清单，以显示可用版本。该元数据在本地缓存并定期刷新。
- **游戏安装数据**：下载的 Minecraft 游戏文件、库和资源存储在您指定的游戏目录中。这些文件从 Mojang 官方内容分发网络和 Mod 加载器分发服务器（如 Forge、Fabric、Quilt）获取。

**1.3 我们不收集的信息：**
- 我们**绝不**在任何时候收集、存储或传输您的微软账户密码。登录过程完全由微软认证服务器通过设备码流程处理。
- 我们**绝不**收集个人身份信息（真实姓名、电子邮件地址、电话号码等）。
- 我们**绝不**收集设备硬件标识符或唯一设备指纹。
- 我们**绝不**收集使用分析、遥测或跟踪数据。
- 我们**绝不**收集您的 IP 地址。
- 我们**绝不**使用 Cookie 或任何跟踪技术。
- 我们**绝不**与任何第三方共享任何数据用于广告、营销或任何其他目的。

### 2. 信息的使用方式

- **身份验证令牌**仅用于登录您的 Minecraft 账户并启动游戏。令牌在过期时自动刷新，绝不会与微软和 Mojang 官方服务以外的任何方共享。
- **启动器配置**用于记住您的偏好设置并在启动器内提供个性化体验。
- **玩家名称**用于设置您的游戏内显示名称，仅在游戏过程中传输到您连接的 Minecraft 服务器。
- **缓存的版本元数据**用于显示可用的 Minecraft 版本并管理安装。

### 3. 数据存储和安全

- 所有数据**仅存储在您的本地设备上**，以纯文本或 JSON 格式保存在启动器工作目录或 Minecraft 游戏目录中。
- 身份验证令牌存储在本地文件（`ms_auth.json`）中。虽然我们不对这些令牌进行本地加密，但它们具有短暂的有效期，您可以随时通过微软账户安全设置撤销。
- 我们在代码中实施标准安全实践，但没有任何电子存储方式是 100% 安全的。您有责任维护设备安全和存储在其中的数据安全。

### 4. 数据保留和删除

- 所有数据在您使用启动器期间保留在本地设备上。您始终对这些数据拥有完全控制权。
- 要删除所有启动器数据，您可以：
  - 删除启动器工作目录中的配置和身份验证文件。
  - 如果您希望删除所有游戏相关数据，删除 Minecraft 游戏目录。
  - 卸载启动器应用程序。
- 撤销微软账户访问权限：您可以随时访问 [微软账户隐私设置](https://account.microsoft.com/privacy)，在"应用和服务"下移除应用的权限来撤销启动器对您微软账户的访问。

### 5. 第三方服务

本启动器与以下第三方服务交互。每个服务都有自己的隐私政策：

| 服务 | 用途 | 隐私政策 |
|------|------|----------|
| 微软认证（login.microsoftonline.com） | 通过 OAuth 2.0 登录账户 | [微软隐私声明](https://privacy.microsoft.com/) |
| Mojang API（piston-meta.mojang.com） | Minecraft 版本清单 | [Mojang 隐私政策](https://www.minecraft.net/en-us/privacy) |
| Modrinth API（api.modrinth.com） | Mod 浏览和下载 | [Modrinth 隐私政策](https://modrinth.com/privacy) |
| Forge/Fabric/Quilt 分发服务器 | Mod 加载器安装 | 各项目隐私政策 |

启动器不控制也不对上述第三方服务的隐私实践负责。我们建议您查阅它们的隐私政策。

### 6. 儿童隐私

本启动器不会故意收集 13 岁以下儿童（或您所在司法管辖区适用的同意年龄）的个人信息。由于我们不收集任何用户的个人信息，我们的做法天然符合儿童隐私法规，包括 COPPA 和 GDPR-K。如有疑虑的家长或监护人，可通过项目的 GitHub 仓库联系我们。

### 7. 国际用户

本启动器面向全球提供。所有数据存储在您的本地设备上。启动器本身不会跨境传输数据。但当您通过微软认证或下载游戏文件时，这些服务可能会根据其自身的隐私政策进行国际数据传输。

### 8. 您在 GDPR 和其他法规下的权利

如果您位于欧洲经济区（EEA）、英国或任何有数据保护法的地区，您对本地存储的数据享有以下权利：
- **访问权**：您可以随时在本地配置文件中查看启动器存储的所有数据。
- **更正权**：您可以通过启动器界面直接修改任何本地存储的数据，或编辑配置文件。
- **删除权**：您可以随时按照第 4 节所述删除所有本地存储的数据。
- **数据可携权**：由于所有数据以标准格式（JSON、纯文本）存储在本地，您可以自由复制和传输。
- **反对权**：您可以随时停止使用启动器并删除所有相关数据。

由于我们不在服务器上收集或处理个人数据，这些权利可以由您直接行使，无需联系我们。

### 9. 本隐私政策的变更

我们可能会不时更新本隐私政策。任何变更将反映在本文档中，并更新修订日期。在变更后继续使用启动器即表示您接受修订后的政策。我们建议您定期查看本政策。

### 10. 联系方式

如果您对本隐私政策有疑问或疑虑，请在项目的 [GitHub 仓库](https://github.com/aaa112222/DILApp-Launcher/issues) 中提交 Issue。

## Thanks
- corona studio: projbobcat
- PCL2: some of xaml
- Terracotta | 陶瓦联机

## License

This project is licensed under the [MIT License](LICENSE).