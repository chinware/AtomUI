# Separator 使用文档

本文档介绍 AtomUI Separator 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/General/SeparatorShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Separator，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Separator, VerticalSeparator
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本水平分割线

最基本的用法——在段落之间添加分隔线：

```xml
<StackPanel Orientation="Vertical">
    <atom:TextBlock TextWrapping="Wrap">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed nonne merninisti licere mihi ista probare, quae sunt a te dicta? Refert tamen, quo modo.
    </atom:TextBlock>
    <atom:Separator />
    <atom:TextBlock TextWrapping="Wrap">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed nonne merninisti licere mihi ista probare, quae sunt a te dicta? Refert tamen, quo modo.
    </atom:TextBlock>
    <atom:Separator />
    <atom:TextBlock TextWrapping="Wrap">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed nonne merninisti licere mihi ista probare, quae sunt a te dicta? Refert tamen, quo modo.
    </atom:TextBlock>
</StackPanel>
```

**使用场景**：内容段落之间的视觉分隔，如文章区域划分、列表分组。

---

## 2. 带标题的分割线

通过 `Title` 属性在线条中嵌入标题文本，通过 `TitlePosition` 控制标题位置：

### 居中标题（默认）

```xml
<atom:Separator Title="Text" />
```

### 左对齐标题

```xml
<atom:Separator Title="Left text" TitlePosition="Left" FontWeight="Bold" />
```

### 右对齐标题

```xml
<atom:Separator Title="Right text" TitlePosition="Right" FontStyle="Oblique" />
```

### 自定义标题边距

通过 `OrientationMargin` 属性精确控制标题距边缘的像素距离：

```xml
<!-- 标题紧贴左边缘（0 像素边距） -->
<atom:Separator Title="Left Text with 0 orientationMargin"
                 TitlePosition="Left"
                 FontStyle="Oblique"
                 FontWeight="Medium"
                 OrientationMargin="0" />

<!-- 标题距右边缘 50 像素 -->
<atom:Separator Title="Right Text with 50px orientationMargin"
                 TitlePosition="Right"
                 TitleColor="Coral"
                 FontWeight="Medium"
                 OrientationMargin="50" />
```

**使用场景**：用带标题的分割线为内容分组命名，例如表单中的"基本信息"、"高级设置"分区。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/SeparatorShowCase.axaml` 中 "Divider with title" 示例。

---

## 3. 普通文字模式

通过 `IsPlain="True"` 使标题使用正常字号和字重（而非默认的大号加粗标题样式）：

```xml
<StackPanel Orientation="Vertical">
    <atom:TextBlock TextWrapping="Wrap">Content above...</atom:TextBlock>
    <atom:Separator Title="Text" IsPlain="True" />
    <atom:TextBlock TextWrapping="Wrap">Content below...</atom:TextBlock>
    <atom:Separator Title="Left Text" TitlePosition="Left" IsPlain="True" />
    <atom:TextBlock TextWrapping="Wrap">Content below...</atom:TextBlock>
    <atom:Separator Title="Right Text" TitlePosition="Right" IsPlain="True" />
    <atom:TextBlock TextWrapping="Wrap">Content below...</atom:TextBlock>
</StackPanel>
```

**对比效果**：

| 模式 | 字号 | 字重 | 视觉效果 |
|---|---|---|---|
| 默认标题（`IsPlain=False`） | `FontSizeLG`（16px） | 500（Medium） | 醒目的区域标题 |
| 普通文字（`IsPlain=True`） | `FontSize`（14px） | Normal | 轻量的内联注释 |

**使用场景**：当分割线标题只是辅助说明而非区域标题时，使用普通文字模式降低视觉权重。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/SeparatorShowCase.axaml` 中 "Text without heading style" 示例。

---

## 4. 间距尺寸

通过 `SizeType` 控制水平分割线的垂直间距（上下 Margin），适用于不同密度的布局：

