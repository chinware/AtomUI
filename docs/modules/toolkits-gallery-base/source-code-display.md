# GalleryBase 源码展示模块架构设计

本文档定义 GalleryBase 中 ShowCase 源码展示模块的最新设计状态。该模块用于在 Gallery 示例卡片中打开当前示例的源码视图，让用户在不离开 ShowCase 页面、不打断示例运行状态的前提下查看示例 AXAML 和相关代码片段。

源码展示模块由 GalleryBase 运行时控件、产品侧源码片段 Provider 和 GalleryBase 专用 Source Generator 三部分组成。GalleryBase 只提供产品中立的 UI、交互和运行时契约；具体产品 Gallery 负责提供源码片段数据；Source Generator 负责把产品 ShowCase 源文件转换成 AOT 友好的强类型 catalog。

## 1. 模块定位

源码展示模块是 `AtomUI.Toolkits.GalleryBase` 的 ShowCase 辅助能力，服务于 `ShowCaseItem` 示例浏览场景。

模块职责：

- 在 `ShowCaseItem` 上提供查看源码入口。
- 使用共享 Drawer 展示源码，不把大段代码嵌入示例卡片内部。
- 使用多标签只读编辑器展示 AXAML、code-behind、ViewModel 等源码片段。
- 延迟加载源码片段和编辑器实例，避免增加 ShowCase 首屏创建成本。
- 通过产品侧 Provider 获取源码片段，不在 GalleryBase 中引用具体产品页面。
- 通过 Source Generator 生成产品侧源码片段 catalog，避免运行时文件扫描和反射扫描。

模块非职责：

- 不编辑源码，不提供保存、格式化、诊断、跳转定义或智能提示能力。
- 不替代 API / Design Token 表格。
- 不展示整个源码文件，除非该文件本身就是当前示例的最小片段。
- 不从运行时文件系统读取 `ShowCases/**/*.axaml`。
- 不在 GalleryBase 内写入 `AtomUIGallery.ShowCases.*` 或任何产品命名空间。

## 2. 设计语言

源码展示是 ShowCase 示例的辅助阅读层，不是主要内容层。示例卡片继续优先展示真实控件运行效果；源码入口作为卡片工具动作出现。

交互语言：

- 入口轻量：`ShowCaseItem` 只显示 icon-only 查看源码按钮。
- 阅读聚焦：源码内容在 Drawer 中展示，避免长代码撑开 Masonry 卡片。
- 上下文保留：打开 Drawer 不销毁当前示例、不切换页面、不重建 ShowCase 内容。
- 延迟成本：只有用户点击源码按钮后才解析片段、创建 Drawer 内容和代码编辑器。
- 多文件表达：同一示例可包含多个片段标签，基础生成范围包含 AXAML。

Drawer 而不是卡片内展开，是因为 Gallery 示例代码量差异很大。Drawer 提供稳定宽度、内部滚动、多标签和复制工具条，能承载 DataGrid、Form、TreeView 等复杂示例的代码阅读需求。

## 3. 模块分层

源码展示模块采用五层结构：

| 层级 | 所属项目 | 职责 |
|---|---|---|
| 入口层 | `AtomUI.Toolkits.GalleryBase` | `ShowCaseItem` 展示源码按钮，发出打开源码请求 |
| 宿主层 | `AtomUI.Toolkits.GalleryBase` | `GalleryShowCaseCodeDrawerHost` 承载单例 Drawer，接收打开请求 |
| 查看层 | `AtomUI.Toolkits.GalleryBase` | `GalleryShowCaseCodeDrawer` 和 `GalleryCodeViewer` 展示多标签只读源码 |
| 数据层 | 产品 Gallery 项目 | 实现 `IShowCaseCodeSnippetProvider`，从生成 catalog 查询片段 |
| 生成层 | `AtomUI.Toolkits.GalleryBase.Generator` | 扫描产品 AdditionalFiles，生成 `ShowCaseCodeSnippetCatalog.g.cs` |

