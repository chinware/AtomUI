# Mentions

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Mentions 是 AtomUI 桌面数据录入体系中的提及输入控件，用于在多行文本输入中识别触发前缀，展示候选项弹层，并把用户选择的候选项插入到文本中。它以 `TextArea` / `AbstractTextInput` 为逻辑输入层，以 `InputControlFrame` 为输入表面，结合 `Popup`、`CandidateList`、同步/异步数据加载、过滤、Form 和 Token 体系，提供类似 `@user`、`#tag` 的文本提及体验。

Mentions 的职责是编辑单个字符串值，并在触发上下文中完成候选项选择和文本插入。它不负责远程服务协议、权限判断、富文本 token 渲染、结构化 mention 实体存储、消息发送或搜索结果管理。业务层需要保存结构化 mention 信息时，应在 `Value` 文本和业务数据之间建立自己的解析模型。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Mentions` |
| 状态 | Stable |

## 何时使用

Mentions 的设计语言来自文本区域、触发符和候选弹层的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 文本语义 | 用户编辑包含 mention 标记的普通文本。 | `Value`、`DefaultValue`、`PlaceholderText`。 |
| 触发语义 | 输入触发符后进入候选选择上下文。 | `TriggerPrefix`、`CandidateTriggered`、`FilterValue`。 |
| 候选数据 | 候选项可来自本地集合或异步 loader。 | `OptionsSource`、`OptionsAsyncLoader`、`Populating`、`Populated`。 |
| 输入表面 | 输入框边框、背景和反馈状态。 | `StyleVariant`、`Status`、`SizeType`。 |
| 弹层位置 | 候选弹层相对触发位置向上或向下展开。 | `Placement=Top/Bottom`。 |
| 多行输入 | 输入区域可固定行数或自动高度。 | `Lines`、`MinLines`、`MaxLines`、`IsAutoSize`。 |
| 加载状态 | 异步候选加载时弹层展示 loading。 | `IsLoading`、`AsyncLoadDebounce`、`AsyncLoadTimeout`。 |

`Custom` 尺寸不是 Mentions 的第四套专属 Token。它沿用 TextArea 的 Custom size 规则：未显式设置时以 `Middle` 为视觉基线，用户可通过常规尺寸属性覆盖实际大小。

## 公共 API

Mentions 的公共 API 由文本值、触发符、候选数据、过滤、弹层、输入表面和 Form 集成组成。属性不应被机械理解为独立功能；它们共同描述 mention 输入的状态机。

文本与输入 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Value` | `string?` | 当前文本值，默认 `TwoWay` 绑定到内部 `MentionTextArea.Text`，并接入 Avalonia `DataValidationErrors`。 |
| `DefaultValue` | `string?` | 初始化时 `Value` 为空时写入的默认文本。 |
| `PlaceholderText` | `string?` | 空文本状态的占位提示。 |
| `IsReadOnly` | `bool` | 只读状态，内部文本区域不可编辑。 |
| `IsAllowClear` / `ClearIcon` | `bool` / `PathIcon?` | 清除按钮入口和图标。 |
| `IsAutoFocus` | `bool` | 控件加载后请求内部文本区域焦点。 |
| `IsAutoSize` | `bool` | 内部 TextArea 根据内容自动调整高度。 |
| `Lines` / `MinLines` / `MaxLines` | `int` | 多行输入显示行数和自动高度边界。 |

候选与触发 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `TriggerPrefix` | `IList<string>?` | mention 触发前缀。当前触发识别只支持长度为 1 的字符串。默认值为 `["@"]`。 |
| `Split` | `string?` | 插入候选项时使用的分隔符。为空时默认用空格包裹 mention 文本。 |
| `OptionsSource` | `IEnumerable<IMentionOption>?` | 本地候选项集合。集合变更会同步到内部缓存和候选视图。 |
| `OptionsAsyncLoader` | `IMentionOptionsAsyncLoader?` | 异步候选加载器。存在时，触发候选弹层会优先使用异步加载。 |
| `OptionTemplate` | `IDataTemplate?` | 候选项显示模板。默认模板显示 `IMentionOption.Header`。 |
| `Filter` | `IValueFilter?` | 候选过滤器。为空时使用 contains 过滤。 |
| `FilterValueSelector` | `DefaultFilterValueSelector?` | 从候选项提取过滤文本的 selector。 |
| `DisplayCandidateCount` | `int` | 弹层可显示的候选项数量，用于计算最大高度。默认 `10`。 |

