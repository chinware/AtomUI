# Modal 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Dialog` / `MessageBox` | public 打开意图、内容、结果和事件入口。 | `IsOpen`, `OpenAsync`, `Result`, `BeforeCloseAsync` | `DialogToken` | stable |
| `host` | Overlay presenter / native Window | 承载模态、placement、尺寸和宿主生命周期。 | `DialogHostType`, `IsModal`, `PlacementTarget` | SharedToken motion | internal-observable |
| `surface` | `DialogSurface` | 共享标题、正文、Footer、按钮和 focus scope。 | `Content`, `StandardButtons`, `IsLoading` | `ContentBg`, padding/footer tokens | internal-observable |
| `content` | Content / `MessageBoxContent` | 呈现任意 Dialog 内容或 MessageBox 语义内容。 | `Content`, `ContentTemplate`, `Style`, `Icon` | typography/color tokens | stable |
| `motion` | Overlay `MotionActor` + `PART_SurfaceContentLayer` | Overlay 等待外层、前景内容和 mask 的关闭边界；Window 使用原生 Opened/Closed 边界。 | `IsMotionEnabled` | `MotionDurationMid` | internal-observable |

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外运行时组合层。

## Template Parts

| Part | 所属类型 | 职责 |
| --- | --- | --- |
| `PART_Header` | `DialogSurface` | Overlay 标题栏、拖动与 caption 操作。 |
| `PART_ButtonBox` | `DialogSurface` | 当前标准/自定义按钮序列。 |
| `PART_Resizer` | `DialogSurface` | Overlay resize handles。 |
| `PART_LeftGroup` / `PART_CenterGroup` / `PART_RightGroup` | `DialogButtonBox` | 按按钮角色布局。 |
| `PART_MaskMotionActor` | `OverlayDialogPresenter` | modal mask 及其 motion。 |
| `PART_SurfaceMotionActor` | `OverlayDialogPresenter` | DialogSurface 入场/退出 motion。 |
| `PART_SurfaceContentLayer` | `DialogSurface` 内部模板节点 | 包围 Header、ContentFrame 和 FooterFrame；Overlay 关闭时承载前景 opacity 动画，不是 public Semantic Part。 |

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

- `IsOpen` 表示最新声明式意图；`DialogSession` 表示一次实际展示。两者不能由 presenter 或 template part 反向拥有。
- modal Overlay 的真实 pointer 输入命中 mask，modeless Overlay 在 Surface 外穿透到底层。只有栈顶 presenter 响应 mask 与 Escape。栈顶 modal mask 外点默认以 `HostCloseRequest` 发起普通关闭；`IsMaskClosable=false` 时该次点击被吞掉且不产生任何关闭请求，不进入 `Closing`/`BeforeCloseAsync` 管道。Window host 没有 mask，外点本来就不触发关闭。
- 所有平台的 `AtomUI.Window` 都把 Overlay presenter 放在 owning `TopLevel` 的 Avalonia `OverlayLayer`。modal 活跃时通过 Window 引用计数租约隐藏 managed/drawn chrome overlay，使 mask 覆盖完整 Avalonia 可绘制窗口轮廓并阻断 caption input；位于客户端 visual tree 外的原生系统 chrome 仍由平台管理。
- Dialog 内容区内的 popup 类控件(ComboBox、Select、DatePicker、Tooltip、Flyout、ContextMenu 等)由同一 Window 的 `PopupOverlayLayer` 或原生 Popup host 承载，位于 Dialog `OverlayLayer` 之上并保持可命中；二者之间的 `LightDismissOverlayLayer` 保证外点关闭与输入穿透语义和普通页面一致。层级与所有权契约见 [Modal 内容弹层叠放设计](popup-layering-design.md)。
- Overlay mask bounds、Window visible frame 与 Dialog 正文 owner bounds 独立：mask 使用完整 layer bounds；所有平台的 Surface 正文定位、拖动、resize 和 maximize 使用 visible frame 按当前有效 drawn frame thickness 内缩后的范围，允许进入 managed/drawn title bar，但不能覆盖窗口 frame。Dialog BoxShadow 只参与绘制并允许在窗口边缘由统一 visual-layer clip 裁剪。
- Enter/Escape 根据当前有效按钮序列查找 default/escape 按钮，运行时修改标准按钮或自定义按钮会立即生效。
- `IsConfirmLoading=true` 只阻止用户发起的普通关闭，不阻止 owner close、detach、取消和失败 teardown。
- 打开后焦点进入 DialogSurface；嵌套 Dialog 关闭时恢复下层 Surface，最后一层关闭时恢复原触发控件。
- Overlay 等待 mask 与 Surface 的 opening/closing motion；关闭时同一 presenter 还等待内容层 opacity 动画，并在聚合任务完成后才断开 composition children、释放 Surface 和移除 layer。`IsMotionEnabled=false` 只跳过这些 motion，不跳过宿主附加、移除和释放。Window 不创建 Surface `MotionActor`，其打开与关闭分别等待原生 `DialogWindow.Opened` 和 `DialogWindow.Closed`。
- `IsResizable=true` 允许在有效尺寸区间内交互缩放，不表示无约束 resize。结构性最小尺寸在宿主容量允许时始终保留标题、Footer 和非零正文 viewport；`HostMin*` 只能提高该下限，`HostMax*=PositiveInfinity` 仍受 owner 或 screen capacity 限制。Overlay handle 捕获 pointer，release 或 capture lost 都会完整结束当前 resize，不复用上一次拖拽 origin。

