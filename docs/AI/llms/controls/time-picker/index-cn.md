# TimePicker

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

TimePicker 是 AtomUI 桌面控件体系中的时间选择控件，用于在输入壳体和时间面板之间选择时分秒。

TimePicker 不负责日期选择、时区转换或业务排班模型。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/TimePicker`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker` |
| 状态 | Stable |

## 何时使用

TimePicker 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | TimePicker 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | TimePicker 是 AtomUI 桌面控件体系中的时间选择控件，用于在输入壳体和时间面板之间选择时分秒。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `IsShowHeader`、`ItemFormat`、`ItemHeight`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | TimePicker Token + ControlTheme。 |

## 公共 API

TimePicker 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `IsShowHeader`、`ItemFormat`、`ItemHeight` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `RangeEndSelectedTime`、`RangeStartSelectedTime`、`SelectedTime`、`SelectorRowCount` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsNeedConfirm`、`IsShowNow`、`ShouldLoop` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 其他稳定入口 | `ClockIdentifier`、`DefaultTime`、`MinuteIncrement`、`PanelType`、`PickerDisplayTime`、`RangeEndDefaultTime`、`RangeStartDefaultTime`、`SecondIncrement` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

`SelectedTime` 是 `TimePicker` 的受控 Form 值属性，默认绑定模式为 `TwoWay`，并启用 Avalonia data validation。Form、绑定验证和输入壳体错误视觉必须基于 `DataValidationErrors` 投射，不允许另建与 native validation 并行的错误状态。

主要公开类型与枚举：

- 类型：`CellDbClickedEventArgs`、`CellHoverEventArgs`、`DateTimePickerPanel`、`RangeTimePicker`、`TimePicker`、`TimePickerPresenter`、`TimeSelectedEventArgs`、`TimeView`、`TimeViewCell`、`en_US`、`zh_CN`、`zh_TW`。
- 枚举：`ClockIdentifierType`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ButtonsFrame` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ButtonsLayout` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ConfirmButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ContentPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_FirstSpacer` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_HeaderText` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_HourHost` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_HourSelector` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_MainFrame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_MainLayout` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_MinuteHost` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_MinuteSelector` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_NowButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_PeriodHost` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_PeriodSelector` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_PickerContainer` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_RootLayout` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_SecondHost` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_SecondSelector` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_SecondSpacer` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ThirdSpacer` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_TimeView` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

TimePicker 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。
- 类型：`CellDbClickedEventArgs`、`CellHoverEventArgs`、`DateTimePickerPanel`、`RangeTimePicker`、`TimePicker`、`TimePickerPresenter`、`TimeSelectedEventArgs`、`TimeView`、`TimeViewCell`、`en_US`、`zh_CN`、`zh_TW`。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Views/TimePickerShowCase.axaml:35`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:TimePicker PlaceholderText="选择时间" IsNeedConfirm="False" IsShowNow="True" />
```

### SelectedTime 绑定

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Views/TimePickerShowCase.axaml:47`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical" Spacing="16">
    <StackPanel Orientation="Vertical" Spacing="12">
        <atom:TimePicker SelectedTime="{Binding BoundSelectedTime}"
                         PlaceholderText="选择时间" />
        <StackPanel Orientation="Horizontal" Spacing="8">
            <atom:TextBlock VerticalAlignment="Center"
                            Text="选中值：" />
            <atom:TextBlock VerticalAlignment="Center"
                            Text="{Binding BoundSelectedTimeText}" />
        </StackPanel>
        <StackPanel Orientation="Horizontal" Spacing="8">
            <atom:Button SizeType="Small"
                         Click="SetBoundSelectedTimeToNoon"
                         Content="设置为中午" />
            <atom:Button SizeType="Small"
                         Click="ClearBoundSelectedTime"
                         Content="清空" />
        </StackPanel>
    </StackPanel>
    <StackPanel Orientation="Vertical" Spacing="12">
        <atom:RangeTimePicker RangeStartSelectedTime="{Binding BoundRangeStartSelectedTime}"
                              RangeEndSelectedTime="{Binding BoundRangeEndSelectedTime}"
                              PlaceholderText="开始时间"
                              SecondaryPlaceholderText="结束时间" />
        <StackPanel Orientation="Horizontal" Spacing="8">
            <atom:TextBlock VerticalAlignment="Center"
                            Text="选中范围：" />
            <atom:TextBlock VerticalAlignment="Center"
                            Text="{Binding BoundRangeSelectedTimeText}" />
        </StackPanel>
        <StackPanel Orientation="Horizontal" Spacing="8">
            <atom:Button SizeType="Small"
                         Click="SetBoundSelectedTimeRangeToWorkHours"
                         Content="设置工作时间" />
            <atom:Button SizeType="Small"
                         Click="ClearBoundSelectedTimeRange"
                         Content="清空" />
        </StackPanel>
    </StackPanel>
