# ScrollViewer 使用文档

本文档介绍 AtomUI ScrollViewer 控件的各种使用方式，示例代码摘自 Gallery 演示程序及实际使用场景。

> 📖 ScrollViewer 作为基础设施控件，在 Gallery 中没有独立的 ShowCase 页面，但在多个组件的 ShowCase 中被广泛使用。以下示例均来自实际项目中的用法。

---

## 前置准备

在 AXAML 中使用 ScrollViewer，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // ScrollViewer, ScrollBar, ScrollBarThumb
using AtomUI.Controls.Commons;    // AbstractScrollViewer 等基类（如需继承）
```

---

## 1. 基本用法

最基本的 ScrollViewer 用法——包裹超出可视区域的内容，自动显示垂直滚动条：

```xml
<atom:ScrollViewer Height="300"
                   VerticalScrollBarVisibility="Auto">
    <StackPanel>
        <TextBlock Text="Item 1" Margin="10" />
        <TextBlock Text="Item 2" Margin="10" />
        <TextBlock Text="Item 3" Margin="10" />
        <!-- ... 更多内容 ... -->
        <TextBlock Text="Item 50" Margin="10" />
    </StackPanel>
</atom:ScrollViewer>
```

---

## 2. 标准模式 vs 极简模式

### 标准模式（默认）

滚动条始终可见，占据独立空间，不覆盖内容区域：

```xml
<atom:ScrollViewer Height="300"
                   VerticalScrollBarVisibility="Auto"
                   AllowAutoHide="False"
                   IsLiteMode="False">
    <StackPanel>
        <!-- 内容 -->
    </StackPanel>
</atom:ScrollViewer>
```

### 极简模式

滚动条默认隐藏，鼠标进入时淡入显示，滑块初始为细线，悬浮时膨胀：

```xml
<atom:ScrollViewer Height="300"
                   VerticalScrollBarVisibility="Auto"
                   AllowAutoHide="True"
                   IsLiteMode="True">
    <StackPanel>
        <!-- 内容 -->
    </StackPanel>
</atom:ScrollViewer>
```

---

## 3. 使用附加属性

`IsLiteMode` 是附加属性，可以设置在任何内部使用 ScrollViewer 的控件上：

```xml
<!-- 在导航菜单上启用极简模式滚动条 -->
<atom:NavMenu Mode="Inline"
              Width="260"
              atom:ScrollViewer.IsLiteMode="True"
              ScrollViewer.AllowAutoHide="True">
    <atom:NavMenuNode Header="General">
        <atom:NavMenuNode Header="Button" />
        <atom:NavMenuNode Header="Typography" />
        <atom:NavMenuNode Header="Icon" />
    </atom:NavMenuNode>
    <atom:NavMenuNode Header="Layout">
        <atom:NavMenuNode Header="Divider" />
        <atom:NavMenuNode Header="Space" />
    </atom:NavMenuNode>
    <!-- 更多菜单项... -->
</atom:NavMenu>
```

> 📖 参考：`controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml`

---

## 4. 水平 + 垂直滚动

同时启用两个方向的滚动条：

```xml
<atom:ScrollViewer Height="400"
                   Width="600"
                   HorizontalScrollBarVisibility="Auto"
                   VerticalScrollBarVisibility="Auto">
    <Canvas Width="2000" Height="2000">
        <!-- 大画布内容 -->
    </Canvas>
</atom:ScrollViewer>
```

---

## 5. 在容器模板中使用

ScrollViewer 常用在 ControlTheme 模板中作为内容滚动容器。以下示例摘自 Gallery 的 ShowCasePanel 模板：

```xml
<ControlTheme x:Key="{x:Type gallery:ShowCasePanel}"
              TargetType="{x:Type gallery:ShowCasePanel}">
    <Setter Property="Template">
        <ControlTemplate TargetType="{x:Type gallery:ShowCasePanel}">
            <atom:ScrollViewer VerticalScrollBarVisibility="Auto"
                               AllowAutoHide="False"
                               BringIntoViewOnFocusChange="False"
                               IsLiteMode="False">
                <Grid Margin="5" Name="PART_MainPanel"
                      RowSpacing="15" ColumnSpacing="15">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                    </Grid.ColumnDefinitions>
                </Grid>
            </atom:ScrollViewer>
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

