# Tooltip 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Tooltip` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外运行时组合层。

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content` | 继承 ToolTip 的轻量说明内容入口。 |
| 交互与状态 | `IsMotionEnabled` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 宿主内容与呈现 | `Tip`、`TipHostWidth`、`PresetColor`、`Color`、`IsArrowVisible` | 以附加属性配置在任意目标 `Control` 上，目标控件是这些值的 owner。 |
| 宿主文本布局 | `TextWrapping`、`TextTrimming` | 附加属性；控制 Tip 文本在最大宽度约束内的换行与截断行为，默认 `Wrap` / `None`。 |
| 宿主定位与时机 | `Placement`、`HorizontalOffset`、`VerticalOffset`、`MarginToAnchor`、`IsPointAtCenter`、`ShowDelay`、`BetweenShowDelay` | 附加属性；控制弹层相对宿主的定位与悬停出现时机。 |
| 打开状态与服务开关 | `IsOpen`、`IsCustomShowAndHide`、`ServiceEnabled`、`ShowOnDisabled`、`IsUseOverlayHost` | `IsOpen` 是声明式期望打开状态（见第 4.1 节），赋值时序不影响最终物理状态。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | motion。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Tooltip Token + ControlTheme。 |

## State Flow

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

## Theme and Token Boundaries

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

Token 边界：

Tooltip Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ToolTipToken`，scope id 为 `ToolTip`，源码位于 `src/AtomUI.Desktop.Controls/Tooltip/ToolTipToken.cs`。

## Customization Boundaries

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

维护不变量：

维护 Tooltip 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
- TextBox 内部按钮、feedback、padding 或模板重套用改变有效 viewport 时，OverflowTip 必须由度量通知重新判断，不能依赖 owner Bounds 恰好变化。
- `IsOpen`、`Tip` 与宿主 attach/detach 变化只能经调和入口影响弹层物理开关；不新增第二条直接开关 popup 的路径。
- 只有 `ToolTipOpening` 否决和弹层外部关闭可以回写 `IsOpen=false`。
