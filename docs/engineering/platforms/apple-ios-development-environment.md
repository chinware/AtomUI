# Apple iOS 开发环境配置与真机部署

这份文档记录 AtomUI 使用 Avalonia/.NET 构建 iOS Demo、配置 Apple 签名、部署真机和使用模拟器验证的标准流程。它来自 AtomUI Gallery iOS 预览版的本机配置过程，重点保留可复用规则和已验证的排障结论。

最近验证环境：macOS + Xcode 26.x、.NET 10 Homebrew 安装、Avalonia 12.1、iPhone 11 Pro Max 真机、iOS 26.5 模拟器运行时。

移动平台开发环境文档按平台拆分维护。本文只覆盖 Apple/iOS；后续 Android 环境、签名、模拟器和真机部署规则应新增平行文档，例如 `android-development-environment.md`，不要混入本文。

## 适用范围

本文适用于：

- 创建或维护 AtomUI/Avalonia iOS Demo 工程。
- 在本机连接 iPhone 做开发签名、安装和启动。
- 使用 iOS Simulator 先验证布局、资源和启动页，再部署真机。
- 排查 .NET iOS 构建、Apple Development 证书、provisioning profile、AppIcon、LaunchScreen 和启动速度问题。

本文不覆盖 App Store/TestFlight 发布、企业签名、Push/Associated Domains 等需要额外 entitlement 的发布链路。

## 前置条件

本机需要准备：

- Xcode 已安装并完成首次启动组件安装。
- Xcode Command Line Tools 指向当前 Xcode。
- Homebrew 安装的 .NET SDK 可用。
- Apple Developer 账号已加入对应 Team。
- 真机已通过 USB 或网络与 Xcode 配对，并打开开发者模式。
- 需要部署的 iPhone 已加入 Apple Developer portal 的 Devices。

基础检查：

```bash
xcodebuild -version
xcode-select -p
xcrun devicectl list devices
/opt/homebrew/Cellar/dotnet/10.0.300/bin/dotnet --info
```

如果升级过 Xcode，先打开 Xcode 完成 Components 安装。必要时执行：

```bash
sudo xcode-select -s /Applications/Xcode.app/Contents/Developer
sudo xcodebuild -license accept
sudo xcodebuild -runFirstLaunch
```

## Apple 证书与 Profile

开发真机包使用 `Apple Development` 证书，不使用 `Developer ID Application`。后者是 macOS Developer ID 签名，不适用于 iOS 真机开发安装。

推荐流程：

1. 在 Keychain Access 中创建 Certificate Signing Request。
2. 在 Apple Developer portal 创建 `Apple Development` certificate。
3. 下载并双击安装证书。
4. 在 Keychain Access 确认证书位于 login keychain，并且证书下方带有 private key。
5. 创建 App ID，Bundle ID 使用项目实际值，例如 `com.atomui.isogallery`。
6. 创建 `iOS App Development` provisioning profile。
7. Profile 选择对应 App ID、Apple Development 证书和目标设备。
8. 下载 `.mobileprovision` 并安装。

CSR 的邮箱建议填写 Apple Developer 账号或团队邮箱，便于识别；关键是证书必须和本机 private key 成对存在。

验证可用签名身份：

```bash
security find-identity -v -p codesigning
```

输出中应能看到类似：

```text
"Apple Development: <name> (<team-id>)"
```

如果双击 `.mobileprovision` 没有安装，可手动复制到 Xcode/Apple 工具链默认目录：

```bash
security cms -D -i AtomUI_iOS_Gallery_Development.mobileprovision > /tmp/atomui-profile.plist
/usr/libexec/PlistBuddy -c 'Print :UUID' /tmp/atomui-profile.plist
mkdir -p "$HOME/Library/MobileDevice/Provisioning Profiles"
cp AtomUI_iOS_Gallery_Development.mobileprovision "$HOME/Library/MobileDevice/Provisioning Profiles/<profile-uuid>.mobileprovision"
```

构建命令中的 `CodesignProvision` 使用 profile 名称，不是 `.mobileprovision` 文件路径。

## iOS Demo 工程结构

最小 Avalonia iOS 工程应包含：

```text
AtomUI.IosDemo.csproj
App.cs
MainView.cs
Assets/
Platforms/iOS/AppDelegate.cs
Platforms/iOS/Entitlements.plist
Platforms/iOS/Info.plist
Platforms/iOS/Resources/Assets.xcassets/
Platforms/iOS/Resources/LaunchScreen.storyboard
```

`AppDelegate` 使用 Avalonia iOS host：

