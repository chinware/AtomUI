# TimePicker 桌面版架构设计

本文档定义 `TimePicker` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [TimePicker 桌面版实现原理](implementation.md)，TimePicker Token 的专项设计见 [TimePicker Token 设计](token.md)，设计和契约变化记录见 [TimePicker Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker` |
| 控件状态 | Stable |

TimePicker 是 AtomUI 桌面控件体系中的时间选择控件，用于在输入壳体和时间面板之间选择时分秒。

TimePicker 不负责日期选择、时区转换或业务排班模型。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/TimePicker`

## 2. 设计语言

TimePicker 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | TimePicker 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | TimePicker 是 AtomUI 桌面控件体系中的时间选择控件，用于在输入壳体和时间面板之间选择时分秒。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `IsShowHeader`、`ItemFormat`、`ItemHeight`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | TimePicker Token + ControlTheme。 |

## 3. API 与契约模型

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

## 4. 行为与状态模型

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

## 5. 视觉与主题模型

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

## 6. 控件家族或集成关系

TimePicker 与同分类控件共享尺寸、状态、Token、Gallery 展示和验证规则。组合或派生控件应显式说明哪些 API 被继承、覆盖或不支持。

主要协作类型：

- `DateTimePickerPanel`：布局面板，负责测量、排列、虚拟化或集合内容布局。
- `RangeTimePicker`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TimePicker`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TimePickerPresenter`：模板协作类型，承载内容展示、宿主或视觉边界。
- `TimePickerToken`：组件 Token scope，负责从全局 token 派生控件语义变量。
- `TimeView`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `TimeViewCell`：集合项、节点或容器类型，承载单项状态和模板协作。
- `en_US`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `zh_CN`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `zh_TW`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。

集成关系：

- 与 ThemeManager、SharedToken、ControlTheme 和 Gallery ShowCase 的示例/API/Token 表保持一致。
- 涉及 ItemsSource、Popup、Flyout、Window、Form 或 CompactSpace 的路径必须保持生命周期释放和数据状态同步。
- 源码目录中的共享基类和内部协作类型形成维护边界，不能只修改桌面包装类而忽略共享状态 owner。

## 7. 兼容性不变量

维护 TimePicker 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 选择与当前项模型

TimePicker 的当前项状态必须由单一 owner 推导。public 选择属性、集合项容器和伪类之间只能做单向同步，集合替换、清空和模板重套用时必须回放当前状态。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [TimePicker 桌面版实现原理](implementation.md)
- [TimePicker Token 设计](token.md)
- [TimePicker Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `TimePicker` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + Gallery API / Token / ShowCase | 生成 `controls/time-picker/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/time-picker/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | Gallery DesignTokenDataGrid、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和继承语义。 |
| 状态模型 | 覆盖 selection/checked/active、disabled、hover、pressed、focus 以及控件特有状态。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和 Browser 主题。 |
| Token | 检查 TokenKind、AXAML token resource、Gallery Token 表和文档同步。 |
| Gallery | 走查对应 ShowCase 示例、API 表和 Token 表入口。 |
