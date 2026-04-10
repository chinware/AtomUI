# AtomUI 媒体查询系统 — 架构设计

> 本文档描述 AtomUI 媒体查询系统的内部架构，包括断点检测管道、核心类型定义、事件流转机制以及各层组件的职责划分。

---

## 1. 架构总览

媒体查询系统由三层组件协作实现：

```
┌────────────────────────────────────────────────────────────────────────┐
│  Window 主题 (WindowTheme.axaml)                                       │
│  ┌──────────────────────────────────────────────────────────────────┐  │
│  │  ContainerQuery 声明（6 组 min-width / max-width 查询）           │  │
│  │  → 驱动 WindowMediaQueryIndicator.MediaBreakPoint 属性变更        │  │
│  └──────────────────────────────────────────────────────────────────┘  │
│                          │ 属性变更                                     │
│                          ▼                                             │
│  ┌──────────────────────────────────────────────────────────────────┐  │
│  │  WindowMediaQueryIndicator (internal)                             │  │
│  │  → 调用 Window.NotifyMediaBreakPointChanged()                    │  │
│  └──────────────────────────────────────────────────────────────────┘  │
│                          │ 事件通知                                     │
│                          ▼                                             │
│  ┌──────────────────────────────────────────────────────────────────┐  │
│  │  Window : IMediaBreakAwareControl                                 │  │
│  │  → 更新 MediaBreakPoint 属性                                      │  │
│  │  → 触发 MediaBreakPointChanged 事件                               │  │
│  └──────────────────────────────────────────────────────────────────┘  │
│                          │ 事件订阅                                     │
│                          ▼                                             │
│  ┌──────────────────────────────────────────────────────────────────┐  │
│  │  消费控件：Row, FormItem, Descriptions 等                          │  │
│  │  → 订阅 Window.MediaBreakPointChanged                            │  │
│  │  → 根据新断点重新计算布局                                           │  │
│  └──────────────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 2. 断点检测管道

### 2.1 容器声明

在 `WindowTheme.axaml` 的控件模板中，Window 的内容区域 `Border#ContentFrame` 被声明为查询容器：

```xml
<Border Name="ContentFrame"
        Container.Name="{x:Static atom:IMediaBreakAwareControl.GlobalQueryContainerName}"
        Container.Sizing="WidthAndHeight"
        Padding="{TemplateBinding Padding}">
    <Panel>
        <ContentPresenter ... />
        <atom:WindowMediaQueryIndicator
            Name="{x:Static atom:WindowThemeConstants.MediaQueryIndicatorPart}" />
    </Panel>
</Border>
```

关键设置：
- **`Container.Name`**：值为 `IMediaBreakAwareControl.GlobalQueryContainerName`（常量 `"PART_GlobalQueryContainer"`），所有 ContainerQuery 通过此名称关联。
- **`Container.Sizing`**：设为 `WidthAndHeight`，使容器跟踪宽度和高度的实际尺寸。

### 2.2 ContainerQuery 断点规则

在 `WindowTheme.axaml` 的 ControlTheme 顶层声明了 6 组 ContainerQuery，覆盖所有断点：

```xml
<!-- xs: 宽度 ≤ 575 -->
<ContainerQuery Name="{x:Static atom:IMediaBreakAwareControl.GlobalQueryContainerName}"
                Query="max-width:575">
    <Style Selector="atom|WindowMediaQueryIndicator#PART_MediaQueryIndicator">
        <Setter Property="MediaBreakPoint" Value="ExtraSmall" />
    </Style>
</ContainerQuery>

<!-- sm: 宽度 ≥ 576 -->
<ContainerQuery Name="..." Query="min-width:576">
    <Style ...><Setter Property="MediaBreakPoint" Value="Small" /></Style>
</ContainerQuery>

<!-- md: 宽度 ≥ 768 -->
<ContainerQuery Name="..." Query="min-width:768">
    <Style ...><Setter Property="MediaBreakPoint" Value="Medium" /></Style>
</ContainerQuery>

<!-- lg: 宽度 ≥ 992 -->
<ContainerQuery Name="..." Query="min-width:992">
    <Style ...><Setter Property="MediaBreakPoint" Value="Large" /></Style>
</ContainerQuery>

<!-- xl: 宽度 ≥ 1200 -->
<ContainerQuery Name="..." Query="min-width:1200">
    <Style ...><Setter Property="MediaBreakPoint" Value="ExtraLarge" /></Style>
</ContainerQuery>

<!-- xxl: 宽度 ≥ 1600 -->
<ContainerQuery Name="..." Query="min-width:1600">
    <Style ...><Setter Property="MediaBreakPoint" Value="ExtraExtraLarge" /></Style>
</ContainerQuery>
```

