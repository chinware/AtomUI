# Select API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### SelectMode

选择模式枚举，定义 Select 的选择行为。

| 值 | 说明 |
|---|---|
| `Single` | 单选模式，选中后下拉自动关闭 |
| `Multiple` | 多选模式，已选项以标签形式展示 |
| `Tags` | 标签模式，在多选基础上允许自由输入创建新标签 |

### SelectPopupPlacement

下拉弹出面板的位置枚举。

| 值 | 说明 |
|---|---|
| `TopEdgeAlignedLeft` | 上方弹出，左对齐 |
| `TopEdgeAlignedRight` | 上方弹出，右对齐 |
| `BottomEdgeAlignedLeft` | 下方弹出，左对齐（默认） |
| `BottomEdgeAlignedRight` | 下方弹出，右对齐 |

### InputControlStyleVariant（来自 `AtomUI.Controls`）

输入控件样式变体枚举。

| 值 | 说明 |
|---|---|
| `Outline` | 有边框轮廓（默认） |
| `Filled` | 填充背景 |
| `Borderless` | 无边框 |

### InputControlStatus（来自 `AtomUI.Controls`）

输入控件验证状态枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认状态 |
| `Error` | 错误状态（红色边框） |
| `Warning` | 警告状态（橙色边框） |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## 核心数据类型

### ISelectOption 接口

选项数据接口，继承自 `IListItemData`。

```csharp
public interface ISelectOption : IListItemData
{
    object? Header { get; }        // 选项显示文本
    bool IsDynamicAdded { get; }   // 是否为 Tags 模式动态添加的选项
}
```

### SelectOption 记录类

内置的 `ISelectOption` 实现，支持分组。

```csharp
public record SelectOption : ListItemData, ISelectOption, IGroupListItemData
{
    public object? Header { get; init; }
    public bool IsDynamicAdded { get; init; } = false;
}
```

`ListItemData` 基类提供的关键属性：

| 属性 | 类型 | 说明 |
|---|---|---|
| `Content` | `object?` | 选项的值（用于匹配和提交） |
| `IsEnabled` | `bool` | 是否可选 |
| `Group` | `string?` | 分组名称（配合 `IsGroupEnabled` 使用） |

### ISelectOptionsAsyncLoader 接口

异步选项加载器接口。

```csharp
public interface ISelectOptionsAsyncLoader
{
    Task<SelectOptionsLoadResult> LoadAsync(object? context, CancellationToken token);
}
```

### SelectOptionsLoadResult

异步加载结果。

| 属性 | 类型 | 说明 |
|---|---|---|
| `IsSuccess` | `bool` | 加载是否成功 |
| `StatusCode` | `RpcStatusCode` | 状态码 |
| `UserFriendlyMessage` | `string?` | 用户友好的错误信息 |
| `Data` | `IReadOnlyList<ISelectOption>?` | 加载到的选项数据 |

### SelectSelectionChangedEventArgs

选择变更事件参数。

| 属性 | 类型 | 说明 |
|---|---|---|
| `Mode` | `SelectMode` | 当前选择模式 |
| `OldValue` | `object?` | 变更前的值 |
| `NewValue` | `object?` | 变更后的值 |

### ISelectTagTextProvider 接口

自定义标签文本提供器接口。

```csharp
public interface ISelectTagTextProvider
{
    string? TagText { get; }
}
```

---

## AbstractSelect 公共属性

以下属性定义在 `AbstractSelect` 基类中，`Select` 自动继承。

### 基础行为属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsAllowClear` | `bool` | `false` | 是否显示清除按钮，允许一键清空已选值 |
| `IsAutoClearSearchValue` | `bool` | `false` | 选中选项后是否自动清除搜索文本 |
| `IsDefaultOpen` | `bool` | `false` | 初始渲染时是否默认展开下拉 |
| `IsDropDownOpen` | `bool` | `false` | 控制下拉面板的展开/关闭状态 |
| `IsFilterEnabled` | `bool` | `false` | 是否启用搜索过滤功能 |
| `IsPopupMatchSelectWidth` | `bool` | `true` | 下拉面板宽度是否与 Select 控件等宽 |
| `DisplayPageSize` | `int` | `10` | 下拉面板最多可见的选项条数（控制最大高度） |
| `Placement` | `SelectPopupPlacement` | `BottomEdgeAlignedLeft` | 下拉面板弹出位置 |
| `FilterValue` | `object?` | `null` | 当前搜索过滤值 |

