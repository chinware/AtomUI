# Collapse 使用文档

本文档介绍 AtomUI Collapse 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CollapseShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Collapse，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Collapse, CollapseItem 控件
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本用法

默认模式下，任意数量的面板可以同时展开。通过 `IsSelected="True"` 设置默认展开的面板：

```xml
<atom:Collapse>
    <atom:CollapseItem Header="This is panel header 1" IsSelected="True">
        <atom:TextBlock TextWrapping="Wrap">
            A dog is a type of domesticated animal. Known for its loyalty and faithfulness,
            it can be found as a welcome guest in many households across the world.
        </atom:TextBlock>
    </atom:CollapseItem>
    <atom:CollapseItem Header="This is panel header 2">
        <atom:TextBlock TextWrapping="Wrap">
            A dog is a type of domesticated animal. Known for its loyalty and faithfulness,
            it can be found as a welcome guest in many households across the world.
        </atom:TextBlock>
    </atom:CollapseItem>
    <atom:CollapseItem Header="This is panel header 3">
        <atom:TextBlock TextWrapping="Wrap">
            A dog is a type of domesticated animal. Known for its loyalty and faithfulness,
            it can be found as a welcome guest in many households across the world.
        </atom:TextBlock>
    </atom:CollapseItem>
</atom:Collapse>
```

---

## 2. 手风琴模式

通过 `IsAccordion="True"` 启用手风琴模式，同时只能展开一个面板：

```xml
<atom:Collapse IsAccordion="True">
    <atom:CollapseItem Header="This is panel header 1" IsSelected="True">
        Content 1
    </atom:CollapseItem>
    <atom:CollapseItem Header="This is panel header 2">
        Content 2
    </atom:CollapseItem>
    <atom:CollapseItem Header="This is panel header 3">
        Content 3
    </atom:CollapseItem>
</atom:Collapse>
```

**使用场景**：FAQ 列表、设置项分组等「一次只关注一件事」的场景。

---

## 3. 三种尺寸

通过 `SizeType` 属性设置面板尺寸，支持 `Large`、`Middle`（默认）、`Small` 三种：

```xml
<!-- 默认尺寸 -->
<atom:Collapse SizeType="Middle">
    <atom:CollapseItem Header="This is default size panel header">
        <atom:TextBlock TextWrapping="Wrap">Default size content</atom:TextBlock>
    </atom:CollapseItem>
</atom:Collapse>

<!-- 小号 -->
<atom:Collapse SizeType="Small">
    <atom:CollapseItem Header="This is small size panel header">
        <atom:TextBlock TextWrapping="Wrap">Small size content</atom:TextBlock>
    </atom:CollapseItem>
</atom:Collapse>

<!-- 大号 -->
<atom:Collapse SizeType="Large">
    <atom:CollapseItem Header="This is large size panel header">
        <atom:TextBlock TextWrapping="Wrap">Large size content</atom:TextBlock>
    </atom:CollapseItem>
</atom:Collapse>
```

**使用场景指引**：
- **Large**：信息密度低的场景，如产品详情页
- **Middle**：大多数常规场景
- **Small**：信息密度高的场景，如侧边栏、辅助面板

---

## 4. 无边框模式

通过 `IsBorderless="True"` 去除外边框，视觉更轻量：

```xml
<atom:Collapse IsBorderless="True">
    <atom:CollapseItem Header="This is panel header 1">
        <atom:TextBlock TextWrapping="Wrap">Content 1</atom:TextBlock>
    </atom:CollapseItem>
    <atom:CollapseItem Header="This is panel header 2">
        <atom:TextBlock TextWrapping="Wrap">Content 2</atom:TextBlock>
    </atom:CollapseItem>
    <atom:CollapseItem Header="This is panel header 3">
        <atom:TextBlock TextWrapping="Wrap">Content 3</atom:TextBlock>
    </atom:CollapseItem>
</atom:Collapse>
```

---

## 5. 幽灵模式

通过 `IsGhostStyle="True"` 使面板背景透明，最轻量化视觉效果：

```xml
<atom:Collapse IsGhostStyle="True">
    <atom:CollapseItem Header="This is panel header 1">
        <atom:TextBlock TextWrapping="Wrap">Content 1</atom:TextBlock>
    </atom:CollapseItem>
    <atom:CollapseItem Header="This is panel header 2">
        <atom:TextBlock TextWrapping="Wrap">Content 2</atom:TextBlock>
    </atom:CollapseItem>
    <atom:CollapseItem Header="This is panel header 3">
        <atom:TextBlock TextWrapping="Wrap">Content 3</atom:TextBlock>
    </atom:CollapseItem>
</atom:Collapse>
```

**使用场景**：嵌入到有背景色的区域中，需要与周围内容融为一体。

---

## 6. 展开图标位置

通过 `ExpandIconPosition` 控制展开图标在左侧或右侧：