**工作原理**：Avalonia 的 ContainerQuery 按声明顺序求值，后匹配的规则覆盖先匹配的。例如当容器宽度为 1024px 时：
1. `max-width:575` → 不匹配
2. `min-width:576` → 匹配，设为 `Small`
3. `min-width:768` → 匹配，**覆盖**为 `Medium`
4. `min-width:992` → 匹配，**覆盖**为 `Large`
5. `min-width:1200` → 不匹配
6. `min-width:1600` → 不匹配

最终结果：`MediaBreakPoint.Large`。

### 2.3 WindowMediaQueryIndicator（中继控件）

```csharp
// src/AtomUI.Desktop.Controls/Window/WindowMediaQueryIndicator.cs
internal class WindowMediaQueryIndicator : Control
{
    public static readonly StyledProperty<MediaBreakPoint> MediaBreakPointProperty = 
        MediaBreakAwareControlProperty.MediaBreakPointProperty.AddOwner<WindowMediaQueryIndicator>();

    public Window? OwnerWindow { get; set; }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == MediaBreakPointProperty)
        {
            OwnerWindow?.NotifyMediaBreakPointChanged(MediaBreakPoint);
        }
    }
}
```

**设计原因**：ContainerQuery 的 Style 不能直接影响容器本身或其祖先（Avalonia 约束，防止循环更新）。因此引入一个轻量的内部中继控件 `WindowMediaQueryIndicator`，它是容器的子孙控件，可以被 ContainerQuery 的 Style 正常设置属性。当其 `MediaBreakPoint` 属性被 ContainerQuery 更新时，它通知 `Window` 更新断点状态。

### 2.4 Window 断点分发

```csharp
// src/AtomUI.Desktop.Controls/Window/Window.cs
public class Window : AvaloniaWindow, IMediaBreakAwareControl, ...
{
    public static readonly StyledProperty<MediaBreakPoint> MediaBreakPointProperty = 
        MediaBreakAwareControlProperty.MediaBreakPointProperty.AddOwner<Window>();

    public MediaBreakPoint MediaBreakPoint
    {
        get => GetValue(MediaBreakPointProperty);
        internal set => SetValue(MediaBreakPointProperty, value);
    }

    public event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;

    internal void NotifyMediaBreakPointChanged(MediaBreakPoint breakPoint)
    {
        SetCurrentValue(MediaBreakPointProperty, breakPoint);
        MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(breakPoint));
    }
}
```

`Window` 在 `OnApplyTemplate` 中将自身引用注入 `WindowMediaQueryIndicator.OwnerWindow`，建立中继关系。

---

## 3. 核心类型定义

### 3.1 基础类型（`AtomUI.Controls.Shared/MediaQuery/`）

这些类型定义在基础共享层，所有控件项目均可引用。

#### `MediaBreakPoint` 枚举

```csharp
public enum MediaBreakPoint
{
    ExtraSmall      = 0,     // xs
    Small           = 576,   // sm
    Medium          = 768,   // md
    Large           = 992,   // lg (默认值)
    ExtraLarge      = 1200,  // xl
    ExtraExtraLarge = 1600,  // xxl
}
```

枚举值即为该断点的最小宽度像素值，可直接用于比较运算。

#### `IMediaBreakAwareControl` 接口

```csharp
public interface IMediaBreakAwareControl
{
    public const string GlobalQueryContainerName = "PART_GlobalQueryContainer";
    MediaBreakPoint MediaBreakPoint { get; }
    event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;
}
```

任何顶级容器（如 `Window`）实现此接口后，其子控件树中的响应式控件即可通过 `TopLevel.GetTopLevel(this) is IMediaBreakAwareControl` 获取断点信息。

#### `MediaBreakAwareControlProperty` 抽象属性持有者

```csharp
public abstract class MediaBreakAwareControlProperty
{
    public const string MediaBreakPointPropertyName = "MediaBreakPoint";
    
    public static readonly StyledProperty<MediaBreakPoint> MediaBreakPointProperty = 
        AvaloniaProperty.Register<StyledElement, MediaBreakPoint>(
            MediaBreakPointPropertyName, MediaBreakPoint.Large);
}
```

