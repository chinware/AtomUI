# ToggleSwitch 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

ToggleSwitch 主控件公开 `root`、`content` 与 `indicator` 三个职责区域，与上游稳定 Semantic DOM
（`root` / `content` / `indicator`）对齐。`indicator` 对应滑动把手区域，由模板中的 `SwitchKnob` 节点承载；`content`
对应开关内部 on/off 内容区域，由模板中的两个 `ContentPresenter`（`PART_OnContentPresenter` 与
`PART_OffContentPresenter`）承载。

`AbstractToggleSwitch`、`SwitchKnob` 与 `WaveSpiritDecorator` 均不持有独立 Semantic descriptor：

- 上游 Switch 只提供一个 owner 的 Semantic DOM（`root` / `content` / `indicator`），`AbstractToggleSwitch` 是跨平台共享
  基类，不是对应用公开的独立 owner，因此不为它声明 descriptor。
- `SwitchKnob` 是 internal 类型，不能作为公共 descriptor owner；其把手职责通过 `ToggleSwitch` 的 `indicator` Part 对外
  公开，并以 `TemplatedControl` 作为最低 ContractType。
- `WaveSpiritDecorator` 是 checked 变化时的视觉反馈 actor，不是用户可定制的公共区域，不公开 Part。

因此本控件的 Semantic Part 只由 `ToggleSwitch` owner 公开。

### 1.1 `ToggleSwitch`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `ToggleSwitch` |
| Part | `root` |
| Selector | ToggleSwitch 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `ToggleSwitch` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | ToggleSwitch owner |
| 职责 | ToggleSwitch root 是开关值、状态、内容与根视觉样式的统一 owner。 |
| 相关 API | `IsChecked`、`GrooveBackground`、`OnContent`、`OffContent`、`OnContentTemplate`、`OffContentTemplate`、`SizeType`、`IsLoading`、`IsMotionEnabled`、`IsWaveSpiritEnabled`、`TrackHeight`、`TrackMinWidth`、`TrackPadding`、`KnobSize` |
| 相关 Token | ToggleSwitchToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `content`

| 字段 | 值 |
| --- | --- |
| Owner | `ToggleSwitch` |
| Part | `content` |
| Selector | `.semantic-content` |
| SelectorRoute | `/template/ .semantic-content` |
| Style Type | `ToggleSwitchContentStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | On/Off 内容 `ContentPresenter` |
| 职责 | 统一表示开关内部 checked / unchecked 内容区域的文本与视觉职责。 |
| 相关 API | `OnContent`、`OffContent`、`OnContentTemplate`、`OffContentTemplate` |
| 相关 Token | `ContentIconSize`、`ContentIconSizeSM`、`ExtraInfoFontSize`、`ExtraInfoFontSizeSM`、`InnerMinMargin`、`InnerMaxMargin` |
| 稳定性 | stable since 6.0 |

#### `indicator`

| 字段 | 值 |
| --- | --- |
| Owner | `ToggleSwitch` |
| Part | `indicator` |
| Selector | `.semantic-indicator` |
| SelectorRoute | `/template/ .semantic-indicator` |
| Style Type | `ToggleSwitchIndicatorStyle` |
| ContractType | `TemplatedControl` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 滑动把手 `SwitchKnob` |
| 职责 | 统一表示开关的滑动把手视觉职责，含把手填充、阴影与 loading 指示。 |
| 相关 API | `IsChecked`、`IsLoading` |
| 相关 Token | `HandleBg`、`HandleShadow`、`HandleSize`、`HandleSizeSM`、`SwitchColor`、`OffStateLoadIndicatorColor` |
| 稳定性 | stable since 6.0 |

`root` 是隐式 Part，不添加 `.semantic-root`。`ContractType` 只定义 Setter 可以稳定依赖的最低 public 类型，并通过
`x:SetterTargetType` 提供 AXAML 编译期类型上下文；它不参与 `.semantic-*` 的身份匹配。`indicator` 的 `ContractType` 为
`TemplatedControl` 而非 `SwitchKnob`，因为 `SwitchKnob` 是 internal 类型，不能作为公共 Setter 依赖的最低类型。
`content` 的 `Cardinality` 为 `Multiple`：模板中始终存在 on 与 off 两个内容 `ContentPresenter`，二者是同一 content
职责的两个替代节点，不随 `IsChecked` 增删。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Switch/Themes/ToggleSwitchTheme.axaml`

```xml
<Panel>
    <WaveSpiritDecorator Name="PART_WaveSpirit" />
    <Canvas Name="PART_MainContainer">
        <ContentPresenter Name="PART_OnContentPresenter" />
        <ContentPresenter Name="PART_OffContentPresenter" />
    </Canvas>
    <SwitchKnob Name="PART_SwitchKnob" />
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
ToggleSwitch
  -> ToggleSwitch (control theme, ToggleSwitchTheme.axaml)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#PART_WaveSpirit (template-stable)
        -> Canvas#PART_MainContainer (template-stable)
           -> ContentPresenter#PART_OnContentPresenter (template-stable)
           -> ContentPresenter#PART_OffContentPresenter (template-stable)
        -> SwitchKnob#PART_SwitchKnob (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `ToggleSwitch` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ToggleSwitch` | control theme | `ToggleSwitchTheme.axaml` | 用户代码 / 控件宿主 | `IsChecked`, `IsMotionEnabled`, `IsWaveSpiritEnabled`, `OffContent`, `OffContentTemplate`, `OnContent` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `ToggleSwitchTheme.axaml` | ToggleSwitch | `IsChecked`, `IsMotionEnabled`, `IsWaveSpiritEnabled`, `OffContent`, `OffContentTemplate`, `OnContent` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_WaveSpirit` | template node (WaveSpiritDecorator) | `ToggleSwitchTheme.axaml` | ToggleSwitch | `IsMotionEnabled`, `IsWaveSpiritEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MainContainer` | template node (Canvas) | `ToggleSwitchTheme.axaml` | ToggleSwitch | `OffContent`, `OffContentTemplate`, `OnContent`, `OnContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_OnContentPresenter` | template node (ContentPresenter) | `ToggleSwitchTheme.axaml` | ToggleSwitch | `OnContent`, `OnContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_OffContentPresenter` | template node (ContentPresenter) | `ToggleSwitchTheme.axaml` | ToggleSwitch | `OffContent`, `OffContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SwitchKnob` | template node (SwitchKnob) | `ToggleSwitchTheme.axaml` | ToggleSwitch | `IsChecked`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_WaveSpirit` | `WaveSpiritDecorator` | checked 变化时播放胶囊波纹。 |
