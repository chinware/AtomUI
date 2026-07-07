# Gallery ShowCase Design Pattern

本文档约定 AtomUI Gallery 控件 ShowCase 页面的稳定结构。标准控件页面面向示例浏览，直接展示 `ExamplesContent`；API 和 Design Token 继续以旁路 DataGrid 文件保留，作为文档、生成器和元数据资产，不再作为页面一级 Tab 展示。

## 页面模型

标准控件 ShowCase 页面采用下面的结构：

```text
ShowCase Page
  GalleryStickyTabsHost
    Header
      GalleryShowCaseHeader
    Content
      ExamplesContent
        ShowCasePanel
          ShowCaseItem
            DeferredContentTemplate
```

推荐 AXAML 结构：

```xml
<gallery:GalleryStickyTabsHost>
    <gallery:GalleryStickyTabsHost.Header>
        <gallery:GalleryShowCaseHeader
            Title="Button"
            Category="{gallery:ButtonShowCaseLangResource ComponentCategory}"
            Status="{gallery:ButtonShowCaseLangResource ComponentStatusStable}"
            Subtitle="{gallery:ButtonShowCaseLangResource PageSubtitle}"
            Description="{gallery:ButtonShowCaseLangResource PageDescription}"
            Namespace="AtomUI.Controls"
            Package="AtomUI.Desktop.Controls"
            BaseClass="AtomUIButton" />
    </gallery:GalleryStickyTabsHost.Header>

    <gallery:ShowCasePanel Name="ExamplesContent"
                           IsScrollEnabled="False"
                           IsDeferredLoadingEnabled="True"
                           InitialDeferredLoadItemCount="4"
                           DeferredLoadBatchSize="2"
                           ContentMargin="28,10,28,28">
        <gallery:ShowCaseItem Title="..."
                              Description="..."
                              IsDeferredContentEnabled="True">
            <gallery:ShowCaseItem.DeferredContentTemplate>
                <DataTemplate x:DataType="vm:CurrentShowCaseViewModel">
                    <!-- demo content -->
                </DataTemplate>
            </gallery:ShowCaseItem.DeferredContentTemplate>
        </gallery:ShowCaseItem>
    </gallery:ShowCasePanel>
</gallery:GalleryStickyTabsHost>
```

标准页面不得声明：

- `GalleryStickyTabsHost.StickyContent`
- `ScenarioTabs`
- `ScenarioContentHost`
- `Tag="Api"` 或 `Tag="DesignToken"`
- `GalleryShowCaseScenarioController`
- `CreateScenarioContent`
- `new XxxApiDataGrid()` 或 `new XxxDesignTokenDataGrid()`

`IconShowCase`、`PaletteShowCase` 这类示例本身需要分组的特殊页面可以继续使用 `TabStrip + ContentControl`。这些 Tab 表示示例分组，例如 `Outlined/Filled/TwoTone` 或 `Light/Dark`，不是标准页面的 API/Design Token 导航。

## Header 范式

标准页面必须在 `GalleryStickyTabsHost.Header` 中使用 `GalleryShowCaseHeader`，不再为每个页面手写 `StackPanel + Tag + metadata` 页头结构。

推荐内容：

- 控件名。
- 控件类别，例如 `General`、`Data Entry`、`Data Display`。
- 稳定性或状态，例如 `Stable`、`Preview`。
- 一句话 subtitle。
- 一段 description。
- `Namespace`、`Package`、`BaseClass` 等紧凑 metadata。

Header 不应放大量营销文案、复杂交互示例、可替代 Examples 的演示内容，或会让首屏过高的长篇说明。

## Examples 范式

Examples 使用 `ShowCasePanel + ShowCaseItem`。标准页面中 `ShowCasePanel` 关闭内部滚动，把滚动权交给 `GalleryStickyTabsHost`：

