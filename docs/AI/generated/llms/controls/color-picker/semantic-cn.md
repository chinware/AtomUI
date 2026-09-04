# ColorPicker 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

`ColorPicker` 与 `GradientColorPicker` 各自是唯一 Semantic owner，公开 5 个 Semantic Part（与上游
ColorPicker 的官方语义 API 逐一对齐：`root` / `body` / `content` / `description` / `popup.root`）。
声明分别位于 `ColorPicker.SemanticParts.cs` 与 `GradientColorPicker.SemanticParts.cs` partial 文件。

触发区部件（`body` / `description`）与弹层根部件（`popup.root`）的 marker 均位于两个 owner 自有的
`Themes/ColorPickerTheme.axaml`、`Themes/GradientColorPickerTheme.axaml` 宿主模板内；`content` 的
marker 位于共享的 `Themes/ColorBlockTheme.axaml`（ColorBlock 自身模板内），因此 `content` 声明
`CrossNestedOwners=true`——这与 Cascader 的 `clear` 部件（marker 在共享 `SelectHandle` 模板内）同构。

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `ColorPicker` / `GradientColorPicker` |
| Part | `root` |
| Selector | owner 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `ColorPicker` / `GradientColorPicker` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | owner 根 |
| 职责 | 触发器容器：边框、圆角、尺寸、状态与布局的组织边界。 |
| 相关 API | `SizeType`、`Status`、`BorderBrush`、`TriggerPadding` 等 owner public API |
| 相关 Token | ColorPickerToken、SharedToken（ColorBorder、BorderRadius*） |
| 稳定性 | stable since 6.0 |

#### `body`

| 字段 | 值 |
| --- | --- |
| Owner | `ColorPicker` / `GradientColorPicker` |
| Part | `body` |
| Selector | `.semantic-body` |
| SelectorRoute | `/template/ .semantic-body` |
| Style Type | `ColorPickerBodyStyle` / `GradientColorPickerBodyStyle` |
| ContractType | `Control` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | owner 模板中的 `PART_ColorIndicator`（内部控件 `ColorBlock` 的模板实例节点；公共契约承诺 Avalonia `Control`） |
| 职责 | 触发器内的色块容器，承载底色、空色斜线与棋盘格呈现。 |
| 相关 API | `ColorBlockSize`、`ColorBlockBackground` |
| 相关 Token | ColorPickerHandlerSize*、ColorBlockInnerShadows |
| 稳定性 | stable since 6.0 |

#### `content`

| 字段 | 值 |
| --- | --- |
| Owner | `ColorPicker` / `GradientColorPicker` |
| Part | `content` |
| Selector | `.semantic-content` |
| SelectorRoute | `/template/ .semantic-body /template/ .semantic-content` |
| Style Type | `ColorPickerContentStyle` / `GradientColorPickerContentStyle` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `ColorBlockTheme.axaml` 中 `PART_ColorPreview`（`semantic-body` 节点自有模板内） |
| 职责 | 色块颜色元素，呈现实际选择的颜色填充。 |
| 相关 API | —（随 owner 的 `Value` 联动，不单独开放） |
| 相关 Token | ColorBlockInnerShadows |
| 稳定性 | stable since 6.0 |

#### `description`

| 字段 | 值 |
| --- | --- |
| Owner | `ColorPicker` / `GradientColorPicker` |
| Part | `description` |
| Selector | `.semantic-description` |
| SelectorRoute | `/template/ .semantic-description` |
| Style Type | `ColorPickerDescriptionStyle` / `GradientColorPickerDescriptionStyle` |
| ContractType | `TextBlock`（GradientColorPicker：`Panel`） |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `ColorPickerTheme.axaml` 中 `PART_ColorText`；`GradientColorPickerTheme.axaml` 中 `PART_ColorTextPanel`（WrapPanel，逐渐变 stop 的文本格） |
| 职责 | 触发器文本区：单色模式显示格式化颜色文本；渐变模式显示逐 stop 文本格。 |
| 相关 API | `IsTextVisible`、`ColorTextFormatter`（attached）、`Format` |
| 相关 Token | TriggerTextMargin、SharedToken（FontSize*、ColorText） |
| 稳定性 | stable since 6.0 |

#### `popup.root`

