# ToggleSwitch

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

ToggleSwitch 是 AtomUI 桌面数据录入体系中的布尔状态切换控件，用于表达立即生效的开/关选择。它以 Avalonia `ToggleButton` 的选中状态为基础，接入 AtomUI 的尺寸、加载态、文字或图标内容、WaveSpirit、Form 和 Token 体系。

ToggleSwitch 的职责是编辑单个 `bool?` 状态。它不承担多选、单选组、延迟提交、异步命令调度、复杂权限判断或表单校验消息展示职责。

桌面 public 类型 `ToggleSwitch` 继承 `AtomUI.Controls.AbstractToggleSwitch`。`ToggleSwitch` 本体只注册桌面 `ToggleSwitchToken` scope，公共 API、布局、交互和 Form 集成由 `AbstractToggleSwitch` 承载。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch` |
| 状态 | Stable |

## 何时使用

ToggleSwitch 的设计语言来自胶囊轨道、滑动把手和可选内容的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 状态语义 | 当前值是启用或关闭。 | `IsChecked=True/False/null`。 |
| 输入密度 | 控件在表单、设置面板和工具栏中的尺寸等级。 | `SizeType=Middle/Small/Custom/Large`。 |
| 内容语义 | 开关内部可显示文字或图标。 | `OnContent`、`OffContent`、对应模板。 |
| 操作反馈 | 控件可处于加载、禁用、hover、pressed 和 checked 状态。 | `IsLoading`、`:disabled`、`:pointerover`、`:pressed`、`IsChecked`。 |
| 动效语义 | 切换时把手、内容和背景平滑过渡。 | `IsMotionEnabled`、`IsWaveSpiritEnabled`。 |

`IsLoading=true` 表示开关动作正在处理。此时控件保持当前 `IsChecked` 状态，禁止继续切换，并在把手内显示加载指示。

## 公共 API

ToggleSwitch 的公共契约由 Avalonia `ToggleButton` 状态和 AtomUI 扩展属性组成。

| API | 类型 | 语义 |
| --- | --- | --- |
| `IsChecked` | `bool?` | 当前开关值，继承自 `ToggleButton`，支持三态数据绑定。 |
| `GrooveBackground` | `IBrush?` | 轨道背景，通常由主题 selector 根据 checked、hover、disabled 和 loading 状态设置。 |
| `OnContent` / `OffContent` | `object?` | checked / unchecked 状态下显示的内容。 |
| `OnContentTemplate` / `OffContentTemplate` | `IDataTemplate?` | checked / unchecked 内容模板。 |
| `SizeType` | `CustomizableSizeType` | 尺寸密度，支持 `Middle`、`Small`、`Large` 和 `Custom`。 |
| `IsLoading` | `bool` | 是否显示加载态并禁止切换。 |
| `IsMotionEnabled` | `bool` | 是否启用把手、内容偏移、背景和透明度动效。 |
| `IsWaveSpiritEnabled` | `bool` | checked 状态变化时是否播放 wave spirit。 |

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_WaveSpirit` | `WaveSpiritDecorator` | checked 变化时播放胶囊波纹。 |
| `PART_MainContainer` | `Canvas` | 承载内容 presenter 和把手，并提供裁剪边界。 |
| `PART_OnContentPresenter` | `ContentPresenter` | checked 状态下内容承载。 |
| `PART_OffContentPresenter` | `ContentPresenter` | unchecked 状态下内容承载。 |
| `PART_SwitchKnob` | `SwitchKnob` | 把手、加载指示和把手动画承载。 |

公共接口契约：

| 接口 | 语义 |
| --- | --- |
| `ICustomizableSizeTypeAware` | 接入支持 `Custom` 的尺寸模型。 |
| `ICustomHitTest` | 只在有效轨道区域命中，loading 和 disabled 时不可命中。 |
| `IWaveSpiritAwareControl` | 接入 WaveSpirit 开关。 |
| `IFormItemAware` | Form 以 `IsChecked` 作为表单值。 |

## 事件与命令

ToggleSwitch 的事件与命令以控件文档、源码 public surface 和 Avalonia 基类契约为准；生成器不从源码发明额外事件。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchShowCase.axaml:62`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel HorizontalAlignment="Left" Spacing="10" Orientation="Horizontal">
    <atom:ToggleSwitch />
</StackPanel>
```

