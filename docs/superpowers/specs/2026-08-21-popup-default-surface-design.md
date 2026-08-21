# AtomUI Popup 默认表面根源修复设计

## 1. 背景与问题

`AtomUI.Desktop.Controls.Popup` 当前负责 popup host 选择、定位、light-dismiss、动画、阴影和翻转状态，但不绘制内容表面。
当调用方直接把一个没有 `Background` 的控件作为 `Popup.Child` 时，`PopupRoot` 或 `OverlayPopupHost` 仍能正确建立，
内容区域却会透出下层视觉。永久人工回归应用中的 Direct Popup 使用了只有 `Padding` 的透明 `Border`，因此出现弹层文字与
Dialog 内下一项文字叠穿。

这个现象与 Issue #441 已修复的 owning TopLevel / Overlay / Popup 层级问题无关。Popup host 已位于正确的
`PopupOverlayLayer`；缺失的是直接 Popup 的默认表面契约。

现有 Flyout、MenuFlyout、ToolTip、ContextMenu、选择器、Picker、菜单和 Tour 没有依赖 Popup host 绘制表面。它们分别由
`ArrowDecoratedBox`、专用 Presenter 或模板内 `PopupFrame` 绘制背景、圆角、Padding、箭头和边框。任何共享修复都必须
保持这些控件的现有视觉不变。

## 2. 目标与非目标

### 2.1 目标

- 直接使用 `AtomUI.Desktop.Controls.Popup` 时，即使 `Child` 没有背景，也默认获得主题化不透明表面。
- 同一实现同时覆盖 native `PopupRoot` 和 `OverlayPopupHost`，不得在 Dialog 或 TestApp 中增加特殊判断。
- 现有 Popup-bearing 控件继续由自己的 Presenter / `PopupFrame` 绘制表面，背景、圆角、Padding、尺寸、箭头、阴影和定位
  均保持不变。
- 透明 Popup 仍有明确、稳定且可文档化的 opt-out。
- 新增 Popup 入口必须显式声明由 host 还是 content 拥有表面，并由库存测试守卫。
- 保持 AOT 友好，不增加反射、运行时类型扫描或动态注册。

### 2.2 非目标

- 不重写 Avalonia Popup host 的创建、定位或 light-dismiss 算法。
- 不改变 Dialog、Drawer、`OverlayLayer`、`PopupOverlayLayer` 或 drawn chrome suppression 架构。
- 不统一重做各控件现有 Presenter / `PopupFrame`。
- 不新增 Popup 默认 Padding、边框或箭头；这些仍由 Child 或专用 Presenter 负责。
- 不把 `PopupRoot.Background` 改为不透明，也不填充 Popup 窗口的 shadow buffer。

## 3. 设计原则

### 3.1 Host 与 Surface 分离

Popup window/overlay host 继续保持透明，只负责承载和窗口级能力。默认表面只绘制在实际 `Child` 的布局边界内：

```text
Popup
  -> PopupRoot or OverlayPopupHost      transparent host
     -> PopupMotionActor               motion
        -> VisualLayerManager
           -> ShadowsAwareContainer    frame shadow + optional default surface
              -> Child                 business content or specialized presenter
```

`PopupRoot.Background` 必须继续为 `null`，以保留原生透明窗口合成、阴影透明区和非矩形内容能力。

### 3.2 显式表面所有权

直接 Popup 默认由 host frame 绘制表面。拥有专用 Presenter / `PopupFrame` 的控件显式把 host 表面设置为 `null`，由内容
继续承担全部表面视觉。禁止通过运行时检查 `Child.Background`、控件类型或透明度来猜测所有权，因为这种猜测无法区分
“尚未应用 Theme”和“有意透明”，也会造成首次显示闪烁和不可预测的主题行为。

### 3.3 其他控件零视觉变化

对 content-owned Popup，frame renderer 的填充 Brush 必须与修复前完全相同：`Brushes.Transparent`。现有阴影绘制调用、
Child bounds、CornerRadius、measure、arrange 和 z-order 不变。所有 Runtime 内部 Popup 入口显式 opt-out，并由测试阻止
漏配。

## 4. Public API

在 `AtomUI.Desktop.Controls.Popup` 新增一个 StyledProperty：

```csharp
public static readonly StyledProperty<IBrush?> SurfaceBackgroundProperty;

public IBrush? SurfaceBackground
{
    get => GetValue(SurfaceBackgroundProperty);
    set => SetValue(SurfaceBackgroundProperty, value);
}
```

契约：

- Popup 默认 Theme 将 `SurfaceBackground` 映射到 Shared Token `ColorBgElevated`。
- 非 `null`：host frame 在 `Child` bounds 内绘制该 Brush。
- `null`：host frame 不绘制可见填充，Child/Presenter 完全拥有表面。
- 用户需要透明直接 Popup 时显式设置 `SurfaceBackground="{x:Null}"`。
- `SurfaceBackground` 不增加 Padding，不改变 Child 的 DesiredSize，也不替代 Child 自己的 Background。
- Child 已经绘制不透明背景时，调用方应使用 `null`，避免形成双重表面所有权。