### 占位文本属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `PlaceholderText` | `string?` | `null` | 未选中时显示的占位提示文本 |
| `PlaceholderForeground` | `IBrush?` | 由 Token 控制 | 占位文本颜色 |

### 多选标签属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `MaxCount` | `int` | `int.MaxValue` | 最大可选数量限制 |
| `IsShowMaxCountIndicator` | `bool` | `false` | 是否显示最大选择数指示器（如 `2/5`） |
| `MaxTagCount` | `int?` | `null` | 最多显示的标签数量，超出折叠为 `+N` |
| `IsResponsiveTagMode` | `bool` | `false` | 根据控件宽度自动计算可见标签数 |
| `MaxTagPlaceholder` | `string?` | `null` | 标签折叠后的自定义占位文本 |

### 图标属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ClearIcon` | `PathIcon?` | `null` | 自定义清除图标 |
| `SuffixIcon` | `PathIcon?` | `DownOutlined` | 下拉箭头图标 |
| `SuffixLoadingIcon` | `PathIcon?` | `LoadingOutlined`（旋转） | 加载中图标 |

### 样式与状态属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `StyleVariant` | `InputControlStyleVariant` | `Outline` | 样式变体（Outlined/Filled/Borderless） |
| `Status` | `InputControlStatus` | `Default` | 验证状态（Default/Error/Warning） |
| `SizeType` | `SizeType` | `Middle` | 控件尺寸 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用动画效果 |

### AddOn 装饰属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `LeftAddOn` | `object?` | `null` | 左侧外部附加内容 |
| `LeftAddOnTemplate` | `IDataTemplate?` | `null` | 左侧外部附加内容模板 |
| `RightAddOn` | `object?` | `null` | 右侧外部附加内容 |
| `RightAddOnTemplate` | `IDataTemplate?` | `null` | 右侧外部附加内容模板 |
| `ContentLeftAddOn` | `object?` | `null` | 左侧内部前缀内容（如文本 `"User"`） |
| `ContentLeftAddOnTemplate` | `IDataTemplate?` | `null` | 左侧内部前缀内容模板 |
| `ContentRightAddOn` | `object?` | `null` | 右侧内部后缀内容 |
| `ContentRightAddOnTemplate` | `IDataTemplate?` | `null` | 右侧内部后缀内容模板 |

### 空状态指示属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `EmptyIndicator` | `object?` | `null` | 空状态指示器内容 |
| `EmptyIndicatorTemplate` | `IDataTemplate?` | `null` | 空状态指示器模板 |
| `IsShowEmptyIndicator` | `bool` | `true` | 是否在选项为空时显示空状态指示器 |
| `EmptyIndicatorPadding` | `Thickness` | 默认值 | 空状态指示器内边距 |

### 只读属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `IsLoading` | `bool` | 是否正在异步加载选项（只读） |

---

## Select 公共属性

以下属性定义在 `Select` 类中（继承 `AbstractSelect`）。

### 核心属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Mode` | `SelectMode` | `Single` | 选择模式：Single / Multiple / Tags |
| `OptionsSource` | `IEnumerable<ISelectOption>?` | `null` | 绑定的选项数据源 |
| `OptionTemplate` | `IDataTemplate?` | 默认显示 `Header` | 自定义选项渲染模板 |
| `OptionFontSize` | `double` | 由 Token 控制 | 选项字体大小 |
| `IsDefaultActiveFirstOption` | `bool` | `false` | 是否默认激活第一个选项 |
| `IsGroupEnabled` | `bool` | `false` | 是否启用选项分组 |
| `GroupPropertySelector` | `DefaultFilterValueSelector?` | `null` | 分组属性提取函数 |
| `IsHideSelectedOptions` | `bool` | `false` | 多选时是否隐藏已选选项 |
| `AutoScrollToSelectedOptions` | `bool` | `false` | 展开时是否自动滚动到已选选项 |

