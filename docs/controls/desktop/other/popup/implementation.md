# Popup 桌面版实现原理

本文档描述 Popup 的宿主选择、frame surface/shadow、定位、动效和生命周期实现。公共契约见
[Popup 桌面版架构设计](overview.md)，Token 语义见 [Popup Token 设计](token.md)，变化记录见
[Popup Changelog](changelog.md)。

## 1. 实现定位

Popup 实现把 Avalonia 的 native/overlay host 能力与 AtomUI 的定位、动效、阴影和可选 surface 合并为一个共享原语。
实现边界是：host 默认透明，frame 只在调用方显式提供 Brush 时于 Child bounds 内绘制表面，content-owned 控件继续由自身
Presenter 绘制视觉。

## 2. 源码文件结构

- `src/AtomUI.Desktop.Controls/Popup/Popup.cs`：公共 API、自定义定位、翻转通知、frame shadow 选择、动效和 wheel guard。
- `src/AtomUI.Desktop.Controls/Popup/PopupUtils.cs`：placement 算法、popup scope 和 owning popup 查询。
- `src/AtomUI.Desktop.Controls/Popup/PopupToken.cs`：Popup 家族的阴影、圆角和 anchor margin Token。
- `src/AtomUI.Desktop.Controls/Popup/Themes/PopupTheme.axaml`：Popup shadow 和 motion Theme 值；不覆盖 surface 默认值。
- `src/AtomUI.Desktop.Controls/Popup/Themes/PopupRootTheme.axaml`：native transparent host 组合。
- `src/AtomUI.Desktop.Controls/Popup/Themes/OverlayPopupHostTheme.axaml`：overlay host 组合。
- `src/AtomUI.Desktop.Controls/Primitives/ShadowsAwareContainer.cs`：两类 host 共享的 frame surface/shadow renderer 与几何适配。

## 3. 核心类职责

`Popup` 拥有公开 surface/shadow/placement/motion 状态。Avalonia 创建的 `PopupRoot` 或 `OverlayPopupHost` 拥有实际 host
生命周期；host Theme 中的 `ShadowsAwareContainer` 拥有 frame renderer 和从 ancestor Popup 取得的 relay bindings。

`ShadowsAwareContainer` 不创建第二套 popup host，也不拥有 Child 内容表面。它只负责读取 Popup 当前 frame 状态、解析
Child 圆角/箭头几何、在 Child 之前绘制 surface/shadow，以及为 native shadow 预留有效 thickness。内部
`PopupFrameRenderer` 是纯 frame 绘制节点，不处理 placement、输入或 open state。

## 4. 状态与数据流

```text
Popup property metadata
  SurfaceBackground <- null
PopupTheme
  PopupRootShadow   <- PopupToken.PopupRootShadow
  OverlayHostShadow <- PopupToken.OverlayHostShadow
Caller local/style value (optional)
  SurfaceBackground <- explicit Brush
        ↓
Popup.ConfigureFrameShadow(host mode)
        ↓
Popup.FrameShadow + Popup.SurfaceBackground
        ↓ logical attach relay bindings
ShadowsAwareContainer
        ↓ one-way renderer bindings
PopupFrameRenderer.Render
```

surface ownership 由 Popup 的最终属性值决定：没有 local/style value 时使用属性元数据的 `null` 默认值；调用方显式提供
非空 Brush 时切换为 host-owned。Theme 和 AtomUI 内置消费控件都不重复设置 `null`。renderer 不检查 Child 类型、
Background 或 opacity。

## 5. 内部算法与关键流程

### 5.1 frame renderer

`PopupFrameRenderer` 在 shadow 非空或 surface 非 `null` 时惰性创建，位于 Child 之前，不增加 layout wrapper。
renderer 使用现有 Child bounds；arrow 可见时从 frame body bounds 扣除 indicator 占用。圆角从
`IArrowAwareShadowMaskInfoProvider`、`Border` 或 `TemplatedControl` 解析。

fill 固定为 `SurfaceBackground ?? Brushes.Transparent`。shadow 为空且 surface 为 `null` 时不绘制；surface 非 `null`
时即使没有 shadow也绘制。`SurfaceBackground` 只触发 render，不加入 `AffectsMeasure`。

### 5.2 layout 与 placement

native host 的 measure 根据 box-shadow thickness 扩展尺寸，arrange 将 Child 偏移到 shadow buffer 内；arrow 可见时按方向
扣除 indicator 已占用的 thickness。overlay host 使用 `IsOverlayMode=true`，不把 native shadow buffer 加入 layout。

