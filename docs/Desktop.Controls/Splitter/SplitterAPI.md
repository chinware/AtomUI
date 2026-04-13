# Splitter API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举与辅助类型

### SplitterCollapsibleIconDisplayMode

折叠图标显示模式枚举，控制手柄上折叠/展开箭头图标的显示时机。

| 值 | 别名 | 说明 |
|---|---|---|
| `Hover` | `Auto` | 鼠标悬浮在手柄上时显示（默认） |
| `Always` | `True` | 始终显示 |
| `Hidden` | `False` | 始终隐藏 |

### SplitterPanelCollapsible

面板折叠配置类，通过 `Splitter.Collapsible` 附加属性使用。支持从字符串自动转换（`TypeConverter`）。

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsEnabled` | `bool` | `true` | 是否允许折叠 |
| `ShowCollapsibleIcon` | `SplitterCollapsibleIconDisplayMode` | `Hover` | 折叠图标显示模式 |

**AXAML 中的字符串简写语法**：

| 字符串值 | 等价配置 |
|---|---|
| `"True"` / `"true"` | `IsEnabled = true` |
| `"False"` / `"false"` | `IsEnabled = false` |
| `"Hover"` | `ShowCollapsibleIcon = Hover` |
| `"Always"` | `ShowCollapsibleIcon = Always` |
| `"None"` | `ShowCollapsibleIcon = Hidden` |

### Dimension（来自 `AtomUI`）

尺寸值结构体，支持百分比和像素两种单位。

| 属性 | 类型 | 说明 |
|---|---|---|
| `Value` | `double` | 数值部分 |
| `UnitType` | `DimensionUnitType` | 单位类型（`Percentage` / `Pixel`） |
| `IsPercentage` | `bool` | 是否为百分比 |
| `IsAbsolute` | `bool` | 是否为像素 |

**AXAML 中的字符串语法**：`"30%"`（百分比）、`"200"` 或 `"200px"`（像素）。

### SplitterResizeEventArgs

拖拽事件参数类，继承自 `EventArgs`。

| 属性 | 类型 | 说明 |
|---|---|---|
| `HandleIndex` | `int` | 被拖拽的手柄索引（从 0 开始） |
| `Sizes` | `IReadOnlyList<double>` | 所有面板的当前尺寸列表（像素值） |

---

## Splitter 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Orientation` | `Orientation` | `Orientation.Vertical` | 分割方向。`Vertical` 为左右分割（水平排列面板），`Horizontal` 为上下分割（垂直排列面板） |
| `IsLazy` | `bool` | `false` | 延迟渲染模式。为 `true` 时拖拽过程中仅移动手柄视觉位置，释放后才更新面板尺寸 |
| `HandleSize` | `double` | 由 Token `SplitBarHandleSize` 控制 | 手柄区域的大小（像素），影响手柄的可拖拽范围 |
| `CollapsePreviousIcon` | `IconTemplate?` | `null`（使用内置方向箭头） | 自定义「向前折叠」按钮图标模板 |
| `CollapseNextIcon` | `IconTemplate?` | `null`（使用内置方向箭头） | 自定义「向后折叠」按钮图标模板 |

### 继承自 TemplatedControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `ClipToBounds` | `bool` | `true`（由主题设置） | 是否裁剪超出边界的内容 |
| `Height` / `Width` | `double` | `NaN` | 控件尺寸 |

---

## Splitter 内容属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Children` | `Controls`（`[Content]`） | 子面板集合。支持在 AXAML 中直接添加子控件，无需显式包裹 |

---

## Splitter 附加属性（AttachedProperty）

以下附加属性设置在 Splitter 的**子面板**上，用于控制每个面板的布局行为。

| 属性名 | 类型 | 默认值 | 绑定模式 | 说明 |
|---|---|---|---|---|
| `Splitter.Size` | `Dimension?` | `null` | `TwoWay` | 面板当前尺寸。支持百分比（`"30%"`）和像素（`"200"`）。拖拽过程中会实时更新。为 `null` 时面板参与弹性分配 |
| `Splitter.DefaultSize` | `Dimension?` | `null` | `OneWay` | 面板初始默认尺寸。仅在 `Size` 未设置时作为初始值使用 |
| `Splitter.MinSize` | `Dimension?` | `null` | `OneWay` | 面板最小尺寸约束。支持百分比和像素 |
| `Splitter.MaxSize` | `Dimension?` | `null` | `OneWay` | 面板最大尺寸约束。支持百分比和像素 |
| `Splitter.IsResizable` | `bool` | `true` | `OneWay` | 面板是否允许通过拖拽调整大小。设为 `false` 时相邻手柄不可拖拽 |
| `Splitter.Collapsible` | `SplitterPanelCollapsible?` | `null` | `OneWay` | 面板折叠配置。为 `null` 时面板不可折叠 |
| `Splitter.IsCollapsed` | `bool` | `false` | `TwoWay` | 面板是否处于折叠状态。可用于编程式控制折叠/展开 |