| 字段 | 值 |
| --- | --- |
| Owner | `ColorPicker` / `GradientColorPicker` |
| Part | `popup.root` |
| Selector | `.semantic-popup-root` |
| SelectorRoute | `/template/ .semantic-popup-root` |
| Style Type | `ColorPickerPopupRootStyle` / `GradientColorPickerPopupRootStyle` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `false` |
| AtomUI 节点 | owner 模板 `PART_Popup` 直接子节点 `ColorPickerPopupRootFrame`（`Border` 子类，向内容层 `ArrowDecoratedBox` 转发 `IArrowAwareShadowMaskInfoProvider`），语义边框贴合弹层外沿（对齐上游 popup root） |
| 职责 | 弹层根容器：承载弹层边框、背景类视觉的定制入口；弹层 View 本身由 `CreatePresenter()` 动态创建，不经此 Part 发布。 |
| 相关 API | `IsPopupPinnedOpen`（6.0 公共化）、`Placement`、`IsArrowVisible` |
| 相关 Token | ColorPickerInsetShadow、SharedToken（ColorBgElevated） |
| 稳定性 | stable since 6.0 |

### GradientColorPicker 差异

`GradientColorPicker` 与 `ColorPicker` 共享同一套 5 部件语义契约，仅 `description` 的 `ContractType`
不同：触发文本区是 `WrapPanel`（`PART_ColorTextPanel`，内含逐 stop 的文本格），公共契约放宽为
`Panel`。其推荐 selector 为 `atom|GradientColorPicker /template/ Panel.semantic-description`。其余四
部件的 selector、ContractType、Cardinality 与 `ColorPicker` 完全一致。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls.ColorPicker/Themes/ColorPickerTheme.axaml`

```xml
<Panel>
    <PixelAlignedBorder Name="PART_Frame">
        <StackPanel>
            <ColorBlock Name="PART_ColorIndicator" />
            <TextBlock Name="PART_ColorText" />
        </StackPanel>
    </PixelAlignedBorder>
    <Popup Name="PART_Popup">
        <ColorPickerPopupRootFrame>
            <ArrowDecoratedBox>
                <ContentPresenter />
            </ArrowDecoratedBox>
        </ColorPickerPopupRootFrame>
    </Popup>
</Panel>
```

## Composition Model

该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外运行时组合层。

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ColorValue`、`ColorValueBrush`、`DefaultValue`、`EmptyColorText`、`GradientValue`、`IsTextVisible`、`MaxValue`、`MinValue`、`Value`、`ValueSyncStrategy` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `ColorModel`、`IsEmptyColorMode`、`IsPaletteGroupEnabled` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsActivated`、`IsAlphaEnabled`、`IsAlphaVisible`、`IsArrowVisible`、`IsClearEnabled`、`IsColorSpectrumSliderVisible`、`IsFormatEnabled`、`IsMotionEnabled`、`IsPerceptive`、`IsPointAtCenter` 等 15 项 | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `Color`、`ColorComponent`、`ColorSpectrumComponents`、`HsvColor`、`MarginToAnchor`、`Placement`、`PlacementAnchor`、`PlacementGravity`、`Shape`、`Size` 等 12 项 | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 动效与异步 | `MouseEnterDelay`、`MouseLeaveDelay` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `ActivatedThumb`、`Components`、`DecreaseButton`、`Format`、`IncreaseButton`、`MaxHue`、`MaxSaturation`、`Maximum`、`MinHue`、`MinSaturation` 等 14 项 | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close、collection/filter、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、Control Own Token 和模板绑定如何表达视觉。 | ColorPicker Token + ControlTheme。 |

## State Flow

ColorPicker 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- open/close、collection/filter、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `IsPickerOpen` 是 picker 的业务打开状态，`Popup.IsOpen` 是物理宿主状态；pinned 期间普通关闭不能改变业务状态，锚点隐藏、detach 或 TopLevel 失效仍可关闭物理宿主，并在有效性恢复后重新打开。
- `Value`、trigger 色块、trigger 文本、picker presenter 和 Form 值必须由同一份 current value 派生；清空状态以 `Value=null` 为源头。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

ColorPicker 的视觉模型由控件模板、ControlTheme、SharedToken 和 ColorPicker Own Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AbstractColorPickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorBlockTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorPickerPaletteGroupTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorPickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `AbstractColorPickerSliderTrackTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorPickerSliderTrackTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorSliderTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorSliderThumbTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `GradientColorPickerTrackTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `GradientColorSliderTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `AbstractColorPickerViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorPickerInput.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorPickerViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ColorSpectrumTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `GradientColorPickerViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `GradientColorPickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `PaletteColorItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |

ColorPicker 拥有独立 Control identity；`ColorPickerToken` 只表达 ColorPicker Own Token 语义，不承载 open/close、collection/filter、input/value、motion 或 visual option 运行时状态。Control 级 Global Token 覆盖与 Own Token 通过 `ColorPickerTokenResource` 统一读取。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

ColorPicker Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ColorPickerToken`，scope id 为 `ColorPicker`，源码位于 `src/AtomUI.Desktop.Controls.ColorPicker/ColorPickerToken.cs`。

## Customization Boundaries

维护 ColorPicker 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 ColorPicker 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
