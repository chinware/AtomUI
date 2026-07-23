# Select Token 设计

本文档定义 `AtomUI.Desktop.Controls.SelectToken` 的专属语义、分类、使用范围和兼容边界。控件 Token 的通用分层、命名、计算、Theme Variables 边界和预设色规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/control-token-guidelines.md)。Select 整体架构见 [Select 桌面版架构设计](overview.md)，内部实现原理见 [Select 桌面版实现原理](implementation.md)，设计和契约变化记录见 [Select Changelog](changelog.md)。

## 1. 定位

SelectToken 是 Select 的组件级 Token scope，描述多选标签、候选项、候选弹层 padding 和 Select 输入内容 padding。输入壳体的通用边框、圆角、状态色、focus ring、disabled 背景和 AddOn 结构来自 SharedToken、AddOnDecoratedBoxToken 和 PopupHostToken。

SelectToken 不承载以下状态：

- `OptionsSource`、`Options`、`SelectedOption`、`SelectedOptions`、`DefaultValues` 等数据状态。
- `FilterValue`、`IsDropDownOpen`、`IsLoading`、`Status`、`Mode` 等运行状态。
- 候选项 hover、pressed、selected、disabled 的当前实例状态。
- Popup 当前宽度、高度、offset、placement 或异步加载结果。

## 2. Token 分类

### 2.1 多选标签 Token

- `MultipleItemBg`
- `MultipleItemHeight`
- `MultipleItemHeightSM`
- `MultipleItemHeightLG`
- `MultipleSelectorBgDisabled`
- `MultipleItemColorDisabled`
- `FixedItemMargin`

这些 Token 控制 Multiple / Tags 模式下 `SelectTag` 的背景、尺寸、禁用态颜色和标签间距。高度按 SharedToken 的控件高度和基础 padding 计算，使 Large / Middle / Small 三种预设尺寸与输入框高度对齐。

### 2.2 候选项 Token

- `OptionSelectedColor`
- `OptionSelectedFontWeight`
- `OptionSelectedBg`
- `OptionActiveBg`
- `OptionPadding`
- `OptionFontSize`
- `OptionHeight`

这些 Token 控制 `SelectCandidateListItem` 的选中、active、文本和行高基准。当前弹层最大高度使用实例 `ItemHeight` 计算，主题默认值来自 `SharedToken.ControlHeight`；调整 `OptionHeight` 时必须同时验证候选项实际高度和弹层可视行数是否仍然一致。

### 2.3 输入内容 padding Token

- `Padding`
- `PaddingSM`
- `PaddingLG`
- `MultiModePadding`
- `MultiModePaddingSM`
- `MultiModePaddingLG`
- `SelectAffixPadding`

这些 Token 控制 Select 输入区域和多选内容区域的 padding。单选模式主要使用输入框默认内容布局；多选和 Tags 模式根据是否已有选中项切换普通 padding 与 multi mode padding。

`SizeType=Custom` 在主题中与 `Middle` 共享 `Padding` 和 `MultiModePadding` 分支。它不是独立 Token 组。

### 2.4 弹层 Token

- `PopupContentPadding`

该 Token 控制 `PopupFrame` 内部 padding，并参与 `MaxPopupHeight` 计算。

## 3. 控件专项模型中的 Token 使用

SelectToken 使用路径：

```text
SharedToken
   ↓
SelectToken
   ↓
SelectTheme / SelectAddOnDecoratedBoxTheme / SelectTagTheme / SelectCandidateListItemTheme
   ↓
Select input content + tags + candidate list + popup frame
```

运行时派生尺寸：

```text
MaxPopupHeight = ItemHeight * DisplayPageSize
               + PopupContentPadding.Top
               + PopupContentPadding.Bottom
```

`MaxPopupHeight`、`EffectivePopupWidth`、`SelectedCount`、`IsSelectionEmpty` 和 `IsEffectiveFilterEnabled` 都是实例运行状态，不是 Token。

## 4. 控件家族影响

SelectToken 直接影响 Select。TreeSelect 有独立 `TreeSelectToken`，ComboBox 有独立 `ComboBoxToken`，普通 LineEdit / NumericUpDown 不使用 SelectToken。

调整 SelectToken 时必须评估：

- `SelectTheme.axaml`
- `SelectAddOnDecoratedBoxTheme.axaml`
- `SelectCandidateListItemTheme.axaml`
- `SelectTagTheme.axaml`
- Select token.md 语义说明
- `SelectShowCasePageTests`

## 5. 兼容性要求

SelectToken 属于 Select 主题契约。即使 `SelectToken` 是 internal 类型，生成的 `SelectTokenKind`、AXAML resource 使用点和 token.md 语义说明已经形成稳定依赖。

Token 变更要求：

- 不擅自重命名或删除现有 Token。
- 不把实例数据、过滤状态、loading 状态、选择集合、popup 打开状态或 Form 状态迁移为 Token。
- 不在 SelectToken 中复制 AddOnDecoratedBox 的边框、圆角、focus、error、warning 或 disabled 通用状态 Token。
- 不把 `DisplayPageSize`、`MaxCount`、`MaxTagCount` 或 `IsResponsiveTagMode` 变成 Token；它们是实例行为属性。
- 需要破坏性变更时，必须先说明影响范围并获得授权。

## 6. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| 修改多选标签 Token | 验证 Multiple / Tags 模式下 Large / Middle / Small / Custom 标签高度、间距、关闭按钮和 disabled 状态。 |
| 修改候选项 Token | 验证候选项高度、active/selected 视觉、键盘导航滚动和 `DisplayPageSize` 下的可视行数一致性。 |
| 修改输入 padding Token | 验证单选、多选、Tags、空选择、有选择、prefix/suffix/addon 和 CompactSpace 对齐。 |
| 修改 `PopupContentPadding` | 验证 popup 内边距、候选列表裁剪、loading/empty 视觉和最大高度计算。 |
| 删除或重命名 Token | 默认不允许；如获授权，需同步 AXAML 引用、生成文件、Token 类型、生成数据和 token.md和控件文档。 |
| 文档改动 | 运行 `git diff --check`，检查文档链接存在。 |
