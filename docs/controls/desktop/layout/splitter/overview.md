# Splitter 桌面版架构设计

本文档定义 `Splitter` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [Splitter 桌面版实现原理](implementation.md)，Splitter Token 的专项设计见 [Splitter Token 设计](token.md)，设计和契约变化记录见 [Splitter Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Layout/Splitter` |
| 控件状态 | Stable |

Splitter 是 AtomUI 桌面控件体系中的分割面板控件，用于让用户拖拽调整相邻区域尺寸。

Splitter 不负责窗口停靠系统、路由容器或业务布局状态持久化。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Splitter`

## 2. 设计语言

Splitter 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Splitter 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Splitter 是 AtomUI 桌面控件体系中的分割面板控件，用于让用户拖拽调整相邻区域尺寸。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `CollapseNextIcon`、`CollapsePreviousIcon`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Splitter Token + ControlTheme。 |

## 3. API 与契约模型

Splitter 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CollapseNextIcon`、`CollapsePreviousIcon` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsDragEnabled`、`IsLazy` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `HandleSize`、`LineBrush`、`LineThickness`、`Orientation` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`Splitter`、`SplitterDragBar`、`SplitterHandle`、`SplitterPanel`、`SplitterPanelCollapsible`、`SplitterPanelCollapsibleConverter`、`SplitterResizeEventArgs`。
- 枚举：`SplitterCollapsibleIconDisplayMode`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_CollapseIconsHost` | `?` | 展示图标、状态图标或操作图标。 |
| `PART_CollapseNextButton` | `IconButton` | 承载用户触发入口、导航或关闭动作。 |
| `PART_CollapsePrevButton` | `IconButton` | 承载用户触发入口、导航或关闭动作。 |
| `PART_DragBar` | `SplitterDragBar` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_Grip` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_HandleLine` | `Border` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_SplitterPanel` | `?` | 承载集合项、布局面板或虚拟化内容。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 4. 行为与状态模型

Splitter 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 5. 视觉与主题模型

Splitter 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `SplitterDragBarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SplitterHandleTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `SplitterTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SplitterThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Splitter 使用 `SplitterToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

## 6. 控件家族或集成关系

Splitter 与同分类控件共享尺寸、状态、Token、Gallery 展示和验证规则。组合或派生控件应显式说明哪些 API 被继承、覆盖或不支持。

主要协作类型：

- `Splitter`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `SplitterDragBar`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `SplitterHandle`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `SplitterPanel`：布局面板，负责测量、排列、虚拟化或集合内容布局。
- `SplitterPanelCollapsible`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `SplitterPanelCollapsibleConverter`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `SplitterToken`：组件 Token scope，负责从全局 token 派生控件语义变量。

集成关系：

- 与 ThemeManager、SharedToken、ControlTheme 和 Gallery ShowCase 的示例/API/Token 表保持一致。
- 涉及 ItemsSource、Popup、Flyout、Window、Form 或 CompactSpace 的路径必须保持生命周期释放和数据状态同步。
- 源码目录中的共享基类和内部协作类型形成维护边界，不能只修改桌面包装类而忽略共享状态 owner。

## 7. 兼容性不变量

维护 Splitter 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 视觉选项模型

Splitter 的视觉选项通过 public API 归一为 theme variables、伪类或模板绑定。Token 保存组件语义值，不能保存实例运行时状态或业务色值。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [Splitter 桌面版实现原理](implementation.md)
- [Splitter Token 设计](token.md)
- [Splitter Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Splitter` | 布局控件根语义区域，承载布局 public API、尺寸和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `container` | `布局容器` | 组织子元素、间距、断点、对齐或分割状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `布局项` | 承载子内容、占位、跨度、排序或尺寸约束。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `theme` | `主题区域` | 连接 SharedToken、布局主题资源和 Gallery 可观察样式。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + Gallery API / Token / ShowCase | 生成 `controls/splitter/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/splitter/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | Gallery DesignTokenDataGrid、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和继承语义。 |
| 状态模型 | 覆盖 visual option、disabled、hover、pressed、focus 以及控件特有状态。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和 Browser 主题。 |
| Token | 检查 TokenKind、AXAML token resource、Gallery Token 表和文档同步。 |
| Gallery | 走查对应 ShowCase 示例、API 表和 Token 表入口。 |
