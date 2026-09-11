# Android 构建与验证

本文定义 Android Host 的 Debug/Release、target framework、ABI/RID、package artifact、trim/AOT 和运行验证要求。当前没有
Android Host 项目，因此不提供项目路径或未经执行的构建命令；真实命令只有在 Host 存在并完成执行后才加入。

## 构建契约

Foundation 需要明确并验证：

- 仓库集中管理的 Android target framework 和最低/目标平台策略。
- Host 输出类型、Application ID、manifest/resources 和 Mobile adapter 注册入口。
- Debug 与 Release 的 signing owner 和 artifact identity。
- 支持的 ABI/RID 与 Emulator/真机对应关系。
- 产物类型、安装入口和日志/诊断方式。
- trim/AOT/linker 配置及与 Avalonia、Theme、Localization、DataTemplate 的兼容性。

文档不得从 iOS TFM/RID、第三方示例或未执行的 CLI 片段推导 Android 生产配置。

## 验证层级

| 层级 | 必须证明 |
| --- | --- |
| Compile | 真实 Host、target framework、ABI/RID 和生成资产可以编译 |
| Package | Manifest、resources、Application ID、signing 和 artifact 内容有效 |
| Emulator | 安装、启动、system bars/back、IME、旋转和 Activity lifecycle 可运行 |
| Physical device | 触摸、输入、前后台、设备差异和 TalkBack 场景通过 |
| Release | Release signing、trim/AOT 产物构建、安装、启动和资源发现通过 |
| Publication | 发布渠道、版本、artifact digest 和日期可追溯 |

Compile 或 package 成功不能标记 Emulator/真机通过；Emulator Debug 不能替代 Release；Release build 未安装启动时仍是未验证。

## Adapter 注册

Host 启动必须证明：

- 恰好注册一个 Android capability adapter。
- 每个 `TopLevel` 只有一个 Mobile Runtime scope。
- Adapter 在 UI thread 边界投递 system bars、input pane、back、lifecycle 和 accessibility snapshot。
- 缺失/重复 adapter 在 startup/acquire 阶段产生可诊断失败。
- Activity recreation 后旧 subscription 和 owner 不残留。

## Trim/AOT 与资源

- Control/Token/Theme/Localization 使用显式 descriptor 或 Source Generator，不扫描程序集。
- Adapter、ControlTheme、DataTemplate、AutomationPeer 和 language bundle 在裁剪后仍可发现。
- Analyzer 通过不能替代真实 Release artifact。
- Release 产物必须在 Emulator 和至少一个 physical device 上安装并启动。
- AOT/trim mode、包体、启动和运行错误需要与构建配置一起记录，不能只保留“成功”结论。

## 启动与恢复

性能和稳定性记录至少区分冷/热启动、Emulator/真机、Debug/Release、Activity recreation 和 process restart。启动测量不能用
部署工具总耗时代替首帧；恢复验证需要确认 viewport 先刷新，Navigation/Overlay 不重复提交，adapter 和 Runtime scope 正确重建。

## 当前状态

Android Host、target framework、ABI/RID、artifact、命令和 Release 配置均未验证。Foundation 落地后，本页必须引用真实项目、
已执行构建记录和平台 evidence，再把对应状态逐项提升。
