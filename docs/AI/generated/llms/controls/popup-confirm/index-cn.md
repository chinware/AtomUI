# PopupConfirm

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

PopupConfirm 是 AtomUI 桌面控件体系中的气泡确认框控件，用于在目标元素附近完成轻量确认。

PopupConfirm 不负责完整 Dialog、复杂表单或菜单系统。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/PopupConfirm`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Feedback/PopupConfirm` |
| 状态 | Stable |

## 何时使用

PopupConfirm 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | PopupConfirm 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | PopupConfirm 是 AtomUI 桌面控件体系中的气泡确认框控件，用于在目标元素附近完成轻量确认。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `CancelText`、`ConfirmContent`、`ConfirmContentTemplate`、`Icon`、`OkText`、`Title`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | input/value。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | PopupConfirm Token + ControlTheme。 |

## 公共 API

PopupConfirm 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CancelText`、`ConfirmContent`、`ConfirmContentTemplate`、`Icon`、`OkText`、`Title` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `ConfirmStatus`、`IsShowCancelButton` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 其他稳定入口 | `OkButtonType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

稳定事件包括 `Cancelled`、`Confirmed`、`PopupClick`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`PopupConfirm`、`PopupConfirmClickEventArgs`、`PopupConfirmContainer`、`PopupConfirmFlyout`、`PopupConfirmPseudoClass`。
- 枚举：`PopupConfirmStatus`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ButtonLayout` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_CancelButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_Content` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ContentPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_IconPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_MainLayout` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_OkButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_Title` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

控件专属或内部伪类包括 `EmptyContent=:empty-content`、`PopupConfirmPseudoClass.EmptyContent`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

PopupConfirm 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `Cancelled`、`Confirmed`、`PopupClick`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。
- 类型：`PopupConfirm`、`PopupConfirmClickEventArgs`、`PopupConfirmContainer`、`PopupConfirmFlyout`、`PopupConfirmPseudoClass`。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

- `controlgallery/AtomUIGallery/ShowCases/Feedback/PopupConfirm/Views/PopupConfirmShowCase.axaml`

## 状态模型

PopupConfirm 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- input/value 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

PopupConfirm 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `PopupConfirmContainerTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `PopupConfirmTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |

PopupConfirm 使用 `PopupConfirmToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 input/value 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

PopupConfirm Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `PopupConfirmToken`，scope id 为 `PopupConfirm`，源码位于 `src/AtomUI.Desktop.Controls/PopupConfirm/PopupConfirmToken.cs`。

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

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/PopupConfirm/PopupConfirm.cs`
- `src/AtomUI.Desktop.Controls/PopupConfirm/PopupConfirmContainer.cs`
- `src/AtomUI.Desktop.Controls/PopupConfirm/PopupConfirmFlyout.cs`
- `src/AtomUI.Desktop.Controls/PopupConfirm/PopupConfirmPseudoClass.cs`
- `src/AtomUI.Desktop.Controls/PopupConfirm/PopupConfirmToken.cs`
- `src/AtomUI.Desktop.Controls/PopupConfirm/Themes/PopupConfirmContainerTheme.axaml`
- `src/AtomUI.Desktop.Controls/PopupConfirm/Themes/PopupConfirmTheme.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/feedback/popup-confirm/overview.md`
- 实现文档：`docs/controls/desktop/feedback/popup-confirm/implementation.md`
- Token 文档：`docs/controls/desktop/feedback/popup-confirm/token.md`
- 变更记录：`docs/controls/desktop/feedback/popup-confirm/changelog.md`
- 语义结构：`./semantic-cn.md`
