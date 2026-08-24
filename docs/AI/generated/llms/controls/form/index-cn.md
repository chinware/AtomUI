# Form

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Form 是 AtomUI 桌面数据录入体系中的表单容器，用于组织多个 `FormItem`，向子输入控件传播尺寸、状态、样式和验证配置，并统一处理表单值收集、验证、提交、重置和动态表单项删除。

Form 的职责是把输入控件接入表单语义。它不负责业务对象持久化、远程提交协议、权限控制、复杂 ViewModel 状态管理或跨页面流程编排；这些能力应由应用层或业务层承载。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Form` |
| 状态 | Stable |

## 何时使用

Form 的设计语言来自表单布局、标签语义、输入控件状态和验证反馈的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 布局 | 表单项在水平、垂直或行内布局中排列。 | `FormLayout`、`LabelColInfo`、`WrapperColInfo`。 |
| 标签 | 标签文本、冒号、必填标记、可选标记和 tooltip 表达字段语义。 | `LabelText`、`IsShowColon`、`RequiredMark`、`Tooltip`。 |
| 输入表面 | 表单统一向子控件传播尺寸和输入外观。 | `SizeType`、`StyleVariant`、`IsMotionEnabled`。 |
| 验证 | 表单项按触发时机和策略执行验证器。 | `ValidateTrigger`、`ValidateStrategy`、`ValidateDebounce`。 |
| 反馈 | 错误、警告、成功和验证中状态投射到输入控件及反馈图标。 | `ValidateStatus`、`ValidateResult`、`IsValidateFeedbackEnabled`。 |
| 操作 | 提交、重置和动态删除由专用按钮或事件进入 Form。 | `SubmitButton`、`ResetButton`、`ItemDeleteButton`。 |

`SizeType=Custom` 不是 Form 的第四套专属 token。Form 会把 `CustomizableSizeType` 传递给支持 `ICustomizableSizeTypeAware` 的子控件；对于只支持旧 `ISizeTypeAware` 的子控件，`Custom` 按 `Middle` 映射，保持旧控件的尺寸基线稳定。

## 公共 API

Form 的公共契约由 Form、FormItem、FormItemDecorator、FormValidateFeedback、SubmitButton、ResetButton、验证器和表单感知接口组成。

### 3.1 Form 容器契约

| API | 语义 |
| --- | --- |
| `FormLayout` | 控制表单项整体排列。`Horizontal` / `Vertical` 使用纵向表单项列表，`Inline` 使用横向表单项列表。 |
| `LabelColInfo` / `WrapperColInfo` | 水平布局下标签列和内容列的响应式 GridLength 信息。 |
| `LabelAlign` / `LabelWrapping` | 标签对齐和换行策略。 |
| `IsShowColon` | 水平布局下是否显示标签冒号。 |
| `RequiredMark`、`CustomRequireMark`、`CustomOptionalMark` | 必填或可选标记策略。 |
| `SizeType` | 表单统一尺寸密度，支持 `Large/Middle/Small/Custom`。 |
| `StyleVariant` | 传递给支持输入外观变体的子控件。 |
| `IsMotionEnabled` | 传递给支持动效开关的子控件。 |
| `ValidateTrigger` | 表单默认验证触发时机，默认 `OnChanged`，即字段值变化时触发验证。 |
| `IsValidateFeedbackEnabled` | 是否创建并传递验证反馈控件。 |
| `InitialValues` | 表单加载时按 `FieldName` 写入初始值。 |
| `IsFormValid` | 基于表单项验证状态聚合的只读有效性状态。 |
| `Validate()` / `ValidateAsync()` | 手动验证当前可验证表单项。 |
| `Submit()` | 验证成功后收集 `FormValues` 并触发提交。 |
| `Reset()` | 清空表单项值、取消未完成验证并重置验证状态。 |
| `SetFormValues()` | 按 `FieldName` 批量写入表单项值。 |
| `DeleteFormItem()` | 删除指定表单项。 |

事件契约：

| 事件 | 语义 |
| --- | --- |
| `AboutToValidate` | 表单验证开始前触发。 |
| `Validated` | 验证结果需要对外通知时触发；独立验证失败时携带消息，提交成功时携带 values。 |
| `Submitted` | 提交成功后触发，携带收集到的 `FormValues`。 |
| `ItemValueChanged` | 任一表单项值变化后触发，并通知其他表单项。 |
| `ResetCompleted` | 重置流程完成后触发。 |

### 3.2 FormItem 契约

`FormItem` 是单个字段的表单项容器。它承载标签、字段名、验证器、帮助文案、额外内容、内容控件和当前验证状态。

稳定契约：

- `FieldName` 是表单值读写和验证消息聚合的字段键。
- `Content` 必须实现 `IFormItemAware`，除非该表单项显式关闭内容类型验证。
- `Validators` 按 `ValidateStrategy` 执行；error 结果写入内容控件的 `DataValidationErrors`，并同步投射到 `ValidateStatus`、`ValidateResult`、错误消息和 feedback。
- `Warning`、`Validating` 和 `Success` 是 Form 扩展状态，写入内容控件的 `FormStatus`；它们不写成 `DataValidationErrors` error。
- `ValidateTrigger` 可以继承 Form 默认值，也可以在 FormItem 上覆盖。
- `ValidateDebounce` 只影响延迟触发的表单项验证，不改变手动验证和提交验证的契约。
- `Help` 与验证消息共同决定表单项辅助信息区域是否保留空间。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_BodyLayout` | `Grid` | FormItem 标签列和内容列布局。 |
| `PART_LabelLayout` | `Panel` | 标签、必填标记、tooltip、可选标记和冒号承载。 |
| `PART_ContentLayout` | `Panel` | 输入内容、Extra、错误消息和帮助文本承载。 |