依赖方向：

```text
AtomUIGallery
  -> AtomUI.Toolkits.GalleryBase
  -> AtomUI.Toolkits.GalleryBase.Generator (Analyzer)

AtomUI.Toolkits.GalleryBase
  -> AvaloniaEdit / TextMate integration
  -> AtomUI.Desktop.Controls

AtomUI.Toolkits.GalleryBase.Generator
  -> Microsoft.CodeAnalysis
```

GalleryBase 运行时不能依赖 generator 项目。Generator 只在编译期作为 Analyzer 被产品 Gallery 引用。

## 4. 运行时契约模型

源码展示运行时契约由 key、snippet、snippet group 和 provider 组成。

### 4.1 Snippet Key

`ShowCaseCodeSnippetKey` 标识一个 ShowCase 示例：

```csharp
public readonly record struct ShowCaseCodeSnippetKey(
    string ViewTypeName,
    string PanelKey,
    int ItemIndex,
    string? SourceKey = null);
```

字段语义：

| 字段 | 语义 |
|---|---|
| `ViewTypeName` | 当前 ShowCase 页面类型全名，例如 `AtomUIGallery.ShowCases.Button.ButtonShowCase` |
| `PanelKey` | 当前 `ShowCasePanel` 的稳定 key，使用 `Name`，例如 `ExamplesContent` |
| `ItemIndex` | 当前 panel 中 `ShowCaseItem` 的声明顺序索引 |
| `SourceKey` | 可选手工 key，用于复杂页面显式绑定特定源码片段 |

默认 key 使用 `ViewTypeName + PanelKey + ItemIndex`。`SourceKey` 只作为特殊页面的稳定覆盖项，不要求每个 `ShowCaseItem` 手写。参与默认源码匹配的 `ShowCasePanel` 必须有稳定 `Name`；没有 `Name` 的 panel 只能通过 `SourceKey` 显式绑定源码片段。

### 4.2 Snippet Group

一个示例返回一个 `ShowCaseCodeSnippetGroup`：

```csharp
public sealed record ShowCaseCodeSnippetGroup(
    string Title,
    IReadOnlyList<ShowCaseCodeSnippet> Snippets);
```

`Title` 用于 Drawer 标题。默认使用 `ShowCaseItem.Title` 的当前文本值；Provider 返回的标题只作为 fallback。

### 4.3 Snippet

每个标签页对应一个 `ShowCaseCodeSnippet`：

```csharp
public sealed record ShowCaseCodeSnippet(
    string TabTitle,
    string Language,
    string Text,
    string? SourceFilePath,
    int StartLine,
    int EndLine);
```

字段语义：

| 字段 | 语义 |
|---|---|
| `TabTitle` | 标签标题，例如 `AXAML`、`Code-behind`、`ViewModel` |
| `Language` | 语法高亮语言，例如 `axaml`、`xml`、`csharp` |
| `Text` | 展示的源码片段 |
| `SourceFilePath` | repo 相对路径，用于调试和测试 |
| `StartLine` / `EndLine` | 源文件行号范围 |

Generator 的基础生成范围是 `AXAML` 标签。code-behind 和 ViewModel 标签只有在产品侧或 generator 配置提供明确片段提取规则时才生成，不能自动展示整个 `.axaml.cs` 或 ViewModel 文件。

### 4.4 Provider

GalleryBase 定义产品侧 provider 契约：

```csharp
public interface IShowCaseCodeSnippetProvider
{
    bool TryGetSnippetGroup(
        ShowCaseCodeSnippetKey key,
        out ShowCaseCodeSnippetGroup group);
}
```

Provider 注册在 GalleryBase 配置中：

```csharp
public sealed class GallerySourceCodeDisplayOptions
{
    public bool IsEnabled { get; set; }
    public IShowCaseCodeSnippetProvider? SnippetProvider { get; set; }
}
```

该 options 作为 `GalleryBaseOptions` 的子配置存在。未设置 Provider 或 `IsEnabled=false` 时，`ShowCaseItem` 不显示源码按钮。

