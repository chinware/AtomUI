# Button Token 设计

本文档定义 `AtomUI.Desktop.Controls.ButtonToken` 的 Button 专属语义、分类、使用范围和兼容边界。控件 Token 的通用分层、命名、计算、Theme Variables 边界和预设色规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/control-token-guidelines.md)。Button 整体架构见 [Button 桌面版架构设计](overview.md)，内部实现原理见 [Button 桌面版实现原理](implementation.md)，设计和契约变化记录见 [Button Changelog](changelog.md)。

## 1. 定位

ButtonToken 是 Button 的组件级设计变量层。它把全局设计体系中的颜色、尺寸、间距、字体和阴影转换为 Button 可消费的语义值。

ButtonToken 服务以下主题和控件：

- `ButtonTheme.axaml`
- `DropdownButtonTheme.axaml`
- `SplitButtonTheme.axaml`
- `HyperLinkButtonTheme.axaml`
- Button 家族控件中的 icon、额外内容、下拉间距和状态视觉

ButtonToken 不承载 `IsPressed`、`IsPointerOver`、`IsLoading`、`EffectiveColor`、`EffectiveVariant` 等实例状态。这些状态由 Button 状态模型和主题变量处理。

## 2. Token 分类

ButtonToken 当前按 Button 语义分为七类。

### 2.1 排版 Token

- `FontWeight`
- `ContentFontSize`
- `ContentFontSizeLG`
- `ContentFontSizeSM`
- `ContentLineHeight`
- `ContentLineHeightLG`
- `ContentLineHeightSM`

用于控制 Button 内容文本的字体大小、行高和字重。字体大小默认来自 SharedToken；行高在 `CalculateTokenValues` 中根据字体大小计算。

### 2.2 尺寸与间距 Token

- `Padding`
- `PaddingLG`
- `PaddingSM`
- `CirclePadding`
- `IconOnyPadding`
- `IconOnyPaddingLG`
- `IconOnyPaddingSM`
- `IconMargin`
- `ExtraContentItemSpacing`
- `ExtraContentMargin`
- `ExtraContentMarginLG`
- `ExtraContentMarginSM`
- `GutterToFlyout`

用于 Button 的尺寸密度、icon-only 内边距、圆形按钮内边距、DropdownButton 额外内容和下拉菜单间距。

`SizeType=Custom` 不新增 ButtonToken。Custom 尺寸的默认值复用 `Middle` 档 Token；实例级定制通过 Button 现有布局和排版属性完成，Token 仍只表达预设尺寸语义。

### 2.3 Icon Token

- `OnlyIconSize`
- `OnlyIconSizeLG`
- `OnlyIconSizeSM`
- `IconSize`
- `IconSizeLG`
- `IconSizeSM`

用于区分普通 icon 和 icon-only 场景。icon-only 按钮的图标尺寸独立于普通内容按钮。

### 2.4 默认按钮颜色 Token

- `DefaultColor`
- `DefaultBg`
- `DefaultBorderColor`
- `DefaultBorderColorDisabled`
- `DefaultHoverBg`
- `DefaultHoverColor`
- `DefaultHoverBorderColor`
- `DefaultActiveBg`
- `DefaultActiveColor`
- `DefaultActiveBorderColor`
- `BorderColorDisabled`

用于普通动作的文本、背景、边框及 hover / active / disabled 状态。该组 Token 服务 `ButtonType=Default`、`ButtonType=Dashed` 和 `Color=Default` 的基础视觉。

### 2.5 主色与危险 Token

- `PrimaryColor`
- `DangerColor`
- `PrimaryShadow`
- `DangerShadow`

用于主动作和危险动作的前景与阴影。危险语义优先级高，不应通过普通样式覆盖隐式消失。

### 2.6 Ghost、Text、Link Token

- `DefaultGhostColor`
- `GhostBg`
- `DefaultGhostBorderColor`
- `SolidTextColor`
- `TextTextColor`
- `TextTextHoverColor`
- `TextTextActiveColor`
- `TextHoverBg`
- `LinkHoverBg`