这是一个新增 Public API，并将直接 AtomUI Popup 的默认行为从“无 host surface”改为“使用主题 elevated surface”。这是本次
根源修复唯一有意改变的视觉契约。其他公共控件必须显式选择 content-owned 模式并保持现状。

## 5. 共享实现

### 5.1 Popup Theme

`PopupTheme.axaml` 增加：

```xml
<Setter Property="SurfaceBackground" Value="{atom:SharedTokenResource ColorBgElevated}" />
```

资源绑定位于 Visual `Popup` 的正常 Theme 生命周期中，不在 C# 构造函数创建全局 token subscription。

### 5.2 ShadowsAwareContainer

复用现有 `ShadowsAwareContainer`，不新增第二个 host wrapper：

- 增加 internal `SurfaceBackground` property。
- 在 attach 时与 ancestor `Popup.SurfaceBackground` 建立 relay binding。
- 将现有 `_shadowsRenderDisposable` 收敛为明确的 frame binding owner，统一持有 shadow 与 surface binding。
- reattach 前释放旧 binding；detach 时对称释放。
- `SurfaceBackground` 变化只触发 renderer 创建/重绘，不改变 measure 和 arrange。

### 5.3 Frame Renderer

现有 `BoxShadowRenderer` 扩展为同时绘制 frame surface 与 shadow 的内部 renderer；名称应反映稳定职责，例如
`PopupFrameRenderer`。Renderer 继续位于 Child 之前，使用现有 Child bounds 和已解析 CornerRadius：

```csharp
var fill = SurfaceBackground ?? Brushes.Transparent;
context.DrawRectangle(fill, null, roundedRect, BoxShadow);
```

行为边界：

- `SurfaceBackground == null` 时仍传入修复前相同的 `Brushes.Transparent`，保证专用控件阴影视觉不变。
- `BoxShadow.Count == 0` 且 surface 为 `null` 时不绘制。
- `BoxShadow.Count == 0` 但 surface 非 `null` 时仍绘制 surface。
- renderer bounds、CornerRadius 和 shadow geometry 继续来自现有 `ShadowsAwareContainer` 算法。
- 不增加新的 Visual wrapper；只扩展现有 renderer 的稳定职责。

## 6. 现有消费者隔离

所有 AtomUI Runtime 自有 Popup 入口必须显式使用 content-owned 表面：

### 6.1 AXAML Popup 入口

现有 17 个 AXAML `<atom:Popup>` 入口增加：

```xml
SurfaceBackground="{x:Null}"
```

范围包括 ColorPicker、GradientColorPicker、AutoComplete 三种形态、Cascader、ComboBox、RangeDatePicker、Mentions、
MenuItem、TopLevelMenuItem、NavMenu、InfoPickerInput、RangeInfoPickerInput、Select、Tour 和 TreeSelect。

### 6.2 C# Popup 入口

以下共享构造路径创建 Popup 时设置 `SurfaceBackground = null`：

- `Flyout.CreatePopup()`；覆盖 Flyout、MenuFlyout、TreeViewFlyout、PopupConfirm 及其消费控件。
- `ToolTip` 的 Popup 创建路径。
- `ContextMenu` 的 Popup 创建路径。

这些设置只选择既有内容表面所有权，不修改对应 Presenter、Token 或模板。

### 6.3 Direct Popup TestApp

永久 Demo 保留透明 `Border` Child，不设置 `Background`，也不设置 `SurfaceBackground`。它必须依赖 Popup 的默认 Theme
获得 elevated surface。禁止在 TestApp 绑定 `ColorBgElevated`、包裹特例 Presenter 或复制生产样式。

## 7. 库存与防回归

扩展 `PopupEntryInventoryTests`，在现有“共享不变量 + 原语 + 家族矩阵”基础上增加表面所有权守卫：

- 每个 Runtime AXAML Popup 入口必须显式包含 `SurfaceBackground="{x:Null}"`。
- Flyout、ToolTip、ContextMenu 三个 C# 创建源必须显式设置 `SurfaceBackground = null`。
- TestApp Direct Popup 必须保持没有 Child Background 和没有 SurfaceBackground 局部值。
- `PopupTheme` 必须把默认 SurfaceBackground 映射到 `ColorBgElevated`。
- `PopupRootTheme` 必须继续保持 `Background={x:Null}`。

库存变化时，维护者必须先决定表面所有者，不能只更新 allowlist。

## 8. TDD 与验证矩阵

实现严格执行 Red-Green-Refactor。

### 8.1 RED

先新增真实行为测试并确认失败：

1. 直接 Popup 在 Overlay host 中打开时，frame surface 使用主题 elevated background。
2. Popup Child 的 DesiredSize、Bounds 和 placement 不因默认 surface 增加 Padding 或 wrapper 尺寸。
3. `SurfaceBackground=null` 时 renderer 使用透明 fill，并保留原 shadow geometry。
4. TestApp 的透明 Border 不自行设置背景，证明修复来自 Popup 原语。
5. Runtime Popup 表面所有权库存守卫在尚未完成 opt-out 时失败。

