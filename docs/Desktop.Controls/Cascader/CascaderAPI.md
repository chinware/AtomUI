# Cascader API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### CascaderViewExpandTrigger

级联菜单展开触发方式枚举。

| 值 | 说明 |
|---|---|
| `Click` | 点击选项展开子级（默认） |
| `Hover` | 鼠标悬浮在选项上时自动展开子级 |

### TreeSelectCheckedStrategy

多选模式下的选中策略枚举（来自 `AtomUI.Controls`）。

| 值 | 说明 |
|---|---|
| `All` | 显示所有选中的节点（默认） |
| `ShowParent` | 当所有子节点都被选中时，只显示父节点 |
| `ShowChild` | 只显示叶子节点 |

### TextBlockHighlightStrategy

搜索高亮策略枚举（来自 `AtomUI.Controls`）。

| 值 | 说明 |
|---|---|
| `All` | 高亮所有匹配文本（默认） |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

### InputControlStatus（来自 `AtomUI.Controls`）

输入控件状态枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认状态 |
| `Error` | 错误状态（红色边框） |
| `Warning` | 警告状态（橙色边框） |

### InputControlStyleVariant（来自 `AtomUI.Controls`）

输入控件样式变体枚举。

| 值 | 说明 |
|---|---|
| `Outline` | 有边框（默认） |
| `Filled` | 填充背景 |
| `Borderless` | 无边框 |
| `Underlined` | 下划线 |

---

## Cascader 公共属性（StyledProperty）

### Cascader 自身属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsMultiple` | `bool` | `false` | 是否启用多选模式 |
| `ShowCheckedStrategy` | `TreeSelectCheckedStrategy` | `All` | 多选模式下的标签展示策略 |
| `OptionsSource` | `IEnumerable<ICascaderOption>?` | `null` | 选项数据源（绑定用） |
| `OptionTemplate` | `IDataTemplate?` | 内置 `TreeDataTemplate` | 选项渲染模板 |
| `SelectedOption` | `ICascaderOption?` | `null` | 当前选中的单选选项（DirectProperty） |
| `SelectedOptions` | `IList<ICascaderOption>?` | `null` | 当前选中的多选选项列表（DirectProperty） |
| `ExpandIcon` | `IconTemplate?` | `RightOutlined` | 选项展开箭头图标 |
| `LoadingIcon` | `IconTemplate?` | `LoadingOutlined (Spin)` | 异步加载时的旋转图标 |
| `ExpandTrigger` | `CascaderViewExpandTrigger` | `Click` | 子级菜单展开触发方式 |
| `DataLoader` | `ICascaderItemDataLoader?` | `null` | 异步数据加载器 |
| `Filter` | `ICascaderItemFilter?` | `DefaultCascaderItemFilter` | 自定义过滤器 |
| `FilterHighlightStrategy` | `TextBlockHighlightStrategy` | `All` | 搜索关键词高亮策略 |
| `FilterHighlightForeground` | `IBrush?` | `CascaderToken.FilterHighlightColor` | 搜索关键词高亮前景色 |
| `DefaultSelectOptionPath` | `TreeNodePath?` | `null` | 默认选中路径（通过 `ItemKey` 或 `Value` 匹配） |
| `IsAllowSelectParent` | `bool` | `false` | 是否允许选择非叶子（父）节点 |
| `Options` | `ItemCollection` | 空集合 | 直接在 AXAML 中声明的选项集合（`[Content]` 属性） |

### 继承自 AbstractSelect 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsAllowClear` | `bool` | `false` | 是否显示清除按钮 |
| `IsDropDownOpen` | `bool` | `false` | 下拉面板是否打开 |
| `PlaceholderText` | `string?` | `null` | 占位文本 |
| `PlaceholderForeground` | `IBrush?` | `ColorTextPlaceholder` | 占位文本颜色 |
| `IsFilterEnabled` | `bool` | `false` | 是否启用搜索过滤 |
| `IsPopupMatchSelectWidth` | `bool` | `true` | 弹出面板是否匹配选择框宽度 |
| `MaxCount` | `int` | `int.MaxValue` | 多选模式下最大选择数量 |
| `IsShowMaxCountIndicator` | `bool` | `false` | 是否显示最大数量指示器 |
| `MaxTagCount` | `int?` | `null` | 多选标签最大显示数量 |
| `IsResponsiveTagMode` | `bool` | `false` | 是否启用响应式标签模式 |
| `SizeType` | `SizeType` | `Middle` | 控件尺寸 |
| `StyleVariant` | `InputControlStyleVariant` | `Outline` | 样式变体 |
| `Status` | `InputControlStatus` | `Default` | 输入控件状态 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `LeftAddOn` | `object?` | `null` | 左侧附加内容 |
| `RightAddOn` | `object?` | `null` | 右侧附加内容 |
| `ContentLeftAddOn` | `object?` | `null` | 内容区左侧附加内容（前缀图标） |
| `ContentRightAddOn` | `object?` | `null` | 内容区右侧附加内容 |
| `SuffixIcon` | `PathIcon?` | 内置下拉箭头 | 后缀图标 |
| `PopupPlacement` | `SelectPopupPlacement` | `BottomEdgeAlignedLeft` | 弹出面板位置 |
| `MaxPopupHeight` | `double` | `CascaderToken.DropdownHeight` | 弹出面板最大高度 |
| `IsEnabled` | `bool` | `true` | 是否启用 |