</StackPanel>
```

### 弹出面板显示时间

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Views/TimePickerShowCase.axaml:98`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:TimePicker PlaceholderText="选择时间"
```

### 12 小时和 24 小时格式

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Views/TimePickerShowCase.axaml:111`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:TimePicker PlaceholderText="选择时间" IsNeedConfirm="True" IsShowNow="True"
```

## 状态模型

TimePicker 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `SelectedTime` 是单时间选择的唯一用户值 owner；外部绑定、Form set/get、清除和弹层提交都必须收敛到该属性。
- `PickerDisplayTime` 只定义弹出面板打开时的显示锚点；它不得写入 `SelectedTime`，也不得改变 `DefaultTime` 的 reset 语义。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

TimePicker 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `RangeTimePickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TimePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |
| `TimePickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TimePickerThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `TimeViewCellTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TimeViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

TimePicker 使用 `TimePickerToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

TimePicker Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TimePickerToken`，scope id 为 `TimePicker`，源码位于 `src/AtomUI.Desktop.Controls/TimePicker/TimePickerToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/TimePicker/Localization/en_US.cs`
- `src/AtomUI.Desktop.Controls/TimePicker/Localization/zh_CN.cs`
- `src/AtomUI.Desktop.Controls/TimePicker/Localization/zh_TW.cs`
- `src/AtomUI.Desktop.Controls/TimePicker/RangeTimePicker.cs`
- `src/AtomUI.Desktop.Controls/TimePicker/Themes/RangeTimePickerTheme.axaml`
- `src/AtomUI.Desktop.Controls/TimePicker/Themes/TimePickerPresenterTheme.axaml`
- `src/AtomUI.Desktop.Controls/TimePicker/Themes/TimePickerTheme.axaml`
- `src/AtomUI.Desktop.Controls/TimePicker/Themes/TimePickerThemes.axaml`
- `src/AtomUI.Desktop.Controls/TimePicker/Themes/TimeViewCellTheme.axaml`
- `src/AtomUI.Desktop.Controls/TimePicker/Themes/TimeViewTheme.axaml`
- `src/AtomUI.Desktop.Controls/TimePicker/TimePicker.cs`
- `src/AtomUI.Desktop.Controls/TimePicker/TimePickerPresenter.cs`
- `src/AtomUI.Desktop.Controls/TimePicker/TimePickerToken.cs`
- `src/AtomUI.Desktop.Controls/TimePicker/TimeView/DateTimePickerPanel.cs`
- `src/AtomUI.Desktop.Controls/TimePicker/TimeView/TimeView.cs`
- `src/AtomUI.Desktop.Controls/TimePicker/TimeView/TimeViewCell.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/time-picker/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/time-picker/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/time-picker/token.md`
- 变更记录：`docs/controls/desktop/data-entry/time-picker/changelog.md`
- 语义结构：`./semantic-cn.md`
