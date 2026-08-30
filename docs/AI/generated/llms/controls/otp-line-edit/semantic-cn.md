# OtpLineEdit 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `OtpLineEdit` | 控件根语义区域，承载 public API、文本值、验证状态和主题入口。 | `Text`、`Length`、`Status`、`SizeType` | `OtpLineEditToken`、SharedToken | stable |
| `cellList` | `PART_CellsHost` | 根据 `Length` 展示 cell 和 separator。 | `Length`、`Separator` | `CellGap`、`CellWidth*` | template-stable |
| `cell` | `OtpLineEditCell` | 展示单个字符、placeholder、mask、active/focus 和 error 状态。 | `Text`、`IsMasked`、`MaskChar` | `CellWidth`、LineEdit 输入字号 | internal-observable |
| `separator` | 分隔符容器 Border | 展示 cell 之间的分隔符字形，由 `OtpSeparatorPresenter` 按墨迹盒自动居中。 | `Separator`、`SeparatorTemplate` | — | template-stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/OtpLineEdit/Themes/OtpLineEditTheme.axaml`

```xml
<Grid Name="PART_RootPanel">
    <StackPanel>
        <ItemsControl Name="PART_CellsHost" />
        <InputClearIconButton Name="PART_ClearButton" />
        <ContentPresenter Name="PART_FormFeedBack" />
    </StackPanel>
</Grid>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
OtpLineEdit
  -> OtpLineEditCell (control theme, OtpLineEditCellTheme.axaml)
     -> PixelAlignedBorder#PART_Frame (template-stable)
        -> Panel (template-stable)
           -> ContentPresenter (internal-observable)
           -> Rectangle#PART_Caret (template-stable)
  -> OtpLineEdit (control theme, OtpLineEditTheme.axaml)
     -> Grid#PART_RootPanel (template-stable)
        -> StackPanel (template-stable)
           -> ItemsControl#PART_CellsHost (template-stable)
           -> InputClearIconButton#PART_ClearButton (template-stable)
           -> ContentPresenter#PART_FormFeedBack (template-stable)
  -> OtpTextBox (control theme, OtpTextBoxTheme.axaml)
     -> ScrollViewer#PART_ScrollViewer (template-stable)
        -> Panel (template-stable)
           -> TextBlock#PART_Placeholder (template-stable)
           -> InputTextPresenter#PART_TextPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `OtpLineEdit` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `OtpLineEditCell` | control theme | `OtpLineEditCellTheme.axaml` | OtpLineEdit | `Background`, `BorderBrush`, `BorderThickness`, `BoxShadow`, `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_Frame` | template node (PixelAlignedBorder) | `OtpLineEditCellTheme.axaml` | OtpLineEditCell | `Background`, `BorderBrush`, `BorderThickness`, `BoxShadow`, `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `OtpLineEditCellTheme.axaml` | OtpLineEditCell | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `OtpLineEditCellTheme.axaml` | OtpLineEditCell | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_Caret` | template node (Rectangle) | `OtpLineEditCellTheme.axaml` | OtpLineEditCell | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `OtpLineEdit` | control theme | `OtpLineEditTheme.axaml` | 用户代码 / 控件宿主 | `CellItems`, `ClearIcon`, `FormFeedback`, `IsEffectiveShowClearButton`, `IsFormFeedbackVisible`, `IsMotionEnabled` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_RootPanel` | template node (Grid) | `OtpLineEditTheme.axaml` | OtpLineEdit | `CellItems`, `ClearIcon`, `FormFeedback`, `IsEffectiveShowClearButton`, `IsFormFeedbackVisible`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `OtpLineEditTheme.axaml` | OtpLineEdit | `CellItems`, `ClearIcon`, `FormFeedback`, `IsEffectiveShowClearButton`, `IsFormFeedbackVisible`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CellsHost` | template node (ItemsControl) | `OtpLineEditTheme.axaml` | OtpLineEdit | `CellItems` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ClearButton` | template node (InputClearIconButton) | `OtpLineEditTheme.axaml` | OtpLineEdit | `ClearIcon`, `IsEffectiveShowClearButton`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FormFeedBack` | template node (ContentPresenter) | `OtpLineEditTheme.axaml` | OtpLineEdit | `FormFeedback`, `IsFormFeedbackVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `OtpTextBox` | control theme | `OtpTextBoxTheme.axaml` | OtpLineEdit | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `Cursor`, `FontSize`, `HorizontalContentAlignment` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ScrollViewer` | template node (ScrollViewer) | `OtpTextBoxTheme.axaml` | OtpTextBox | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `Cursor`, `FontSize`, `HorizontalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `OtpTextBoxTheme.axaml` | OtpTextBox | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `FontSize`, `HorizontalContentAlignment`, `PlaceholderForeground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Placeholder` | template node (TextBlock) | `OtpTextBoxTheme.axaml` | OtpTextBox | `FontSize`, `HorizontalContentAlignment`, `PlaceholderForeground`, `PlaceholderText`, `Text`, `TextAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TextPresenter` | template node (InputTextPresenter) | `OtpTextBoxTheme.axaml` | OtpTextBox | `CaretBlinkInterval`, `CaretBrush`, `CaretIndex`, `HorizontalContentAlignment`, `SelectionBrush`, `SelectionEnd` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_RootPanel` | `Panel` | 承载 cell、separator 和清除入口的根布局区域。 |
