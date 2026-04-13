# ImagePreviewer 自定义样式指南

ImagePreviewer 是一个复合控件，由多个内部组件组成。自定义样式主要通过属性设置和 Style 覆盖实现。

---

## 1. 通过属性直接控制

### 封面尺寸

```xml
<!-- 固定宽度，高度自适应 -->
<atom:ImagePreviewer Width="300" />

<!-- 固定宽高 -->
<atom:ImageGroupPreviewer CoverWidth="150" CoverHeight="150" />
```

### 隐藏封面遮罩

```xml
<atom:ImagePreviewer IsShowCoverMask="False" />
```

### 自定义封面图

```xml
<!-- 使用不同的封面图（如模糊缩略图） -->
<atom:ImagePreviewer CoverImageSrc="avares://MyApp/Assets/thumb.png" />
```

### 自定义遮罩指示器内容

```xml
<atom:ImagePreviewer>
    <atom:ImagePreviewer.CoverIndicatorContentTemplate>
        <DataTemplate>
            <TextBlock Text="点击预览" 
                       HorizontalAlignment="Center" 
                       VerticalAlignment="Center"
                       Foreground="White" />
        </DataTemplate>
    </atom:ImagePreviewer.CoverIndicatorContentTemplate>
</atom:ImagePreviewer>
```

### 对话框配置

```xml
<!-- 模态对话框，居中显示，置顶 -->
<atom:ImagePreviewer IsDialogModal="True"
                     DialogHorizontalStartupLocation="Center"
                     DialogVerticalStartupLocation="Center"
                     IsDialogTopmost="True" />
```

### 缩放控制

```xml
<atom:ImagePreviewer ImageScaleStep="0.25"
                     ImageMinScale="0.5"
                     ImageMaxScale="10.0" />
```

### 禁用拖拽和动画

```xml
<atom:ImagePreviewer IsImageMovable="False"
                     IsMotionEnabled="False" />
```

---

## 2. 通过 Style 覆盖

### ImagePreviewer 基本选择器

```xml
<Window.Styles>
    <!-- 全局设置所有 ImagePreviewer 的封面宽度 -->
    <Style Selector="atom|ImagePreviewer">
        <Setter Property="Width" Value="250" />
    </Style>
    
    <!-- 全局设置所有 ImageGroupPreviewer 的封面尺寸 -->
    <Style Selector="atom|ImageGroupPreviewer">
        <Setter Property="CoverWidth" Value="120" />
        <Setter Property="CoverHeight" Value="120" />
    </Style>
</Window.Styles>
```

### 自定义多图组布局面板

```xml
<!-- 使用 WrapPanel 替代默认的水平 StackPanel -->
<atom:ImageGroupPreviewer>
    <atom:ImageGroupPreviewer.ItemsPanel>
        <ItemsPanelTemplate>
            <WrapPanel Orientation="Horizontal" />
        </ItemsPanelTemplate>
    </atom:ImageGroupPreviewer.ItemsPanel>
</atom:ImageGroupPreviewer>
```

---

## 3. 内部组件样式定制

> ⚠️ 以下样式涉及 internal 控件，仅在 AtomUI 程序集内部或通过全局 Style 可访问。主题修改应通过 Token 系统优先实现。

### 封面遮罩样式

封面遮罩通过 `ImagePreviewerCover` 的 `Border#Mask` 实现：

```xml
<!-- 修改遮罩背景色（通过主题 Token 修改更推荐） -->
<Style Selector="atom|ImagePreviewerCover /template/ Border#Mask">
    <Setter Property="Background" Value="rgba(0, 0, 0, 0.5)" />
</Style>
```

### 导航按钮样式

```xml
<!-- 调整导航按钮的内间距 -->
<Style Selector="atom|ImagePreviewNavButton">
    <Setter Property="Padding" Value="8" />
</Style>
```

---

## 样式选择器速查

### 公共控件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ImagePreviewer` | 匹配所有单图预览控件 |
| `atom\|ImageGroupPreviewer` | 匹配所有多图组预览控件 |

### 内部组件选择器（主题级）

| 选择器 | 说明 |
|---|---|
| `atom\|ImagePreviewerCover` | 封面展示组件 |
| `atom\|ImagePreviewerCover:pointerover` | 封面悬浮态（遮罩淡入） |
| `atom\|ImagePreviewerCover[IsShowCoverMask=True]` | 遮罩可见时显示手型光标 |
| `atom\|ImagePreviewerCover /template/ Border#Mask` | 封面遮罩 Border |
| `atom\|ImagePreviewNavButton` | 图片导航按钮（左/右） |
| `atom\|ImagePreviewNavButton:pointerover` | 导航按钮悬浮态 |
| `atom\|ImagePreviewNavButton:disabled` | 导航按钮禁用态 |
| `atom\|ImagePreviewFloatToolbar` | 浮动工具栏 |
| `atom\|ImagePreviewFloatToolbar /template/ atom\|IconButton` | 浮动工具栏内的操作按钮 |
| `atom\|ImagePreviewFloatToolbar /template/ atom\|IconButton:pointerover` | 浮动工具栏按钮悬浮态 |
| `atom\|ImagePreviewFloatToolbar /template/ atom\|IconButton:disabled` | 浮动工具栏按钮禁用态 |
| `atom\|ImagePreviewFloatToolbar /template/ Border#IndicatorFrame` | 浮动工具栏索引指示器框 |
| `atom\|ImagePreviewFloatToolbar /template/ StackPanel#RootLayout` | 浮动工具栏根布局 |
| `atom\|ImagePreviewToolbar /template/ atom\|IconButton` | 标题栏工具栏内的操作按钮 |
| `atom\|ImagePreviewToolbar /template/ atom\|IconButton:pointerover` | 标题栏工具栏按钮悬浮态 |
| `atom\|ImagePreviewToolbar /template/ atom\|IconButton:pressed` | 标题栏工具栏按钮按下态 |

### ControlTheme 文件位置

| 主题文件 | 对应控件 |
|---|---|
| `Themes/ImagePreviewerTheme.axaml` | `ImagePreviewer` |
| `Themes/ImageGroupPreviewerTheme.axaml` | `ImageGroupPreviewer` |
| `Themes/ImagePreviewerDialogTheme.axaml` | `ImagePreviewerDialog` |
| `Themes/ImageViewerTheme.axaml` | `ImageViewer` |
| `Themes/ImagePreviewerCoverTheme.axaml` | `ImagePreviewerCover` |
| `Themes/ImagePreviewNavButtonTheme.axaml` | `ImagePreviewNavButton` |
| `Themes/ImagePreviewFloatToolbarTheme.axaml` | `ImagePreviewFloatToolbar` |
| `Themes/ImagePreviewToolbarTheme.axaml` | `ImagePreviewToolbar` |
