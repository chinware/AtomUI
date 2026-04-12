# Collapse API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### CollapseTriggerType

折叠触发方式枚举，定义用户通过哪种操作触发面板的折叠/展开。

| 值 | 说明 |
|---|---|
| `Header` | 点击整个头部区域触发折叠（默认） |
| `Icon` | 仅点击展开图标触发折叠 |

### CollapseExpandIconPosition

展开图标位置枚举。

| 值 | 说明 |
|---|---|
| `Start` | 展开图标在左侧（默认） |
| `End` | 展开图标在右侧 |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## Collapse 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `SizeType.Middle` | 面板尺寸（共享属性，通过 `AddOwner` 注册） |
| `IsGhostStyle` | `bool` | `false` | 幽灵模式，无边框无背景 |
| `IsBorderless` | `bool` | `false` | 无边框模式，去除外边框 |
| `IsAccordion` | `bool` | `false` | 手风琴模式，同时只展开一个面板 |
| `TriggerType` | `CollapseTriggerType` | `Header` | 折叠触发区域 |
| `ExpandIconPosition` | `CollapseExpandIconPosition` | `Start` | 展开图标位置 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用折叠/展开动画（共享属性） |
| `ItemHeaderPadding` | `Thickness?` | `null`（由 Token 控制） | 统一设置所有子项的头部内间距 |
| `ItemContentPadding` | `Thickness?` | `null`（由 Token 控制） | 统一设置所有子项的内容内间距 |

### 继承自 Avalonia SelectingItemsControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ItemsSource` | `IEnumerable?` | `null` | 数据源绑定 |
| `ItemTemplate` | `IDataTemplate?` | `null` | 子项内容模板 |
| `ItemsPanel` | `ITemplate<Panel?>` | 垂直 `StackPanel` | 子项布局面板 |
| `SelectedIndex` | `int` | `-1` | 当前选中项索引 |
| `SelectedItem` | `object?` | `null` | 当前选中项对象 |
| `SelectedItems` | `IList?` | — | 多选模式下的选中项集合 |
| `SelectionMode` | `SelectionMode` | `Multiple \| Toggle` | 选择模式（手风琴时自动切换为 `Single \| Toggle`） |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `BorderThickness` | `Thickness` | 由 Token 控制 | 边框粗细 |
| `BorderBrush` | `IBrush?` | 由主题控制 | 边框颜色 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 | 圆角半径 |

---

## CollapseItem 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsSelected` | `bool` | `false` | 是否展开（通过 `AddOwner` 注册自 `SelectingItemsControl.IsSelectedProperty`） |
| `IsShowExpandIcon` | `bool` | `true` | 是否显示展开图标 |
| `ExpandIcon` | `PathIcon?` | `RightOutlined` | 自定义展开图标 |
| `AddOnContent` | `object?` | `null` | 头部附加内容（如操作按钮、图标） |
| `AddOnContentTemplate` | `IDataTemplate?` | `null` | 附加内容模板 |
| `HeaderPadding` | `Thickness?` | `null`（由父级 Collapse 或 Token 控制） | 自定义头部内间距 |
| `ContentPadding` | `Thickness?` | `null`（由父级 Collapse 或 Token 控制） | 自定义内容内间距 |

### 继承自 Avalonia HeaderedContentControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Header` | `object?` | `null` | 面板标题 |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 标题模板 |
| `Content` | `object?` | `null` | 面板内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `IsEnabled` | `bool` | `true` | 是否启用（禁用时面板无法折叠/展开） |

---

## 数据接口

### ICollapseItemData

用于数据驱动模式（`ItemsSource` 绑定）时，自动映射数据属性到 CollapseItem。

```csharp
public interface ICollapseItemData
{
    bool IsSelected { get; }
    bool IsEnabled { get; }
    bool IsShowExpandIcon { get; }
    object? Header { get; }
}
```

| 属性名 | 类型 | 说明 |
|---|---|---|
| `IsSelected` | `bool` | 面板是否默认展开 |
| `IsEnabled` | `bool` | 面板是否可交互 |
| `IsShowExpandIcon` | `bool` | 是否显示展开图标 |
| `Header` | `object?` | 面板标题 |

### CollapseItemData（record）

`ICollapseItemData` 的默认实现：

```csharp
public record CollapseItemData : ICollapseItemData
{
    public bool IsSelected { get; init; } = false;
    public bool IsEnabled { get; init; } = true;
    public bool IsShowExpandIcon { get; init; } = true;
    public object? Header { get; init; }
}
```

---

## 事件

| 事件名 | 控件 | 类型 | 说明 |
|---|---|---|---|
| `SelectionChanged` | Collapse | `EventHandler<SelectionChangedEventArgs>` | 面板选中状态变化事件（继承自 `SelectingItemsControl`） |

---

## 伪类（Pseudo-Classes）

### CollapseItem 伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:selected` | `StdPseudoClass.Selected` | 面板展开（`IsSelected == true`） |
| `:pressed` | `StdPseudoClass.Pressed` | 面板被按下 |
| `:custom-header-padding` | `CollapseItemPseudoClass.CustomHeaderPadding` | `HeaderPadding != null`（自定义头部间距） |
| `:custom-content-padding` | `CollapseItemPseudoClass.CustomContentPadding` | `ContentPadding != null`（自定义内容间距） |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:pointerover` | 鼠标悬浮 |

> 注意：`:custom-header-padding` 和 `:custom-content-padding` 伪类用于样式系统内部判断——当设置了自定义间距时，Token 定义的默认间距不再生效。

---

## 实现的接口

### IMotionAwareControl（Collapse）

```csharp
public bool IsMotionEnabled { get; set; }
```

控制折叠/展开动画是否启用。设为 `false` 时，面板展开/折叠直接切换，无过渡动画。

### ISelectable（CollapseItem）

```csharp
public bool IsSelected { get; set; }
```

面板的展开/折叠状态。`true` = 展开，`false` = 折叠。由父级 `Collapse` 的选择机制管理，也可在 AXAML 中直接设置默认值。
