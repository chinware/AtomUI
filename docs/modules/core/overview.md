# AtomUI.Core 模块概览

`AtomUI.Core` 是 AtomUI 的基础设施项目，RootNamespace 为 `AtomUI`。它承载主题、Token、本地化、动画、MotionScene、资源加载、反射扩展和通用工具。

## 职责

- 提供 `AppBuilder.WithAtomUIDefaultOptions()` 和 `Application.UseAtomUI()` 两个主要入口。
- 管理 `ThemeManager`、`ThemeManagerBuilder`、`IThemeManagerBuilder` 和主题生命周期。
- 定义全局 Design Token、Control Token 基类、Token 资源键与 Token 资源扩展。
- 定义语言系统：`LanguageVariant`、`LanguageProvider`、`LanguageResourceExtension`。
- 提供动画 Transition、MotionScene、颜色与几何工具。
- 依赖 `AtomUI.Native` 支撑窗口级底层能力。

## 关键目录

| 目录 | 说明 |
|---|---|
| `Theme/` | 主题管理、主题资源、Token、主题算法 |
| `Theme/TokenSystem/` | DesignToken 与 ControlToken 基础类型和 Attribute |
| `Theme/Styling/` | 默认、暗色、紧凑主题算法和样式扩展 |
| `Language/` | 本地化变体、Provider、资源扩展 |
| `Animations/` | Avalonia Transition 扩展 |
| `MotionScene/` | 进入、离开、移动、折叠等 Motion 抽象 |
| `Assets/Themes/` | 内置主题定义 |
| `Reflection/`、`Input/`、`Media/`、`Utils/` | 面向上层控件复用的内部工具 |

## 对外关系

`AtomUI.Core` 被 `AtomUI.Controls.Shared`、`AtomUI.Controls`、`AtomUI.Icons.*`、`AtomUI.Fonts.*`、`AtomUI.Desktop.Controls` 等项目引用。它对多个上层项目开放 `InternalsVisibleTo`，因此修改内部 API 时需要同时检查上层控件包。

## 相关文档

- [../../architecture/startup-and-registration.md](../../architecture/startup-and-registration.md)
- [../../architecture/runtime-platforms.md](../../architecture/runtime-platforms.md)

