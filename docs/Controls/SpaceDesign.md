# Space 控件实现原理

## 1. 概述

AtomUI 的 `Space` 控件系列是对 [Ant Design 5.0 Space 组件](https://ant.design/components/space) 的 C# / Avalonia 实现。Ant Design 中 `Space` 组件的设计意图是：**为相邻组件提供统一的间距管理**，避免开发者手动为每个组件设置 `margin` 或 `padding`。

AtomUI 将 Space 组件拆分为两个独立控件：

| 控件 | 对应 Ant Design | 功能定位 |
|---|---|---|
| `Space` | `Space` | 通用间距布局面板，支持水平/垂直排列、自动换行、间距预设（Small/Middle/Large/Custom）、Split 分隔符 |
| `CompactSpace` | `Space.Compact` | 紧凑组合模式，将多个控件无间距地组合在一起，自动处理圆角裁剪和边框重叠 |

二者虽然都位于 `AtomUI.Desktop.Controls/Space/` 目录下，但功能和接口设计完全不同。

---

## 2. Space 控件（通用间距面板）

### 2.1 核心属性

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Orientation` | `Orientation` | `Horizontal` | 排列方向 |
| `SizeType` | `CustomizableSizeType` | `Small` | 间距预设档位（Small/Middle/Large/Custom） |
| `ItemSpacing` | `double` | 由 SizeType 决定 | 主轴方向的子项间距 |
| `LineSpacing` | `double` | 由 SizeType 决定 | 换行时的行间距 |
| `ItemsAlignment` | `SpaceItemsAlignment` | `Start` | 交叉轴方向对齐方式（Start/Center/End） |
| `ItemWidth` / `ItemHeight` | `double` | `NaN` | 统一子项宽/高（不设置则使用子项自身尺寸） |
| `SplitTemplate` | `ITemplate<Control>?` | `null` | 子项之间插入的分隔符模板 |
| `Children` | `AvaloniaList<Control>` | — | 子控件列表（标记为 `[Content]`） |

### 2.2 间距计算与 Token 系统

Space 的间距通过 Design Token 系统驱动。在构造函数中，`Space` 通过 `ConfigureInstanceStyle()` 方法注册了三组内联 `Style`，分别绑定到 `SpaceToken` 中的 Token 值：

| SizeType | ItemSpacing / LineSpacing 来源 | SharedToken 对应值 |
|---|---|---|
| `Small` | `SpaceTokenKind.GapSmallSize` | `SharedToken.SpacingXS`（8px） |
| `Middle` | `SpaceTokenKind.GapMiddleSize` | `SharedToken.Spacing`（16px）|
| `Large` | `SpaceTokenKind.GapLargeSize` | `SharedToken.SpacingLG`（24px）|
| `Custom` | 不绑定 Token，由用户直接设置 `ItemSpacing` / `LineSpacing` | — |

```csharp
// SpaceToken.CalculateTokenValues 中的推导逻辑
GapSmallSize  = SharedToken.SpacingXS;
GapMiddleSize = SharedToken.Spacing;
GapLargeSize  = SharedToken.SpacingLG;
```

### 2.3 布局算法

Space 的布局采用 **UV 坐标系模型**（类似 WPF WrapPanel），通过内部 `UVSize` 结构体在水平和垂直方向之间统一抽象：

- **U 轴**：主轴方向（Horizontal 时为水平，Vertical 时为垂直）
- **V 轴**：交叉轴方向

#### MeasureOverride

```
遍历 VisualChildren:
    计算每个子项的 childSize（尊重 ItemWidth/ItemHeight 设置）
    如果当前行累计 U + childSize.U + spacing > 可用空间 U:
        换行：记录当前行高度到 panelSize.V，重置 curLineSize
    否则：
        累加到当前行 curLineSize.U += childSize.U + spacing
    最终合并所有行的尺寸
```

关键细节：
- 不可见子项（`!child.IsVisible`）不产生间距（`nextSpacing = 0`）
- 换行时 `lineSpacing` 仅在已有前一行时才累加

#### ArrangeOverride

按行逐个排列子项，支持 `SpaceItemsAlignment` 交叉轴对齐：
- `Start`：子项贴行的起始边
- `Center`：子项在行高中居中
- `End`：子项贴行的末端

### 2.4 SplitTemplate（分隔符）

当设置了 `SplitTemplate` 时，Space 会在每两个子项之间插入一个由模板实例化的控件。此时集合变更的处理逻辑变为完全重建 Visual/Logical 树（`HandleSplitTemplateChanged`），因为分隔符的数量和位置与子项数量耦合。

### 2.5 对子控件的要求

**Space 对子控件没有任何接口要求**。任何 `Control` 都可以作为 `Space` 的子项，Space 只负责间距排列，不会修改子控件的任何属性。

---

## 3. CompactSpace 控件（紧凑组合面板）

### 3.1 设计意图

CompactSpace 对应 Ant Design 的 `Space.Compact`，用于将多个表单控件（按钮、输入框、选择器等）无间距地组合在一起，形成一个视觉整体。它的核心行为包括：

1. **零间距排列**：子项之间没有间距，紧密贴合
2. **圆角裁剪**：首个子项保留起始侧圆角，末个子项保留结束侧圆角，中间子项圆角全部清零
3. **边框重叠消除**：通过 `RenderTransform` 负偏移消除相邻子项的重复边框
4. **ZIndex 动态管理**：聚焦/悬停的子项自动提升 ZIndex，确保边框不被遮盖

### 3.2 核心属性

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Orientation` | `Orientation` | `Horizontal` | 排列方向 |
| `SizeType` | `SizeType` | `Middle` | 尺寸档位，自动传递给子控件 |
| `Children` | `AvaloniaList<Control>` | — | 子控件列表（必须实现 `ICompactSpaceAware`） |
| `ItemSize`（附加属性） | `CompactSpaceSize` | `Auto` | 子项宽度分配模式（Auto / 像素值 / Star 比例值） |

### 3.3 ICompactSpaceAware 接口

这是 CompactSpace 与子控件交互的**核心契约**。定义在 `ICompactSpaceAware.cs`：

```csharp
internal interface ICompactSpaceAware
{
    void NotifyPositionChange(SpaceItemPosition? position);
    void NotifyOrientationChange(Orientation orientation);
    bool IsAlwaysActiveZIndex() => false;
    bool IgnoreZIndexChange() => false;
    double GetBorderThickness();
}
```

| 方法 | 调用时机 | 子控件应做的响应 |
|---|---|---|
| `NotifyPositionChange(position)` | 子项集合变化或排列更新时 | 更新自身圆角裁剪策略 |
| `NotifyOrientationChange(orientation)` | 方向变化时 | 更新圆角和偏移方向 |
| `IsAlwaysActiveZIndex()` | 添加子项时查询 | 返回 `true` 则始终置于高 ZIndex（如 Primary Button） |
| `IgnoreZIndexChange()` | 添加子项时查询 | 返回 `true` 则不参与 Focus/Hover ZIndex 管理 |
| `GetBorderThickness()` | 布局阶段计算偏移量 | 返回主轴方向的边框厚度 |

### 3.4 位置通知与圆角裁剪

CompactSpace 在 `ConfigureSizeDefinitions()` 中计算每个子项的位置：

```
SpaceItemPosition.First   — 第一个子项
SpaceItemPosition.Middle  — 中间子项
SpaceItemPosition.Last    — 最后一个子项
First | Last              — 唯一的子项（保留全部圆角）
```

子控件收到 `NotifyPositionChange` 后，通过静态辅助方法 `CompactSpace.CalculateEffectiveCornerRadius()` 计算有效圆角：

```
水平方向:
  First  → 清零右侧圆角（TopRight = 0, BottomRight = 0）
  Middle → 全部圆角清零
  Last   → 清零左侧圆角（TopLeft = 0, BottomLeft = 0）

垂直方向:
  First  → 清零底部圆角
  Middle → 全部圆角清零
  Last   → 清零顶部圆角
```

### 3.5 边框重叠消除

`CompactSpaceItem`（每个子项的包装容器）在 `MeasureOverride` 中，根据子项位置和边框厚度计算负偏移：

```csharp
var delta = borderThickness * PositionIndex;
if (CompactSpaceOrientation == Orientation.Horizontal)
    RenderTransform = new TranslateTransform(-delta, 0);
else
    RenderTransform = new TranslateTransform(0, -delta);
```

这使得第 N 个子项向前偏移 N × borderThickness 像素，实现视觉上的边框合并。

### 3.6 ZIndex 动态管理

CompactSpace 为子项注册 `GotFocus`、`PointerEntered`、`PointerExited` 事件和 `FocusWithIn` 伪类监听：

- **聚焦/悬停时**：当前子项 `ZIndex = 1000`（`ACTIVE_ZINDEX`），其他非 Active 子项重置为 `0`
- **`IsAlwaysActiveZIndex()`**：Primary Button 等始终保持高 ZIndex，避免被相邻控件遮盖
- **`IgnoreZIndexChange()`**：如 `CompactSpaceAddOn` 不参与 ZIndex 管理

### 3.7 CompactSpaceSize 尺寸分配

通过附加属性 `CompactSpace.ItemSize` 控制子项在 Grid 中的空间分配：

| 设置方式 | 说明 | 示例 |
|---|---|---|
| `Auto`（默认） | 子项按自身尺寸显示 | `<atom:Button />` |
| 像素值 | 固定像素宽/高 | `atom:CompactSpace.ItemSize="100"` |
| Star 比例值 | 按比例分配剩余空间 | `atom:CompactSpace.ItemSize="3*"` |

### 3.8 辅助组件

| 组件 | 说明 |
|---|---|
| `CompactSpaceItem` | 内部包装容器（`Decorator`），负责中继 `ICompactSpaceAware` 属性、计算偏移量 |
| `CompactSpaceAddOn` | 用于在 CompactSpace 中添加前/后缀标签（如 `$` 符号），实现 `ICompactSpaceAware` |
| `CompactSpaceFiller` | 占位空白区域，不参与圆角/ZIndex/边框计算 |

---

## 4. 已实现 ICompactSpaceAware 的控件

以下控件已实现 `ICompactSpaceAware` 接口，可直接放入 `CompactSpace`：

| 控件 | 所在项目 | 特殊行为 |
|---|---|---|
| `Button` | AtomUI.Desktop.Controls | `IsAlwaysActiveZIndex()` → Primary 类型返回 `true` |
| `TextBox` | AtomUI.Desktop.Controls | 标准圆角裁剪和边框偏移 |
| `ButtonSpinner` | AtomUI.Desktop.Controls | 通过 `DecoratedBox` 判断边框样式 |
| `NumericUpDown` | AtomUI.Desktop.Controls | 继承自 ButtonSpinner 的行为 |
| `AbstractSelect` | AtomUI.Desktop.Controls | 所有选择器的基类 |
| `InfoPickerInput` | AtomUI.Desktop.Controls | 日期/时间选择器的基类 |
| `CompactSpaceAddOn` | AtomUI.Desktop.Controls | `IgnoreZIndexChange()` → 返回 `true` |
| `CompactSpaceFiller` | AtomUI.Desktop.Controls | 空操作实现，不参与任何计算 |
| `AbstractColorPicker` | AtomUI.Desktop.Controls.ColorPicker | 颜色选择器 |

### 实现模式示例（以 Button 为例）

```csharp
public class Button : AvaloniaButton,
                      ISizeTypeAware,
                      ICompactSpaceAware,  // ← 实现接口
                      ...
{
    // 1. 注册 CompactSpace 相关的 StyledProperty（使用 AddOwner 模式）
    internal static readonly StyledProperty<SpaceItemPosition?> CompactSpaceItemPositionProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceItemPositionProperty.AddOwner<Button>();
    
    internal static readonly StyledProperty<Orientation> CompactSpaceOrientationProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceOrientationProperty.AddOwner<Button>();
    
    internal static readonly StyledProperty<bool> IsUsedInCompactSpaceProperty = 
        CompactSpaceAwareControlProperty.IsUsedInCompactSpaceProperty.AddOwner<Button>();

    // 2. 声明 EffectiveCornerRadius（主题中使用此属性代替 CornerRadius）
    internal static readonly DirectProperty<Button, CornerRadius> EffectiveCornerRadiusProperty = ...;

    // 3. 监听属性变化，更新有效圆角
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ...
        if (change.Property == CornerRadiusProperty ||
            change.Property == CompactSpaceItemPositionProperty ||
            change.Property == CompactSpaceOrientationProperty)
        {
            ConfigureEffectiveCornerRadius();
        }
    }

    // 4. 使用静态辅助方法计算有效圆角
    private void ConfigureEffectiveCornerRadius()
    {
        EffectiveCornerRadius = CompactSpace.CalculateEffectiveCornerRadius(
            CornerRadius, IsUsedInCompactSpace, 
            CompactSpaceItemPosition, CompactSpaceOrientation);
    }

    // 5. 实现 ICompactSpaceAware 接口方法
    void ICompactSpaceAware.NotifyPositionChange(SpaceItemPosition? position)
    {
        IsUsedInCompactSpace     = position != null;
        CompactSpaceItemPosition = position;
    }

    void ICompactSpaceAware.NotifyOrientationChange(Orientation orientation)
    {
        CompactSpaceOrientation = orientation;
    }

    bool ICompactSpaceAware.IsAlwaysActiveZIndex()
    {
        return ButtonType == ButtonType.Primary;  // Primary 按钮始终高 ZIndex
    }

    double ICompactSpaceAware.GetBorderThickness()
    {
        return CompactSpaceOrientation == Orientation.Horizontal 
            ? BorderThickness.Left : BorderThickness.Top;
    }
}
```

### 共享属性定义

所有 `ICompactSpaceAware` 控件通过 `CompactSpaceAwareControlProperty` 共享属性（使用 `AddOwner` 模式）：

```csharp
internal abstract class CompactSpaceAwareControlProperty
{
    public static readonly StyledProperty<SpaceItemPosition?> CompactSpaceItemPositionProperty = ...;
    public static readonly StyledProperty<Orientation> CompactSpaceOrientationProperty = ...;
    public static readonly StyledProperty<bool> IsUsedInCompactSpaceProperty = ...;
}
```

---

## 5. 架构关系图

```
CompactSpace
  │
  ├── ConfigureSizeDefinitions()       ── 计算子项位置并通知
  │     │
  │     ├── ICompactSpaceAware.NotifyPositionChange(First/Middle/Last)
  │     └── ICompactSpaceAware.NotifyOrientationChange(Horizontal/Vertical)
  │
  ├── NotifyAddCompactSpaceItem()      ── 包装为 CompactSpaceItem + 事件绑定
  │     │
  │     ├── GotFocus / PointerEntered / PointerExited → ZIndex 管理
  │     ├── ISizeTypeAware? → 绑定 SizeType
  │     └── CompactSpaceItem → RelayBind 属性到子控件
  │
  └── CompactSpaceItem (Decorator)     ── 计算 RenderTransform 偏移消除边框重叠
        │
        └── 子控件 (ICompactSpaceAware)
              │
              ├── EffectiveCornerRadius ← CalculateEffectiveCornerRadius()
              ├── GetBorderThickness()  → 用于偏移量计算
              └── IsAlwaysActiveZIndex() / IgnoreZIndexChange() → ZIndex 策略
```

