# Splitter 使用文档

本文档介绍 AtomUI Splitter 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Layout/SplitterShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Splitter，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Splitter 控件
using AtomUI;                     // Dimension 类型
```

---

## 1. 基本分割

最简单的用法——在 Splitter 中放入两个子面板，自动生成拖拽手柄：

```xml
<atom:Splitter Orientation="Vertical" Height="220">
    <Border atom:Splitter.Size="30%">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">First</TextBlock>
    </Border>
    <Border atom:Splitter.DefaultSize="100" atom:Splitter.MinSize="60">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Second</TextBlock>
    </Border>
</atom:Splitter>
```

**要点说明**：
- `Orientation="Vertical"` 表示面板水平排列（左右分割），手柄为竖线
- 第一个面板使用 `Size="30%"` 百分比尺寸，拖拽过程中实时更新
- 第二个面板使用 `DefaultSize="100"` 像素初始尺寸 + `MinSize="60"` 最小约束

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Layout/SplitterShowCase.axaml` 中 "Basic" 示例。

---

## 2. 水平分割

设置 `Orientation="Horizontal"` 实现上下分割：

```xml
<atom:Splitter Orientation="Horizontal" Height="220">
    <Border atom:Splitter.Size="40%">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Top</TextBlock>
    </Border>
    <Border>
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Bottom</TextBlock>
    </Border>
</atom:Splitter>
```

**方向对照表**：

| Orientation | 面板排列方向 | 手柄方向 | 拖拽方向 |
|---|---|---|---|
| `Vertical` | 左右排列 | 竖线 | 水平拖拽 |
| `Horizontal` | 上下排列 | 横线 | 垂直拖拽 |

> 📖 参考：Gallery 中 "Horizontal" 示例。

---

## 3. 嵌套分割

水平与垂直 Splitter 可以嵌套，构建复杂的多区域 IDE 风格布局：

```xml
<atom:Splitter Orientation="Vertical" Height="260">
    <Border atom:Splitter.Size="40%">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Left</TextBlock>
    </Border>
    <atom:Splitter Orientation="Horizontal">
        <Border>
            <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Top</TextBlock>
        </Border>
        <Border>
            <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Bottom</TextBlock>
        </Border>
    </atom:Splitter>
</atom:Splitter>
```

**效果**：左侧为独立面板，右侧区域被二次水平分割为上下两部分。

> 📖 参考：Gallery 中 "Composite" 示例。

---

## 4. 多面板分割

Splitter 支持任意数量的子面板，系统自动在每对相邻面板间插入手柄：

```xml
<atom:Splitter Orientation="Vertical" Height="240">
    <Border atom:Splitter.Size="20%">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">A</TextBlock>
    </Border>
    <Border atom:Splitter.Size="20%">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">B</TextBlock>
    </Border>
    <Border>
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">C</TextBlock>
    </Border>
    <Border>
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">D</TextBlock>
    </Border>
</atom:Splitter>
```

**尺寸分配规则**：
- A 和 B 分别占可用空间的 20%
- C 和 D 未指定尺寸，平分剩余 60% 空间

> 📖 参考：Gallery 中 "Multi Panels" 示例。

---

## 5. 禁用拖拽

通过 `Splitter.IsResizable="False"` 禁止特定面板参与拖拽：

```xml
<atom:Splitter Orientation="Vertical" Height="220">
    <Border atom:Splitter.Size="35%">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Resizable</TextBlock>
    </Border>
    <Border atom:Splitter.DefaultSize="120" atom:Splitter.IsResizable="False">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Not Resizable</TextBlock>
    </Border>
    <Border atom:Splitter.DefaultSize="120">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Resizable</TextBlock>
    </Border>
</atom:Splitter>
```

**行为说明**：
- 中间面板设置了 `IsResizable="False"`
- 左侧手柄（A|B 之间）不可拖拽，因为 B 不可调整大小
- 右侧手柄（B|C 之间）不可拖拽，因为 B 不可调整大小
- 拖拽光标不会出现在这两个手柄上

> 📖 参考：Gallery 中 "Resizable Disabled" 示例。

---

## 6. 面板折叠

通过 `Splitter.Collapsible` 配置面板可折叠。AXAML 中支持字符串简写：

