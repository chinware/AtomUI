# Button 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Button 公开 `root`、`icon` 和 `content` 三个 Semantic Part。Part 名称表达长期稳定的产品职责，不等同于当前模板节点名称。

### 1.1 `Button`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `Button` |
| Part | `root` |
| Selector | Button 本身 |
| ContractType | `Button` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Button owner |
| 职责 | Button root 是动作、状态与根视觉样式的统一 owner。 |
| 相关 API | 全部 Button public API |
| 相关 Token | ButtonToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `icon`

| 字段 | 值 |
| --- | --- |
| Owner | `Button` |
| Part | `icon` |
| Selector | `.semantic-icon` |
| ContractType | `Control` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 用户图标与 loading 图标区域 |
| 职责 | 统一表示 Button 的用户图标和 loading 图标视觉职责。 |
| 相关 API | `Icon`、`IsLoading`、`IconPlacement`、`IconWidth`、`IconHeight` |
| 相关 Token | `IconSize*`、`OnlyIconSize*`、`IconMargin` |
| 稳定性 | stable since 6.0 |

#### `content`

| 字段 | 值 |
| --- | --- |
| Owner | `Button` |
| Part | `content` |
| Selector | `.semantic-content` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 内容展示区域 |
| 职责 | 表示 Button 的用户内容展示与排版区域。 |
| 相关 API | `Content`、`ContentTemplate` |
| 相关 Token | `ContentFontSize`、`ContentLineHeight`、`FontWeight` |
| 稳定性 | stable since 6.0 |