弹层与异步 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Placement` | `MentionsPlacementMode` | 候选弹层相对触发位置的展开方向，默认 `Bottom`。 |
| `IsDropDownOpen` | `bool` | 候选弹层打开状态。 |
| `ShouldUseOverlayPopup` | `bool` | 是否使用 overlay popup 宿主，默认 `true`。 |
| `AsyncLoadDebounce` | `TimeSpan` | 异步加载前的防抖时间，默认 `TimeSpan.Zero`，值必须非负。 |
| `AsyncLoadTimeout` | `TimeSpan` | 异步加载超时时间，默认 `10` 秒。 |
| `IsLoading` | `bool` | 异步或事件填充过程中的内部 loading 状态，只读公开。 |

输入表面 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `SizeType` | `CustomizableSizeType` | 输入尺寸密度。 |
| `StyleVariant` | `InputControlStyleVariant` | 输入表面样式。 |
| `Status` | `InputControlStatus` | 显式输入反馈状态；最终视觉由 `InputControlFrame.EffectiveStatus` 计算，native validation error 以 `DataValidationErrors` 为唯一真源。 |
| `ContentLeftAddOn` / `ContentRightAddOn` | `object?` | 内部左右附加内容，传递给内部 `MentionTextArea`。 |
| `ContentLeftAddOnTemplate` / `ContentRightAddOnTemplate` | `IDataTemplate?` | 内部左右附加内容模板。 |
| `IsMotionEnabled` | `bool` | 内部输入壳体、候选列表和 popup 动效开关。 |

候选项契约：

| 类型 | 语义 |
| --- | --- |
| `IMentionOption.Key` | 候选项稳定 key，可作为过滤候选值回退。 |
| `IMentionOption.Header` | 候选项显示内容，默认模板显示该值。 |
| `IMentionOption.Value` | 插入文本时优先使用的值。 |
| `IMentionOption.IsEnabled` | 候选项启用状态，供候选列表视觉和选择逻辑使用。 |

事件契约：

| 事件 | 语义 |
| --- | --- |
| `CandidateTriggered` | 触发符被识别时触发，事件参数包含触发字符。 |
| `Populating` | 候选弹层准备填充前触发，可取消默认填充并由业务手动更新候选集合。 |
| `Populated` | 候选视图刷新后触发，事件参数包含只读候选视图。 |
| `OptionsLoaded` | 异步 loader 完成后触发，包含加载结果或错误状态。 |
| `DropDownOpening` / `DropDownClosing` | 弹层打开/关闭前触发，可取消。 |
| `DropDownOpened` / `DropDownClosed` | 弹层打开/关闭后触发。 |

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_TextArea` | `MentionTextArea` | 文本输入、触发符识别、过滤值同步和候选插入。 |
| `PART_Popup` | `Popup` | 候选弹层宿主。 |
| `PART_CandidateList` | `ICandidateList` / `CandidateList` | 候选项展示、键盘导航、提交和取消。 |
| `PopupFrame` | `Border` | 候选弹层背景、圆角、宽度、高度和 padding。 |
| `LoadingIndicator` | `Spin` | 异步候选加载状态展示。 |

`EmptyIndicator`、`EmptyIndicatorTemplate`、`IsShowEmptyIndicator` 和 `EmptyIndicatorPadding` 是当前 public surface 的一部分；默认 `MentionsTheme` 尚未把这些属性绑定到候选弹层视觉结构中，维护时不能把它们描述为已经稳定生效的默认视觉契约。

## 事件与命令

| `IsLoading` | `bool` | 异步或事件填充过程中的内部 loading 状态，只读公开。 |
事件契约：
| 事件 | 语义 |
| `CandidateTriggered` | 触发符被识别时触发，事件参数包含触发字符。 |
| `Populated` | 候选视图刷新后触发，事件参数包含只读候选视图。 |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Mentions/Views/MentionsShowCase.axaml:86`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Mentions Name="BasicMentions"
```