```xml
<atom:Splitter Orientation="Vertical" Height="220">
    <!-- "Always" = 始终显示折叠图标 -->
    <Border atom:Splitter.DefaultSize="33%" atom:Splitter.Collapsible="Always">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">First</TextBlock>
    </Border>
    <!-- "Hover" = 悬浮时显示折叠图标 -->
    <Border atom:Splitter.DefaultSize="34%" atom:Splitter.Collapsible="Hover">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Second</TextBlock>
    </Border>
    <!-- "True" = Always 的别名 -->
    <Border atom:Splitter.Collapsible="True">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Third</TextBlock>
    </Border>
</atom:Splitter>
```

### 在代码中动态切换折叠图标显示模式

```csharp
// 设置面板可折叠，折叠图标始终显示
Splitter.SetCollapsible(panel, new SplitterPanelCollapsible
{
    IsEnabled           = true,
    ShowCollapsibleIcon = SplitterCollapsibleIconDisplayMode.Always
});

// 切换为悬浮时显示
Splitter.SetCollapsible(panel, new SplitterPanelCollapsible
{
    IsEnabled           = true,
    ShowCollapsibleIcon = SplitterCollapsibleIconDisplayMode.Hover
});

// 隐藏折叠图标但保留折叠功能
Splitter.SetCollapsible(panel, new SplitterPanelCollapsible
{
    IsEnabled           = true,
    ShowCollapsibleIcon = SplitterCollapsibleIconDisplayMode.Hidden
});
```

> 📖 参考：Gallery 中 "ShowCollapsibleIcon" 示例及其 code-behind `SplitterShowCase.axaml.cs`。

---

## 7. 编程式折叠/展开

通过 `Splitter.IsCollapsed` 附加属性编程式控制面板状态：

```xml
<!-- AXAML：绑定 ViewModel 属性 -->
<atom:Splitter Orientation="Vertical" Height="220">
    <Border x:Name="LeftPanel"
            atom:Splitter.Size="30%"
            atom:Splitter.Collapsible="Always"
            atom:Splitter.IsCollapsed="{Binding IsLeftCollapsed}">
        <TextBlock>Left</TextBlock>
    </Border>
    <Border>
        <TextBlock>Right</TextBlock>
    </Border>
</atom:Splitter>
```

```csharp
// C# Code-behind 或 ViewModel
// 折叠面板
Splitter.SetIsCollapsed(LeftPanel, true);

// 展开面板
Splitter.SetIsCollapsed(LeftPanel, false);
```

---

## 8. 延迟渲染模式

当面板内容渲染开销较大时，使用 `IsLazy="True"` 提升拖拽流畅度：

```xml
<atom:Splitter Orientation="Vertical" Height="140" IsLazy="True">
    <Border atom:Splitter.Size="50%">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">First</TextBlock>
    </Border>
    <Border>
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Second</TextBlock>
    </Border>
</atom:Splitter>

<atom:Splitter Orientation="Horizontal" Height="140" IsLazy="True">
    <Border atom:Splitter.Size="50%">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">First</TextBlock>
    </Border>
    <Border>
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Second</TextBlock>
    </Border>
</atom:Splitter>
```

**行为差异**：
- `IsLazy="False"`（默认）：拖拽时面板尺寸实时更新
- `IsLazy="True"`：拖拽时仅移动手柄视觉位置（`RenderTransform`），释放鼠标后一次性更新所有面板尺寸

> 📖 参考：Gallery 中 "Lazy" 示例。

---

## 9. 自定义折叠图标

通过 `CollapsePreviousIcon` 和 `CollapseNextIcon` 替换默认方向箭头：

```xml
<atom:Splitter Orientation="Vertical" Height="220"
               CollapsePreviousIcon="{antdicons:AntDesignIconProvider Kind=LeftOutlined}"
               CollapseNextIcon="{antdicons:AntDesignIconProvider Kind=RightOutlined}">
    <Border atom:Splitter.Collapsible="Always">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Left</TextBlock>
    </Border>
    <Border atom:Splitter.Collapsible="Always">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">Right</TextBlock>
    </Border>
</atom:Splitter>
```

**默认图标**（未设置自定义图标时）：

| 方向 | Previous 图标 | Next 图标 |
|---|---|---|
| `Vertical`（左右分割） | `LeftOutlined` ← | `RightOutlined` → |
| `Horizontal`（上下分割） | `UpOutlined` ↑ | `DownOutlined` ↓ |

---

## 10. 监听拖拽事件

通过 `ResizeStarted`、`ResizeDelta`、`ResizeCompleted` 事件监听面板尺寸变化：

