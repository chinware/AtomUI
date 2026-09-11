# DropdownButton

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

DropdownButton 是 AtomUI 桌面控件体系中的下拉按钮，用于从一个动作按钮展开次级命令或菜单内容。

DropdownButton 不负责拆分主次命令、完整 Menu 导航或上下文菜单。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Buttons`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Navigation/DropdownButton` |
| 状态 | Stable |

## 何时使用

DropdownButton 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | DropdownButton 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | DropdownButton 是 AtomUI 桌面控件体系中的下拉按钮，用于从一个动作按钮展开次级命令或菜单内容。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | 继承 Button 的 `Content`、`Icon`、`Command` 等动作入口。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | SharedToken / 关联控件 Token + ControlTheme。 |

## 公共 API

DropdownButton 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content`、`Icon`、`Command` | 继承 Button 的展示内容、图标和动作命令入口。 |
| 交互与状态 | `IsArrowVisible`、`IsPointAtCenter`、`IsShowOpenIndicator`、`ShouldUseOverlayPopup` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `IconWidth`、`IconHeight`、`MarginToAnchor`、`Placement`、`PlacementAnchor`、`PlacementGravity` | 继承 Button 的用户图标和 loading 图标尺寸入口，并管理下拉定位、密度和模板视觉变量。 |
| 弹层与窗口 | `DropdownFlyout` | 控制 popup、flyout、dialog、window 或 overlay 宿主协作。 |
| 动效与异步 | `MouseEnterDelay`、`MouseLeaveDelay` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `OpenIndicator`、`TriggerType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

稳定事件包括 `MenuItemClicked`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`DropdownButton`、`Button`、`MenuFlyout`、`FlyoutStateHelper`。
- 枚举：无。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ButtonIcon` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_LoadingIcon` | `Icon` | 展示继承自 Button loading 状态的图标。 |
| `PART_ContentPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_DropdownIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_RootLayout` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_WaveSpirit` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

DropdownButton 继承 Button 的 `IconWidthProperty`、`IconHeightProperty` 及 CLR wrapper。`PART_ButtonIcon` 与 `PART_LoadingIcon` 通过 `TemplateBinding` 消费这两个属性，保持与 Button 相同的 SizeType 默认值、本地值优先级和 icon-only loading 状态语义。`PART_DropdownIndicator` 展示的 `OpenIndicator` 是独立图标角色，不受 `IconWidth`、`IconHeight` 控制。

## 事件与命令

DropdownButton 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
| 内容与数据 | `Content`、`Icon`、`Command` | 继承 Button 的展示内容、图标和动作命令入口。 |
稳定事件包括 `MenuItemClicked`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

- `controlgallery/AtomUIGallery/ShowCases/Navigation/DropdownButton/Views/DropdownButtonShowCase.axaml`

## 状态模型

DropdownButton 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- open/close、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

DropdownButton 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `DropdownButtonBaseTheme.axaml` | 定义 DropdownButton 与 Button 共享的模板、图标尺寸和基础状态视觉。 |
| `DropdownButtonTheme.axaml` | 定义 DropdownButton 的下拉指示器和具体视觉入口。 |

DropdownButton 当前没有专属 Token 文档；主题通过 SharedToken、关联控件 Token 或继承主题资源表达视觉语义。运行时状态不得写入 Token 模型。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Native 和 Browser 支持宿主必须使用同一套 DropdownButton 主题资产；平台差异不能通过 Browser 专用主题分叉复制视觉。
- 用户 icon 与 loading icon 的尺寸只能从 DropdownButton 自身的 `IconWidth`、`IconHeight` 投影；应用和 Gallery 不得通过 `/template/` 或 `PART_ButtonIcon`、`PART_LoadingIcon` selector 修改内部尺寸。
- `OpenIndicator` 尺寸由 DropdownButton 主题单独管理，不复用用户 icon 的宽高属性。

Token 来源：

- DropdownButton 当前没有专属 `token.md`；LLMS 生成按第 5 节视觉与主题模型、SharedToken、控件家族 Token 或主题资源说明 Token 边界。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 图标尺寸使用 owner StyledProperty、Style 默认值与 `TemplateBinding` 单向投影，不为模板 part 建立运行时订阅或查找同步。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/DropdownButton/DropdownButton.cs`
- `src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonBaseTheme.axaml`
- `src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonTheme.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/navigation/dropdown-button/overview.md`
- 实现文档：`docs/controls/desktop/navigation/dropdown-button/implementation.md`
- 变更记录：`docs/controls/desktop/navigation/dropdown-button/changelog.md`
- 语义结构：`./semantic-cn.md`
