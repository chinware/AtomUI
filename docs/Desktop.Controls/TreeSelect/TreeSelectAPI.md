# TreeSelect API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### TreeSelectCheckedStrategy

勾选策略枚举（`[Flags]`），控制勾选模式下选中项的展示策略。

| 值 | 说明 |
|---|---|
| `ShowParent` (`0x1`) | 当所有子节点都被勾选时，仅展示父节点 |
| `ShowChild` (`0x2`) | 仅展示被勾选的叶子节点 |
| `All` (`ShowParent \| ShowChild`) | 展示所有被勾选的节点（默认值） |

### InputControlStyleVariant（来自 `AtomUI.Controls`）

输入控件样式变体枚举。

| 值 | 说明 |
|---|---|
| `Outline` | 带边框样式（默认） |
| `Filled` | 填充背景样式 |
| `Borderless` | 无边框样式 |
| `Underlined` | 下划线样式 |

### InputControlStatus（来自 `AtomUI.Controls`）

输入控件状态枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认状态 |
| `Error` | 错误状态（红色边框） |
| `Warning` | 警告状态（橙色边框） |

### SelectPopupPlacement

弹窗位置枚举。

| 值 | 说明 |
|---|---|
| `BottomEdgeAlignedLeft` | 底部左对齐（默认） |
| `BottomEdgeAlignedRight` | 底部右对齐 |
| `TopEdgeAlignedLeft` | 顶部左对齐 |
| `TopEdgeAlignedRight` | 顶部右对齐 |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

### TreeFilterHighlightStrategy（来自 `AtomUI.Controls`）

树筛选高亮策略枚举（`[Flags]`）。

| 值 | 说明 |
|---|---|
| `HighlightedWhole` | 高亮匹配项整行 |
| `BoldedMatch` | 加粗匹配文字 |
| `ExpandPath` | 展开匹配项的路径 |
| `HideUnMatched` | 隐藏不匹配的节点 |

---

## 公共属性（TreeSelect 自有）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ShowCheckedStrategy` | `TreeSelectCheckedStrategy` | `All` | 勾选模式下，控制选中项在选择框中的展示策略 |
| `AutoScrollToSelectedItem` | `bool` | `true` | 打开下拉时是否自动滚动到选中项 |
| `IsShowIcon` | `bool` | `false` | 是否显示节点图标 |
| `IsShowLeafIcon` | `bool` | `false` | 是否显示叶子节点图标 |
| `IsShowLine` | `bool` | `false` | 是否在 TreeView 中显示连接线 |
| `IsTreeCheckable` | `bool` | `false` | 是否启用 Checkbox 勾选模式（设为 `true` 会自动将 `IsMultiple` 设为 `true`） |
| `IsMultiple` | `bool` | `false` | 是否启用多选模式 |
| `IsDefaultExpandAll` | `bool` | `false` | 是否默认展开所有节点 |
| `IsShowTreeLine` | `bool` | `false` | 是否显示树形连接线 |
| `IsSwitcherRotation` | `bool` | `false` | 展开/折叠按钮是否使用旋转动画（而非切换图标） |
| `IsTreeCheckStrictly` | `bool` | `false` | 严格勾选模式，父子节点勾选互不关联 |
| `TreeDefaultExpandedPaths` | `IList<TreeNodePath>?` | `null` | 默认展开的节点路径列表 |
| `ItemsSource` | `IEnumerable<ITreeItemNode>?` | `null` | 树形数据源 |
| `ItemTemplate` | `IDataTemplate?` | 内置 `TreeDataTemplate` | 自定义节点渲染模板 |
| `DataLoader` | `ITreeItemNodeLoader?` | `null` | 异步加载器，用于按需加载子节点 |
| `Filter` | `ITreeItemFilter?` | `DefaultTreeItemFilter` | 搜索筛选器 |
| `FilterHighlightStrategy` | `TreeFilterHighlightStrategy` | `HighlightedWhole \| BoldedMatch \| ExpandPath \| HideUnMatched` | 筛选高亮策略 |
| `FilterHighlightForeground` | `IBrush?` | `ColorPrimary` | 筛选匹配文字高亮颜色 |
| `SelectedItem` | `ITreeItemNode?` | `null` | 当前选中项（单选模式） |
| `SelectedItems` | `IList<ITreeItemNode>?` | `null` | 当前选中项列表（多选/勾选模式） |
| `Items` | `ItemCollection` | 空集合 | 直接操作的树节点集合（`[Content]` 属性） |

---