```xml
<StackPanel Orientation="Vertical">
    <atom:TextBlock TextWrapping="Wrap">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit...
    </atom:TextBlock>
    <atom:Separator SizeType="Small" />
    <atom:TextBlock TextWrapping="Wrap">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit...
    </atom:TextBlock>
    <atom:Separator SizeType="Middle" />
    <atom:TextBlock TextWrapping="Wrap">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit...
    </atom:TextBlock>
    <atom:Separator SizeType="Large" />
    <atom:TextBlock TextWrapping="Wrap">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit...
    </atom:TextBlock>
</StackPanel>
```

| 尺寸 | 间距效果 | 适用场景 |
|---|---|---|
| `Small` | 紧凑间距 | 列表内分隔、密集布局 |
| `Middle`（默认） | 标准间距 | 通用段落分隔 |
| `Large` | 宽松间距 | 大区域划分、内容模块分隔 |

尺寸支持数据绑定：

```xml
<atom:Separator SizeType="{Binding SeparatorSizeType}" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/SeparatorShowCase.axaml` 中 "Set the spacing size of the divider" 示例。

---

## 5. 垂直分割线

使用 `VerticalSeparator` 或手动设置 `Orientation="Vertical"` 创建垂直分割线，适用于水平布局中的行内分隔：

```xml
<StackPanel Orientation="Horizontal">
    <atom:TextBlock>Item1</atom:TextBlock>
    <atom:VerticalSeparator />
    <atom:TextBlock>Item2</atom:TextBlock>
    <atom:VerticalSeparator />
    <atom:TextBlock>Item3</atom:TextBlock>
</StackPanel>
```

两种写法等价：

```xml
<!-- 使用便捷类 -->
<atom:VerticalSeparator />

<!-- 手动设置方向 -->
<atom:Separator Orientation="Vertical" />
```

**注意事项**：
- 垂直分割线不支持标题（`Title` 在垂直模式下被忽略，标题 TextBlock 强制隐藏）
- 垂直分割线的高度默认为 1em，但会自动撑满可用高度
- 垂直分割线的宽度 = `LineWidth + VerticalMarginInline`

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/SeparatorShowCase.axaml` 中 "Vertical" 示例。

---

## 6. 线条变体

通过 `Variant` 属性设置线条样式——实线、虚线或点线：

```xml
<StackPanel Orientation="Vertical">
    <atom:TextBlock TextWrapping="Wrap">Content...</atom:TextBlock>
    <atom:Separator Title="Solid" LineColor="#7cb305" />
    <atom:TextBlock TextWrapping="Wrap">Content...</atom:TextBlock>
    <atom:Separator Title="Dotted" LineColor="#7cb305" Variant="Dotted" />
    <atom:TextBlock TextWrapping="Wrap">Content...</atom:TextBlock>
    <atom:Separator Title="Dashed" LineColor="#7cb305" Variant="Dashed" />
    <atom:TextBlock TextWrapping="Wrap">Content...</atom:TextBlock>
</StackPanel>
```

| 变体 | 视觉效果 | 语义 |
|---|---|---|
| `Solid`（默认） | 连续实线 ── | 最强的视觉分隔 |
| `Dashed` | 虚线 - - - | 中等强度分隔 |
| `Dotted` | 点线 · · · | 最轻量的分隔提示 |

**使用场景**：
- `Solid`：通用分隔，默认选择
- `Dashed`：辅助性分隔，如表单内字段组之间
- `Dotted`：最轻量的提示性分隔

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/SeparatorShowCase.axaml` 中 "Variant" 示例。

---

## 7. 自定义颜色

通过 `LineColor` 和 `TitleColor` 属性自定义分割线和标题颜色：

### 自定义线条颜色

```xml
<atom:Separator Title="Green Line" LineColor="#7cb305" />
```

### 自定义标题颜色

```xml
<atom:Separator Title="Coral Title" TitleColor="Coral" />
```

### 同时自定义两种颜色

