# Apple iOS 排障

本页按可观察症状组织有限诊断序列。每个序列先确认 owner 和证据，再决定是否修改证书、项目、资源或设备状态；不要同时
更换多个变量。

## 无法枚举签名证书

**症状：** 构建报告 `CSSM_ModuleLoad`、无法从 keychain 枚举 signing certificates，或找不到期望 identity。

诊断序列：

1. 运行 `security find-identity -v -p codesigning`，确认当前用户能看到目标 Apple Development identity。
2. 在 Keychain Access 确认证书下存在 private key。
3. 确认构建进程运行在可访问该用户 keychain 和 Xcode 服务的环境；受限 shell/CI 需要独立 keychain 配置。
4. 移除未经验证的 `CodesignKeychain` 参数，使用默认 login keychain 重试一次。
5. 只有 identity 确实缺失、过期或 private key 不匹配时才重建/重新导入证书。

## 指定 `CodesignKeychain` 后找不到 signing key

**症状：** 默认构建可以枚举身份，传入 `CodesignKeychain` 后反而报告 key 不存在。

诊断序列：

1. 确认参数指向实际 keychain，而不是证书或目录路径。
2. 运行 `security find-identity -v -p codesigning <keychain-path>`。
3. 确认 keychain 已解锁并在当前进程 search list 中。
4. 本机开发移除该参数并使用默认 login keychain。
5. CI 保留参数时，修正 job-scoped keychain 创建、导入、解锁和清理流程。

## LaunchScreen 未生效

**症状：** 启动显示空白/默认页，或修改 storyboard 后 bundle 行为不变。

诊断序列：

1. 检查 `Info.plist` 的 `UILaunchStoryboardName` 是否为资源名而非文件路径。
2. 检查项目是否以 `InterfaceDefinition` 包含 storyboard。
3. 检查最终 `<app-path>/LaunchScreen.storyboardc` 是否存在且时间戳已更新。
4. 卸载旧应用并重新安装新 bundle，排除已安装资产缓存。
5. 若只在特定设备错位，按测量流程重新校准，不直接复制另一设备的 constraint 常量。

## AppIcon 缺失、黑底或尺寸错误

**症状：** 主屏图标缺失、显示黑色背景，或仍使用旧尺寸/旧图。

诊断序列：

1. 检查项目是否以 `ImageAsset` 包含完整 `Assets.xcassets`。
2. 检查 AppIcon 名称与项目属性和 catalog metadata 一致。
3. 使用 `sips` 检查关键 PNG 的尺寸和 `hasAlpha`；正式图标使用明确不透明背景。
4. 确认最终 bundle 包含最新 `Assets.car`。
5. 卸载设备上的旧应用并重新安装，排除主屏图标缓存。

## Avalonia 资源不显示或启动失败

**症状：** SVG/图片不显示，或访问资源时应用在启动/页面创建阶段失败。

诊断序列：

1. 检查 URI 是否为 `avares://<assembly-name>/Assets/<resource-file>`。
2. 核对 assembly name、路径大小写和资源 Build Action。
3. 检查构建日志和最终 bundle 是否包含对应 Avalonia 资源。
4. 用一个已知存在的简单资源验证 URI/装配链路。
5. 只有资源 URI 与打包证据正确后，再调查图片解码器或平台兼容性。

## 真机安装成功但远程启动失败

**症状：** `devicectl` 已完成 install，但 process launch 报设备 locked/unavailable。

诊断序列：

1. 在设备上解锁屏幕并保持开发机配对连接。
2. 重新运行 device discovery，确认同一 `<device-id>` 可用。
3. 重试 process launch，不先重签或重装。
4. 若仍失败，再检查 bundle ID、设备日志和 Host 是否启动后立即退出。

锁屏启动错误本身不证明签名或安装失败。

## 修改资源后设备仍显示旧资产

**症状：** 源文件已更新，但设备仍显示旧 AppIcon、LaunchScreen 或应用资源。

诊断序列：

1. 确认本次构建输出路径和安装 `<app-path>` 是同一个 bundle。
2. 检查 `Assets.car`、`LaunchScreen.storyboardc` 或目标资源的时间戳/内容。
3. 终止旧进程，卸载已安装应用并重新安装。
4. 对 Simulator 执行同样的 uninstall/install，必要时重启实例。
5. 若 bundle 本身仍旧，清理目标项目的 stale build output 后只重建该 Host。

## 诊断记录

排障 evidence 至少记录：症状、命令退出码、关键日志、证书/profile owner、构建配置、设备状态、尝试过的单一变量和结果。
只有可重复结论才提升为稳定规则；机器特有路径、版本和一次 workaround 保留在日期化 progress 文档。
