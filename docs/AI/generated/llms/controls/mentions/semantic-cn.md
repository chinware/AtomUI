# Mentions 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Mentions` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Mentions/Themes/MentionsTheme.axaml`

```xml
<Panel>
    <MentionTextArea Name="PART_TextArea" />
    <Popup Name="PART_Popup">
        <Border Name="PopupFrame">
            <Panel>
                <Spin Name="LoadingIndicator" />
                <CandidateList Name="PART_CandidateList" />
            </Panel>
        </Border>
    </Popup>
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Mentions
  -> Mentions (control theme, MentionsTheme.axaml)
     -> Panel (template-stable)
        -> MentionTextArea#PART_TextArea (template-stable)
        -> Popup#PART_Popup (template-stable)
           -> Border#PopupFrame (template-stable)
              -> Panel (template-stable)
                 -> Spin#LoadingIndicator (template-stable)
                 -> CandidateList#PART_CandidateList (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Mentions` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Mentions` | control theme | `MentionsTheme.axaml` | 用户代码 / 控件宿主 | `ClearIcon`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `DataValidationErrors` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `MentionsTheme.axaml` | Mentions | `ClearIcon`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `DataValidationErrors` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TextArea` | template node (MentionTextArea) | `MentionsTheme.axaml` | Mentions | `ClearIcon`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate`, `DataValidationErrors` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Popup` | template node (Popup) | `MentionsTheme.axaml` | Mentions | `IsLoading`, `IsMotionEnabled`, `MaxPopupHeight`, `MinPopupWidth`, `OptionTemplate`, `PopupContentPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PopupFrame` | template node (Border) | `MentionsTheme.axaml` | Mentions | `IsLoading`, `IsMotionEnabled`, `MaxPopupHeight`, `MinPopupWidth`, `OptionTemplate`, `PopupContentPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `LoadingIndicator` | template node (Spin) | `MentionsTheme.axaml` | Mentions | `IsLoading` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CandidateList` | template node (CandidateList) | `MentionsTheme.axaml` | Mentions | `IsLoading`, `IsMotionEnabled`, `OptionTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_TextArea` | `MentionTextArea` | 文本输入、触发符识别、过滤值同步和候选插入。 |
| `PART_Popup` | `Popup` | 候选弹层宿主。 |
| `PART_CandidateList` | `ICandidateList` / `CandidateList` | 候选项展示、键盘导航、提交和取消。 |
| `PopupFrame` | `Border` | 候选弹层背景、圆角、宽度、高度和 padding。 |
| `LoadingIndicator` | `Spin` | 异步候选加载状态展示。 |

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

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

## Theme and Token Boundaries

Mentions 的默认视觉由 `MentionsTheme.axaml` 和内部 TextArea 主题协作：

| 主题或资源 | 职责 |
| --- | --- |
| `MentionsTheme.axaml` | 根模板、`MentionTextArea`、popup、loading、候选列表和 MentionsToken 资源绑定。 |
| `TextAreaTheme.axaml` | 输入表面、placeholder、清除按钮、Form feedback、多行高度和状态视觉。 |
| `CandidateList` 主题 | 候选项容器、选中项、键盘导航和提交取消事件。 |
| `PopupHostToken` | popup 阴影、圆角和 anchor margin。 |
| `MentionsToken` | popup 内容 padding、候选项高度和最小宽度。 |

输入框本体不由 Mentions 自己重写边框和文本 presenter，而是通过内部 `MentionTextArea` 复用 TextArea 的 `AbstractTextInput` 与 `InputControlFrame` 体系。Mentions 的控件级 Token 只服务候选弹层，不承载文本高度、输入边框、状态颜色或 Form feedback。

Token 边界：

MentionsToken 是 Mentions 的控件级 Token scope，只描述候选弹层的结构尺寸。输入框本体由内部 `MentionTextArea` 复用 TextArea / `AbstractTextInput` / `InputControlFrame` / SharedToken 体系；候选列表项的选择、hover、disabled 和文本状态由 CandidateList 主题处理。

MentionsToken 不承载以下状态：

- `Value`、`FilterValue`、`TriggerPrefix`、`OptionsSource`、`OptionsAsyncLoader` 等数据状态。
- `IsDropDownOpen`、`IsLoading`、`IsReadOnly`、`IsAutoSize`、`Status` 等运行状态。
- 候选项 selected、hover、disabled、commit、cancel 等交互状态。
- Popup 当前 offset、placement、实际高度或异步加载结果。

## Customization Boundaries

维护 Mentions 时必须保持以下不变量：

- `Value` 必须继续与内部 `MentionTextArea.Text` 双向同步。
- `DefaultValue` 只在初始化且 `Value` 为空时写入。
- `TriggerPrefix` 默认值为 `["@"]`，当前触发识别只支持单字符前缀。
- 触发扫描不能跨越空白或控制字符。
- `OptionsSource` 集合变化必须同步到内部缓存和候选视图。
- `OptionsAsyncLoader` 存在时必须优先走异步加载，并通过 `OptionsLoaded` 报告结果。
- `AsyncLoadDebounce` 变化必须释放旧 timer，避免重复填充。
- 重新套用模板必须解绑旧 `MentionTextArea`、旧 `Popup` 和旧 `CandidateList` 事件。
- 弹层打开期间的可见性、启用状态和窗口失活必须关闭弹层。
- 候选提交必须通过 `MentionTextArea.InsertMentionOption()` 写回文本，保持 undo/redo 快照和 caret/selection 语义。
- MentionsToken 不承载输入文本、过滤值、候选数据、loading、Form 状态或交互状态。

维护不变量：

内部重构必须保持以下不变量：

- `Value` 和 `MentionTextArea.Text` 继续双向同步；`Value` 作为 Form 值必须默认 `TwoWay` 并启用 Avalonia 数据验证。
- `TriggerPrefix` 默认值、单字符识别和空白边界不能改变。
- `FilterValue` 必须随 caret 和文本变化更新。
- 打开候选弹层必须先触发 `CandidateTriggered`，再走打开和填充流程。
- 弹层打开/关闭必须尊重 `DropDownOpening` / `DropDownClosing` 的取消结果。
- 异步 loader 结果不能让过期请求覆盖当前候选视图。
- 候选提交必须使用 `MentionTextArea.InsertMentionOption()`，保留 undo/redo 和 selection 语义。
- 重新套用模板不能泄漏旧 part 的事件订阅。
- MentionsToken 只服务 popup 尺寸，不承载候选数据、过滤值、loading 或输入状态。
