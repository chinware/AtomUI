# Splitter 分割面板

## 概述

分割面板（Splitter）允许用户通过拖拽手柄来动态调整两个或多个面板之间的大小比例。它广泛应用于 IDE 布局、仪表盘、文件管理器等需要灵活空间分配的场景。Splitter 支持水平与垂直方向分割、多面板嵌套、面板折叠/展开、最小/最大尺寸约束、百分比与像素混合指定尺寸等特性。

AtomUI 的 `Splitter` 控件对齐了 [Ant Design 5.0 Splitter](https://ant.design/components/splitter-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的 Splitter 设计哲学

Ant Design 5.0 的 Splitter 组件设计遵循以下原则：

- **直觉化拖拽**：用户通过拖拽分割手柄来调整面板大小，提供即时的视觉反馈。
- **灵活的尺寸约束**：支持百分比（`%`）和像素（`px`）两种尺寸单位，可混合使用，并可设置最小/最大尺寸约束。
- **可折叠面板**：面板可配置为可折叠（collapsible），用户可通过点击手柄上的箭头图标快速折叠/展开面板。
- **嵌套组合**：多个 Splitter 可以嵌套使用，形成复杂的多区域布局。
- **延迟渲染模式**：`IsLazy` 模式下，面板尺寸仅在拖拽释放时更新，适用于内容渲染开销较大的场景。

### AtomUI Splitter 的架构设计

AtomUI 的 Splitter 由三个核心部分组成：

| 组件 | 职责 |
|---|---|
| `Splitter` | 公共 API 入口控件，继承自 `TemplatedControl`。定义公共属性和附加属性，承载子面板集合，转发事件 |
| `SplitterPanel` | 内部布局面板（`internal`），继承自 `Panel`。负责全部布局计算、拖拽交互处理、折叠逻辑 |
| `SplitterHandle` | 内部拖拽手柄（`internal`），继承自 `Thumb`。负责拖拽交互和折叠按钮的呈现与响应 |

**设计分离理由**：

- `Splitter` 作为公共 API 面，暴露给用户的属性和事件均经过精心设计，保持最小化和清晰性。
- `SplitterPanel` 封装了复杂的布局算法（包括多面板尺寸计算、归一化、折叠状态管理），作为 `internal` 类隐藏实现细节。
- `SplitterHandle` 封装了拖拽 Thumb 交互、折叠按钮图标管理、悬浮侧检测等，作为 `internal` 类对用户不可见。

### 尺寸系统

Splitter 使用 `Dimension` 结构体（定义在 `AtomUI.Core`）来表达面板尺寸，支持两种单位：

| 单位类型 | AXAML 语法 | 含义 |
|---|---|---|
| 百分比 | `"30%"` | 占可用空间的百分比（0~100） |
| 像素 | `"200"` 或 `"200px"` | 固定像素值 |

尺寸通过附加属性设置在每个子面板上：

- **`Splitter.Size`**：当前面板尺寸（双向绑定），拖拽过程中会实时更新。
- **`Splitter.DefaultSize`**：面板初始尺寸，仅在首次布局时生效。
- **`Splitter.MinSize`**：面板最小尺寸约束。
- **`Splitter.MaxSize`**：面板最大尺寸约束。

未指定尺寸的面板将平分剩余空间。

### 折叠系统

面板的折叠行为通过 `Splitter.Collapsible` 附加属性配置，使用 `SplitterPanelCollapsible` 类型：

| 属性 | 类型 | 说明 |
|---|---|---|
| `IsEnabled` | `bool` | 是否允许折叠 |
| `ShowCollapsibleIcon` | `SplitterCollapsibleIconDisplayMode` | 折叠图标显示模式 |

`SplitterCollapsibleIconDisplayMode` 枚举：

| 值 | 说明 |
|---|---|
| `Hover`（默认 / `Auto`） | 鼠标悬浮在手柄上时显示折叠图标 |
| `Always`（`True`） | 始终显示折叠图标 |
| `Hidden`（`False`） | 隐藏折叠图标 |

折叠后的面板尺寸变为 0，展开时恢复到折叠前的尺寸。

---

## 功能详解

### 基本分割

最简单的使用方式是将两个或多个子控件放入 Splitter 中，系统自动在相邻面板之间生成拖拽手柄：

```xml
<atom:Splitter Orientation="Vertical" Height="220">
    <Border atom:Splitter.Size="30%">
        <TextBlock>First</TextBlock>
    </Border>
    <Border atom:Splitter.DefaultSize="100" atom:Splitter.MinSize="60">
        <TextBlock>Second</TextBlock>
    </Border>
</atom:Splitter>
```

### 多面板分割

Splitter 支持任意数量的子面板，系统会在每对相邻面板之间自动插入手柄：

```xml
<atom:Splitter Orientation="Vertical" Height="240">
    <Border atom:Splitter.Size="20%"><TextBlock>A</TextBlock></Border>
    <Border atom:Splitter.Size="20%"><TextBlock>B</TextBlock></Border>
    <Border><TextBlock>C</TextBlock></Border>
    <Border><TextBlock>D</TextBlock></Border>
</atom:Splitter>
```

### 嵌套分割

水平与垂直 Splitter 可以嵌套，构建复杂的多区域布局：

```xml
<atom:Splitter Orientation="Vertical" Height="260">
    <Border atom:Splitter.Size="40%">
        <TextBlock>Left</TextBlock>
    </Border>
    <atom:Splitter Orientation="Horizontal">
        <Border><TextBlock>Top</TextBlock></Border>
        <Border><TextBlock>Bottom</TextBlock></Border>
    </atom:Splitter>
</atom:Splitter>
```

### 禁用拖拽

通过 `Splitter.IsResizable="False"` 禁止特定面板参与拖拽：

```xml
<atom:Splitter Orientation="Vertical" Height="220">
    <Border atom:Splitter.Size="35%"><TextBlock>Resizable</TextBlock></Border>
    <Border atom:Splitter.DefaultSize="120" atom:Splitter.IsResizable="False">
        <TextBlock>Not Resizable</TextBlock>
    </Border>
    <Border atom:Splitter.DefaultSize="120"><TextBlock>Resizable</TextBlock></Border>
</atom:Splitter>
```

> **注意**：如果手柄两侧的**任一**面板设置了 `IsResizable="False"`，该手柄将不可拖拽，光标也不会变为调整大小光标。

### 面板折叠

通过 `Splitter.Collapsible` 配置面板可折叠：

```xml
<atom:Splitter Orientation="Vertical" Height="220">
    <Border atom:Splitter.DefaultSize="33%" atom:Splitter.Collapsible="Always">
        <TextBlock>First</TextBlock>
    </Border>
    <Border atom:Splitter.DefaultSize="34%" atom:Splitter.Collapsible="Hover">
        <TextBlock>Second</TextBlock>
    </Border>
    <Border atom:Splitter.Collapsible="True">
        <TextBlock>Third</TextBlock>
    </Border>
</atom:Splitter>
```

也可以通过代码动态设置：

```csharp
Splitter.SetCollapsible(panel, new SplitterPanelCollapsible
{
    IsEnabled           = true,
    ShowCollapsibleIcon = SplitterCollapsibleIconDisplayMode.Always
});
```

### 编程式折叠/展开

通过 `Splitter.IsCollapsed` 附加属性可以编程式控制面板折叠状态：

```xml
<Border x:Name="LeftPanel" atom:Splitter.IsCollapsed="{Binding IsLeftCollapsed}">
    ...
</Border>
```

### 延迟渲染模式

当 `IsLazy="True"` 时，拖拽过程中仅移动手柄的视觉位置（通过 `RenderTransform`），面板实际尺寸在释放鼠标后才更新。适用于面板内容渲染开销较大的场景：

```xml
<atom:Splitter Orientation="Vertical" Height="140" IsLazy="True">
    <Border atom:Splitter.Size="50%"><TextBlock>First</TextBlock></Border>
    <Border><TextBlock>Second</TextBlock></Border>
</atom:Splitter>
```

### 自定义折叠图标

通过 `CollapsePreviousIcon` 和 `CollapseNextIcon` 属性可自定义手柄上的折叠箭头图标：

```xml
<atom:Splitter CollapsePreviousIcon="{antdicons:AntDesignIconProvider Kind=LeftOutlined}"
               CollapseNextIcon="{antdicons:AntDesignIconProvider Kind=RightOutlined}">
    ...
</atom:Splitter>
```

默认图标根据方向自动选择：
- 垂直分割：`LeftOutlined` / `RightOutlined`
- 水平分割：`UpOutlined` / `DownOutlined`

### 拖拽事件

Splitter 提供三个拖拽事件用于监听尺寸变化：

| 事件 | 触发时机 |
|---|---|
| `ResizeStarted` | 拖拽开始 |
| `ResizeDelta` | 拖拽过程中（持续触发） |
| `ResizeCompleted` | 拖拽结束 |

事件参数 `SplitterResizeEventArgs` 包含：
- `HandleIndex`：被拖拽的手柄索引
- `Sizes`：所有面板的当前尺寸列表（`IReadOnlyList<double>`）

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 水平/垂直方向 | ✅ `layout: horizontal / vertical` | ✅ `Orientation` 属性 | ✅ 完全对齐 |
| 面板尺寸 `size` | ✅ 百分比 / 像素 / 自适应 | ✅ `Splitter.Size` 附加属性（`Dimension` 类型） | ✅ 完全对齐 |
| 默认尺寸 `defaultSize` | ✅ 面板初始尺寸 | ✅ `Splitter.DefaultSize` 附加属性 | ✅ 完全对齐 |
| 最小/最大约束 `min` / `max` | ✅ 面板约束 | ✅ `Splitter.MinSize` / `Splitter.MaxSize` | ✅ 完全对齐 |
| 禁止拖拽 `resizable` | ✅ 布尔属性 | ✅ `Splitter.IsResizable` 附加属性 | ✅ 完全对齐 |
| 面板折叠 `collapsible` | ✅ 折叠配置 | ✅ `Splitter.Collapsible` 附加属性 | ✅ 完全对齐 |
| 折叠图标显示模式 | ✅ `showCollapsibleIcon` | ✅ `SplitterCollapsibleIconDisplayMode` | ✅ 完全对齐 |
| 延迟渲染 `lazy` | ✅ 布尔属性 | ✅ `IsLazy` 属性 | ✅ 完全对齐 |
| 拖拽事件 `onResize*` | ✅ 回调函数 | ✅ `ResizeStarted` / `ResizeDelta` / `ResizeCompleted` | ✅ 完全对齐 |
| 嵌套分割 | ✅ 支持 | ✅ Splitter 可嵌套 | ✅ 完全对齐 |
| 多面板 | ✅ 支持 | ✅ 任意数量子面板 | ✅ 完全对齐 |
| 自定义折叠图标 | ✅ `collapsible.start` / `.end` | ✅ `CollapsePreviousIcon` / `CollapseNextIcon` | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.Splitter
```

**注意**：Splitter 控件目前仅存在于桌面平台层（`AtomUI.Desktop.Controls`），没有设备无关的抽象基类（`AtomUI.Controls` 中无 `AbstractSplitter`）。这是因为 Splitter 的交互模式（鼠标拖拽）在移动端有本质不同，未来移动端可能采用完全不同的实现方案。

### 内部组件关系

```
Splitter (public, TemplatedControl)
  ├── [template] SplitterPanel (internal, Panel)
  │     ├── 子面板 (用户提供的 Control)
  │     └── SplitterHandle[] (internal, Thumb) — 自动生成
  │           ├── [template] HandleLine (Border)
  │           ├── [template] HandleGrip (Border)
  │           ├── [template] CollapsePrevButton (IconButton)
  │           └── [template] CollapseNextButton (IconButton)
```

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Splitter/Splitter.cs` | 公共 API 入口控件 |
| 内部布局面板 | `src/AtomUI.Desktop.Controls/Splitter/SplitterPanel.cs` | 布局计算与拖拽处理 |
| 内部拖拽手柄 | `src/AtomUI.Desktop.Controls/Splitter/SplitterHandle.cs` | 手柄交互与折叠按钮 |
| 折叠配置 | `src/AtomUI.Desktop.Controls/Splitter/SplitterPanelCollapsible.cs` | `SplitterPanelCollapsible` 类与枚举 |
| 事件参数 | `src/AtomUI.Desktop.Controls/Splitter/SplitterResizeEventArgs.cs` | `SplitterResizeEventArgs` 类 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Splitter/SplitterToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Splitter/Themes/SplitterTheme.axaml` | Splitter ControlTheme |
| 手柄主题 | `src/AtomUI.Desktop.Controls/Splitter/Themes/SplitterHandleTheme.axaml` | SplitterHandle ControlTheme |
| 模板常量 | `src/AtomUI.Desktop.Controls/Splitter/Themes/SplitterThemeConstants.cs` | 模板部件名称常量 |
| 主题注册 | `src/AtomUI.Desktop.Controls/Splitter/Themes/SplitterThemes.axaml` | 主题合并入口 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Layout/SplitterShowCase.axaml` | 使用范例 |

---

## 模板结构

### Splitter 模板

```
Border#Frame
  └── SplitterPanel (PART_SplitterPanel)
        ├── 用户子面板 [0]
        ├── SplitterHandle [0]     ← 自动生成
        ├── 用户子面板 [1]
        ├── SplitterHandle [1]     ← 自动生成
        └── 用户子面板 [2]
```

Splitter 的模板非常简洁——仅包含一个 `SplitterPanel`，所有子面板和手柄的管理都由 `SplitterPanel` 内部负责。

### SplitterHandle 模板

```
Border (Background=Transparent, ClipToBounds=False)
  └── Grid
        ├── Border#PART_HandleLine           ← 分割线（贯穿全高/全宽）
        ├── Border#PART_HandleGrip           ← 手柄握持区（中央小条）
        └── Canvas#PART_CollapseIconsHost    ← 折叠图标容器（ZIndex 高层）
              ├── IconButton#PART_CollapsePrevButton  ← 向前折叠按钮
              └── IconButton#PART_CollapseNextButton  ← 向后折叠按钮
```

**设计要点**：
- 手柄区域的背景设为透明，实际可拖拽区域由 `SplitTriggerSize` Token 控制，大于可见分割线宽度，提升拖拽易用性。
- 折叠按钮使用 `Canvas` 容器定位，`ClipToBounds=False`，允许按钮超出手柄可见区域显示。
- 根据 `Orientation` 属性动态切换手柄线条和握持区的方向布局。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `SplitterThemeConstants.SplitterPanelPart` | `"PART_SplitterPanel"` | 内部布局面板 |
| `SplitterThemeConstants.HandleLinePart` | `"PART_HandleLine"` | 手柄分割线 |
| `SplitterThemeConstants.HandleGripPart` | `"PART_HandleGrip"` | 手柄握持区 |
| `SplitterThemeConstants.CollapseIconsHostPart` | `"PART_CollapseIconsHost"` | 折叠图标容器 |
| `SplitterThemeConstants.CollapsePrevButtonPart` | `"PART_CollapsePrevButton"` | 向前折叠按钮 |
| `SplitterThemeConstants.CollapseNextButtonPart` | `"PART_CollapseNextButton"` | 向后折叠按钮 |