> 📖 参考：`controlgallery/AtomUIGallery/Controls/ShowCasePanelTheme.axaml`

---

## 6. 与 FloatButton 配合使用

ScrollViewer 为浮动按钮提供滚动容器，FloatButton 会锚定在 ScrollViewer 内容区域的固定位置：

```xml
<atom:ScrollViewer Height="300">
    <Border Background="White">
        <Panel Height="500">
            <atom:FloatButtonHost />
        </Panel>
    </Border>
</atom:ScrollViewer>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/FloatButtonShowCase.axaml`

---

## 7. 滚动条可见性控制

通过 `ScrollBarVisibility` 枚举精确控制滚动条的显示行为：

```xml
<!-- 按需显示（默认行为） — 内容超出时才显示 -->
<atom:ScrollViewer VerticalScrollBarVisibility="Auto">
    <!-- 内容 -->
</atom:ScrollViewer>

<!-- 始终显示 — 即使内容未超出也显示 -->
<atom:ScrollViewer VerticalScrollBarVisibility="Visible">
    <!-- 内容 -->
</atom:ScrollViewer>

<!-- 隐藏但可滚动 — 不显示滚动条但仍可通过滚轮/触控滚动 -->
<atom:ScrollViewer VerticalScrollBarVisibility="Hidden">
    <!-- 内容 -->
</atom:ScrollViewer>

<!-- 完全禁用 — 不显示且不可滚动 -->
<atom:ScrollViewer VerticalScrollBarVisibility="Disabled">
    <!-- 内容 -->
</atom:ScrollViewer>
```

---

## 8. 控制动画行为

```xml
<!-- 禁用过渡动画（滚动条显隐不再有渐变过渡效果） -->
<atom:ScrollViewer IsMotionEnabled="False"
                   VerticalScrollBarVisibility="Auto">
    <StackPanel>
        <!-- 内容 -->
    </StackPanel>
</atom:ScrollViewer>
```

---

## 9. 程序化滚动控制

通过 C# 代码控制 ScrollViewer 的滚动位置：

```csharp
// 获取 ScrollViewer 实例（例如通过 Name 引用）
var scrollViewer = this.FindControl<ScrollViewer>("MyScrollViewer");

// 滚动到指定偏移量
scrollViewer.Offset = new Vector(0, 100); // 水平偏移 0，垂直偏移 100

// 滚动到顶部
scrollViewer.Offset = new Vector(scrollViewer.Offset.X, 0);

// 滚动到底部
scrollViewer.Offset = new Vector(
    scrollViewer.Offset.X,
    scrollViewer.Extent.Height - scrollViewer.Viewport.Height);
```

```xml
<atom:ScrollViewer Name="MyScrollViewer"
                   Height="300"
                   VerticalScrollBarVisibility="Auto">
    <StackPanel>
        <!-- 内容 -->
    </StackPanel>
</atom:ScrollViewer>
```

---

## 常见组合模式

### 侧边栏导航

```xml
<Border Width="250" Height="600">
    <atom:NavMenu Mode="Inline"
                  atom:ScrollViewer.IsLiteMode="True"
                  ScrollViewer.AllowAutoHide="True">
        <!-- 导航菜单项 -->
    </atom:NavMenu>
</Border>
```

### 长表单

```xml
<atom:ScrollViewer VerticalScrollBarVisibility="Auto"
                   AllowAutoHide="False"
                   IsLiteMode="False">
    <atom:Form>
        <!-- 大量表单项 -->
    </atom:Form>
</atom:ScrollViewer>
```

### 可滚动内容面板（常驻滚动条）

```xml
<atom:ScrollViewer VerticalScrollBarVisibility="Auto"
                   AllowAutoHide="False"
                   BringIntoViewOnFocusChange="False">
    <StackPanel Margin="16">
        <!-- 文档内容 -->
    </StackPanel>
</atom:ScrollViewer>
```

### 图片/画布查看器（双向滚动）

```xml
<atom:ScrollViewer HorizontalScrollBarVisibility="Auto"
                   VerticalScrollBarVisibility="Auto"
                   AllowAutoHide="True"
                   IsLiteMode="True">
    <Image Source="/Assets/large-image.png" />
</atom:ScrollViewer>
```
