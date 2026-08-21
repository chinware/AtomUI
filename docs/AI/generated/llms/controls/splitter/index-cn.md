# Splitter

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Splitter 是 AtomUI 桌面控件体系中的分割面板控件，用于把一个区域拆成两个或多个可调整尺寸的面板。它负责面板尺寸计算、拖拽调整、延迟拖拽反馈、面板折叠状态和分割把手视觉。

Splitter 不负责窗口停靠系统、路由容器、业务布局状态持久化、面板内容装饰或复杂 IDE docking。子面板的内容、背景、内部圆角和业务状态应由用户提供的子控件负责。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Splitter/Splitter.cs`
- `src/AtomUI.Desktop.Controls/Splitter/SplitterPanel.cs`
- `src/AtomUI.Desktop.Controls/Splitter/SplitterHandle.cs`
- `src/AtomUI.Desktop.Controls/Splitter/SplitterDragBar.cs`
- `src/AtomUI.Desktop.Controls/Splitter/Themes/`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Layout/Splitter` |
| 状态 | Stable |

## 何时使用

Splitter 的设计语言是“低干扰的可调整边界”。分割线应明确表示可拖拽边界，但不能抢夺面板内容的视觉层级。折叠图标只在可折叠边界上表达动作，不应让普通分割线看起来像常驻工具栏。

| 维度 | 含义 | Splitter 中的表达 |
| --- | --- | --- |
| 产品语义 | 将一个区域拆成若干可调整子区域。 | `Children` 承载面板，`Orientation` 决定分割方向。 |
| 内容承载 | 面板内容由用户直接提供。 | 子控件通过 `Splitter.Size`、`Splitter.MinSize` 等附加属性参与布局。 |
| 状态反馈 | 拖拽、可折叠、禁用拖拽和延迟反馈需要可见状态。 | `ResizeStarted` / `ResizeDelta` / `ResizeCompleted` 事件、handle hover/dragging 视觉和折叠按钮。 |
| 主题语义 | 外层容器与分割把手分开定制。 | `SplitterTheme` 承载根框架，`SplitterHandleTheme` 和 `SplitterDragBarTheme` 承载分割线和拖拽命中区。 |

## 公共 API

Splitter 的公共契约由根控件 API、面板附加属性、折叠模型、事件和主题入口共同组成。`SplitterPanel`、`SplitterHandle` 和 `SplitterDragBar` 是 internal 协作对象，不属于用户可直接依赖的 public surface。

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 根布局 | `Orientation`、`Children` | 定义面板排列方向和面板集合。 |
| 拖拽反馈 | `IsLazy`、`HandleSize` | 控制拖拽反馈时机和把手命中区域尺寸。 |
| 折叠入口 | `CollapsePreviousIcon`、`CollapseNextIcon`、`Splitter.Collapsible`、`Splitter.IsCollapsed` | 定义面板是否可折叠、折叠状态以及折叠图标。 |
| 面板尺寸 | `Splitter.Size`、`Splitter.DefaultSize`、`Splitter.MinSize`、`Splitter.MaxSize` | 定义子面板初始尺寸、默认尺寸和尺寸约束。 |
| 面板交互 | `Splitter.IsResizable` | 控制相邻边界是否允许拖拽调整。 |
| 事件 | `ResizeStarted`、`ResizeDelta`、`ResizeCompleted` | 把 internal drag 流转换为控件级 resize 事件。 |
| 主题入口 | `HandleSize`、控件 Token、根模板外观入口 | 区分命中区域、可见分割线和根框架外观。 |

样式能力边界：

- `HandleSize` 只表示分割把手的命中区域，不等同于可见分割线厚度。
- 可见分割线的厚度、圆角和 hover/dragging 颜色属于 handle 视觉模型，应通过根控件实例属性和 Token 映射到 internal handle，而不是要求用户样式化 internal 类型。
- Splitter 根框架可承接 `Background`、`BorderBrush`、`BorderThickness`、`CornerRadius`、`BorderDashArray`、`BorderDashOffset` 等基础外观属性；子面板圆角仍由用户提供的面板控件自行控制。
- 不新增 `PanelCornerRadius`、`PanelBackground` 这类统一改写子面板的属性，避免 Splitter 篡改用户内容树。

