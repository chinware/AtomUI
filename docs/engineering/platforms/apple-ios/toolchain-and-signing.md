# Apple iOS 工具链与签名

本文维护可跨机器复用的 Xcode、.NET iOS workload、开发签名和 keychain 规则。精确版本、证书名称、Team ID 和安装路径只在
日期化验证 evidence 中记录。

## 工具链前置条件

开发机至少满足：

- Xcode 已安装并完成首次启动组件安装。
- Command Line Tools 的 Developer Directory 指向当前 Xcode。
- Xcode license 和 first-launch setup 已完成。
- 仓库要求的 .NET SDK 与 iOS workload 可被同一个 `dotnet` 入口发现。
- 多个 .NET 安装并存时，构建进程使用同一可执行文件和匹配的 `DOTNET_ROOT`，不混用 shell shim 与另一套 task host。

基础诊断命令：

```bash
xcodebuild -version
xcode-select -p
dotnet --info
dotnet workload list
```

升级 Xcode 后，应先在 Xcode 内完成组件安装，再确认 Developer Directory 和 license 状态。只有诊断证明当前选择错误时才
修改全局 `xcode-select`；CI runner 应在自己的 job 范围显式选择工具链，避免改变共享机器的交互式开发环境。

## 签名身份边界

iOS 开发设备安装使用 `Apple Development` 身份。`Developer ID Application` 用于 macOS Developer ID 分发，不能替代
iOS development certificate。App Store、Ad Hoc 和 Enterprise 使用不同 profile/证书组合，不属于本文流程。

一个可用的开发签名组合必须同时具备：

1. 由目标 Team 签发的 `Apple Development` certificate。
2. 与证书匹配且本机可访问的 private key。
3. 与应用 Bundle ID 精确匹配的 App ID。
4. 选择该 App ID、certificate 和目标 devices 的 iOS development provisioning profile。
5. 构建进程可以读取 certificate/private-key pair 和 profile。

## CSR、Certificate 与 Private Key

推荐流程：

1. 在 Keychain Access 创建 Certificate Signing Request。
2. 在 Apple Developer portal 创建 `Apple Development` certificate。
3. 下载并安装证书到开发用户可访问的 keychain。
4. 在 Keychain Access 确认证书下方存在配对的 private key。
5. 使用 codesigning policy 枚举身份：

```bash
security find-identity -v -p codesigning
```

CSR 邮箱用于团队识别，不决定签名有效性。只有 certificate 与生成 CSR 时的 private key 成对存在，构建进程才能完成签名。

## App ID、Profile 与设备

- App ID 的 Bundle ID 必须与 iOS 项目最终 `ApplicationId` 一致。
- Development profile 必须选择目标 App ID、有效 Apple Development certificate 和需要安装的设备。
- 新设备加入 Apple Developer portal 后，需要重新生成或更新 profile；旧 profile 不会自动获得设备 membership。
- `CodesignProvision` 使用 provisioning profile 的显示名称，不是 `.mobileprovision` 文件路径。

如果自动安装 profile 失败，可以先解析其 UUID，再放入当前用户的标准 Provisioning Profiles 目录；文件名使用 profile UUID，
而构建参数仍使用 profile name：

```bash
security cms -D -i <profile-file> > /tmp/ios-profile.plist
/usr/libexec/PlistBuddy -c 'Print :UUID' /tmp/ios-profile.plist
mkdir -p "$HOME/Library/MobileDevice/Provisioning Profiles"
cp <profile-file> "$HOME/Library/MobileDevice/Provisioning Profiles/<profile-uuid>.mobileprovision"
```

## Keychain 与 CI

本机交互式开发优先使用默认 login keychain，不默认传 `CodesignKeychain`。显式指定错误、未解锁或不在 search list 中的
keychain，可能让 .NET iOS 无法匹配原本可用的 signing identity。

只有以下情况才显式设置 keychain：

- CI 使用 job-scoped 临时 keychain。
- 开发机明确把 certificate/private key 放在非默认 keychain。
- 诊断已证明默认搜索路径不可用。

使用前先验证指定 keychain：

```bash
security find-identity -v -p codesigning <keychain-path>
```

CI 必须独立完成 keychain 创建、解锁、search-list 配置、certificate/private-key 导入和清理。证书密码、profile 和 keychain
密码属于 secret，不进入仓库、日志或正式文档示例。

## 签名门禁

构建前至少确认：

- `security find-identity` 能看到目标 Apple Development identity。
- Certificate 下存在 private key。
- Bundle ID、App ID 和 profile 完全一致。
- 目标设备包含在 development profile 中。
- `CodesignKey` 使用 identity 名称，`CodesignProvision` 使用 profile 名称。
- CI 或专用 keychain 已解锁并位于当前构建进程的 search list。
