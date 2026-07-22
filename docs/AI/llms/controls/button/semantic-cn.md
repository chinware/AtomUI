# Button 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Button` | 控件根语义区域，承载 public API、命令、点击、状态归一和伪类。 | `ButtonType`、`Color`、`Variant`、`IsDanger`、`IsGhost`、`IsLoading`、`SizeType`、`Shape`、`Icon`、`IconPlacement` | ButtonToken、SharedToken | stable |
| `wave` | `PART_WaveSpirit` | 点击 wave 反馈区域，跟随有效圆角和 wave 类型。 | `IsWaveSpiritEnabled`、`IsMotionEnabled` | SharedToken motion / wave 资源 | stable |
| `shadow` | `ShadowsFrame` | 阴影绘制层，独立于主体背景和边框。 | effective state | `DefaultShadow`、`PrimaryShadow`、`DangerShadow` | stable |
| `surface` | `Frame` | 主体背景、边框、圆角、尺寸和虚线边框绘制层。 | `ButtonType`、`Color`、`Variant`、`Shape`、`SizeType`、`CornerRadius`、`Padding` | default、primary、danger、text、link、padding、corner radius 相关 Token | stable |
| `customBackground` | `CustomBackgroundLayer` | normal 状态自定义背景覆层，只服务 `CustomBackground` 视觉模型。 | `CustomBackground` | 不新增专属 Token | internal-stable |
| `contentLayout` | `PART_RootLayout` | loading icon、用户 icon 和内容的排列区域。 | `IconPlacement`、`HorizontalContentAlignment`、`VerticalContentAlignment` | `IconMargin`、尺寸 Token | stable |
| `loadingIcon` | `PART_LoadingIcon` | loading 状态图标区域。 | `IsLoading` | `IconSize`、`OnlyIconSize` 相关 Token | stable |
| `icon` | `PART_ButtonIcon` | 用户 icon 区域，支持内容前后位置和 icon-only 场景。 | `Icon`、`IconPlacement` | `IconSize`、`OnlyIconSize`、`IconMargin` | stable |
| `content` | `PART_ContentPresenter` | 用户内容展示区域。 | `Content`、`ContentTemplate` | `ContentFontSize`、`ContentLineHeight`、`FontWeight` | stable |

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
  -> Button (control theme, BrowserButtonThemes.axaml)
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
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Border#ShadowsFrame (template-stable)
        -> Border#CustomBackgroundLayer (template-stable)
        -> DashedBorder#Frame (template-stable)
           -> DockPanel#PART_RootLayout (template-stable)
              -> IconPresenter#PART_ButtonIcon (template-stable)
              -> LoadingOutlined#PART_LoadingIcon (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Button` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Button` | control theme | `BrowserButtonThemes.axaml` | 用户代码 / 控件宿主 | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `CustomBackground` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `BrowserButtonThemes.axaml` | Button | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `CustomBackground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_WaveSpirit` | template node (WaveSpiritDecorator) | `BrowserButtonThemes.axaml` | Button | `EffectiveCornerRadius`, `WaveSpiritType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ShadowsFrame` | template node (Border) | `BrowserButtonThemes.axaml` | Button | `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (DashedBorder) | `BrowserButtonThemes.axaml` | Button | `Background`, `BackgroundSizing`, `BorderBrush`, `EffectiveBorderThickness`, `EffectiveCornerRadius`, `Height` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CustomBackgroundLayer` | template node (Border) | `BrowserButtonThemes.axaml` | Button | `CustomBackground`, `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RootLayout` | template node (DockPanel) | `BrowserButtonThemes.axaml` | Button | `Content`, `ContentTemplate`, `Foreground`, `HorizontalContentAlignment`, `Icon`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LoadingIcon` | template node (LoadingOutlined) | `BrowserButtonThemes.axaml` | Button | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ButtonIcon` | template node (IconPresenter) | `BrowserButtonThemes.axaml` | Button | `Foreground`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `BrowserButtonThemes.axaml` | Button | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Button` | control theme | `ButtonTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `CustomBackground` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `ButtonTheme.axaml` | Button | `Background`, `BackgroundSizing`, `BorderBrush`, `Content`, `ContentTemplate`, `CustomBackground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_WaveSpirit` | template node (WaveSpiritDecorator) | `ButtonTheme.axaml` | Button | `EffectiveCornerRadius`, `WaveSpiritType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ShadowsFrame` | template node (Border) | `ButtonTheme.axaml` | Button | `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (DashedBorder) | `ButtonTheme.axaml` | Button | `Background`, `BackgroundSizing`, `BorderBrush`, `EffectiveBorderThickness`, `EffectiveCornerRadius`, `Height` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CustomBackgroundLayer` | template node (Border) | `ButtonTheme.axaml` | Button | `CustomBackground`, `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RootLayout` | template node (DockPanel) | `ButtonTheme.axaml` | Button | `Content`, `ContentTemplate`, `Foreground`, `HorizontalContentAlignment`, `Icon`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LoadingIcon` | template node (LoadingOutlined) | `ButtonTheme.axaml` | Button | `Foreground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ButtonIcon` | template node (IconPresenter) | `ButtonTheme.axaml` | Button | `Foreground`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `ButtonTheme.axaml` | Button | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 节点 | 职责 |
