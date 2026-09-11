# Statistic

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Statistic 是 AtomUI 桌面控件体系中的统计数值控件，用于展示标题、数值、前后缀、计时或动态计数。

Statistic 不负责图表系统、表格聚合或实时数据订阅服务。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Statistic`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Statistic` |
| 状态 | Stable |

## 何时使用

Statistic 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Statistic 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Statistic 是 AtomUI 桌面控件体系中的统计数值控件，用于展示标题、数值、前后缀、计时或动态计数。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `AnimatingValue`、`ContentFontSize`、`ContentForeground`、`EndValue`、`Value`、`ValuePrefixAddOn`、`ValuePrefixAddOnTemplate`、`ValueSuffixAddOn` 等 9 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | loading/async、collection/filter、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Statistic Token + ControlTheme。 |

## 公共 API

Statistic 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `AnimatingValue`、`ContentFontSize`、`ContentForeground`、`EndValue`、`Value`、`ValuePrefixAddOn`、`ValuePrefixAddOnTemplate`、`ValueSuffixAddOn`、`ValueSuffixAddOnTemplate` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `GroupSeparator` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsLoading` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 动效与异步 | `RefreshDuration` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `DecimalSeparator`、`Format`、`Precision` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`AbstractStatistic`、`Statistic`、`StatisticCountUp`、`TimerStatistic`。
- 枚举：无。

稳定 template part：

当前控件没有显式 `[TemplatePart]` 契约；主题节点仍通过 ControlTheme key、资源 key 和 Gallery 可观察行为形成稳定边界。

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

Statistic 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Statistic/Views/StatisticShowCase.axaml:34`

Gallery key：`ExamplesContent` / item `0`

```axaml
<UniformGrid Columns="2" Rows="2">
    <atom:Statistic Header="活跃用户" Value="112893" />
    <StackPanel Orientation="Vertical" Spacing="16">
        <atom:Statistic Header="账户余额（CNY）" Value="112893" Precision="2" />
        <atom:Button ButtonType="Primary" Content="充值" />
    </StackPanel>
    <atom:Statistic Header="活跃用户" Value="112893" IsLoading="True" />
</UniformGrid>
```

### 动画数字

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Statistic/Views/StatisticShowCase.axaml:94`

Gallery key：`ExamplesContent` / item `3`

```axaml
<UniformGrid Columns="2" Rows="2">
    <atom:Statistic Header="活跃用户" Value="112893">
        <atom:Statistic.Content>
            <atom:StatisticCountUp EndValue="{Binding Values}" Precision="2" />
        </atom:Statistic.Content>
    </atom:Statistic>
    <atom:Statistic Header="账户余额（CNY）" Value="112893">
        <atom:Statistic.Content>
            <atom:StatisticCountUp EndValue="{Binding Values}" Precision="2" />
        </atom:Statistic.Content>
    </atom:Statistic>
</UniformGrid>
```

### 计时器

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Statistic/Views/StatisticShowCase.axaml:114`

Gallery key：`ExamplesContent` / item `4`

```axaml
<UniformGrid Columns="2" Rows="3" RowSpacing="10">
    <atom:TimerStatistic Value="{Binding Deadline}" />
    <atom:TimerStatistic Header="毫秒" Value="{Binding Deadline}" Format="hh\:mm\:ss\.fff" />
    <atom:TimerStatistic Header="倒计时" Value="{Binding TenSecondsLater}" />
    <atom:TimerStatistic Header="正计时" Value="{Binding Before}" />
    <atom:TimerStatistic Header="天级倒计时" Value="{Binding Deadline}" Format="d\ \天\ h\ \时\ m\ \分\ s\ \秒" />
    <atom:TimerStatistic Header="天级正计时" Value="{Binding Before}" Format="d\ \天\ h\ \时\ m\ \分\ s\ \秒" />
</UniformGrid>
```

## 状态模型

Statistic 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- loading/async、collection/filter、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Statistic 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AbstractStatisticTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `StatisticCountUpTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `StatisticTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TimerStatisticTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Statistic 使用 `StatisticToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 loading/async、collection/filter、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Statistic Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `StatisticToken`，scope id 为 `Statistic`，源码位于 `src/AtomUI.Desktop.Controls/Statistic/StatisticToken.cs`。

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
- 隐藏祖先下的 TimerStatistic 不运行刷新 timer；恢复时必须从绝对时间重算，不补发隐藏期间的 tick。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/Statistic/AbstractStatistic.cs`
- `src/AtomUI.Desktop.Controls/Statistic/Statistic.cs`
- `src/AtomUI.Desktop.Controls/Statistic/StatisticCountUp.cs`
- `src/AtomUI.Desktop.Controls/Statistic/StatisticToken.cs`
- `src/AtomUI.Desktop.Controls/Statistic/StatisticUtils.cs`
- `src/AtomUI.Desktop.Controls/Statistic/Themes/AbstractStatisticTheme.axaml`
- `src/AtomUI.Desktop.Controls/Statistic/Themes/AbstractStatisticTheme.cs`
- `src/AtomUI.Desktop.Controls/Statistic/Themes/StatisticCountUpTheme.axaml`
- `src/AtomUI.Desktop.Controls/Statistic/Themes/StatisticTheme.axaml`
- `src/AtomUI.Desktop.Controls/Statistic/Themes/TimerStatisticTheme.axaml`
- `src/AtomUI.Desktop.Controls/Statistic/TimerStatistic.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/statistic/overview.md`
- 实现文档：`docs/controls/desktop/data-display/statistic/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/statistic/token.md`
- 变更记录：`docs/controls/desktop/data-display/statistic/changelog.md`
- 语义结构：`./semantic-cn.md`
