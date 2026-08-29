# RadioButton

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

RadioButton 控件家族用于在互斥选项集合中选择一个值。`RadioButton` 提供标准单选指示器，`RadioButtonGroup` 管理普通选项组，`OptionButton` 将选项呈现为按钮，`OptionButtonGroup` 管理共享边框的按钮组选项。

RadioButton 不负责多选集合、开关语义或复杂导航菜单。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/RadioButton`
- `src/AtomUI.Controls/RadioButton`
- `src/AtomUI.Desktop.Controls/OptionButtonGroup`
- `src/AtomUI.Controls/OptionButtonGroup`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton` |
| 状态 | Stable |

## 何时使用

RadioButton 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | RadioButton 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | 普通单选指示器与按钮式单选组共享互斥选择语义。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `CheckedItem`、`SelectedItem`、`Items`、`ItemsSource`、`ItemTemplate`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | checked/selected、disabled、pointer、motion、ButtonStyle 和方向组合状态。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | RadioButtonToken、OptionButtonToken 与对应 ControlTheme。 |

## 公共 API

RadioButton 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content`、`Icon`、`Items`、`ItemsSource`、`ItemTemplate` | 定义选项内容、图标、数据和模板入口。 |
| 选择与集合 | `IsChecked`、`CheckedItem`、`SelectedIndex`、`SelectedItem` | 维护普通单选组和按钮式单选组的当前值。 |
| 交互与状态 | `IsEnabled`、`IsMotionEnabled`、`IsWaveSpiritEnabled`、`ButtonStyle` | 表达可用性、动效和 Outline/Solid 状态视觉。 |
| 视觉与布局 | `Orientation`、`ItemSpacing`、`LineSpacing`、`SizeType`、`CornerRadius`、`BorderThickness` | 控制普通组排列、按钮组方向、尺寸和组合几何。 |

`RadioButtonGroup.CheckedChanged` 通知普通组当前项变化；`OptionButtonGroup.OptionCheckedChanged` 通知按钮组选项进入 checked 状态。两者不互相代理，继承的选择和输入事件继续遵循 Avalonia 事件语义。

主要公开类型与枚举：

- 类型：`AbstractRadioButton`、`AbstractRadioButtonGroup`、`RadioButton`、`RadioButtonGroup`、`RadioButtonGroupCheckedChangedEventArgs`、`RadioButtonOption`、`RadioIndicator`、`AbstractOptionButton`、`AbstractOptionButtonGroup`、`OptionButton`、`OptionButtonGroup`、`OptionButtonData`、`OptionCheckedChangedEventArgs`。
- 枚举：`OptionButtonStyle`、`OptionButtonPositionTrait`。`OptionButtonPositionTrait` 用于组合位置协作，不作为 Group 的方向配置入口。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ItemsPresenter` | `ItemsPresenter` | 承载 RadioButtonGroup 或 OptionButtonGroup 的集合容器。 |
| `PART_WaveSpirit` | `WaveSpiritDecorator` | 承载 RadioButton 或 OptionButton 的点击 Wave，使用有效圆角。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

RadioButton 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
`RadioButtonGroup.CheckedChanged` 通知普通组当前项变化；`OptionButtonGroup.OptionCheckedChanged` 通知按钮组选项进入 checked 状态。两者不互相代理，继承的选择和输入事件继续遵循 Avalonia 事件语义。
- 类型：`AbstractRadioButton`、`AbstractRadioButtonGroup`、`RadioButton`、`RadioButtonGroup`、`RadioButtonGroupCheckedChangedEventArgs`、`RadioButtonOption`、`RadioIndicator`、`AbstractOptionButton`、`AbstractOptionButtonGroup`、`OptionButton`、`OptionButtonGroup`、`OptionButtonData`、`OptionCheckedChangedEventArgs`。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Views/RadioButtonShowCase.axaml:36`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel HorizontalAlignment="Left" Spacing="10" Orientation="Horizontal">
    <atom:RadioButton Content="单选框" />
</StackPanel>
```

### 禁用

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Views/RadioButtonShowCase.axaml:49`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel HorizontalAlignment="Left" Orientation="Vertical">
    <StackPanel Orientation="Horizontal">
        <atom:RadioButton x:Name="ToggleDisabledRadioUnChecked"
                          IsEnabled="{Binding ToggleDisabledRadioUnCheckedEnabled}"
                          Content="单选框 1" />
        <atom:RadioButton x:Name="ToggleDisabledRadioChecked"
                          IsChecked="True"
                          IsEnabled="{Binding ToggleDisabledRadioCheckedEnabled}"
                          Content="单选框 2" />
    </StackPanel>
    <atom:Button ButtonType="Primary"
                 x:Name="ToggleDisabledButton"
                 Margin="0, 20, 0, 0"
                 Command="{Binding ToggleDisabledCommand}"
                 Content="切换禁用" />
</StackPanel>
```

### 垂直单选框组

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Views/RadioButtonShowCase.axaml:111`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:RadioButtonGroup Orientation="Vertical" HorizontalAlignment="Left">
    <atom:RadioButton Content="选项 A" />
    <atom:RadioButton Content="选项 B" />
    <atom:RadioButton Content="选项 C" />
    <atom:RadioButton Content="选项 D" />
</atom:RadioButtonGroup>
```

