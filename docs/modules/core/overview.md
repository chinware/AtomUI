# AtomUI.Core 模块概览

`AtomUI.Core` 是 AtomUI 的基础设施项目，RootNamespace 为 `AtomUI`。它承载主题、Token、本地化、动画、MotionScene、资源加载、反射扩展和通用工具。

## 职责

- 提供 `AppBuilder.WithAtomUIDefaultOptions()` 和 `Application.UseAtomUI()` 两个主要入口。
- 管理 `ThemeManager`、`ThemeManagerBuilder`、`IThemeManagerBuilder`、ThemeEngine 和主题事务。
- 定义全局 Design Token、Control Token 基类、Token 资源键与 Token 资源扩展。
- 定义语言系统：`LanguageVariant`、`LanguageProvider`、`LanguageResourceExtension`。
- 提供动画 Transition、MotionScene、颜色与几何工具。
- 依赖 `AtomUI.Native` 支撑窗口级底层能力。

## 关键目录

主题系统按照 [主题系统架构](theme-system.md) 的职责边界组织：

| 目录 | 说明 |
|---|---|
| `Theme/` | ThemeManager、ThemeEngine、事务、ThemeContext、作用域图及公开主题入口 |
| `Theme/Algorithms/` | 算法契约、默认/暗色/紧凑计算器、调色板和颜色计算辅助 |
| `Theme/Configuration/` | ThemeConfig、ControlThemeConfig、配置规范化、继承与覆盖合并 |
| `Theme/Definitions/` | 主题来源、默认选择、XML v1、文件读取、语法模型、Schema 绑定和 diagnostics |
| `Theme/Schema/` | 生成式 Token schema、Control descriptor 和算法 descriptor |
| `Theme/Compilation/` | 纯 ThemeCompiler、不可变 ThemeSnapshot 和 Snapshot 缓存 |
| `Theme/Resources/` | Snapshot-backed ResourceProvider、ControlTheme 发现与聚合、资源键、resolver 和样式加载辅助 |
| `Theme/Tokens/` | 仅供编译阶段使用的 DesignToken、ControlToken builder、定义和 value converter |
| `Language/` | 本地化变体、Provider、资源扩展 |
| `Animations/` | Avalonia Transition 扩展 |
| `MotionScene/` | 进入、离开、移动、折叠等 Motion 抽象 |
| `Assets/Themes/` | 内置主题定义 |
| `Reflection/`、`Input/`、`Media/`、`Utils/` | 面向上层控件复用的内部工具 |

## 对外关系

`AtomUI.Core` 被 `AtomUI.Controls.Shared`、`AtomUI.Controls`、`AtomUI.Icons.*`、`AtomUI.Fonts.*`、`AtomUI.Desktop.Controls` 等项目引用。它对多个上层项目开放 `InternalsVisibleTo`，因此修改内部 API 时需要同时检查上层控件包。

## 相关文档

- [主题系统架构](theme-system.md)
- [主题定义 XML v1 规范](theme-definition-xml.md)
- [AtomUI Theme Definition XML Schema v1](schemas/atomui-theme-v1.xsd)
- [../../architecture/startup-and-registration.md](../../architecture/startup-and-registration.md)
- [../../architecture/runtime-platforms.md](../../architecture/runtime-platforms.md)
