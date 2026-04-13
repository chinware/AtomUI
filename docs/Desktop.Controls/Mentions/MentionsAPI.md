# Mentions API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### MentionsPlacementMode

候选弹窗弹出方向枚举。

| 值 | 说明 |
|---|---|
| `Top` | 候选列表在触发字符上方弹出 |
| `Bottom` | 候选列表在触发字符下方弹出（默认） |

### InputControlStyleVariant（来自 `AtomUI.Controls`）

输入控件样式变体枚举。

| 值 | 说明 |
|---|---|
| `Outline` | 边框样式（默认） |
| `Filled` | 填充背景样式 |
| `Borderless` | 无边框样式 |
| `Underlined` | 下划线样式 |

### InputControlStatus（来自 `AtomUI.Controls`）

输入控件验证状态枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认状态 |
| `Error` | 错误状态（红色视觉） |
| `Warning` | 警告状态（橙色视觉） |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## 接口类型

### IMentionOption

候选项数据接口，所有候选项必须实现此接口。

```csharp
public interface IMentionOption
{
    string? Key { get; }
    object? Header { get; }
    bool IsEnabled { get; }
    object? Value { get; }
}
```

| 成员 | 类型 | 说明 |
|---|---|---|
| `Key` | `string?` | 唯一标识键 |
| `Header` | `object?` | 显示文本（用于列表展示和默认过滤） |
| `IsEnabled` | `bool` | 是否启用 |
| `Value` | `object?` | 选中后插入的值 |

### MentionOption

`IMentionOption` 的默认 `record` 实现：

```csharp
public record MentionOption : IMentionOption
{
    public object? Header { get; init; }
    public bool IsEnabled { get; init; } = true;
    public object? Value { get; init; }
    public string? Key { get; init; }
}
```

### IMentionOptionFilter

自定义候选项过滤器接口。

```csharp
public interface IMentionOptionFilter
{
    bool Filter(IMentionOption option, object? filterValue);
}
```

### IMentionOptionsAsyncLoader

异步候选项加载器接口。

```csharp
public interface IMentionOptionsAsyncLoader
{
    Task<MentionOptionsLoadResult> LoadAsync(string? context, CancellationToken token);
}
```

### MentionOptionsLoadResult

异步加载结果记录。

```csharp
public record MentionOptionsLoadResult
{
    public bool IsSuccess => StatusCode == RpcStatusCode.Success;
    public RpcStatusCode StatusCode { get; init; }
    public string? UserFriendlyMessage { get; init; }
    public IReadOnlyList<IMentionOption>? Data { get; init; }
}
```

---

## 委托类型

### MentionsFilterValueSelector

自定义过滤取值委托，用于从 `IMentionOption` 中提取用于过滤比较的值。

```csharp
public delegate object? MentionsFilterValueSelector(IMentionOption option);
```

---

## 公共属性（StyledProperty）

### 核心属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Value` | `string?` | `null` | 输入框文本值 |
| `DefaultValue` | `string?` | `null` | 初始默认值（控件初始化时若 `Value` 为 null 则使用此值） |
| `TriggerPrefix` | `IList<string>?` | ["@"] | 触发候选弹窗的前缀字符列表 |
| `Split` | `string?` | `null` | 选中候选项后在插入文本前后添加的分隔符 |
| `OptionsSource` | `IEnumerable<IMentionOption>?` | `null` | 候选项数据源 |
| `OptionTemplate` | `IDataTemplate?` | 默认 TextBlock 模板 | 候选项显示模板 |
| `Placement` | `MentionsPlacementMode` | `Bottom` | 候选弹窗弹出方向 |
| `PlaceholderText` | `string?` | `null` | 占位符文本 |
| `DisplayCandidateCount` | `int` | `10` | 弹窗中显示的最大候选项数量（影响弹窗最大高度） |

### 过滤属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Filter` | `IValueFilter?` | `null`（使用内置 `Contains`） | 自定义值过滤器 |
| `OptionFilter` | `IMentionOptionFilter?` | `null` | 自定义候选项过滤器 |
| `FilterValueSelector` | `MentionsFilterValueSelector?` | `null` | 自定义过滤取值委托 |
| `MinimumPopulateDelay` | `TimeSpan` | `TimeSpan.Zero` | 弹窗填充最小延迟时间 |

### 异步加载属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `OptionsAsyncLoader` | `IMentionOptionsAsyncLoader?` | `null` | 异步候选项加载器 |
| `IsLoading` | `bool`（只读） | `false` | 当前是否正在加载候选项 |

### 样式与状态属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `StyleVariant` | `InputControlStyleVariant` | `Outline` | 样式变体（共享属性，通过 `AddOwner` 注册） |
| `Status` | `InputControlStatus` | `Default` | 验证状态（共享属性，通过 `AddOwner` 注册） |
| `SizeType` | `SizeType` | `Middle` | 尺寸类型（共享属性，通过 `AddOwner` 注册） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用弹窗动画（共享属性） |

