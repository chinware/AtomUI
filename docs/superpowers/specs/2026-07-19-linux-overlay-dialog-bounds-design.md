# Linux Overlay Dialog 可见客户区与阴影边界修复设计

## 1. 背景与目标

Dialog/MessageBox 生命周期重构把 Overlay Dialog 的 mask 与 surface 合并到同一个
`OverlayDialogPresenter`，但新的布局实现把整个 TopLevel OverlayLayer 当作 owner 可用区。
在 Linux Wayland CSD 窗口中，OverlayLayer 还包含窗口阴影和标题栏使用的装饰区域，因此出现三个同源问题：

- Dialog 可以拖到标题栏下面，header 和正文被标题栏覆盖。
- 拖动约束只使用 `DialogSurface.Bounds`，没有计算绘制在 Bounds 外的 BoxShadow。
- Mask 与 Dialog 没有使用同一个可见客户区，标题栏下沿会暴露本应被遮挡的 Dialog 或页面内容。

本次修复的目标是让 Linux Overlay Dialog 的 mask、placement、拖动、缩放和最大化共享一个准确的可见客户区，
并让普通状态 Dialog 的完整阴影始终位于该区域内。

本设计是
[`2026-07-17-dialog-messagebox-lifecycle-redesign-design.md`](2026-07-17-dialog-messagebox-lifecycle-redesign-design.md)
中 Overlay Presenter 布局职责的纠错补充，不改变其生命周期和状态所有权设计。

## 2. 范围与兼容性

包含：

- Linux Wayland CSD 窗口的标题栏、窗口阴影和客户区几何。
- Linux 非 CSD 窗口的 managed title bar 与 frame shadow 几何。
- Overlay Dialog 的 mask、初始定位、拖动、缩放、最大化和恢复。
- Dialog 自绘 BoxShadow 的四边外扩。
- owner resize、窗口状态和装饰几何变化后的重新约束。
- 回归测试和目标 Desktop Controls 测试。

不包含：

- Native Window Dialog 的 compositor placement。
- Dialog 生命周期、关闭策略、焦点、按钮或 motion 架构调整。
- Public API、StyledProperty、主题资源 key、template part 名称或 Token 变更。
- Windows/macOS Overlay Dialog 的位置、mask 或阴影边界行为变化。
- Window、Drawer、Popup 或其他 overlay 控件的通用视觉层重构。

## 3. 根因

### 3.1 Owner bounds 退化为整个 OverlayLayer

`OverlayDialogPresenter.ResolveOwnerBounds` 当前直接返回：

```text
Rect(0, 0, layerWidth, layerHeight)
```

Linux CSD 下，`VisualLayerManager` 覆盖整个 TopLevel surface；实际页面内容则通过
`WindowDecorationMargin` 内缩到窗口阴影内沿和标题栏下方。Presenter 忽略该 margin 后，
Dialog 的 Y 最小值仍是 0，所以可进入标题栏区域。

重构前的 `OverlayDialogHost.CalculateOwnerBounds` 已经使用 `WindowDecorationMargin` 处理 CSD，
该平台几何在重构时没有迁移到新的 Presenter。

### 3.2 Surface bounds 不包含 overlay shadow

`DialogSurface` 内的 `ShadowsAwareContainer` 使用 `IsOverlayMode=True`。此模式保持 Dialog 主体测量尺寸不变，
并在主体 Bounds 外绘制 shadow。当前 `ConstrainSurfacePosition` 只约束主体矩形，因此主体贴边时阴影必然越界或被裁剪。

### 3.3 Mask 与 surface 分别依赖偶然层级

Mask 当前 stretch 到整个 Presenter。CSD 标题栏由更高的 drawn-decoration overlay 覆盖，视觉结果依赖两个层级的偶然重叠，
而 Dialog placement 又使用整层 bounds。Mask、Dialog 和可见页面客户区没有共同几何真源，标题栏下沿因此可能出现错误露底。

## 4. 方案比较

### 方案 A：Presenter 内统一可见客户区（采用）

`OverlayDialogPresenter` 从 owner TopLevel 解析一个 Linux 可见客户区，并把同一个 Rect 用于 mask、surface sizing、
placement、拖动、缩放、最大化和恢复。普通 Dialog 在该 Rect 内再扣除自身 shadow extents。

优点：恢复被重构遗漏的平台语义；改动集中；不改变 DialogLayer、HostWidth/HostHeight 或公共契约。

### 方案 B：缩小整个 DialogOverlayLayer（不采用）

让 `DialogOverlayLayer` 自身只覆盖内容区。

问题：会改变所有 Dialog 的 mask hit testing、嵌套层叠和激活范围，也会把平台窗口几何职责提升到多 Dialog 容器，
影响面大于本次修复。

### 方案 C：让 shadow 参与 DialogSurface 测量（不采用）

关闭 `ShadowsAwareContainer.IsOverlayMode` 或给 surface 增加 shadow padding。

问题：会改变 `HostWidth`/`HostHeight`、自然尺寸、resize handle、motion anchor 和 Window-hosted surface 的尺寸语义，
同时不能解决标题栏与 mask 几何错误。

## 5. 详细设计

### 5.1 单一 owner bounds

Presenter 解析 owner TopLevel；仅当 owner 是 AtomUI `Window` 且 `OsType=Linux` 时应用 Linux 装饰规则。

Linux CSD：

```text
ownerBounds = layerBounds deflated by WindowDecorationMargin
```