### Value 绑定

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Mentions/Views/MentionsShowCase.axaml:101`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical" Spacing="12">
    <atom:Mentions HorizontalAlignment="Stretch"
                   OptionsSource="{Binding BasicMentionOptions}"
                   IsAllowClear="True"
                   Value="{Binding BoundValue}" />
    <StackPanel Orientation="Horizontal" Spacing="8">
        <atom:TextBlock VerticalAlignment="Center"
                        Text="绑定值：" />
        <atom:TextBlock VerticalAlignment="Center"
                        Text="{Binding BoundValueText}" />
    </StackPanel>
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button SizeType="Small"
                     Command="{Binding SetBoundValueCommand}"
                     Content="设置提及" />
        <atom:Button SizeType="Small"
                     Command="{Binding ClearBoundValueCommand}"
                     Content="清空" />
    </StackPanel>
</StackPanel>
```

### 变体

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Mentions/Views/MentionsShowCase.axaml:166`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Mentions HorizontalAlignment="Stretch" StyleVariant="Outlined" PlaceholderText="线框风格"/>
    <atom:Mentions HorizontalAlignment="Stretch" StyleVariant="Filled" PlaceholderText="填充风格"/>
    <atom:Mentions HorizontalAlignment="Stretch" StyleVariant="Borderless" PlaceholderText="无边框"/>
    <atom:Mentions HorizontalAlignment="Stretch" StyleVariant="Underlined" PlaceholderText="下划线"/>
</StackPanel>
```

### 异步加载

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Mentions/Views/MentionsShowCase.axaml:182`

Gallery key：`ExamplesContent` / item `4`

```axaml
<atom:Mentions HorizontalAlignment="Stretch" OptionsAsyncLoader="{Binding MentionOptionAsyncLoader}"/>
```

## 状态模型

Mentions 的核心状态流：

```text
Value / DefaultValue
      ↓
MentionTextArea.Text
      ↓
Text input + caret position
      ↓
TriggerPrefix scan
      ↓
FilterValue + CandidateOpenRequest
      ↓
Populate / async load / filter
      ↓
CandidateList view
      ↓
Commit option
      ↓
Insert mention text back into Value
```

触发符识别从当前 `CaretIndex` 向前扫描，遇到空白或控制字符停止。只有长度为 1 的 `TriggerPrefix` 项会被识别。触发后，`MentionTextArea` 计算触发字符位置和过滤文本，并向外请求打开候选弹层。

候选填充优先级：

```text
OptionsAsyncLoader != null
  → async LoadAsync(FilterValue, token)
  → OptionsLoaded
  → OptionsSource = result.Data
  → PopulateComplete()

OptionsAsyncLoader == null
  → Populating(FilterValue)
  → if !Cancel PopulateComplete()
