# AtomUI.Mobile.Controls 启动与打包

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

本文定义目标 `UseMobileControls(...)` 注册职责、多目标公共契约、静态资产注册和 NuGet/AOT 边界。所有 API 名称均为
已批准目标契约；真实签名、TFM、RID、包依赖和命令只能由实际项目与构建证据确认。

## `UseMobileControls(...)` 职责

目标入口与 `UseDesktopControls()` 平行，并接入现有 `UseAtomUI(...)` Builder。它负责：

1. 确保真正共享的 Common Controls 基础只注册一次。
2. 注册 Mobile Control descriptor、Own Token、ControlTheme asset manifest 和产品本地化资源。
3. 注册每 `TopLevel` Mobile Runtime scope 的 factory、attach 和 detach 回调。
4. 连接当前 Host 提供的平台 capability adapter。
5. 在启动/acquire 阶段拒绝缺失、重复或与 Host 不匹配的 adapter。
6. 注册必要的包级 initializer，不把平台生命周期和应用签名职责带入 Control 包。

目标入口不创建可变进程级 Overlay host、不扫描程序集发现 Control，也不通过 OS 名称选择 Public API。

## 多目标公共 API

- iOS 和 Android 目标暴露相同的 Public Control API、事件、Token identity 和本地化 key。
- 平台差异只实现 internal adapter 或 platform profile；Public API 不暴露平台 SDK 类型。
- Headless target/adapter 用于 Contract 和生命周期测试，不构成移动平台发布目标。
- 具体 TFM、RID 与 workload 由仓库集中构建配置和 Foundation 验证固定，不能在预实现文档中猜测。
- iOS-first 只决定实现与体验验收顺序，不允许 Android 编译契约在 Foundation 中缺席。

## 静态注册与 Generator

Mobile 沿用现有生成式注册模型：

| 资产 | 目标注册方式 |
| --- | --- |
| Control/Token descriptor | Generator 输出 exact CLR type、identity、Own Token schema 和强类型构造 |
| ControlTheme asset | 构建期校验并生成 manifest，保持 owner 和引用 identity |
| Localization | 静态 catalog descriptor 与内置 Translation Bundle 注册 |
| Runtime/adapter | 显式 factory 与 Host 提供关系，不进行运行时程序集扫描 |

新的 Generator 能力只有在现有 descriptor 模型无法表达 Mobile 稳定契约时才扩展。运行时反射、字符串成员访问和动态
factory 不能作为缺少静态设计的替代方案。

## Host 组合与校验

iOS/Android Host 在应用组合根完成平台初始化、生命周期桥接和 adapter 提供，再调用 AtomUI 注册链路。注册成功至少证明：

- 恰好存在一个与 Host 匹配的 adapter。
- 每个 `TopLevel` 只创建一个 Mobile Runtime scope。
- Theme、Localization 和 Control descriptor 在首个 Control 创建前已经冻结到有效 snapshot。
- Host detach 可以终止 scope-owned session、pointer chain、subscription 和 pending update。

缺失或重复 adapter 应给出可诊断的启动/acquire 失败；不可延迟到第一次 Overlay、IME 或 gesture 才暴露。

## NuGet 依赖边界

目标包的直接产品依赖是 `AtomUI.Controls`；需要 Avalonia 公共 API 无法表达的能力时引用 `AtomUI.Native`；
`AtomUI.Generator` 以构建期 Analyzer/生成基础设施参与。`AtomUI.Desktop.Controls`、Desktop DataGrid、Desktop
ColorPicker、Desktop Extras 和当前 Desktop-bound GalleryBase 都不得进入 Mobile 包的依赖闭包。

发布包只携带运行时必需程序集、Theme、Localization 和静态 descriptor 资产。平台 Host、签名 profile、设备部署配置和
Gallery 示例属于各自应用工程，不作为 Mobile Control NuGet 包的隐式副作用。

## AOT 与 Trimming

- 内置 Control、Token、Theme、Localization 和 adapter 注册使用显式 descriptor 或 Source Generator。
- DataTemplate、ControlTheme 和语言资源在裁剪后必须可发现。
- 非 Visual `AvaloniaObject` 的动态资源遵循 scoped resource host 生命周期。
- Analyzer 和普通 build 只能证明静态约束，不能替代 iOS/Android Release publish。
- Release 结论要求产物真实构建、安装并启动；两个平台分别记录。

## 当前状态

当前不存在 `UseMobileControls(...)` 实现、Mobile 包、目标 TFM 或发布产物。本页不能作为 NuGet 安装、平台构建或 AOT
通过证据；对应项目落地后必须用真实源码、测试和发布记录校准。