Popup 将普通 Avalonia placement 转换为 custom placement，并统一计算 anchor、gravity、offset、shadow thickness、arrow
center 修正和 flip。center 使用 owning `TopLevel.ClientSize`；pointer 使用 owning TopLevel 的 client 坐标。surface 不进入
任何 placement 输入，因此不能改变位置或尺寸。

## 6. 生命周期与模板接入

logical attach 时，`ShadowsAwareContainer` 创建一个 `CompositeDisposable`，统一持有 shadow 与 surface 两条 relay binding；
重新 attach 前先释放旧 binding，logical detach 时对称释放。固定模板关系不能直接跨 Avalonia popup host 的 logical owner
使用 `TemplateBinding` 表达，因此 binding 由实际 runtime container 持有。

ContentPresenter Child observable 在 Child/Presenter 替换时重新配置圆角和 arrow geometry，detach 时释放。frame renderer
设置 logical parent 并随 container 生命周期存在。

Popup 的 open/close motion cancellation token 由 Popup 实例持有；关闭、快速切换和最终 close 对称取消和释放。打开时安装
Child wheel guard，关闭时释放。placement transform tracking 只在打开且 placement 需要 anchor 时存在；关闭或 target
不可见时释放或关闭 Popup。

## 7. 默认消费路径

AtomUI 自有弹层通过 Popup 原语统一继承 `SurfaceBackground=null`：

- 17 个 runtime AXAML Popup 文件覆盖选择器、自动建议、Picker、菜单、NavMenu、Tour 与 ColorPicker 家族。
- `Flyout.CreatePopup()`、ToolTip 和 ContextMenu 是三个共享 C# 构造路径。

这些入口不写入冗余 local value。Flyout 派生与委托消费控件通过共享 `Flyout.CreatePopup()` 继承同一默认值。新增入口由
`PopupEntryInventoryTests` 的闭集扫描捕获；只有明确需要 host-owned surface 的入口才能显式提供非空 Brush。

## 8. 输入与宿主边界

Popup placement target、Popup logical owner 和实际 host 必须解析到同一 owning `TopLevel`。Dialog 使用低于 popup 的
`OverlayLayer`，content popup 使用同一 Window 的 `PopupOverlayLayer`；native popup 则使用独立 `PopupRoot`。两条 host
路径共享 frame renderer 和 surface ownership。

Child 内部控件已处理的 wheel 不被重复消费；未处理的 wheel 在 popup 边界终止，避免滚动 placement target 外层祖先。
light-dismiss、focus 和 host teardown 继续由 Avalonia Popup 协议负责。

## 9. 资源、性能与 AOT 边界

可选 surface 不增加 host 或 wrapper；只扩展既有 renderer。renderer 因 shadow 或显式 surface 按需创建并复用，surface
更新只 invalidates render。默认路径没有 surface Theme resource，也不创建全局非 Visual resource host。

实现不使用反射、runtime type discovery 或动态注册；StyledProperty 和 ControlTheme 均为静态/AOT 可发现契约。源码库存
测试使用正则扫描，但只存在于测试项目，不进入 runtime 或 NativeAOT 路径。

## 10. 维护不变量

- `PopupRoot.Background` 必须保持 `null`，native window 继续透明合成。
- native 与 overlay host 必须共享 `ShadowsAwareContainer`，不得复制 surface 实现。
- surface 不得参与 measure、arrange、placement、Padding、border 或 arrow 计算。
- `SurfaceBackground` 的属性默认值必须为 `null`，Popup Theme 不得覆盖该默认值。
- content-owned Popup 不重复设置 `null`；host-owned Popup 必须显式提供非空 Brush。
- relay binding 的 attach/re-attach/detach 必须有单一 owner 和对称释放。
- 永久 TestApp 的 Direct Popup 不设置 `SurfaceBackground`；其 Child 使用主题化背景履行 content-owned 契约，并验证不会与
  下层文字发生视觉混叠。

## 11. 测试与验证

- `PopupShadowTests` 验证公开 surface API、`null` 默认值、显式 renderer fill、透明 frame 和 shadow clipping。
- `PopupPlacementTests` 与 `ToolTipPopupModeTests` 验证 placement、host mode 和 transparent PopupRoot。
- `DialogPopupPrimitiveLayeringTests` 验证 Direct Popup 与四类 content-owned 原语。
- `PopupEntryInventoryTests` 守卫所有 runtime 入口、无冗余默认值覆盖和 TestApp 源码契约。
- `DialogPopupControlFamilyTests`、DataGrid popup tests 和代表性控件测试验证家族行为与视觉所有权未变。
- `AtomUI.Desktop.Controls.TestApp/Scenarios/PopupInDialog` 走查真实桌面渲染、pointer、focus 和关闭行为。

实机验证状态为 Windows、macOS 已测试；Linux X11/Wayland 未测试。