```

弹层状态优先级：

```text
Disabled / invisible / window deactivated
> DropDownClosing cancellation
> DropDownOpening cancellation
> IsDropDownOpen
> Candidate trigger state
```

键盘行为：

- 弹层打开时，按键优先交给 `CandidateList.HandleKeyDown()`。
- `Escape` 取消候选并关闭弹层。
- `Enter` 在弹层打开时提交当前候选。
- `F4` 切换弹层打开状态。
- 弹层关闭时，`Down` 可打开弹层，除非该按键被 XY focus 导航占用。

Form 集成以 `Value` 作为表单值。`Value` 是用户拥有的受控文本值，默认双向绑定；错误校验状态通过 `DataValidationErrors` 投射到 shared `InputControlFrame.EffectiveStatus`，驱动外层输入表面和内部 `MentionTextArea`；`NotifyValidateStatus` 只同步 warning、success、validating 等 Form 扩展状态。Form feedback 控件传递给内部 `MentionTextArea`。

## 主题与 Design Token

Mentions 的默认视觉由 `MentionsTheme.axaml` 和内部 TextArea 主题协作：

| 主题或资源 | 职责 |
| --- | --- |
| `MentionsTheme.axaml` | 根模板、`MentionTextArea`、popup、loading、候选列表和 MentionsToken 资源绑定。 |
| `TextAreaTheme.axaml` | 输入表面、placeholder、清除按钮、Form feedback、多行高度和状态视觉。 |
| `CandidateList` 主题 | 候选项容器、选中项、键盘导航和提交取消事件。 |
| `PopupHostToken` | popup 阴影、圆角和 anchor margin。 |
| `MentionsToken` | popup 内容 padding、候选项高度和最小宽度。 |

输入框本体不由 Mentions 自己重写边框和文本 presenter，而是通过内部 `MentionTextArea` 复用 TextArea 的 `AbstractTextInput` 与 `InputControlFrame` 体系。Mentions 的控件级 Token 只服务候选弹层，不承载文本高度、输入边框、状态颜色或 Form feedback。

Token 来源：

MentionsToken 是 Mentions 的控件级 Token scope，只描述候选弹层的结构尺寸。输入框本体由内部 `MentionTextArea` 复用 TextArea / `AbstractTextInput` / `InputControlFrame` / SharedToken 体系；候选列表项的选择、hover、disabled 和文本状态由 CandidateList 主题处理。

MentionsToken 不承载以下状态：

- `Value`、`FilterValue`、`TriggerPrefix`、`OptionsSource`、`OptionsAsyncLoader` 等数据状态。
- `IsDropDownOpen`、`IsLoading`、`IsReadOnly`、`IsAutoSize`、`Status` 等运行状态。
- 候选项 selected、hover、disabled、commit、cancel 等交互状态。
- Popup 当前 offset、placement、实际高度或异步加载结果。

## AOT 与裁剪注意事项

Mentions 不依赖运行时反射发现模板结构。模板协作通过固定 template part、接口、事件和 owner 引用完成。

资源和生命周期边界：

- `_collectionChangeSubscription` 使用弱订阅，但 `OptionsSource` 变化时仍必须 dispose。
- `_subscriptionsOnOpen` 只在 popup 打开期间存在，popup 关闭时必须释放。
- `_deactivationSubscription` 必须在 detach 时释放。
- `_delayTimer` 更换时必须停止旧 timer 并解除 Tick。
- `CandidateList` 替换时必须清空旧列表 `ItemsSource`，避免旧视图保留。
- 异步加载通过 `AsyncSearchLoadCoordinator` 处理超时、取消和跳过旧结果，不在控件里保留业务任务。

AOT 边界：

- `MentionsToken` 通过 token generator 显式注册，生成 `MentionsTokenKind` 和 `MentionsTokenResourceExtension`。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护。
- `OptionTemplate` 和 `EmptyIndicatorTemplate` 是 XAML 模板入口，不依赖运行时成员扫描。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Mentions/Mentions.cs`：public API、状态流、候选加载、过滤、弹层生命周期、键盘处理和 Form 接口。
- `src/AtomUI.Desktop.Controls/Mentions/Mentions.SemanticParts.cs`：Mentions 的 9 个 Semantic Part 声明（见 [Mentions Semantic Part 契约](semantic-part.md)）。
- `src/AtomUI.Desktop.Controls/Mentions/MentionTextArea.cs`：内部 TextArea，负责触发符扫描、过滤值同步和候选插入。
- `src/AtomUI.Desktop.Controls/Mentions/MentionOption.cs`：候选项接口和默认 record。
- `src/AtomUI.Desktop.Controls/Mentions/DataLoad/IMentionOptionsAsyncLoader.cs`：异步候选加载接口。
- `src/AtomUI.Desktop.Controls/Mentions/DataLoad/MentionOptionsLoadResult.cs`：异步加载结果。
- `src/AtomUI.Desktop.Controls/Mentions/*EventArgs.cs`：候选触发、填充、加载和选项事件参数。
- `src/AtomUI.Desktop.Controls/Mentions/MentionsPlacementMode.cs`：弹层展开方向。
- `src/AtomUI.Desktop.Controls/Mentions/MentionPseudoClass.cs`：`:candidateopen` 伪类常量。
- `src/AtomUI.Desktop.Controls/Mentions/MentionsToken.cs`：候选弹层尺寸 Token。
- `src/AtomUI.Desktop.Controls/Mentions/Themes/MentionsTheme.axaml`：Mentions 模板、popup、loading 和候选列表视觉。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/mentions/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/mentions/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/data-entry/mentions/semantic-part.md`
- Token 文档：`docs/controls/desktop/data-entry/mentions/token.md`
- 变更记录：`docs/controls/desktop/data-entry/mentions/changelog.md`
- 语义结构：`./semantic-cn.md`
