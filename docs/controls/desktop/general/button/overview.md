# Button 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.Button` 桌面版的最新架构设计、设计语言、交互模型、API 模型和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，Button Token 的专项设计见 [Button Token 设计](token.md)，设计和契约变化记录见 [Button Changelog](changelog.md)。

## 1. 控件定位

Button 是 AtomUI 桌面控件体系中的基础动作触发控件，用于承载用户可执行的明确操作。Button 的职责是将动作语义稳定映射为公共 API、内部状态、主题 Token、模板结构和交互反馈。

Button 不承担复杂内容布局、导航结构管理或业务状态表达职责。复杂按钮形态应通过 Button 家族控件扩展，而不是扩大 Button 本体的职责边界。

Button 家族控件包括 `DropdownButton`、`SplitButton`、`IconButton` 和 `HyperLinkButton`。这些控件可以有不同的模板结构，但应共享 Button 的动作语义、状态解释、尺寸体系和主题资源。

## 2. 设计语言

Button 的设计语言由三个正交维度组成。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 动作层级 | 操作在当前界面中的优先级。 | 主动作、普通动作、弱强调动作、链接式动作。 |
| 语义颜色 | 操作的语义属性。 | 默认、主要、危险、业务预设色。 |
| 视觉强度 | 同一语义在视觉上的强调程度。 | 实心、描边、虚线、填充、文本、链接。 |

Button 的最新设计模型以 `Color + Variant` 表达语义颜色和视觉强度。`ButtonType` 是历史合成模型，同时承载动作层级、语义颜色和视觉强度，应保留为兼容入口。

颜色不应被视为装饰属性。`Danger` 表达破坏性或高风险动作，预设色表达业务分类或语义扩展。视觉强度不应改变语义颜色，只改变同一语义的呈现强度。

## 3. 架构分层

Button 架构按职责分层，避免公共 API 解析、运行时状态、主题变量和模板结构相互耦合。

| 层 | 责任 | 主要载体 |
| --- | --- | --- |
| Public API | 暴露用户可设置的动作语义和行为入口。 | `Button.cs` |
| Effective State | 将公共 API 归一为模板和主题可消费的稳定状态。 | `Button.cs` internal 状态和伪类 |
| Template Contract | 定义模板节点、template part、伪类和 internal 属性契约。 | `Button.cs`、`ButtonTheme.axaml` |
| Theme Mapping | 将状态映射为视觉属性。 | `ButtonTheme.axaml`、`BrowserButtonThemes.axaml` |
| Component Token | 将全局设计体系转换为 Button 语义值。 | `ButtonToken.cs` |
| Integration | 与 Button 家族、CompactSpace、FormItem、Wave、Browser 主题协同。 | Button 家族控件和主题 |

状态流：

```text
Public API
  ButtonType / IsDanger / IsGhost / IsLoading / Shape / SizeType / Icon
  Color? / Variant? / CustomBackground
        ↓
Effective State
  Button type pseudo-classes
  EffectiveBorderThickness
  EffectiveCornerRadius
  WaveSpiritType
  EffectiveColor / EffectiveVariant
        ↓
Theme Variables
  Text / Background / Border / Shadow
  Custom background layer visibility
        ↓
Template Visual
  Normal / PointerOver / Pressed / Disabled / Loading
```

## 4. API 设计

Button 的公共 API 是控件最重要的稳定契约。API 设计应满足以下原则：

- 语义明确：属性表达用户意图，而不是模板实现细节。
- 优先级明确：多个 API 共同参与状态计算时，解析顺序必须稳定、可测试。
- 兼容性明确：正交 API 不得改变旧兼容 API 的既有行为。
- 模型清晰：旧合成 API 作为语法糖保留，正交 API 承载基础状态模型。

Public API 模型由兼容 API 和正交 API 两部分组成。兼容 API 保持现有使用方式，正交 API 承载 Button 的基础状态模型。