```csharp
[Register("AppDelegate")]
public class AppDelegate : AvaloniaAppDelegate<App>
{
    protected override AppBuilder CreateAppBuilder() => App.BuildAvaloniaApp().UseiOS();

    internal static void Main(string[] args) => UIApplication.Main(args, null, typeof(AppDelegate));
}
```

项目文件需要显式包含 iOS 资源：

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <TargetFramework>net10.0-ios</TargetFramework>
  <SupportedOSPlatformVersion>13.0</SupportedOSPlatformVersion>
  <AvaloniaSingleProject>true</AvaloniaSingleProject>
  <ApplicationTitle>AtomUI Gallery</ApplicationTitle>
  <ApplicationId>com.atomui.isogallery</ApplicationId>
  <AppIcon>AppIcon</AppIcon>
  <IncludeAllAppIcons>true</IncludeAllAppIcons>
  <AppBundleManifest>Platforms/iOS/Info.plist</AppBundleManifest>
  <CodesignEntitlements>Platforms/iOS/Entitlements.plist</CodesignEntitlements>
</PropertyGroup>

<ItemGroup>
  <AvaloniaResource Include="Assets/**/*.*" />
  <ImageAsset Include="Platforms/iOS/Resources/Assets.xcassets/**/*.*" />
  <InterfaceDefinition Include="Platforms/iOS/Resources/LaunchScreen.storyboard" />
