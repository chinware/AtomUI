# Fonts 模块概览

字体相关项目当前包括：

- `AtomUI.Fonts.AlibabaSans`
- `AtomUI.Fonts.AlibabaPuHuiTi`

## 职责

- 封装字体资源文件。
- 提供 Avalonia `FontCollection`。
- 提供 `IThemeManagerBuilder` 扩展方法，把字体集合加入 `FontManager.Current`。

## 注册入口

```csharp
builder.UseAlibabaSansFont();
builder.UseAlibabaPuHuiTiFont();
```

字体注册只把字体集合加入 Avalonia FontManager。默认字体族可以通过 `WithDefaultFontFamily()` 或 `UseAtomUI()` 内部 ThemeLoaded 回调写入共享 Token。

## 使用关系

`AtomUI.Controls` 直接依赖 `AtomUI.Fonts.AlibabaSans`。Browser Gallery 额外引用 `AtomUI.Fonts.AlibabaPuHuiTi`，用于展示环境的中文字体覆盖。