## Theme and Token Boundaries

| 主题文件 | 职责 |
| --- | --- |
| `DialogTheme.axaml` | Dialog 默认属性和 Token 映射。 |
| `DialogSurfaceTheme.axaml` | 共享标题、内容、Footer、按钮与 resize 结构。 |
| `OverlayDialogPresenterTheme.axaml` | 同一 presenter 内组合 mask 与 Surface motion。 |
| `DialogButtonBoxTheme.axaml` | 三组按钮布局。 |
| `OverlayDialogHeaderTheme.axaml` | Overlay 标题栏。 |
| `OverlayDialogMaskTheme.axaml` | modal mask。 |
| `OverlayDialogResizerTheme.axaml` | Overlay resize handles。 |
| `MessageBoxTheme.axaml` / `MessageBoxContentTheme.axaml` | MessageBox 默认尺寸和语义内容。 |

`DialogToken` 提供背景、文字、间距、尺寸和 Footer 视觉。motion duration 使用 Dialog scope 的 SharedToken `MotionDurationMid`，不在代码中硬编码。

Token 边界：

Modal Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `DialogToken`，scope id 为 `Dialog`，源码位于 `src/AtomUI.Desktop.Controls/Dialog/DialogToken.cs`。

## Customization Boundaries

后续维护必须保持当前稳定契约：