</ItemGroup>
```

SVG 等 Avalonia 资源使用 assembly resource URI，不要使用文件系统相对路径：

```csharp
Path = "avares://AtomUI.IosDemo/Assets/atomui-oss.svg";
```

资源 URI 错误会表现为启动后闪退或资源不显示，不应先假设 SVG 包不支持 iOS。

## App Icon

AppIcon 必须放在 asset catalog 中，并通过 `ImageAsset` 编译。最终 `.app` 中应出现 `Assets.car`。

```text
Platforms/iOS/Resources/Assets.xcassets/AppIcon.appiconset/
```

注意事项：

- iOS App Icon 不要依赖透明背景。
- PNG 应合成为不透明背景，AtomUI 当前预览版使用白底。
- 如果图标显示黑底，优先检查 PNG 是否带 alpha。
- `IncludeAllAppIcons` 可以保留，避免某些尺寸未被打入 catalog。

验证：

```bash
sips -g pixelWidth -g pixelHeight -g hasAlpha Platforms/iOS/Resources/Assets.xcassets/AppIcon.appiconset/icon-1024.png
sips -g pixelWidth -g pixelHeight -g hasAlpha Platforms/iOS/Resources/Assets.xcassets/AppIcon.appiconset/icon-60@3x.png
find bin/Release/net10.0-ios/ios-arm64/*.app -maxdepth 1 -name Assets.car -print
```

`hasAlpha` 应为 `no`。

## LaunchScreen

启动页必须使用 iOS 原生 storyboard，并通过 `InterfaceDefinition` 编译；只把 `.xib` 或 `.storyboard` 当普通文件复制不会生效。

`Info.plist` 中需要：

```xml
<key>UILaunchStoryboardName</key>
<string>LaunchScreen</string>
```

最终 `.app` 中应出现：

```text
LaunchScreen.storyboardc
```

验证：

```bash
/usr/libexec/PlistBuddy -c 'Print :UILaunchStoryboardName' bin/Release/net10.0-ios/ios-arm64/<AppName>.app/Info.plist
find bin/Release/net10.0-ios/ios-arm64/<AppName>.app -maxdepth 1 -name LaunchScreen.storyboardc -print
```

### Splash 与正式页面坐标

不要靠肉眼反复调整 LaunchScreen 的 `constant`。UIKit LaunchScreen 和 Avalonia 页面虽然都使用 point 作为布局单位，但它们不是同一个布局树：

- LaunchScreen 是 UIKit storyboard 根视图坐标。
- 正式页面是 Avalonia `TopLevel`、root view、padding、safe area 与控件测量后的坐标。
- 同一张 logo 图片和 SVG 控件还可能存在素材 viewBox、透明边界和可见边界差异。

需要无缝切换时，先测量正式页面 logo 的真实位置：

1. 在同型号模拟器或真机上运行正式页面。
2. 使用 Avalonia `TranslatePoint`、`Bounds` 或截图像素检测记录 logo 可见中心。
3. 用同一目标设备的 UIKit point 坐标计算 LaunchScreen 约束。
4. 重新编译 `LaunchScreen.storyboard`，验证 `.app/LaunchScreen.storyboardc` 更新。
5. 在模拟器截图和真机上复核切换是否跳动。

本次 iPhone 11 Pro Max 验证中，错误路径是把 Splash 写成固定 `centerY` 偏移或单一比例。正确做法是根据正式页面 Grid 布局和实际测量值推导约束，例如使用 `screenHeight` 的比例项加常量项，而不是盲调：

```xml
<constraint firstItem="launch-logo"
            firstAttribute="centerY"
            secondItem="root-view"
            secondAttribute="bottom"
            multiplier="0.31"
            constant="44.1" />
```

上面的数值只适用于当前页面布局和已验证设备族；页面结构或 logo 尺寸变化后必须重新测量。

## 构建命令

本机优先使用 Homebrew Cellar 中真实的 dotnet 路径，避免 shell shim、IDE task host 或 `DOTNET_ROOT` 不一致导致 .NET iOS task host 握手异常。

```bash
export DOTNET_ROOT=/opt/homebrew/Cellar/dotnet/10.0.300/libexec
export DOTNET_ROOT_ARM64=/opt/homebrew/Cellar/dotnet/10.0.300/libexec
export AVALONIA_TELEMETRY_OPTOUT=1
export DOTNET_BIN=/opt/homebrew/Cellar/dotnet/10.0.300/bin/dotnet
```

Debug 真机构建：

```bash
env -u SDKROOT \
DOTNET_ROOT="$DOTNET_ROOT" \
DOTNET_ROOT_ARM64="$DOTNET_ROOT_ARM64" \
AVALONIA_TELEMETRY_OPTOUT=1 \
"$DOTNET_BIN" build <ios-csproj> \
-f net10.0-ios -c Debug \
-p:RuntimeIdentifier=ios-arm64 \
-p:CodesignKey="Apple Development: <name> (<team-id>)" \
-p:CodesignProvision="<profile-name>" \
-v:minimal
```

Release 真机构建：

```bash
env -u SDKROOT \
DOTNET_ROOT="$DOTNET_ROOT" \
DOTNET_ROOT_ARM64="$DOTNET_ROOT_ARM64" \
AVALONIA_TELEMETRY_OPTOUT=1 \
"$DOTNET_BIN" build <ios-csproj> \
-f net10.0-ios -c Release \
-p:RuntimeIdentifier=ios-arm64 \
-p:CodesignKey="Apple Development: <name> (<team-id>)" \
-p:CodesignProvision="<profile-name>" \
-v:minimal
```

首次 Release 构建会执行 trim、link、AOT，可能需要数分钟。增量构建通常明显更快。

不要默认传 `-p:CodesignKeychain=...`。本次验证中，显式传 login keychain 反而让 .NET iOS signing detection 报 signing key 找不到。优先让 .NET 从默认 login keychain 枚举证书；只有在 CI 或专用 keychain 中才考虑显式传 keychain，并单独验证。

## 真机安装与启动

列出设备：

```bash
xcrun devicectl list devices
```

安装：

```bash
xcrun devicectl device install app \
--device <device-id> \
bin/Release/net10.0-ios/ios-arm64/<AppName>.app
```

启动：

```bash
xcrun devicectl device process launch \
--device <device-id> \
--terminate-existing \
<bundle-id>
```

如果设备锁屏，启动会失败：

```text
Unable to launch <bundle-id> because the device was not, or could not be, unlocked.
```

安装不要求设备保持亮屏，但远程启动要求 iPhone 已解锁。

## 模拟器验证

优先创建与真机同型号或同尺寸的模拟器验证布局和启动页。

列出运行时和设备类型：

```bash
xcrun simctl list runtimes available
xcrun simctl list devicetypes
```

创建同型号模拟器：

```bash
xcrun simctl create AtomUI-iPhone11ProMax \
com.apple.CoreSimulator.SimDeviceType.iPhone-11-Pro-Max \
com.apple.CoreSimulator.SimRuntime.iOS-26-5
```

启动并打开 Simulator：

```bash
xcrun simctl boot <simulator-id>
xcrun simctl bootstatus <simulator-id> -b
open -a Simulator
```

构建模拟器版本：

```bash
env -u SDKROOT \
DOTNET_ROOT="$DOTNET_ROOT" \
DOTNET_ROOT_ARM64="$DOTNET_ROOT_ARM64" \
AVALONIA_TELEMETRY_OPTOUT=1 \
"$DOTNET_BIN" build <ios-csproj> \
-f net10.0-ios -c Debug \
-p:RuntimeIdentifier=iossimulator-arm64 \
-v:minimal
```

安装、启动、截图：

```bash
xcrun simctl install <simulator-id> bin/Debug/net10.0-ios/iossimulator-arm64/<AppName>.app
xcrun simctl launch <simulator-id> <bundle-id>
xcrun simctl io <simulator-id> screenshot /tmp/atomui-ios-screenshot.png
```

模拟器截图是验证 Splash/正式页面坐标关系的首选工具；手机拍屏会有透视、刷新和曝光误差，只适合最终体验确认。

## 启动速度判断

不要用 Debug 包评估启动速度。Debug iOS 包会带调试、诊断、热重载或更少优化的运行时路径，Avalonia/.NET 首屏可能明显慢于 Release。

性能判断规则：

- 用户可感知启动速度以 Release 真机包为准。
- Splash 必须是原生 LaunchScreen，保证 .NET/Avalonia runtime 初始化前已有首屏。
- 远程 `devicectl launch` 的命令耗时不是用户点击图标后的真实首屏时间。
- 修改页面、资源或启动页后，都部署 Release 包再判断体验。

本次验证中，极简页面 Debug 首屏约数秒，Release 真机启动明显变快，因此不能把 Debug 表现作为产品体验结论。

## 常见问题

### `CSSM_ModuleLoad` 或无法枚举签名证书

现象：

```text
CSSM_ModuleLoad(): One or more parameters passed to a function were not valid.
Could not enumerate signing certificates from the keychain.
```

处理：

- 先运行 `security find-identity -v -p codesigning`，确认 login keychain 中证书和 private key 可见。
- 不要急着重建证书；证书本身可能是好的。
- 确认构建进程运行在能访问 macOS login keychain 和 Xcode 服务的真实用户环境中。
- 在 Codex、CI 或受限 shell 中遇到该错误时，改用允许访问钥匙串和 Xcode 工具链的执行环境。
- 移除不必要的 `CodesignKeychain` 显式参数。

### 传了 `CodesignKeychain` 后提示 signing key 找不到

本机开发优先不传 `CodesignKeychain`。让 .NET iOS task 从默认 login keychain 匹配 `CodesignKey` 通常更稳。

只有 CI 使用临时 keychain，或本机明确使用非默认 keychain 时，才传：

```bash
-p:CodesignKeychain=<keychain-path>
```

并先用 `security find-identity -v -p codesigning <keychain-path>` 验证。

### LaunchScreen 没生效

检查：

- `Info.plist` 中有 `UILaunchStoryboardName=LaunchScreen`。
- `.csproj` 中有 `<InterfaceDefinition Include="Platforms/iOS/Resources/LaunchScreen.storyboard" />`。
- 最终 `.app` 中有 `LaunchScreen.storyboardc`。

只复制 `.xib` 或 `.storyboard` 到 bundle 不是有效启动页配置。

### 桌面图标缺失或黑底

检查：

- `.csproj` 中有 `<ImageAsset Include="Platforms/iOS/Resources/Assets.xcassets/**/*.*" />`。
- 最终 `.app` 中有 `Assets.car`。
- AppIcon PNG 不带 alpha。
- `Info.plist` 或生成后的 plist 中 `CFBundleIcons` 指向 `AppIcon`。

