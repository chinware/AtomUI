# AutoComplete API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### AutoCompletePlacementMode

候选列表弹出方向。

| 值 | 说明 |
|---|---|
| `Top` | 候选列表在输入框上方弹出 |
| `Bottom` | 候选列表在输入框下方弹出（默认） |

### InputControlStyleVariant（来自 `AtomUI.Controls`）

样式变体枚举。

| 值 | 说明 |
|---|---|
| `Outline` | 默认带边框样式 |
| `Filled` | 填充背景样式 |
| `Borderless` | 无边框样式 |
| `Underlined` | 下划线样式 |

### InputControlStatus（来自 `AtomUI.Controls`）

输入控件验证状态。

| 值 | 说明 |
|---|---|
| `Default` | 默认状态 |
| `Warning` | 警告状态（橙色系） |
| `Error` | 错误状态（红色系） |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

### ValueFilterMode（来自 `AtomUI.Controls.Utils`）

过滤模式枚举，用于指定候选项匹配规则。

| 值 | 说明 |
|---|---|
| `None` | 不过滤，返回所有项 |
| `StartsWith` | 前缀匹配，不区分大小写（默认） |
| `StartsWithCaseSensitive` | 前缀匹配，区分大小写 |
| `StartsWithOrdinal` | 前缀匹配，不区分大小写（序数比较） |
| `StartsWithOrdinalCaseSensitive` | 前缀匹配，区分大小写（序数比较） |
| `Contains` | 包含匹配，不区分大小写 |
| `ContainsCaseSensitive` | 包含匹配，区分大小写 |
| `ContainsOrdinal` | 包含匹配，不区分大小写（序数比较） |
| `ContainsOrdinalCaseSensitive` | 包含匹配，区分大小写（序数比较） |
| `Equals` | 完全匹配，不区分大小写 |
| `EqualsCaseSensitive` | 完全匹配，区分大小写 |
| `EqualsOrdinal` | 完全匹配，不区分大小写（序数比较） |
| `EqualsOrdinalCaseSensitive` | 完全匹配，区分大小写（序数比较） |
| `Custom` | 自定义过滤 |

---

## 公共属性（StyledProperty / DirectProperty）

### AbstractAutoComplete 公共属性

以下属性定义在 `AbstractAutoComplete` 基类中，所有 AutoComplete 变体（`AutoComplete`、`AutoCompleteSearchEdit`、`AutoCompleteTextArea`）均可使用。

#### 输入值相关

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Value` | `string?` | `null` | 输入框当前文本值（双向绑定） |
| `DefaultValue` | `string?` | `null` | 默认值 |
| `CaretIndex` | `int` | `0` | 光标位置（双向绑定） |
| `MaxLength` | `int` | `0` | 最大输入字符数，`0` 表示不限制 |
| `IsReadOnly` | `bool` | `false` | 是否只读 |
| `PlaceholderText` | `string?` | `null` | 占位文本 |
| `PlaceholderForeground` | `IBrush?` | 由 Token 控制 | 占位文本前景色 |

#### 候选列表数据

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `OptionsSource` | `IEnumerable<IAutoCompleteOption>?` | `null` | 静态候选数据源 |
| `Options` | `ItemCollection` | 空集合 | 候选项集合（`[Content]` 属性，可直接在 AXAML 中添加子元素） |
| `OptionsView` | `ItemsSourceView`（只读） | — | 候选项的只读视图 |
| `OptionsAsyncLoader` | `ICompleteOptionsAsyncLoader?` | `null` | 异步候选数据加载器 |
| `OptionTemplate` | `IDataTemplate?` | 默认 TextBlock 模板 | 候选项渲染模板 |
| `SelectedOption` | `IAutoCompleteOption?` | `null` | 当前选中的候选项（双向绑定） |

#### 过滤相关

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Filter` | `IValueFilter?` | `StartsWith`（初始化时设置） | 候选过滤器 |
| `FilterValue` | `string?`（只读） | `string.Empty` | 当前过滤关键词（只读 DirectProperty） |
| `FilterValueSelector` | `AutoCompleteFilterValueSelector?` | `null` | 自定义取值委托，控制从 `IAutoCompleteOption` 中取哪个字段做匹配 |

