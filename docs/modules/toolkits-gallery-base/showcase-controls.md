# GalleryBase ShowCase 控件设计

本文档细化 GalleryBase 中展示控件的迁移和稳定边界。ShowCase 控件是 Gallery 页面最核心的复用资产，负责让示例内容以文档化、可滚动、可延迟创建的方式呈现。

## 设计目标

- 复用当前 Gallery 中已经验证过的 ShowCase 页面范式。
- 保留控件名称和 XAML 使用方式，降低现有 `AtomUIGallery` 迁移成本。
- 把控件与 AtomUI 产品内容分离，使其他产品可以使用同一套展示控件。
- 保持 Browser 端渐进挂载和 ShowCaseItem 延迟创建能力。
- 为复杂 Demo 页面提供统一场景切换和 lazy tab 缓存。

## 控件清单

第一阶段迁入 GalleryBase：

| 类型 | 职责 |
|---|---|
| `ShowCaseItem` | 示例卡片，包含标题、描述、内容和延迟内容模板 |
| `ShowCasePanel` | 示例集合宿主，支持瀑布流、内部滚动开关、viewport 延迟创建 |
| `ShowCaseMasonryPanel` | 瀑布流布局面板 |
| `GalleryStickyTabsHost` | 文档式页面宿主，Header、StickyContent、Content 同一滚动上下文 |
| `GalleryStickyTabsPanel` | StickyContent 吸顶布局实现 |
| `GalleryShowCaseScenarioController` | Examples/API/Design Token 等场景 lazy 创建和缓存 |
| `GalleryReactiveUserControl<TViewModel>` | ReactiveUI View 激活辅助基类 |
| `GalleryShowCaseRuntimeOptions` | 诊断开关和延迟创建运行时控制 |

## 命名策略

第一阶段不把 `ShowCase` 改成 `DemoCase` 或其他更中立名称。

原因：

- 当前所有 ShowCase 页面已经大量使用 `ShowCasePanel` 和 `ShowCaseItem`。
- 名称表达的是 Gallery 页面中的展示案例，不是 AtomUI 产品专有概念。
- 同时迁移程序集和重命名控件会造成快照、XAML、文档和测试大范围 churn。

如果后续需要中立别名，可以新增类型别名或派生包装，但不在第一阶段做破坏性重命名。

## ShowCaseItem

核心属性：

```csharp
public class ShowCaseItem : ContentControl
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsOccupyEntireRow { get; set; }
    public ShowCaseItemSpan Span { get; set; }
    public bool IsDeferredContentEnabled { get; set; }
    public IDataTemplate? DeferredContentTemplate { get; set; }
    public object? DeferredContent { get; set; }
    public double DeferredPlaceholderHeight { get; set; }
    public bool IsDeferredContentMaterialized { get; }
}
```

延迟创建规则：

- `IsDeferredContentEnabled=false` 时保持普通 `ContentControl` 行为。
- `IsDeferredContentEnabled=true` 且有 `DeferredContentTemplate` 时，内容只在 `MaterializeDeferredContent()` 后创建。
- 已创建内容不回收，避免复杂控件状态、焦点、Popup/Flyout 生命周期被破坏。
- `DeferredContent` 为空时使用 `DataContext` 构建模板。
- DataContext 变化后，已创建内容需要同步 DataContext。

XAML 使用规范：

```xml
<gallery:ShowCaseItem Title="Basic"
                      Description="Basic usage"
                      IsDeferredContentEnabled="True">
    <gallery:ShowCaseItem.DeferredContentTemplate>
        <DataTemplate x:DataType="vm:CurrentShowCaseViewModel">
            <!-- demo content -->
        </DataTemplate>
    </gallery:ShowCaseItem.DeferredContentTemplate>
</gallery:ShowCaseItem>
```

迁移后的新 ShowCase 不允许把复杂 demo 内容直接写在 `ShowCaseItem.Content`。

## ShowCasePanel

核心属性：

```csharp
public class ShowCasePanel : TemplatedControl
{
    public double MinItemWidth { get; set; }
    public int MaxColumns { get; set; }
    public double ColumnGap { get; set; }
    public double RowGap { get; set; }
    public Thickness ContentMargin { get; set; }
    public bool IsScrollEnabled { get; set; }
    public bool IsDeferredLoadingEnabled { get; set; }
    public int InitialDeferredLoadItemCount { get; set; }
    public int DeferredLoadBatchSize { get; set; }
    public double DeferredLoadViewportBuffer { get; set; }
    public Controls Children { get; }
}
```