### SVG 不显示或启动闪退

先检查 Avalonia resource URI：

```text
avares://<AssemblyName>/Assets/<file>.svg
```

不要使用本地文件路径，也不要先假设 `Svg.Controls.Avalonia` 不支持 iOS。

### 真机安装成功但无法远程启动

如果报 `Locked`，解锁 iPhone 后重试。这个错误不代表签名或安装失败。

### 修改资源后手机仍显示旧图标

iOS 可能缓存桌面图标。确认新包有 `Assets.car` 且 AppIcon PNG 已更新后，必要时卸载 App 再重新安装。

## 收尾验证清单

每次部署前至少确认：

```bash
dotnet build <ios-csproj> -f net10.0-ios -c Release -p:RuntimeIdentifier=ios-arm64 ...
find bin/Release/net10.0-ios/ios-arm64/<AppName>.app -maxdepth 1 -name Assets.car -print
find bin/Release/net10.0-ios/ios-arm64/<AppName>.app -maxdepth 1 -name LaunchScreen.storyboardc -print
/usr/libexec/PlistBuddy -c 'Print :CFBundleDisplayName' -c 'Print :CFBundleIdentifier' -c 'Print :UILaunchStoryboardName' bin/Release/net10.0-ios/ios-arm64/<AppName>.app/Info.plist
xcrun devicectl device install app --device <device-id> bin/Release/net10.0-ios/ios-arm64/<AppName>.app
```

如果这几项通过，再让用户在真机上手动点击 App 评估首屏和交互体验。