### 选中值属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `SelectedOption` | `ISelectOption?` | 单选模式下的已选选项（DirectProperty，支持双向绑定） |
| `SelectedOptions` | `IList<ISelectOption>?` | 多选/标签模式下的已选选项列表（DirectProperty） |
| `Options` | `ItemCollection` | 选项集合（`[Content]` 属性，支持 AXAML 直接声明子元素） |

### 搜索过滤属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Filter` | `IValueFilter?` | `null` | 自定义过滤器实现 |
| `FilterValueSelector` | `DefaultFilterValueSelector?` | `HeaderFilterPropertySelector` | 过滤值提取函数 |

### 异步加载属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `OptionsLoader` | `ISelectOptionsAsyncLoader?` | `null` | 异步选项加载器 |
| `OptionsAsyncLoadContext` | `object?` | `null` | 传递给加载器的上下文对象 |

### 默认值属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `DefaultValues` | `IList<object>?` | 默认选中值列表（通过 `Content` 值匹配） |
| `DefaultValueCompareFn` | `Func<object, ISelectOption, bool>?` | 自定义默认值比较函数 |

---

## 事件

### AbstractSelect 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `DropDownOpening` | `EventHandler<CancelEventArgs>` | 下拉即将展开（可取消） |
| `DropDownOpened` | `EventHandler` | 下拉已展开 |
| `DropDownClosing` | `EventHandler<CancelEventArgs>` | 下拉即将关闭（可取消） |
| `DropDownClosed` | `EventHandler` | 下拉已关闭 |

### Select 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `SelectionChanged` | `EventHandler<SelectSelectionChangedEventArgs>` | 选中值变更 |
| `OptionsLoading` | `EventHandler<SelectOptionsLoadingEventArgs>` | 异步加载开始（可取消） |
| `OptionsLoaded` | `EventHandler<SelectOptionsLoadedEventArgs>` | 异步加载完成 |

---

## 公共方法

| 方法名 | 返回值 | 说明 |
|---|---|---|
| `ClearValue()` | `void` | 清空所有已选值（`SelectedOption` 和 `SelectedOptions`） |

---

## 伪类（Pseudo-Classes）

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:dropdownopen` | `SelectPseudoClass.DropdownOpen` | 下拉面板处于打开状态 |
| `:error` | `StdPseudoClass.Error` | `Status == InputControlStatus.Error` |
| `:warning` | `StdPseudoClass.Warning` | `Status == InputControlStatus.Warning` |
| `:outline` | `AddOnDecoratedBoxPseudoClass.Outline` | `StyleVariant == Outline` |
| `:filled` | `AddOnDecoratedBoxPseudoClass.Filled` | `StyleVariant == Filled` |
| `:borderless` | `AddOnDecoratedBoxPseudoClass.Borderless` | `StyleVariant == Borderless` |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 控件按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

### IInputControlStatusAware

```csharp
public InputControlStatus Status { get; set; }
```

### IInputControlStyleVariantAware

```csharp
public InputControlStyleVariant StyleVariant { get; set; }
```

### ICompactSpaceAware

在 `Space.Compact` 容器中使用时自动调整圆角和边框。根据在紧凑空间中的位置自动裁剪圆角。

### IFormItemAware

可作为 `FormItem` 的子控件参与表单验证流程。支持 `SetFormValue` / `GetFormValue` / `ClearFormValue` / `NotifyValidateStatus`。

### IFormItemFeedbackAware

接收表单验证反馈控件（如错误图标、警告图标），并在 `SelectHandle` 区域展示。

---

## 静态字段

| 字段名 | 类型 | 说明 |
|---|---|---|
| `HeaderFilterPropertySelector` | `DefaultFilterValueSelector` | 按 `Header` 属性过滤的默认提取函数 |
| `ValueFilterPropertySelector` | `DefaultFilterValueSelector` | 按 `Content` 属性过滤的提取函数 |