```xml
<!-- 默认：图标在左侧 -->
<atom:Collapse ExpandIconPosition="Start">
    <atom:CollapseItem Header="Panel 1">Content 1</atom:CollapseItem>
</atom:Collapse>

<!-- 图标在右侧 -->
<atom:Collapse ExpandIconPosition="End">
    <atom:CollapseItem Header="Panel 1">Content 1</atom:CollapseItem>
</atom:Collapse>
```

展开图标位置支持数据绑定，可在运行时动态切换：

```xml
<atom:Collapse ExpandIconPosition="{Binding CollapseExpandIconPosition}">
    <atom:CollapseItem Header="Panel 1"
                       AddOnContent="{antdicons:AntDesignIconProvider Kind=SettingOutlined}">
        Content 1
    </atom:CollapseItem>
</atom:Collapse>
```

---

## 7. 触发方式

通过 `TriggerType` 控制折叠/展开的触发区域：

```xml
<!-- 默认：点击头部任意位置触发 -->
<atom:Collapse>
    <atom:CollapseItem Header="This panel can only be collapsed by clicking text">
        Content
    </atom:CollapseItem>
</atom:Collapse>

<!-- 仅点击图标触发 -->
<atom:Collapse TriggerType="Icon">
    <atom:CollapseItem Header="This panel can only be collapsed by clicking icon">
        Content
    </atom:CollapseItem>
</atom:Collapse>
```

**使用场景**：当头部包含可交互元素（如按钮、链接）时，使用 `TriggerType="Icon"` 避免误触。

---

## 8. 隐藏特定面板的展开图标

通过 CollapseItem 的 `IsShowExpandIcon="False"` 隐藏单个面板的展开图标：

```xml
<atom:Collapse>
    <atom:CollapseItem Header="With expand icon">
        Content
    </atom:CollapseItem>
    <atom:CollapseItem Header="No expand icon" IsShowExpandIcon="False">
        Content (can still be collapsed by clicking header)
    </atom:CollapseItem>
</atom:Collapse>
```

---

## 9. 附加内容（AddOnContent）

在头部右侧放置操作按钮或图标，通过 `AddOnContent` 属性设置：

```xml
<atom:Collapse ExpandIconPosition="End">
    <atom:CollapseItem Header="Panel with settings icon"
                       AddOnContent="{antdicons:AntDesignIconProvider Kind=SettingOutlined}">
        <atom:TextBlock TextWrapping="Wrap">Content with extra action</atom:TextBlock>
    </atom:CollapseItem>
    <atom:CollapseItem Header="Panel with another icon"
                       AddOnContent="{antdicons:AntDesignIconProvider Kind=SettingOutlined}">
        <atom:TextBlock TextWrapping="Wrap">Another content</atom:TextBlock>
    </atom:CollapseItem>
</atom:Collapse>
```

也可以使用 `AddOnContentTemplate` 设置更复杂的附加内容。

---

## 10. 嵌套折叠面板

Collapse 支持嵌套使用：

```xml
<atom:Collapse>
    <atom:CollapseItem Header="This is panel header 1">
        <atom:Collapse>
            <atom:CollapseItem Header="This is panel header 1">
                <atom:TextBlock TextWrapping="Wrap">
                    Nested content
                </atom:TextBlock>
            </atom:CollapseItem>
        </atom:Collapse>
    </atom:CollapseItem>
    <atom:CollapseItem Header="This is panel header 2">
        <atom:TextBlock TextWrapping="Wrap">
            Regular content
        </atom:TextBlock>
    </atom:CollapseItem>
</atom:Collapse>
```

---

## 11. 自定义头部和内容间距

通过 `ItemHeaderPadding` 和 `ItemContentPadding` 统一设置所有面板的间距：

```xml
<!-- 自定义统一间距 -->
<atom:Collapse ItemHeaderPadding="5" ItemContentPadding="5">
    <atom:CollapseItem Header="Panel 1">Content 1</atom:CollapseItem>
    <atom:CollapseItem Header="Panel 2">Content 2</atom:CollapseItem>
</atom:Collapse>

<!-- 零间距幽灵模式 -->
<atom:Collapse ItemHeaderPadding="0" ItemContentPadding="0" IsGhostStyle="True">
    <atom:CollapseItem Header="Panel 1">Content 1</atom:CollapseItem>
    <atom:CollapseItem Header="Panel 2">Content 2</atom:CollapseItem>
</atom:Collapse>
```

也可以在 CollapseItem 级别单独设置：

```xml
<atom:Collapse>
    <atom:CollapseItem Header="Custom padding" HeaderPadding="20" ContentPadding="20">
        Content with more padding
    </atom:CollapseItem>
    <atom:CollapseItem Header="Default padding">
        Content with default padding
    </atom:CollapseItem>
</atom:Collapse>
```

---