| `PART_CellsHost` | `ItemsControl` 或等价 host | 根据 `Length` 生成 cell 和 separator。 |
| `PART_ClearButton` | `InputClearIconButton` | 清除当前验证码文本。 |
| `PART_FormFeedBack` | `ContentPresenter` | Form feedback 内容承载。 |

## Pseudo Classes

稳定伪类：

| 伪类 | 语义 |
| --- | --- |
| `:focus` | 控件拥有键盘焦点，active cell 显示焦点视觉。 |
| `:error` | Avalonia native validation error 状态，由 `DataValidationErrors` 驱动。 |
| `:warning` | AtomUI warning 扩展状态，仅在无 native error 时生效。 |
| `:readonly` | 只读状态。 |
| `:filled` | 所有 cell 均有字符。 |
| `:empty` | `Text` 为空。 |

## State Flow

OtpLineEdit 的交互优先级：

```text
Disabled
> ReadOnly
> Native Error
> Form Error
> Form Warning
> Explicit Warning
> Explicit Error
> Focus
> PointerOver
> Normal
```

值归一化流程：

```text
User input / Paste / Form.SetValue / Binding update
      ↓
Formatter
      ↓
InputMode filter
      ↓
Length clamp
      ↓
Text
      ↓
Cell presentation + Form value changed + Completed check
```

输入行为：

- 单字符输入写入 active cell 对应位置，随后 active cell 移到下一个空 cell。
- 多字符输入或粘贴从 active cell 起顺序分发，超过 `Length` 的字符被丢弃。
- 点击后方空 cell 时，active cell 回到第一个空 cell，避免跳过中间空位。
- `Backspace` 在当前 cell 有值时清除当前字符；当前 cell 为空时回到前一位并清除。
- `Delete` 清除当前 cell，不改变后续字符顺序。
- `Left` / `Right` 在有效 cell 范围内移动 active cell。
- `Home` / `End` 分别定位到起始 cell 和最后一个可编辑 cell。

完成状态：

```text
IsCompleted = !string.IsNullOrEmpty(Text) && Text.Length == Length
```

`Completed` 只由从未完成状态进入完成状态的有效写入触发。外部绑定写入已完成文本时同样遵守完成事件语义，但重新写入相同文本不重复触发。

Form 集成以 `Text` 作为表单值。错误校验状态以 Avalonia `DataValidationErrors` 为真源；Form validator 产生的 error 写入 OtpLineEdit 根控件的 native validation 通道。内部 cell 不维护独立 error 状态，所有 cell 的 error 视觉来自根控件 effective status 投射。

## Theme and Token Boundaries

OtpLineEdit 使用“根输入控件 + cell presenter + separator + action 区”的视觉分层：

| 主题 | 职责 |
| --- | --- |
| `OtpLineEditTheme.axaml` | 根模板、cell host、清除入口、Form feedback、focus/error/warning/disabled 视觉投射。 |
| `OtpLineEditCellTheme.axaml` | 单个 cell 的输入表面、字符显示、placeholder、mask 和 active 状态。 |

