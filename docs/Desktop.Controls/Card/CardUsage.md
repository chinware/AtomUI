# Card 使用文档

本文档介绍 AtomUI Card 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CardShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Card，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Card, CardMetaContent, CardTabsContent 等
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本卡片

最基本的卡片，包含标题、内容和额外区域。支持三种尺寸。

```xml
<WrapPanel ItemSpacing="20" LineSpacing="20">
    <atom:Card Header="Large size card" SizeType="Large" Width="300">
        <atom:Card.Extra>
            <atom:HyperLinkButton>More</atom:HyperLinkButton>
        </atom:Card.Extra>
        <StackPanel Orientation="Vertical" Spacing="3">
            <atom:TextBlock>Card content</atom:TextBlock>
            <atom:TextBlock>Card content</atom:TextBlock>
            <atom:TextBlock>Card content</atom:TextBlock>
        </StackPanel>
    </atom:Card>

    <atom:Card Header="Default size card" SizeType="Middle" Width="300">
        <atom:Card.Extra>
            <atom:HyperLinkButton>More</atom:HyperLinkButton>
        </atom:Card.Extra>
        <StackPanel Orientation="Vertical" Spacing="3">
            <atom:TextBlock>Card content</atom:TextBlock>
            <atom:TextBlock>Card content</atom:TextBlock>
            <atom:TextBlock>Card content</atom:TextBlock>
        </StackPanel>
    </atom:Card>

    <atom:Card Header="Small size card" SizeType="Small" Width="300">
        <atom:Card.Extra>
            <atom:HyperLinkButton>More</atom:HyperLinkButton>
        </atom:Card.Extra>
        <StackPanel Orientation="Vertical" Spacing="3">
            <atom:TextBlock>Card content</atom:TextBlock>
            <atom:TextBlock>Card content</atom:TextBlock>
            <atom:TextBlock>Card content</atom:TextBlock>
        </StackPanel>
    </atom:Card>
</WrapPanel>
```

**使用场景指引**：
- 默认使用 `Middle` 尺寸
- 页面级概览卡片使用 `Large`
- 紧凑布局或辅助信息使用 `Small`

---

## 2. 简洁卡片（无标题）

不设置 `Header` 和 `Extra`，卡片将不显示头部区域（`:headerless` 伪类激活）：

```xml
<atom:Card Width="300" HorizontalAlignment="Left">
    <StackPanel Orientation="Vertical" Spacing="3">
        <atom:TextBlock>Card content</atom:TextBlock>
        <atom:TextBlock>Card content</atom:TextBlock>
        <atom:TextBlock>Card content</atom:TextBlock>
    </StackPanel>
</atom:Card>
```

---

## 3. 无边框卡片

在灰色或有色背景上使用无边框风格，通过阴影划定卡片边界：

```xml
<Border Padding="20" Background="#F0F2F5">
    <atom:Card Header="Card title" Width="300" StyleVariant="Borderless"
               HorizontalAlignment="Left">
        <atom:Card.Extra>
            <atom:HyperLinkButton>More</atom:HyperLinkButton>
        </atom:Card.Extra>
        <StackPanel Orientation="Vertical" Spacing="3">
            <atom:TextBlock>Card content</atom:TextBlock>
            <atom:TextBlock>Card content</atom:TextBlock>
            <atom:TextBlock>Card content</atom:TextBlock>
        </StackPanel>
    </atom:Card>
</Border>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CardShowCase.axaml` 中 "No border" 示例。

---

## 4. 卡片列布局

多张卡片在网格中排列，常用于概览页面：

