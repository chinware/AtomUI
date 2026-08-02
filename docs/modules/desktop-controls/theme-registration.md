# Desktop Controls 主题注册

`AtomUI.Desktop.Controls` 的包级注册入口是 `UseDesktopControls()`。Control、Token 和主题资产通过约定与生成代码
进入该入口，不逐 Control 手工注册。主题系统完整契约见 [AtomUI 主题系统架构](../core/theme-system.md)，Control
开发规则见 [Control Token 设计规范](../../engineering/control-token-guidelines.md)。

## 注册流程

```mermaid
sequenceDiagram
    participant App as Application.UseAtomUI
    participant Builder as IThemeManagerBuilder
    participant Common as UseCommonControls
    participant Desktop as UseDesktopControls
    participant Registry as ThemeSchemaRegistry
    participant Theme as ThemeManager

    App->>Builder: 创建 ThemeManagerBuilder
    App->>Desktop: builder.UseDesktopControls()
    Desktop->>Common: 注册公共 Control 包
    Common->>Builder: 注册生成 descriptor / asset manifest
    Desktop->>Builder: 注册 Desktop 生成 manifest
    Desktop->>Builder: 注册 Desktop 或 Browser Theme Provider
    Desktop->>Builder: 注册语言 Provider 和初始化回调
    Builder->>Registry: 校验 type / identity / asset 并冻结 schema
    App->>Theme: Build + Configure + NotifyInitialized
```

冻结顺序不可交换。内置、第三方包和应用必须先注册完整 Global Token schema、带 exact CLR type/identity 的
Control descriptor 和 ControlTheme asset manifest。ThemeConfig 随后按“全部 Global Token + 当前 Control Own
Token”完成校验和规范化，不依赖主题消费清单。

## 生成注册内容

Control 包注册以下生成结果：

- `ControlTokenDescriptor`：每个对外可主题化 Control 的 exact CLR type、identity、可选 Own Token、强类型构造和
  资源投影；没有 Own Token 的 Control 也拥有 descriptor。
- `ControlThemeAssetManifest`：资产 URI、owner identity、引用的 Control identities、Semantic Part Theme 契约和
  静态校验结果。
- `XxxTokens.Identity`、`XxxTokenKey` 和 `XxxTokenResourceExtension`。
- Language Provider 和其他包级静态注册项。

运行时不扫描程序集、不解析 AXAML 文本，也不通过 TargetType、Control 继承或 ControlTheme `BasedOn` 推断 Token
identity。

Catalog 是包级 identity 边界：内置 AtomUI 包固定使用 `AtomUI`，第三方包由生成 target 默认使用项目
`AssemblyName`。主题资产的 owner identity 始终来自声明资产的包，即使其 `TargetType` 指向 Avalonia 或其他
外部 Control。

## Desktop 与 Browser Provider

`UseDesktopControls()` 根据 `RuntimePlatform.Features.SupportsNativeWindow` 选择：

- `DesktopControlThemesProvider`。
- `BrowserDesktopControlThemesProvider`。

公共 Control 包同样选择 `CommonControlThemesProvider` 或 `BrowserCommonControlThemesProvider`。Provider 只负责把
生成 manifest 中适用于当前平台的主题资产接入 Avalonia Styles；它不维护逐 Control 的手工 merged dictionary
清单，也不提供 Token scope。

## 新增 Control

标准目录：

```text
Rating/
+-- Rating.cs
+-- RatingToken.cs            optional
\-- Themes/
    \-- RatingTheme.axaml
```

只有一个 ControlTheme 时不增加源码包装层。多个主题资产直接加入 `Themes/`，由同一 manifest 注册。开发者不添加
`ControlThemeAssets` Attribute、Theme Module、聚合 AXAML、手工 identity 或逐 Theme 注册调用。构建系统可为
ResourceDictionary 和 typed ControlTheme 生成加载包装；这些生成资产不是 Control 作者需要维护的额外层级。

第三方包遵循同一规则，并只公开一次包级入口，例如 `UseAcmeControls()`。

## 初始化回调

Desktop Control 包在 ThemeManager 初始化后执行：

- 注册 `TransformOperations` 的自定义 Motion Animator。
- Desktop 环境下绑定 `ToolTipService`。
- Desktop 环境下启动 `MediaBreakPointThemeBootstrapper`。

这些回调安装运行时服务，不得承担 Token schema、Control identity 或主题资产的动态发现。