遵循 AtomUI 的共享属性模式（`AddOwner` 模式），供 `Window`、`WindowMediaQueryIndicator` 等控件复用同一 `StyledProperty` 定义。

#### `MediaBreakPointChangedEventArgs`

```csharp
public class MediaBreakPointChangedEventArgs : EventArgs
{
    public MediaBreakPoint MediaBreakPoint { get; }
}
```

#### `MediaBreakGridLength`

用于在不同断点下指定不同的 `GridLength` 值。典型用途：`Form` / `FormItem` 的 `LabelColInfo` 和 `WrapperColInfo` 属性。

```csharp
public record MediaBreakGridLength
{
    public GridLength ExtraSmall { get; init; }
    public GridLength Small { get; init; }
    public GridLength Medium { get; init; }
    public GridLength Large { get; init; }
    public GridLength ExtraLarge { get; init; }
    public GridLength ExtraExtraLarge { get; init; }

    // 支持从字符串解析：
    // 单一值: "2*"           → 所有断点使用 2*
    // 分断点: "xs:1*,lg:2*"  → xs 断点 1*, lg 断点 2*
    public static MediaBreakGridLength Parse(string input) { ... }
}
```

### 3.2 Grid 响应式类型（`AtomUI.Desktop.Controls/Grid/`）

#### `GridColSpanInfo`

用于 `Col.Span` 属性，支持在不同断点下指定不同的列跨度：

```csharp
[TypeConverter(typeof(GridColSpanInfoConverter))]
public readonly record struct GridColSpanInfo
{
    public int ExtraSmall { get; init; }
    public int Small { get; init; }
    public int Medium { get; init; }
    public int Large { get; init; }
    public int ExtraLarge { get; init; }
    public int ExtraExtraLarge { get; init; }

    // 单一值: "8"               → 所有断点 span=8
    // 分断点: "xs:24,sm:12,lg:8" → 按断点指定
    public static GridColSpanInfo Parse(string input) { ... }
    public int GetValue(MediaBreakPoint breakPoint) { ... }
}
```

#### `GridGutterInfo`

用于 `Row.Gutter` 的水平/垂直分量，支持按断点设置不同间距值：

```csharp
public record GridGutterInfo
{
    public double ExtraSmall { get; init; }
    public double Small { get; init; }
    // ... 6 个断点属性

    // 单一值: "16"                → 所有断点 16px
    // 分断点: "xs:8,sm:16,lg:24"  → 按断点指定
    public static GridGutterInfo Parse(string input) { ... }
    internal double GetValue(MediaBreakPoint breakPoint) { ... }
}
```

#### `GridGutter`

`Row.Gutter` 属性的完整类型，包含水平和垂直两个 `GridGutterInfo`：

```csharp
[TypeConverter(typeof(GridGutterConverter))]
public record GridGutter
{
    public GridGutterInfo Horizontal { get; init; }
    public GridGutterInfo Vertical { get; init; }

    // "16"          → 水平 16px, 垂直 0
    // "16,24"       → 水平 16px, 垂直 24px
    // "xs:8,lg:16"  → 水平按断点, 垂直 0
    // "xs:8,lg:16;xs:4,lg:12" → 水平和垂直各按断点（用 ; 分隔）
    public static GridGutter Parse(string input) { ... }
}
```

#### `GridColSize`

用于 `Col` 的响应式断点属性（`Xs`、`Sm`、`Md`、`Lg`、`Xl`、`Xxl`），可覆盖该断点下的 Span、Offset、Order、Push、Pull：

```csharp
[TypeConverter(typeof(GridColSizeConverter))]
public record GridColSize
{
    public int? Span { get; init; }
    public int? Offset { get; init; }
    public int? Order { get; init; }
    public int? Push { get; init; }
    public int? Pull { get; init; }

    // 纯数字: "12"                        → 仅设 Span=12
    // 键值对: "span:12,offset:2,order:1"  → 按名称设置
    public static GridColSize Parse(string input) { ... }
}
```

#### `DescriptionsMediaBreakInfo`

用于 `Descriptions.ColumnInfo` 和 `DescriptionItem.Span`，按断点指定列数或跨度：

