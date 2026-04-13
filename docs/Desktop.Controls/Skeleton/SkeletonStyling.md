# Skeleton 自定义样式指南

Skeleton 系列控件的视觉表现通过 `ControlTheme` + Design Token 系统控制。由于 Skeleton 是一个控件族（包含多个子控件），每个子控件有独立的 ControlTheme，但共享同一套 `SkeletonToken`。

---

## 1. 使用属性直接控制

### 组合骨架屏

```xml
<!-- 基本骨架屏（标题 + 段落） -->
<atom:Skeleton IsLoading="True" />

<!-- 带头像的骨架屏 -->
<atom:Skeleton IsShowAvatar="True" IsLoading="True" />

<!-- 自定义段落行数 -->
<atom:Skeleton IsShowAvatar="True" ParagraphRows="4" IsLoading="True" />

<!-- 启用流光动画 -->
<atom:Skeleton IsActive="True" IsLoading="True" />

<!-- 胶囊形圆角 -->
<atom:Skeleton IsRound="True" IsLoading="True" />

<!-- 自定义标题宽度 -->
<atom:Skeleton TitleWidth="80%" IsLoading="True" />

<!-- 包裹真实内容 -->
<atom:Skeleton IsLoading="{Binding IsDataLoading}">
    <TextBlock>真实内容</TextBlock>
</atom:Skeleton>
```

### 独立元素骨架屏

```xml
<!-- 按钮骨架（不同形状） -->
<atom:SkeletonButton IsActive="True" Shape="Square" />
<atom:SkeletonButton IsActive="True" Shape="Round" />
<atom:SkeletonButton IsActive="True" Shape="Circle" />

<!-- 按钮骨架（不同尺寸） -->
<atom:SkeletonButton SizeType="Large" IsActive="True" />
<atom:SkeletonButton SizeType="Middle" IsActive="True" />
<atom:SkeletonButton SizeType="Small" IsActive="True" />

<!-- 按钮骨架（Block 模式） -->
<atom:SkeletonButton IsBlock="True" IsActive="True" />

<!-- 头像骨架 -->
<atom:SkeletonAvatar IsActive="True" Shape="Circle" />
<atom:SkeletonAvatar IsActive="True" Shape="Square" SizeType="Large" />

<!-- 输入框骨架 -->
<atom:SkeletonInput IsActive="True" />
<atom:SkeletonInput IsActive="True" IsBlock="True" />

<!-- 图片骨架 -->
<atom:SkeletonImage IsActive="True" />

<!-- 自定义节点骨架 -->
<atom:SkeletonNode IsActive="True">
    <antdicons:DotChartOutlined StrokeBrush="#bfbfbf" Width="40" Height="40" />
</atom:SkeletonNode>

<!-- 自定义节点（自定义宽度） -->
<atom:SkeletonNode IsActive="True" Width="160" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SkeletonShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

### 全局启用流光动画

```xml
<Window.Styles>
    <Style Selector="atom|Skeleton">
        <Setter Property="IsActive" Value="True" />
    </Style>
</Window.Styles>
```

### 自定义独立元素的默认尺寸

```xml
<Window.Styles>
    <Style Selector="atom|SkeletonButton">
        <Setter Property="SizeType" Value="Large" />
    </Style>
    <Style Selector="atom|SkeletonInput">
        <Setter Property="SizeType" Value="Large" />
    </Style>
</Window.Styles>
```

### 统一按钮骨架为胶囊形

```xml
<Window.Styles>
    <Style Selector="atom|SkeletonButton">
        <Setter Property="Shape" Value="Round" />
    </Style>
</Window.Styles>
```

### 自定义头像骨架为方形

```xml
<Window.Styles>
    <Style Selector="atom|SkeletonAvatar">
        <Setter Property="Shape" Value="Square" />
    </Style>
</Window.Styles>
```

### 组合骨架屏默认启用头像

```xml
<Window.Styles>
    <Style Selector="atom|Skeleton">
        <Setter Property="IsShowAvatar" Value="True" />
        <Setter Property="ParagraphRows" Value="4" />
    </Style>