```xml
<Border Padding="20" Background="#F0F2F5">
    <Grid RowDefinitions="*" ColumnDefinitions="*, *, *" ColumnSpacing="20">
        <atom:Card Header="Card title" StyleVariant="Borderless"
                   Grid.Row="0" Grid.Column="0" HorizontalAlignment="Stretch">
            <StackPanel Orientation="Vertical" Spacing="3">
                <atom:TextBlock>Card content</atom:TextBlock>
                <atom:TextBlock>Card content</atom:TextBlock>
                <atom:TextBlock>Card content</atom:TextBlock>
            </StackPanel>
        </atom:Card>
        <atom:Card Header="Card title" StyleVariant="Borderless"
                   Grid.Row="0" Grid.Column="1" HorizontalAlignment="Stretch">
            <StackPanel Orientation="Vertical" Spacing="3">
                <atom:TextBlock>Card content</atom:TextBlock>
                <atom:TextBlock>Card content</atom:TextBlock>
                <atom:TextBlock>Card content</atom:TextBlock>
            </StackPanel>
        </atom:Card>
        <atom:Card Header="Card title" StyleVariant="Borderless"
                   Grid.Row="0" Grid.Column="2" HorizontalAlignment="Stretch">
            <StackPanel Orientation="Vertical" Spacing="3">
                <atom:TextBlock>Card content</atom:TextBlock>
                <atom:TextBlock>Card content</atom:TextBlock>
                <atom:TextBlock>Card content</atom:TextBlock>
            </StackPanel>
        </atom:Card>
    </Grid>
</Border>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CardShowCase.axaml` 中 "Card in column" 示例。

---

## 5. 悬浮阴影卡片

通过 `IsHoverable` 启用鼠标悬浮效果，卡片在悬浮时「升起」：

```xml
<atom:Card Width="240" IsHoverable="True" HorizontalAlignment="Left">
    <atom:Card.Cover>
        <Image Source="/Assets/CardShowCase/Cover1.png" />
    </atom:Card.Cover>
    <atom:CardMetaContent Header="Europe Street beat"
                          Content="www.instagram.com" />
</atom:Card>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CardShowCase.axaml` 中 "Customized content" 示例。

---

## 6. 带封面和 Meta 内容

使用 `Cover` 属性设置封面图片，`CardMetaContent` 展示结构化信息：

```xml
<atom:Card Width="300" HorizontalAlignment="Left">
    <atom:Card.Cover>
        <Image Source="/Assets/CardShowCase/Cover2.png" />
    </atom:Card.Cover>
    <atom:CardMetaContent Header="Card title"
                          Content="This is the description">
        <atom:CardMetaContent.Avatar>
            <atom:Avatar Src="avares://AtomUIGallery/Assets/AvatarShowCase/PeopleAvatar1.svg" />
        </atom:CardMetaContent.Avatar>
    </atom:CardMetaContent>
    <atom:Card.Actions>
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=EditOutlined}" />
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}" />
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=EllipsisOutlined}" />
    </atom:Card.Actions>
</atom:Card>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CardShowCase.axaml` 中 "Support more content configuration" 示例。

---

## 7. 加载状态

通过 `IsLoading` 控制骨架屏加载占位，适用于异步数据加载场景：

### 静态加载状态

```xml
<atom:Card MinWidth="300" IsLoading="True" HorizontalAlignment="Left">
    <atom:CardMetaContent Header="Card title">
        <atom:CardMetaContent.Avatar>
            <atom:Avatar Src="avares://AtomUIGallery/Assets/AvatarShowCase/PeopleAvatar1.svg" />
        </atom:CardMetaContent.Avatar>
        <StackPanel Orientation="Vertical" Spacing="3">
            <atom:TextBlock>This is the description</atom:TextBlock>
            <atom:TextBlock>This is the description</atom:TextBlock>
        </StackPanel>
    </atom:CardMetaContent>
    <atom:Card.Actions>
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=EditOutlined}" />
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}" />
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=EllipsisOutlined}" />
    </atom:Card.Actions>
</atom:Card>
```

### 动态加载状态（数据绑定）

```xml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:ToggleSwitch IsChecked="{Binding IsLoading, Mode=TwoWay}" />
    <atom:Card MinWidth="300" IsLoading="{Binding IsLoading}" HorizontalAlignment="Left">
        <atom:CardMetaContent Header="Card title">
            <StackPanel Orientation="Vertical" Spacing="3">
                <atom:TextBlock>This is the description</atom:TextBlock>
            </StackPanel>
        </atom:CardMetaContent>
    </atom:Card>
</StackPanel>
```

