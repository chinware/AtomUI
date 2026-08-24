# AtomUI.Controls 模块概览

`AtomUI.Controls` 是公共控件和通用 Primitives 模块。它依赖 `AtomUI.Core`、`AtomUI.Controls.Shared`、`AtomUI.Fonts.AlibabaSans`、`AtomUI.Icons.AntDesign` 和 `AtomUI.Generator`。

## 职责

- 提供桌面控件包复用的基础控件、抽象控件和 Primitives。
- 提供公共主题 Provider：`CommonControlThemesProvider` 与 `BrowserCommonControlThemesProvider`。
- 注册公共控件 Token、本地化 Catalog 和内置 Translation Bundle。
- 提供 Icon、ItemsControl、ScrollViewer、Form、Watermark、QRCode、Badge 抽象等公共能力。

## 注册入口

入口位于 `src/AtomUI.Controls/ThemeManagerBuildExtensions.cs`：

```csharp
builder.UseCommonControls();
```

该方法会：

1. 注册源生成器生成的公共控件 Token 类型。
2. 根据 `RuntimePlatform.Features.SupportsNativeWindow` 选择桌面或浏览器公共主题 Provider。
3. 通过生成式模块注册把公共 Catalog 和内置翻译加入 `builder.Localization`。

`UseDesktopControls()` 会先调用 `UseCommonControls()`，所以普通桌面应用无需单独调用本方法。

## 图片加载职责

`AtomUI.Controls` 拥有：

- `AsyncImage` 通用异步显示控件及 Theme。
- `ImageLoadController`，统一 `AsyncImage` 与 Avatar 的 generation、取消、fallback、progress、状态提交和结果租约。
- Avatar 的 `Source`/`FallbackSource`/`RequestOptions` 统一 API，并删除 `Src`、`BitmapSrc` 和 proposed `BitmapUrl`。
- 只匹配 `avares` Asset 的 trusted SVG codec，以及在 `UseCommonControls()` 中对该 codec 和 Shared image service 的显式注册。

Controls 不拥有 HTTP、memory/file cache、全局 scheduler 或第二个 loader。`UseCommonControls()` 幂等地补齐
Shared 默认图片注册并添加本包 SVG codec；`UseDesktopControls()` 继续通过 Common 得到同一个 application-scoped loader。
准确 API 和生命周期只在 [统一图片加载系统](../../architecture/systems/image-loading/overview.md)及其
[控件 API](../../architecture/systems/image-loading/control-apis.md)中定义；底层类型见
[公共契约](../../architecture/systems/image-loading/public-contracts.md)。

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