```csharp
public record DescriptionsMediaBreakInfo
{
    public int ExtraSmall { get; init; }
    public int Small { get; init; }
    // ... 6 个断点属性

    // 单一值: "3"              → 所有断点 3 列
    // 分断点: "xs:1,sm:2,lg:3" → 按断点指定列数
    public static DescriptionsMediaBreakInfo Parse(string input) { ... }
}
```

---

## 4. 消费控件的订阅模式

所有需要响应断点变化的控件遵循统一的订阅模式：

```csharp
public class SomeResponsiveControl : Control
{
    private MediaBreakPoint? _breakPoint;
    private Window? _attachedWindow;

    // 1. 附加到视觉树时，订阅 Window 的断点变更事件
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (TopLevel.GetTopLevel(this) is Window window)
        {
            _attachedWindow = window;
            _breakPoint = window.MediaBreakPoint;
            window.MediaBreakPointChanged += HandleMediaBreakChanged;
        }
    }

    // 2. 从视觉树分离时，取消订阅
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_attachedWindow != null)
        {
            _attachedWindow.MediaBreakPointChanged -= HandleMediaBreakChanged;
            _attachedWindow = null;
        }
    }

    // 3. 断点变更时，重新计算布局
    private void HandleMediaBreakChanged(object? sender, MediaBreakPointChangedEventArgs args)
    {
        _breakPoint = args.MediaBreakPoint;
        InvalidateMeasure();
        InvalidateArrange();
    }
}
```

**注意**：
- `Row` 使用 `TopLevel.GetTopLevel(this) is IMediaBreakAwareControl` 来获取断点（更通用的接口方式）。
- `FormItem` 和 `Descriptions` 直接转型为 `Window`（因为桌面平台的 TopLevel 就是 Window）。
- 默认断点为 `MediaBreakPoint.Large`（992px），当无法获取窗口引用时使用此值。

---

## 5. 文件索引

### 基础层（`AtomUI.Controls.Shared/MediaQuery/`）

| 文件 | 类型 | 职责 |
|---|---|---|
| `MediaBreakPoints.cs` | `MediaBreakPoint` 枚举 | 六级断点定义，枚举值即最小宽度像素 |
| `IMediaBreakAware.cs` | `IMediaBreakAwareControl` 接口 + `MediaBreakAwareControlProperty` | 断点感知接口和共享属性定义 |
| `MediaBreakPointChangedEventArgs.cs` | `MediaBreakPointChangedEventArgs` | 断点变更事件参数 |
| `MediaBreakGridLength.cs` | `MediaBreakGridLength` record | 按断点的 GridLength 映射，用于 Form 布局 |

### 平台层（`AtomUI.Desktop.Controls/`）

| 文件 | 类型 | 职责 |
|---|---|---|
| `Window/Window.cs` | `Window` | 实现 `IMediaBreakAwareControl`，集中管理断点状态 |
| `Window/WindowMediaQueryIndicator.cs` | `WindowMediaQueryIndicator` | ContainerQuery 的中继控件 |
| `Window/Themes/WindowTheme.axaml` | ControlTheme | 声明容器和 6 组 ContainerQuery |
| `Grid/Row.cs` | `Row` | 24 栏栅格行布局，消费断点驱动 Gutter 和列布局 |
| `Grid/Col.cs` | `Col` | 栅格列，按断点解析 Span/Offset/Order/Push/Pull |
| `Grid/GridGutter.cs` | `GridGutter` | 响应式间距值（水平 + 垂直） |
| `Grid/GridGutterInfo.cs` | `GridGutterInfo` | 按断点的间距值映射 |
| `Grid/GridColSpanInfo.cs` | `GridColSpanInfo` | 按断点的列跨度映射 |
| `Grid/GridColSize.cs` | `GridColSize` + `GridColLayout` | 响应式列配置（完整覆盖） |
| `Grid/ColInfoExtension.cs` | `ColInfoExtension` | AXAML 标记扩展 `{atom:ColInfo}` |
| `Form/Form.cs` | `Form` | 表单容器，传递 `LabelColInfo` / `WrapperColInfo` |
| `Form/FormItem.cs` | `FormItem` | 表单项，按断点计算标签/内容列宽 |
| `Descriptions/Descriptions.cs` | `Descriptions` | 描述列表，按断点计算列数 |
| `Descriptions/DescriptionsMediaBreakInfo.cs` | `DescriptionsMediaBreakInfo` | 按断点的整数值映射（列数/跨度） |

