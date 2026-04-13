# NavMenu API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### NavMenuMode

菜单模式枚举，定义菜单的展示方式。

| 值 | 说明 |
|---|---|
| `Vertical` | 垂直模式，子菜单通过 Popup 弹出 |
| `Horizontal` | 水平模式，菜单项水平排列，子菜单通过 Popup 弹出 |
| `Inline` | 内嵌模式，子菜单在当前面板内展开/折叠 |

---

## NavMenu 公共属性（StyledProperty / DirectProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Mode` | `NavMenuMode` | `NavMenuMode.Inline` | 菜单模式（水平/垂直/内嵌） |
| `IsDarkStyle` | `bool` | `false` | 是否使用暗色主题样式 |
| `IsAccordionMode` | `bool` | `false` | 手风琴模式，同级只允许一个子菜单展开 |
| `SelectedItem` | `INavMenuNode?` | `null` | 当前选中的菜单节点（双向绑定，`BindingMode.TwoWay`） |
| `DefaultOpenPaths` | `IList<TreeNodePath>?` | `null` | 初始展开的路径列表，路径格式为 `/Key1/Key2` |
| `DefaultSelectedPath` | `TreeNodePath?` | `null` | 初始选中的菜单项路径 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用动画效果（共享属性，通过 `AddOwner` 注册） |
| `ShouldUseOverlayLayer` | `bool` | `false` | 是否使用 OverlayLayer 弹出子菜单 |

### 继承自 Avalonia.Controls.ItemsControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ItemsSource` | `IEnumerable?` | `null` | 数据源绑定（绑定 `INavMenuNode` 集合） |
| `ItemTemplate` | `IDataTemplate?` | `null` | 子项模板（使用 `TreeDataTemplate` 支持层级结构） |
| `ItemsPanel` | `ITemplate<Panel?>` | 垂直 `StackPanel` | 子项面板模板（水平模式下自动切换为水平布局） |
| `ItemCount` | `int` | `0` | 子项数量（只读） |
| `IsEnabled` | `bool` | `true` | 是否启用 |

---

## NavMenu 事件

| 事件名 | 类型 | 路由策略 | 说明 |
|---|---|---|---|
| `NavMenuItemClick` | `EventHandler<NavMenuItemClickEventArgs>` | Bubble | 任意菜单项被点击时触发 |
| `NavMenuNodeSelected` | `EventHandler<NavMenuNodeSelectedEventArgs>` | Bubble | 叶子菜单节点被选中时触发 |

### NavMenuItemClickEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `NavMenuItem` | `INavMenuItem` | 被点击的菜单项接口引用 |

### NavMenuNodeSelectedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `NavMenuNode` | `INavMenuNode` | 被选中的菜单节点数据 |

---

## NavMenu 伪类（Pseudo-Classes）

NavMenu 支持以下伪类，可在样式选择器中使用：

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:inline-mode` | `NavMenuPseudoClass.InlineMode` | `Mode == NavMenuMode.Inline` |
| `:horizontal-mode` | `NavMenuPseudoClass.HorizontalMode` | `Mode == NavMenuMode.Horizontal` |
| `:vertical-mode` | `NavMenuPseudoClass.VerticalMode` | `Mode == NavMenuMode.Vertical` |
| `:dark` | `NavMenuPseudoClass.DarkStyle` | `IsDarkStyle == true` |
| `:light` | `NavMenuPseudoClass.LightStyle` | `IsDarkStyle == false` |

### NavMenuItem 伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:toplevel` | `NavMenuItemPseudoClass.TopLevel` | 菜单项为 NavMenu 的直接子项 |
| `:separator` | `NavMenuItemPseudoClass.Separator` | 菜单项为分隔符 |
| `:icon` | `NavMenuItemPseudoClass.Icon` | 菜单项有图标 |
| `:open` | `StdPseudoClass.Open` | 子菜单处于展开状态 |
| `:pressed` | `StdPseudoClass.Pressed` | 菜单项被按下 |
| `:selected` | `StdPseudoClass.Selected` | 菜单项被选中 |

---

## NavMenu 公共方法

| 方法 | 返回值 | 说明 |
|---|---|---|
| `Close()` | `void` | 关闭所有子菜单并清除 `SelectedItem` |