### 输入行为属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsAllowClear` | `bool` | `false` | 是否显示清除按钮 |
| `ClearIcon` | `PathIcon?` | `CloseCircleFilled` | 清除按钮图标 |
| `IsAutoFocus` | `bool` | `false` | 是否自动获取焦点 |
| `IsAutoSize` | `bool` | `false` | 是否根据内容自动调整高度 |
| `IsReadOnly` | `bool` | `false` | 是否只读 |
| `Lines` | `int` | `1` | 固定行数 |
| `MinLines` | `int` | 跟随 `Lines` | 最小行数（配合 `IsAutoSize`） |
| `MaxLines` | `int` | 跟随 `Lines` | 最大行数（配合 `IsAutoSize`） |
| `IsDropDownOpen` | `bool` | `false` | 候选弹窗是否打开 |

### 附加内容属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ContentLeftAddOn` | `object?` | `null` | 输入框左侧附加内容 |
| `ContentLeftAddOnTemplate` | `IDataTemplate?` | `null` | 左侧附加内容模板 |
| `ContentRightAddOn` | `object?` | `null` | 输入框右侧附加内容 |
| `ContentRightAddOnTemplate` | `IDataTemplate?` | `null` | 右侧附加内容模板 |

### 空内容指示器属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `EmptyIndicator` | `object?` | `null` | 候选列表为空时显示的内容 |
| `EmptyIndicatorTemplate` | `IDataTemplate?` | `null` | 空内容指示器模板 |
| `IsShowEmptyIndicator` | `bool` | `true` | 是否显示空内容指示器 |
| `EmptyIndicatorPadding` | `Thickness` | `0` | 空内容指示器内间距 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `CandidateTriggered` | `EventHandler<MentionCandidateTriggeredEventArgs>` | 触发字符被检测到时触发，携带触发字符信息 |
| `Populating` | `EventHandler<MentionsPopulatingEventArgs>` | 候选列表填充前触发，可取消（`Cancel = true`） |
| `Populated` | `EventHandler<MentionsPopulatedEventArgs>` | 候选列表填充完成后触发，携带过滤后的候选项列表 |
| `OptionsLoaded` | `EventHandler<MentionOptionsLoadedEventArgs>` | 异步加载完成后触发，携带加载结果 |
| `DropDownOpening` | `EventHandler<CancelEventArgs>` | 候选弹窗打开前触发，可取消 |
| `DropDownOpened` | `EventHandler` | 候选弹窗已打开 |
| `DropDownClosing` | `EventHandler<CancelEventArgs>` | 候选弹窗关闭前触发，可取消 |
| `DropDownClosed` | `EventHandler` | 候选弹窗已关闭 |

### 事件参数类型

**MentionCandidateTriggeredEventArgs**：

| 属性 | 类型 | 说明 |
|---|---|---|
| `TriggerChar` | `string` | 触发的前缀字符 |

**MentionsPopulatingEventArgs**（继承 `CancelEventArgs`）：

| 属性 | 类型 | 说明 |
|---|---|---|
| `Predicate` | `string?` | 当前过滤文本 |
| `Cancel` | `bool` | 设为 `true` 可取消默认填充，需手动调用 `PopulateComplete()` |

**MentionsPopulatedEventArgs**：

| 属性 | 类型 | 说明 |
|---|---|---|
| `Options` | `IList<IMentionOption>?` | 过滤后的候选项列表 |

**MentionOptionsLoadedEventArgs**：

| 属性 | 类型 | 说明 |
|---|---|---|
| `Predicate` | `string?` | 过滤文本 |
| `Result` | `MentionOptionsLoadResult` | 加载结果 |

---

## 公共方法

| 方法名 | 返回类型 | 说明 |
|---|---|---|
| `PopulateComplete()` | `void` | 手动完成候选列表填充（当 `Populating` 事件中设置 `Cancel = true` 时调用） |

---

## 伪类（Pseudo-Classes）

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:candidateopen` | `MentionPseudoClass.CandidatePopupOpen` | `IsDropDownOpen == true`，候选弹窗处于打开状态 |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点 |
| `:pressed` | 按下状态 |

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

可作为 `FormItem` 的子控件参与表单验证流程。`Value` 属性作为表单值。

### IFormItemFeedbackAware

支持接收表单验证反馈控件。

---

## 键盘交互

| 按键 | 弹窗关闭时 | 弹窗打开时 |
|---|---|---|
| ↓ | 打开候选弹窗 | 选中下一个候选项 |
| ↑ | — | 选中上一个候选项 |
| Enter | — | 确认选中项并插入文本 |
| Escape | — | 取消选择并关闭弹窗 |
| F4 | 切换弹窗开关 | 切换弹窗开关 |
