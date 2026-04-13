# Menu 使用文档

本文档介绍 AtomUI Menu 控件家族的使用方式，涵盖 `Menu`（顶部菜单栏）、`MenuItem`（菜单项）、`ContextMenu`（右键菜单）、`MenuSeparator`（分隔线）和 `MenuFlyout`（弹出菜单）。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml`

---

## 前置准备

### AXAML 命名空间

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

### C# 引用

```csharp
using AtomUI.Desktop.Controls;
```

---

## 1. 基本用法

最简单的菜单栏，包含顶层菜单项和嵌套子菜单。

> 📖 对应 Gallery：`MenuShowCase.axaml` → **"Basic"** ShowCaseItem

```xml
<atom:Menu>
    <atom:MenuItem Header="_File">
        <atom:MenuItem Header="New Text File" InputGesture="Ctrl+N" />
        <atom:MenuItem Header="New File" InputGesture="Ctrl+Alt+N" />
        <atom:MenuItem Header="New Window" InputGesture="Ctrl+Shift+N" />
    </atom:MenuItem>
    <atom:MenuItem Header="_Edit">
        <atom:MenuItem Header="Undo" InputGesture="Ctrl+Shift+Z" />
        <atom:MenuSeparator />
        <atom:MenuItem Header="Cut" InputGesture="Ctrl+X" />
    </atom:MenuItem>
    <atom:MenuItem Header="Disabled Item" IsEnabled="False" />
</atom:Menu>
```

**要点：**
- `Header` 中的 `_` 前缀字符用于定义键盘访问键（Access Key）
- `InputGesture` 用于设置快捷键提示文本
- `MenuSeparator` 用于在菜单项之间插入分隔线
- `IsEnabled="False"` 设置禁用状态

---

## 2. 通过 ItemsSource 数据驱动生成菜单

使用数据模型和 `ItemTemplate` 动态生成菜单结构，适用于从后端数据或配置动态构建菜单。

> 📖 对应 Gallery：`MenuShowCase.axaml` → **"Generate MenuItem by ItemsSource"** ShowCaseItem

### AXAML

```xml
<atom:Menu Name="BasicItemsSourceMenu">
    <atom:Menu.ItemTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}" 
                          x:DataType="atom:IMenuItemData">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:Menu.ItemTemplate>
</atom:Menu>
```

### ViewModel 数据构建

```csharp
using AtomUI.Desktop.Controls;

// 构建菜单数据
var menuItems = new List<IMenuItemData>
{
    new MenuItemData
    {
        Header = "File",
        Children = new List<IMenuItemData>
        {
            new MenuItemData { Header = "New Text File" },
            new MenuItemData { Header = "New File" },
            new MenuSeparatorData(),  // 分隔线
            new MenuItemData { Header = "Save" },
        }
    },
    new MenuItemData
    {
        Header = "Edit",
        Children = new List<IMenuItemData>
        {
            new MenuItemData { Header = "Undo" },
            new MenuItemData { Header = "Cut", IsEnabled = false },
        }
    }
};