- 一次实际展示只有一个 `DialogSession`，一次关闭只有一个提交结果。
- Overlay 与 Window 使用相同内容、按钮、关闭、焦点和 teardown 语义。
- Content 可以是字符串、POCO 或 Control；实现不得修改用户 Control 的 TemplatedParent。
- 自定义按钮集合的 Add/Remove/Replace/Move/Reset/Clear 都要更新有效序列并对称管理事件订阅。
- mask、Surface、内容、按钮、binding、逻辑/资源 parent、owner/target 订阅必须在所有关闭路径释放。
- Window mask 必须覆盖完整 Avalonia 可绘制窗口轮廓；存在 drawn title bar 时必须位于其上方。所有平台的 Dialog Surface 正文都使用包含 managed/drawn 标题栏、排除透明 frame shadow 与有效 drawn frame 的 owner bounds；不能把 mask bounds、visible frame bounds、正文 owner bounds 与 BoxShadow 绘制范围合并为同一个矩形。
- Dialog 内容、placement target 与 owning Window 必须保持在同一 `TopLevel`；Dialog 使用 `OverlayLayer`，内容弹层使用更高的 `PopupOverlayLayer`，并保留中间的 light-dismiss 层。
- Window 外轮廓只能由现有 `WindowVisualLayerClip` 统一裁剪；Overlay Dialog 不单独复制 frame shadow margin 或 CornerRadius。
- Overlay 与 Window 必须使用同一套 Surface 正文尺寸解析。Window 只允许在 presenter 边界加回 chrome；不能把 Surface `HostMin/Max` 直接解释为包含标题栏和 frame 的 Window client constraints。
- 用户 resize、runtime `HostMin/Max`、主题或宿主容量变化不得无条件重置已调整尺寸；actual size 只有越出最新有效区间时才被 clamp。
- 不重新引入同步 DispatcherFrame、callback close、隐藏 MessageBox Dialog 或分离的 Popup mask。
- 关闭入口开关保持正交：`IsClosable` 管标题栏 X，`IsMaskClosable` 管 Overlay modal mask 外点，互不推导；`IsMaskClosable=false` 时 mask 外点不产生 `HostCloseRequest`。

维护不变量：

- `Dialog` 打开意图与一个当前 Session 是唯一生命周期 owner。
- 所有关闭来源最终执行同一个 `CompleteCloseAsync` teardown。
- 普通 veto 发生在结果提交前；结果提交后只允许完成 teardown 和传播异常。
- Overlay 与 Window 的 `ShowAsync`/`CloseAsync` 都等待真实 presentation 边界。
- mask 与 Surface 必须保留在同一个 Overlay presenter 中。
- Overlay 关闭时 Surface 外层、`PART_SurfaceContentLayer` 和 modal mask 的任务必须由同一个 presenter 聚合；所有任务完成前不得断开 composition children、Dispose Surface 或移除 presenter。
- `AbstractMotion` 只能在全部 transition 完成或安全 timeout 后报告 Motion 完成；不能按首个 transition 的完成通知 teardown。
- `PART_SurfaceContentLayer` 是可选内部协作节点；缺失时仅退化为外层 motion，不能阻断基本关闭流程。动画期间不得改变 Surface Bounds、布局或 visual parent。
- 所有平台的 Overlay presenter 必须保留在 owning `TopLevel` 的 `OverlayLayer`；drawn decorations overlay 只绘制 chrome，不能承载业务 presentation。
- Dialog 内容、Popup placement target 与 owning Window 必须解析到同一 `TopLevel`；Popup 使用更高的 Avalonia popup layer，并保留中间 light-dismiss 层。
- mask bounds、Window visible frame、Dialog body owner bounds 和 Dialog BoxShadow extents 必须保持独立。mask 覆盖完整 layer；所有平台的 Surface 正文都可进入 managed/drawn 标题栏但不能覆盖有效 frame；BoxShadow 允许由 Window visual-layer clip 在外轮廓处裁剪。
- Dialog 与 Drawer 共享 Window chrome suppression 的引用计数 owner，但不共享 layer、容器或 presentation 生命周期状态。
- Surface structural minimum、requested Host constraints 和 host capacity 必须由同一纯值规则解析；Overlay 与 Window 不能分别定义默认最小尺寸语义。
- Window presenter 只能在 Surface constraints 解析完成后加回 Window chrome；live resize 热路径不能重新测量结构区域。
- MessageBox 继续作为 Dialog 派生类，不增加平行 host/session/button cache 生命周期。
- mask 外点关闭入口只由 `IsMaskClosable` 在 Overlay presenter 的 mask 输入路径统一门控；不引入第二条 mask 关闭路径，也不在 Session veto 层复制该判断。
- 新增 binding、事件、资源 parent、motion source 或内容引用时，必须在同一个 owner 中增加释放点和回归测试。
