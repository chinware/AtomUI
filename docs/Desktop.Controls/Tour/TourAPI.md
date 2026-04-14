# Tour API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### TourStyleType

Tour 风格类型枚举，定义气泡卡片的视觉风格。

| 值 | 说明 |
|---|---|
| `Default` | 默认风格，白色背景卡片 |
| `Primary` | 主色调风格，主色调背景卡片 |

### TourPlacementMode

弹出位置枚举，定义气泡卡片相对目标控件的弹出方向。

| 值 | 说明 |
|---|---|
| `Center` | 屏幕居中（无 Target 时自动使用） |
| `Left` | 目标左侧 |
| `LeftTop` | 目标左侧，上对齐 |
| `LeftBottom` | 目标左侧，下对齐 |
| `Right` | 目标右侧 |
| `RightTop` | 目标右侧，上对齐 |
| `RightBottom` | 目标右侧，下对齐 |
| `Top` | 目标上方 |
| `TopLeft` | 目标上方，左对齐 |
| `TopRight` | 目标上方，右对齐 |
| `Bottom` | 目标下方（默认） |
| `BottomLeft` | 目标下方，左对齐 |
| `BottomRight` | 目标下方，右对齐 |

---

## Tour 公共属性（StyledProperty / DirectProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsOpen` | `bool` | `false` | 控制 Tour 的显示/隐藏，支持双向绑定 |
| `CurrentIndex` | `int` | `-1` | 当前显示步骤的索引，支持双向绑定（`DirectProperty`） |
| `StepCount` | `int` | `0` | 步骤总数，只读（`DirectProperty`） |
| `StyleType` | `TourStyleType` | `TourStyleType.Default` | 全局风格类型，每个步骤可单独覆盖 |
| `Placement` | `TourPlacementMode` | `TourPlacementMode.Bottom` | 默认弹出位置，每个步骤可单独覆盖 |
| `IsShowArrow` | `bool` | `true` | 是否显示指向目标的箭头 |
| `IsPointAtCenter` | `bool` | `false` | 箭头是否指向目标中心 |
| `IsShowMask` | `bool` | `true` | 是否显示遮罩层 |
| `MaskColor` | `IBrush?` | `SharedToken.ColorBgMask` | 遮罩层颜色 |
| `GapOffsetX` | `double` | `6` | 镂空区域水平外扩偏移（px） |
| `GapOffsetY` | `double` | `6` | 镂空区域垂直外扩偏移（px） |
| `GapRadius` | `double` | `2` | 镂空区域圆角半径（px） |
| `IsScrollIntoView` | `bool` | `true` | 是否自动将目标控件滚动到可视区域 |
| `IsDisabledInteraction` | `bool` | `false` | 是否禁用目标控件的交互 |
| `CloseIcon` | `IIconTemplate?` | `CloseOutlined` 图标 | 关闭按钮图标模板 |
| `Indicator` | `TourIndicator?` | `DefaultTourIndicator` | 步骤指示器实例 |
| `StepsSource` | `IEnumerable<ITourStepOption>?` | `null` | 步骤数据源（用于数据绑定场景） |
| `ItemTemplate` | `IDataTemplate?` | `null` | 数据绑定场景下的步骤项模板 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（共享属性） |

### Tour 集合属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Steps` | `ItemCollection`（`[Content]` 属性） | 步骤集合，在 AXAML 中直接声明 `TourStep` 子元素 |
| `CustomActions` | `ItemCollection` | 自定义操作按钮集合，插入到导航区域前方 |

---

## TourStep 公共属性（StyledProperty）

