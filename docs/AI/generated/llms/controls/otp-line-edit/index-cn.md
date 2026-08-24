# OtpLineEdit

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

OtpLineEdit 是 AtomUI 桌面数据录入体系中的一次性验证码输入控件，用于短信验证码、邮件验证码、二次验证代码、恢复代码短段输入等场景。它以单个文本值作为数据契约，以多个视觉 cell 表达逐位输入体验，并接入 AtomUI 的输入尺寸、输入表面、Form、native validation error、主题 Token 和 Gallery 文档体系。

OtpLineEdit 不负责发送验证码、倒计时、后端校验、风险控制、二维码扫描、密钥绑定或业务 Captcha 逻辑。业务侧的“发送验证码”按钮、倒计时和接口状态应组合在 Form 或业务控件中，验证码输入本身由 OtpLineEdit 承担。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml` |
| 状态 | Design |

## 何时使用

OtpLineEdit 的设计语言来自“单一验证码值”和“分格输入表面”的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 验证码语义 | 用户输入一个短文本验证码，提交时作为一个整体值。 | `Text`、`Length`、`Completed`。 |
| 分格表达 | 每个字符拥有独立视觉 cell，增强位数感知和粘贴反馈。 | cell、focus cell、separator。 |
| 输入约束 | 控制允许输入的字符范围和写入前格式化。 | `InputMode`、`Formatter`、`MaxLength` 派生自 `Length`。 |
| 安全显示 | 可用掩码隐藏每个字符，但不改变提交值。 | `IsMasked`、`MaskChar`。 |
| 输入密度 | 控件在表单、弹窗和验证流程中的尺寸等级。 | `SizeType=Large/Middle/Small/Custom`。 |
| 输入表面 | cell 边框和背景的视觉强度。 | `Outlined`、`Filled`、`Borderless`、`Underlined`。 |
| 反馈状态 | 输入的校验和业务状态。 | `Default`、`Error`、`Warning`，error 以 `DataValidationErrors` 为真源。 |

`OTP` 作为产品文案使用全大写；代码类型和 API 使用 `.NET` PascalCase 缩写形式 `Otp`，例如 `OtpLineEdit`、`OtpLineEditInputMode`、`OtpLineEditCompletedEventArgs`。

## 公共 API

OtpLineEdit 的公共 API 以文本值、长度、输入约束、显示辅助和输入外观为核心。`Text` 是唯一对外值源，支持双向绑定；视觉 cell 不暴露独立值属性。

基础值 API：

| API | 语义 |
| --- | --- |
| `Text` | 当前验证码文本，支持双向绑定，是 Form、验证和完成事件的唯一值源。 |
| `Length` | 验证码位数，默认 `6`；控件按该值生成 cell，并把 `Text` 限制在该长度内。 |
| `PlaceholderText` | 空 cell 的占位提示；多字符占位按 cell 展示策略分配。 |
| `IsReadOnly` | 允许查看和复制当前验证码文本，但禁止编辑、删除、粘贴和清除。 |
| `IsAllowClear` | 是否显示清除入口；清除动作必须进入统一 `Clear()` 语义。 |

输入约束 API：

| API | 语义 |
| --- | --- |
| `InputMode` | 输入字符范围，支持文本、数字、字母数字等稳定模式。 |
| `Formatter` | 写入 `Text` 前的归一化函数，用于大写转换、空白裁剪或业务字符过滤。 |
| `IsMasked` | 是否以掩码显示已输入字符。 |
| `MaskChar` | 掩码字符，默认使用密码输入常见圆点。 |

视觉与布局 API：

| API | 语义 |
| --- | --- |
| `SizeType` | 输入尺寸密度，类型为 `CustomizableSizeType`。 |
| `StyleVariant` | 输入表面样式，复用 `InputControlStyleVariant`。 |
| `Status` | 显式输入反馈状态；最终视觉由 `InputControlFrame.EffectiveStatus` 计算，native validation error 以 `DataValidationErrors` 为唯一真源。 |
| `Separator` | 分隔符内容，仅参与视觉展示，不进入 `Text`。 |
| `SeparatorInterval` | 分隔符间隔，例如 `3` 表示 `123-456`。 |
| `SeparatorTemplate` | 分隔符内容模板。 |

事件与方法：

| API | 语义 |
| --- | --- |
| `Completed` | 当归一化后的 `Text.Length == Length` 时触发，事件参数携带完整文本。 |
| `Clear()` | 清空 `Text`，重置 active cell 到起始位置，并通知 Form value changed。 |
| `Focus()` | 使控件进入输入状态，active cell 定位到第一个空 cell；全部填满时定位到最后一个 cell。 |

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_RootPanel` | `Panel` | 承载 cell、separator 和清除入口的根布局区域。 |
| `PART_CellsHost` | `ItemsControl` 或等价 host | 根据 `Length` 生成 cell 和 separator。 |
| `PART_ClearButton` | `InputClearIconButton` | 清除当前验证码文本。 |
| `PART_FormFeedBack` | `ContentPresenter` | Form feedback 内容承载。 |

