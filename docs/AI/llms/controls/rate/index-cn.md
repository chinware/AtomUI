# Rate

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

Rate 是 AtomUI 桌面控件体系中的评分控件，用于以图标序列表达评分、半选和只读展示。

Rate 不负责进度条、投票系统或任意图标列表。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Controls/Rate`
- `src/AtomUI.Desktop.Controls/Rate`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Rate` |
| 状态 | Stable |

## 何时使用

Rate 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Rate 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Rate 是 AtomUI 桌面控件体系中的评分控件，用于以图标序列表达评分、半选和只读展示。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `DefaultValue`、`Value`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Rate Token + ControlTheme。 |

## 公共 API

Rate 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `DefaultValue`、`Value` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `Count`、`SelectedState` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsAllowClear`、`IsAllowHalf`、`IsKeyboardEnabled`、`IsMotionEnabled` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `Background`、`FontSize`、`FontStyle`、`Foreground`、`SizeType`、`StarBgColor`、`StarColor` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Character`、`FontFamily`、`FontWeight` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`AbstractRate`、`Rate`、`RateCharacter`、`RateItem`、`RateItemsControl`、`RateValueChangedEventArgs`。
- 枚举：无。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ItemsPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_RateItems` | `ItemsControl` | 承载集合项、布局面板或虚拟化内容。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

Rate 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。
- 类型：`AbstractRate`、`Rate`、`RateCharacter`、`RateItem`、`RateItemsControl`、`RateValueChangedEventArgs`。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Rate/Views/RateShowCase.axaml:36`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Rate/>
```

### 双向绑定

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Rate/Views/RateShowCase.axaml:48`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Rate Value="{Binding TwoWayValue}" />
    <atom:TextBlock Text="{Binding TwoWayValueSummary}" />
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button SizeType="Small"
                     Command="{Binding SetFourStarsCommand}"
                     Content="设为 4 星" />
        <atom:Button SizeType="Small"
                     Command="{Binding ClearTwoWayValueCommand}"
                     Content="清空" />
    </StackPanel>
</StackPanel>
```

### 半星

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Rate/Views/RateShowCase.axaml:70`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:Rate DefaultValue="3.5" IsAllowHalf="True"/>
```

### 显示文案

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Rate/Views/RateShowCase.axaml:81`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Rate Name="ToolTipRate"
               ToolTips="{Binding Tooltips}"
               ValueChanged="HandleValueChanged"/>
    <TextBlock Text="{Binding ActiveTooltip}"/>
</StackPanel>
```

## 状态模型

Rate 的状态流按以下路径收敛：

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
- `Rate.Value` 是评分控件的外部值 owner，默认 `BindingMode.TwoWay` 并启用 Avalonia data validation；用户评分、键盘调整、Form value 和 ViewModel 更新必须收敛到同一份数值状态。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Rate 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `RateItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `RateItemsControlTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `RateTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `RateThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Rate 使用 `RateToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Rate Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `RateToken`，scope id 为 `Rate`，源码位于 `src/AtomUI.Desktop.Controls/Rate/RateToken.cs`。

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

- `src/AtomUI.Controls/Rate/AbstractRate.cs`
- `src/AtomUI.Controls/Rate/RateCharacter.cs`
- `src/AtomUI.Controls/Rate/RateItem.cs`
- `src/AtomUI.Controls/Rate/RateItemsControl.cs`
- `src/AtomUI.Controls/Rate/RateValueChangedEventArgs.cs`
- `src/AtomUI.Desktop.Controls/Rate/Rate.cs`
- `src/AtomUI.Desktop.Controls/Rate/RateToken.cs`
- `src/AtomUI.Desktop.Controls/Rate/Themes/RateItemTheme.axaml`
- `src/AtomUI.Desktop.Controls/Rate/Themes/RateItemsControlTheme.axaml`
- `src/AtomUI.Desktop.Controls/Rate/Themes/RateTheme.axaml`
- `src/AtomUI.Desktop.Controls/Rate/Themes/RateThemes.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/rate/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/rate/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/rate/token.md`
- 变更记录：`docs/controls/desktop/data-entry/rate/changelog.md`
- 语义结构：`./semantic-cn.md`