职责：

- 把 `ShowCaseItem` 挂载到 `ShowCaseMasonryPanel`。
- 控制是否使用内部 ScrollViewer。
- 在 Browser 端渐进挂载首批 item，降低初始渲染压力。
- 使用一个 panel 级 `EffectiveViewportChanged` 监听 materialize 近 viewport item。

滚动规则：

| 场景 | `IsScrollEnabled` |
|---|---|
| 老式独立示例面板 | `true` |
| `GalleryStickyTabsHost` 内 Examples | `false` |
| 特殊全视口页面 | 由页面自己决定，但必须避免双垂直滚动条 |

## ShowCaseMasonryPanel

布局规则：

- 根据可用宽度、`MinItemWidth`、`MaxColumns` 计算列数。
- 普通 item 放入当前最短列。
- `ShowCaseItemSpan.Full` 或 `IsOccupyEntireRow=true` 占满整行。
- 不做虚拟化，只负责已挂载 children 的测量和排列。

该面板保持简单，不引入 ItemsControl、ItemsSource 或数据模板。ShowCase 页面是文档式页面，不是无限列表。

## GalleryStickyTabsHost

结构：

```text
GalleryStickyTabsHost
  Header
  StickyContent
  Content
```

职责：

- 提供页面级 ScrollViewer。
- 让 Header、StickyContent、Content 共用一个滚动上下文。
- StickyContent 到达顶部后保持可见。
- 通过只读 overlay mirror 解决窗口级 Adorner 覆盖 sticky tabs 的问题。

规则：

- 真实 StickyContent 不移出原视觉树。
- overlay mirror 只绘制视觉镜像，`IsHitTestVisible=false`。
- 不在滚动时销毁或重建 TabStrip。
- detach 时释放 ScrollViewer、StickyPanel、LayoutUpdated 和 overlay 资源。

## GalleryShowCaseScenarioController

典型场景：

- Examples
- API
- Design Token

职责：

- 监听 `TabStrip.SelectionChanged`。
- Examples 内容可预先存在。
- API/Design Token 等内容首次切换时创建。
- 创建后缓存，避免重复构建 DataGrid。
- DataContext 变化时同步到已创建内容。
- detach 时清理 lazy content cache。

使用方式：

```csharp
_scenarioController = new GalleryShowCaseScenarioController(
    ScenarioTabs,
    ScenarioContentHost,
    CreateScenarioContent,
    ExamplesContent);
```

`CreateScenarioContent` 仍由产品 ShowCase 页面提供，因为 API 和 Design Token 控件属于产品内容。

## GalleryReactiveUserControl

职责：

- 提供 `ViewModel` StyledProperty。
- 同步 `DataContext` 与 `ViewModel`。
- 提供 `WhenActivated` 生命周期。
- 在 view loaded/unloaded 时激活或停用 `IActivatableViewModel`。

该类型可以继续作为产品 ShowCase 页面基类，减少产品侧重复 ReactiveUI 生命周期代码。

## Runtime Options

`GalleryShowCaseRuntimeOptions` 提供：

- 环境变量关闭延迟创建。
- F5/F6 自动切页压测时运行时关闭或恢复延迟创建。
- 事件通知已存在 panel 更新 materialization 状态。

环境变量沿用：

```text
ATOMUI_GALLERY_DISABLE_SHOWCASE_DEFERRED
```

该名称可在迁移后保留，避免破坏现有性能和诊断脚本。后续可增加新名称并兼容旧名称。

## 主题资源

ShowCase 控件主题迁入 GalleryBase：

- `ShowCaseItemTheme.axaml`
- `ShowCasePanelTheme.axaml`
- `GalleryStickyTabsHostTheme.axaml`

主题必须使用 GalleryBase 的 XAML namespace：

```xml
xmlns:gallery="https://atomui.net/toolkits/gallery-base"
```

## 测试要求

- `ShowCaseItem.DeferredContentTemplate` 在 materialize 前不创建内容。
- materialize 后不重复创建内容。
- DataContext 变化同步到已 materialize 内容。
- `ShowCasePanel` 只使用一个 panel 级 viewport 监听。
- Browser 渐进挂载不影响 deferred content 策略。
- Masonry full span item 占满整行。
- Sticky host detach 后释放 overlay mirror。
- Scenario controller 首次切换创建 lazy content，后续切换复用缓存。
