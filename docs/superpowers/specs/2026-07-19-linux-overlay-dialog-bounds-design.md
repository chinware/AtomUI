# Linux Overlay Dialog 主体、Mask 与阴影边界修复设计

## 1. 背景与目标

Dialog/MessageBox 生命周期重构把 Overlay Dialog 的 mask 与 surface 合并到同一个
`OverlayDialogPresenter`。重构后的布局把整个 TopLevel visual layer 当成 Dialog 可用区，遗漏了 Linux
窗口标题栏和装饰几何。第一轮修复又把三个语义不同的矩形合并成一个 `ownerBounds`，并把 Dialog 自身的
BoxShadow 当成布局边距，因而只部分修复了问题，并引入了新的可见错误：

- Dialog 主体虽不再进入标题栏，但拖动和最大尺寸为自身 BoxShadow 预留了空白。
- `HostWidth` / `HostHeight` 的有效可用范围被 Dialog BoxShadow 缩小，与主体尺寸契约不一致。
- Mask 复用标题栏以下的 Dialog 可用区，没有覆盖标题栏和窗口边框。
- Mask 再次扣除窗口左右及底部装饰，与既有 `WindowVisualLayerClip` 形成二次内缩，底边和圆角处出现缝隙。

本次修复把以下三套几何彻底分开：

1. **Mask bounds**：覆盖完整 visual layer，由窗口的统一 frame clip 裁成可见窗口轮廓。
2. **Dialog body bounds**：仅表示标题栏以下、窗口阴影以内的 Dialog 主体可用区。
3. **Dialog BoxShadow extents**：纯绘制外扩，不参与主体测量、尺寸限制或位置约束。

本设计是
[`2026-07-17-dialog-messagebox-lifecycle-redesign-design.md`](2026-07-17-dialog-messagebox-lifecycle-redesign-design.md)
中 Overlay Presenter 布局职责的纠错补充，不改变其生命周期和状态所有权设计。

## 2. 范围与兼容性

包含：

- Linux Wayland CSD 窗口的标题栏、窗口阴影、圆角和客户区几何。
- Linux 非 CSD 窗口的 managed title bar 与 frame shadow 几何。
- Overlay Dialog 的 mask、初始定位、拖动、缩放、最大化和恢复。
- Dialog 自绘 BoxShadow 与 `HostWidth` / `HostHeight` 的尺寸语义。
- owner resize、窗口状态和装饰几何变化后的重新约束。
- 回归测试和目标 Desktop Controls 测试。

不包含：

- Native Window Dialog 的 compositor placement。
- Dialog 生命周期、关闭策略、焦点、按钮或 motion 架构调整。
- Public API、StyledProperty、主题资源 key、template part 名称或 Token 变更。
- Windows/macOS Overlay Dialog 的位置、mask 或阴影边界行为变化。
- Window、Drawer、Popup 或其他 overlay 控件的通用视觉层重构。

## 3. 根因

### 3.1 Dialog body bounds 退化为整个 visual layer

TopLevel popup visual layer 保持窗口 surface 的完整坐标系；Linux 正常窗口的该坐标系还包括窗口阴影和标题栏区域。
Dialog 主体如果直接使用整层 bounds，就可以进入标题栏或窗口阴影区域。

AtomUI Window 已经提供两组平台几何：

- CSD 的 `WindowDecorationMargin` 与内容 Panel 使用同一边界，表示窗口内容布局范围。
- 非 CSD 的 `FrameShadowThickness` 加 visible managed title bar 表示标题栏以下的主体范围。

这些值只应用于 Dialog 主体，不应用于 mask。

### 3.2 Dialog BoxShadow 被误当成主体布局边距

`DialogSurface` 的 `PART_ShadowHost` 使用 `ShadowsAwareContainer.IsOverlayMode=True`。该模式明确保持 child 的测量和
arrange 尺寸不变，只把 BoxShadow 绘制到主体 Bounds 外。当前 Presenter 却读取该 BoxShadow thickness：

- 用它再次缩小 `_surface.MaxWidth` / `_surface.MaxHeight`。
- 用它把拖动、缩放和启动位置的 clamp 向内推。