### 禁用

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchShowCase.axaml:75`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel HorizontalAlignment="Left" Spacing="10" Orientation="Vertical">
    <atom:ToggleSwitch x:Name="ToggleDisabledSwitch"
                       IsEnabled="{Binding IsDisabledDemoEnabled}" />
    <atom:Button ButtonType="Primary"
                 Click="HandleToggleDisabledButtonClick"
                 Content="切换禁用" />
</StackPanel>
```

### 尺寸

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchShowCase.axaml:126`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel HorizontalAlignment="Left" Spacing="10" Orientation="Vertical">
    <atom:ToggleSwitch />
    <atom:ToggleSwitch SizeType="Small" />
    <atom:ToggleSwitch Name="CustomSizeTypeToggleSwitch"
                       SizeType="Custom"
                       IsChecked="True"
                       OnContent="自定义"
                       OffContent="自定义" />
</StackPanel>
```

### 加载中

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchShowCase.axaml:145`

Gallery key：`ExamplesContent` / item `4`

```axaml
<StackPanel HorizontalAlignment="Left" Spacing="10" Orientation="Vertical">
    <atom:ToggleSwitch IsLoading="{Binding IsLoadingDemoLoading}" IsChecked="true" x:Name="ToggleSwitchDefault" />
    <atom:ToggleSwitch SizeType="Small" IsLoading="{Binding IsLoadingDemoLoading}" x:Name="ToggleSwitchSmall" />
    <atom:Button ButtonType="Primary"
                 Click="HandleToggleLoadingButtonClick"
                 Content="切换加载" />
</StackPanel>
```

## 状态模型

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

## 主题与 Design Token

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

Token 来源：

ToggleSwitchToken 是 ToggleSwitch 的控件级 Token scope，描述轨道尺寸、把手尺寸、内容边距、图标尺寸、开关颜色、禁用透明度、内容字体、把手阴影和加载指示。

ToggleSwitchToken 不承载以下状态：

- `IsChecked`、`IsLoading`、`IsPressed`、`IsPointerOver`、`IsEnabled` 等实例状态。
- `OnContent`、`OffContent` 或内容模板。
- Form value、校验状态或业务异步状态。
- 当前把手位置、内容偏移、加载旋转角度或动画运行状态。

## AOT 与裁剪注意事项

ToggleSwitch 不通过反射访问模板结构。模板结构由稳定 part 和 AXAML `TemplateBinding` 表达。

资源与生命周期边界：

- on/off 图标内容的 relay binding 必须在内容替换时释放。
- `IsLoading` 设置的 cursor local value 必须在退出 loading 时 dispose。
- `SwitchKnob` 的 loading `CancellationTokenSource` 必须在停止 loading 和 detach 时释放。
- 隐藏祖先下的 loading animation 不得继续推进 animation clock。
- Token 只表达尺寸、颜色、阴影、字体和加载动画周期，不承载 `IsChecked`、`IsLoading` 或内容实例状态。

当前图标内容绑定使用 C# relay binding，因为目标对象来自用户提供的 runtime content，不是稳定模板 part。模板内部固定关系应继续优先使用 AXAML binding 和 selector。

AOT 边界：

- Token 类型通过 generator 显式注册。
- API 与 Token 契约由控件文档、源码 public surface、Token 类型或生成数据维护。
- 文档中列出的 part 名称必须与 AXAML 和 C# 查找代码一致。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Switch/ToggleSwitch.cs`：public 桌面控件，注册 `ToggleSwitchToken` resource scope。
- `src/AtomUI.Controls/Switch/AbstractToggleSwitch.cs`：公共 API、测量、布局、内容绑定、loading、hit test、Form 和渲染主逻辑。
- `src/AtomUI.Controls/Switch/SwitchKnob.cs`：把手绘制、加载指示绘制、把手宽度动画和加载动画生命周期。
- `src/AtomUI.Desktop.Controls/Switch/ToggleSwitchToken.cs`：ToggleSwitch 控件级 Token。
- `src/AtomUI.Desktop.Controls/Switch/Themes/ToggleSwitchTheme.axaml`：主模板、状态 selector、SizeType 分支和 Token 引用。
- `src/AtomUI.Desktop.Controls/Switch/Themes/SwitchKnobTheme.axaml`：把手主题、加载动画周期和动效。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/toggle-switch/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/toggle-switch/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/data-entry/toggle-switch/semantic-part.md`
- Token 文档：`docs/controls/desktop/data-entry/toggle-switch/token.md`
- 变更记录：`docs/controls/desktop/data-entry/toggle-switch/changelog.md`
- 语义结构：`./semantic-cn.md`