```csharp
// ViewModel
public class CardViewModel : ReactiveObject
{
    [Reactive]
    public bool IsLoading { get; set; } = true;
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CardShowCase.axaml` 中 "Loading card" 示例。

---

## 8. Grid 网格卡片

使用 `CardGridContent` 和 `CardGridItem` 创建网格布局：

```xml
<atom:Card HorizontalAlignment="Stretch" Header="Card Title" SizeType="Large">
    <atom:CardGridContent ColumnDefinitions="*, *, *, *"
                          RowDefinitions="Auto, Auto">
        <atom:CardGridItem Row="0" Column="0">Content</atom:CardGridItem>
        <atom:CardGridItem Row="0" Column="1" IsHoverable="False">Content</atom:CardGridItem>
        <atom:CardGridItem Row="0" Column="2">Content</atom:CardGridItem>
        <atom:CardGridItem Row="0" Column="3">Content</atom:CardGridItem>
        <atom:CardGridItem Row="1" Column="0">Content</atom:CardGridItem>
        <atom:CardGridItem Row="1" Column="1">Content</atom:CardGridItem>
        <atom:CardGridItem Row="1" Column="2">Content</atom:CardGridItem>
    </atom:CardGridContent>
</atom:Card>
```

**特点**：
- `ColumnDefinitions` 和 `RowDefinitions` 的语法与 Avalonia `Grid` 一致
- 每个 `CardGridItem` 通过 `Row` 和 `Column` 定位
- 可通过 `IsHoverable="False"` 禁用单个格子的悬浮效果
- 网格内容模式下头部底部边框自动隐藏

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CardShowCase.axaml` 中 "Grid card" 示例。

---

## 9. 内嵌卡片

通过 `IsInnerMode` 实现卡片嵌套，内层卡片头部自动变色以区分层级：

```xml
<atom:Card Header="Card title" HorizontalAlignment="Stretch" SizeType="Large">
    <StackPanel Orientation="Vertical" Spacing="20">
        <atom:Card Header="Card title" HorizontalAlignment="Stretch" IsInnerMode="True">
            <atom:Card.Extra>
                <atom:HyperLinkButton>More</atom:HyperLinkButton>
            </atom:Card.Extra>
            <StackPanel Orientation="Vertical" Spacing="3">
                <atom:TextBlock>Card content</atom:TextBlock>
                <atom:TextBlock>Card content</atom:TextBlock>
                <atom:TextBlock>Card content</atom:TextBlock>
            </StackPanel>
        </atom:Card>

        <atom:Card Header="Card title" HorizontalAlignment="Stretch" IsInnerMode="True">
            <atom:Card.Extra>
                <atom:HyperLinkButton>More</atom:HyperLinkButton>
            </atom:Card.Extra>
            <StackPanel Orientation="Vertical" Spacing="3">
                <atom:TextBlock>Card content</atom:TextBlock>
                <atom:TextBlock>Card content</atom:TextBlock>
                <atom:TextBlock>Card content</atom:TextBlock>
            </StackPanel>
        </atom:Card>
    </StackPanel>
</atom:Card>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CardShowCase.axaml` 中 "Inner card" 示例。

---

## 10. 内嵌标签页

使用 `CardTabsContent` 在卡片中嵌入标签页：

### 标题 + Tabs

```xml
<atom:Card Header="Card title" HorizontalAlignment="Stretch" SizeType="Large">
    <atom:Card.Extra>
        <atom:HyperLinkButton>More</atom:HyperLinkButton>
    </atom:Card.Extra>
    <atom:CardTabsContent>
        <atom:TabItem Header="Tab1">content1</atom:TabItem>
        <atom:TabItem Header="Tab2">content2</atom:TabItem>
    </atom:CardTabsContent>