Semantic Part 契约：`Splitter` 公开 `root`、`panel`、`dragger` 三个 Semantic Part。`panel` 的 marker 由
`SplitterPanel` 运行时添加到用户面板，`dragger` 命中每个 handle 模板内的 `PART_DragBar`；二者均为
`RuntimeCreated` 的 Selector 型 Part，完整字段、路由与定制边界见 [Splitter Semantic Part 契约](semantic-part.md)。

稳定 template part：

| Template Part | 类型 | 稳定性 | 职责 |
| --- | --- | --- | --- |
| `PART_SplitterPanel` | `SplitterPanel` | public control template part | 承载子面板并生成 internal handle。 |
| `PART_HandleLine` | `Border` | internal handle template part | 展示可见分割线。 |
| `PART_DragBar` | `SplitterDragBar` | internal handle template part | 提供拖拽命中区和拖拽事件源。 |
| `PART_Grip` | `Border` | internal drag-bar template part | 展示拖拽区域内的 grip。 |
| `PART_CollapsePrevButton` | `IconButton` | internal handle template part | 触发前侧面板折叠或展开。 |
| `PART_CollapseNextButton` | `IconButton` | internal handle template part | 触发后侧面板折叠或展开。 |

`PART_HandleLine`、`PART_DragBar`、`PART_Grip` 和折叠按钮虽然是稳定 internal theme part，但不应作为用户自定义入口暴露。用户级定制应通过 `Splitter` public API、控件 Token、ControlTheme 和 Gallery 展示的稳定用法进入。

伪类模型：

- `Splitter` 根控件当前不定义专属伪类。
- `SplitterHandle` 使用 `:pointerover` 和 `:dragging` 驱动分割线颜色。
- `SplitterDragBar` 使用 `:dragging` 表达拖拽状态。
- `IconButton` 折叠按钮继续使用标准 pointer/pressed/disabled 视觉。

## 事件与命令

Splitter 的公共契约由根控件 API、面板附加属性、折叠模型、事件和主题入口共同组成。`SplitterPanel`、`SplitterHandle` 和 `SplitterDragBar` 是 internal 协作对象，不属于用户可直接依赖的 public surface。
| 事件 | `ResizeStarted`、`ResizeDelta`、`ResizeCompleted` | 把 internal drag 流转换为控件级 resize 事件。 |
| `PART_DragBar` | `SplitterDragBar` | internal handle template part | 提供拖拽命中区和拖拽事件源。 |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Views/SplitterShowCase.axaml:97`

Gallery key：`ExamplesContent` / item `0`

```axaml
<Border Classes="splitter-surface">
    <atom:Splitter Orientation="Vertical" Height="220">
        <Border Classes="splitter-panel" atom:Splitter.Size="30%">
            <TextBlock Classes="splitter-label" Text="第一项" />
        </Border>
        <Border Classes="splitter-panel" atom:Splitter.DefaultSize="100" atom:Splitter.MinSize="60">
            <TextBlock Classes="splitter-label" Text="第二项" />
        </Border>
    </atom:Splitter>
</Border>
```

### 水平分割

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Views/SplitterShowCase.axaml:115`

Gallery key：`ExamplesContent` / item `1`

```axaml
<Border Classes="splitter-surface">
    <atom:Splitter Orientation="Horizontal" Height="220">
        <Border Classes="splitter-panel" atom:Splitter.Size="40%">
            <TextBlock Classes="splitter-label" Text="顶部" />
        </Border>
        <Border Classes="splitter-panel alt">
            <TextBlock Classes="splitter-label" Text="底部" />
        </Border>
    </atom:Splitter>
</Border>
```