`root` 是隐式 Part，不添加 `.semantic-root`。`ContractType` 只定义 Setter 可以稳定依赖的最低 public 类型，并通过
`x:SetterTargetType` 提供 AXAML 编译期类型上下文；它不参与 `.semantic-*` 的身份匹配。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonTheme.axaml`

```xml
<Panel>
    <WaveSpiritDecorator Name="PART_WaveSpirit" />
    <Border Name="ShadowsFrame" />
    <DashedBorder Name="Frame" />
    <Border Name="CustomBackgroundLayer" />
    <Border>
        <DockPanel Name="PART_RootLayout">
            <LoadingOutlined Name="PART_LoadingIcon" />
            <IconPresenter Name="PART_ButtonIcon" />
            <ContentPresenter Name="PART_ContentPresenter" />
        </DockPanel>
    </Border>
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Button
  -> Button (control theme, ButtonTheme.axaml)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> DashedBorder#Frame (template-stable)
        -> Border#CustomBackgroundLayer (template-stable)
        -> Border (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> LoadingOutlined#PART_LoadingIcon (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> Border#CustomBackgroundLayer (template-stable)
        -> DashedBorder#Frame (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> LoadingOutlined#PART_LoadingIcon (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> Border#CustomBackgroundLayer (template-stable)
        -> DashedBorder#Frame (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> LoadingOutlined#PART_LoadingIcon (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
  -> Button (control theme, ButtonTheme.axaml)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> DashedBorder#Frame (template-stable)
        -> Border#CustomBackgroundLayer (template-stable)
        -> Border (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> LoadingOutlined#PART_LoadingIcon (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> Border#CustomBackgroundLayer (template-stable)
        -> DashedBorder#Frame (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> LoadingOutlined#PART_LoadingIcon (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> Border#CustomBackgroundLayer (template-stable)
        -> DashedBorder#Frame (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> LoadingOutlined#PART_LoadingIcon (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Button` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Button` | control theme | `ButtonTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `CustomBackground` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `ButtonTheme.axaml` | Button | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `CustomBackground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_WaveSpirit` | template node (WaveSpiritDecorator) | `ButtonTheme.axaml` | Button | `EffectiveCornerRadius`, `IsMotionEnabled`, `IsWaveSpiritEnabled`, `WaveSpiritType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ShadowsFrame` | template node (Border) | `ButtonTheme.axaml` | Button | `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (DashedBorder) | `ButtonTheme.axaml` | Button | `Background`, `BackgroundSizing`, `BorderBrush`, `EffectiveBorderThickness`, `EffectiveCornerRadius`, `Height` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CustomBackgroundLayer` | template node (Border) | `ButtonTheme.axaml` | Button | `CustomBackground`, `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RootLayout` | template node (DockPanel) | `ButtonTheme.axaml` | Button | `Content`, `ContentTemplate`, `Foreground`, `HorizontalContentAlignment`, `Icon`, `IconHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LoadingIcon` | template node (LoadingOutlined) | `ButtonTheme.axaml` | Button | `IconHeight`, `IconWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ButtonIcon` | template node (IconPresenter) | `ButtonTheme.axaml` | Button | `Foreground`, `Icon`, `IconHeight`, `IconWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `ButtonTheme.axaml` | Button | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 节点 | 职责 |
| --- | --- |
| `PART_WaveSpirit` | 承载点击 wave 反馈。 |
| `ShadowsFrame` | 承载按钮阴影。 |
| `Frame` | 承载主体背景、边框、圆角和尺寸基底。 |
| `CustomBackgroundLayer` | 主题内部自定义背景覆层，不作为用户 template part。 |
| `PART_RootLayout` | 排列 loading icon、icon 和 content，并根据 `IconPlacement` 调整用户 icon 位置。 |
| `PART_LoadingIcon` | 展示 loading 状态图标，宽高通过 `TemplateBinding` 跟随 `IconWidth`、`IconHeight`。 |
| `PART_ButtonIcon` | 展示用户设置的 icon，位置由 `IconPlacement` 控制，宽高通过 `TemplateBinding` 跟随 `IconWidth`、`IconHeight`。 |
| `PART_ContentPresenter` | 展示用户内容。 |

## Pseudo Classes

- icon-only、loading、custom background 可见性相关伪类。

## State Flow

Button 的交互优先级：

```text
Disabled
> Loading
> Pressed
> PointerOver
> Normal
```

`Disabled` 表示控件不可交互，应屏蔽 hover、pressed、wave 等交互反馈。`Loading` 表示操作正在进行中，应影响 loading icon、透明度、原 icon 展示和 wave 播放条件，但不等价于 API 层面的禁用状态。

Button 继承 Avalonia Button 的基础点击、命令和键盘行为。AtomUI 扩展逻辑不得绕过或破坏基础控件语义。

Button 的 effective state 由 C# 层归一，AXAML 主题只消费已经归一的状态或主题变量。核心 effective state 包括：

- `EffectiveColor`、`EffectiveVariant`。
- `EffectiveIsDanger`、`EffectiveIsGhost`、`EffectiveIsBordered`。
- `EffectiveBorderThickness`、`EffectiveCornerRadius`。
- `WaveSpiritType`。
- icon-only、loading、custom background 可见性相关伪类。

## Theme and Token Boundaries

Button 模板应保持阴影层、主体绘制层、内容层、wave 层和自定义背景覆层的职责分离。可以移除无明确职责的包装层，但不得合并承担不同视觉职责的节点。

Button 主题采用分层变量模型，避免直接展开 `Color × Variant × State` 的组合样式。

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

Token 边界：

ButtonToken 是 Button 的组件级设计变量层。它把全局设计体系中的颜色、尺寸、间距、字体和阴影转换为 Button 可消费的语义值。

ButtonToken 服务以下主题和控件：

- `ButtonTheme.axaml`
- `DropdownButtonTheme.axaml`
- `SplitButtonTheme.axaml`
- `HyperLinkButtonTheme.axaml`
- Button 家族控件中的 icon、额外内容、下拉间距和状态视觉

ButtonToken 不承载 `IsPressed`、`IsPointerOver`、`IsLoading`、`EffectiveColor`、`EffectiveVariant` 等实例状态。这些状态由 Button 状态模型和主题变量处理。

## Customization Boundaries

优化或扩展 Button 时必须保持以下不变量：

- 现有五种 `ButtonType` 的默认、hover、pressed、disabled、danger、ghost 渲染不变。
- `IsDanger` 与各 `ButtonType` 的组合行为不变。
- `IsGhost` 与现有按钮类型的组合行为不变。
- loading icon、opacity、原 icon 隐藏逻辑不变。
- icon-only 判断与布局不变。
- 普通状态下的用户 icon，包括非 loading 的 icon-only 场景，继续使用 `IconSizeLG`、`IconSize`、`IconSizeSM` 默认值；不得切换到 `OnlyIconSize*`。
- 只有 `:icononly:loading` 状态的 loading icon 使用 `OnlyIconSizeLG`、`OnlyIconSize`、`OnlyIconSizeSM` 默认值。
- 用户在 Button 上设置的本地 `IconWidth`、`IconHeight` 必须覆盖 Theme 默认值，并同时作用于用户 icon 与 loading icon。
- `IconPlacement` 默认值必须保持 `Start`；`IconPlacement=End` 只允许改变用户 icon 的内容侧位置和间距方向。
- `Shape=Circle`、`Shape=Round` 的尺寸和圆角计算不变。
- `SizeType=Large/Middle/Small` 使用对应 Token 提供 `MinHeight`、字体、内边距、圆角和 icon 尺寸基线；内容或合法
  Semantic Part 布局 Setter 可以使最终高度超过该基线。
- `SizeType=Custom` 未显式设置尺寸相关属性时复用 `Middle` 的字体、内边距、圆角和 icon 默认值，但不继承 Middle
  的 `MinHeight`；用户设置的 `Height`、`MinHeight`、`Padding`、`FontSize`、`CornerRadius`、`IconWidth`、
  `IconHeight` 等现有属性必须按 Avalonia 原生优先级生效。
- CompactSpace 下的有效圆角、有效边框和 z-index 行为不变。
- wave 播放条件不变；播放前必须从 Button 当前最终视觉属性解析 wave brush，依次检查有效实色
  `BorderBrush` 和 `Background`，使 Theme 状态、Semantic root Style 与普通用户 Style 使用同一视觉事实源。
- 透明、纯白或非实色的最终 Brush 不作为 wave 颜色；无有效颜色时清除 Button 写入的 wave brush，使
  `WaveSpiritDecorator` 回到主题默认值。`CustomBackground` 是独立覆层，不参与该取色顺序。
- 同一 Button 家族主题资产必须在 Native 与 Browser 支持宿主下保持同一 API 语义；不得维护
  `Buttons/Themes/Browser/` 或 `BrowserButtonThemes.axaml` 形式的平台主题分叉。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- public API、Avalonia 属性字段和 CLR wrapper 名称不变。
- StyledProperty / DirectProperty / RoutedEvent 与支持字段的定义顺序符合控件代码规范。
- `Button.cs` 保留公共属性、事件、方法和接口入口；实现拆分只承载内部逻辑。
- `Color + Variant` 优先级和旧 API 映射结果不变。
- `ButtonType=Text` 保持 `Default + Text` 中性语义；`Primary + Text` 跟随当前主题 `ColorPrimary` 色阶。
- Button 家族主题必须共享 C# 计算出的最终颜色变量，不得按平台或兼容 API 复制颜色矩阵。
- `SizeType=Large/Middle/Small` 只通过 `MinHeight` 建立预设高度基线，不以主题 `Height` 封死自然测量。
- `SizeType=Custom` 不引入 Button 专属 `Custom*` 尺寸属性，不设置预设 `MinHeight`；未设置本地尺寸属性时复用
  `Middle` 的非高度指标，设置本地属性时由 Avalonia 属性优先级自然覆盖。
- 主题不得以高于本地值的优先级写入 Custom 默认尺寸。
- Circle/Round 的几何计算必须基于已经应用 Button Width/Height/Min/Max 约束的测量结果。
- `IconWidthProperty`、`IconHeightProperty` 及其 CLR wrapper 是 Button 公共契约，属性变化必须参与 measure invalidation。
- `PART_ButtonIcon`、`PART_LoadingIcon` 的默认 Width 和 Height 通过 `TemplateBinding IconWidth/IconHeight` 投影；
  外部局部覆盖只依赖 `.semantic-icon`，Setter 类型通过 `x:SetterTargetType="Control"` 提供，不得依赖类型前缀或
  `PART_*` 名称。
- 共享 Button ControlTheme 的每个 Button ControlTemplate 都必须具有两个 `.semantic-icon` marker 和一个
  `.semantic-content` marker；AtomUI 模板使用静态 `Classes.semantic-*="True"`，Button root 不添加
  `.semantic-root`。
- 普通用户 icon 和非 loading 的 icon-only 用户 icon 保持 `IconSize*` 默认值；只有 icon-only loading 默认使用 `OnlyIconSize*`。
- DropdownButton 继承同一图标尺寸属性与投影规则，`OpenIndicator` 继续由独立的 DropdownButton 主题尺寸控制；SplitButton 不纳入这一属性继承范围。
- `CustomBackgroundLayer` 不成为用户可依赖 template part。
- wave brush 不从 `CustomBackground` 覆层或内部模板节点反推；Button root 的最终 hover、pressed 或外部样式结果是
  合法取色输入。
- CompactSpace 圆角和边框折叠行为不变。
- 同一 Button 家族主题资产必须在 Native 与 Browser 支持宿主下保持同一 API 语义；不得维护
  `Buttons/Themes/Browser/` 或 `BrowserButtonThemes.axaml` 形式的平台主题分叉。