兼容 API：

- `ButtonType`: `Default`、`Dashed`、`Primary`、`Link`、`Text`
- `ButtonShape`: `Default`、`Circle`、`Round`
- `IsDanger`
- `IsGhost`
- `IsLoading`
- `SizeType`
- `Icon`
- `IsMotionEnabled`
- `IsWaveSpiritEnabled`
- `CustomBackground`

正交 API：

```csharp
public ButtonColor? Color { get; set; }
public ButtonVariant? Variant { get; set; }
```

`Color` 与 `Variant` 应使用 nullable 类型，用于区分“未参与新模型”和“显式设置默认值”。该区分是保持 `ButtonType`、`IsDanger` 与新 API 兼容优先级的必要条件。

解析顺序：

```text
显式 Color + Variant
> ButtonType / IsDanger 兼容映射
> Variant=Solid 自动补 Color=Primary
> Color=Default, Variant=Outlined
```

旧 API 映射：

| 旧 API | 正交模型 |
| --- | --- |
| `ButtonType=Default` | `Color=Default, Variant=Outlined` |
| `ButtonType=Primary` | `Color=Primary, Variant=Solid` |
| `ButtonType=Dashed` | `Color=Default, Variant=Dashed` |
| `ButtonType=Text` | `Color=Default, Variant=Text` |
| `ButtonType=Link` | internal link color, `Variant=Link` |

`ButtonColor` 不建议暴露 `Link`。`ButtonType.Link` 是兼容入口，内部映射到链接视觉即可。

自定义视觉覆层 API：

```csharp
public IBrush? CustomBackground { get; set; }
```

`CustomBackground` 表示 Button normal 状态的受控自定义背景覆层，主要用于渐变、图片或其他非纯色表面。该属性不是颜色语义，不参与 `Color + Variant` 的状态归一、文字色、边框色、阴影或 wave 颜色计算。`CustomBackground == null` 表示不启用自定义背景覆层。

`CustomBackground` 只在 `EffectiveVariant=Solid`、非危险态、非禁用态下生效。Hover 与 Pressed 状态隐藏自定义背景覆层，并露出 Button 标准 `Color + Variant` 状态背景。Text、Link、Filled、Outlined、Dashed 和 Danger 场景不应用该覆层。

## 5. 行为交互模型

Button 的交互状态应具有明确优先级。

```text
Disabled
> Loading
> Pressed
> PointerOver
> Normal
```

`Disabled` 表示控件不可交互，应屏蔽 hover、pressed、wave 等交互反馈。`Loading` 表示操作正在进行中，应影响 loading icon、透明度、原 icon 展示和 wave 播放条件，但不应被视为 API 层面的禁用状态。

`Pressed` 与 `PointerOver` 属于瞬时交互反馈，只影响视觉状态，不改变按钮语义。`IsGhost` 是背景呈现策略，不应改变颜色语义或动作层级。`IsDanger` 表达危险语义，不应被普通颜色选择隐式覆盖；如果用户显式设置 `Color`，需要按 API 解析优先级处理。

Button 继承 Avalonia Button 的基础点击、命令和键盘行为。AtomUI 的扩展逻辑不应绕过或破坏基础控件语义。

## 6. 状态模型

Button 的状态计算应由 C# 层完成，AXAML 主题只消费已经归一的状态或主题变量。该分层可以避免复杂 selector 承担 API 优先级判断。

状态模型包括：

- `ButtonType` 与 `IsDanger` 转换为主题伪类。
- `Icon`、`Content`、`IsLoading` 共同决定 `:icononly`、loading icon 和 icon 可见性。
- `Shape` 决定 `WaveSpiritType`，并参与 `Circle`、`Round` 的尺寸和圆角计算。
- `BorderThickness`、`ButtonType`、`IsEnabled` 等属性共同决定 `EffectiveBorderThickness`。
- `CornerRadius` 与 CompactSpace 状态共同决定 `EffectiveCornerRadius`。
- `CustomBackground`、`EffectiveVariant`、`EffectiveIsDanger` 和 `IsEnabled` 共同决定自定义背景覆层是否可见。