</Window.Styles>
```

---

## 3. 尺寸控制

### 独立元素尺寸

通过 `SizeType` 控制 `SkeletonButton`、`SkeletonInput`、`SkeletonAvatar` 的高度：

```xml
<!-- 大号 -->
<atom:SkeletonButton SizeType="Large" IsActive="True" />
<atom:SkeletonAvatar SizeType="Large" IsActive="True" />
<atom:SkeletonInput SizeType="Large" IsActive="True" />

<!-- 小号 -->
<atom:SkeletonButton SizeType="Small" IsActive="True" />
<atom:SkeletonAvatar SizeType="Small" IsActive="True" />
<atom:SkeletonInput SizeType="Small" IsActive="True" />
```

### Block 模式

```xml
<!-- SkeletonButton 和 SkeletonInput 支持 Block 模式 -->
<atom:SkeletonButton IsBlock="True" IsActive="True" />
<atom:SkeletonInput IsBlock="True" IsActive="True" />
```

### 自定义头像尺寸

```xml
<!-- 通过 Size 属性指定精确像素值 -->
<atom:SkeletonAvatar Size="64" IsActive="True" />
```

---

## 4. 动画控制

```xml
<!-- 启用流光动画 -->
<atom:Skeleton IsActive="True" IsLoading="True" />
<atom:SkeletonButton IsActive="True" />

<!-- 静态骨架（无动画） -->
<atom:Skeleton IsActive="False" IsLoading="True" />
<atom:SkeletonButton IsActive="False" />
```

尺寸和动画支持数据绑定：

```xml
<atom:SkeletonButton IsActive="{Binding IsSkeletonActive}"
                      SizeType="{Binding SkeletonSizeType}"
                      Shape="{Binding SkeletonButtonShape}" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Skeleton` 语法引用 `atom` XML 命名空间下的控件类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|Skeleton` | 匹配组合骨架屏 |
| `atom\|SkeletonButton` | 匹配按钮骨架 |
| `atom\|SkeletonInput` | 匹配输入框骨架 |
| `atom\|SkeletonAvatar` | 匹配头像骨架 |
| `atom\|SkeletonImage` | 匹配图片骨架 |
| `atom\|SkeletonNode` | 匹配自定义节点骨架 |
| `atom\|SkeletonLine` | 匹配单行骨架（段落内子元素） |
| `atom\|SkeletonParagraph` | 匹配段落骨架 |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|Skeleton[IsShowAvatar=True]` | 匹配显示头像的组合骨架屏 |
| `atom\|Skeleton[IsShowTitle=True]` | 匹配显示标题的组合骨架屏 |
| `atom\|Skeleton[IsShowParagraph=True]` | 匹配显示段落的组合骨架屏 |
| `atom\|SkeletonButton[Shape=Circle]` | 匹配圆形按钮骨架 |
| `atom\|SkeletonButton[Shape=Round]` | 匹配胶囊形按钮骨架 |
| `atom\|SkeletonButton[IsBlock=True]` | 匹配 Block 模式的按钮骨架 |
| `atom\|SkeletonAvatar[Shape=Square]` | 匹配方形头像骨架 |
| `atom\|SkeletonButton[SizeType=Large]` | 匹配大号按钮骨架 |
| `atom\|SkeletonButton[SizeType=Small]` | 匹配小号按钮骨架 |
| `atom\|SkeletonLine[IsRound=True]` | 匹配胶囊形圆角的行骨架 |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Skeleton /template/ atom\|SkeletonAvatar#PART_Avatar` | 组合骨架屏内的头像部件 |
| `atom\|Skeleton /template/ atom\|SkeletonTitle#PART_Title` | 组合骨架屏内的标题部件 |
| `atom\|Skeleton /template/ atom\|SkeletonParagraph#PART_Paragraph` | 组合骨架屏内的段落部件 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Skeleton[IsShowAvatar=True][IsShowParagraph=True]` | 同时显示头像和段落的组合骨架屏 |
| `atom\|Skeleton[IsShowAvatar=False][IsShowTitle=True]` | 无头像有标题的组合骨架屏（段落行数默认3） |
| `atom\|SkeletonButton[SizeType=Large][Shape=Round]` | 大号胶囊形按钮骨架 |
| `atom\|SkeletonAvatar[Shape=Square][SizeType=Small]` | 小号方形头像骨架 |