---

## NavMenuNode 数据模型

`NavMenuNode` 是 `INavMenuNode` 的默认实现，继承自 `AvaloniaObject`，可直接在 AXAML 中声明使用。`Children` 属性标记了 `[Content]` 特性，支持 AXAML 内容语法。

### NavMenuNode 属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Header` | `object?` | `null` | 菜单项标题内容（文本或自定义控件） |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 标题模板（覆盖 NavMenu 的全局 ItemTemplate） |
| `ItemKey` | `EntityKey?` | `null` | 节点唯一标识键（用于 `TreeNodePath` 路径定位） |
| `Icon` | `PathIcon?` | `null` | 菜单项图标（使用 Ant Design 图标集） |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Children` | `IList<INavMenuNode>` | 空 `AvaloniaList` | 子节点集合（`[Content]` 属性） |
| `ParentNode` | `ITreeNode<INavMenuNode>?` | `null` | 父节点引用（只读，添加到 Children 时自动设置） |

---

## INavMenuNode 接口

```csharp
public interface INavMenuNode : ITreeNode<INavMenuNode>
{
    IDataTemplate? HeaderTemplate { get; }
    void UpdateParentNode(INavMenuNode? parentNode);
}
```

---

## INavMenu 接口

```csharp
public interface INavMenu : INavMenuElement
{
    INavMenuNode? SelectedItem { get; }
    IRenderRoot? VisualRoot { get; }
    IList<TreeNodePath>? DefaultOpenPaths { get; set; }
    TreeNodePath? DefaultSelectedPath { get; set; }
    void Close();
}
```

---

## INavMenuItem 接口

```csharp
public interface INavMenuItem : INavMenuElement
{
    EntityKey? ItemKey { get; }
    INavMenuNode? Node { get; }
    bool HasSubMenu { get; }
    bool IsPointerOverSubMenu { get; }
    bool IsSubMenuOpen { get; set; }
    bool StaysOpenOnClick { get; set; }
    bool IsTopLevel { get; }
    INavMenuElement? Parent { get; }
    void Open();
    void Close();
}
```

---

## INavMenuElement 接口

```csharp
public interface INavMenuElement : IInputElement, ILogical
{
    IEnumerable<INavMenuItem> SubItems { get; }
}
```

---

## NavNodeKey 结构体

菜单节点键值结构体，用于在 `TreeNodePath` 中标识节点。

```csharp
public readonly struct NavNodeKey : IEquatable<NavNodeKey>
{
    public NavNodeKey(string value);
    public string Value { get; }
}
```

支持与 `string` 类型的相等性比较（`==` / `!=`）。

---

## TreeNodePath

树节点路径类型，格式为 `/Key1/Key2/Key3`，用于指定默认展开/选中的路径。

```csharp
// 使用方式
var path = new TreeNodePath("/3/SubGroup2/Option3");
```

路径中的每一段对应一个 `NavMenuNode` 的 `ItemKey` 值。

---

## NavMenu 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制 NavMenu 及其子项的动画效果。在内嵌模式下影响子菜单展开/折叠动画，在所有模式下影响菜单项头部的背景/前景色过渡动画。

### IFocusScope

NavMenu 作为焦点域（Focus Scope），其子项可以独立管理焦点。

---

## BaseNavMenuItemHeader 属性

`BaseNavMenuItemHeader` 是所有模式菜单项头部的基类，承载通用属性和动画过渡逻辑。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Header` | `object?` | `null` | 菜单项标题（从 NavMenuItem 绑定） |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 标题模板 |
| `Icon` | `PathIcon?` | `null` | 菜单项图标 |
| `IsMotionEnabled` | `bool` | 跟随父级 | 是否启用过渡动画 |
| `HasSubMenu` | `bool` | `false` | 是否有子菜单 |
| `IsSubMenuOpen` | `bool` | `false` | 子菜单是否展开 |
| `IsDarkStyle` | `bool` | `false` | 是否暗色模式 |
| `IsSelected` | `bool` | `false` | 是否被选中 |
| `IsInSelectedPath` | `bool` | `false` | 是否在选中路径上 |