### 组合布局

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Views/SplitterShowCase.axaml:133`

Gallery key：`ExamplesContent` / item `2`

```axaml
<Border Classes="splitter-surface">
    <atom:Splitter Orientation="Vertical" Height="260">
        <Border Classes="splitter-panel" atom:Splitter.Size="40%">
            <TextBlock Classes="splitter-label" Text="左侧" />
        </Border>
        <atom:Splitter Orientation="Horizontal">
            <Border Classes="splitter-panel">
                <TextBlock Classes="splitter-label" Text="顶部" />
            </Border>
            <Border Classes="splitter-panel alt">
                <TextBlock Classes="splitter-label" Text="底部" />
            </Border>
        </atom:Splitter>
    </atom:Splitter>
</Border>
```

### 禁用拖拽调整

来源：`controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Views/SplitterShowCase.axaml:156`

Gallery key：`ExamplesContent` / item `3`

```axaml
<Border Classes="splitter-surface">
    <atom:Splitter Orientation="Vertical" Height="220">
        <Border Classes="splitter-panel" atom:Splitter.Size="35%">
            <TextBlock Classes="splitter-label" Text="Resizable" />
        </Border>
        <Border Classes="splitter-panel alt" atom:Splitter.DefaultSize="120" atom:Splitter.IsResizable="False">
            <TextBlock Classes="splitter-label" Text="Not Resizable" />
        </Border>
        <Border Classes="splitter-panel" atom:Splitter.DefaultSize="120">
            <TextBlock Classes="splitter-label" Text="Resizable" />
        </Border>
    </atom:Splitter>
</Border>
```

## 状态模型

Splitter 的状态流按以下路径收敛：

```text
Public API / attached panel properties / pointer drag / collapse button
  -> SplitterPanel layout state
  -> SplitterHandle drag and collapse state
  -> SplitterDragBar pointer state
  -> ControlTheme selector / template binding / resize events
  -> Gallery 可观察行为
```

行为规则：

- `Orientation=Vertical` 表示左右分割，handle 横向拖动；`Orientation=Horizontal` 表示上下分割，handle 纵向拖动。
- `HandleSize` 控制 handle 在布局中占用的命中区域，不能被可见线条厚度替代。
- `IsLazy=False` 时拖拽过程中实时调整面板尺寸；`IsLazy=True` 时拖拽过程中移动 drag bar，完成后提交尺寸。
- `Splitter.Size` 是用户可双向绑定的实际尺寸入口；`DefaultSize` 是未提供实际尺寸时的初始化入口。
- `MinSize`、`MaxSize`、`IsResizable` 和折叠状态共同决定某个 handle 是否可拖拽。
- 折叠按钮只在相邻面板支持折叠或存在可恢复折叠面板时显示。
- resize 事件以 handle index 和当前尺寸快照暴露，不让用户直接依赖 internal handle 实例。

## 主题与 Design Token

Splitter 的视觉模型由根控件模板、internal 面板和 handle 模板、SharedToken、控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `SplitterTheme.axaml` | 定义 `Splitter` 根模板、外层 Frame 和 `PART_SplitterPanel`。 |
| `SplitterHandleTheme.axaml` | 定义可见分割线、拖拽命中区和折叠按钮的组合结构。 |
| `SplitterDragBarTheme.axaml` | 定义拖拽命中区、grip 尺寸、grip 圆角和方向 cursor。 |

视觉语义拆分：

- 根框架：承载 Splitter 整体背景、边框、圆角和裁剪。
- 命中区域：由 `HandleSize` 和 `SplitBarHandleSize` 定义，保证拖拽易用性。
- 可见分割线：由 line thickness、line corner radius 和 line color 定义，不能反向改变命中区域。
- grip：由 drag bar 模板展示，使用较短尺寸提示可拖拽。
- 折叠按钮：依赖 `SplitterPanelCollapsible` 和 hover/press 状态显示，不参与面板尺寸计算。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、dragging、collapsed 等运行时状态写入 Token。
- 调整线条厚度时必须同时检查 `PART_HandleLine` 和 `PART_Grip`，避免可见线条与拖拽提示尺寸不一致。

Token 来源：

Splitter Token 只表达组件级视觉变量，包括分割线尺寸、拖拽命中区、折叠按钮定位、handle 颜色和图标尺寸。Token 不承载运行时拖拽状态、折叠状态、面板尺寸、业务布局数据或用户内容背景。

当前 Token scope：

- `SplitterToken`，scope id 为 `Splitter`，源码位于 `src/AtomUI.Desktop.Controls/Splitter/SplitterToken.cs`。

## AOT 与裁剪注意事项

资源边界：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- handle、drag bar、button 的事件订阅必须在模板重套用或 handle 移除时解绑。
- `_trackedPanels` 中的面板属性订阅必须在面板离开时解绑。

性能边界：

- 面板集合变化时只重建必要的 handle 结构，避免把业务子控件重新包装成不可追踪结构。
- 拖拽过程中避免创建临时视觉对象。
- lazy 模式下拖拽反馈应复用现有 drag bar 状态，不引入额外弹层或全局输入监听。
- 分割线样式变化应通过 Avalonia 属性和模板绑定流动，不使用手动遍历视觉树刷新。

AOT 边界：

- API 与 Token 契约不通过运行时反射生成。
- Source generator 生成文件不手工编辑。
- 文档、Gallery 和源码发生冲突时，应修复源文档或结构化数据，不直接改 `docs/AI/generated/llms` 生成产物。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/Splitter/Splitter.cs`
- `src/AtomUI.Desktop.Controls/Splitter/Splitter.SemanticParts.cs`
- `src/AtomUI.Desktop.Controls/Splitter/SplitterPanel.cs`
- `src/AtomUI.Desktop.Controls/Splitter/SplitterHandle.cs`
- `src/AtomUI.Desktop.Controls/Splitter/SplitterDragBar.cs`
- `src/AtomUI.Desktop.Controls/Splitter/SplitterPanelCollapsible.cs`
- `src/AtomUI.Desktop.Controls/Splitter/SplitterResizeEventArgs.cs`
- `src/AtomUI.Desktop.Controls/Splitter/SplitterToken.cs`
- `src/AtomUI.Desktop.Controls/Splitter/Themes/SplitterTheme.axaml`
- `src/AtomUI.Desktop.Controls/Splitter/Themes/SplitterHandleTheme.axaml`
- `src/AtomUI.Desktop.Controls/Splitter/Themes/SplitterDragBarTheme.axaml`