#### 弹出层控制

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsDropDownOpen` | `bool` | `false` | 候选列表是否打开 |
| `Placement` | `AutoCompletePlacementMode` | `Bottom` | 候选列表弹出方向 |
| `DisplayCandidateCount` | `int` | `10` | 最大可见候选项数量 |
| `MaxDropDownHeight` | `double` | `double.PositiveInfinity` | 弹出层最大高度 |
| `IsPopupMatchSelectWidth` | `bool` | `true` | 弹出层是否与输入框等宽 |
| `MinimumPrefixLength` | `int` | `1` | 触发候选的最小输入字符数（`-1` 表示不限制） |
| `MinimumPopulateDelay` | `TimeSpan` | `TimeSpan.Zero` | 触发候选的防抖延迟 |

#### 外观与行为

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `SizeType.Middle` | 尺寸类型（共享属性，通过 `AddOwner` 注册） |
| `StyleVariant` | `InputControlStyleVariant` | `Outline` | 样式变体（共享属性） |
| `Status` | `InputControlStatus` | `Default` | 验证状态（共享属性） |
| `IsAllowClear` | `bool` | `false` | 是否显示清除按钮 |
| `ClearIcon` | `PathIcon?` | `null` | 自定义清除按钮图标 |
| `IsAutoFocus` | `bool` | `false` | 是否自动获取焦点 |
| `IsCompletionEnabled` | `bool` | `false` | 是否启用文本自动完成（回填最佳匹配项） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（共享属性） |
| `IsLoading` | `bool`（只读） | `false` | 是否正在加载数据（只读 DirectProperty） |
| `ClearSelectionOnLostFocus` | `bool` | `false` | 失去焦点时是否清除文本选择 |

#### 附加内容

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ContentLeftAddOn` | `object?` | `null` | 输入框左侧附加内容 |
| `ContentLeftAddOnTemplate` | `IDataTemplate?` | `null` | 左侧附加内容模板 |
| `ContentRightAddOn` | `object?` | `null` | 输入框右侧附加内容 |
| `ContentRightAddOnTemplate` | `IDataTemplate?` | `null` | 右侧附加内容模板 |

### AutoCompleteSearchEdit 额外属性

`AutoCompleteSearchEdit` 在基类属性之上增加了搜索按钮相关属性：

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SearchButtonStyle` | `SearchEditButtonStyle` | — | 搜索按钮样式（共享自 `SearchEdit`） |
| `SearchButtonText` | `string` | — | 搜索按钮文本 |
| `IsOperating` | `bool` | `false` | 搜索按钮是否处于操作中状态 |

### AutoCompleteTextArea 额外属性

`AutoCompleteTextArea` 在基类属性之上增加了多行输入相关属性：

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Lines` | `int` | — | 文本区域行数（共享自 `TextArea`） |
| `IsAutoSize` | `bool` | — | 是否自动调整大小 |
| `IsShowCount` | `bool` | — | 是否显示字符计数 |
| `IsResizable` | `bool` | — | 是否可手动调整大小 |

---

## 事件

### 路由事件（RoutedEvent）

| 事件名 | 类型 | 路由策略 | 说明 |
|---|---|---|---|
| `ValueChanged` | `EventHandler<CompleteValueChangedEventArgs>` | Bubble | 输入值发生变化时触发 |
| `SelectionChanged` | `EventHandler<SelectionChangedEventArgs>` | Bubble | 选中候选项发生变化时触发 |

### 普通事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Populating` | `EventHandler<CompletePopulatingEventArgs>` | 候选列表开始填充时触发（可取消） |
| `Populated` | `EventHandler<CompletePopulatedEventArgs>` | 候选列表填充完成时触发 |
| `OptionsLoaded` | `EventHandler<CompleteOptionsLoadedEventArgs>` | 异步加载完成时触发 |
| `DropDownOpening` | `EventHandler<CancelEventArgs>` | 候选列表即将打开时触发（可取消） |
| `DropDownOpened` | `EventHandler` | 候选列表已打开时触发 |
| `DropDownClosing` | `EventHandler<CancelEventArgs>` | 候选列表即将关闭时触发（可取消） |
| `DropDownClosed` | `EventHandler` | 候选列表已关闭时触发 |