```xml
<atom:Separator Title="Right Text with 50px orientationMargin"
                 TitlePosition="Right"
                 TitleColor="Coral"
                 FontWeight="Medium"
                 OrientationMargin="50" />
```

---

## 8. 自定义字体样式

Separator 的标题文本支持所有标准字体属性：

```xml
<!-- 斜体标题 -->
<atom:Separator Title="Italic Text" FontStyle="Italic" />

<!-- 粗体标题 -->
<atom:Separator Title="Bold Left" TitlePosition="Left" FontWeight="Bold" />

<!-- 斜体 + 中等粗细 -->
<atom:Separator Title="Oblique Right" TitlePosition="Right" FontStyle="Oblique" />
```

---

## 常见组合模式

### 表单内容分组

使用带标题的分割线为表单划分区域：

```xml
<StackPanel Orientation="Vertical" Spacing="8">
    <atom:Separator Title="Basic Information" TitlePosition="Left" />
    <!-- 姓名、邮箱等基本信息字段 -->
    
    <atom:Separator Title="Advanced Settings" TitlePosition="Left" />
    <!-- 高级配置字段 -->
    
    <atom:Separator Title="Permissions" TitlePosition="Left" IsPlain="True" />
    <!-- 权限相关字段 -->
</StackPanel>
```

### 操作链接分隔

使用垂直分割线分隔行内操作链接：

```xml
<StackPanel Orientation="Horizontal">
    <atom:HyperLinkButton>Edit</atom:HyperLinkButton>
    <atom:VerticalSeparator />
    <atom:HyperLinkButton IsDanger="True">Delete</atom:HyperLinkButton>
    <atom:VerticalSeparator />
    <atom:HyperLinkButton>More</atom:HyperLinkButton>
</StackPanel>
```

### 文章段落分隔

在长文内容中使用不同样式的分割线区分段落层级：

```xml
<StackPanel Orientation="Vertical">
    <!-- 主要区域使用带标题实线 -->
    <atom:Separator Title="Chapter 1" />
    <atom:TextBlock TextWrapping="Wrap">Chapter content...</atom:TextBlock>
    
    <!-- 次级分隔使用小号无标题 -->
    <atom:Separator SizeType="Small" />
    <atom:TextBlock TextWrapping="Wrap">Sub-section content...</atom:TextBlock>
    
    <!-- 最轻量分隔使用点线 -->
    <atom:Separator Variant="Dotted" SizeType="Small" />
    <atom:TextBlock TextWrapping="Wrap">Minor content...</atom:TextBlock>
</StackPanel>
```

### 信息卡片分隔

在卡片内部使用分割线分隔不同信息区域：

```xml
<Border CornerRadius="8" BorderBrush="#d9d9d9" BorderThickness="1" Padding="16">
    <StackPanel Orientation="Vertical">
        <atom:TextBlock FontWeight="Bold" FontSize="16">Card Title</atom:TextBlock>
        <atom:Separator SizeType="Small" />
        <atom:TextBlock TextWrapping="Wrap">Card body content...</atom:TextBlock>
        <atom:Separator Variant="Dashed" SizeType="Small" />
        <StackPanel Orientation="Horizontal">
            <atom:HyperLinkButton>Action 1</atom:HyperLinkButton>
            <atom:VerticalSeparator />
            <atom:HyperLinkButton>Action 2</atom:HyperLinkButton>
        </StackPanel>
    </StackPanel>
</Border>
```

### 颜色主题分割线

使用自定义颜色创建与业务主题匹配的分割线：

```xml
<!-- 成功主题 -->
<atom:Separator Title="Completed" LineColor="#52c41a" TitleColor="#52c41a" />

<!-- 警告主题 -->
<atom:Separator Title="Attention" LineColor="#faad14" TitleColor="#faad14" Variant="Dashed" />

<!-- 错误主题 -->
<atom:Separator Title="Error Section" LineColor="#ff4d4f" TitleColor="#ff4d4f" />
```