### 3.3 子控件接入契约

表单内容控件通过以下接口接入 Form：

| 接口 | 职责 |
| --- | --- |
| `IFormItemAware` | 提供值读写、清空、值变化通知和扩展验证状态通知；error 真源仍是内容控件的 `DataValidationErrors`。 |
| `IFormItemFeedbackAware` | 接收 `FormValidateFeedback` 控件，用于在输入框内部展示反馈图标。 |
| `ICustomizableSizeTypeAware` | 接收 `CustomizableSizeType`，包含 `Custom` 尺寸。 |
| `ISizeTypeAware` | 旧尺寸接口；Form 将 `Custom` 映射为 `Middle` 后传递。 |
| `IInputControlStyleVariantAware` | 接收 Form 的 `StyleVariant`。 |
| `IMotionAwareControl` | 接收 Form 的 `IsMotionEnabled`。 |

`FormItemDecorator` 用于把一个或多个输入控件组合成单个表单项内容。它自身实现 Form 接入接口，并把 value、`FormStatus`、feedback、size、motion 和 style variant 转发给子控件；涉及 error 时必须保持 Form-owned `DataValidationErrors` 与子控件 `NativeValidationStatus` 目标一致。

### 3.4 按钮契约

`SubmitButton` 和 `ResetButton` 继承 Button，通过冒泡路由事件与最近的 Form 协作。`SubmitButton.IsWatchValidateResult=true` 时会查找逻辑树或视觉树中的 Form，并根据 `Form.IsFormValid` 控制自身 `IsEnabled`。

## 事件与命令

事件契约：
| 事件 | 语义 |
`SubmitButton` 和 `ResetButton` 继承 Button，通过冒泡路由事件与最近的 Form 协作。`SubmitButton.IsWatchValidateResult=true` 时会查找逻辑树或视觉树中的 Form，并根据 `Form.IsFormValid` 控制自身 `IsEnabled`。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

- `controlgallery/AtomUIGallery/ShowCases/DataEntry/Form/Views/FormShowCase.axaml`

## 状态模型

Form 的核心状态流：

