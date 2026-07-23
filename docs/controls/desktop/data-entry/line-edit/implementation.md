# LineEdit 桌面版实现原理

本文档描述 LineEdit 桌面版输入家族的内部状态同步、模板接入、清除/reveal/字数统计、SearchEdit、TextArea resize、Form 和 CompactSpace 集成。公共设计与 API 契约见 [LineEdit 桌面版架构设计](overview.md)，Token 语义见 [LineEdit Token 设计](token.md)，变化记录见 [LineEdit Changelog](changelog.md)。

## 1. 实现定位

LineEdit 家族的实现以 Avalonia `TextBox` 为文本编辑内核，AtomUI 负责输入壳体、状态伪类、Addon、内部 action、Form、CompactSpace 和多行扩展。实现文档聚焦 AtomUI 增强层，不重新说明 Avalonia 文本编辑、选择和滚动算法。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/Input/TextBox.cs`：AtomUI 基础 TextBox，提供 `SizeType`、清除按钮、密码 reveal、字数统计、Form 和 CompactSpace。
- `src/AtomUI.Desktop.Controls/Input/LineEdit.cs`：标准单行输入框，提供 `StyleVariant`、`Status`、外部 AddOn、内部右侧内容绑定和 Form 扩展状态映射。
- `src/AtomUI.Desktop.Controls/Input/SearchEdit.cs`：搜索输入框，提供搜索按钮样式、搜索按钮文本、加载态和搜索点击事件。
- `src/AtomUI.Desktop.Controls/Input/TextArea.cs`：多行输入框，提供固定行数、自动高度、resize、清除、字数统计、Form 和 feedback。
- `src/AtomUI.Desktop.Controls/Input/InputTextPresenter.cs`：输入文本 presenter，处理 Avalonia 12 selection foreground 缓存刷新。
- `src/AtomUI.Desktop.Controls/Input/SearchEditDecoratedBox.cs`：SearchEdit 输入壳体与搜索按钮协作。
- `src/AtomUI.Desktop.Controls/Input/TextAreaDecoratedBox.cs`：TextArea 输入壳体、scroll viewer 和 resize 相关协作。
- `src/AtomUI.Desktop.Controls/Input/ResizeHandle.cs`：TextArea resize 拖拽入口。
- `src/AtomUI.Desktop.Controls/Input/TextBoxToken.cs`：基础 TextBox 边框、padding、hover/focus 和 shadow Token。
- `src/AtomUI.Desktop.Controls/Input/LineEditToken.cs`：单行输入字号 Token。
- `src/AtomUI.Desktop.Controls/Input/TextAreaToken.cs`：TextArea 字号、右侧 padding 和 resize Token。
- `src/AtomUI.Desktop.Controls/Input/Themes/*.axaml`：TextBox、LineEdit、SearchEdit、TextArea 和内部按钮主题。

## 3. 核心类职责

`TextBox` 是 AtomUI 文本输入基础类。它不处理输入表面 variant/status；它只处理尺寸、清除按钮、reveal、字数统计、Form、feedback 可见性和 CompactSpace 基础圆角。

`LineEdit` 在 `TextBox` 基础上加入输入表面和验证视觉。错误状态以 `DataValidationErrors` 为真源，`Status` 仅表达手动 error/warning 视觉请求或 Form warning 等扩展状态；它把外部 AddOn、内部右侧内容、clear/reveal/form/count presenter 的运行时绑定装配到模板。

`SearchEdit` 在 `LineEdit` 基础上把搜索按钮加入输入壳体。搜索按钮点击由 `SearchEditDecoratedBox` 通知 `SearchEdit`，再由控件抛出 `SearchButtonClick` 路由事件。

`TextArea` 独立继承 Avalonia `TextBox`，因为多行输入需要不同模板、scroll viewer 接入、固定行数测量和 resize 流程。它复用 LineEdit 家族的状态、尺寸、清除、字数统计和 Form 模型。

## 4. 状态与数据流

文本值流：

```text
User input / Form.SetValue
      ↓
Text
      ↓
TextChanged class handler
      ↓
CountText + Form ValueChanged
```

清除按钮流：

```text
IsAllowClear / IsReadOnly / Text / AcceptsReturn
      ↓
IsEffectiveShowClearButton
      ↓
PART_ClearButton.IsVisible
      ↓
Click → Clear()
```

Form 状态流：

```text
Form.SetValue(object?) → Text
TextChanged            → IFormItemAware.ValueChanged
Form.GetValue()        → Text
Form.ClearValue()      → Text = null
DataValidationErrors   → native error visual + AddOn effective error state
ValidateStatus         → Warning/Success/Validating extension state
Form feedback control  → FormFeedback + IsFormFeedbackVisible
```

状态伪类：

- native validation error 设置 Avalonia `:error`。
- 显式 `Status=Error` 只能作为无 native error 时的手动错误视觉请求，不能覆盖或清除 Avalonia `:error`。
- `Status=Warning` 只在无 native validation error 时设置 `:warning`，native error 出现后 warning 视觉必须退让。
- `StyleVariant=Outlined/Filled/Borderless` 设置输入壳体相关伪类。
- `Underlined` 作为 style variant 属性参与 selector，不需要额外专属伪类。

## 5. 生命周期与模板接入

`TextBox.OnInitialized` 在未设置 `ClearIcon` 时写入默认 `CloseCircleFilled`。`TextBox` 构造时注册 `TextBoxToken` resource scope，`LineEdit` 构造时注册 `LineEditToken` resource scope；`TextArea` 当前也注册 `LineEditToken` scope，同时主题中的 TextArea 字号和 resize 资源使用 `TextAreaTokenResource`。

`OnApplyTemplate` 规则：

- 旧清除按钮 click 订阅必须解除，再绑定新 `PART_ClearButton`。
- `TextBox` 每次套用模板都重新获取 `InnerBoxDecorator`，先禁用初始 transitions，再在 dispatcher 队列中恢复，避免模板初始资源注入触发边框动画。
- `LineEdit` 每次套用模板都重新获取 `PART_AddOnDecoratedBox`，用于 CompactSpace 边框厚度计算。
- `LineEdit` / `TextArea` 的 `_contentRightAddOnBindings` 在重新套用模板时先 dispose，再绑定 clear/reveal/form/inner-right/count presenter。
- `SearchEdit` 获取 `SearchEditDecoratedBox` 后设置 `OwningSearchEdit`，由 decorated box 回调搜索事件。
- `TextArea` 获取 `TextAreaDecoratedBox` 后设置 `Owner`，获取 `ResizeHandle` 后设置 `Owner`。
- Form feedback 订阅在 `FormFeedback` 变化时替换，在 detach 时释放。

模板 part 属于主题契约。需要调整内部视觉时，应优先在 AXAML 中维护静态模板和 selector，不把 clear/reveal/search/resize 视觉搬到 C# 动态创建。

## 6. 交互与事件处理

清除按钮点击进入 `NotifyClearButtonClicked()`，默认调用 `Clear()`。派生控件可以重写 hook，但必须保持 `Text`、Form value 和字数统计同步。

密码 reveal 通过 `RevealButton.IsChecked` 与 `RevealPassword` 双向绑定完成。`IsEnableRevealButton` 只控制按钮可见性，不改变 `PasswordChar` 自身语义。

SearchEdit 搜索点击流程：

```text
Search button click
  → SearchEditDecoratedBox
  → SearchEdit.NotifySearchButtonClicked()
  → if !IsOperating raise SearchButtonClick
```

TextArea resize 流程：

```text
Pointer pressed → NotifyAboutToResize()
Pointer moved   → NotifyResizing(delta)
Pointer released→ NotifyResizeCompleted()
```

`NotifyAboutToResize()` 记录原始高度，并根据 `MinLines` / `MaxLines` 和当前 scroll viewer 垂直开销计算可拖拽高度范围。

## 7. 内部算法与关键流程

### 7.1 字数统计

`IsShowCount=true` 时，输入变化更新 `CountText`，格式为 `当前长度 / MaxLength`。`MaxLength` 来自 Avalonia `TextBox`，当业务需要无限长度时仍应评估显示文案是否合理。

### 7.2 CompactSpace 边框厚度

`TextBox` 默认根据 `CompactSpaceOrientation` 返回当前 `BorderThickness.Left` 或 `BorderThickness.Top`。`LineEdit` 在使用 `AddOnDecoratedBox` 且 `StyleVariant=Outlined` 时返回 decorated box 的内部边框厚度；非 outlined 或未接入 decorated box 时返回 `0`。

### 7.3 TextArea 固定行数测量

TextArea 在 `IsAutoSize=false`、`Lines>0` 且未显式设置 `Height` 时，使用 `TextLayout` 计算指定行数的文本高度，再叠加 scroll viewer 与 text presenter 的垂直空间，写入 scroll viewer 的 `MinHeight` / `MaxHeight`。

### 7.4 TextArea resize 边界

TextArea resize 以控件当前 `Bounds.Height` 为起点。拖拽时高度被限制在 `MinLines` 和 `MaxLines` 对应的 scroll viewer 高度范围内；若 `MaxLines<=0`，最大值为正无穷。

### 7.5 Selection foreground 刷新

`InputTextPresenter` 在 `SelectionStart`、`SelectionEnd`、`SelectionForegroundBrush` 和 `ShowSelectionHighlight` 变化时调用 `InvalidateTextLayout()`，用于避免 Avalonia 12 文本 run 缓存导致选中文本前景色滞留。

## 8. 资源、性能与 AOT 边界

LineEdit 家族不依赖运行时反射发现模板结构。跨模板协作通过稳定 part、`TemplateBinding`、`BindUtils.RelayBind`、接口和 owner 引用完成。

资源和生命周期边界：

- `_contentRightAddOnBindings` 必须在重新套用模板前 dispose。
- `_feedbackStatusSubscription` 必须在 `FormFeedback` 变化和 detach 时释放。
- clear button click 订阅必须在新模板接入前解绑旧按钮。
- TextArea resize 不创建全局订阅；拖拽状态保存在控件实例字段中。
- Token 只表达尺寸、字体、padding 和 resize 视觉语义，不承载文本值、清除状态、Form 状态或搜索运行状态。

AOT 边界：

- Token 类型通过 generator 显式注册。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护，不依赖运行时反射扫描。
- 文档中描述的 template part 名称应与 AXAML 和 C# 查找代码保持一致。

## 9. 维护不变量

内部重构必须保持以下不变量：

- `TextChanged` 继续驱动字数统计和 Form value changed。
- 清除按钮可见性不在 AXAML 与 C# 中形成相互冲突的状态源。
- `IsCustomFontSize=true` 不能被 SizeType 字体样式覆盖。
- `LineEdit` 的 error 视觉必须优先响应 `DataValidationErrors`；`Status` 只作为无 native error 时的手动视觉请求，并继续支持 warning 扩展视觉。
- `SearchEdit.IsOperating=true` 必须阻止重复搜索事件。
- `TextArea` 的 fixed lines、auto-size 和 resize 不互相覆盖高度状态。
- 重新套用模板不能泄漏旧按钮 click、旧 binding 或旧 Form feedback 订阅。
- TextPresenter margin 是输入模板视觉契约；宽度计算问题应在使用方模板或测量源头解决，不在业务控件中加入隐藏补偿。

## 10. 测试与验证

验证范围：

- `LineEditShowCasePageTests` 覆盖 Gallery 页面结构、示例 snapshot 和源码片段入口。
- 清除按钮：空文本、非空文本、read-only、TextArea 和 single-line 差异。
- SizeType：Large/Middle/Small/Custom 字号、高度、line height 和 `IsCustomFontSize` 优先级。
- Variant/status：Outlined、Filled、Borderless、Underlined、Error、Warning、focus、disabled。
- AddOn：外部 left/right AddOn、内部 left/right content、Form feedback、字数统计顺序。
- SearchEdit：Default/Primary 搜索按钮、loading、搜索事件和 disabled。
- TextArea：Lines、MinLines、MaxLines、IsAutoSize、IsResizable、resize 边界和字数统计。
- 文档改动运行 `git diff --check`，并检查相对链接存在。
