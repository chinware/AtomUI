# Popup

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Popup 是 AtomUI 桌面弹层体系的共享低层原语，继承 Avalonia `Popup` 的 anchor、placement、light-dismiss、native
window 与 overlay host 能力，并增加 AtomUI 的定位、阴影、动效、翻转通知和可选表面契约。Flyout、ToolTip、
ContextMenu、菜单、选择器和 Picker 等专用控件复用该原语，但继续拥有各自的 Presenter 和内容表面。

Popup 不提供默认 Padding、边框、箭头或内容布局。调用方负责 Child 内容结构；Popup 只在 Child bounds 内提供可选的
frame surface 与 host shadow。

Popup 同时拥有弹层家族的内部钉住打开物理契约。`internal IsPopupPinnedOpen` 只供测试和内部诊断使用：内容、锚点、attach、effective visible/enabled、TopLevel 和 placement 全部有效后保持 Popup 打开，普通关闭和关闭动效被拦截；PlacementTarget、logical owner、content、TopLevel 或窗口生命周期失效时仍必须立即清理。解除 pin 不关闭已打开 Popup，但必须取消尚未打开的 pending 请求。控件语义 owner、Flyout、ToolTip 和 ContextMenu 的接入规则见 [Popup 钉住打开设计](popup-pinned-open-design.md)。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `` |
| 状态 | Stable |

## 何时使用

Popup 的设计语言是“可定位的透明承载原语”。宿主保持透明并提供窗口/layer 能力，内容默认拥有自己的表面；调用方
显式提供 frame surface 时，该表面只覆盖实际内容 bounds。阴影可以延伸到透明区域，但不能把整个 host 变成不透明矩形。

该模型把 host、frame 和 content 分成稳定职责：host 决定 native/overlay、定位和输入；frame 决定可选表面与阴影；
content 决定 Padding、边框、箭头和业务视觉。

## 公共 API

| API | 类型 | 语义 |
| --- | --- | --- |
| `SurfaceBackground` | `IBrush?` | Popup frame 在 Child bounds 内绘制的可选表面；默认 `null`，表示内容拥有表面。只有显式非空 Brush 才启用 host-owned surface。 |
| `PopupRootShadow` | `BoxShadows` | native `PopupRoot` 路径的 frame shadow。 |
| `OverlayHostShadow` | `BoxShadows` | `OverlayPopupHost` 路径的 frame shadow。 |
| `RequestedPlacement` | `PlacementMode?` | AtomUI 自定义定位请求；有效值会投射到 Avalonia custom placement。 |
| `MarginToAnchor` | `double` | 内容 frame 与 anchor 之间的逻辑像素间距。 |
| `IsPointAtCenter` | `bool` | 箭头型内容是否按目标中心修正定位。 |
| `CustomPlacementCallback` | `CustomPlacementCallback?` | AtomUI 完成 placement 后的扩展回调。 |
| `IsHorizontalFlipped` / `IsVerticalFlipped` | `bool` | 当前定位相对请求方向的只读翻转结果。 |
| `PositionFlipped` | `EventHandler<PopupFlippedEventArgs>` | 翻转结果变化通知。 |
| `OpenMotion` / `CloseMotion` | `AbstractMotion?` | host 内容打开和关闭动效。 |
| `MotionDuration` / `IsMotionEnabled` | `TimeSpan` / `bool` | 当前 Popup 的动效时长和开关。 |

Avalonia `Popup` 继承属性继续负责 `Child`、`PlacementTarget`、`IsOpen`、`IsLightDismissEnabled`、offset、anchor、
gravity、overlay 选择和焦点行为。AtomUI 不复制这些契约。

## 事件与命令

| `PositionFlipped` | `EventHandler<PopupFlippedEventArgs>` | 翻转结果变化通知。 |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

未找到对应 Gallery 目录；生成器只链接源文档，不发明示例。

## 状态模型

Popup 使用显式 surface ownership，不根据 Child 类型、Child Background、透明度、Dialog ancestry 或 Theme 应用时序推断。

| 模式 | `SurfaceBackground` | 表面所有者 | 适用场景 |
| --- | --- | --- | --- |
| content-owned | `null`，默认 | Child / Presenter / `PopupFrame` | Direct Popup、Flyout、ToolTip、ContextMenu、菜单、选择器、Picker 等。 |
| host-owned | 显式非 `null` | Popup frame | 调用方明确要求 Popup frame 提供表面，Child 只声明内容和 Padding。 |

`SurfaceBackground` 只改变 frame fill，不增加 wrapper、Padding、border、arrow、DesiredSize、placement offset 或 shadow
thickness。Direct Popup 的 host frame 默认透明；可见弹层内容必须由 Child 自己绘制背景，只有确实需要 frame 拥有表面时，
调用方才显式提供 Brush。

AtomUI 自有的 content-owned 消费者继承 Popup 原语的 `null` 默认值，不在 AXAML 入口或共享 C# 构造路径重复赋值。
新增 Popup-bearing 控件只有在明确选择 host-owned 模式时才设置非空 Brush，并更新库存测试和控件家族回归。

关闭分为普通交互关闭和生命周期 teardown。普通关闭在实际 PlacementTarget 与打开时的 owning `TopLevel` 仍属于同一
有效会话时允许播放 `CloseMotion`；若 Popup 打开时存在 logical owner，该 owner 也必须继续有效。PlacementTarget detach、
已建立的 Popup logical owner detach、PlacementTarget 切换到其他 `TopLevel` 或目标无法转换到原 TopLevel 时，关闭不得被
动效延迟，必须立即释放 Avalonia PopupHost 和定位订阅。自身没有 logical owner、但具有有效显式 PlacementTarget 的 Direct
Popup 仍可使用普通关闭动效。