| `PART_MainContainer` | `Canvas` | 承载内容 presenter 和把手，并提供裁剪边界。 |
| `PART_OnContentPresenter` | `ContentPresenter` | checked 状态下内容承载。 |
| `PART_OffContentPresenter` | `ContentPresenter` | unchecked 状态下内容承载。 |
| `PART_SwitchKnob` | `SwitchKnob` | 把手、加载指示和把手动画承载。 |

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

ToggleSwitch 的交互优先级：

```text
Disabled
> Loading
> Pressed
> PointerOver
> Checked / Unchecked
> Normal
```

`Disabled` 表示控件不可交互，主题降低整体透明度，把手保持当前状态。`Loading` 表示业务处理中，控件命中测试返回 false，pointer press/release 不进入基类切换逻辑，并把 cursor 设置为默认箭头。

checked 状态流：

```text
Pointer / keyboard / binding / Form.SetValue
      ↓
IsChecked
      ↓
CalculateElementsOffset
      ↓
KnobRect + content offset + GrooveBackground selector
```

按下状态会临时拉伸把手宽度，并同步调整当前可见内容偏移。释放后重新计算标准把手和内容位置。

Form 集成以 `IsChecked` 作为值。`ClearFormValue()` 会把 `IsChecked` 设置为 `null`，因此绑定方必须明确是否允许三态。

## Theme and Token Boundaries

ToggleSwitch 使用控件本体绘制轨道，模板负责内容和把手。

| 主题 | 职责 |
| --- | --- |
| `ToggleSwitchTheme.axaml` | 主模板、状态 selector、SizeType 分支、轨道背景、WaveSpirit 和 SwitchKnob token 传递。 |
| `SwitchKnobTheme.axaml` | 把手加载透明度、加载动画周期和把手宽度动效。 |

视觉状态主要由主题 selector 表达：

- `IsChecked=True` 使用 `SwitchColor`，hover 使用 `ColorPrimaryHover`。
- `IsChecked=False` 使用 SharedToken 的文本弱化色，hover 使用更强弱化色。
- `IsLoading=True` 和 disabled 使用 `SwitchDisabledOpacity`。
- `SizeType=Small` 使用 small token 组。
- `SizeType=Middle`、`SizeType=Custom` 和 `SizeType=Large` 共享普通 token 组。

`Custom` 不是独立 Token 组；未显式覆盖时与 `Middle` 保持同一视觉基线。

Token 边界：

ToggleSwitchToken 是 ToggleSwitch 的控件级 Token scope，描述轨道尺寸、把手尺寸、内容边距、图标尺寸、开关颜色、禁用透明度、内容字体、把手阴影和加载指示。

ToggleSwitchToken 不承载以下状态：

- `IsChecked`、`IsLoading`、`IsPressed`、`IsPointerOver`、`IsEnabled` 等实例状态。
- `OnContent`、`OffContent` 或内容模板。
- Form value、校验状态或业务异步状态。
- 当前把手位置、内容偏移、加载旋转角度或动画运行状态。

## Customization Boundaries

维护 ToggleSwitch 时必须保持以下不变量：

- 不修改 `ToggleButton.IsChecked` 的 checked、unchecked、null 和绑定语义。
- `IsLoading=true` 时不得触发新的切换。
- loading 状态必须在 detach 时停止动画并释放 cancellation token。
- `PART_SwitchKnob`、`PART_OnContentPresenter`、`PART_OffContentPresenter`、`PART_WaveSpirit` 和 `PART_MainContainer` 名称不变。
- `SizeType=Custom` 默认与 Middle 分支一致，不新增未授权的专属 Token 组。
- `OnContent` / `OffContent` 为 `PathIcon` 或 `Icon` 时，图标尺寸和前景色必须随控件状态更新。
- Form value 始终是 `IsChecked`，不把 loading、content 或视觉状态作为表单值。
- Token 名称、语义和 AXAML resource key 不擅自重命名、删除或迁移。

维护不变量：

内部重构必须保持以下不变量：

- `ToggleSwitch` public 类型保持轻量桌面入口，不把共享实现复制到 Desktop 包。
- `AbstractToggleSwitch` 继续作为 API 和状态核心。
- loading 不触发新切换，并释放 cursor local value 和 animation token。
- 内容图标 relay binding 在内容替换时释放。
- `MeasureOverride` 必须同时考虑 on/off content 的最大宽度。
- `SwitchKnob` loading animation detach 时必须取消。
- `SizeType=Custom` 默认与 Middle 视觉一致。
- `SwitchOpacity` 只表达 disabled/loading 视觉透明度，不改变 enabled 状态。
- WaveSpirit 只作为视觉反馈，不影响 `IsChecked`。