```text
Form config
  → Prepare / sync FormItem style values
  → FormItem content via IFormItemAware
  → value changed / blur / submit validation trigger
  → FormItem validators
  → DataValidationErrors for error
  → ValidateStatus / ValidateResult / feedback / messages as projection
  → Form IsFormValid aggregation
  → SubmitButton watch state and submit result
```

验证触发模型：

- `OnChanged` 是 Form 的默认触发时机；内容控件触发 `IFormItemAware.ValueChanged` 后按 `ValidateDebounce` 延迟验证，使提交后或编辑中的错误能够随输入及时更新。
- `OnSubmit` 仅在手动验证或提交时验证，适合显式要求只在提交入口展示错误的表单。
- `OnBlur` 在 FormItem 失去焦点时按 `ValidateDebounce` 延迟验证。
- 手动 `Validate()`、`ValidateAsync()` 和 `Submit()` 直接进入验证流程，不依赖输入变化触发。

验证策略模型：

| 策略 | 行为 |
| --- | --- |
| `StopWhenFirstFailed` | 顺序执行验证器，遇到第一个 error 后停止，同时保留 error 前已产生的 warning。 |
| `Sequential` | 顺序执行所有验证器。 |
| `Parallel` | 并行执行所有验证器并聚合结果。 |

验证结果模型：

- `Error` 会使 Form 聚合为无效状态，并阻止 `Submit()` 继续提交。
- error 状态以内容控件的 `DataValidationErrors.HasErrors` 为最高优先级；Form validators 产生的 error 也写入同一 native validation 通道。
- `Warning` 会展示警告状态和警告消息，但 Form 聚合只把 error 作为提交阻断条件。
- `Validating` 和 `Default` 在 `IsFormValid` 聚合中不视为有效完成状态。
- 重置会取消未完成验证、清空 Form-owned 消息并把表单项扩展状态恢复为 `Default`；它只能清理由 Form 写入的 validation error，不能清掉 binding 或 ViewModel 写入的 native error。

提交与重置模型：

```text
Submit()
  → cancel previous form validation run
  → ValidateAsync()
  → if success, collect FormValues by FieldName
  → Validated(success, values)
  → Submitted(values)
```

```text
Reset()
  → cancel pending form validation
  → mark IsResetting
  → FormItem.ResetItemValue()
  → NotifyReset()
  → IsFormValid = false
  → ResetCompleted
  → clear IsResetting on dispatcher
```

## 主题与 Design Token

Form 视觉由 Form 根模板、FormItem 模板、FormValidateFeedback 模板和按钮主题协作完成。

| 主题或资源 | 职责 |
| --- | --- |
| `FormTheme.axaml` | Form 根模板、ItemsPresenter、表单项排列方向、反馈模板和默认操作图标。 |
| `FormItemTheme.axaml` | 标签、内容、辅助信息、删除按钮、必填标记、冒号和尺寸 selector。 |
| `FormItemDecoratorTheme.axaml` | 组合表单控件的水平内容和 Extra 布局。 |
| `FormValidateFeedbackTheme.axaml` | 根据 `ValidateStatus` 选择成功、警告、错误或验证中反馈内容。 |
| `SubmitButtonTheme.axaml` | 提交按钮默认 primary 风格和本地化文案。 |
| `ResetButtonTheme.axaml` | 重置按钮默认 default 风格和本地化文案。 |
| `FormToken` | 标签颜色、必填标记颜色、标签字体和表单项间距。 |
| `SharedToken` | 输入高度、图标尺寸、状态色、字体和通用 spacing。 |

视觉兼容边界：

- `FormItem` 标签列和内容列必须继续由 `PART_BodyLayout`、`PART_LabelLayout`、`PART_ContentLayout` 协作。
- `FormLayout=Inline` 必须保持横向表单项排列和 `InlineItemSpacing`。
- `FormLayout=Horizontal/Vertical` 必须保持纵向表单项排列。
- `SizeType=Custom` 的 FormItem 标签最小高度默认走 `Middle` 分支，子输入控件的自定义尺寸由子控件自身属性决定。

Token 来源：