### 事件参数类

#### CompleteValueChangedEventArgs

```csharp
public class CompleteValueChangedEventArgs : RoutedEventArgs
{
    public string? Value { get; }  // 变更后的值
}
```

#### CompletePopulatingEventArgs

```csharp
public class CompletePopulatingEventArgs : CancelEventArgs
{
    public string? Predicate { get; }  // 当前过滤关键词
    // 设置 Cancel = true 可中止自动填充流程
}
```

#### CompletePopulatedEventArgs

```csharp
public class CompletePopulatedEventArgs : EventArgs
{
    public IList<IAutoCompleteOption>? Data { get; }  // 填充后的候选数据
}
```

#### CompleteOptionsLoadedEventArgs

```csharp
public class CompleteOptionsLoadedEventArgs : EventArgs
{
    public string? Predicate { get; }                // 查询关键词
    public CompleteOptionsLoadResult Result { get; }  // 加载结果
}
```

---

## 伪类（Pseudo-Classes）

AutoComplete 支持以下伪类：

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:candidateopen` | `AutoCompletePseudoClass.CandidatePopupOpen` | `IsDropDownOpen == true`，候选列表处于打开状态 |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 鼠标按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点 |

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

### IFormItemAware

可作为 `FormItem` 的子控件参与表单验证。AutoComplete 实现了以下表单集成行为：
- `SetFormValue` → 设置 `Value` 属性
- `GetFormValue` → 返回 `Value` 属性
- `ClearFormValue` → 清空 `Value`
- `NotifyValidateStatus` → 根据验证状态自动设置 `Status` 属性

### IFormItemFeedbackAware

支持接收表单验证反馈控件（错误/警告图标）。

### ICompactSpaceAware（仅 AutoComplete 和 AutoCompleteSearchEdit）

在 `Space.Compact` 容器中使用时自动调整圆角和边框。

---

## 数据模型

### IAutoCompleteOption

候选选项接口，继承自 `IListItemData`。

```csharp
public interface IAutoCompleteOption : IListItemData
{
    object? Header { get; }  // 候选项显示文本
}
```

`IListItemData` 提供的属性：

| 属性 | 类型 | 说明 |
|---|---|---|
| `Content` | `object?` | 选项内容（选中后填入输入框的值） |
| `IsEnabled` | `bool` | 是否可用 |
| `IsSelected` | `bool` | 是否选中 |
| `ItemKey` | `EntityKey?` | 唯一标识 |
| `Group` | `string?` | 分组标识 |

### AutoCompleteOption

默认的候选选项 record 实现：

```csharp
public record AutoCompleteOption : IAutoCompleteOption
{
    public object? Header { get; set; }
    public string? Group { get; set; }
    public bool IsEnabled { get; set; } = true;
    public object? Content { get; set; }
    public bool IsSelected { get; set; }
    public EntityKey? ItemKey { get; init; }
}
```

### ICompleteOptionsAsyncLoader

异步数据加载器接口：

```csharp
public interface ICompleteOptionsAsyncLoader
{
    Task<CompleteOptionsLoadResult> LoadAsync(string? context, CancellationToken token);
}
```

### CompleteOptionsLoadResult

异步加载结果：

```csharp
public class CompleteOptionsLoadResult
{
    public bool IsSuccess => StatusCode == RpcStatusCode.Success;
    public RpcStatusCode StatusCode { get; init; } = RpcStatusCode.Success;
    public string? UserFriendlyMessage { get; init; }
    public IReadOnlyList<IAutoCompleteOption>? Data { get; init; }
}
```

### AutoCompleteFilterValueSelector

自定义过滤取值委托：

```csharp
public delegate object? AutoCompleteFilterValueSelector(IAutoCompleteOption option);
```