用于 ghost 背景策略、文本按钮、链接按钮和实心按钮前景策略。Ghost 是背景呈现策略，Text / Link 是低强调动作表达。

Text Token 必须保持以下语义：

- `ButtonType=Text` 等价于 `Color=Default + Variant=Text`，文字使用 `TextTextColor`、
  `TextTextHoverColor` 和 `TextTextActiveColor`，默认都来自 `ColorText`，不隐式跟随品牌主色。
- `TextHoverBg` 默认来自 `ColorFillTertiary`；pressed 背景使用 `ColorFill`。
- `Color=Primary + Variant=Text` 的 normal、hover、pressed 文字分别来自 `ColorPrimary`、
  `ColorPrimaryHover`、`ColorPrimaryActive`，背景分别为透明、`ColorPrimaryBg`、`ColorPrimaryBorder`。
- Danger Text 的 hover 背景使用 `ColorErrorBg`，pressed 背景使用 `ColorErrorBgActive`。

### 2.7 结构协同 Token

- `DefaultShadow`
- `GroupBorderColor`

用于默认按钮阴影和按钮组边框协调。该组 Token 影响 Button 与 Button 家族或周边控件的视觉关系。

## 3. Button 多彩模型中的 Token 使用

Button 多彩模型不在 ButtonToken 中展开所有颜色与 variant 组合。

ButtonToken 保留 Button 组件常用语义值，例如 default、primary、danger、text、link、shadow、padding。Preset color 的 base / hover / active / light / shadow 来源于全局 palette 或通用 preset palette resource。

Button 颜色状态按以下职责使用 Token：

```text
ButtonToken
  提供 default / primary / danger / text / link 等组件语义值

Palette
  提供 preset color 色阶

Effective color resolver
  根据 EffectiveColor 选择 ButtonToken、SharedToken 或 Palette 值

Variant resolver
  将颜色变量映射到文字、背景、边框和阴影变量

ControlTheme
  只把最终主题变量投影到 normal、hover、pressed 和 disabled 视觉
```

ButtonToken 不定义 `BlueSolidHoverBg`、`PurpleFilledActiveBg` 这类组合 Token。

## 4. Button 家族影响

ButtonToken 当前被 Button 家族主题共同引用。Token 变更必须评估：

- 默认 Button
- Browser Button
- DropdownButton
- SplitButton
- HyperLinkButton
- IconButton 相关主题

如果某个值只服务特定家族控件，应确认它是否仍属于 Button 体系共享语义。只有共享语义值才应进入 ButtonToken。

Button 家族控件支持 Custom 尺寸时，应沿用同一原则：未设置本地尺寸属性时使用 Middle 默认值；设置本地属性时由本地值覆盖。不得为家族控件私自复制一组 Custom 专属 Token，除非该值已经证明是稳定的家族级语义。

## 5. 兼容性要求

ButtonToken 属于 Button 主题契约。即使 `ButtonToken` 是 internal 类型，生成的 `ButtonTokenKind` 和 AXAML resource 使用点已经形成稳定依赖。

ButtonToken 变更要求：

- 不擅自重命名既有 Token。
- 不擅自删除既有 Token。
- 不改变既有 Token 的语义含义。
- 不把实例状态迁移到 Token。
- 不在 ButtonToken 中硬编码 Button 私有 preset 色表。
- 需要破坏性变更时，必须先说明影响范围并获得授权。

## 6. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| 新增 ButtonToken | 检查生成的 `ButtonTokenKind`、AXAML 引用和默认值计算。 |
| 修改 ButtonToken 计算 | 覆盖 light / dark 主题，检查 Button 与 Button 家族视觉。 |
| 删除或重命名 ButtonToken | 默认不允许；如获授权，需同步所有 AXAML 引用和生成文件。 |
| 多彩按钮 Token 调整 | 验证 default、primary、danger、preset color 与各 variant 的状态映射。 |