## 12. 禁用状态

通过 `IsEnabled="False"` 禁用整个折叠面板：

```xml
<!-- 禁用（折叠态） -->
<atom:Collapse IsEnabled="False">
    <atom:CollapseItem Header="This panel can't be collapsed">
        <atom:TextBlock TextWrapping="Wrap">Content</atom:TextBlock>
    </atom:CollapseItem>
</atom:Collapse>

<!-- 禁用（展开态） -->
<atom:Collapse IsEnabled="False">
    <atom:CollapseItem Header="This panel can't be collapsed" IsSelected="True">
        <atom:TextBlock TextWrapping="Wrap">Content</atom:TextBlock>
    </atom:CollapseItem>
</atom:Collapse>
```

---

## 13. 数据驱动（ItemsSource）

通过 `ItemsSource` 绑定数据集合自动生成面板，配合 `ICollapseItemData` 接口映射属性：

```csharp
// 数据模型
public class MyCollapseData : ICollapseItemData
{
    public object? Header { get; set; }
    public bool IsSelected { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsShowExpandIcon { get; set; } = true;
    public string Content { get; set; } = "";
}

// ViewModel
public ObservableCollection<MyCollapseData> CollapsePanels { get; } = new()
{
    new MyCollapseData { Header = "Panel 1", Content = "Content 1", IsSelected = true },
    new MyCollapseData { Header = "Panel 2", Content = "Content 2" },
    new MyCollapseData { Header = "Panel 3", Content = "Content 3" },
};
```

```xml
<atom:Collapse ItemsSource="{Binding CollapsePanels}">
    <atom:Collapse.ItemTemplate>
        <DataTemplate>
            <atom:TextBlock Text="{Binding Content}" TextWrapping="Wrap" />
        </DataTemplate>
    </atom:Collapse.ItemTemplate>
</atom:Collapse>
```

也可以使用内置的 `CollapseItemData` record：

```csharp
var panels = new ObservableCollection<CollapseItemData>
{
    new() { Header = "Panel 1", IsSelected = true },
    new() { Header = "Panel 2" },
    new() { Header = "Panel 3", IsShowExpandIcon = false },
};
```

---

## 14. 控制动画行为

```xml
<!-- 禁用折叠/展开动画 -->
<atom:Collapse IsMotionEnabled="False">
    <atom:CollapseItem Header="No animation">Content</atom:CollapseItem>
</atom:Collapse>
```

---

## 常见组合模式

### FAQ 列表

```xml
<atom:Collapse IsAccordion="True" IsBorderless="True">
    <atom:CollapseItem Header="What is AtomUI?">
        <atom:TextBlock TextWrapping="Wrap">
            AtomUI is a cross-platform UI control library that reproduces
            Ant Design 5.0 on .NET / Avalonia.
        </atom:TextBlock>
    </atom:CollapseItem>
    <atom:CollapseItem Header="Which platforms are supported?">
        <atom:TextBlock TextWrapping="Wrap">
            Currently supports Windows, macOS, and Linux. Mobile support is planned.
        </atom:TextBlock>
    </atom:CollapseItem>
</atom:Collapse>
```

### 设置面板

```xml
<atom:Collapse>
    <atom:CollapseItem Header="General Settings" IsSelected="True">
        <!-- 通用设置内容 -->
    </atom:CollapseItem>
    <atom:CollapseItem Header="Advanced Settings">
        <!-- 高级设置内容 -->
    </atom:CollapseItem>
    <atom:CollapseItem Header="About">
        <!-- 关于信息 -->
    </atom:CollapseItem>
</atom:Collapse>
```

### 带操作按钮的面板

```xml
<atom:Collapse ExpandIconPosition="End">
    <atom:CollapseItem Header="Network Configuration"
                       AddOnContent="{antdicons:AntDesignIconProvider Kind=SettingOutlined}">
        <!-- 网络配置内容 -->
    </atom:CollapseItem>
    <atom:CollapseItem Header="Security Settings"
                       AddOnContent="{antdicons:AntDesignIconProvider Kind=LockOutlined}">
        <!-- 安全设置内容 -->
    </atom:CollapseItem>
</atom:Collapse>
```

### 嵌套分组（仅图标触发）

```xml
<atom:Collapse TriggerType="Icon">
    <atom:CollapseItem Header="Database" IsSelected="True">
        <atom:Collapse IsGhostStyle="True" SizeType="Small">
            <atom:CollapseItem Header="Connection">Connection settings</atom:CollapseItem>
            <atom:CollapseItem Header="Performance">Performance tuning</atom:CollapseItem>
        </atom:Collapse>
    </atom:CollapseItem>
    <atom:CollapseItem Header="Cache">
        <atom:TextBlock TextWrapping="Wrap">Cache configuration</atom:TextBlock>
    </atom:CollapseItem>
</atom:Collapse>
```