`WindowDecorationMargin` 是平台和 Avalonia 共同发布的 CSD 内容边界，包含标题栏及窗口装饰占用，
不能再次叠加 `TitleBarHeight`。

Linux 非 CSD：

```text
visibleFrame = layerBounds deflated by FrameShadowThickness
ownerBounds  = visibleFrame excluding the visible managed title bar height
```

FullScreen 下标题栏不可见，不扣除 title bar；Maximized 下保留标题栏，但 frame shadow 通常为零。

非 Linux owner 继续使用整个 layer bounds，保持既有行为。

所有 deflate 计算把负宽高截为零，避免极小窗口或异常装饰值传入 `Math.Clamp`。

### 5.2 Mask 几何

Modal mask 使用解析后的 `ownerBounds`：

- 左上角等于客户区左上角。
- 宽高等于客户区宽高。
- 不进入标题栏或窗口阴影区域。
- 下沿和页面客户区精确一致，不依赖 drawn title bar 覆盖 mask 来制造边界。

Mask 与 Dialog 继续由同一个 `OverlayDialogPresenter` 拥有，topmost、关闭和 motion 语义不变。

### 5.3 Shadow-aware surface bounds

`DialogSurface` 继续保持主体尺寸语义。Presenter 在 template 已应用后读取 `PART_ShadowHost.BoxShadow.Thickness()`，
得到 `left/top/right/bottom` 外扩。

普通状态可用主体矩形为：

```text
surfaceBounds.X      = ownerBounds.X + shadow.Left
surfaceBounds.Y      = ownerBounds.Y + shadow.Top
surfaceBounds.Width  = ownerBounds.Width  - shadow.Left - shadow.Right
surfaceBounds.Height = ownerBounds.Height - shadow.Top  - shadow.Bottom
```

初始 placement 仍按 Dialog 主体尺寸和现有 anchor/offset 计算，再 clamp 到 `surfaceBounds`。
拖动和缩放使用同一 clamp，确保主体外加 shadow 后完全落在 `ownerBounds` 内。

`HostWidth`/`HostHeight` 仍表示主体尺寸；当请求尺寸大于可用主体矩形时，现有 min/max 解析会把实际 surface 尺寸限制到可用范围。

非 Linux 不扣除 Dialog shadow，避免扩大本次行为变更范围。

### 5.4 最大化与恢复

最大化 Overlay Dialog 填充 Linux `ownerBounds`，因此不会覆盖标题栏或 window shadow。
最大化仍使用无圆角 surface；恢复后重新读取当前 owner bounds、当前 shadow extents 和 Dialog placement。

最大化不改变 Dialog 自绘 shadow 的主题契约。本次只保证最大化主体的客户区边界；普通状态的完整 shadow 是严格约束对象。

### 5.5 更新时机与生命周期

Presenter 在以下时机重新解析几何：

- 首次加入 DialogLayer 并完成 template/layout。
- DialogLayer/TopLevel size 变化。
- Dialog HostWidth/Height/Min/Max 变化。
- Window CSD、WindowDecorationMargin、FrameShadowThickness、TitleBarHeight、标题栏可见性或 WindowState 变化。
- surface size 变化、最大化和恢复。

新增订阅由 Presenter 现有 disposable owner 持有，并在 `DisposeAsync` 释放；不增加全局服务、timer、dispatcher delay 或 suppression flag。

## 6. 测试设计

先添加失败回归，再修改生产代码。

`OverlayDialogPresenterTests` 增加：

1. Linux CSD modal mask 精确匹配标题栏以下的客户区，没有露底间距。
2. Linux CSD Dialog 拖到顶部时，主体和 top shadow 都位于客户区内。
3. Linux CSD Dialog 拖到左、右、下边界时，四边 shadow 都不越界。
4. Linux owner resize 或装饰边界变化后，mask 与 Dialog 重新约束到同一 owner bounds。
5. Linux 非 CSD managed title bar 不属于 Dialog 可用区。
6. 非 Linux fixture 保留当前 layer bounds 与位置断言，防止范围外行为变化。

现有测试相应调整：原先“surface 主体贴到 Presenter 四边”的断言改为验证 Linux 的完整 visual bounds；
通用非 Linux 测试继续保持原断言。

验证命令：

```bash
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj \
  --framework net10.0 --no-restore \
  --filter FullyQualifiedName~OverlayDialogPresenterTests

dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj \
  --framework net10.0 --no-restore \
  --filter "FullyQualifiedName~Dialog|FullyQualifiedName~WindowResizeArtifactTests|FullyQualifiedName~WindowingPlatformDetectionTests"

git diff --check
```

Linux Wayland Gallery 手动验证：

- 打开 Modal Overlay，确认 mask 从标题栏下沿开始且没有缝隙。
- 分别拖到上、下、左、右边界，确认标题栏/header/正文不互相覆盖，完整阴影保持可见。
- resize、maximize、restore owner 后重复上述检查。

## 7. 契约与风险

- Public API/theme contract changed：No。
- Observable behavior changed：Yes，仅修正 Linux Overlay Dialog 的错误边界。
- Rendered result changed：Yes，mask 与 Dialog 不再进入 Linux 标题栏/窗口阴影区域。
- Files split：No。
- AOT impact：No reflection、dynamic discovery 或新动态 binding。
- Lifecycle risk：低；平台属性订阅必须进入现有 Presenter disposable owner。
- Residual risk：当前开发环境不能直接运行 Linux Wayland compositor，最终仍需要 Linux Gallery 手动验证。