## 继承自 AbstractSelect 的公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsAllowClear` | `bool` | `false` | 是否显示清除按钮 |
| `IsAutoClearSearchValue` | `bool` | `false` | 选中后是否自动清空搜索框 |
| `IsDefaultOpen` | `bool` | `false` | 是否默认展开下拉弹窗 |
| `IsDropDownOpen` | `bool` | `false` | 下拉弹窗是否打开 |
| `PlaceholderText` | `string?` | `null` | 未选择时的占位提示文本 |
| `PlaceholderForeground` | `IBrush?` | `ColorTextPlaceholder` | 占位文本颜色 |
| `IsPopupMatchSelectWidth` | `bool` | `true` | 弹窗宽度是否匹配选择框宽度 |
| `IsFilterEnabled` | `bool` | `false` | 是否启用搜索筛选 |
| `DisplayPageSize` | `int` | `10` | 弹窗中可见的选项数量（影响弹窗最大高度） |
| `MaxCount` | `int` | `int.MaxValue` | 最大可选数量（多选模式） |
| `IsShowMaxCountIndicator` | `bool` | `false` | 是否显示已选/最大数量指示器 |
| `MaxTagCount` | `int?` | `null` | 多选模式下最大显示的 Tag 数量 |
| `IsResponsiveTagMode` | `bool` | `false` | 是否启用响应式 Tag 模式 |
| `MaxTagPlaceholder` | `string?` | `null` | 响应式模式下 Tag 被隐藏后显示的替代文本 |
| `LeftAddOn` | `object?` | `null` | 左侧附加内容（AddOn 区域） |
| `LeftAddOnTemplate` | `IDataTemplate?` | `null` | 左侧附加内容模板 |
| `RightAddOn` | `object?` | `null` | 右侧附加内容 |
| `RightAddOnTemplate` | `IDataTemplate?` | `null` | 右侧附加内容模板 |
| `ContentLeftAddOn` | `object?` | `null` | 内容区域左侧附加内容（前缀） |
| `ContentLeftAddOnTemplate` | `IDataTemplate?` | `null` | 内容区域左侧附加内容模板 |
| `ContentRightAddOn` | `object?` | `null` | 内容区域右侧附加内容 |
| `ContentRightAddOnTemplate` | `IDataTemplate?` | `null` | 内容区域右侧附加内容模板 |
| `StyleVariant` | `InputControlStyleVariant` | `Outline` | 样式变体（Outline / Filled / Borderless / Underlined） |
| `Status` | `InputControlStatus` | `Default` | 验证状态（Default / Error / Warning） |
| `Placement` | `SelectPopupPlacement` | `BottomEdgeAlignedLeft` | 弹窗弹出位置 |
| `FilterValue` | `object?` | `null` | 当前搜索筛选值 |
| `ClearIcon` | `PathIcon?` | `null` | 自定义清除按钮图标 |
| `SuffixIcon` | `PathIcon?` | `DownOutlined` | 下拉箭头图标 |
| `SuffixLoadingIcon` | `PathIcon?` | `LoadingOutlined（旋转）` | 加载状态图标 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用弹窗展开/收起动画 |
| `SizeType` | `SizeType` | `SizeType.Middle` | 控件尺寸 |
| `EmptyIndicator` | `object?` | `null` | 空状态指示器内容 |
| `EmptyIndicatorTemplate` | `IDataTemplate?` | `null` | 空状态指示器模板 |
| `IsShowEmptyIndicator` | `bool` | `true` | 是否显示空状态指示器 |
| `EmptyIndicatorPadding` | `Thickness` | `default` | 空状态指示器内边距 |
| `IsLoading` | `bool` | `false` | 是否处于加载状态（只读） |

---

## 公共方法

| 方法名 | 返回类型 | 说明 |
|---|---|---|
| `Clear()` | `void` | 清空所有选中项（同时清空 `SelectedItem` 和 `SelectedItems`） |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `DropDownOpening` | `EventHandler<CancelEventArgs>` | 下拉弹窗即将打开时触发，可通过 `Cancel = true` 阻止打开 |
| `DropDownOpened` | `EventHandler` | 下拉弹窗已打开后触发 |
| `DropDownClosing` | `EventHandler<CancelEventArgs>` | 下拉弹窗即将关闭时触发，可通过 `Cancel = true` 阻止关闭 |
| `DropDownClosed` | `EventHandler` | 下拉弹窗已关闭后触发 |

---

## 伪类（Pseudo-Classes）

TreeSelect 支持以下伪类，可在样式选择器中使用：

### Select 共享伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:dropdownopen` | `SelectPseudoClass.DropdownOpen` | 下拉弹窗处于打开状态 |

### 从 AddOnDecoratedBox 继承的样式变体伪类

| 伪类 | 触发条件 |
|---|---|
| `:outline` | `StyleVariant == Outline` |
| `:filled` | `StyleVariant == Filled` |
| `:borderless` | `StyleVariant == Borderless` |

### 标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pressed` | 控件被按下（鼠标按住未释放） |
| `:disabled` | `IsEnabled == false` |
| `:focus-within` | 控件或其子控件获得焦点 |
| `:error` | `Status == Error` |
| `:warning` | `Status == Warning` |

---

## 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制弹窗展开/收起动画是否启用。

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

支持 `Small` / `Middle` / `Large` 三种尺寸。

### IInputControlStatusAware

```csharp
public InputControlStatus Status { get; set; }
```

支持 `Default` / `Error` / `Warning` 三种验证状态。

### IInputControlStyleVariantAware

```csharp
public InputControlStyleVariant StyleVariant { get; set; }
```

支持 `Outline` / `Filled` / `Borderless` / `Underlined` 四种样式变体。

### ICompactSpaceAware

在 `Space.Compact` 容器中使用时自动调整圆角和边框。

### IFormItemAware

可作为 `FormItem` 的子控件参与表单验证流程。TreeSelect 实现了 `SetFormValue` / `GetFormValue` / `ClearFormValue`：
- 单选模式：表单值为 `ITreeItemNode?`
- 多选模式：表单值为 `IList<ITreeItemNode>?`

### IFormItemFeedbackAware

支持表单验证反馈控件展示（如错误/警告图标）。