稳定伪类：

| 伪类 | 语义 |
| --- | --- |
| `:focus` | 控件拥有键盘焦点，active cell 显示焦点视觉。 |
| `:error` | Avalonia native validation error 状态，由 `DataValidationErrors` 驱动。 |
| `:warning` | AtomUI warning 扩展状态，仅在无 native error 时生效。 |
| `:readonly` | 只读状态。 |
| `:filled` | 所有 cell 均有字符。 |
| `:empty` | `Text` 为空。 |

## 事件与命令

| `Text` | 当前验证码文本，支持双向绑定，是 Form、验证和完成事件的唯一值源。 |
事件与方法：
| `Completed` | 当归一化后的 `Text.Length == Length` 时触发，事件参数携带完整文本。 |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

- `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml`

## 状态模型

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

## 主题与 Design Token

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

Token 来源：

OtpLineEdit 使用 `OtpLineEditToken` 表达 OTP 分格输入的专属布局语义。它只定义 cell 宽度和 cell 间距，不承载验证码文本、separator 内容、active cell、mask、placeholder、Form 状态、validation error、focus、hover、pressed 或 disabled 等运行时状态。

输入表面的颜色、边框、背景、focus shadow、disabled、error 和 warning 语义统一复用 `InputControlFrameTheme` 和 SharedToken。OtpLineEditToken 不复制这些已有输入体系 Token。

## AOT 与裁剪注意事项

资源和生命周期边界：

- 动态生成 cell 是 `Length` 驱动的控件内部结构，必须由 owner 管理创建、复用和释放。
- cell 输入由根控件 routed input 处理，不为每个 cell 建立 owner 外部订阅。
- clear button click 订阅必须在新模板接入前解绑旧按钮。
- Form feedback subscription 必须在 feedback 对象变化和 logical detach 时释放，并在 logical attach 时重新建立。
- separator context 不持有 owner 强引用，避免模板内容长期保留控件实例。

性能边界：

- `Text` 变化只进行 `Length` 范围内的 projection，不扫描外部视觉树。
- 粘贴和多字符输入只写入一次 `Text`，不逐 cell 触发多次 public value changed。
- `Length` 合理限制为短验证码输入场景；超长分格文本不是 OtpLineEdit 的职责。
- `Formatter` 由业务提供时应保持同步、快速和无副作用；控件不为 formatter 引入异步状态。

AOT 边界：

- 不通过运行时反射扫描 public API、template part 或 token。
- Token 类型通过 generator 显式注册。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护，不依赖 NativeAOT 不友好的运行时成员扫描。
- separator template 和 formatter 不依赖运行时动态类型发现。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/OtpLineEdit/OtpLineEdit.cs`：公共 API、Avalonia 属性注册、事件、Form 接口、状态入口和模板生命周期。
- `src/AtomUI.Desktop.Controls/OtpLineEdit/OtpLineEditCell.cs`：内部 cell 控件，承载单字符显示、placeholder、active、mask 和事件回调。
- `src/AtomUI.Desktop.Controls/OtpLineEdit/OtpLineEditSeparatorContext.cs`：separator 模板上下文，提供前后 cell index 和 display index。
- `src/AtomUI.Desktop.Controls/OtpLineEdit/OtpLineEditToken.cs`：cell 宽度和 cell 间距 Token。
- `src/AtomUI.Desktop.Controls/OtpLineEdit/Themes/OtpLineEditTheme.axaml`：根模板、cell host、清除按钮和 feedback 区。
- `src/AtomUI.Desktop.Controls/OtpLineEdit/Themes/OtpLineEditCellTheme.axaml`：cell 的字符显示、mask、placeholder、active/focus/error 状态。

测试与 Gallery：

- `tests/AtomUI.Desktop.Controls.Tests/OtpLineEdit/OtpLineEditBehaviorTests.cs`：值归一化、输入、粘贴、删除、导航和 completed。
- `tests/AtomUI.Desktop.Controls.Tests/OtpLineEdit/OtpLineEditFormValueTests.cs`：Form value、reset、validation 和 `DataValidationErrors`。
- `tests/AtomUIGallery.Tests/ShowCases/LineEditShowCasePageTests.cs`：LineEdit 页面内的 OtpLineEdit 示例入口、v6.0.8 标记和 Form validator Provider 写法。
- `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml`：Gallery 示例和源码片段来源。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/otp-line-edit/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/otp-line-edit/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/otp-line-edit/token.md`
- 变更记录：`docs/controls/desktop/data-entry/otp-line-edit/changelog.md`
- 语义结构：`./semantic-cn.md`
