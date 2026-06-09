# AtomUI.Controls 模块概览

`AtomUI.Controls` 是公共控件和通用 Primitives 模块。它依赖 `AtomUI.Core`、`AtomUI.Controls.Shared`、`AtomUI.Fonts.AlibabaSans`、`AtomUI.Icons.AntDesign` 和 `AtomUI.Generator`。

## 职责

- 提供桌面控件包复用的基础控件、抽象控件和 Primitives。
- 提供公共主题 Provider：`CommonControlThemesProvider` 与 `BrowserCommonControlThemesProvider`。
- 注册公共控件 Token 和语言 Provider。
- 提供 Icon、ItemsControl、ScrollViewer、Form、Watermark、QRCode、Badge 抽象等公共能力。

## 注册入口

入口位于 `src/AtomUI.Controls/ThemeManagerBuildExtensions.cs`：

```csharp
themeManagerBuilder.UseCommonControls();
```

该方法会：

1. 注册源生成器生成的公共控件 Token 类型。
2. 根据 `RuntimePlatform.Features.SupportsNativeWindow` 选择桌面或浏览器公共主题 Provider。
3. 注册公共语言 Provider。

`UseDesktopControls()` 会先调用 `UseCommonControls()`，所以普通桌面应用无需单独调用本方法。

## 关键目录

| 目录 | 说明 |
|---|---|
| `Primitives/` | 桌面控件复用的基础装饰、动效、视觉层 |
| `Icon/` | 图标控件与图标主题 |
| `ItemsControl/` | ItemsControl 相关反射扩展和主题 |
| `Form/` | 表单验证基础能力 |
| `Grid/`、`FlexPanel/` | 布局基础 |
| `Badge/`、`Buttons/`、`Select/` | 多个桌面控件复用的抽象层 |

具体桌面控件文档应放在 [../../controls/desktop/overview.md](../../controls/desktop/overview.md)，本模块文档只解释公共基础层。