```xml
<atom:Splitter Orientation="Vertical" Height="220"
               ResizeStarted="HandleResizeStarted"
               ResizeDelta="HandleResizeDelta"
               ResizeCompleted="HandleResizeCompleted">
    <Border atom:Splitter.Size="50%"><TextBlock>A</TextBlock></Border>
    <Border><TextBlock>B</TextBlock></Border>
</atom:Splitter>
```

```csharp
private void HandleResizeStarted(object? sender, SplitterResizeEventArgs e)
{
    Console.WriteLine($"开始拖拽手柄 #{e.HandleIndex}");
    Console.WriteLine($"初始尺寸: [{string.Join(", ", e.Sizes.Select(s => $"{s:F1}px"))}]");
}

private void HandleResizeDelta(object? sender, SplitterResizeEventArgs e)
{
    // 拖拽过程中持续触发，可用于实时显示面板尺寸
    Console.WriteLine($"当前尺寸: [{string.Join(", ", e.Sizes.Select(s => $"{s:F1}px"))}]");
}

private void HandleResizeCompleted(object? sender, SplitterResizeEventArgs e)
{
    Console.WriteLine($"拖拽完成，最终尺寸: [{string.Join(", ", e.Sizes.Select(s => $"{s:F1}px"))}]");
}
```

---

## 11. 尺寸约束

通过 `MinSize` 和 `MaxSize` 限制面板的可调范围：

```xml
<atom:Splitter Orientation="Vertical" Height="220">
    <!-- 最小 100px，最大 50% -->
    <Border atom:Splitter.Size="30%"
            atom:Splitter.MinSize="100"
            atom:Splitter.MaxSize="50%">
        <TextBlock>Constrained</TextBlock>
    </Border>
    <Border>
        <TextBlock>Flexible</TextBlock>
    </Border>
</atom:Splitter>
```

**支持的尺寸格式**：
- 像素：`"100"`、`"200px"`
- 百分比：`"30%"`、`"50%"`
- 百分比和像素可在同一 Splitter 中混合使用

---

## 常见组合模式

### IDE 三栏布局

```xml
<atom:Splitter Orientation="Vertical" Height="400">
    <!-- 文件树 -->
    <Border atom:Splitter.Size="20%"
            atom:Splitter.MinSize="150"
            atom:Splitter.Collapsible="Always">
        <TextBlock>Explorer</TextBlock>
    </Border>
    <!-- 编辑器 + 终端 -->
    <atom:Splitter Orientation="Horizontal">
        <Border atom:Splitter.Size="70%">
            <TextBlock>Editor</TextBlock>
        </Border>
        <Border atom:Splitter.Collapsible="Hover">
            <TextBlock>Terminal</TextBlock>
        </Border>
    </atom:Splitter>
    <!-- 属性面板 -->
    <Border atom:Splitter.Size="20%"
            atom:Splitter.MinSize="150"
            atom:Splitter.Collapsible="Always">
        <TextBlock>Properties</TextBlock>
    </Border>
</atom:Splitter>
```

### 仪表盘布局

```xml
<atom:Splitter Orientation="Horizontal" Height="600">
    <atom:Splitter Orientation="Vertical" atom:Splitter.Size="60%">
        <Border atom:Splitter.Size="50%"><TextBlock>Chart A</TextBlock></Border>
        <Border><TextBlock>Chart B</TextBlock></Border>
    </atom:Splitter>
    <atom:Splitter Orientation="Vertical">
        <Border atom:Splitter.Size="33%"><TextBlock>Stats</TextBlock></Border>
        <Border atom:Splitter.Size="33%"><TextBlock>Logs</TextBlock></Border>
        <Border><TextBlock>Alerts</TextBlock></Border>
    </atom:Splitter>
</atom:Splitter>
```

### 固定侧边栏 + 可折叠面板

```xml
<atom:Splitter Orientation="Vertical" Height="400">
    <!-- 固定导航栏（不可拖拽） -->
    <Border atom:Splitter.DefaultSize="60"
            atom:Splitter.IsResizable="False">
        <TextBlock>Nav</TextBlock>
    </Border>
    <!-- 可折叠侧边栏 -->
    <Border atom:Splitter.DefaultSize="200"
            atom:Splitter.MinSize="150"
            atom:Splitter.Collapsible="Always">
        <TextBlock>Sidebar</TextBlock>
    </Border>
    <!-- 主内容区（弹性） -->
    <Border>
        <TextBlock>Main Content</TextBlock>
    </Border>
</atom:Splitter>
```
