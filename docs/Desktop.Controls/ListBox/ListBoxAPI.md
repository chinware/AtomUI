# ListBox API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### TextBlockHighlightStrategy（Flags 枚举）

过滤高亮策略，可组合使用。

| 值 | 十六进制 | 说明 |
|---|---|---|
| `HighlightedMatch` | `0x01` | 高亮匹配的文本片段 |
| `HighlightedWhole` | `0x02` | 高亮整个文本 |
| `BoldedMatch` | `0x04` | 加粗匹配的文本片段 |
| `HideUnMatched` | `0x08` | 隐藏未匹配的列表项 |
| `All` | `0x0D` | `HighlightedMatch \| BoldedMatch \| HideUnMatched` |

### SizeType（来自 `AtomUI.Controls`）

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## ListBox

`AtomUI.Desktop.Controls.ListBox` 继承自 `Avalonia.Controls.ListBox`，通过 `using AvaloniaListBox = Avalonia.Controls.ListBox;` 别名引用。

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsSelectable` | `bool` | `true` | 是否允许选择项目（`false` 时禁用选择，纯展示模式） |
| `SizeType` | `SizeType` | `Middle` | 列表尺寸（共享属性，通过 `AddOwner` 注册） |
| `IsBorderless` | `bool` | `false` | 是否无边框模式（嵌入其他控件时使用） |
| `ItemHoverBg` | `IBrush?` | 由 Token 控制 | 列表项悬浮态背景色 |
| `ItemSelectedBg` | `IBrush?` | 由 Token 控制 | 列表项选中态背景色 |
| `IsShowSelectedIndicator` | `bool` | `false` | 是否显示选中指示器（勾选图标） |
| `SelectedIndicator` | `IconTemplate?` | `CheckOutlined` | 选中指示器图标模板 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `EmptyIndicatorPadding` | `Thickness` | 由 Token 控制 | 空状态指示器内边距 |
| `EmptyIndicator` | `object?` | `null` | 空状态指示器内容 |
| `EmptyIndicatorTemplate` | `IDataTemplate?` | `Empty(Simple)` | 空状态指示器数据模板（默认为 Simple Empty 控件） |
| `IsShowEmptyIndicator` | `bool` | `true` | 是否在列表为空时显示空状态指示器 |
| `ItemFilter` | `IListBoxItemFilter?` | `DefaultListBoxItemFilter` | 项目过滤器（默认为字符串包含匹配） |
| `ItemFilterValue` | `object?` | `null` | 过滤关键字（设置后触发过滤） |
| `ItemFilterHighlightStrategy` | `TextBlockHighlightStrategy` | `All` | 过滤高亮策略 |
| `FilterHighlightForeground` | `IBrush?` | 由 Token 控制 | 过滤高亮文本前景色 |

### 只读属性（DirectProperty）

| 属性名 | 类型 | 说明 |
|---|---|---|
| `FilterResultCount` | `int` | 过滤匹配结果数量（只读） |
| `IsFiltering` | `bool` | 是否正在过滤（当 `ItemFilter` 和 `ItemFilterValue` 都非空时为 `true`，只读） |

### 继承自 Avalonia ListBox 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Items` | `ItemCollection` | 空集合 | 项目集合（直接添加） |
| `ItemsSource` | `IEnumerable?` | `null` | 数据源绑定 |
| `ItemTemplate` | `IDataTemplate?` | 自定义（见主题） | 项目数据模板 |
| `ItemsPanel` | `ITemplate<Panel>?` | `StackPanel` | 项目面板模板 |
| `SelectedItem` | `object?` | `null` | 当前选中的项目 |
| `SelectedIndex` | `int` | `-1` | 当前选中的索引 |
| `SelectedItems` | `IList?` | `null` | 多选时的选中项集合 |
| `SelectionMode` | `SelectionMode` | `Single` | 选择模式（Single / Multiple / Toggle / AlwaysSelected） |
| `ItemCount` | `int` | `0` | 项目总数（只读） |
| `WrapSelection` | `bool` | `false` | 键盘导航到末尾时是否循环 |
| `Background` | `IBrush?` | — | 背景色 |
| `BorderBrush` | `IBrush?` | 由 Token 控制 | 边框颜色 |
| `BorderThickness` | `Thickness` | 由 Token 控制 | 边框粗细 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 | 圆角半径 |
| `Padding` | `Thickness` | 由 Token 控制 | 内边距 |

### 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `ItemClicked` | `EventHandler<ListBoxItemClickedEventArgs>` | 列表项被点击时触发（CLR 事件） |
| `ItemCountChanged` | `EventHandler<ItemCountChangedEventArgs>` | 列表项目数量变化时触发（CLR 事件） |
| `SelectionChanged` | `EventHandler<SelectionChangedEventArgs>` | 选择变化时触发（继承自 Avalonia） |

### 实现的接口

| 接口 | 说明 |
|---|---|
| `ISizeTypeAware` | 支持三种尺寸切换 |
| `IMotionAwareControl` | 支持动画开关 |
| `IListVirtualizingContextAware` | 虚拟化上下文管理 |

---

## ListBoxItem

`AtomUI.Desktop.Controls.ListBoxItem` 继承自 `Avalonia.Controls.ListBoxItem`，是 ListBox 中每个选项的容器控件。

> ⚠️ ListBoxItem 的大部分扩展属性为 `internal`，由 ListBox 在 `PrepareContainerForItemOverride` 中自动传递。通常不需要手动设置。

### 公共事件

| 事件名 | 类型 | 路由策略 | 说明 |
|---|---|---|---|
| `Clicked` | `EventHandler<RoutedEventArgs>` | `Bubble` | 列表项左键点击时触发 |

### 继承自 Avalonia ListBoxItem 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 列表项内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容数据模板 |
| `IsSelected` | `bool` | `false` | 是否选中 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Foreground` | `IBrush?` | 由 Token 控制 | 文本前景色 |
| `Background` | `IBrush?` | 由 Token 控制 | 背景色 |
| `Padding` | `Thickness` | 由 Token 控制 | 内边距 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 | 圆角半径 |

### 伪类（Pseudo-Classes）

| 伪类 | 触发条件 |
|---|---|
| `:selected` | `IsSelected == true` |
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |

---

## 辅助类型

### ListBoxItemClickedEventArgs

```csharp
public class ListBoxItemClickedEventArgs : EventArgs
{
    public ListBoxItem Item { get; }
}
```

### IListBoxItemFilter

过滤器接口，允许自定义过滤逻辑。

```csharp
public interface IListBoxItemFilter
{
    bool Filter(ListBox listBox, ListBoxItem listBoxItem, object? filterValue);
}
```

### DefaultListBoxItemFilter

默认过滤器实现，基于字符串包含匹配。

```csharp
public class DefaultListBoxItemFilter : IListBoxItemFilter
{
    public DefaultListBoxItemFilter(ValueFilterMode filterMode = ValueFilterMode.Contains);
    public bool Filter(ListBox listBox, ListBoxItem listBoxItem, object? filterValue);
}
```

### IListItemData（来自 `AtomUI.Controls.Shared`）

列表数据项的标准接口。当 `ItemsSource` 中的数据实现此接口时，ListBox 能自动提取内容文本用于过滤和高亮。

### ItemCountChangedEventArgs（来自 `AtomUI.Controls.Shared`）

```csharp
public class ItemCountChangedEventArgs : EventArgs
{
    public int ItemCount { get; }
}
```