## 5. 交互与状态模型

源码展示的核心状态属于 Drawer Host，不属于每个 `ShowCaseItem`。

状态：

| 状态 | Owner | 说明 |
|---|---|---|
| `IsCodeActionVisible` | `ShowCaseItem` | 当前 item 是否显示查看源码按钮 |
| `CurrentKey` | Drawer Host | 当前打开的源码 key |
| `IsOpen` | Drawer Host | Drawer 是否打开 |
| `IsLoading` | Drawer | 是否正在解析片段 |
| `SelectedSnippetIndex` | Drawer | 当前标签页 |
| `LoadedEditors` | Drawer | 已创建的 editor 缓存 |

打开流程：

```text
user clicks ShowCaseItem code button
  -> ShowCaseItem creates ShowCaseCodeSnippetKey
  -> request bubbles to nearest GalleryShowCaseCodeDrawerHost
  -> host opens Drawer with title and loading state
  -> host asks provider for ShowCaseCodeSnippetGroup
  -> Drawer renders tabs
  -> first tab creates GalleryCodeViewer
```

关闭流程：

```text
user closes Drawer
  -> Drawer clears current group
  -> disposes created GalleryCodeViewer instances
  -> releases TextMate installations
  -> keeps ShowCaseItem demo content untouched
```

切换标签流程：

```text
user selects tab
  -> Drawer resolves snippet
  -> creates GalleryCodeViewer for that tab if needed
  -> existing editors stay cached while Drawer remains open
```

错误状态：

- Provider 查不到片段：Drawer 显示空状态，并提供当前 key 的调试信息。
- Snippet group 无标签：Drawer 显示空状态。
- 编辑器初始化失败：Drawer 显示纯文本 fallback，不吞掉错误上下文。

## 6. 视觉与主题模型

源码展示使用 AtomUI Drawer 承载，使用 AvaloniaEdit 作为只读代码查看器。Drawer 位于 Gallery Shell 级浮层，不参与 `ShowCasePanel` Masonry 布局。`GalleryShowCaseCodeDrawerHost` 必须包住 Shell 根布局，让 Drawer mask 覆盖导航区和内容区；不要只挂在右侧内容列上。

视觉结构：

```text
GalleryShowCaseCodeDrawerHost
  ContentPresenter(page content)
  Drawer
    Header
      Title
      Copy current tab
      Copy all
      Close
    Body
      TabStrip
      ContentControl
        GalleryCodeViewer
```

主题规则：

- Drawer 背景、边框、标题、工具条间距使用 AtomUI SharedToken 和 GalleryBase Token。
- `GalleryCodeViewer` 使用等宽字体，默认字体族为 `Cascadia Code, Consolas, Menlo, Monospace`。
- 代码编辑器 TextMate theme 必须跟随当前主题模式：
  - light 使用 TextMate `LightPlus`
  - dark 使用 TextMate `DarkPlus`
- 当 `ActualThemeVariant` 是 Avalonia 内置 `ThemeVariant.Light` 或 `ThemeVariant.Dark` 时，优先按该值选择 TextMate theme。
- 当 `ActualThemeVariant` 是 AtomUI 生成的自定义变体，例如 `DaybreakBlue-Dark` 或 `DaybreakBlue-Dark-Compact`，不能用 `ActualThemeVariant == ThemeVariant.Dark` 判断暗色。`GalleryCodeViewer` 必须通过 `Application.IsDarkThemeMode()` 扩展方法读取当前应用暗色模式结果。
- `GalleryCodeViewer` 暴露 `LightSyntaxTheme` 和 `DarkSyntaxTheme`，默认值分别为 `LightPlus` 和 `DarkPlus`。产品 Gallery 可以在不改 GalleryBase 源码的前提下替换为 `VisualStudioLight`、`VisualStudioDark`、`AtomOneDark`、`Dracula` 等 TextMate 内置主题。
- `GalleryCodeViewer` 只安装一次 TextMate。主题变化时调用 `TextMate.Installation.SetTheme(...)` 切换主题，不销毁 `TextEditor`、不重建 document、不中断当前滚动位置。
- `Language` 变化只更新 grammar，`ActualThemeVariant` 或 syntax theme 属性变化只更新 TextMate theme，两条路径不能互相重建。
- 已打开的 Drawer 编辑器必须监听主题变体变化作为同步信号。纯 Avalonia 主题切换可由 `Application.ActualThemeVariantChanged` 覆盖；AtomUI 主题切换必须订阅 `IThemeManager.ThemeVariantProperty`，因为它在激活主题按算法更新后触发。不要直接订阅 `IThemeManager.IsDarkThemeModeProperty`，否则会早于主题激活流程，读到旧的 `ActivatedTheme`。
- 行号默认显示。
- 编辑器只读，允许文本选择和复制。
- 长代码在 Drawer 内部滚动，不让页面滚动条承载代码阅读。

