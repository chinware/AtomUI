# Popup 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Popup` | 拥有 public placement、motion、shadow 和 surface 状态。 | `SurfaceBackground`、`RequestedPlacement`、`IsOpen` | `PopupToken` | stable |
| `host` | `PopupRoot` / `OverlayPopupHost` | 提供透明 native/overlay host、输入和 layer 能力。 | `ShouldUseOverlayLayer` | `PopupRootShadow`、`OverlayHostShadow` | stable |
| `surface` | `ShadowsAwareContainer` frame | 在 Child bounds 内绘制可选 surface 与 shadow。 | `SurfaceBackground` | 无默认颜色 Token；Brush 由调用方提供 | stable |
| `content` | `Popup.Child` | 承载调用方内容或专用 Presenter。 | `Child` | 由内容 owner 决定 | stable |

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Popup
  -> OverlayPopupHost (control theme, OverlayPopupHostTheme.axaml)
     -> PopupMotionActor (internal-observable)
        -> VisualLayerManager (template-stable)
           -> ShadowsAwareContainer (internal-observable)
              -> ContentPresenter (internal-observable)
  -> PopupRoot (control theme, PopupRootTheme.axaml)
     -> PopupMotionActor#{x:Static atom:BaseMotionActor.MotionActorPart} (internal-observable)
        -> Panel (template-stable)
           -> Border#PART_TransparencyFallback (template-stable)
           -> VisualLayerManager (template-stable)
              -> ShadowsAwareContainer (internal-observable)
                 -> ContentPresenter (internal-observable)
  -> Popup (control theme, PopupTheme.axaml)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Popup` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `OverlayPopupHost` | control theme | `OverlayPopupHostTheme.axaml` | Popup | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PopupMotionActor` | template node (PopupMotionActor) | `OverlayPopupHostTheme.axaml` | OverlayPopupHost | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ShadowsAwareContainer` | template node (ShadowsAwareContainer) | `OverlayPopupHostTheme.axaml` | OverlayPopupHost | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `OverlayPopupHostTheme.axaml` | OverlayPopupHost | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PopupRoot` | control theme | `PopupRootTheme.axaml` | Popup | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:BaseMotionActor.MotionActorPart}` | template node (PopupMotionActor) | `PopupRootTheme.axaml` | PopupRoot | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `PopupRootTheme.axaml` | PopupRoot | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TransparencyFallback` | template node (Border) | `PopupRootTheme.axaml` | PopupRoot | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ShadowsAwareContainer` | template node (ShadowsAwareContainer) | `PopupRootTheme.axaml` | PopupRoot | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `PopupRootTheme.axaml` | PopupRoot | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Popup` | control theme | `PopupTheme.axaml` | Popup | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

源文档未声明稳定 Template Part。维护模板时应以源码和主题文件中的实际声明为准。

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

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

## Theme and Token Boundaries

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

Token 边界：

`PopupToken` 是 internal control token，服务 Popup frame 和多个 Popup-bearing 控件：

| Token | 派生来源 | 语义与消费者 |
| --- | --- | --- |
| `PopupRootShadow` | 两层固定 alpha shadow | native `PopupRoot` frame shadow；菜单、选择器和自动建议等家族复用。 |
| `OverlayHostShadow` | `BoxShadowsSecondary` | `OverlayPopupHost` frame shadow；Flyout/Menu/ContextMenu 等 overlay 路径复用。 |
| `PopupCornerRadius` | `BorderRadiusLG` | 专用 Presenter / `PopupFrame` 的默认圆角。 |
| `MarginToAnchor` | `UniformlyMarginXXS` | Popup 内容 frame 与 anchor 的默认间距。 |

`PopupCornerRadius` 不由 transparent host 绘制；它由 Child、Presenter 或 `PopupFrame` 提供给
`ShadowsAwareContainer`，使 frame shadow 与显式可选 surface 和内容圆角一致。

## Customization Boundaries

- Popup placement target、host 与 Dialog/Drawer presentation 必须属于同一 owning `TopLevel`。
- Pinned 打开只能在 content、anchor、attach、effective visible/enabled 和 placement 同时有效时发生；显式 `ShowAt` 与自动恢复使用同一门禁。
- 业务 open state 和 Popup `IsOpen` 的 coercion 不得发布瞬态关闭；`false` 必须保持真实的 false 请求语义。
- native/overlay 切换只改变宿主和 shadow token，不改变 `SurfaceBackground` 语义。
- `SurfaceBackground=null` 时 frame renderer 继续使用透明 fill，专用 Presenter 的背景、圆角、Padding、阴影和定位保持不变。
- Popup Child 内未被内部滚动控件消费的 wheel 事件在 popup 边界终止，避免滚动外层 placement target 祖先。
- 普通 close motion、快速重开和 motion completion 必须保持单一关闭状态流；placement target detach、logical detach、
  跨 `TopLevel` 与 transform 失效属于不可延迟的 host teardown。
- `SurfaceBackground` 是可选公共 StyledProperty；默认 `null` 保持 Direct Popup 与专用 Popup 家族的透明 host frame 契约；
  需要遮挡下层内容的 Direct Popup Child 必须拥有自己的背景。

维护不变量：

- `PopupRoot.Background` 必须保持 `null`，native window 继续透明合成。
- native 与 overlay host 必须共享 `ShadowsAwareContainer`，不得复制 surface 实现。
- surface 不得参与 measure、arrange、placement、Padding、border 或 arrow 计算。
- `SurfaceBackground` 的属性默认值必须为 `null`，Popup Theme 不得覆盖该默认值。
- content-owned Popup 不重复设置 `null`；host-owned Popup 必须显式提供非空 Brush。
- relay binding 的 attach/re-attach/detach 必须有单一 owner 和对称释放。
- close motion 只能延迟仍连接到打开时 owning TopLevel 的普通关闭；打开时已存在的 logical owner、host 或 anchor 生命周期
  失效时不得保留 Avalonia open state。没有 logical owner 的 Direct Popup 以显式 PlacementTarget 会话为准。
- Pinned open 的有效性必须同时包含 content、target attach、effective visible/enabled、TopLevel 和 placement transform；无效 true 不得发布 `Opened`。
- Pinned 普通关闭包括外点、Escape、失焦、window deactivation、`Close`/`Hide` 和业务 open state false；lifecycle close 必须跳过 motion 并释放 host、binding、subscription、tracker、wheel guard 和 timer。
- Unpin 不关闭已打开 Popup；pending unpin 必须清除隐藏 open request，target 恢复后不得复活旧请求。
- 业务 open state coercion 不得通过 suppression flag 发布瞬态 false；lifecycle close scope 是唯一允许 pinned 业务状态变为 false 的路径。
- Popup 必须以共享 `MotionExecutionState` 表达关闭动效阶段；`Pending`、`Playing` 和 `Completing` 单向收敛，重复
  close 不得创建并行关闭动效，`Closed` 必须回到 `Idle`。