---

## 公共方法

| 方法名 | 返回类型 | 说明 |
|---|---|---|
| `Clear()` | `void` | 清除所有选中值（`SelectedOption`、`SelectedOptions`、`SelectedOptionPath`） |

---

## 事件

Cascader 本身不直接公开自定义事件，但其内部 `CascaderView` 提供以下事件：

| 事件名 | 事件参数类型 | 说明 |
|---|---|---|
| `SelectedOptionsChanged` | `CascaderOptionsSelectedChangedEventArgs` | 多选选中项发生变化 |
| `ItemClicked` | `CascaderItemClickedEventArgs` | 选项项被点击 |
| `ItemDoubleClicked` | `CascaderItemDoubleClickedEventArgs` | 选项项被双击 |
| `OptionSelected` | `CascaderOptionSelectedEventArgs` | 选项被选中 |
| `ItemExpanded` | `CascaderItemExpandedEventArgs` | 选项项被展开 |
| `ItemCollapsed` | `CascaderItemCollapsedEventArgs` | 选项项被折叠 |
| `ItemAsyncLoaded` | `CascaderViewItemLoadedEventArgs` | 异步加载完成 |

通常通过属性变化回调（`SelectedOptionProperty.Changed`、`SelectedOptionsProperty.Changed`）监听选中值的变化。

---

## 伪类（Pseudo-Classes）

### CascaderViewItem 伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:checked` | `CascaderViewPseudoClass.NodeToggleTypeCheckBox` | 多选模式下，选项的复选框类型激活 |
| `:pressed` | `StdPseudoClass.Pressed` | 选项项被按下 |
| `:expanded` | `StdPseudoClass.Expanded` | 选项项已展开（有子级可见） |
| `:selected` | `StdPseudoClass.Selected` | 选项项被选中 |

### Cascader 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 控件被按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:dropdown-open` | 下拉面板打开 |

---

## 接口定义

### ICascaderOption

级联选项数据接口，继承自 `ITreeNode<ICascaderOption>`。

```csharp
public interface ICascaderOption : ITreeNode<ICascaderOption>
{
    object? Header { get; set; }       // 显示文本
    bool? IsChecked { get; set; }      // 复选状态（多选模式）
    bool IsCheckBoxEnabled { get; set; } // 复选框是否可用
    bool IsExpanded { get; set; }      // 是否展开
    bool IsLeaf { get; set; }         // 是否为叶子节点
    object? Value { get; set; }        // 选项值
}
```

### CascaderOption

`ICascaderOption` 的默认实现（`record` 类），可直接在 AXAML 中使用。

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Header` | `object?` | `null` | 显示文本 |
| `Value` | `object?` | `null` | 选项值 |
| `ItemKey` | `EntityKey?` | `null` | 选项唯一标识（用于路径匹配） |
| `Icon` | `PathIcon?` | `null` | 选项图标 |
| `IsEnabled` | `bool` | `true` | 是否可用 |
| `IsChecked` | `bool?` | `false` | 复选状态 |
| `IsLeaf` | `bool` | `false` | 是否为叶子节点 |
| `IsExpanded` | `bool` | `false` | 是否展开 |
| `IsCheckBoxEnabled` | `bool` | `true` | 复选框是否可用 |
| `Children` | `IList<ICascaderOption>` | 空列表 | 子选项列表 |

### ICascaderItemFilter

自定义过滤接口。

```csharp
public interface ICascaderItemFilter
{
    bool Filter(CascaderView cascaderView, ICascaderItemInfo cascaderItemInfo, object? filterValue);
}
```

### ICascaderItemDataLoader

异步数据加载接口。

```csharp
public interface ICascaderItemDataLoader
{
    Task<CascaderItemLoadResult> LoadAsync(ICascaderOption targetNode, CancellationToken token);
}
```

---

## 实现的接口

### ISizeTypeAware（通过 AbstractSelect）

```csharp
public SizeType SizeType { get; set; }
```

### IMotionAwareControl（通过 AbstractSelect）

```csharp
public bool IsMotionEnabled { get; set; }
```

### ICompactSpaceAware（通过 AbstractSelect）

在 `Space.Compact` 容器中使用时自动调整圆角。

### IInputControlStatusAware（通过 AbstractSelect）

```csharp
public InputControlStatus Status { get; set; }
```

### IInputControlStyleVariantAware（通过 AbstractSelect）

```csharp
public InputControlStyleVariant StyleVariant { get; set; }
```

### IFormItemAware（通过 AbstractSelect）

可作为 `FormItem` 的子控件参与表单验证流程。支持通过 `NotifySetFormValue` / `NotifyGetFormValue` / `NotifyClearFormValue` 进行表单值的读写和清除。
