# AtomUI 控件 Token 设计规范

本文档定义控件级 Token 的设计规则，适用于 `AtomUI.Controls`、`AtomUI.Desktop.Controls`、DataGrid、ColorPicker 以及后续新增控件包。单个控件的 `token.md` 只应记录该控件的专属 Token 语义、分类和兼容边界，通用规则保留在本文档。

## Token 分层

AtomUI 的控件 Token 位于全局设计体系与控件主题之间。

```text
DesignToken / SharedToken / Palette
        ↓
Control Token
        ↓
ControlTheme / BrowserTheme / family themes
        ↓
Template / Runtime visual state
```

各层职责：

| 层 | 职责 |
| --- | --- |
| `DesignToken` | 定义全局设计体系、种子 token、派生 token 和 palette。 |
| `SharedToken` | 提供跨控件共享的尺寸、颜色、动效、排版和交互值。 |
| Control Token | 将全局值转换为某个控件可消费的组件语义值。 |
| Theme Variables | 保存某个控件实例在当前状态下的最终视觉变量。 |
| Template | 消费最终视觉变量并完成渲染。 |

控件 Token 不替代全局设计 token。它只在控件范围内定义从全局 token 到组件语义的映射。

## 适用边界

控件 Token 应表达组件级设计值，例如：

- 控件默认、主色、危险、弱强调等语义色值。
- 控件尺寸、间距、字体、行高、阴影和状态背景。
- 控件家族共享的结构性设计值。

控件 Token 不应表达：

- 单个控件实例的运行时状态。
- `IsPressed`、`IsPointerOver`、`IsLoading` 等交互状态。
- `EffectiveColor`、`EffectiveVariant` 等由 API 解析得到的实例状态。
- 模板节点是否可见。
- 业务自定义颜色值。

## 与 Theme Variables 的关系

Control Token 和 internal theme variables 是不同层级。

```text
Control Token
  提供组件语义设计值

AXAML Selector
  根据控件状态选择 Token 或 Palette 值

Internal StyledProperty Theme Variables
  保存当前实例的有效视觉变量

Template
  将视觉变量应用到 Foreground / Background / BorderBrush / BoxShadow
```

需要通过 AXAML `Setter`、selector、动态资源和主题切换参与状态映射的变量，应定义为 internal `StyledProperty`。普通 CLR 属性不适合作为主题变量，`DirectProperty` 仅适用于不参与 Style 系统的内部运行时状态。

## 命名原则

控件 Token 命名应表达组件语义，而不是模板实现细节。

符合命名原则：

- `DefaultHoverColor`
- `PrimaryShadow`
- `TextHoverBg`
- `ContentFontSizeLG`

不符合命名原则：

- `BlueSolidHoverBackground`
- `FrameBorderPointerOverBrush`
- `Panel1Padding`
- `PrimaryButtonMouseOverColor`

Token 名称应稳定表达“控件语义 + 状态或规格”。如果名称需要描述模板节点、selector 条件或具体布局容器，通常说明该值更适合作为 theme variable，而不是控件 Token。

## 计算原则

控件 Token 的计算应集中在 `CalculateTokenValues` 中完成。

- 从 `SharedToken`、`DesignToken`、Palette 和控件默认值派生组件语义值。
- 允许用户预设值覆盖时，应先保留预设值，再计算缺省值。
- 尺寸、间距、字体、行高和阴影应基于全局 token 的稳定规则计算。
- 计算逻辑不应依赖单个控件实例属性，例如 `IsPressed`、`IsPointerOver`、`Shape` 或 `IsLoading`。
- 明暗主题差异应通过全局 token、palette 或 theme calculator 进入控件 Token，不应在控件主题中复制计算逻辑。

## 组合 Token 约束

控件 Token 不应展开所有颜色、variant、状态的笛卡尔积。

避免定义：

```text
BlueSolidHoverBg
BlueSolidPressedBg
BlueOutlinedHoverBorder
PurpleFilledActiveBg
```

推荐结构：

```text
Control Token
  保存控件常用语义值

Palette / Preset Resource
  提供 preset color 的 base / hover / active / light / shadow

Theme Selector
  根据 Effective State 选择 Token 或 Palette 值

Theme Variables
  承载当前实例最终文字、背景、边框、阴影变量
```

如果需要增加 Token，应优先增加语义 token，而不是组合 token。例如可以定义“filled variant 的默认弱背景策略”一类语义值，而不是为每个 preset color 增加一组 Token。

## 预设色规则

预设色应来源于 AtomUI 现有 palette 和 token 系统，例如：

- `PresetPrimaryColor`
- `PresetColorType`
- `DesignToken.ColorPalettes`
- 主题计算器生成的 light / dark palette

控件不应私有复制 preset 色十六进制表。多个控件共享 preset 色能力时，应优先设计通用 preset palette resource，而不是在单个控件 Token 中复制颜色常量。

## 兼容性要求

控件 Token 属于主题契约的一部分。即使 Token 类型是 internal，生成的 `TokenKind` 和 AXAML resource 使用点也形成稳定依赖。

Token 变更要求：

- 不擅自重命名既有 Token。
- 不擅自删除既有 Token。
- 不改变既有 Token 的语义含义。
- 不把实例状态迁移到 Token。
- 不在 Token 中硬编码控件私有 preset 色表。
- 需要破坏性变更时，必须先说明影响范围并获得授权。

## 验证要求

不同 Token 改动对应不同验证范围。

| 改动类型 | 验证要求 |
| --- | --- |
| 新增 Token | 检查生成的 `TokenKind`、AXAML 引用和默认值计算。 |
| 修改 Token 计算 | 覆盖 light / dark 主题，检查控件和相关控件家族视觉。 |
| 删除或重命名 Token | 默认不允许；如获授权，需同步所有 AXAML 引用和生成文件。 |
| 引入 preset colors | 验证 default、primary、danger、preset color 与各 variant 的状态映射。 |

