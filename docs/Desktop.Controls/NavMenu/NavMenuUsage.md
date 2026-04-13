# NavMenu 使用文档

本文档介绍 AtomUI NavMenu 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 NavMenu，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // NavMenu, NavMenuNode, NavMenuMode
using AtomUI.Controls.Primitives; // TreeNodePath, ITreeNode
```

---

## 1. 垂直导航菜单（Vertical）

垂直模式下，子菜单通过 Popup 弹出层在右侧展示：

```xml
<atom:NavMenu Mode="Vertical" Width="300" Margin="0, 0, 0, 20">
    <atom:NavMenuNode Header="Navigation One"
                      Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
    <atom:NavMenuNode Header="Navigation Two"
                      Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}"
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

**使用场景**：侧边栏导航，子菜单不占用主面板空间。

---

## 2. 内嵌导航菜单（Inline）

内嵌模式下，子菜单在当前面板内折叠/展开，带有动画效果：

```xml
<atom:NavMenu Mode="Inline" Width="300" Margin="0, 0, 0, 20">
    <atom:NavMenuNode Header="Navigation One"
                      Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
    <atom:NavMenuNode Header="Navigation Two"
                      Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}"
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

**使用场景**：侧边栏导航，用户需要同时看到展开的子菜单项。

---

## 3. 水平顶部导航（Horizontal）

水平模式用于页面顶部全局导航，支持亮色和暗色两种主题：

```xml
<!-- 亮色水平导航 -->
<atom:NavMenu Mode="Horizontal">
    <atom:NavMenuNode Header="Navigation One"
                      Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
    <atom:NavMenuNode Header="Navigation Two"
                      Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}"
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

<!-- 暗色水平导航 -->
<atom:NavMenu IsDarkStyle="True" Mode="Horizontal">
    <atom:NavMenuNode Header="Navigation One"
                      Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
    <atom:NavMenuNode Header="Navigation Two"
                      Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}"
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

**使用场景**：网站顶部全局导航栏，暗色版适合深色背景设计。

---

## 4. 默认展开与选中路径

通过 `DefaultOpenPaths` 和 `DefaultSelectedPath` 属性指定菜单的初始状态。需要为每个节点设置 `ItemKey` 属性：

```xml
<atom:NavMenu Mode="Inline"
              DefaultOpenPaths="{Binding DefaultOpenPaths}"
              DefaultSelectedPath="{Binding DefaultSelectedPath}">
    <atom:NavMenuNode Header="Navigation One"
                      Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}"
                      ItemKey="1" />
    <atom:NavMenuNode Header="Navigation Two"
                      Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}"
                      IsEnabled="False"
                      ItemKey="2" />
    <atom:NavMenuNode Header="Navigation Three - Submenu"
                      Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}"
                      ItemKey="3">
        <atom:NavMenuNode Header="Item 1" ItemKey="SubGroup1">
            <atom:NavMenuNode Header="Option 1" ItemKey="Option1" />
            <atom:NavMenuNode Header="Option 2" ItemKey="Option2" />
        </atom:NavMenuNode>
        <atom:NavMenuNode Header="Item 2" ItemKey="SubGroup2">
            <atom:NavMenuNode Header="Option 3" ItemKey="Option3" />
            <atom:NavMenuNode Header="Option 4" ItemKey="Option4" />
        </atom:NavMenuNode>
    </atom:NavMenuNode>
    <atom:NavMenuNode Header="Navigation Four" ItemKey="4" />
</atom:NavMenu>
```

```csharp
// ViewModel 中设置默认展开路径和选中路径
var defaultOpenPaths = new List<TreeNodePath>();
defaultOpenPaths.Add(new TreeNodePath("/3/SubGroup2"));
viewModel.DefaultOpenPaths = defaultOpenPaths;
viewModel.DefaultSelectedPath = new TreeNodePath("/3/SubGroup1/Option1");
```

路径格式说明：`/3/SubGroup1/Option1` 表示从根节点 ItemKey="3" → 子节点 ItemKey="SubGroup1" → 叶子节点 ItemKey="Option1"。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml` 中 "Default Opened paths" 示例。

---

## 5. 通过 SelectedItem 直接选中

除了路径方式，还可以直接设置 `SelectedItem` 属性为 `INavMenuNode` 对象。`SelectedItem` 的优先级高于 `DefaultSelectedPath`：

```xml
<atom:NavMenu Name="ItemsSourceDemoNavMenu"
              Mode="Inline"
              SelectedItem="{Binding DefaultSelectedNode}" />
```

```csharp
// Code-behind
var selectedNode = new NavMenuNode()
{
    Header  = "Option 4",
    ItemKey = "Option4",
    Icon    = new TwitterOutlined()
};

// 将节点添加到树形结构中后设置选中
ItemsSourceDemoNavMenu.SelectedItem = selectedNode;
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml` 中 "Generate NavMenuItem by ItemsSource" 示例。

---

## 6. 通过 ItemsSource 绑定数据

NavMenu 支持通过 `ItemsSource` 和 `ItemTemplate` 绑定数据模型，适合动态菜单场景：

```xml
<atom:NavMenu Name="InlineModeMenu" Mode="Inline">
    <atom:NavMenu.ItemTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}"
                          x:DataType="atom:IMenuItemData">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:NavMenu.ItemTemplate>