视觉状态必须与 AtomUI 输入体系一致：

- `SizeType` 控制 cell 高度、宽度、字号和间距。
- `StyleVariant` 与 `LineEdit` 保持一致，支持 `Outlined`、`Filled`、`Borderless` 和 `Underlined` 四种输入表面；`Filled` 使用填充背景，`Borderless` 移除边框，`Underlined` 只保留下边线。
- `Status` 形成 `ExplicitStatus`，在无 native error 且无更高优先级 Form 状态时参与有效状态计算。
- `DataValidationErrors.HasErrors=true` 时，根控件和所有 cell 呈现 error 视觉。
- `IsMasked=true` 只改变字符展示，不改变 `Text`、复制、Form 值或 Completed 事件。
- `Separator` 只占据视觉布局位置，不参与输入、复制、验证或长度计算。

`OtpLineEditToken` 定义 cell 宽度和 cell 间距。separator 的默认外边距属于模板布局常量；边框、背景、focus shadow、disabled、error、warning 等输入表面语义由 `InputControlFrameTheme` 与 SharedToken 统一提供，cell 文本字号复用 SharedToken 的输入字号。

Token 边界：

OtpLineEdit 使用 `OtpLineEditToken` 表达 OTP 分格输入的专属布局语义。它只定义 cell 宽度和 cell 间距，不承载验证码文本、separator 内容、active cell、mask、placeholder、Form 状态、validation error、focus、hover、pressed 或 disabled 等运行时状态。

输入表面的颜色、边框、背景、focus shadow、disabled、error 和 warning 语义统一复用 `InputControlFrameTheme` 和 SharedToken。OtpLineEditToken 不复制这些已有输入体系 Token。

## Customization Boundaries

维护 OtpLineEdit 时必须保持以下不变量：

- `Text` 是唯一对外值源，内部 cell 不暴露独立绑定值。
- `Length` 只控制 cell 数量和最大文本长度，不把 separator 或 mask 计入长度。
- `Formatter`、`InputMode`、`Length` 的执行顺序必须稳定。
- 外部绑定、Form.SetValue、用户输入和粘贴必须进入同一归一化路径。
- `Completed` 只在从未完成进入完成时触发，不因视觉刷新或模板重建重复触发。
- `IsMasked` 和 `Separator` 只影响视觉，不改变 `Text`。
- error 状态必须以 `DataValidationErrors` 为最高优先级，不建立独立错误系统。
- Form reset、验证成功或重新验证只能清理由 Form 写入的 error，不能清除 ViewModel 或 binding 写入的 native validation error。
- 重新套用模板不能泄漏 cell 事件、清除按钮事件、binding 或 Form feedback 订阅。
- 动态 cell 生成必须由 `Length` 和 `Text` 派生，不能形成第二套验证码状态。

维护不变量：

内部重构必须保持以下不变量：

- `Text` 是唯一验证码值源，cell 不能保存可独立提交的值。
- `OtpLineEdit` 是唯一真实键盘焦点 owner，cell `TextBox` 只能作为 caret host，不允许通过 `Focus()` 主动抢焦点。
- `OtpLineEdit` 允许自身响应外部 `BringIntoView`，但不允许 cell/TextPresenter 发出的内部 `RequestBringIntoView` 冒泡到外层滚动容器。
- 所有写入路径必须复用同一归一化函数。
- `Length` 变化必须同步裁剪 `Text`、重建 cell projection 和更新 active cell。
- `Completed` 不能由模板刷新、mask 切换、separator 切换或 validation 状态刷新触发。
- native validation error 必须从根控件投射到全部 cell，不允许 cell 自行维护 error。
- `IsReadOnly=true` 时禁止输入、粘贴、删除和清除，但保留复制和焦点视觉。
- `IsEnabled=false` 时禁止全部交互入口。
- 模板重建不能泄漏旧按钮事件、binding 或 separator context；logical reattach 后模板按钮交互保持有效，Form feedback subscription 必须恢复。
- separator 和 mask 不参与 `Text`、Form value、复制、验证和 completed 判断。
- 粘贴分发必须通过 overlay 单次写入，避免视觉闪烁和状态中间态暴露。