// 绑定到 Menu
BasicItemsSourceMenu.ItemsSource = menuItems;
```

**要点：**
- 使用 `TreeDataTemplate` 以支持嵌套子菜单
- `IMenuItemData.Children` 用于定义子菜单树形结构
- 使用 `MenuSeparatorData` 类型的数据项自动生成 `MenuSeparator`
- `MenuItemData` 自动维护 `ParentNode` 父节点引用

---

## 3. 带图标和子菜单

菜单项可以包含 Ant Design 图标。

> 📖 对应 Gallery：`MenuShowCase.axaml` → **"Icon and submenu"** ShowCaseItem

```xml
<atom:Menu>
    <atom:MenuItem Header="_File">
        <atom:MenuItem Header="New Text File" InputGesture="Ctrl+N" />
        <atom:MenuItem Header="New File" InputGesture="Ctrl+Alt+N" />
        <atom:MenuItem Header="New Window" InputGesture="Ctrl+Shift+N" />
        <atom:MenuSeparator />
        <atom:MenuItem Header="Save" InputGesture="Ctrl+S" />
        <atom:MenuItem Header="Save As..." InputGesture="Ctrl+Shift+S" />
        <atom:MenuItem Header="Save All" InputGesture="Ctrl+K" />
        <atom:MenuSeparator />
        <atom:MenuItem Header="Exit" />
    </atom:MenuItem>
    <atom:MenuItem Header="_Edit">
        <atom:MenuItem Header="Undo" InputGesture="Ctrl+Shift+Z" />
        <atom:MenuSeparator />
        <atom:MenuItem Header="Cut" InputGesture="Ctrl+X"
                       Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />
        <atom:MenuItem Header="Copy" InputGesture="Ctrl+C"
                       Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
        <atom:MenuItem Header="Delete" InputGesture="Ctrl+D"
                       Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
        <atom:MenuItem Header="Paste">
            <atom:MenuItem Header="Paste" InputGesture="Ctrl+P"
                           Icon="{antdicons:AntDesignIconProvider Kind=FileDoneOutlined}" />
            <atom:MenuItem Header="Paste from History" InputGesture="Ctrl+Shift+V" />
        </atom:MenuItem>
    </atom:MenuItem>
</atom:Menu>
```

**要点：**
- 使用 `antdicons:AntDesignIconProvider` 标记扩展设置 Ant Design 图标
- 支持多级嵌套子菜单（如 Paste → Paste/Paste from History）
- 图标与文字间距由 `MenuToken.ItemIconMarginInlineEnd` Token 控制

---

## 4. CheckBox / Radio 切换菜单项

菜单项支持 CheckBox 和 Radio 切换交互。

> 📖 对应 Gallery：`MenuShowCase.axaml` → **"menu item with ToggleType"** ShowCaseItem

```xml
<atom:Menu>
    <atom:MenuItem Header="_Menu A">
        <!-- Radio 类型 — 同组互斥 -->
        <atom:MenuItem Header="New Text File" InputGesture="Ctrl+N"
                       ToggleType="Radio" GroupName="Group1" />
        <atom:MenuItem Header="New File" InputGesture="Ctrl+Alt+N"
                       ToggleType="Radio" GroupName="Group1" />
        <atom:MenuItem Header="New Window" InputGesture="Ctrl+Shift+N"
                       ToggleType="Radio" GroupName="Group1" />
        <atom:MenuSeparator />
        <!-- CheckBox 类型 — 独立选中 -->
        <atom:MenuItem Header="Save" InputGesture="Ctrl+S"
                       ToggleType="CheckBox" />
        <atom:MenuItem Header="Save As..." InputGesture="Ctrl+Shift+S"
                       ToggleType="CheckBox"
                       Icon="{antdicons:AntDesignIconProvider Kind=GithubOutlined}" />
        <atom:MenuItem Header="Save All" InputGesture="Ctrl+K"
                       ToggleType="CheckBox"
                       Icon="{antdicons:AntDesignIconProvider Kind=CheckOutlined}" />
        <atom:MenuSeparator />
        <atom:MenuItem Header="Exit" />
        <atom:MenuItem Header="Disabled" IsEnabled="False"
                       Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
    </atom:MenuItem>