</atom:Card>
```

### Tabs + 标签栏额外内容（无标题）

```xml
<atom:Card HorizontalAlignment="Stretch">
    <atom:CardTabsContent>
        <atom:CardTabsContent.TabBarExtraContent>
            <atom:HyperLinkButton>More</atom:HyperLinkButton>
        </atom:CardTabsContent.TabBarExtraContent>
        <atom:TabItem Header="article">article content</atom:TabItem>
        <atom:TabItem Header="app">app content</atom:TabItem>
        <atom:TabItem Header="project">project content</atom:TabItem>
    </atom:CardTabsContent>
</atom:Card>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CardShowCase.axaml` 中 "With tabs" 示例。

---

## 11. 操作区按钮

通过 `Card.Actions` 添加操作按钮，操作区自动等分布局：

```xml
<atom:Card Width="300">
    <atom:CardMetaContent Header="Card title" Content="Description text">
        <atom:CardMetaContent.Avatar>
            <atom:Avatar Src="avares://AtomUIGallery/Assets/AvatarShowCase/PeopleAvatar1.svg" />
        </atom:CardMetaContent.Avatar>
    </atom:CardMetaContent>
    <atom:Card.Actions>
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=EditOutlined}" />
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}" />
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=EllipsisOutlined}" />
    </atom:Card.Actions>
</atom:Card>
```

**特点**：
- 操作区使用 `UniformGrid` 等分布局
- 操作按钮之间自动绘制分割线
- 操作区与内容区之间自动绘制顶部分割线
- 图标悬浮时变为主色（`ColorPrimary`）

---

## 12. 控制动画行为

```xml
<!-- 禁用过渡动画（BoxShadow 悬浮切换时不再平滑过渡） -->
<atom:Card Header="Card Title" IsHoverable="True" IsMotionEnabled="False" Width="300">
    <atom:TextBlock>No animation transition</atom:TextBlock>
</atom:Card>
```

---

## 常见组合模式

### 信息展示卡片

```xml
<atom:Card Header="用户信息" SizeType="Large" Width="350">
    <atom:Card.Extra>
        <atom:HyperLinkButton>编辑</atom:HyperLinkButton>
    </atom:Card.Extra>
    <atom:CardMetaContent Header="张三" Content="前端工程师 · 杭州">
        <atom:CardMetaContent.Avatar>
            <atom:Avatar Text="张" />
        </atom:CardMetaContent.Avatar>
    </atom:CardMetaContent>
</atom:Card>
```

### 产品卡片

```xml
<atom:Card Width="280" IsHoverable="True">
    <atom:Card.Cover>
        <Image Source="/Assets/product.png" />
    </atom:Card.Cover>
    <atom:CardMetaContent Header="产品名称" Content="产品描述信息" />
    <atom:Card.Actions>
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}" />
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=ShoppingCartOutlined}" />
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=ShareAltOutlined}" />
    </atom:Card.Actions>
</atom:Card>
```

### 功能入口网格

```xml
<atom:Card Header="快捷操作" HorizontalAlignment="Stretch">
    <atom:CardGridContent ColumnDefinitions="*, *, *" RowDefinitions="Auto">
        <atom:CardGridItem Row="0" Column="0">操作一</atom:CardGridItem>
        <atom:CardGridItem Row="0" Column="1">操作二</atom:CardGridItem>
        <atom:CardGridItem Row="0" Column="2">操作三</atom:CardGridItem>
    </atom:CardGridContent>
</atom:Card>
```

### 多维度信息展示

```xml
<atom:Card HorizontalAlignment="Stretch">
    <atom:CardTabsContent>
        <atom:CardTabsContent.TabBarExtraContent>
            <atom:HyperLinkButton>更多</atom:HyperLinkButton>
        </atom:CardTabsContent.TabBarExtraContent>
        <atom:TabItem Header="概览">概览内容</atom:TabItem>
        <atom:TabItem Header="详情">详情内容</atom:TabItem>
        <atom:TabItem Header="日志">日志内容</atom:TabItem>
    </atom:CardTabsContent>
</atom:Card>
```