这与 `IsOverlayMode` 以及 `HostWidth` / `HostHeight` 表示 surface 主体尺寸的契约冲突。BoxShadow 是视觉装饰，
允许在窗口边缘由上层 frame clip 裁剪，不应制造主体与边界之间的空白。

### 3.3 Mask 错误复用 Dialog body bounds

Mask 和 Dialog 主体当前都使用 `ResolveOwnerBounds`。这会让 mask 从标题栏下方开始，并在 CSD 下同时扣除
`WindowDecorationMargin` 的左右和底部，导致 mask 没有完整覆盖窗口。

AtomUI Window 已有 `WindowVisualLayerClip`，它把整个 `VisualLayerManager` 按 `FrameShadowThickness` 和 Window
`CornerRadius` 统一裁剪，并刻意不改变 visual layer 坐标系。Mask 再构造一个内缩矩形或单独圆角，会重复窗口
frame clip，产生边框间隙和抗锯齿接缝。

## 4. 方案比较

### 方案 A：分离 mask、Dialog 主体和视觉阴影几何（采用）

- Mask 填满 presenter visual layer。
- Linux Dialog 主体单独解析标题栏以下的 body bounds。
- Dialog BoxShadow 不进入任何尺寸或位置计算。
- 窗口阴影和圆角只由 `WindowVisualLayerClip` 裁剪。

优点：每套几何只有一个职责和真源；符合既有 Window clip 与 `IsOverlayMode` 契约；不会重复圆角计算。

### 方案 B：手动计算 mask frame bounds 并绑定圆角（不采用）

单独使用 `FrameShadowThickness` 缩小 mask，再把 Window CornerRadius 复制给 mask。

问题：重复 `WindowVisualLayerClip` 的职责；布局舍入和抗锯齿稍有差异就会再次产生细缝；窗口状态变化还需要额外同步。

### 方案 C：继续共享一个 bounds，通过特殊分支补边（不采用）

保留 `ownerBounds`，为 mask 清除部分 margin，并为 Dialog shadow 增加条件开关。

问题：继续混合不同坐标语义，容易在 CSD、非 CSD、最大化和装饰变化时产生新的边界组合 bug。

## 5. 详细设计

### 5.1 Mask bounds

`PART_MaskMotionActor` 始终使用 presenter 的完整 layer bounds：

```text
maskBounds = Rect(0, 0, layerWidth, layerHeight)
```

在 AtomUI Window 中，mask 位于 `WindowVisualLayerClip` 下方的 VisualLayerManager 内，因此：

- Mask 覆盖标题栏、内容和窗口可见边框。
- 外部窗口阴影由 frame clip 排除，不会被 mask 着色。
- Window CornerRadius 由同一个 frame clip 一次性裁剪，mask 不再建立第二套圆角边界。
- 最大化和全屏时 Window 现有 CornerRadius / FrameShadowThickness 状态继续决定最终轮廓。

非 Linux 继续保持现有整层 mask 行为。

### 5.2 Linux Dialog body bounds

Presenter 只为 Dialog 主体解析 Linux 可用区。

Linux CSD：

```text
dialogBodyBounds = layerBounds deflated by WindowDecorationMargin
```

`WindowDecorationMargin` 与 CSD 内容 Panel 使用同一布局边界，已经包含标题栏和窗口装饰占用，不再叠加
`TitleBarHeight`。

Linux 非 CSD：

```text
visibleFrame     = layerBounds deflated by FrameShadowThickness
dialogBodyBounds = visibleFrame excluding the visible managed title bar height
```

FullScreen 下标题栏不可见，不扣 title bar；Maximized 下保留标题栏，但 frame shadow 通常为零。

所有 deflate 计算把负宽高截为零，避免极小窗口或异常装饰值传入 `Math.Clamp`。

### 5.3 Dialog body size 与 BoxShadow

`HostWidth` / `HostHeight`、`HostMin*` 和 `HostMax*` 只约束 `DialogSurface` 主体：

```text
surface.MaxWidth  <= dialogBodyBounds.Width
surface.MaxHeight <= dialogBodyBounds.Height
```

不再读取 `PART_ShadowHost.BoxShadow.Thickness()` 来缩小上述范围。显式 `HostWidth=320`、`HostHeight=180` 时，
surface 主体仍为 `320 × 180`；BoxShadow 在该矩形外绘制。

