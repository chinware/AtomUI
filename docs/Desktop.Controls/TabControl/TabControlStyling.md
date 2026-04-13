# TabControl 自定义样式指南

本文档介绍如何通过 Avalonia 的 `Style` 机制自定义 TabControl 控件家族的视觉表现。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/TabControlShowCase.axaml`

---

## 1. 通过属性直接控制

最简单的定制方式是通过控件公开的属性：

```xml
<!-- 线条式标签页，标签栏在左侧，大号尺寸 -->
<atom:TabControl TabStripPlacement="Left" SizeType="Large">
    <atom:TabItem Header="Tab 1">内容 1</atom:TabItem>
    <atom:TabItem Header="Tab 2">内容 2</atom:TabItem>
</atom:TabControl>

<!-- 卡片式标签页，标签居中，显示添加按钮 -->
<atom:CardTabControl TabAlignmentCenter="True" IsShowAddTabButton="True">
    <atom:TabItem Header="Tab 1">内容 1</atom:TabItem>
    <atom:TabItem Header="Tab 2">内容 2</atom:TabItem>
</atom:CardTabControl>
```

---

## 2. 通过 Style 覆盖

### 2.1 修改标签文本样式

```xml
<Window.Styles>
    <!-- 修改所有 TabItem 的字体大小 -->
    <Style Selector="atom|TabItem">
        <Setter Property="FontSize" Value="16" />
        <Setter Property="FontWeight" Value="SemiBold" />
    </Style>
</Window.Styles>
```

### 2.2 修改标签栏边框

```xml
<Window.Styles>
    <!-- 加粗标签栏底部分割线 -->
    <Style Selector="atom|TabControl">
        <Setter Property="BorderThickness" Value="2" />
    </Style>
</Window.Styles>
```

### 2.3 根据方向定制样式

利用伪类 `:top` / `:bottom` / `:left` / `:right`：

```xml
<Window.Styles>
    <!-- 仅在左侧布局时增加内容区域间距 -->
    <Style Selector="atom|TabControl:left">
        <Setter Property="TabAndContentGutter" Value="20" />
    </Style>
</Window.Styles>
```

### 2.4 定制单个标签项状态

```xml
<Window.Styles>
    <!-- 悬浮时加粗 -->
    <Style Selector="atom|TabItem:pointerover">
        <Setter Property="FontWeight" Value="Bold" />
    </Style>

    <!-- 选中时下划线加粗 -->
    <Style Selector="atom|TabItem:selected">
        <Setter Property="FontWeight" Value="Bold" />
    </Style>
</Window.Styles>
```

---

## 3. 样式选择器速查

### TabControl / CardTabControl 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TabControl` | 匹配所有线条式 TabControl |
| `atom\|CardTabControl` | 匹配所有卡片式 TabControl |
| `atom\|BaseTabControl` | 匹配两种 TabControl 基类 |
| `atom\|TabControl:top` | 标签栏在顶部 |
| `atom\|TabControl:bottom` | 标签栏在底部 |
| `atom\|TabControl:left` | 标签栏在左侧 |
| `atom\|TabControl:right` | 标签栏在右侧 |

### TabStrip / CardTabStrip 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TabStrip` | 匹配所有线条式 TabStrip |
| `atom\|CardTabStrip` | 匹配所有卡片式 TabStrip |
| `atom\|TabStrip:top` | 标签栏在顶部 |
| `atom\|TabStrip:left` | 标签栏在左侧 |

### TabItem / TabStripItem 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TabItem` | 匹配所有 TabItem |
| `atom\|TabItem:selected` | 选中的标签 |
| `atom\|TabItem:pointerover` | 鼠标悬浮的标签 |
| `atom\|TabItem:pressed` | 鼠标按下的标签 |
| `atom\|TabItem:disabled` | 禁用的标签 |
| `atom\|TabStripItem` | 匹配所有 TabStripItem |
| `atom\|TabStripItem:selected` | 选中的 StripItem |

### 模板内部选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TabItem /template/ atom\|IconPresenter#ItemIconPresenter` | 标签图标 |
| `atom\|TabItem /template/ atom\|IconButton#PART_ItemCloseButton` | 关闭按钮 |
| `atom\|TabItem /template/ ContentPresenter#ContentPresenter` | 标签标题内容 |

---

## 4. 通过 Token 主题全局定制

如果需要全局修改 TabControl 的外观，建议通过 Token 系统而非直接修改 Style，以保持主题一致性。

详见 [TabControl Design Token](./TabControlToken.md) 文档。

---

## 5. Gallery 中的示例

以下示例均可在 Gallery 项目中查看实际效果：

| 示例 | 说明 | 关键属性 |
|---|---|---|
| Basic | 基本三标签切换 | 默认配置 |
| Disabled | 禁用特定标签 | `IsEnabled="False"` |
| Centered | 标签居中 | `TabAlignmentCenter="True"` |
| Icon | 带图标标签 | `Icon="{antdicons:AntDesignIconProvider}"` |
| Slide | 标签溢出滚动 | 20+ 标签 |
| Card type tab | 卡片式标签 | `atom:CardTabControl` |
| Closable Tab | 可关闭标签 | `IsTabClosable="True"` |
| Position | 四方向切换 | `TabStripPlacement` |
| Size | 三种尺寸 | `SizeType` |
| Add and close | 添加/关闭标签 | `IsShowAddTabButton="True"` |
| Extra content | 头部额外内容 | `HeaderStartExtraContent` / `HeaderEndExtraContent` |

> 📖 完整示例路径：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/TabControlShowCase.axaml`
