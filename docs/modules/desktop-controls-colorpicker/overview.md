# AtomUI.Desktop.Controls.ColorPicker 模块概览

`AtomUI.Desktop.Controls.ColorPicker` 是桌面 ColorPicker 独立包，RootNamespace 为 `AtomUI.Desktop.Controls`。它依赖 `AtomUI.Desktop.Controls`、`AtomUI.Generator` 和 Avalonia 的 `Avalonia.Controls.ColorPicker`。

## 职责

- 提供普通颜色选择和渐变颜色选择。
- 提供 ColorView、ColorSpectrum、ColorSlider、GradientColorSlider 等内部组件。
- 提供颜色格式、HSV/RGB 辅助、透明背景画刷工具。
- 提供 ColorPicker Token、主题和本地化资源。
- 通过 `UseDesktopColorPicker()` 注册到 AtomUI 主题系统。

## 关键目录

| 目录 | 说明 |
|---|---|
| `ColorView/` | 色谱视图、颜色输入、渐变颜色视图 |
| `ColorSlider/` | 色相/透明度/渐变滑块和轨道 |
| `Themes/` | ColorPicker、ColorBlock、Palette、Gradient 主题 |
| `Localization/` | 多语言资源 |
| `Utils/` | HSV/RGB、步进、透明背景等工具 |

## 注册入口

`ThemeManagerBuilderExtensions.UseDesktopColorPicker()` 会注册：

- `ControlTokenTypePool.GetTokenTypes()`
- `AtomUIColorPickerThemesProvider`
- `LanguageProviderPool.GetLanguageProviders()`

具体 ColorPicker 使用/API 文档应放在 `docs/controls/desktop/data-entry/color-picker.md`。