### 8.2 GREEN

实现最小共享原语改动，并逐项使测试通过。不得为单个 Demo、Dialog 或控件增加条件分支。

### 8.3 相邻回归

- Popup targeted tests：定位、翻转、阴影、native/overlay mode、关闭动画和 wheel guard。
- Dialog Popup primitive tests：Direct Popup、Flyout、MenuFlyout、ToolTip、ContextMenu。
- Dialog Popup control family matrix。
- Tooltip、Flyout、Menu、ComboBox、Select、Picker、ColorPicker、Tour 代表性 Theme/behavior tests。
- Popup inventory tests。
- TestApp build。
- `git diff --check`。

验证其他控件零视觉变化时，不以“测试没崩溃”替代视觉契约。代表性测试必须同时断言：

- Runtime-owned Popup 的 `SurfaceBackground` 为 `null`。
- 既有 Presenter / `PopupFrame` 仍解析原背景 Token。
- host frame 的 surface fill 为透明。
- popup host bounds、Child bounds、CornerRadius 和 shadow thickness 未增加新的尺寸或偏移。

### 8.4 人工验证

在永久 TestApp 中验证：

- Direct Popup 内容不再透出下方文本。
- Flyout、MenuFlyout、ToolTip、ContextMenu、选择器、Picker、菜单和特殊 Popup 视觉保持原状。
- 打开、命中、light-dismiss、关闭和 host cleanup 正常。

平台证据分别记录：Windows、macOS、Linux X11、Linux Wayland。当前基线仍是 Windows 与 macOS 已测试，Linux X11 /
Wayland 未测试；共享 Headless 测试不能把 Linux 状态改为已测试。

## 9. 生命周期、性能与 AOT

- `Popup.SurfaceBackground` 是 Visual Control 上的正常 Theme resource，不触发非 Visual `AvaloniaObject` 全局资源宿主问题。
- `ShadowsAwareContainer` 新增的 relay binding 与现有 shadow binding 由同一个 logical attach/detach owner 对称释放。
- 不在构造函数创建 `TokenResourceBinder.CreateGlobalTokenBinding`。
- 不使用反射、运行时扫描、动态类型发现或平台特例；NativeAOT 风险不增加。
- 不增加 Popup host 或额外 wrapper。已有 renderer 只在 shadow 或默认 surface 需要绘制时存在。
- surface 变化只失效 render；不应触发 measure/arrange。

## 10. 文档与兼容性

实现完成时同步：

- 新增 `docs/controls/desktop/other/popup/overview.md`、`implementation.md`、`token.md` 和 `changelog.md`，记录
  `SurfaceBackground` 公共契约、host/surface 边界和透明 opt-out。
- 更新 `docs/architecture/systems/rendering/visual-layers.md`，明确 Popup host 透明与 frame surface 的不同职责。
- 更新 `docs/controls/desktop/feedback/modal/popup-layering-design.md` 的库存和测试要求。
- 更新 TestApp README 的 Direct Popup 验收步骤和 Linux X11/Wayland 启动方式。
- 按 AtomUI 控件文档规范更新 LLMS source coverage；不手改生成输出。

兼容性说明：

- 新增 `Popup.SurfaceBackground`，无成员删除或重命名。
- 直接 AtomUI Popup 的默认视觉有意从透明改为 elevated surface。
- 需要旧透明行为的直接 Popup 设置 `SurfaceBackground=null`。
- AtomUI 内置 Popup-bearing 控件显式使用 `null`，因此视觉和行为保持不变。

## 11. 被否决方案

### 11.1 在 Demo Border 上绑定背景

只修复当前截图，其他直接 Popup 仍会透底，属于触发点补丁。

### 11.2 给 PopupRoot / OverlayPopupHost 设置 Background

会填满 popup window/overlay host，包括 shadow buffer，并破坏原生透明合成、箭头、圆角和透明 Popup。

### 11.3 根据 Child 类型或 Background 自动猜测

Theme 应用时序、透明 Brush 和自绘控件无法可靠区分，容易产生首次显示闪烁和新控件漏判。

### 11.4 新增 PopupSurface wrapper 并只修改 Demo

仍要求调用方主动使用 wrapper，不能修复直接 Popup 的默认契约；同时与现有 Presenter / PopupFrame 重复。

## 12. 完成条件

只有同时满足以下条件才能声明完成：

- Direct Popup 默认 surface 的失败测试先红后绿。
- TestApp 不包含背景补丁或专用分支。
- native 与 overlay host 共用同一 frame surface 实现。
- 所有 Runtime-owned Popup 入口显式选择 content-owned surface。
- 代表性家族的背景、几何和阴影契约证明未改变。
- Popup、Dialog popup、家族矩阵、库存测试和 TestApp build 通过。
- 控件与架构文档完成同步，Windows/macOS/Linux 实机状态没有被错误推断。
- `git diff --check` 通过。