</atom:NavMenu>
```

```csharp
// Code-behind
private void InitInlineNavMenuNodes(MenuViewModel viewModel)
{
    var nodes = new List<INavMenuNode>();
    nodes.Add(new NavMenuNode()
    {
        Header  = "Navigation One",
        Icon    = new MailOutlined(),
        ItemKey = "1"
    });
    nodes.Add(new NavMenuNode()
    {
        Header  = "Navigation Two",
        Icon    = new AppstoreOutlined(),
        ItemKey = "2"
    });
    nodes.Add(new NavMenuNode()
    {
        Header  = "Navigation Three - Submenu",
        Icon    = new SettingOutlined(),
        ItemKey = "3",
        Children = [
            new NavMenuNode()
            {
                Header  = "Item 1",
                ItemKey = "SubGroup1",
                Children = [
                    new NavMenuNode() { Header = "Option 1", ItemKey = "Option1" },
                    new NavMenuNode() { Header = "Option 2", ItemKey = "Option2" }
                ]
            },
            new NavMenuNode()
            {
                Header  = "Item 2",
                ItemKey = "SubGroup2",
                Children = [
                    new NavMenuNode() { Header = "Option 3", ItemKey = "Option3" },
                    new NavMenuNode() { Header = "Option 4", ItemKey = "Option4" }
                ]
            }
        ]
    });
    nodes.Add(new NavMenuNode() { Header = "Navigation Four", ItemKey = "4" });

    viewModel.InlineNavMenuNodes = nodes;
}

// 绑定数据源
this.OneWayBind(ViewModel, vm => vm.InlineNavMenuNodes, v => v.InlineModeMenu.ItemsSource);
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml.cs`

---

## 7. 动态切换模式与主题

NavMenu 支持运行时动态切换 `Mode` 和 `IsDarkStyle`：

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
        <atom:NavMenuNode Header="Navigation One"
                          Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
        <atom:NavMenuNode Header="Navigation Three - Submenu"
                          Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}">
            <atom:NavMenuNode Header="Item 1">
                <atom:NavMenuNode Header="Option 1" />
                <atom:NavMenuNode Header="Option 2" />
            </atom:NavMenuNode>
        </atom:NavMenuNode>
    </atom:NavMenu>
</StackPanel>
```

**注意**：切换 `Mode` 时，NavMenu 会自动关闭所有已展开的子菜单并重新配置交互处理器。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml` 中 "Switch the menu type" 示例。

---

## 8. 禁用菜单项

通过 `IsEnabled="False"` 禁用单个菜单项：

```xml
<atom:NavMenu Mode="Inline" Width="300">
    <atom:NavMenuNode Header="Navigation One"
                      Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
    <atom:NavMenuNode Header="Navigation Two (Disabled)"
                      Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}"
                      IsEnabled="False" />
    <atom:NavMenuNode Header="Navigation Three"
                      Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}" />
</atom:NavMenu>
```

禁用的菜单项会使用 `ItemDisabledColor`（或 `DarkItemDisabledColor`）文字颜色，且不响应交互。

---

## 9. 事件处理

### 监听菜单项点击

```csharp
navMenu.NavMenuItemClick += (sender, args) =>
{
    var menuItem = args.NavMenuItem;
    Console.WriteLine($"Clicked: {menuItem.ItemKey}");
};
```

### 监听节点选中

```csharp
navMenu.NavMenuNodeSelected += (sender, args) =>
{
    var node = args.NavMenuNode;
    Console.WriteLine($"Selected: {node.Header}");
};
```

---

## 常见组合模式

### 带侧边导航的页面布局

```xml
<DockPanel>
    <!-- 侧边导航 -->
    <atom:NavMenu Mode="Inline" Width="256" DockPanel.Dock="Left">
        <atom:NavMenuNode Header="Dashboard" Icon="{antdicons:AntDesignIconProvider Kind=DashboardOutlined}" />
        <atom:NavMenuNode Header="Users" Icon="{antdicons:AntDesignIconProvider Kind=UserOutlined}">
            <atom:NavMenuNode Header="User List" />
            <atom:NavMenuNode Header="User Groups" />
        </atom:NavMenuNode>
        <atom:NavMenuNode Header="Settings" Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}" />
    </atom:NavMenu>

    <!-- 主内容区 -->
    <Border Padding="16">
        <atom:TextBlock Text="Main Content Area" />
    </Border>
</DockPanel>
```

### 顶部+侧边组合导航

```xml
<DockPanel>
    <!-- 顶部导航 -->
    <atom:NavMenu Mode="Horizontal" DockPanel.Dock="Top" IsDarkStyle="True">
        <atom:NavMenuNode Header="Home" />
        <atom:NavMenuNode Header="Products" />
        <atom:NavMenuNode Header="About" />
    </atom:NavMenu>

    <!-- 侧边导航 -->
    <atom:NavMenu Mode="Inline" Width="200" DockPanel.Dock="Left">
        <atom:NavMenuNode Header="Overview" />
        <atom:NavMenuNode Header="Details" />
    </atom:NavMenu>

    <!-- 主内容区 -->
    <Border Padding="16">
        <atom:TextBlock Text="Content" />
    </Border>
</DockPanel>
```
