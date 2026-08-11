# Android 工具链与签名

本文定义 Android 工具链和签名材料的责任模型。当前 Host 未实现、环境未验证，因此不固定 JDK、Android SDK、.NET Android
workload 版本、安装路径或成功命令。

## 工具链 Ownership

| 能力 | 本地开发 owner | CI owner | 版本固定证据 |
| --- | --- | --- | --- |
| .NET SDK / Android workload | 开发机 SDK 管理与 workload 安装 | runner image 或显式 workload provision | Host restore/build 与 Release 验证记录 |
| JDK | 与 .NET Android/Avalonia 目标兼容的 JDK 安装 | runner image、环境变量和缓存策略 | 官方约束加真实 build/package 证据 |
| Android SDK | platform、build-tools、platform-tools、emulator 和 system image | runner SDK provision 与 license 状态 | Emulator/真机构建部署证据 |
| AVD/system image | 本地场景覆盖 | CI Emulator image 与缓存 | boot/install/launch 和场景日志 |

Foundation 实现前只能写验证要求。版本只有同时满足官方兼容约束、仓库集中配置和实际 Host 构建/运行证据后，才进入正式支持
说明；一次开发机可用不能直接成为推荐版本。

## 环境边界

- 同一构建进程必须使用一致的 `dotnet`、JDK 和 Android SDK 解析结果。
- 本地 shell、IDE 和 CI 应记录各自解析来源，避免一个工具调用另一套 SDK。
- License、workload manifest、platform/build-tools 和 system image 缺失要在 toolchain 阶段诊断，不延迟到 adapter 运行。
- 机器绝对路径只进入日期化 evidence 或 CI 配置，不进入长期工程规范。

## Application Identity 与 Signing

Android Host 需要稳定 Application ID。Debug 与 Release signing identity 的 owner 必须明确：

- Debug signing 只证明开发部署，不等于 Release publication identity。
- Release keystore、alias、有效期和轮换策略属于发布资产。
- Keystore 文件、store/key password 和私钥材料属于 secret，不能提交到仓库或写入日志。
- CI 使用 secret store 和 job-scoped materialization；job 完成后清理临时文件。
- 本地开发只使用团队批准的开发 identity，不复制生产 secret 到普通工作区。

## 版本与签名证据门禁

在正式文档固定版本或签名流程前，至少具备：

1. 真实 Android Host 项目和集中 target framework 配置。
2. Toolchain discovery 结果及各工具的解析来源。
3. Debug package build、Emulator 安装和启动证据。
4. Release package 使用批准 signing identity 构建、安装和启动的证据。
5. trim/AOT、adapter、Theme、Localization 和 DataTemplate 在产物中的可发现性。
6. CI secret 注入、日志脱敏和临时 signing material 清理记录。

当前上述证据均未建立，因此本页不提供可复制的标准命令或推荐版本。