初始 placement、拖动和缩放都直接 clamp 主体矩形：

```text
dialogBodyBounds.Left   <= surface.Left
surface.Right           <= dialogBodyBounds.Right
dialogBodyBounds.Top    <= surface.Top
surface.Bottom          <= dialogBodyBounds.Bottom
```

当主体位于边缘时，外扩 BoxShadow 可以被 `WindowVisualLayerClip` 裁剪。Presenter 不为阴影预留空白。

### 5.4 最大化与恢复

最大化 Overlay Dialog 填充 `dialogBodyBounds`，因此主体仍不会覆盖标题栏或窗口外部阴影。
最大化继续使用无圆角 surface；恢复后按当前 body bounds 和 Dialog placement 重新布局。

Mask 独立填满 visual layer，不随 Dialog 最大化或恢复改变覆盖范围。

### 5.5 更新时机与生命周期

Presenter 在以下时机重新解析 Dialog body 几何：

- 首次加入 DialogLayer 并完成 template/layout。
- DialogLayer/TopLevel size 变化。
- Dialog HostWidth/Height/Min/Max 变化。
- Window CSD、WindowDecorationMargin、FrameShadowThickness、TitleBarHeight、标题栏可见性或 WindowState 变化。
- surface size 变化、最大化和恢复。

现有订阅继续由 Presenter 的 `_bindings` 持有并在 `DisposeAsync` 释放。不增加全局服务、timer、dispatcher delay、
suppression flag 或新的 public contract。

## 6. 测试设计

先添加失败回归，再修改生产代码。

`OverlayDialogPresenterTests` 增加或修正：

1. Linux CSD mask 的 origin 为 `(0, 0)`，尺寸等于完整 presenter layer，不扣标题栏或装饰边距。
2. Linux 非 CSD mask 同样覆盖完整 layer；窗口 frame clip 是唯一外轮廓裁剪者。
3. Linux CSD Dialog 拖到左上边界时，主体恰好贴合 `dialogBodyBounds`，不增加 Dialog shadow inset。
4. Linux CSD Dialog 拖到右下边界时，主体 Bounds 不越界，但不为 BoxShadow 预留空间。
5. 非零 Dialog BoxShadow 下，显式 `HostWidth` / `HostHeight` 仍精确表示主体尺寸。
6. 请求尺寸超过客户区时，最大主体尺寸等于 `dialogBodyBounds`，不再额外扣 Dialog shadow。
7. CSD decoration、owner resize 或窗口状态变化后，mask 仍覆盖完整 layer，Dialog 主体重新约束到当前 body bounds。
8. 非 Linux fixture 保留整层 mask 与原位置断言，防止范围外行为变化。

验证命令：

```bash
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj \
  --framework net10.0 --no-restore \
  --filter FullyQualifiedName~OverlayDialogPresenterTests

dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj \
  --framework net10.0 --no-restore \
  --filter "FullyQualifiedName~Dialog|FullyQualifiedName~WindowResizeArtifactTests|FullyQualifiedName~WindowingPlatformDetectionTests"

dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj \
  --framework net10.0 --no-restore

git diff --check
```

Linux Wayland Gallery 手动验证：

- 打开 Modal Overlay，确认 mask 覆盖标题栏、内容和四周可见边框，底边及圆角无缝隙。
- 确认 mask 不着色窗口外部 compositor shadow。
- 分别把 Dialog 拖到上、下、左、右边界，确认白色主体可以贴边且不进入标题栏。
- 确认 Dialog 自身阴影不改变主体尺寸；靠边时允许被窗口 frame clip 裁剪。
- resize、maximize、restore owner 后重复上述检查。

## 7. 契约与风险

- Public API/theme contract changed：No。
- Observable behavior changed：Yes，仅修正 Linux Overlay Dialog 的错误边界。
- Rendered result changed：Yes，mask 完整覆盖窗口可见轮廓，Dialog 主体不再为自身阴影留白。
- Files split：No。
- AOT impact：No reflection、dynamic discovery 或新动态 binding。
- Lifecycle risk：低；继续使用现有 Presenter disposable owner。
- Residual risk：当前开发环境不能直接运行 Linux Wayland compositor，最终仍需要 Linux Gallery 手动验证。