源码展示模块可以新增 GalleryBase 专属 Token，例如：

| Token | 用途 |
|---|---|
| `CodeDrawerWidth` | Drawer 默认宽度 |
| `CodeToolbarPadding` | Drawer 内工具条 padding |
| `CodeViewerMinHeight` | 代码查看器最小高度 |
| `CodeViewerMaxHeight` | 代码查看器最大高度 |
| `CodeViewerFontSize` | 代码字体大小 |

Token 是否独立成类取决于实现时是否已有合适的 GalleryBase Token 承载。新增 Token 必须进入 GalleryBase theme provider 和生成资源。

## 7. 编辑器模型

源码展示选择 AvaloniaEdit 作为代码查看器，并通过 `AvaloniaEdit.TextMate` 接入 TextMate 语法高亮。

`GalleryCodeViewer` 是 GalleryBase 对 AvaloniaEdit 的封装：

```csharp
public class GalleryCodeViewer : TemplatedControl
{
    public string? CodeText { get; set; }
    public string Language { get; set; }
    public bool ShowLineNumbers { get; set; }
}
```

封装职责：

- 创建和配置 `TextEditor`。
- 安装 TextMate registry。
- 根据 `Language` 设置 grammar。
- 同步 AtomUI 主题到 TextMate theme。
- 在 `Application.ActualThemeVariantChanged` 和 `IThemeManager.ThemeVariantProperty` 变化时重新应用 syntax theme；暗色判断读取当前激活主题的算法结果，避免主题状态来源分叉。
- 设置只读、行号、字体、换行和滚动策略。
- dispose 时释放 TextMate installation，取消事件订阅，清空 document 引用。

`ShowCaseItem` 和 Drawer 不直接操作 AvaloniaEdit API，避免第三方编辑器细节扩散到 ShowCase 控件。

## 8. 编译期生成模型

产品 Gallery 使用 `AtomUI.Toolkits.GalleryBase.Generator` 生成源码片段 catalog。Generator 以 Analyzer 方式被产品 Gallery 引用，不作为运行时依赖。

项目归属：

| 项目 | 归属 | 说明 |
|---|---|---|
| `src/AtomUI.Toolkits.GalleryBase` | 运行时 | 定义源码查看 UI、options、key、snippet、provider 契约 |
| `src/AtomUI.Toolkits.GalleryBase.Generator` | 编译期 | 读取产品 Gallery 的 `AdditionalFiles` 并生成 catalog |
| `controlgallery/AtomUIGallery` | 产品侧 | 添加 `AdditionalFiles`、实现 provider、注册 options |
| `tests/AtomUI.Toolkits.GalleryBase.Generator.Tests` | 编译期测试 | 覆盖 AXAML 提取、诊断和生成物稳定性 |

Generator 项目使用 `netstandard2.0`，设置 `IsRoslynComponent=true`，并通过 `TreatAsLocalProperty` 隔离 `PublishAot`、`PublishTrimmed`、`RuntimeIdentifier` 等发布属性。产品项目引用 generator 时使用 Analyzer 引用，不把 generator assembly 带入运行时发布产物。

