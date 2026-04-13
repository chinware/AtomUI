# ListView API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### ListPaginationVisibility

分页器可见性枚举。

| 值 | 说明 |
|---|---|
| `None` | 不显示分页器 |
| `Top` | 仅显示顶部分页器 |
| `Bottom` | 仅显示底部分页器（默认） |
| `Both` | 同时显示顶部和底部分页器 |

### PaginationAlign（来自 `AtomUI.Desktop.Controls`）

分页器对齐方式。

| 值 | 说明 |
|---|---|
| `Start` | 左对齐 |
| `Center` | 居中对齐 |
| `End` | 右对齐（默认） |

### SelectionMode（来自 Avalonia）

选择模式，Flags 枚举，可组合使用。

| 值 | 说明 |
|---|---|
| `Single` | 单选（默认） |
| `Multiple` | 多选（配合 Ctrl/Shift 键） |
| `Toggle` | 切换选择（点击已选中项取消选中） |
| `AlwaysSelected` | 始终有选中项（不允许取消所有选择） |

### SizeType（来自 `AtomUI.Controls`）

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## ListView

`AtomUI.Desktop.Controls.ListView` 继承自 `Avalonia.Controls.ItemsControl`，自行实现了完整的选择系统。

### 公共属性（StyledProperty）

#### 基础属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsSelectable` | `bool` | `true` | 是否允许选择项目（`false` 时禁用选择，纯展示模式） |
| `SizeType` | `SizeType` | `Middle` | 列表尺寸（共享属性，通过 `AddOwner` 注册） |
| `IsBorderless` | `bool` | `false` | 是否无边框模式（嵌入其他控件时使用） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `ItemClickMode` | `ClickMode` | `Release` | 列表项点击触发时机 |

#### 选择属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SelectionMode` | `SelectionMode` | `Single` | 选择模式（Single / Multiple / Toggle / AlwaysSelected） |
| `AutoScrollToSelectedItem` | `bool` | `true` | 选中项变化时是否自动滚动到可视区域 |
| `IsTextSearchEnabled` | `bool` | `false` | 是否启用键入文本快速搜索 |
| `WrapSelection` | `bool` | `false` | 键盘导航到末尾时是否循环到开头 |
| `SelectedValueBinding` | `IBinding?` | `null` | 用于从选中项中提取 `SelectedValue` 的绑定 |
| `SelectedValue` | `object?` | `null` | 通过 `SelectedValueBinding` 从选中项提取的值（双向绑定） |

#### 视觉定制属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ItemHoverBg` | `IBrush?` | 由 Token 控制 | 列表项悬浮态背景色 |
| `ItemSelectedBg` | `IBrush?` | 由 Token 控制 | 列表项选中态背景色 |
| `IsShowSelectedIndicator` | `bool` | `false` | 是否显示选中指示器（勾选图标） |
| `SelectedIndicator` | `IconTemplate?` | `CheckOutlined` | 选中指示器图标模板 |

#### 空状态属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsShowEmptyIndicator` | `bool` | `true` | 是否在列表为空时显示空状态指示器 |
| `EmptyIndicator` | `object?` | `null` | 空状态指示器内容 |
| `EmptyIndicatorTemplate` | `IDataTemplate?` | `Empty(Simple)` | 空状态指示器数据模板（默认为 Simple Empty 控件） |
| `EmptyIndicatorPadding` | `Thickness` | 由 Token 控制 | 空状态指示器内边距 |

#### 分组属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsGroupEnabled` | `bool` | `false` | 是否启用分组 |
| `GroupPropertySelector` | `DefaultFilterValueSelector?` | 默认使用 `IGroupHeader.Group` | 分组键提取委托 |
| `GroupItemTemplate` | `IDataTemplate?` | `null` | 分组标题项的数据模板 |

#### 排序与过滤属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SortDescriptions` | `IList<IListSortDescription>?` | `null` | 排序描述集合 |
| `Filter` | `IValueFilter?` | `null` | 过滤器实例 |
| `FilterValue` | `object?` | `null` | 过滤关键字 |
| `FilterValueSelector` | `DefaultFilterValueSelector?` | 默认提取 `IListItemData.Content` | 过滤值提取委托 |

#### 分页属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `TopPagination` | `AbstractPagination?` | `null` | 顶部分页器控件 |
| `BottomPagination` | `AbstractPagination?` | `null` | 底部分页器控件 |
| `PageIndex` | `int` | `1` | 当前页索引 |
| `PageSize` | `int` | `0` | 每页数据量（`0` 表示不分页） |
| `PaginationVisibility` | `ListPaginationVisibility` | `Bottom` | 分页器显示位置 |
| `TopPaginationAlign` | `PaginationAlign` | `End` | 顶部分页器对齐方式 |
| `BottomPaginationAlign` | `PaginationAlign` | `End` | 底部分页器对齐方式 |
| `IsHideOnSinglePage` | `bool` | `false` | 仅一页时是否隐藏分页器 |

#### 加载状态属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsOperating` | `bool` | `false` | 是否正在操作中（显示加载遮罩） |
| `OperatingMsg` | `string?` | `null` | 操作中提示文本 |
| `CustomOperatingIndicator` | `object?` | `null` | 自定义操作指示器内容 |
| `CustomOperatingIndicatorTemplate` | `IDataTemplate?` | `null` | 自定义操作指示器数据模板 |

### 只读属性（DirectProperty）

