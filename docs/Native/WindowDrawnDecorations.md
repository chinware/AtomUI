# WindowDrawnDecorations 实战指南

> 面向团队成员的 Avalonia 12 自定义窗体装饰（CSD）完整使用文档。

---

## 目录

1. [背景：为什么是 WindowDrawnDecorations](#1-背景为什么是-windowdrawndecorations)
2. [核心概念与对象模型](#2-核心概念与对象模型)
3. [激活流程（必读）](#3-激活流程必读)
4. [跨平台行为矩阵](#4-跨平台行为矩阵)
5. [自定义 ControlTheme（手把手）](#5-自定义-controltheme手把手)
6. [必须实现的 PART 与可选 PART](#6-必须实现的-part-与可选-part)
7. [伪类与样式选择器](#7-伪类与样式选择器)
8. [关键计算属性：ShadowThickness / FrameThickness / WindowDecorationMargin](#8-关键计算属性shadowthickness--framethickness--windowdecorationmargin)
9. [Window 端正确配置](#9-window-端正确配置)
10. [Resize 抓手机制与"幽灵 resize 区"问题](#10-resize-抓手机制与幽灵-resize-区问题)
11. [X11 输入区裁剪（点击穿透阴影）](#11-x11-输入区裁剪点击穿透阴影)
12. [Wayland 上的对应方案](#12-wayland-上的对应方案)
13. [阴影设计建议](#13-阴影设计建议)
14. [Window ControlTheme 跨平台设计模式](#14-window-controltheme-跨平台设计模式)
15. [常见坑](#15-常见坑)
16. [核心源码索引](#16-核心源码索引)

---

## 1. 背景：为什么是 WindowDrawnDecorations

Avalonia 11 时代自定义标题栏依靠：`Window.TitleBar` + `CaptionButtons` + `ChromeOverlayLayer`，三块分散逻辑、各自加事件、平台一致性差。

Avalonia 12 把它们合并为一个统一控件：**`WindowDrawnDecorations`**（命名空间 `Avalonia.Controls.Chrome`）。

它的本质是：**一个 `StyledElement`**（注意：**不是 `Control`、也不是可视树上的节点**），由框架在窗体激活 CSD 时延迟创建一次、把模板里的 PART 拆成三个 LayerWrapper（Underlay / Overlay / FullscreenPopover），分别插到 `TopLevel.VisualChildren` 的不同层级。

简化结构：

```
Window (TopLevel)
└── VisualChildren
    ├── LayerWrapper(Underlay)         ← Decorations 模板的 Underlay slot
    ├── 用户内容 (Window.Content)
    ├── LayerWrapper(Overlay)          ← Decorations 模板的 Overlay slot
    ├── LayerWrapper(FullscreenPopover)
    └── ResizeGripLayer (内部)
```

Underlay 在用户内容**底下**（画背景圆角/标题栏底色），Overlay 在用户内容**上面**（放 caption 按钮），ResizeGripLayer 在最上层吃 resize 鼠标事件。

---

## 2. 核心概念与对象模型

### 关键类型

| 类型 | 作用 |
|---|---|
| `WindowDrawnDecorations : StyledElement` | 装饰控件本体。**不是 Visual，无法用 GetVisualDescendants 找到它**。 |
| `WindowDrawnDecorationsTemplate : ITemplate` | 模板包装，跟 `ControlTemplate` 类似，只生成 `WindowDrawnDecorationsContent`。 |
| `WindowDrawnDecorationsContent : Control` | 三槽容器，提供 `Underlay / Overlay / FullscreenPopover` 三个 `Control` 属性。 |
| `IWindowDrawnDecorationsTemplate` | 接口（供主题用）。 |
| `DrawnWindowDecorationParts` | `[Flags]` 枚举：`Shadow / Border / TitleBar / ResizeGrips / All`。 |
| `WindowDecorations` | 用户级开关：`None / TitleBar / Full`（设在 `Window` 上）。 |
| `WindowDecorationProperties` | 附加属性提供方，给 PART 标记角色（`ElementRole`）。 |
| `WindowDecorationsElementRole` | 枚举：`DecorationsElement / TitleBar / CloseButton / MinimizeButton / MaximizeButton / FullScreenButton / ResizeN/S/E/W/NW/NE/SW/SE`。 |
| `ResizeGripLayer` | 内部不可见 8 抓手 Control，自动被框架管理。 |

### Decorations vs Decoration**s**（容易搞混）

- `Window.WindowDecorations`：用户开关（`None` 完全无装饰、`TitleBar` 仅标题栏、`Full` 标题栏+边框+阴影）。
- `WindowDrawnDecorations`：装饰控件本体（被框架自动创建，用户不直接 new）。

---

## 3. 激活流程（必读）

不理解这个流程 → 改半天看不到效果。

```
[用户代码] Window.ExtendClientAreaToDecorationsHint = true
          │
          ▼
[Window.cs] PlatformImpl.SetExtendClientAreaToDecorationsHint(true)
          │
          ▼
[平台后端 IWindowImpl] 决定是否需要管理装饰：
          • Win32  : NeedsManagedDecorations = _isClientAreaExtended
                     RequestedDrawnDecorations = TitleBar
          • X11    : NeedsManagedDecorations = UseManagedDecorations
                     ⚠ 取决于 X11PlatformOptions.EnableDrawnDecorations(实验性)
                     RequestedDrawnDecorations = TitleBar | Border | Shadow | ResizeGrips
          • macOS  : NeedsManagedDecorations 硬编码 = false  ❌ 永远不激活
          │
          ▼
[平台后端] 触发 ExtendClientAreaToDecorationsChanged 事件
          │
          ▼
[Window.UpdateDrawnDecorations()]
   ├─ 不 needs → 拆掉旧的（如有）
   └─ needs    → ComputeDecorationParts()  → (parts, state, theme)
                 │
                 ▼
            [TopLevelHost.UpdateDrawnDecorations(parts, state, theme)]
              ├─ 首次：new WindowDrawnDecorations(); 应用 ControlTheme
              ├─ 把 Underlay/Overlay/FullscreenPopover 插入 VisualChildren
              ├─ _decorations.Attach(window)：绑定按钮点击/拖动
              └─ UpdateResizeGripThickness()
```

**关键事实**：

1. `ExtendClientAreaToDecorationsHint=true` 是激活开关。
2. **X11 还要额外打开 `X11PlatformOptions.EnableDrawnDecorations=true`**，否则 `X11Window.SetExtendClientAreaToDecorationsHint` 会在 `X11Window.cs:1547-1548` 直接 `return`，原生标题栏不消失，自定义装饰也不生成。
3. `EnableDrawnDecorations` 标了 `[Experimental("AVALONIA_X11_CSD")]`，编译期警告，需要在 `csproj` 里 `<NoWarn>AVALONIA_X11_CSD</NoWarn>`。
4. macOS 永远走原生 NSWindow titlebar，自定义模板会被忽略。

### 在 `Program.cs` 里启用 X11 CSD

```csharp
public static AppBuilder BuildAvaloniaApp() =>
    AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .With(new X11PlatformOptions { EnableDrawnDecorations = true })
        .WithInterFont()
        .LogToTrace();
```

---

## 4. 跨平台行为矩阵

| 后端 | NeedsManagedDecorations | RequestedDrawnDecorations | `:has-shadow` | `:has-border` | 备注 |
|---|---|---|---|---|---|
| **Win32** | `_isClientAreaExtended` | `TitleBar` | ❌ | ❌ | OS 还在画 frame/shadow，自定义只覆盖标题栏区。 |
| **X11**（`EnableDrawnDecorations=true`） | `true` | `TitleBar \| Border \| Shadow \| ResizeGrips` | ✅ | ✅ | 完全 CSD，框架画一切。 |
| **X11**（默认） | `false` | `default` (None) | — | — | WM 全权负责，自定义 hint 被忽略。 |
| **macOS Native** | 硬编码 `false` | `default` (None) | — | — | 永远走 NSWindow，自定义无效。 |
| **Wayland** | 取决于合成器 | … | — | — | 大多数合成器走 SSD。 |

### 4.1 三句话结论（源码级，必读）

> 不要凭印象，下面每条都给到了 Avalonia 源码行号。

1. **macOS：永不启用 WindowDrawnDecorations**
   - `src/Avalonia.Native/WindowImpl.cs:223` —— `NeedsManagedDecorations => false`，**硬编码常量**，无任何条件分支。
   - `Window.ComputeDecorationParts()`（`src/Avalonia.Controls/Window.cs:762`）首句 `if (!NeedsManagedDecorations) return null;` 直接短路。
   - **后果**：在 macOS 上写 `ControlTheme x:Key="{x:Type WindowDrawnDecorations}"` 是**死代码**，不会被实例化。需要自定义 macOS 标题栏请走 `ExtendClientAreaTitleBarHeightHint` + 系统红绿灯（与 CSD 完全是两套机制）。

2. **Linux（X11）：唯一全功能平台**
   - `src/Avalonia.X11/X11Window.cs:1642-1648` ——
     ```csharp
     RequestedDrawnDecorations =>
       UseManagedDecorations
         ? Border | ResizeGrips | TitleBar | Shadow
         : None;
     ```
   - 激活双门：`X11PlatformOptions.EnableDrawnDecorations=true`（`X11Platform.cs:485`）+ `Window.ExtendClientAreaToDecorationsHint=true`（门见 `X11Window.cs:1547`）。
   - **后果**：四件套（标题栏 / 边框 / 阴影 / resize 抓手）全部由 Avalonia 渲染；`:has-shadow` / `:has-border` / `:has-resize-grips` 伪类全部为 `true`。本文 §10/§11/§13 的所有"幽灵 resize 区""SHAPE 输入区裁剪""三层阴影"技巧**仅在此平台成立**。

3. **Windows（Win32）：仅标题栏可定制**
   - `src/Windows/Avalonia.Win32/WindowImpl.cs:1618-1623` ——
     ```csharp
     NeedsManagedDecorations => _isClientAreaExtended;
     RequestedDrawnDecorations =>
       _isClientAreaExtended
         ? PlatformRequestedDrawnDecoration.TitleBar
         : PlatformRequestedDrawnDecoration.None;
     ```
   - 边框、阴影、resize 抓手始终由 **DWM** 提供。
   - **后果**：
      - `:has-shadow` / `:has-border` / `:has-resize-grips` 伪类**永远为 false** —— 任何依赖这三个伪类的样式（margin / boxshadow / 圆角）在 Win32 都不会触发，**必须用无伪类选择器写一份兜底**。
      - 在 `WindowDrawnDecorations` 里画 `BoxShadow` 也是浪费 —— DWM 已经画了一层，你画的会被它的客户区裁切。
      - X11 SHAPE 那套点击穿透**完全不需要** —— Win32 根本没有阴影 buffer。
      - `ShadowThickness` 在 Win32 上保持 `0` 即可（也是默认值）。

### 4.2 一图速记

```
                            macOS              Linux/X11           Windows
WindowDrawnDecorations 实例化  ❌ 永远 null      ✅ (双门激活)        ✅
PART_TitleBar 渲染           —                ✅                 ✅
PART_WindowBorder 渲染       —                ✅                 ❌ (DWM 画)
BoxShadow / ShadowThickness  —                ✅                 ❌ (DWM 画)
ResizeGrips                  —                ✅                 ❌ (DWM 提供)
:has-shadow / :has-border    —                ✅                 ❌ 永 false
需要 SHAPE 点击穿透          —                ✅ (§11)            ❌
```

---

## 5. 自定义 ControlTheme（手把手）

> Avalonia 12.0.1 NuGet **没有暴露** `Window.WindowDecorationsTheme` 属性（只有 master 分支有）。
> 所以**只能**通过覆盖 `{x:Type WindowDrawnDecorations}` 的默认 `ControlTheme` 来定制。

### Step 1：创建主题资源字典 `Themes/CustomWindowDrawnDecorations.axaml`

```xml
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:chrome="clr-namespace:Avalonia.Controls.Chrome;assembly=Avalonia.Controls">

  <ControlTheme x:Key="{x:Type chrome:WindowDrawnDecorations}"
                TargetType="chrome:WindowDrawnDecorations">

    <Setter Property="DefaultTitleBarHeight" Value="36"/>
    <Setter Property="DefaultFrameThickness" Value="1"/>
    <Setter Property="DefaultShadowThickness" Value="32"/>

    <Setter Property="Template">
      <chrome:WindowDrawnDecorationsTemplate>
        <chrome:WindowDrawnDecorationsContent>

          <!-- ===== UNDERLAY：用户内容下方，画背景/边框/标题栏底色 ===== -->
          <chrome:WindowDrawnDecorationsContent.Underlay>
            <Panel x:Name="PART_UnderlayWrapper">
              <Border x:Name="PART_WindowBorder"
                      Background="White"
                      BorderBrush="#22000000"
                      BorderThickness="{TemplateBinding FrameThickness}"
                      CornerRadius="10"
                      IsHitTestVisible="False"/>
              <Border x:Name="PART_TitleBar"
                      VerticalAlignment="Top"
                      Height="{TemplateBinding TitleBarHeight}"
                      Background="#F5F5F5"
                      CornerRadius="10,10,0,0"
                      IsVisible="{TemplateBinding HasTitleBar}"
                      chrome:WindowDecorationProperties.ElementRole="TitleBar"/>
            </Panel>
          </chrome:WindowDrawnDecorationsContent.Underlay>

          <!-- ===== OVERLAY：用户内容上方，放 caption 按钮+标题文字 ===== -->
          <chrome:WindowDrawnDecorationsContent.Overlay>
            <Panel x:Name="PART_OverlayWrapper">
              <DockPanel VerticalAlignment="Top"
                         Height="{TemplateBinding TitleBarHeight}"
                         IsVisible="{TemplateBinding HasTitleBar}">
                <StackPanel DockPanel.Dock="Right" Orientation="Horizontal">
                  <Button x:Name="PART_MinimizeButton" Content="—"
                          chrome:WindowDecorationProperties.ElementRole="MinimizeButton"/>
                  <Button x:Name="PART_MaximizeButton" Content="□"
                          chrome:WindowDecorationProperties.ElementRole="MaximizeButton"/>
                  <Button x:Name="PART_FullScreenButton" Content="⛶"
                          chrome:WindowDecorationProperties.ElementRole="FullScreenButton"/>
                  <Button x:Name="PART_CloseButton" Content="×"
                          chrome:WindowDecorationProperties.ElementRole="CloseButton"/>
                </StackPanel>

                <Panel x:Name="PART_TitleTextPanel">
                  <TextBlock Text="{Binding $parent[Window].Title}"
                             VerticalAlignment="Center"
                             HorizontalAlignment="Center"/>
                </Panel>
              </DockPanel>
            </Panel>
          </chrome:WindowDrawnDecorationsContent.Overlay>

          <!-- ===== FullscreenPopover：全屏模式下从顶部下拉的浮层 ===== -->
          <chrome:WindowDrawnDecorationsContent.FullscreenPopover>
            <Border Background="#80000000" Height="36">
              <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
                <Button x:Name="PART_PopoverFullScreenButton" Content="⛶"/>
                <Button x:Name="PART_PopoverCloseButton" Content="×"/>
              </StackPanel>
            </Border>
          </chrome:WindowDrawnDecorationsContent.FullscreenPopover>

        </chrome:WindowDrawnDecorationsContent>
      </chrome:WindowDrawnDecorationsTemplate>
    </Setter>

    <!-- 伪类样式（见 §7） -->
    <Style Selector="^:has-shadow /template/ Panel#PART_UnderlayWrapper">
      <Setter Property="Margin" Value="{TemplateBinding ShadowThickness}"/>
    </Style>
    <Style Selector="^:has-shadow /template/ Panel#PART_OverlayWrapper">
      <Setter Property="Margin" Value="{TemplateBinding ShadowThickness}"/>
    </Style>
    <Style Selector="^ /template/ Border#PART_WindowBorder">
      <Setter Property="BoxShadow"
              Value="0 6 16 0 #14000000,
                     0 3 6 -4 #1F000000,
                     0 9 28 8 #0D000000"/>
    </Style>
    <Style Selector="^:maximized /template/ Border#PART_WindowBorder">
      <Setter Property="CornerRadius" Value="0"/>
    </Style>
    <Style Selector="^:maximized /template/ Border#PART_TitleBar">
      <Setter Property="CornerRadius" Value="0"/>
    </Style>
  </ControlTheme>
</ResourceDictionary>
```

### Step 2：把字典合并进 `App.Resources`

```xml
<!-- App.axaml -->
<Application xmlns="https://github.com/avaloniaui" …>
  <Application.Styles>
    <FluentTheme/>
  </Application.Styles>
  <Application.Resources>
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <ResourceInclude Source="avares://YourApp/Themes/CustomWindowDrawnDecorations.axaml"/>
      </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
  </Application.Resources>
</Application>
```

合并到 **`Application.Resources`**，不是 `Application.Styles`。`ControlTheme` 通过 `x:Key="{x:Type WindowDrawnDecorations}"` 索引、覆盖 Fluent 内置的同 key 主题。

---

## 6. 必须实现的 PART 与可选 PART

`WindowDrawnDecorations.OnApplyTemplate` 通过命名查找绑定按钮事件。**缺一不可**（缺则按钮不工作或抛 NRE）。

### 必填 PART

| 名称 | 类型 | 作用 |
|---|---|---|
| `PART_TitleBar` | `Border` (or any Control) | 拖动区，自动挂 `BeginMoveDrag`。 |
| `PART_CloseButton` | `Button` | 关闭。 |
| `PART_MinimizeButton` | `Button` | 最小化。 |
| `PART_MaximizeButton` | `Button` | 最大化/还原（同一按钮，按 WindowState 切换 glyph）。 |
| `PART_FullScreenButton` | `Button` | 全屏切换。 |
| `PART_PopoverCloseButton` | `Button` | 全屏弹出层的关闭按钮。 |
| `PART_PopoverFullScreenButton` | `Button` | 全屏弹出层的退出全屏按钮。 |

### 可选 PART（命名约定，便于样式定位）

| 名称 | 用途 |
|---|---|
| `PART_WindowBorder` | 主背景/圆角/阴影承载 `Border`。 |
| `PART_UnderlayWrapper` | Underlay 根 Panel，便于设 ShadowThickness Margin。 |
| `PART_OverlayWrapper` | Overlay 根 Panel，同上。 |
| `PART_TitleTextPanel` | 标题文字区，便于 `:has-border` 时整体偏移。 |

### `WindowDecorationProperties.ElementRole` 附加属性

不是 PART 命名，但**框架靠它判断元素角色**（拖动 / 双击最大化 / 系统菜单等）。建议给所有按钮 + TitleBar 都标。

枚举：`DecorationsElement / TitleBar / CloseButton / MinimizeButton / MaximizeButton / FullScreenButton / ResizeN/S/E/W/NW/NE/SW/SE`。

```xml
<Border chrome:WindowDecorationProperties.ElementRole="TitleBar" .../>
<Button chrome:WindowDecorationProperties.ElementRole="CloseButton" .../>
```

---

## 7. 伪类与样式选择器

`WindowDrawnDecorations` 在不同状态下自动加伪类：

| 伪类 | 触发条件 |
|---|---|
| `:normal` | `WindowState == Normal` |
| `:maximized` | `WindowState == Maximized` |
| `:fullscreen` | `WindowState == FullScreen` |
| `:has-titlebar` | RequestedParts 含 `TitleBar` |
| `:has-shadow` | RequestedParts 含 `Shadow`（仅 X11 CSD 满足） |
| `:has-border` | RequestedParts 含 `Border`（仅 X11 CSD 满足） |
| `:has-minimize` | `Window.CanMinimize == true` |
| `:has-maximize` | `Window.CanMaximize == true` |
| `:has-fullscreen` | `Window.CanFullScreen == true` |

样式写法（`^` 是当前 ControlTheme 自身）：

```xml
<!-- 仅最大化时圆角归零 -->
<Style Selector="^:maximized /template/ Border#PART_WindowBorder">
  <Setter Property="CornerRadius" Value="0"/>
</Style>

<!-- 没有最小化按钮时隐藏对应 Button -->
<Style Selector="^:not(:has-minimize) /template/ Button#PART_MinimizeButton">
  <Setter Property="IsVisible" Value="False"/>
</Style>
```

⚠ Win32 上 `:has-shadow` / `:has-border` 永远 false（OS 自己画框），任何依赖这俩伪类的样式在 Windows 不会生效。要 Win32 也有阴影/边框 → 写无条件选择器（不带伪类）。

---

## 8. 关键计算属性：ShadowThickness / FrameThickness / WindowDecorationMargin

### 在 `WindowDrawnDecorations` 上

| 属性 | 类型 | 说明 |
|---|---|---|
| `DefaultShadowThickness` | `Thickness` | 主题里设；激活了 Shadow part 才生效。 |
| `DefaultFrameThickness` | `Thickness` | 边框宽度；激活了 Border part 才生效。 |
| `DefaultTitleBarHeight` | `double` | 标题栏高度。 |
| `ShadowThickness` (`DirectProperty`) | `Thickness` | **运行时实际值**（DPI rounded、parts gating 之后）。 |
| `FrameThickness` (`DirectProperty`) | `Thickness` | 同上。 |
| `TitleBarHeight` (`DirectProperty`) | `double` | 同上。 |

### 在 `Window` 上

```csharp
// Window.cs:127-131, 800-836
public static readonly DirectProperty<Window, Thickness> WindowDecorationMarginProperty;

public Thickness WindowDecorationMargin { get; private set; }
// = (frame.Left+shadow.Left, titleBarHeight+frame.Top+shadow.Top, ...)
```

**这是用户内容应该 inset 的 margin**。直接绑给 `Window.Content` 根容器：

```xml
<Window …>
  <Border Margin="{Binding $parent[Window].WindowDecorationMargin}"
          Background="White">
    <!-- 用户实际内容 -->
  </Border>
</Window>
```

不绑 → 用户内容会铺满整个 OS 客户区（包括阴影 buffer + 标题栏区域），跟 underlay 不对齐。

---

## 9. Window 端正确配置

```xml
<Window xmlns="https://github.com/avaloniaui" …
        Title="My Window"
        Width="900" Height="600"
        ExtendClientAreaToDecorationsHint="True"
        Background="Transparent"
        TransparencyLevelHint="Transparent"
        WindowDecorations="Full">
  <Border Margin="{Binding $parent[Window].WindowDecorationMargin}"
          Background="White"
          CornerRadius="0,0,10,10">
    <!-- 用户内容 -->
  </Border>
</Window>
```

| 属性 | 必填 | 说明 |
|---|---|---|
| `ExtendClientAreaToDecorationsHint="True"` | ✅ | 激活 CSD 流程。 |
| `Background="Transparent"` | ✅ | 让 Underlay 的圆角能看见桌面背景。 |
| `TransparencyLevelHint="Transparent"` | ✅ X11/Win32 | 请求 OS 提供透明合成。 |
| `WindowDecorations` | 可选 | `Full`（默认）/ `TitleBar` / `None`。 |

---

## 10. Resize 抓手机制与"幽灵 resize 区"问题

源码 `TopLevelHost.Decorations.cs:188-200`：

```csharp
// Grips strictly cover frame + shadow area, never client area
_resizeGrips.GripThickness = new Thickness(
    frame.Left + shadow.Left, frame.Top + shadow.Top,
    frame.Right + shadow.Right, frame.Bottom + shadow.Bottom);
```

`ResizeGripLayer` 占据 **整个 frame + shadow 区**，鼠标进去就：
1. 光标变 ↘ / ↔（resize cursor）
2. 鼠标事件被 OS 窗体吃掉，**无法穿透到背后的应用**（X11 默认 input region = 整个 OS 窗口矩形）

视觉上那块是透明的、看着像"窗外"，但其实功能上是"窗框边缘"。

**取舍**：
- `ShadowThickness` 越大 → 阴影越好看 → resize 抓手区越大 → "幽灵区"越违和
- 不加任何处理时建议 `ShadowThickness ≤ 16`

---

## 11. X11 输入区裁剪（点击穿透阴影）

要兼顾"大阴影 + 不违和"，需要绕过 Avalonia 调 X11 SHAPE 扩展，把 input region 收窄到只包含可见窗体 + 一个窄的 resize 抓手带。

### 思路

`XShapeCombineRectangles(display, win, ShapeInput, ShapeSet, &rect, 1, ...)`：把 input region 替换成单个矩形。矩形外的像素事件 fall-through 到下层窗口。

### 推荐的辅助类组织

建议在你的项目里拆成两个静态类（命名仅供参考）：

- **`X11InputShape`**：纯 P/Invoke 封装 `libX11.so.6` + `libXext.so.6`，对外只暴露 `SetInputRectangle / ResetInputRegion` 两个方法、`IsSupported` 一个属性。
- **`ClickThroughShadow`**：挂载到 Avalonia `Window`，监听 `Bounds / WindowDecorationMargin / WindowState / TransparencyLevelHint` 变化，自动调用上面那个方法重算 input region。

#### `X11InputShape`

```csharp
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

[SupportedOSPlatform("linux")]
internal static class X11InputShape
{
    private const string LibX11 = "libX11.so.6";
    private const string LibXext = "libXext.so.6";

    [DllImport(LibX11)] private static extern IntPtr XOpenDisplay(IntPtr displayName);
    [DllImport(LibX11)] private static extern int XFlush(IntPtr display);

    [DllImport(LibXext)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool XShapeQueryExtension(
        IntPtr display, out int eventBase, out int errorBase);

    [DllImport(LibXext)]
    private static extern void XShapeCombineRectangles(
        IntPtr display, IntPtr window, int kind,
        int xOff, int yOff,
        IntPtr rectangles, int nRectangles,
        int op, int ordering);

    private const int ShapeInput = 2;
    private const int ShapeSet   = 0;
    private const int Unsorted   = 0;

    [StructLayout(LayoutKind.Sequential)]
    private struct XRectangle
    {
        public short  X;
        public short  Y;
        public ushort Width;
        public ushort Height;
    }

    private static readonly object s_lock = new();
    private static IntPtr s_display;
    private static bool   s_init;
    private static bool   s_supported;

    public static bool IsSupported
    {
        get { EnsureInit(); return s_supported; }
    }

    private static void EnsureInit()
    {
        if (s_init) return;
        lock (s_lock)
        {
            if (s_init) return;
            try
            {
                // ⚠ 自己开独立 display 连接，不要复用 Avalonia 的——
                // 避免和主线程的 X11 事件循环抢锁。
                s_display = XOpenDisplay(IntPtr.Zero);
                if (s_display != IntPtr.Zero)
                    s_supported = XShapeQueryExtension(s_display, out _, out _);
            }
            catch (DllNotFoundException)       { s_supported = false; }
            catch (EntryPointNotFoundException) { s_supported = false; }
            finally                             { s_init = true; }
        }
    }

    /// <summary>
    /// 把 window 的 X11 input region 替换为单个矩形。
    /// 矩形外的像素事件 fall-through 到下层窗口（跨进程也可）。
    /// 坐标是 device pixel，不是 DIP。
    /// </summary>
    public static void SetInputRectangle(IntPtr window, int x, int y, int width, int height)
    {
        if (!IsSupported || window == IntPtr.Zero) return;
        if (width <= 0 || height <= 0) return;

        var rect = new XRectangle
        {
            X      = (short) Math.Clamp(x, short.MinValue, short.MaxValue),
            Y      = (short) Math.Clamp(y, short.MinValue, short.MaxValue),
            Width  = (ushort)Math.Min(width,  ushort.MaxValue),
            Height = (ushort)Math.Min(height, ushort.MaxValue),
        };

        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<XRectangle>());
        try
        {
            Marshal.StructureToPtr(rect, ptr, false);
            lock (s_lock)
            {
                XShapeCombineRectangles(
                    s_display, window, ShapeInput,
                    xOff: 0, yOff: 0,
                    rectangles: ptr, nRectangles: 1,
                    op: ShapeSet, ordering: Unsorted);
                XFlush(s_display);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    public static void ResetInputRegion(IntPtr window, int width, int height)
        => SetInputRectangle(window, 0, 0, width, height);
}
```

#### `ClickThroughShadow`

```csharp
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Controls;

internal static class ClickThroughShadow
{
    /// <summary>
    /// 内边保留多少 DIP 作为 resize 抓手（外圈 = shadowThickness - ResizeBand 全穿透）。
    /// </summary>
    public const double ResizeBand = 6.0;

    public static void Attach(Window window, double shadowThickness)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;
        if (!IsX11Supported()) return;
        if (shadowThickness <= 0) return;

        void Reapply()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Apply(window, shadowThickness);
        }

        window.Opened += (_, _) => Reapply();
        window.PropertyChanged += (_, e) =>
        {
            if (e.Property == Visual.BoundsProperty
                || e.Property == Window.WindowDecorationMarginProperty
                || e.Property == Window.WindowStateProperty
                || e.Property == TopLevel.TransparencyLevelHintProperty)
            {
                Reapply();
            }
        };
    }

    private static bool IsX11Supported()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return false;
        return X11SupportedCore();
    }

    [SupportedOSPlatform("linux")]
    private static bool X11SupportedCore() => X11InputShape.IsSupported;

    [SupportedOSPlatform("linux")]
    private static void Apply(Window window, double shadowThickness)
    {
        // 仅 X11 后端的 PlatformHandle 是 "XID"；Wayland 后端将来是别的描述符
        var handle = window.TryGetPlatformHandle();
        if (handle is null || handle.HandleDescriptor != "XID") return;
        var xid = handle.Handle;
        if (xid == IntPtr.Zero) return;

        var size = window.ClientSize;
        if (size.Width <= 0 || size.Height <= 0) return;

        var scale = window.RenderScaling <= 0 ? 1.0 : window.RenderScaling;
        var fullW = (int)Math.Round(size.Width  * scale);
        var fullH = (int)Math.Round(size.Height * scale);

        // 最大化 / 全屏：框架把 ShadowThickness 归零，整个客户区都要可点
        if (window.WindowState != WindowState.Normal)
        {
            X11InputShape.ResetInputRegion(xid, fullW, fullH);
            return;
        }

        var inset = Math.Max(0, shadowThickness - ResizeBand);
        if (inset <= 0)
        {
            X11InputShape.ResetInputRegion(xid, fullW, fullH);
            return;
        }

        var x = (int)Math.Round(inset * scale);
        var y = (int)Math.Round(inset * scale);
        var w = (int)Math.Round((size.Width  - inset * 2) * scale);
        var h = (int)Math.Round((size.Height - inset * 2) * scale);

        X11InputShape.SetInputRectangle(xid, x, y, w, h);
    }
}
```

### 调用

```csharp
public MainWindow()
{
    InitializeComponent();
    ClickThroughShadow.Attach(this, shadowThickness: 32);
    //                                          ^^ 必须跟主题里 DefaultShadowThickness 一致
}
```

### 关键点

1. **`shadowThickness` 参数必须与主题里的 `DefaultShadowThickness` 同步**：因为 `WindowDrawnDecorations` 是 `StyledElement`，不在可视树，外部代码读不到运行时计算的 `ShadowThickness`，只能手动传值。
2. **`ResizeBand=6`**：保留 6px DIP 的内边作为 resize 抓手；外圈 `shadowThickness - 6` 完全穿透。
3. **DPI**：用 `Window.RenderScaling` 把 DIP 转物理像素后再喂给 X11。
4. **自己开 display 连接**：`XOpenDisplay(IntPtr.Zero)` 拿独立连接，避免和 Avalonia 主线程的 X11 事件循环抢锁。
5. **平台 gating**：`RuntimeInformation.IsOSPlatform(OSPlatform.Linux)` + `XShapeQueryExtension`，Win32/macOS/Wayland 自动 no-op。
6. **maximized / fullscreen** 时框架把 `ShadowThickness` 归零，`ClickThroughShadow` 把 input region 重置为整个 OS 客户区。

---

## 12. Wayland 上的对应方案

Wayland 有**直接等价**的 API，而且比 X11 SHAPE 更干净——它就是核心协议的一部分：

```c
// 等价于 XShapeCombineRectangles(... ShapeInput ...)
struct wl_region *r = wl_compositor_create_region(compositor);
wl_region_add(r, x, y, w, h);              // 可叠加多个矩形
wl_surface_set_input_region(surface, r);
wl_surface_commit(surface);                // ⚠ 必须 commit 才生效
wl_region_destroy(r);
```

| 维度 | X11 `XShapeCombineRectangles` | Wayland `wl_surface.set_input_region` |
|---|---|---|
| 协议层 | XShape 扩展（理论上可缺） | 核心协议（`wl_compositor` v1+，**必有**） |
| 坐标单位 | 物理像素 | surface 本地坐标（合成器自动应用 buffer scale） |
| 多矩形 | `XRectangle[]` + `nRectangles` | `wl_region_add()` 累加 |
| `NULL`/重置 | 不支持（必须显式给矩形） | `set_input_region(NULL)` = 整个 surface（默认） |
| 生效时机 | `XFlush` 后立刻 | 必须 `wl_surface.commit` 后下一帧 |

GTK / Qt 在 Wayland 下画 CSD 阴影**就是用这个 API** 让阴影点击穿透——这是 Wayland CSD 的事实标准。

### 但有个关键现实：Avalonia 12 没有原生 Wayland 后端

- Avalonia 12.0.1 Linux 端**只有 `Avalonia.X11`**，没有 `Avalonia.Wayland`（master 分支有 PR 在推进，未发布）。
- Wayland 桌面上跑 Avalonia 应用 → 实际走 **XWayland 兼容层**（X11 协议被合成器翻译执行）。
- 验证方式：

  ```bash
  echo $XDG_SESSION_TYPE   # wayland
  echo $DISPLAY            # 仍有值（如 :1），说明 XWayland 在线
  xdpyinfo | grep -i shape # SHAPE 扩展可用
  ```

- **结论**：上述 `X11InputShape` 助手在 XWayland 下**原样工作**，无需额外代码。Wayland 用户也享受点击穿透。

### 如果将来 Avalonia 出了原生 Wayland 后端

要新增一个 `WaylandInputRegion.cs` 平行于 `X11InputShape.cs`，并在 `ClickThroughShadow.Apply` 里按 `HandleDescriptor` 分发。骨架（伪代码）：

```csharp
[SupportedOSPlatform("linux")]
internal static class WaylandInputRegion
{
    [DllImport("libwayland-client.so.0")]
    private static extern IntPtr wl_compositor_create_region(IntPtr compositor);
    [DllImport("libwayland-client.so.0")]
    private static extern void wl_region_add(IntPtr region, int x, int y, int w, int h);
    [DllImport("libwayland-client.so.0")]
    private static extern void wl_region_destroy(IntPtr region);
    [DllImport("libwayland-client.so.0")]
    private static extern void wl_proxy_marshal(IntPtr proxy, uint opcode, /* args */);
    // wl_surface.set_input_region opcode = 2 (检 wayland.xml)
    // wl_surface.commit         opcode = 6

    public static void SetInputRectangle(
        IntPtr wlSurface, IntPtr wlCompositor,
        int x, int y, int w, int h)
    {
        var r = wl_compositor_create_region(wlCompositor);
        wl_region_add(r, x, y, w, h);
        wl_proxy_marshal(wlSurface, 2 /*set_input_region*/, r);
        wl_proxy_marshal(wlSurface, 6 /*commit*/);
        wl_region_destroy(r);
    }
}
```

### 落地难点（提前预判）

1. **拿 `wl_surface*`**
   Avalonia 需要在 `IPlatformHandle` 暴露 Wayland 描述符（X11 是 `"XID"`，Wayland 大概率是 `"wl_surface"` 或类似）。在分发时：

   ```csharp
   switch (handle.HandleDescriptor)
   {
       case "XID":        X11InputShape.SetInputRectangle(handle.Handle, …); break;
       case "wl_surface": WaylandInputRegion.SetInputRectangle(handle.Handle, comp, …); break;
   }
   ```

2. **拿 `wl_compositor*`**
   Wayland 对象 ID 是**连接相关**的（不像 X11 的 XID 是 server-side 的全局 ID），所以**不能像 X11 那样自己 `wl_display_connect()` 开独立连接**——拿不到 Avalonia 创建的 surface。必须从 Avalonia 后端拿到主连接的 `wl_compositor*`。这要等 Avalonia 暴露相应 API。

3. **Commit 时机**
   Wayland 状态是双缓冲，`set_input_region` 必须等 `wl_surface.commit` 才生效；而 Avalonia 在 frame callback 里 commit，从外部插一刀 commit 会和合成器协议争抢——正确做法是把 `set_input_region` 调用排到主循环、跟 Avalonia 的下一次 commit 合并。需要 Avalonia 提供 hook（如"准备 commit 前"事件）。

4. **Subsurface 问题**
   如果 Avalonia 用 subsurface 实现弹窗/Popup，每个 subsurface 的 input region 是**独立**的，需要分别处理。X11 没有这个问题（一个窗口一个 XID）。

5. **DPI / fractional scaling**
   坐标单位是 surface local，合成器会按 `wl_surface.set_buffer_scale` / `wp_fractional_scale_v1` 自动缩放——通常**不需要**像 X11 那样自己乘 `RenderScaling`，反而要避免双重缩放。

### 实操建议

**现阶段**：什么都不用做。X11 + XWayland 的现有方案覆盖 100% Linux 桌面用户。

**长期跟踪**：
- 关注 Avalonia GitHub 上的 Wayland PR（搜索 issue/PR `Avalonia.Wayland`）；
- 一旦发布原生 Wayland 后端，需要的不只是补个 `WaylandInputRegion.cs`，而是要 Avalonia 先暴露 `wl_compositor*` + commit hook，否则做不出来；
- 在那之前，X11 路径就是 Linux 上的唯一答案。

---

## 13. 阴影设计建议

### 配方一：Ant Design 风（轻盈、悬浮感）

`ShadowThickness=32`，需要 input region 裁剪：

```xml
<Setter Property="BoxShadow"
        Value="0 6 16 0 #14000000,    <!-- rgba(0,0,0,0.08) -->
               0 3 6 -4 #1F000000,    <!-- rgba(0,0,0,0.12) -->
               0 9 28 8 #0D000000"/>  <!-- rgba(0,0,0,0.05) -->
```

### 配方二：macOS Big Sur 风（深、自然）

`ShadowThickness=24`，需要 input region 裁剪：

```xml
<Setter Property="BoxShadow"
        Value="0 2 6 0 #33000000,    <!-- ambient -->
               0 12 36 0 #59000000"/> <!-- key -->
```

### 配方三：保守（不裁剪 input region 也不违和）

`ShadowThickness=16`：

```xml
<Setter Property="BoxShadow"
        Value="0 1 3 0 #2E000000,
               0 6 14 0 #4A000000"/>
```

### 通用规则

- **不要带颜色**（青/蓝/紫）→ 永远显假。只用纯黑 + 低 alpha。
- **至少两层**：ambient（紧贴）+ key（远投），单层永远糊。
- **Spread 慎用**：spread > 0 让阴影向外膨胀；负 spread 让阴影"内缩"，做近距亮影时配 `0 3 6 -4` 挺好用。
- **BoxShadow 总延展 ≤ ShadowThickness**：估算公式 `|offsetY| + blur/2 + spread` 必须 ≤ ShadowThickness，否则被 OS 客户区裁切发硬。
- **Color #AARRGGBB**：rgba alpha 转 hex：`Math.Round(alpha * 255).ToString("X2")`。0.08 ≈ `#14`，0.12 ≈ `#1F`，0.05 ≈ `#0D`。

---

## 14. Window ControlTheme 跨平台设计模式

> **场景**：你给 `Window`（或自定义 `AppWindow : Window`）写了一份 ControlTheme，里面有自绘的标题栏控件（比如 macOS 风红绿灯）。
> macOS 下 `WindowDrawnDecorations` 永不实例化，必须由 Window 自己的模板出标题栏；
> Win32 / Linux 下 `WindowDrawnDecorations` 已经替你画了标题栏 —— **如果 Window 模板里也画一份，就出现"双标题栏"**。
>
> 这一节给出团队推荐的优雅解法。

### 14.1 核心信号：`WindowDecorationMargin.Top > 0`

不要用 `OperatingSystem.IsMacOS()` 之类做 OS 嗅探 —— 嗅探**真正的运行时信号**才是 Avalonia way：

| 平台 | `WindowDecorationMargin.Top` | 含义 |
|---|---|---|
| macOS | **永远 `0`**（`NeedsManagedDecorations` 硬编码 false）| 没人替你画 → 你自己画 |
| Win32（extend hint=on） | **= 标题栏高** > 0 | `WindowDrawnDecorations` 在画 |
| X11（双门激活） | **= 标题栏高 + 阴影厚** > 0 | `WindowDrawnDecorations` 在画 |
| 任意平台关 extend hint | `0` | OS 原生标题栏 |

**结论**：`WindowDecorationMargin.Top > 0 ⇔ "WindowDrawnDecorations 正在替我画标题栏"**。

这个信号有三大优点：
1. **天然跨平台**：不写 `#if MACOS`
2. **运行时响应**：用户切 `ExtendClientAreaToDecorationsHint`、X11 关掉 `EnableDrawnDecorations`、最大化导致阴影变 0 → 全部自动适配
3. **未来友好**：等 Avalonia 出原生 Wayland 后端，无需改一行代码

### 14.2 推荐方案：用 Style 替换 `Template`，而不是 `IsVisible`

**反模式（不推荐）**：

```xml
<!-- ❌ Border 实例一直存在，只是不显示 -->
<Border x:Name="PART_CustomTitleBar"
        IsVisible="{Binding $parent[Window].WindowDecorationMargin.Top,
                            Converter={x:Static local:DoubleIsZeroConverter.Instance}}">
  <local:MacTrafficLights/>
</Border>
```

问题：CSD 激活时，`PART_CustomTitleBar` 仍在 visual tree 里占内存、参与 measure 遍历；同时 `WindowDrawnDecorations` 内部又有一份 PART_TitleBar —— **两份 caption 实例并存**。

**推荐模式**：在 ControlTheme 里挂 `Style`，伪类激活时**整体替换 `Template`**，让自绘标题栏从根本上不存在：

```xml
<ControlTheme x:Key="{x:Type local:AppWindow}" TargetType="local:AppWindow">

  <!-- 默认模板：包含自绘标题栏（macOS / 关 CSD 场景） -->
  <Setter Property="Template">
    <ControlTemplate>
      <Panel>
        <Border x:Name="PART_CustomTitleBar"
                Height="32" VerticalAlignment="Top"
                Background="{DynamicResource TitleBarBrush}">
          <local:MacTrafficLights/>
        </Border>
        <ContentPresenter Content="{TemplateBinding Content}"
                          Margin="0,32,0,0"/>
      </Panel>
    </ControlTemplate>
  </Setter>

  <!-- CSD 激活：换一份不带自绘 TitleBar 的模板 -->
  <Style Selector="^:csd-active">
    <Setter Property="Template">
      <ControlTemplate>
        <Panel>
          <ContentPresenter Content="{TemplateBinding Content}"
                            Margin="{Binding $parent[local:AppWindow].WindowDecorationMargin}"/>
        </Panel>
      </ControlTemplate>
    </Setter>
  </Style>

</ControlTheme>
```

伪类切换时 Avalonia 会**销毁旧 visual tree、按新 Template 重建**，自绘标题栏的 Border **从此不存在**。`Content` 是引用绑定，应用内容状态不丢。

### 14.3 维护伪类：`AppWindow` 基类

```csharp
public class AppWindow : Window
{
    static AppWindow()
    {
        WindowDecorationMarginProperty.Changed
            .AddClassHandler<AppWindow>((w, _) => w.UpdateCsdPseudoClass());
    }

    public AppWindow() => UpdateCsdPseudoClass();

    private void UpdateCsdPseudoClass()
        => PseudoClasses.Set(":csd-active", WindowDecorationMargin.Top > 0);
}
```

`WindowDecorationMargin` 是 `StyledProperty`，订阅静态 `Changed` 事件即可全局响应所有实例。

### 14.4 两种方案对照

| 维度 | `IsVisible` 隐藏 | Style 替换 Template ⭐ |
|---|---|---|
| Border 实例存在？ | ✅ 一直在 | ❌ CSD 激活时根本不创建 |
| 切换时 visual tree | 不变 | 重建一次（轻量） |
| Measure / Arrange 节点 | 包含隐藏节点 | 完全干净 |
| 输入命中遍历 | 跳过 IsVisible=False | 完全跳过 |
| 模板内部状态 | 保留 | 丢失（`Content` 不受影响）|
| 代码风格 | 命令式 | 声明式 |
| Avalonia 内部惯用？ | 偶尔 | **是**，Fluent 主题大量使用 |

**结论**：永远用 Style 替换 Template，除非你确实需要在切换时保留模板内部状态。

### 14.5 共享 caption UI：避免实现两份红绿灯

如果不想在 Window ControlTheme 和 `WindowDrawnDecorations` ControlTheme 里各写一份红绿灯按钮，把它抽成 `UserControl`：

```
App.Controls/
  AppWindow.cs                    -- 维护 :csd-active 伪类
  MacTrafficLights.axaml(.cs)     -- ⭐ 共享 caption 控件

Themes/
  AppWindow.axaml                 -- 双 Template ControlTheme（§14.2）
  CustomWindowDrawnDecorations.axaml
                                  -- WindowDrawnDecorations 的 ControlTheme，
                                     PART_TitleBar 里也塞 <local:MacTrafficLights/>
```

两个标题栏宿主复用同一个 caption 实现：

```xml
<!-- Themes/CustomWindowDrawnDecorations.axaml -->
<Border x:Name="PART_TitleBar" Height="36">
  <local:MacTrafficLights/>          <!-- ⭐ 同一个 -->
</Border>

<!-- Themes/AppWindow.axaml （默认模板分支）-->
<Border x:Name="PART_CustomTitleBar" Height="32">
  <local:MacTrafficLights/>          <!-- ⭐ 同一个 -->
</Border>
```

**唯一一份 caption UI 实现，两个宿主复用** —— 跨平台视觉自动对齐，零冗余。

### 14.6 反模式集合（别这么干）

```csharp
// ❌ OS 嗅探：写死分支无法响应运行时变化
if (OperatingSystem.IsMacOS()) titleBar.IsVisible = true;
```

```xml
<!-- ❌ x:OnPlatform 在 XAML load 时一次性求值，不响应运行时切换 -->
<Border IsVisible="{x:OnPlatform True, OSX=False}"/>
```

```xml
<!-- ❌ 既画自绘 TitleBar 又开 ExtendClientAreaToDecorationsHint，没做任何条件控制 -->
<!-- → Linux/Win32 上双标题栏并存 -->
```

```csharp
// ❌ 想从外部读 WindowDrawnDecorations 的属性来判断
var hasCsd = window.GetVisualDescendants<WindowDrawnDecorations>().Any();
//                                       ^^^^^^^^^^^^^^^^^^^^^^^^^^
// WindowDrawnDecorations 是 StyledElement，不是 Visual，永远找不到
```

### 14.7 小结流程

把这套模式落地的清单：

1. **创建** `AppWindow : Window`，监听 `WindowDecorationMargin` 维护 `:csd-active` 伪类
2. **抽出** caption 控件成 `UserControl`（如 `MacTrafficLights`）
3. **写** `AppWindow` 的 ControlTheme，用 §14.2 的双 Template 模式
4. **写** `WindowDrawnDecorations` 的 ControlTheme，`PART_TitleBar` 里复用同一个 caption 控件
5. **应用** `ExtendClientAreaToDecorationsHint=true` 到所有 `AppWindow` 实例
6. **配置** `X11PlatformOptions { EnableDrawnDecorations = true }`

最终效果：

| 平台 | 谁画标题栏 | caption 控件实例 |
|---|---|---|
| macOS | `AppWindow` 默认模板的 `PART_CustomTitleBar` | 1 份 |
| X11 | `WindowDrawnDecorations` 的 `PART_TitleBar` | 1 份 |
| Win32 | `WindowDrawnDecorations` 的 `PART_TitleBar` | 1 份 |

**永远只有一份 caption 实例**，零 OS 分支，自动响应运行时变化。

---

## 15. 常见坑

| 坑 | 现象 | 解决 |
|---|---|---|
| 没设 `EnableDrawnDecorations=true` | X11 上原生标题栏还在 | `Program.cs` 加 `X11PlatformOptions { EnableDrawnDecorations = true }` |
| `EnableDrawnDecorations` 编译警告 | `AVALONIA_X11_CSD` 实验性 | csproj `<NoWarn>AVALONIA_X11_CSD</NoWarn>` |
| 引用 `Avalonia.Diagnostics 12.0.1` | NuGet 没有这个包 | 删掉，只到 11.3.x |
| 用 `Window.WindowDecorationsTheme` | 12.0.1 NuGet 没暴露 | 改用覆盖 `{x:Type WindowDrawnDecorations}` 的 ControlTheme |
| 用户内容跟标题栏不对齐 | 内容铺满 OS 客户区 | `Margin="{Binding $parent[Window].WindowDecorationMargin}"` |
| 阴影太假（青色光晕） | BoxShadow 带颜色 | 改纯黑 + 低 alpha + 双层 |
| 阴影区变 resize 光标 | `ResizeGripLayer.GripThickness = frame+shadow` | (a) 缩小 ShadowThickness；(b) 用 X11 SHAPE 裁 input region |
| macOS 自定义没生效 | `NeedsManagedDecorations` 硬编码 false | 接受现实，macOS 走原生 |
| Win32 没阴影/边框 | Win32 只 request `TitleBar` | 用无条件选择器（不带 `:has-shadow`）画 BoxShadow |
| 模板里少 PART | 按钮无响应 / 启动 NRE | 7 个 `PART_*` 按钮 + `PART_TitleBar` 一个不能少 |
| `GetVisualDescendants<WindowDrawnDecorations>()` 找不到 | 它是 `StyledElement` 不是 `Visual` | 别从可视树找，需要它的属性请用 `Window.WindowDecorationMargin` 间接读，或在调用方手动传 |
| Wayland 上崩 | 没 X11 SHAPE | 实际走 XWayland，X11 SHAPE 仍可用；`ClickThroughShadow.Attach` 已 OS gating，原生 Wayland 后端发布前无需改动（详见 §12） |
| Linux/Win32 出现双标题栏 | Window ControlTheme 自绘 + `WindowDrawnDecorations` 也画 | 用 Style 替换 Template 的双模板模式（详见 §14）|
| 用 OS 嗅探切自绘标题栏 | `#if MACOS` / `OperatingSystem.IsMacOS()` | 改用 `:csd-active` 伪类 + `WindowDecorationMargin.Top > 0` 信号（§14.1）|
| 用 `IsVisible=False` 隐藏自绘标题栏 | Border 实例残留，两份 caption 并存 | 改用 Style 替换 Template，让自绘标题栏从 visual tree 消失（§14.2）|

---

## 16. 核心源码索引

参考路径：`AtomUIV6/.referenceprojects/Avalonia/`（团队克隆）

| 文件 | 关键行 | 内容 |
|---|---|---|
| `src/Avalonia.Controls/Chrome/WindowDrawnDecorations.cs` | 28 | `class WindowDrawnDecorations : StyledElement` |
|  | 70-204 | `DefaultShadowThickness/FrameThickness/TitleBarHeight` 属性 |
|  | 295-298 | 运行时 `ShadowThickness` direct property |
|  | 444+ | `OnApplyTemplate` PART 绑定 |
|  | 622-623 | `ShadowThickness = parts.HasFlag(Shadow) ? rounded : zero` |
| `src/Avalonia.Controls/Chrome/WindowDrawnDecorationsContent.cs` | 全 | `Underlay/Overlay/FullscreenPopover` 三槽 |
| `src/Avalonia.Controls/Chrome/DrawnWindowDecorationParts.cs` | 全 | `[Flags]` enum |
| `src/Avalonia.Controls/Chrome/ResizeGripLayer.cs` | 12-91 | 8 抓手内部 Control |
| `src/Avalonia.Controls/Chrome/WindowDecorationProperties.cs` | 全 | `ElementRole` 附加属性 |
| `src/Avalonia.Controls/Window.cs` | 127-131, 360-362 | `WindowDecorationMargin` direct property |
|  | 800-836 | `WindowDecorationMargin` 计算 |
| `src/Avalonia.Controls/TopLevelHost.Decorations.cs` | 100-115 | `_decorations / _resizeGrips` 创建 |
|  | 185-201 | `UpdateResizeGripThickness` |
|  | 203-215 | 几何变化通知 Window |
| `src/Avalonia.X11/X11Window.cs` | 191 | `Handle = new PlatformHandle(_handle, "XID")` |
|  | 1547-1548 | `EnableDrawnDecorations` gating |
|  | 1640-1648 | `RequestedDrawnDecorations = TitleBar\|Border\|Shadow\|ResizeGrips` |
| `src/Avalonia.Themes.Fluent/Controls/WindowDrawnDecorations.xaml` | 全 | 默认 ControlTheme，自定义时的最佳参考 |
| `src/Avalonia.Win32/WindowImpl.cs` | 1618 | `RequestedDrawnDecorations = TitleBar` |
| `src/Avalonia.Native/WindowImpl.cs` | — | `NeedsManagedDecorations => false`（永不激活） |

`dotnet run` 即可启动。Linux 下务必确认 `xprop` 能查到 X11 SHAPE 扩展（一般默认有）。