```xml
<gallery:ShowCasePanel IsScrollEnabled="False"
                       IsDeferredLoadingEnabled="True"
                       InitialDeferredLoadItemCount="4"
                       DeferredLoadBatchSize="2">
```

延迟加载是 ShowCase 创建规范的一部分，不是可选优化：

- `IsDeferredLoadingEnabled` 必须为 `True`。
- `InitialDeferredLoadItemCount` 默认使用 `4`。
- `DeferredLoadBatchSize` 默认使用 `2`。
- 每个有演示内容的 `ShowCaseItem` 必须设置 `IsDeferredContentEnabled="True"`。
- 演示控件必须放入 `ShowCaseItem.DeferredContentTemplate`。
- `DeferredContentTemplate` 必须声明 `x:DataType`，通常使用当前 ShowCase 的 ViewModel 类型。
- 已 materialize 的内容不回收，避免破坏状态、焦点、Popup/Flyout 生命周期。

硬边界：

- 移入 `DeferredContentTemplate` 只能改变创建时机，不能改变演示控件树。
- 不允许把演示控件直接写成 `ShowCaseItem.Content`。
- 改造页面结构时不顺手调整示例控件数量、层级、默认值、文案或视觉布局。
- 对已整理过的页面保留 snapshot 测试，确保示例内容不会被布局改造误改。

## API 与 Design Token

API 和 Design Token 继续使用独立 UserControl 承载 DataGrid，但它们不再由主 ShowCase 页面实例化：

```text
ButtonShowCase.axaml
ButtonShowCase.axaml.cs
ButtonApiDataGrid.axaml
ButtonApiDataGrid.axaml.cs
ButtonDesignTokenDataGrid.axaml
ButtonDesignTokenDataGrid.axaml.cs
```

DataGrid sidecar 规则：

- 文件继续跟随对应控件 ShowCase 放在同一个 `Views` 目录。
- DataGrid 自己绑定 `ApiRows` 或 `DesignTokenRows`。
- DataGrid 仍使用真实 `atom:DataGrid`，不在主页面手写普通布局模拟表格。
- 第一列用于 key/name，应固定。
- 描述列使用 `Width="*"` 承接剩余宽度，只设置合理 `MinWidth`。
- `Margin="28,10,28,28"` 保持与 Examples 左右和底部节奏一致。

主 ShowCase 页面不得把 API/Token DataGrid 直接写进 `ButtonShowCase.axaml`，也不得在 code-behind 中 `new ButtonApiDataGrid()` 或 `new ButtonDesignTokenDataGrid()`。

## Sticky Host

`GalleryStickyTabsHost` 是 Gallery 专用文档页宿主，负责页面级滚动、Header 和主体内容布局。标准页面没有 `StickyContent` 时，模板中的 sticky 行必须折叠，不保留空白导航条。

特殊页面使用 `StickyContent` 时仍遵循原 sticky 规则：

- 真实 `StickyContent` 始终保留在模板里的 inline presenter。
- 不把同一个 TabStrip 移动到 Popup、OverlayLayer 或其他 presenter。
- sticky mirror 只作为只读视觉镜像，`IsHitTestVisible=false`。
- 没有 `StickyContent` 时不创建 sticky mirror。
- `ScrollChanged` 订阅必须在 detach 时释放。

## Spacing

标准页面默认节奏：

- Header 左右：由 `GalleryShowCaseHeader` 和宿主样式控制。
- Examples：`ContentMargin="28,10,28,28"`。
- API/Token sidecar DataGrid：`Margin="28,10,28,28"`。

标准页面不再需要让 Header、Tab、Examples、API、Token 五者对齐；只需要 Header 与 Examples 的阅读节奏稳定。

## Code-Behind

标准页面 code-behind 只保留当前控件自己的运行时逻辑，例如：

- 示例事件 handler。
- ViewModel 初始化或语言切换数据刷新。
- 示例控件 materialize 后的数据源补接。
- detach 时清理示例数据源或订阅。