| 属性名 | 类型 | 说明 |
|---|---|---|
| `SelectedIndex` | `int` | 当前选中的索引（双向绑定） |
| `SelectedItem` | `object?` | 当前选中的项目（双向绑定） |
| `SelectedItems` | `IList?` | 多选时的选中项集合 |
| `Selection` | `ISelectionModel` | 选择模型实例（可替换为自定义实现） |
| `IsFiltering` | `bool` | 是否正在过滤（当 `Filter` 和 `FilterValue` 都非空时为 `true`） |
| `TotalItemCount` | `int` | 数据项总数（包含所有页面） |

### 继承自 Avalonia ItemsControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Items` | `ItemCollection` | 空集合 | 项目集合（直接添加） |
| `ItemsSource` | `IEnumerable?` | `null` | 数据源绑定（自动包装为 `IListCollectionView`） |
| `ItemTemplate` | `IDataTemplate?` | 自定义（见主题） | 项目数据模板 |
| `ItemsPanel` | `ITemplate<Panel>?` | `VirtualizingStackPanel` | 项目面板模板 |
| `ItemCount` | `int` | `0` | 当前页的项目数量（只读） |
| `DisplayMemberBinding` | `IBinding?` | `null` | 显示成员绑定 |
| `Background` | `IBrush?` | — | 背景色 |
| `BorderBrush` | `IBrush?` | 由 Token 控制 | 边框颜色 |
| `BorderThickness` | `Thickness` | 由 Token 控制 | 边框粗细 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 | 圆角半径 |
| `Padding` | `Thickness` | 由 Token 控制 | 内边距 |

### 附加属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `IsSelected` | `bool`（附加属性） | 用于标记容器控件是否被选中（`GetIsSelected` / `SetIsSelected`） |

### 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `ItemClicked` | `EventHandler<ListViewItemClickedEventArgs>` | 列表项被点击时触发（CLR 事件） |
| `ItemCountChanged` | `EventHandler<ItemCountChangedEventArgs>` | 列表项目数量变化时触发（CLR 事件） |
| `FilterContextChanged` | `EventHandler?` | 过滤上下文变化时触发（CLR 事件） |
| `SelectionChanged` | `EventHandler<SelectionChangedEventArgs>` | 选择变化时触发（路由事件，冒泡） |

### 实现的接口

| 接口 | 说明 |
|---|---|
| `ISizeTypeAware` | 支持三种尺寸切换 |
| `IMotionAwareControl` | 支持动画开关 |
| `IListVirtualizingContextAware` | 虚拟化上下文管理 |

---

## ListViewItem

`AtomUI.Desktop.Controls.ListViewItem` 继承自 `Avalonia.Controls.ContentControl`，是 ListView 中每个选项的容器控件。

> ⚠️ ListViewItem 的大部分扩展属性为 `internal`，由 ListView 在 `PrepareContainerForItemOverride` 中自动传递。通常不需要手动设置。

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsSelected` | `bool` | `false` | 是否选中（通过 `AddOwner` 从 `ListView.IsSelectedProperty` 注册） |

### 公共事件

| 事件名 | 类型 | 路由策略 | 说明 |
|---|---|---|---|
| `Clicked` | `EventHandler<RoutedEventArgs>` | `Bubble` | 列表项点击时触发 |

### 继承自 Avalonia ContentControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 列表项内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容数据模板 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Foreground` | `IBrush?` | 由 Token 控制 | 文本前景色 |
| `Background` | `IBrush?` | 由 Token 控制 | 背景色 |
| `Padding` | `Thickness` | 由 Token 控制 | 内边距 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 | 圆角半径 |

### 实现的接口

| 接口 | 说明 |
|---|---|
| `IListItemVirtualizingContextAware` | 虚拟化上下文感知（保存/恢复虚拟化索引） |
| `ISelectable` | 可选择标记接口 |

### 伪类（Pseudo-Classes）

| 伪类 | 触发条件 |
|---|---|
| `:selected` | `IsSelected == true` |
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 鼠标按下 |
| `:disabled` | `IsEnabled == false` |

---

## ListView 伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:empty` | `ListPseudoClass.Empty` | `TotalItemCount == 0` |
| `:singleitem` | `ListPseudoClass.SingleItem` | `TotalItemCount == 1` |

---

## 辅助类型

### ListViewItemClickedEventArgs

```csharp
public class ListViewItemClickedEventArgs : EventArgs
{
    public ListViewItem Item { get; }
}
```

### IValueFilter（来自 `AtomUI.Controls.Shared`）

过滤器接口，允许自定义过滤逻辑。

```csharp
public interface IValueFilter
{
    ValueFilterMode Mode => ValueFilterMode.Custom;
    bool Filter(object? value, object? filterValue);
}
```

### DefaultFilterValueSelector（来自 `AtomUI.Controls.Shared`）

从数据记录中提取过滤/分组属性值的委托类型。

```csharp
public delegate object? DefaultFilterValueSelector(object record);
```

### IListItemData（来自 `AtomUI.Controls.Shared`）

列表数据项的标准接口。当 `ItemsSource` 中的数据实现此接口时，ListView 能自动提取内容文本用于展示。

### ItemCountChangedEventArgs（来自 `AtomUI.Controls.Shared`）

```csharp
public class ItemCountChangedEventArgs : EventArgs
{
    public int ItemCount { get; }
}
```

### IGroupHeader

分组标题接口。数据项实现此接口后，默认 `GroupPropertySelector` 会使用其 `Group` 属性作为分组键。

### ListSortDescription

排序描述，用于配置 `SortDescriptions` 属性。可通过 `ListSortDescription.FromPath("PropertyName")` 快速创建。