Pinned 状态不改变上述会话分类：外点、Escape、失焦、window deactivation、`Close`/`Hide` 和业务 open state 写 `false`
属于普通关闭并被拒绝；content removal、target/owner detach、effective visible/enabled 失效、跨 TopLevel、模板重建和窗口销毁
属于生命周期 teardown。无效的 pinned `IsOpen=true` 只保留请求，不发布 `Opened`；unpin 时已打开 Popup 保持打开，pending
请求则被清理且不能在锚点恢复后复活。

## 主题与 Design Token

```text
Popup
  -> PopupRoot or OverlayPopupHost       transparent host
     -> PopupMotionActor                 open/close motion
        -> VisualLayerManager
           -> ShadowsAwareContainer      frame surface + shadow
              -> Child                   direct content or specialized presenter
```

native 与 overlay 两条路径共享相同的 frame surface 实现：

| 路径 | 宿主 | frame shadow | layer 语义 |
| --- | --- | --- | --- |
| native | `PopupRoot` / OS popup window | `PopupRootShadow` | 独立窗口，不参与 owning Window 内部 Z-order。 |
| overlay | `OverlayPopupHost` / `PopupOverlayLayer` | `OverlayHostShadow` | owning `TopLevel` 的 popup layer，高于 Dialog `OverlayLayer`。 |

`PopupRoot.Background` 保持 `null`，`TransparencyLevelHint` 保持 `Transparent`。不透明表面只覆盖 Child bounds，不能填满
native popup 的透明 shadow buffer。overlay host 使用同一原则，避免 host 背景改变圆角、箭头或 shadow 几何。

Token 来源：

`PopupToken` 是 internal control token，服务 Popup frame 和多个 Popup-bearing 控件：

| Token | 派生来源 | 语义与消费者 |
| --- | --- | --- |
| `PopupRootShadow` | 两层固定 alpha shadow | native `PopupRoot` frame shadow；菜单、选择器和自动建议等家族复用。 |
| `OverlayHostShadow` | `BoxShadowsSecondary` | `OverlayPopupHost` frame shadow；Flyout/Menu/ContextMenu 等 overlay 路径复用。 |
| `PopupCornerRadius` | `BorderRadiusLG` | 专用 Presenter / `PopupFrame` 的默认圆角。 |
| `MarginToAnchor` | `UniformlyMarginXXS` | Popup 内容 frame 与 anchor 的默认间距。 |

`PopupCornerRadius` 不由 transparent host 绘制；它由 Child、Presenter 或 `PopupFrame` 提供给
`ShadowsAwareContainer`，使 frame shadow 与显式可选 surface 和内容圆角一致。

## AOT 与裁剪注意事项

可选 surface 不增加 host 或 wrapper；只扩展既有 renderer。renderer 因 shadow 或显式 surface 按需创建并复用，surface
更新只 invalidates render。默认路径没有 surface Theme resource，也不创建全局非 Visual resource host。

Pinned target/ancestor 订阅只在请求有效期内存在，并由 target replacement、unpin、logical detach 或 lifecycle teardown
释放。Pinned open 不使用 retry timer、程序集扫描、runtime type discovery、反射查找控件或字符串 binding；恢复只依赖
generation-checked Dispatcher callback 和 AvaloniaProperty relay。

`PopupReflectionExtensions` 集中反射 Avalonia Popup 的私有 closing event、parent setter、open-state flag 与定位刷新入口，
每个反射成员都通过 `DynamicDependency` 声明 NativeAOT 保留要求；Popup 不做程序集扫描或 runtime type discovery。
StyledProperty 和 ControlTheme 均为静态/AOT 可发现契约。源码库存测试使用正则扫描，但只存在于测试项目，不进入 runtime
或 NativeAOT 路径。

## 源码索引

- `src/AtomUI.Desktop.Controls/Popup/Popup.cs`：公共 API、自定义定位、翻转通知、frame shadow 选择、动效和 wheel guard。
- `src/AtomUI.Desktop.Controls/Popup/PopupReflectionExtensions.cs`：对 Avalonia Popup 私有 closing、parent 与定位入口的集中反射桥接。
- `src/AtomUI.Desktop.Controls/Popup/PopupUtils.cs`：placement 算法、popup scope 和 owning popup 查询。
- `src/AtomUI.Desktop.Controls/Popup/PopupToken.cs`：Popup 家族的阴影、圆角和 anchor margin Token。
- `src/AtomUI.Core/MotionScene/MotionExecutionState.cs`：MotionScene 共享的 internal 动效执行生命周期定义。
- `src/AtomUI.Desktop.Controls/Popup/Themes/PopupTheme.axaml`：Popup shadow 和 motion Theme 值；不覆盖 surface 默认值。
- `src/AtomUI.Desktop.Controls/Popup/Themes/PopupRootTheme.axaml`：native transparent host 组合。
- `src/AtomUI.Desktop.Controls/Popup/Themes/OverlayPopupHostTheme.axaml`：overlay host 组合。
- `src/AtomUI.Desktop.Controls/Primitives/ShadowsAwareContainer.cs`：两类 host 共享的 frame surface/shadow renderer 与几何适配。

## 相关文档

- 源设计文档：`docs/controls/desktop/other/popup/overview.md`
- 实现文档：`docs/controls/desktop/other/popup/implementation.md`
- Token 文档：`docs/controls/desktop/other/popup/token.md`
- 变更记录：`docs/controls/desktop/other/popup/changelog.md`
- 语义结构：`./semantic-cn.md`