| --- | --- |
| `PART_WaveSpirit` | 承载点击 wave 反馈。 |
| `ShadowsFrame` | 承载按钮阴影。 |
| `Frame` | 承载主体背景、边框、圆角和尺寸基底。 |
| `CustomBackgroundLayer` | 主题内部自定义背景覆层，不作为用户 template part。 |
| `PART_RootLayout` | 排列 loading icon、icon 和 content，并根据 `IconPlacement` 调整用户 icon 位置。 |
| `PART_LoadingIcon` | 展示 loading 状态图标。 |
| `PART_ButtonIcon` | 展示用户设置的 icon，位置由 `IconPlacement` 控制。 |
| `PART_ContentPresenter` | 展示用户内容。 |

## Pseudo Classes

| `root` | `Button` | 控件根语义区域，承载 public API、命令、点击、状态归一和伪类。 | `ButtonType`、`Color`、`Variant`、`IsDanger`、`IsGhost`、`IsLoading`、`SizeType`、`Shape`、`Icon`、`IconPlacement` | ButtonToken、SharedToken | stable |
| `wave` | `PART_WaveSpirit` | 点击 wave 反馈区域，跟随有效圆角和 wave 类型。 | `IsWaveSpiritEnabled`、`IsMotionEnabled` | SharedToken motion / wave 资源 | stable |
| `shadow` | `ShadowsFrame` | 阴影绘制层，独立于主体背景和边框。 | effective state | `DefaultShadow`、`PrimaryShadow`、`DangerShadow` | stable |
| `surface` | `Frame` | 主体背景、边框、圆角、尺寸和虚线边框绘制层。 | `ButtonType`、`Color`、`Variant`、`Shape`、`SizeType`、`CornerRadius`、`Padding` | default、primary、danger、text、link、padding、corner radius 相关 Token | stable |
| `customBackground` | `CustomBackgroundLayer` | normal 状态自定义背景覆层，只服务 `CustomBackground` 视觉模型。 | `CustomBackground` | 不新增专属 Token | internal-stable |
| `contentLayout` | `PART_RootLayout` | loading icon、用户 icon 和内容的排列区域。 | `IconPlacement`、`HorizontalContentAlignment`、`VerticalContentAlignment` | `IconMargin`、尺寸 Token | stable |
| `loadingIcon` | `PART_LoadingIcon` | loading 状态图标区域。 | `IsLoading` | `IconSize`、`OnlyIconSize` 相关 Token | stable |
| `icon` | `PART_ButtonIcon` | 用户 icon 区域，支持内容前后位置和 icon-only 场景。 | `Icon`、`IconPlacement` | `IconSize`、`OnlyIconSize`、`IconMargin` | stable |
| `content` | `PART_ContentPresenter` | 用户内容展示区域。 | `Content`、`ContentTemplate` | `ContentFontSize`、`ContentLineHeight`、`FontWeight` | stable |

`CustomBackgroundLayer` 是主题内部实现细节，不作为用户可直接依赖的 template part。LLMS semantic 文档可以记录它的存在和边界，但应明确它只服务 `CustomBackground` 受控视觉模型。

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
- `BrowserButtonThemes.axaml`
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
- `IconPlacement` 默认值必须保持 `Start`；`IconPlacement=End` 只允许改变用户 icon 的内容侧位置和间距方向。
- `Shape=Circle`、`Shape=Round` 的尺寸和圆角计算不变。
- `SizeType=Large/Middle/Small` 的预设尺寸、字体、内边距、圆角和 icon 尺寸不变。
- `SizeType=Custom` 未显式设置尺寸相关属性时必须按 `Middle` 默认值渲染；用户在 Button 上设置的本地 `Height`、`Padding`、`FontSize`、`CornerRadius` 等现有属性必须覆盖 Custom 默认值。
- CompactSpace 下的有效圆角、有效边框和 z-index 行为不变。
- wave 播放条件和危险态 wave brush 不变。
- `CustomBackground` 不改变 `WaveSpiritDecorator` 的 wave brush，wave 颜色仍由 `EffectiveColor + EffectiveVariant` 推导。
- Browser 主题与桌面主题在同一 API 下语义一致。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- public API、Avalonia 属性字段和 CLR wrapper 名称不变。
- StyledProperty / DirectProperty / RoutedEvent 与支持字段的定义顺序符合控件代码规范。
- `Button.cs` 保留公共属性、事件、方法和接口入口；实现拆分只承载内部逻辑。
- `Color + Variant` 优先级和旧 API 映射结果不变。
- `SizeType=Custom` 不引入 Button 专属 `Custom*` 尺寸属性；未设置本地尺寸属性时表现等同 `Middle`，设置本地属性时由 Avalonia 属性优先级自然覆盖。
- 主题不得以高于本地值的优先级写入 Custom 默认尺寸。
- `CustomBackgroundLayer` 不成为用户可依赖 template part。
- wave brush 不从 `CustomBackground`、模板背景或 hover 背景反推。
- CompactSpace 圆角和边框折叠行为不变。
- Browser 主题与桌面主题在同一 API 下语义一致。