输入：

```text
controlgallery/AtomUIGallery/ShowCases/**/*.axaml
```

产品项目通过 `AdditionalFiles` 将 ShowCase AXAML 文件传给 generator。

提取规则：

- 读取 `x:Class` 得到 `ViewTypeName`。
- 读取 `gallery:ShowCasePanel`，使用 `Name` 作为 `PanelKey`。
- 按同一 panel 内声明顺序为 `gallery:ShowCaseItem` 分配 `ItemIndex`。
- 对迁移后的 item，提取 `ShowCaseItem.DeferredContentTemplate > DataTemplate` 内部节点。
- 对未迁移 item，提取 `ShowCaseItem` 的直接内容。
- 保留源文件相对路径和行号范围。
- 默认生成 `Language="axaml"`、`TabTitle="AXAML"`。

输出：

```csharp
internal static partial class ShowCaseCodeSnippetCatalog
{
    public static bool TryGetSnippetGroup(
        string viewTypeName,
        string panelKey,
        int itemIndex,
        string? sourceKey,
        out ShowCaseCodeSnippetGroup group)
    {
        // generated switch
    }
}
```

生成代码应使用显式 `switch` 或稳定表，不进行运行时文件系统读取、assembly scan 或 XML 解析。

源码片段数据只放在产品 Gallery 编译产物内的 generated C# catalog 中：

- 不生成运行时 JSON。
- 不生成单独源码资源包。
- 不把源码片段嵌入 GalleryBase runtime assembly。
- 不从安装目录、repo 工作区或 `avares://` 资源读取源码文件。
- `Text` 以 C# 字符串字面量或等价静态数据写入 generated code。
- `SourceFilePath` 使用 repo 相对路径，只用于调试、空状态和测试断言。

生成物命名应位于产品 Gallery namespace 下，例如 `AtomUIGallery.Generated.ShowCaseCodeSnippetCatalog`。GalleryBase 只认识 provider 契约，不认识生成物名称。

Generator 诊断遵循 [AtomUI 编译期诊断规范](../../engineering/compiler-diagnostics-guidelines.md)，诊断 ID 使用 `ATOMUIGEN` 领域前缀并登记到诊断注册表。源码展示 generator 已使用 `ATOMUIGEN101` 覆盖默认源码匹配缺少 panel key 的场景，其他诊断按同一领域前缀扩展。

| 场景 | Severity | 行为 |
|---|---|---|
| AXAML 无法解析 | Warning | 跳过当前文件，提示文件路径和解析错误摘要 |
| 根节点缺少 `x:Class` | Warning | 跳过当前文件，提示补齐页面类型 |
| `ShowCasePanel` 缺少 `Name` 且没有显式 `SourceKey` | Warning | 跳过该 panel 的默认 key 生成 |
| `ShowCaseItem` 无法定位可展示内容 | Warning | 为该 item 不生成 snippet，并保留其他 item |
| 同一 key 产生重复 snippet group | Warning | 后出现的 key 不覆盖先出现的 key，提示重复来源 |

## 9. 产品集成模型

具体产品 Gallery 负责接入生成 catalog：

```csharp
internal sealed class AtomUIGalleryShowCaseCodeSnippetProvider
    : IShowCaseCodeSnippetProvider
{
    public bool TryGetSnippetGroup(
        ShowCaseCodeSnippetKey key,
        out ShowCaseCodeSnippetGroup group)
    {
        return ShowCaseCodeSnippetCatalog.TryGetSnippetGroup(
            key.ViewTypeName,
            key.PanelKey,
            key.ItemIndex,
            key.SourceKey,
            out group);
    }
}
```

产品模块在 `UseGalleryBase(...)` 配置阶段注册：

```csharp
options.SourceCodeDisplay.IsEnabled = true;
options.SourceCodeDisplay.SnippetProvider =
    new AtomUIGalleryShowCaseCodeSnippetProvider();
```