`Color + Variant` 归一后形成以下有效状态：

- `EffectiveColor`
- `EffectiveVariant`
- `EffectiveIsDanger`
- `EffectiveIsGhost`
- `EffectiveIsBordered`

如果 AXAML selector 需要消费这些状态，应定义为 Avalonia 属性；如果仅供 C# 内部计算使用，可以使用 private 字段或方法。不能用普通 CLR 属性承载 AXAML 主题变量。

## 7. 模板与视觉架构

Button 模板应保持职责清晰的视觉分层。

| 节点 | 职责 |
| --- | --- |
| `PART_WaveSpirit` | 承载点击 wave 反馈。 |
| `ShadowsFrame` | 承载按钮阴影。 |
| `Frame` | 承载主体背景、边框、圆角和尺寸基底。 |
| `CustomBackgroundLayer` | 覆盖在 `Frame` 上方，承载 Button normal 状态的受控自定义背景覆层，仅由 Button 主题内部使用。 |
| `PART_RootLayout` | 排列 loading icon、icon 和 content。 |
| `PART_LoadingIcon` | 展示 loading 状态图标。 |
| `PART_ButtonIcon` | 展示用户设置的 icon。 |
| `PART_ContentPresenter` | 展示用户内容。 |

AXAML 优化应以保持职责边界为前提。可以移除无明确职责的包装层，但不得合并承担不同视觉职责的节点，例如阴影层、主体绘制层和 wave 层。

`ShadowsFrame` 不应被视为普通包装层。它将阴影从主体背景和边框中分离出来，使 shadow、background、border、corner radius 和 `BackgroundSizing` 可以保持独立职责。

`CustomBackgroundLayer` 是 Button 主题内部视觉层，不作为用户可依赖的 template part 暴露。用户应通过 `CustomBackground` 设置自定义背景，不应通过 `/template/` selector 操作该层。该层只覆盖 normal 状态表面，不承载 border、shadow、content、hit test 或 wave 职责。

启用 `CustomBackground` 时，`Frame` 仍保留标准 `Color + Variant` 背景和边框，作为 hover / pressed 以及覆层 opacity 过渡期间的底色。`CustomBackgroundLayer` 覆盖在 `Frame` 上方，normal 状态遮盖底层边框和背景；hover / pressed 状态隐藏覆层，露出标准状态背景。

## 8. Theme 架构

Button 主题实现应采用分层变量模型，避免直接展开 `Color × Variant × State` 的组合样式。

```text
Color Selector
  设置颜色语义变量

Variant Selector
  将颜色语义变量映射到文字、背景、边框、阴影变量

State Selector
  将 Normal / PointerOver / Pressed / Disabled / Loading 状态应用到最终视觉属性

Custom Background Selector
  在受支持状态显示自定义背景覆层，在 hover / pressed / disabled / danger 状态隐藏覆层
```

用于 AXAML `Setter`、selector、动态资源和主题切换的变量应定义为 internal `StyledProperty`。普通 CLR 属性不适合作为主题变量，`DirectProperty` 仅适用于不参与 Style 系统的内部运行时状态。

ButtonToken 负责提供组件级语义值，Theme 负责把状态映射为视觉属性。Token 的分类、用途、边界和扩展策略见 [Button Token 设计](token.md)。

## 9. Button 家族协同

Button 家族控件应共享一致的动作语义和状态解释。

