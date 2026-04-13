# Expander 自定义样式指南

本文档介绍如何通过属性、Style 选择器和 ControlTheme 自定义 AtomUI Expander 的外观。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ExpanderShowCase.axaml`

---

## 1. 通过属性直接控制

### 基本风格

```xml
<!-- 默认风格（带边框 + 灰色头部背景） -->
<atom:Expander Header="默认风格" />

<!-- 无边框风格 -->
<atom:Expander IsBorderless="True" Header="无边框风格" />

<!-- 幽灵风格（头部背景透明） -->
<atom:Expander IsGhostStyle="True" Header="幽灵风格" />
```

### 尺寸控制

```xml
<atom:Expander SizeType="Large" Header="大号面板" />
<atom:Expander SizeType="Middle" Header="中号面板（默认）" />
<atom:Expander SizeType="Small" Header="小号面板" />
```

### 展开图标

```xml
<!-- 隐藏展开图标 -->
<atom:Expander Header="无图标" IsShowExpandIcon="False" />

<!-- 图标在右侧 -->
<atom:Expander Header="右侧图标" ExpandIconPosition="End" />

<!-- 仅图标可触发展开 -->
<atom:Expander Header="仅图标触发" TriggerType="Icon" />
```

### 附加内容

```xml
<!-- 头部右侧添加设置图标 -->
<atom:Expander Header="带附加内容"
               AddOnContent="{antdicons:AntDesignIconProvider Kind=SettingOutlined}" />
```

### 自定义间距

```xml
<!-- 自定义头部和内容内边距 -->
<atom:Expander Header="自定义间距" HeaderPadding="5" ContentPadding="5" />
```

### 展开方向

```xml
<atom:Expander Header="向下展开" ExpandDirection="Down" />
<atom:Expander Header="向上展开" ExpandDirection="Up" />
<atom:Expander Header="向左展开" ExpandDirection="Left" />
<atom:Expander Header="向右展开" ExpandDirection="Right" />
```

---

## 2. 通过 Style 覆盖

### 全局统一风格

```xml
<Window.Styles>
    <!-- 所有 Expander 使用小号尺寸 + 无边框 -->
    <Style Selector="atom|Expander">
        <Setter Property="SizeType" Value="Small" />
        <Setter Property="IsBorderless" Value="True" />
    </Style>
</Window.Styles>
```

### 修改头部背景色

```xml
<Window.Styles>
    <Style Selector="atom|Expander /template/ Border#PART_HeaderDecorator">
        <Setter Property="Background" Value="#E6F7FF" />
    </Style>
</Window.Styles>
```

### 修改展开状态下的头部样式

```xml
<Window.Styles>
    <Style Selector="atom|Expander:expanded /template/ Border#PART_HeaderDecorator">
        <Setter Property="Background" Value="#BAE7FF" />
    </Style>
</Window.Styles>
```

### 修改内容区域背景

```xml
<Window.Styles>
    <Style Selector="atom|Expander /template/ ContentPresenter#PART_ContentPresenter">
        <Setter Property="Background" Value="#FAFAFA" />
    </Style>
</Window.Styles>
```

---

## 3. 通过 Token 覆盖

Expander 的大部分视觉属性由 `ExpanderToken` 控制，可通过主题定制修改默认值。详见 [ExpanderToken.md](./ExpanderToken.md)。

---

## 样式选择器速查

### 控件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Expander` | 匹配所有 Expander |
| `atom\|Expander:expanded` | 展开状态 |
| `atom\|Expander:up` | 向上展开 |
| `atom\|Expander:down` | 向下展开 |
| `atom\|Expander:left` | 向左展开 |
| `atom\|Expander:right` | 向右展开 |
| `atom\|Expander:disabled` | 禁用状态 |

### 属性选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Expander[SizeType=Large]` | 大号尺寸 |
| `atom\|Expander[SizeType=Small]` | 小号尺寸 |
| `atom\|Expander[IsBorderless=True]` | 无边框风格 |
| `atom\|Expander[IsGhostStyle=True]` | 幽灵风格 |
| `atom\|Expander[TriggerType=Icon]` | 仅图标触发模式 |
| `atom\|Expander[ExpandIconPosition=End]` | 图标在右侧 |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Expander /template/ Border#PART_Frame` | 外层边框 |
| `atom\|Expander /template/ Border#PART_HeaderDecorator` | 头部装饰容器 |
| `atom\|Expander /template/ ContentPresenter#PART_HeaderPresenter` | 头部内容 |
| `atom\|Expander /template/ ContentPresenter#PART_ContentPresenter` | 折叠内容 |
| `atom\|Expander /template/ atom\|IconButton#PART_ExpandButton` | 展开图标 |