FormToken 是 Form 的控件级 Token scope，描述标签视觉、必填标记、冒号间距和表单项布局间距。输入控件自身高度、边框、状态色、图标尺寸、字体基础值和通用 spacing 来自 SharedToken 或对应输入控件 Token。

FormToken 不承载以下状态：

- `FieldName`、`InitialValues`、提交 values 或业务数据。
- `ValidateStatus`、`ValidateResult`、错误消息、警告消息或验证中状态。
- `FormLayout`、`RequiredMark`、`ValidateTrigger`、`ValidateStrategy` 等实例配置。
- 动态表单项数量、删除按钮显示状态或响应式 breakpoint 当前值。

## AOT 与裁剪注意事项

Form 不依赖运行时反射扫描控件能力。控件协作通过显式接口、固定 template part、路由事件、Avalonia property 和 XAML 模板完成。

资源和生命周期边界：

- FormItem 内容变化时释放旧内容的 `ValueChanged` 订阅、feedback 引用和尺寸/style/motion relay binding。
- FormItem detach 时释放 media breakpoint 订阅并取消验证。
- FormItem feedback 变化或禁用 feedback 时释放旧 feedback 状态 relay binding。
- Form 级验证运行通过 `_validationTokenSource` 取消旧提交或验证流程。
- SubmitButton watch 模式在 logical/visual detach 时释放 Form `IsFormValid` 订阅。

AOT 边界：

- `FormToken` 通过 token generator 显式注册，生成 token kind 和资源扩展。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护。
- Form 验证器是显式类型和接口调用，不依赖动态成员发现。
- AXAML 模板、DataTemplate 和 Form validator provider 是显式 XAML 入口，不依赖运行时 API 扫描。

## 源码索引

主要源码：

- `src/AtomUI.Controls/Form/FormEnums.cs`：布局、必填标记、验证触发、验证策略和验证状态枚举。
- `src/AtomUI.Controls/Form/IForm.cs`：Form 容器操作接口。
- `src/AtomUI.Controls/Form/IFormItem.cs`：FormItem 字段级值读写和验证接口。
- `src/AtomUI.Controls/Form/IFormItemAware.cs`：输入控件接入 Form 的值、清空、状态和事件接口。
- `src/AtomUI.Controls/Form/IFormValidateFeedback.cs`、`FormValidateFeedback.cs`：验证反馈控件契约和基础实现。
- `src/AtomUI.Controls/Form/IFormValidator.cs`、`Validators/*.cs`：验证器契约和内置验证器。
- `src/AtomUI.Controls/Form/FormValues.cs`、`FormValidateMessage.cs`、事件参数类型：表单值和验证事件数据结构。
- `src/AtomUI.Desktop.Controls/Form/Form.cs`：桌面 Form 容器、表单项配置同步、验证聚合、提交、重置、动态删除和 ItemsControl 容器逻辑。
- `src/AtomUI.Desktop.Controls/Form/FormItem.cs`：字段级布局、模板接入、内容接入、Form 配置接收、反馈创建和响应式列宽。
- `src/AtomUI.Desktop.Controls/Form/FormItem.Validation.cs`：FormItem 验证运行、debounce、取消、策略执行、结果应用和消息 inlines 构建。
- `src/AtomUI.Desktop.Controls/Form/FormItemDecorator.cs`：组合输入控件的表单能力转发。
- `src/AtomUI.Desktop.Controls/Form/FormSizeTypeBindingHelper.cs`：`CustomizableSizeType` 到新旧尺寸接口的转发规则。
- `src/AtomUI.Desktop.Controls/Form/SubmitButton.cs`、`ResetButton.cs`：表单操作按钮和路由事件。
- `src/AtomUI.Desktop.Controls/Form/FormToken.cs`：Form 控件 Token。
- `src/AtomUI.Desktop.Controls/Form/Themes/*.axaml`：Form、FormItem、Decorator、Feedback 和操作按钮主题。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/form/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/form/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/form/token.md`
- 变更记录：`docs/controls/desktop/data-entry/form/changelog.md`
- 语义结构：`./semantic-cn.md`