</atom:Menu>
```

**要点：**
- `ToggleType="Radio"` + `GroupName` 实现同组互斥选择
- `ToggleType="CheckBox"` 实现独立选中/取消
- 可同时设置图标和 ToggleType
- 通过 `IsCheckStateChanged` 事件监听选中状态变化

### 监听选中变化

```csharp
menuItem.IsCheckStateChanged += (sender, e) =>
{
    var item = (MenuItem)sender!;
    Console.WriteLine($"{item.Header}: IsChecked = {item.IsChecked}");
};
```

---

## 5. 可滚动菜单

当子菜单项过多时，弹窗会自动出现滚动条。`DisplayPageSize` 控制可见项数量。

> 📖 对应 Gallery：`MenuShowCase.axaml` → **"Scrollable menu"** ShowCaseItem

```xml
<atom:Menu>
    <atom:MenuItem Header="_Menu">
        <atom:MenuItem Header="Menu Item" />
        <atom:MenuItem Header="Menu Item" />
        <!-- ... 大量菜单项 -->
        <atom:MenuItem Header="Menu Item" />
    </atom:MenuItem>
</atom:Menu>
```

**要点：**
- 默认 `DisplayPageSize=10`，即弹窗最多显示 10 项，超出后出现滚动条
- 可以通过设置 `DisplayPageSize` 调整可见项数量

---

## 6. 右键上下文菜单（ContextMenu）

通过 `ContextMenu` 属性附加到任意控件，右键点击触发弹出。

> 📖 对应 Gallery：`MenuShowCase.axaml` → **"Context menu"** ShowCaseItem

```xml
<Border>
    <Border.ContextMenu>
        <atom:ContextMenu>
            <atom:MenuItem Header="Cut" InputGesture="Ctrl+X"
                           Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />
            <atom:MenuItem Header="Copy" InputGesture="Ctrl+C"
                           Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
            <atom:MenuItem Header="Delete" InputGesture="Ctrl+D"
                           Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
            <atom:MenuItem Header="Paste">
                <atom:MenuItem Header="Paste" InputGesture="Ctrl+P"
                               Icon="{antdicons:AntDesignIconProvider Kind=FileDoneOutlined}" />
                <atom:MenuItem Header="Paste from History" InputGesture="Ctrl+Shift+V" />
            </atom:MenuItem>
        </atom:ContextMenu>
    </Border.ContextMenu>
    <atom:TextBlock Text="Right Click to show Context Menu" />
</Border>
```

### 数据驱动的 ContextMenu

> 📖 对应 Gallery：`MenuShowCase.axaml` → **"Context menu, Generate MenuItem by ItemsSource"** ShowCaseItem

```xml
<Border>
    <Border.ContextMenu>
        <atom:ContextMenu Name="BasicContextMenu">
            <atom:ContextMenu.ItemTemplate>
                <TreeDataTemplate ItemsSource="{Binding Children}"
                                  x:DataType="atom:IMenuItemData">
                    <atom:TextBlock Text="{Binding Header}" />
                </TreeDataTemplate>
            </atom:ContextMenu.ItemTemplate>
        </atom:ContextMenu>
    </Border.ContextMenu>
    <atom:TextBlock Text="Right Click to show Context Menu" />
</Border>
```

---

## 7. MenuFlyout 弹出菜单

`MenuFlyout` 可作为 `ContextFlyout` 使用，通过右键触发弹出。

> 📖 对应 Gallery：`MenuShowCase.axaml` → **"Menu Flyout"** ShowCaseItem

```xml
<Border>
    <Border.ContextFlyout>
        <atom:MenuFlyout IsMotionEnabled="True"
                         atom:Flyout.HorizontalOffset="10"
                         atom:Flyout.VerticalOffset="15">
            <atom:MenuItem Header="Cut" InputGesture="Ctrl+X"
                           Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />
            <atom:MenuItem Header="Copy" InputGesture="Ctrl+C"
                           Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
            <atom:MenuItem Header="Delete" InputGesture="Ctrl+D"
                           Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
            <atom:MenuItem Header="Paste">
                <atom:MenuItem Header="Paste" InputGesture="Ctrl+P"
                               Icon="{antdicons:AntDesignIconProvider Kind=FileDoneOutlined}" />
                <atom:MenuItem Header="Paste from History" InputGesture="Ctrl+Shift+V" />
            </atom:MenuItem>
        </atom:MenuFlyout>
    </Border.ContextFlyout>
    <atom:TextBlock Text="Right Click to show Context Flyout" />