### 通过 ItemsSource 生成单选框组

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Views/RadioButtonShowCase.axaml:127`

Gallery key：`ExamplesContent` / item `4`

```axaml
<atom:RadioButtonGroup HorizontalAlignment="Left"
```

## 状态模型

RadioButton 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `RadioButtonGroup.CheckedItem` 是单选组的外部值 owner，默认 `BindingMode.TwoWay` 并启用 Avalonia data validation；用户选择和 ViewModel 更新必须收敛到同一份当前项状态。
- `OptionButtonGroup` 以 SelectingItemsControl 的选择状态作为按钮组选中 source of truth，用户 checked、键盘导航和外部 `SelectedIndex` / `SelectedItem` 必须收敛到同一选择。
- `OptionButtonGroup.Orientation` 是排列方向、组合圆角、分隔线方向和方向键导航的唯一 owner，默认值为 `Horizontal`。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

RadioButton 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `RadioButtonGroupTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `RadioButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `RadioIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `OptionButtonGroupTheme.axaml` | 定义按钮组 ItemsPresenter、方向布局入口、尺寸和组级边框资源。 |
| `OptionButtonTheme.axaml` | 定义按钮内容、Outline/Solid、checked/disabled、方向对齐和 Wave 视觉。 |

普通单选控件使用 `RadioButtonToken`，按钮式选项使用 `OptionButtonToken`。Token 只表达组件视觉语义，不承载 checked/selected、Orientation、GroupPositionTrait 或 EffectiveCornerRadius 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

RadioButton 和 OptionButton Token 只表达组件级视觉变量，例如指示器尺寸、内容 Padding、字体和交互状态颜色。Token 不承载 checked/selected、Orientation、GroupPositionTrait、EffectiveCornerRadius 或容器 Bounds 等运行时状态。

当前 Token scope：

- `RadioButtonToken`，scope id 为 `RadioButton`，源码位于 `src/AtomUI.Desktop.Controls/RadioButton/RadioButtonToken.cs`。
- `OptionButtonToken`，scope id 为 `OptionButton`，源码位于 `src/AtomUI.Desktop.Controls/OptionButtonGroup/OptionButtonToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。
- OptionButtonGroup 的方向和容器位置更新为 O(realized item count)，只在方向或集合结构变化时执行。
- Group renderer 保持 O(item count)，不得在 Render 热路径创建容器列表、事件订阅或方向策略对象。
- 标准 StackPanel 负责横向自然宽度与纵向等宽排列，不为方向能力新增 VisualTree 层级。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/RadioButton/RadioButton.cs`
- `src/AtomUI.Desktop.Controls/RadioButton/RadioButtonGroup.cs`
- `src/AtomUI.Desktop.Controls/RadioButton/RadioButtonToken.cs`
- `src/AtomUI.Desktop.Controls/RadioButton/Themes/RadioButtonGroupTheme.axaml`
- `src/AtomUI.Desktop.Controls/RadioButton/Themes/RadioButtonTheme.axaml`
- `src/AtomUI.Desktop.Controls/RadioButton/Themes/RadioIndicatorTheme.axaml`
- `src/AtomUI.Controls/RadioButton/AbstractRadioButton.cs`
- `src/AtomUI.Controls/RadioButton/AbstractRadioButtonGroup.cs`
- `src/AtomUI.Controls/RadioButton/RadioButtonGroupCheckedChangedEventArgs.cs`
- `src/AtomUI.Controls/RadioButton/RadioButtonGroupManager.cs`
- `src/AtomUI.Controls/RadioButton/RadioButtonOption.cs`
- `src/AtomUI.Controls/RadioButton/RadioIndicator.cs`
- `src/AtomUI.Desktop.Controls/OptionButtonGroup/OptionButton.cs`
- `src/AtomUI.Desktop.Controls/OptionButtonGroup/OptionButtonGroup.cs`
- `src/AtomUI.Desktop.Controls/OptionButtonGroup/OptionButtonToken.cs`
- `src/AtomUI.Desktop.Controls/OptionButtonGroup/Themes/OptionButtonGroupTheme.axaml`
- `src/AtomUI.Desktop.Controls/OptionButtonGroup/Themes/OptionButtonTheme.axaml`
- `src/AtomUI.Controls/OptionButtonGroup/AbstractOptionButton.cs`
- `src/AtomUI.Controls/OptionButtonGroup/AbstractOptionButtonGroup.cs`
- `src/AtomUI.Controls/OptionButtonGroup/OptionButtonData.cs`
- `src/AtomUI.Controls/OptionButtonGroup/OptionButtonGroupEnums.cs`
- `src/AtomUI.Controls/OptionButtonGroup/OptionCheckedChangedEventArgs.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/radio-button/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/radio-button/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/radio-button/token.md`
- 变更记录：`docs/controls/desktop/data-entry/radio-button/changelog.md`
- 语义结构：`./semantic-cn.md`
