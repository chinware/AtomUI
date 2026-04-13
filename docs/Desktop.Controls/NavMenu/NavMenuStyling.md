# NavMenu 自定义样式指南

NavMenu 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 NavMenu 的公共属性来控制外观和行为：

### 不同模式

```xml
<!-- 内嵌模式（默认） -->
<atom:NavMenu Mode="Inline" Width="300">
    <atom:NavMenuNode Header="Navigation One" Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
    <atom:NavMenuNode Header="Navigation Two" Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}" />
</atom:NavMenu>

<!-- 垂直模式 -->
<atom:NavMenu Mode="Vertical" Width="300">
    <atom:NavMenuNode Header="Navigation One" Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
    <atom:NavMenuNode Header="Navigation Two" />
</atom:NavMenu>

<!-- 水平模式 -->
<atom:NavMenu Mode="Horizontal">
    <atom:NavMenuNode Header="Navigation One" Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
    <atom:NavMenuNode Header="Navigation Two" />
</atom:NavMenu>
```

### 暗色主题

```xml
<atom:NavMenu IsDarkStyle="True" Mode="Horizontal">
    <atom:NavMenuNode Header="Navigation One" Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
    <atom:NavMenuNode Header="Navigation Two" />
</atom:NavMenu>
```

### 带图标和子菜单

```xml
<atom:NavMenu Mode="Inline" Width="300">
    <atom:NavMenuNode Header="Navigation One" Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
    <atom:NavMenuNode Header="Navigation Two" Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}"
                      IsEnabled="False" />
    <atom:NavMenuNode Header="Navigation Three - Submenu"
                      Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}">
        <atom:NavMenuNode Header="Item 1">
            <atom:NavMenuNode Header="Option 1" />
            <atom:NavMenuNode Header="Option 2" />
        </atom:NavMenuNode>
        <atom:NavMenuNode Header="Item 2">
            <atom:NavMenuNode Header="Option 3" />
            <atom:NavMenuNode Header="Option 4" />
        </atom:NavMenuNode>
    </atom:NavMenuNode>
    <atom:NavMenuNode Header="Navigation Four" />
</atom:NavMenu>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 NavMenu 进行全局或局部样式覆盖：

### 全局设置宽度

```xml
<Window.Styles>
    <Style Selector="atom|NavMenu">
        <Setter Property="Width" Value="256" />
    </Style>
</Window.Styles>
```

### 按模式定制

```xml
<!-- 水平模式时设置背景 -->
<Style Selector="atom|NavMenu:horizontal-mode">
    <Setter Property="Background" Value="#f0f0f0" />
</Style>

<!-- 内嵌模式时设置边框 -->
<Style Selector="atom|NavMenu:inline-mode">
    <Setter Property="BorderBrush" Value="#e8e8e8" />
    <Setter Property="BorderThickness" Value="0,0,1,0" />
</Style>
```

### 暗色/亮色主题定制

```xml
<!-- 暗色主题时的自定义背景 -->
<Style Selector="atom|NavMenu:dark">
    <Setter Property="Background" Value="#001529" />
</Style>

<!-- 亮色主题时的自定义背景 -->
<Style Selector="atom|NavMenu:light">
    <Setter Property="Background" Value="White" />
</Style>
```

---

## 3. 默认展开与默认选中

通过 `DefaultOpenPaths` 和 `DefaultSelectedPath` 属性指定初始状态，路径格式为 `/Key1/Key2/Key3`：

```xml
<atom:NavMenu Mode="Inline"
              DefaultOpenPaths="{Binding DefaultOpenPaths}"
              DefaultSelectedPath="{Binding DefaultSelectedPath}">
    <atom:NavMenuNode Header="Navigation One" ItemKey="1" />
    <atom:NavMenuNode Header="Navigation Two" ItemKey="2" />
    <atom:NavMenuNode Header="Navigation Three" ItemKey="3">
        <atom:NavMenuNode Header="Item 1" ItemKey="SubGroup1">
            <atom:NavMenuNode Header="Option 1" ItemKey="Option1" />
            <atom:NavMenuNode Header="Option 2" ItemKey="Option2" />
        </atom:NavMenuNode>
        <atom:NavMenuNode Header="Item 2" ItemKey="SubGroup2">
            <atom:NavMenuNode Header="Option 3" ItemKey="Option3" />
            <atom:NavMenuNode Header="Option 4" ItemKey="Option4" />
        </atom:NavMenuNode>
    </atom:NavMenuNode>
</atom:NavMenu>
```

```csharp
// ViewModel
public class MenuViewModel : ReactiveObject
{
    public IList<TreeNodePath>? DefaultOpenPaths { get; set; }
    public TreeNodePath? DefaultSelectedPath { get; set; }

    public MenuViewModel()
    {
        DefaultOpenPaths = new List<TreeNodePath>
        {
            new TreeNodePath("/3/SubGroup2")
        };
        DefaultSelectedPath = new TreeNodePath("/3/SubGroup1/Option1");
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml` 中 "Default Opened paths" 示例。

---

## 4. 动态切换模式和主题

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:ToggleSwitch Name="ChangeModeSwitch" />
        <atom:TextBlock>Change Mode</atom:TextBlock>
        <atom:ToggleSwitch Margin="10, 0, 0, 0" Name="ChangeStyleSwitch" />
        <atom:TextBlock>Change Style</atom:TextBlock>
    </StackPanel>
    <atom:NavMenu Mode="{Binding Mode}" Width="300"
                   IsDarkStyle="{Binding IsDark}">
        <atom:NavMenuNode Header="Navigation One" Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
        <atom:NavMenuNode Header="Navigation Two" />
    </atom:NavMenu>
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml` 中 "Switch the menu type" 示例。

---

## 5. 控制动画行为

```xml
<!-- 禁用展开/折叠动画 -->
<atom:NavMenu Mode="Inline" IsMotionEnabled="False">
    <atom:NavMenuNode Header="Navigation One" />
</atom:NavMenu>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|NavMenu` 语法引用 `atom` XML 命名空间下的 `NavMenu` 类型，其中 `|` 是命名空间分隔符。

### 按模式选择

| 选择器 | 说明 |
|---|---|
| `atom\|NavMenu` | 匹配所有 NavMenu 实例 |
| `atom\|NavMenu:inline-mode` | 匹配内嵌模式的 NavMenu |
| `atom\|NavMenu:vertical-mode` | 匹配垂直模式的 NavMenu |
| `atom\|NavMenu:horizontal-mode` | 匹配水平模式的 NavMenu |

### 按主题选择

| 选择器 | 说明 |
|---|---|
| `atom\|NavMenu:dark` | 匹配暗色主题的 NavMenu |
| `atom\|NavMenu:light` | 匹配亮色主题的 NavMenu |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|NavMenu[Mode=Inline]` | 匹配 Mode 为 Inline 的 NavMenu |
| `atom\|NavMenu[Mode=Horizontal]` | 匹配 Mode 为 Horizontal 的 NavMenu |
| `atom\|NavMenu[IsDarkStyle=True]` | 匹配暗色样式的 NavMenu |
| `atom\|NavMenu[IsAccordionMode=True]` | 匹配手风琴模式的 NavMenu |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|NavMenu:horizontal-mode:dark` | 暗色水平导航菜单 |
| `atom\|NavMenu:inline-mode:light` | 亮色内嵌导航菜单 |
| `atom\|NavMenu:disabled` | 禁用状态的 NavMenu |