`TourStep` 继承自 `ContentControl`，实现 `ITourStepOption` 接口。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Target` | `Control?` | `null` | 目标控件引用（通过 `ElementName` 绑定），为 null 时居中显示 |
| `Title` | `object?` | `null` | 步骤标题（支持字符串或自定义控件） |
| `TitleTemplate` | `IDataTemplate?` | `null` | 标题数据模板 |
| `Description` | `object?` | `null` | 步骤描述文字 |
| `DescriptionTemplate` | `IDataTemplate?` | `null` | 描述数据模板 |
| `Cover` | `object?` | `null` | 封面图内容（Image 或任意控件） |
| `CoverTemplate` | `IDataTemplate?` | `null` | 封面数据模板 |
| `CloseIcon` | `PathIcon?` | `null` | 关闭按钮图标（覆盖 Tour 全局设置） |
| `IsShowArrow` | `bool?` | `null`（跟随 Tour） | 是否显示箭头（覆盖 Tour 全局设置） |
| `IsPointAtCenter` | `bool?` | `null`（跟随 Tour） | 箭头是否指向中心（覆盖 Tour 全局设置） |
| `Placement` | `TourPlacementMode?` | `null`（跟随 Tour） | 弹出位置（覆盖 Tour 全局设置） |
| `StyleType` | `TourStyleType?` | `null`（跟随 Tour） | 视觉风格（覆盖 Tour 全局设置） |
| `IsShowMask` | `bool?` | `null`（跟随 Tour） | 是否显示遮罩（覆盖 Tour 全局设置） |
| `MaskColor` | `IBrush?` | `null`（跟随 Tour） | 遮罩颜色（覆盖 Tour 全局设置） |
| `IsScrollIntoView` | `bool?` | `null`（跟随 Tour） | 是否滚动到视图（覆盖 Tour 全局设置） |

> **注意**：`TourStep` 的可空属性（`bool?`、`TourPlacementMode?` 等）在为 `null` 时自动继承 `Tour` 上的全局设置，非 null 时覆盖全局设置。

---

## TourIndicator 公共属性（TourIndicator 抽象基类）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `StepCount` | `int` | `0` | 步骤总数（由 Tour 自动绑定） |
| `ActiveIndex` | `int` | `0` | 当前活跃步骤索引（由 Tour 自动绑定） |
| `StyleType` | `TourStyleType` | `TourStyleType.Default` | 视觉风格（由 Tour 自动绑定） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用动画（由 Tour 自动绑定） |

### DefaultTourIndicator 额外属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IndicatorSize` | `double` | 由 Token 控制（6） | 圆点大小 |
| `IndicatorColor` | `IBrush?` | `SharedToken.ColorFill` | 非活跃圆点颜色 |
| `IndicatorActiveColor` | `IBrush?` | `SharedToken.ColorPrimary` | 活跃圆点颜色 |
| `ItemSpacing` | `double` | `SharedToken.SpacingXXS` | 圆点之间的间距 |

### TextTourIndicator

`TextTourIndicator` 无额外公共属性。内部通过 `IndicatorText`（`DirectProperty`）自动生成 `"当前索引 / 总数"` 格式文本。

---

## 接口

### ITourStepOption

步骤选项数据接口，用于 `StepsSource` 数据绑定场景。同时也被 `TourStep` 控件实现。

```csharp
public interface ITourStepOption
{
    Control? Target { get; set; }
    bool? IsShowArrow { get; set; }
    bool? IsPointAtCenter { get; set; }
    PathIcon? CloseIcon { get; set; }
    object? Cover { get; set; }
    object? Title { get; set; }
    IDataTemplate? TitleTemplate { get; set; }
    object? Description { get; set; }
    IDataTemplate? DescriptionTemplate { get; set; }
    TourPlacementMode? Placement { get; set; }
    TourStyleType? StyleType { get; set; }
    bool? IsShowMask { get; set; }
    IBrush? MaskColor { get; set; }
    bool? IsScrollIntoView { get; set; }
}
```

### TourStepOption（POCO 实现）

`TourStepOption` 是 `ITourStepOption` 的简单 POCO 实现类，适用于在 ViewModel 中构造步骤数据。

### ITourAction

自定义操作按钮接口，添加到 `Tour.CustomActions` 中的控件如果实现此接口，将自动接收步骤状态同步。

```csharp
public interface ITourAction
{
    int StepCount { get; set; }
    int ActiveIndex { get; set; }
    TourStyleType StyleType { get; set; }
    void NotifyAttached(Tour tour) {}
}
```

---

## 公共方法

| 方法名 | 返回值 | 说明 |
|---|---|---|
| `ShowTour()` | `void` | 显示 Tour 引导（等效于设置 `IsOpen = true`） |
| `HideTour()` | `void` | 隐藏 Tour 引导（等效于设置 `IsOpen = false`） |
| `ResetState()` | `void` | 重置 Tour 状态：关闭 Tour 并将 `CurrentIndex` 重置为 0 |

---

## 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制 Tour 相关的过渡动画是否启用。
