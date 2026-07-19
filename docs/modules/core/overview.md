# AtomUI.Core 模块概览

`AtomUI.Core` 是 AtomUI 的基础设施项目，RootNamespace 为 `AtomUI`。它承载主题、Token、本地化、动画、MotionScene、资源加载、反射扩展和通用工具。

## 职责

- 提供 `AppBuilder.WithAtomUIDefaultOptions()` 和 `Application.UseAtomUI()` 两个主要入口。
- 管理唯一运行时 `ThemeManager`、五阶段主题事务、稳定 ThemeContext、局部作用域图以及独立 TopLevel 的
  context lease/resource bridge。
- 定义生成式全局/Control Token schema、不可变 `ThemeSnapshot`、稳定资源键与统一 Token 资源扩展。
- 定义语言系统：`LanguageVariant`、`LanguageProvider`、`LanguageResourceExtension`。
- 提供动画 Transition、MotionScene、颜色与几何工具。
- 依赖 `AtomUI.Native` 支撑窗口级底层能力。

## 关键目录

主题系统按照 [主题系统架构](theme-system.md) 的职责边界组织：

| 目录 | 说明 |
|---|---|
| `Theme/` | 唯一运行时 ThemeManager、五阶段事务、ThemeContext、作用域图、TopLevel lease/bridge 及公开主题入口 |
| `Theme/Algorithms/` | 算法契约、默认/暗色/紧凑计算器、调色板和颜色计算辅助 |
| `Theme/Configuration/` | ThemeConfig、ControlThemeConfig、配置规范化、继承与覆盖合并 |
| `Theme/Definitions/` | 主题来源、默认选择、XML v1、文件读取、语法模型、Schema 绑定和 diagnostics |
| `Theme/Schema/` | 生成式 Token schema、Control/算法 descriptor、ControlTheme asset descriptor 与 manifest 契约 |
| `Theme/Compilation/` | 纯 ThemeCompiler、不可变 ThemeSnapshot 和 Snapshot 缓存 |
| `Theme/Resources/` | Snapshot-backed ResourceProvider、统一 SharedTokenResource、单一生成式 ControlTokenScope.Identity、ControlTheme 聚合、resolver 和样式加载辅助 |
| `Theme/DesignTokens/` | 仅供编译阶段使用的 DesignToken、ControlToken builder、定义和 value converter |
| `Language/` | 本地化变体、Provider、资源扩展 |
| `Animations/` | Avalonia Transition 扩展 |
| `MotionScene/` | 进入、离开、移动、折叠等 Motion 抽象 |
| `Assets/Themes/` | 内置主题定义 |
| `Reflection/`、`Input/`、`Media/`、`Utils/` | 面向上层控件复用的内部工具 |

## 对外关系

`AtomUI.Core` 被 `AtomUI.Controls.Shared`、`AtomUI.Controls`、`AtomUI.Icons.*`、`AtomUI.Fonts.*`、`AtomUI.Desktop.Controls` 等项目引用。各控件包通过源生成器提供 Token descriptor、ControlTheme asset manifest 和稳定 identity；Core 冻结 registry 后不扫描上层程序集或 AXAML。它对多个上层项目开放 `InternalsVisibleTo`，因此修改内部 API 时需要同时检查上层控件包。

## 相关文档

- [主题系统架构](theme-system.md)
- [主题定义 XML v1 规范](theme-definition-xml.md)
- [AtomUI Theme Definition XML Schema v1](schemas/atomui-theme-v1.xsd)
- [../../architecture/startup-and-registration.md](../../architecture/startup-and-registration.md)
- [../../architecture/runtime-platforms.md](../../architecture/runtime-platforms.md)