### 附加属性 AXAML 用法

```xml
<atom:Splitter Orientation="Vertical">
    <Border atom:Splitter.Size="30%"
            atom:Splitter.MinSize="100"
            atom:Splitter.MaxSize="50%"
            atom:Splitter.Collapsible="Always"
            atom:Splitter.IsCollapsed="False">
        <TextBlock>Panel Content</TextBlock>
    </Border>
    <Border atom:Splitter.DefaultSize="200"
            atom:Splitter.IsResizable="False">
        <TextBlock>Fixed Panel</TextBlock>
    </Border>
</atom:Splitter>
```

### 附加属性 C# 用法

```csharp
// 获取和设置面板尺寸
Dimension? size = Splitter.GetSize(panel);
Splitter.SetSize(panel, new Dimension(30, DimensionUnitType.Percentage));

// 获取和设置折叠配置
Splitter.SetCollapsible(panel, new SplitterPanelCollapsible
{
    IsEnabled = true,
    ShowCollapsibleIcon = SplitterCollapsibleIconDisplayMode.Always
});

// 编程式折叠/展开
Splitter.SetIsCollapsed(panel, true);   // 折叠
Splitter.SetIsCollapsed(panel, false);  // 展开

// 约束
Splitter.SetMinSize(panel, new Dimension(100, DimensionUnitType.Pixel));
Splitter.SetMaxSize(panel, new Dimension(50, DimensionUnitType.Percentage));

// 禁止拖拽
Splitter.SetIsResizable(panel, false);
```

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `ResizeStarted` | `EventHandler<SplitterResizeEventArgs>` | 拖拽开始时触发，提供拖拽手柄索引和初始面板尺寸 |
| `ResizeDelta` | `EventHandler<SplitterResizeEventArgs>` | 拖拽过程中持续触发，提供实时面板尺寸预览 |
| `ResizeCompleted` | `EventHandler<SplitterResizeEventArgs>` | 拖拽结束时触发，提供最终面板尺寸 |

### 事件用法

```csharp
splitter.ResizeStarted += (sender, e) =>
{
    Console.WriteLine($"开始拖拽手柄 {e.HandleIndex}");
    Console.WriteLine($"初始尺寸: {string.Join(", ", e.Sizes)}");
};

splitter.ResizeDelta += (sender, e) =>
{
    Console.WriteLine($"拖拽中: {string.Join(", ", e.Sizes)}");
};

splitter.ResizeCompleted += (sender, e) =>
{
    Console.WriteLine($"拖拽完成: {string.Join(", ", e.Sizes)}");
};
```

---

## 伪类（Pseudo-Classes）

### SplitterHandle 伪类

SplitterHandle 是内部控件，其伪类由主题系统使用：

| 伪类 | 触发条件 | 说明 |
|---|---|---|
| `:pointerover` | 鼠标悬浮在手柄上 | 分割线颜色变为 `HandleLineHoverColor` |
| `:dragging` | 正在拖拽手柄 | 分割线颜色变为 `HandleLineDragColor` |

---

## 内部组件（Internal）

以下组件为 `internal` 可见性，用户不应直接使用，但了解其结构有助于自定义样式。

### SplitterPanel

内部布局面板，继承自 `Avalonia.Controls.Panel`，负责所有布局和交互逻辑。

### SplitterHandle

内部拖拽手柄，继承自 `AtomUI.Controls.Primitives.Thumb`。

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Orientation` | `Orientation` | 手柄方向（从 Splitter 同步） |
| `LineBrush` | `IBrush?` | 分割线颜色（由主题控制） |
| `LineThickness` | `double` | 分割线粗细（由 Token `SplitBarSize` 控制） |
| `IsDragEnabled` | `bool` | 是否允许拖拽（根据相邻面板 `IsResizable` 自动计算） |