Gallery 与文档结构：

- `controlgallery/AtomUIGallery/ShowCases/Layout/Splitter`
- `tests/AtomUI.Desktop.Controls.Tests/Splitter/SplitterSemanticPartTests.cs`
- `tests/AtomUIGallery.Tests/ShowCases/SplitterShowCasePageTests.cs`
- `tests/AtomUIGallery.Tests/ShowCases/SplitterShowCaseExamples.snapshot`
- `docs/controls/desktop/layout/splitter/`

职责边界：

- `Splitter.cs` 保留 public API、附加属性、Children 同步、template part 获取和 resize 事件抛出。
- `Splitter.SemanticParts.cs` 只承载 `[SemanticPart]` descriptor 声明，生成 `SplitterSemanticParts` 常量与
  `SplitterPanelStyle` / `SplitterDraggerStyle` Semantic Style。
- `SplitterPanel.cs` 保留布局、拖拽、折叠、尺寸约束、handle 状态刷新和 Semantic Part marker 维护。
- `SplitterHandle.cs` 保留单个 handle 的按钮、hover、dragging、collapse request 和 drag event 转发。
- `SplitterDragBar.cs` 保留 Thumb 输入事件和禁用拖拽拦截。
- Theme 文件负责静态视觉结构、template binding、selector、TokenResource 映射和静态 semantic marker 声明。
- Token 文件只提供组件视觉变量，不保存实例状态。

## 相关文档

- 源设计文档：`docs/controls/desktop/layout/splitter/overview.md`
- 实现文档：`docs/controls/desktop/layout/splitter/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/layout/splitter/semantic-part.md`
- Token 文档：`docs/controls/desktop/layout/splitter/token.md`
- 变更记录：`docs/controls/desktop/layout/splitter/changelog.md`
- 语义结构：`./semantic-cn.md`
