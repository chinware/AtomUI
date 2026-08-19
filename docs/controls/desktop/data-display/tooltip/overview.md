# Tooltip 桌面版架构设计

本文档定义 `Tooltip` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/development/control-development-guidelines.md)，内部实现原理见 [Tooltip 桌面版实现原理](implementation.md)，Tooltip Token 的专项设计见 [Tooltip Token 设计](token.md)，设计和契约变化记录见 [Tooltip Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tooltip` |
| 控件状态 | Stable |

Tooltip 是 AtomUI 桌面控件体系中的工具提示控件，用于在目标元素附近展示短文本或轻量说明。

Tooltip 不负责复杂弹层、菜单或模态对话框。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Tooltip`

## 2. 设计语言

Tooltip 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Tooltip 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Tooltip 是 AtomUI 桌面控件体系中的工具提示控件，用于在目标元素附近展示短文本或轻量说明。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | 继承 ToolTip 的 `Content` 与目标控件 tooltip 绑定。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | motion。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Tooltip Token + ControlTheme。 |

## 3. API 与契约模型

Tooltip 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content` | 继承 ToolTip 的轻量说明内容入口。 |
| 交互与状态 | `IsMotionEnabled` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 宿主内容与呈现 | `Tip`、`TipHostWidth`、`PresetColor`、`Color`、`IsArrowVisible` | 以附加属性配置在任意目标 `Control` 上，目标控件是这些值的 owner。 |
| 宿主文本布局 | `TextWrapping`、`TextTrimming` | 附加属性；控制 Tip 文本在最大宽度约束内的换行与截断行为，默认 `Wrap` / `None`。 |
| 宿主定位与时机 | `Placement`、`HorizontalOffset`、`VerticalOffset`、`MarginToAnchor`、`IsPointAtCenter`、`ShowDelay`、`BetweenShowDelay` | 附加属性；控制弹层相对宿主的定位与悬停出现时机。 |
| 打开状态与服务开关 | `IsOpen`、`IsCustomShowAndHide`、`ServiceEnabled`、`ShowOnDisabled`、`IsUseOverlayHost` | `IsOpen` 是声明式期望打开状态（见第 4.1 节），赋值时序不影响最终物理状态。 |

公开路由事件（Direct 路由，挂接在目标控件上）：

| 事件 | 语义 |
| --- | --- |
| `ToolTipOpening` | 打开前引发，可取消；否决时把 `IsOpen` 回写为 `false`。 |
| `ToolTipClosing` | 关闭前引发。 |

主要公开类型与枚举：

- 类型：`ToolTip`、`ToolTipPseudoClass`。
- 枚举：无。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ArrowDecorator` | `ArrowDecoratedBox` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ContentPresenter` | `ContentPresenter` | 展示用户内容、文本、图标或模板化数据。 |

控件专属或内部伪类包括 `Open=:open`、`ToolTipPseudoClass.Open`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 4. 行为与状态模型

Tooltip 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- motion 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

### 4.1 打开状态调和模型

`IsOpen` 附加属性表达期望打开状态，不是赋值瞬间执行的命令。期望打开的完整条件是：`IsOpen` 为 `true`、`Tip` 内容就绪、宿主控件已挂入 visual tree。物理弹层的开关由唯一调和流程在这些输入变化时统一兑现，赋值时序不影响最终结果：

- 满足期望条件且弹层未打开：先引发可取消的 `ToolTipOpening`；被取消时把 `IsOpen` 回写为 `false`，否则打开弹层。
- `IsOpen` 转为 `false` 且弹层已打开：关闭弹层并引发 `ToolTipClosing`。
- 宿主控件从 visual tree 卸载：物理关闭弹层但保留 `IsOpen`，重新挂入后由调和流程自动重开。
- `Tip` 未就绪不重置 `IsOpen`；内容就绪后调和流程自动完成打开。
- 弹层被外部原因关闭时，`IsOpen` 回写为 `false`，期望状态与实际状态保持一致。

悬停打开由 `ToolTipService` 驱动同一个 `IsOpen` 属性；编程式声明与悬停交互共享同一状态 owner 和调和出口，不存在第二套开关路径。

## 5. 视觉与主题模型

Tooltip 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `ToolTipTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Tooltip 使用 `ToolTipToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 motion 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

## 6. 控件家族或集成关系

Tooltip 与同分类控件共享尺寸、状态、Token、Gallery 展示和验证规则。组合或派生控件应显式说明哪些 API 被继承、覆盖或不支持。

主要协作类型：

- `ToolTip`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ToolTipPseudoClass`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ToolTipToken`：控件 Token scope，负责从全局 token 派生控件语义变量。

集成关系：

- 与 ThemeManager、SharedToken、ControlTheme、控件文档和 Gallery ShowCase 示例保持一致。
- 涉及 ItemsSource、Popup、Flyout、Window、Form 或 CompactSpace 的路径必须保持生命周期释放和数据状态同步。
- 源码目录中的共享基类和内部协作类型形成维护边界，不能只修改桌面包装类而忽略共享状态 owner。

## 7. 兼容性不变量

维护 Tooltip 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- `IsOpen` 的声明式语义保持稳定：赋值时序无关、`ToolTipOpening` 否决回写、弹层外部关闭回写、宿主 detach/reattach 自动重开。
- Tip 文本的默认布局语义保持稳定：内容受 `ToolTipMaxWidth` 约束，超出时在约束内换行（`TextWrapping.Wrap`）而不是被裁剪或截断。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 动效模型

Tooltip 的动效只表达状态变化反馈，不应改变 public API 语义。初始加载、禁用态和卸载路径应能抑制或取消动效，避免保留旧控件实例。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [Tooltip 桌面版实现原理](implementation.md)
- [Tooltip Token 设计](token.md)
- [Tooltip Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Tooltip` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + `implementation.md` + `token.md` + Gallery ShowCase | 生成 `controls/tooltip/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/tooltip/semantic-cn.md` |
| API 表 | overview.md 语义摘要 + 源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | token.md、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和继承语义。 |
| 状态模型 | 覆盖 motion、disabled、hover、pressed、focus 以及控件特有状态。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和 Browser 主题。 |
| Token | 检查 TokenKind、AXAML token resource、Token 类型、生成数据和 token.md和文档同步。 |
| Gallery | 走查对应 ShowCase 示例和源码片段入口。 |
