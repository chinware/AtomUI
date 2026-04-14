# Window 自定义样式指南

Window 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Window 的公共属性来控制外观：

```xml
<!-- 基本窗口 -->
<atom:Window Title="我的应用"
             WindowStartupLocation="CenterScreen"
             Width="1200" Height="800">
    <!-- 内容 -->
</atom:Window>

<!-- 带 Logo 的窗口 -->
<atom:Window Title="我的应用">
    <atom:Window.Logo>
        <Image Source="/Assets/app-logo.png" />
    </atom:Window.Logo>
    <!-- 内容 -->
</atom:Window>

<!-- 隐藏标题栏 -->
<atom:Window IsTitleBarVisible="False">
    <!-- 无标题栏的自定义窗口 -->
</atom:Window>

<!-- 配置标题按钮 -->
<atom:Window IsPinCaptionButtonEnabled="True"
             IsFullScreenCaptionButtonEnabled="True">
    <!-- 显示置顶和全屏按钮 -->
</atom:Window>

<!-- 限制窗口最大尺寸为屏幕的 80% -->
<atom:Window MaxWidthScreenRatio="0.8"
             MaxHeightScreenRatio="0.8">
    <!-- 内容 -->
</atom:Window>
```

> 📖 Gallery 主窗口使用范例：`controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Window 进行全局或局部样式覆盖：

### 自定义窗口背景色

```xml
<atom:Window.Styles>
    <Style Selector="atom|Window">
        <Setter Property="Background" Value="#1a1a2e" />
        <Setter Property="Foreground" Value="#e0e0e0" />
    </Style>
</atom:Window.Styles>
```

### 自定义内容区域背景

```xml
<atom:Window ContentFrameBackground="#f5f5f5">
    <!-- 内容区域使用浅灰色背景 -->
</atom:Window>
```

### 自定义标题栏背景

```xml
<atom:Window TitleBarFrameBackground="LightBlue">
    <!-- 标题栏使用蓝色背景 -->
</atom:Window>
```

### 平台差异化样式

```xml
<atom:Window.Styles>
    <!-- 仅在 Linux 下添加额外内间距 -->
    <Style Selector="atom|Window[OsType=Linux]">
        <Setter Property="Padding" Value="4" />
    </Style>
    
    <!-- 仅在 macOS 下调整标题字号 -->
    <Style Selector="atom|Window[OsType=macOS]">
        <Setter Property="TitleFontSize" Value="14" />
    </Style>
</atom:Window.Styles>
```

---

## 3. 使用框架层实现高级视觉效果

### 窗口级渐变背景

```xml
<atom:Window>
    <atom:Window.WindowFrameLayer>
        <Border>
            <Border.Background>
                <LinearGradientBrush StartPoint="0%,0%" EndPoint="100%,100%">
                    <GradientStop Color="#667eea" Offset="0" />
                    <GradientStop Color="#764ba2" Offset="1" />
                </LinearGradientBrush>
            </Border.Background>
        </Border>
    </atom:Window.WindowFrameLayer>
    <!-- 内容 -->
</atom:Window>
```

### 带不透明度的内容区背景图

```xml
<atom:Window ContentFrameLayerOpacity="0.3">
    <atom:Window.ContentFrameLayer>
        <Image Source="/Assets/bg-pattern.png" Stretch="UniformToFill" />
    </atom:Window.ContentFrameLayer>
    <!-- 内容 -->
</atom:Window>
```

### 使用模板定义框架层内容

```xml
<atom:Window>
    <atom:Window.WindowFrameLayerTemplate>
        <DataTemplate>
            <Border>
                <Border.Background>
                    <RadialGradientBrush>
                        <GradientStop Color="#1677ff" Offset="0" />
                        <GradientStop Color="Transparent" Offset="1" />
                    </RadialGradientBrush>
                </Border.Background>
            </Border>
        </DataTemplate>
    </atom:Window.WindowFrameLayerTemplate>
    <!-- 内容 -->
</atom:Window>
```

---

## 4. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Window 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomWindow" TargetType="atom:Window">
    <Setter Property="Template">
        <ControlTemplate TargetType="atom:Window">
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}">
                <DockPanel>
                    <!-- 自定义标题栏区域 -->
                    <ContentPresenter DockPanel.Dock="Top"
                                      Content="{TemplateBinding TitleBar}" />
                    <!-- 内容区域 -->
                    <ContentPresenter Content="{TemplateBinding Content}" />
                </DockPanel>
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:Window Theme="{StaticResource MyCustomWindow}">
    <!-- 内容 -->
</atom:Window>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的媒体查询、对话框层管理、窗口缩放器等功能。建议优先使用 Style 覆盖和框架层实现自定义效果。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Window` 语法引用 `atom` XML 命名空间下的 `Window` 类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|Window` | 匹配所有 AtomUI Window 实例 |
| `atom\|WindowTitleBar` | 匹配窗口标题栏控件 |

### 按操作系统选择

| 选择器 | 说明 |
|---|---|
| `atom\|Window[OsType=Windows]` | 匹配 Windows 平台的窗口 |
| `atom\|Window[OsType=Linux]` | 匹配 Linux 平台的窗口 |
| `atom\|Window[OsType=macOS]` | 匹配 macOS 平台的窗口 |

### 按窗口状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|Window[WindowState=Normal]` | 匹配正常状态的窗口 |
| `atom\|Window[WindowState=Maximized]` | 匹配最大化状态的窗口 |
| `atom\|Window[WindowState=FullScreen]` | 匹配全屏状态的窗口 |
| `atom\|Window[WindowState=Minimized]` | 匹配最小化状态的窗口 |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|Window[IsTitleBarVisible=False]` | 匹配隐藏了标题栏的窗口 |
| `atom\|Window[Topmost=True]` | 匹配置顶的窗口 |
| `atom\|Window[CanResize=False]` | 匹配不可调整大小的窗口 |

### 模板内部件选择

| 选择器 | 说明 |
|---|---|
| `atom\|Window /template/ Border#WindowFrame` | 窗口主框架 Border |
| `atom\|Window /template/ ContentPresenter#WindowFrameLayer` | 窗口级框架背景层 |
| `atom\|Window /template/ ContentPresenter#TitleBarFrameLayer` | 标题栏框架背景层 |
| `atom\|Window /template/ ContentPresenter#ContentFrameLayer` | 内容区框架背景层 |
| `atom\|Window /template/ Border#ContentFrame` | 内容框架（Container Query 容器） |
| `atom\|Window /template/ DockPanel#PART_ContentLayout` | 内容布局容器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Window[OsType=Linux][WindowState=Normal]` | Linux 平台正常状态的窗口（有圆角和边框） |
| `atom\|Window[WindowState=FullScreen], atom\|Window[WindowState=Maximized]` | 全屏或最大化时（清除圆角和边框） |
| `atom\|Window:not([OsType=Linux])` | 非 Linux 平台的窗口（无自定义圆角） |