标准页面不得保留标准 API/Token 场景切换逻辑：

- 不声明 `ApiScenario` / `DesignTokenScenario`。
- 不持有 `GalleryShowCaseScenarioController`。
- 不在 attach/detach/DataContextChanged 中调用 `_scenarioController`。
- 不声明 `CreateScenarioContent`。

`IconShowCase`、`PaletteShowCase` 等特殊分组页面可以继续使用 `GalleryShowCaseScenarioController` 管理自己的示例分组 lazy content。

## 测试范式

每个标准 ShowCase 至少覆盖三类测试。

结构测试：

- 页面使用 `GalleryStickyTabsHost`。
- Header 使用 `GalleryShowCaseHeader`。
- 主页面不存在 `ScenarioTabs`、`ScenarioContentHost`、`Tag="Api"`、`Tag="DesignToken"`。
- 存在直接声明的 `ExamplesContent`。
- `ExamplesContent` 设置 `IsScrollEnabled="False"`。
- `ExamplesContent` 设置 `IsDeferredLoadingEnabled="True"`。
- 每个有演示内容的 `ShowCaseItem` 设置 `IsDeferredContentEnabled="True"`。
- `ShowCaseItem` 数量与 `DeferredContentTemplate` 数量一致。

API/Token sidecar 测试：

- `XxxApiDataGrid.axaml` 和 `XxxDesignTokenDataGrid.axaml` 仍存在。
- sidecar DataGrid 自己绑定 `ApiRows` / `DesignTokenRows`。
- 主页面 XAML 不绑定 `ApiRows` / `DesignTokenRows`。
- code-behind 不创建 API/Token DataGrid。

演示内容保护测试：

- 从第一个 `ShowCaseItem` 到 `ShowCasePanel` 结束提取示例内容。
- 与 approved snapshot 比较。
- snapshot normalize 可以剥离 deferred wrapper，但不得掩盖真实演示控件变化。

## 迁移检查清单

迁移或新增标准 ShowCase 时按以下顺序处理：

1. 保留原 `ShowCaseItem` 内容，先建立或确认 snapshot。
2. 使用 `GalleryShowCaseHeader`。
3. 用 `GalleryStickyTabsHost` 包裹页面主体。
4. 删除标准 `Examples/API/Design Token` Tab。
5. 删除 `ScenarioContentHost`，让 `ExamplesContent` 成为直接主体。
6. 保持 `ShowCasePanel IsScrollEnabled=False`。
7. 开启 panel 级延迟加载：`IsDeferredLoadingEnabled=True`、`InitialDeferredLoadItemCount=4`、`DeferredLoadBatchSize=2`。
8. 每个有演示内容的 `ShowCaseItem` 使用 `IsDeferredContentEnabled=True` 和 `DeferredContentTemplate`。
9. 保留 API/Design Token sidecar DataGrid 文件。
10. 从 code-behind 删除标准场景 controller 和 `CreateScenarioContent`。
11. 跑结构测试、deferred 创建测试、snapshot 测试和 Gallery 测试。

## 不做事项

- 不删除 API/Token DataGrid sidecar 文件。
- 不把 API/Token 表格改写进主 ShowCase。
- 不把演示控件直接写在 `ShowCaseItem` 内容区。
- 不为标准页面保留 `Examples/API/Design Token` 一级 Tab。
- 不在每个页面私有一套标准场景 lazy cache。
- 不让 Header 和 Examples 拥有不同滚动上下文。
- 不为 sticky tabs 复制一份可交互 TabStrip。

## 推广结果

标准控件 ShowCase 已按“Header + direct ExamplesContent”范式迁移。API/Design Token sidecar 文件继续保留为 Gallery 文档数据源；`GalleryShowCaseScenarioController` 只服务 Icon、Palette 这类真实需要示例分组 lazy content 的特殊页面。