GalleryBase 不直接寻找 `ShowCaseCodeSnippetCatalog`，也不约定产品 catalog 的 namespace。产品侧 provider 是唯一桥接点。

## 10. 生命周期与资源边界

源码展示模块必须保持 ShowCase 页面延迟创建语义：

- `ShowCaseItem` 显示源码按钮不能触发 `MaterializeDeferredContent()`。
- Drawer 打开不能创建未 materialize 的示例控件树。
- Snippet 查询只读取生成 catalog，不访问当前 demo visual tree。
- Drawer 关闭时释放 editor、TextMate installation、事件订阅和 clipboard 状态。
- Drawer Host detach 时关闭 Drawer 并释放当前 group。
- Provider 不持有 ShowCase 页面或 `ShowCaseItem` 实例引用。

Clipboard 操作通过当前 `TopLevel` 获取剪贴板服务。没有可用 clipboard 时，复制按钮进入禁用或错误提示状态，不抛出未处理异常。

## 11. AOT、裁剪与 Browser 边界

源码展示模块必须满足 Gallery AOT 和 Browser 发布要求：

- 运行时不使用 `Assembly.GetTypes()`、`Activator.CreateInstance(...)`、反射字段扫描或运行时文件扫描。
- Source Generator 输出的 catalog 是普通 C# 代码，trimmer 可静态分析。
- Generator 项目使用 `netstandard2.0`，并隔离 `PublishAot`、`PublishTrimmed`、`RuntimeIdentifier` 等发布属性。
- TextMate grammar 资源必须通过包资源路径加载，并在 Browser 发布中验证。
- Drawer 和 editor 均按用户操作延迟创建，Browser 首屏不初始化 AvaloniaEdit。
- 生成 catalog 不应无限增长到包含整页或整文件源码；只保存与示例直接相关的片段。

涉及 NativeAOT 发布时，除普通测试外必须执行 Gallery NativeAOT publish 验证。

## 12. 兼容性不变量

实现源码展示模块时必须保持以下不变量：

- 现有 ShowCase 示例视觉树不因源码功能变化而改变。
- 现有 `ShowCasePanel + ShowCaseItem` 双层延迟创建规则不变。
- `ShowCaseItem` 标题、描述、Badge、占整行和 deferred placeholder 语义不变。
- 未配置 Provider 的产品 Gallery 不显示源码按钮，也不产生运行时错误。
- Provider 查不到片段时不会阻断 ShowCase 页面展示。
- Drawer 始终作为浮层显示，不参与 Masonry 布局测量。
- GalleryBase 不引用产品项目。
- Source Generator 不生成产品运行时依赖之外的反射路径。
- Snapshot 测试中的 demo 内容提取规则仍以 demo XAML 为准，不把源码展示 UI 混入示例内容 snapshot。

## 13. 文档导航与验证策略

相关文档：

- [AtomUI.Toolkits.GalleryBase 模块概览](overview.md)
- [AtomUI.Toolkits.GalleryBase 设计文档](architecture.md)
- [GalleryBase ShowCase 控件设计](showcase-controls.md)
- [Gallery ShowCase Design Pattern](../../gallery/gallery-showcase-design-pattern.md)
- [AtomUI AOT 编程规范](../../engineering/aot-programming-guidelines.md)
- [AtomUI 编译期诊断规范](../../engineering/compiler-diagnostics-guidelines.md)

验证要求：

| 层级 | 验证 |
|---|---|
| Generator | generator tests 覆盖 AXAML 提取、panel key、item index、line range 和空输入 |
| GalleryBase runtime | Drawer 打开/关闭、editor lazy 创建、provider 缺失、clipboard fallback |
| ShowCase 结构 | 全量迁移页面每个 `ShowCaseItem` 可匹配源码片段 |
| AOT | Gallery NativeAOT publish 验证生成 catalog 和编辑器依赖 |
| 文档 | `git diff --check`，并确认本文件从 GalleryBase 模块入口可达 |