</Border>
```

### 数据驱动的 MenuFlyout

```xml
<Border>
    <Border.ContextFlyout>
        <atom:MenuFlyout IsMotionEnabled="True"
                         ItemsSource="{Binding MenuFlyoutItems}"
                         atom:Flyout.HorizontalOffset="10"
                         atom:Flyout.VerticalOffset="15">
            <atom:MenuFlyout.ItemTemplate>
                <TreeDataTemplate ItemsSource="{Binding Children}"
                                  x:DataType="atom:IMenuItemData">
                    <atom:TextBlock Text="{Binding Header}" />
                </TreeDataTemplate>
            </atom:MenuFlyout.ItemTemplate>
        </atom:MenuFlyout>
    </Border.ContextFlyout>
    <atom:TextBlock Text="Right Click to show Context Flyout" />
</Border>
```

---

## 8. 尺寸变体

Menu 支持三种尺寸：`Small`、`Middle`（默认）、`Large`。尺寸影响菜单栏的最小高度、顶层菜单项的内间距和字体大小。

```xml
<!-- 大号菜单 -->
<atom:Menu SizeType="Large">
    <atom:MenuItem Header="File" />
    <atom:MenuItem Header="Edit" />
</atom:Menu>

<!-- 小号菜单 -->
<atom:Menu SizeType="Small">
    <atom:MenuItem Header="File" />
    <atom:MenuItem Header="Edit" />
</atom:Menu>
```

**尺寸影响的 Token 对应关系：**

| SizeType | 最小高度 | 顶层圆角 | 顶层内间距 | 顶层字体 |
|---|---|---|---|---|
| `Large` | `ControlHeightLG` | `TopLevelItemBorderRadiusLG` | `TopLevelItemPaddingLG` | `TopLevelItemFontSizeLG` |
| `Middle` | `ControlHeight` | `TopLevelItemBorderRadius` | `TopLevelItemPadding` | `TopLevelItemFontSize` |
| `Small` | `ControlHeightSM` | `TopLevelItemBorderRadiusSM` | `TopLevelItemPaddingSM` | `TopLevelItemFontSizeSM` |

---

## 9. 动画控制

通过 `IsMotionEnabled` 属性控制子菜单弹出/关闭的动画效果。默认跟随全局 Token `EnableMotion`。

```xml
<!-- 关闭动画 -->
<atom:Menu IsMotionEnabled="False">
    <atom:MenuItem Header="File">
        <atom:MenuItem Header="New" />
    </atom:MenuItem>
</atom:Menu>
```

---

## 常见场景

### 应用顶部菜单栏

```xml
<DockPanel>
    <atom:Menu DockPanel.Dock="Top">
        <atom:MenuItem Header="_File">
            <atom:MenuItem Header="New" InputGesture="Ctrl+N" />
            <atom:MenuItem Header="Open" InputGesture="Ctrl+O" />
            <atom:MenuSeparator />
            <atom:MenuItem Header="Exit" />
        </atom:MenuItem>
        <atom:MenuItem Header="_Edit">
            <atom:MenuItem Header="Undo" InputGesture="Ctrl+Z" />
            <atom:MenuItem Header="Redo" InputGesture="Ctrl+Y" />
        </atom:MenuItem>
        <atom:MenuItem Header="_Help">
            <atom:MenuItem Header="About" />
        </atom:MenuItem>
    </atom:Menu>
    <!-- 主内容区域 -->
</DockPanel>
```

### 右键上下文菜单 + Command 绑定

```xml
<atom:ContextMenu>
    <atom:MenuItem Header="Copy"
                   Command="{Binding CopyCommand}"
                   InputGesture="Ctrl+C"
                   Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
    <atom:MenuItem Header="Paste"
                   Command="{Binding PasteCommand}"
                   InputGesture="Ctrl+V" />
</atom:ContextMenu>
```