- `DropdownButton` 应继承 Button 的基础动作状态，并扩展下拉触发能力。
- `SplitButton` 应保持主动作和附加动作的语义区分，但两部分的颜色、尺寸和禁用状态应与 Button 体系一致。
- `IconButton` 应保留 Button 的动作语义，只改变内容呈现密度。
- `HyperLinkButton` 应保持链接式动作语义，并复用 Button 尺寸、字体和 icon 相关 Token。
- `BrowserButtonThemes` 可以存在平台视觉差异，但同一 API 的语义解释必须与桌面主题一致。

`Color + Variant` 等语义能力不应只覆盖默认 Button。Button 家族控件必须明确继承、覆盖或声明不支持该语义。

## 10. 兼容性不变量

优化或扩展必须保持以下不变量：

- 现有五种 `ButtonType` 的默认、hover、pressed、disabled、danger、ghost 渲染不变。
- `IsDanger` 与各 `ButtonType` 的组合行为不变。
- `IsGhost` 与现有按钮类型的组合行为不变。
- loading icon、opacity、原 icon 隐藏逻辑不变。
- icon-only 判断与布局不变。
- `Shape=Circle`、`Shape=Round` 的尺寸和圆角计算不变。
- CompactSpace 下的有效圆角、有效边框和 z-index 行为不变。
- wave 播放条件和危险态 wave brush 不变。
- `CustomBackground` 不改变 `WaveSpiritDecorator` 的 wave brush，wave 颜色仍由 `EffectiveColor + EffectiveVariant` 推导。
- Browser 主题与桌面主题在同一 API 下语义一致。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

## 11. Color / Variant 模型

多彩按钮属于 Button 当前设计模型的一部分，不应通过扩展 `ButtonType` 枚举表达。

模型定义：

- `ButtonColor` 表达颜色语义。
- `ButtonVariant` 表达视觉强度。
- `ButtonType` 保留为兼容语法糖。
- `IsDanger` 保留为危险语义的旧入口。
- 预设色来源于 AtomUI palette / token 系统，不在 Button 主题中维护私有色表。

实现必须以状态归一层为基础，再进行主题变量映射。不得直接在 AXAML 中通过大量 selector 组合模拟状态模型。

### 11.1 CustomBackground 视觉覆层模型

`CustomBackground` 是 Button 的受控视觉覆层模型，用于表达 normal 状态下的自定义按钮表面。它解决渐变背景等纯色 Token 无法表达的视觉需求，但不改变 Button 的动作语义、颜色语义或交互状态。

模型定义：

- `CustomBackground` 是 public `StyledProperty<IBrush?>`，用户通过属性或样式设置。
- `CustomBackground` 不需要额外启用开关；非空值表示请求显示自定义背景覆层。
- 自定义背景覆层只在 `EffectiveVariant=Solid`、非危险态、非禁用态下显示。
- Hover 与 Pressed 状态将覆层透明度降为 `0`，标准 Button hover / pressed 背景继续由 `VariantBackgroundHoverBrush` 和 `VariantBackgroundPressedBrush` 决定。
- `CustomBackground` 不影响 `VariantTextBrush`、`VariantBackgroundBrush`、`VariantBorderBrush`、`VariantShadow` 或 wave brush。
- 自定义背景覆层是主题内部实现细节，不形成用户可依赖的 `/template/` 样式入口。

该模型等价于 Ant Design 渐变按钮示例中的 `::before` 覆层：normal 状态显示自定义表面，交互状态回落到 Button 原有语义状态。

## 12. 验证策略

不同改动类型对应不同验证范围。

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | `git diff --check`，确认未改实现文件。 |
| C# 状态模型改动 | Button 行为测试，覆盖旧 API 兼容映射。 |
| AXAML 主题改动 | Button 视觉状态检查，必要时补 Gallery 截图。 |
| Token / Palette 改动 | Light / Dark 主题检查，确认 Browser 主题一致性。 |
| Button 家族影响 | 覆盖 `DropdownButton`、`SplitButton`、`IconButton`、`HyperLinkButton` 关联场景。 |
| Public API 改动 | 需要授权，并补充 API 兼容测试与文档。 |
