# Menu API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## Menu 类

### 继承关系

```
Avalonia.Controls.Menu → AtomUI.Desktop.Controls.Menu
```

### 实现的接口

| 接口 | 说明 |
|---|---|
| `ISizeTypeAware` | 支持 Small/Middle/Large 三种尺寸 |
| `IMotionAwareControl` | 支持开启/关闭弹出动画 |

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `Middle` | 尺寸类型（共享属性，通过 `AddOwner` 注册） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用子菜单弹出动画 |
| `DisplayPageSize` | `int` | `10` | 子菜单弹窗中显示的最大菜单项数量（影响弹窗最大高度） |
| `ShouldUseOverlayLayer` | `bool` | `false` | 是否使用 OverlayLayer 作为 Popup 宿主 |

#### 继承自 Avalonia `Menu` 的关键属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `IsOpen` | `bool` | 菜单是否处于打开状态 |
| `ItemsSource` | `IEnumerable?` | 数据源绑定 |
| `ItemTemplate` | `IDataTemplate?` | 数据模板 |
| `ItemContainerTheme` | `ControlTheme?` | 菜单项容器主题 |

### 公共方法

| 方法名 | 返回类型 | 说明 |
|---|---|---|
| `Close()` | `void` | 关闭菜单及所有子菜单（重写，支持动画关闭） |

### 继承事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Opened` | `EventHandler<RoutedEventArgs>` | 菜单打开时触发 |
| `Closed` | `EventHandler<RoutedEventArgs>` | 菜单关闭时触发 |

### 虚方法（可扩展）

| 方法名 | 说明 |
|---|---|
| `PrepareMenuItem(MenuItem, object?, int)` | 准备菜单项容器时的扩展点，子类可重写以自定义菜单项初始化 |

---

## MenuItem 类

### 继承关系

```
Avalonia.Controls.MenuItem → AtomUI.Desktop.Controls.MenuItem
```

### 实现的接口

| 接口 | 说明 |
|---|---|
| `IMenuItemData` | 菜单项数据接口（含 `ItemKey`、`Children` 等树结构信息） |

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Icon` | `PathIcon?` | `null` | 菜单项图标（`new` 重写基类，使用 `PathIcon` 而非 Avalonia `Icon`） |
| `SizeType` | `SizeType` | `Middle` | 尺寸类型（共享属性） |
| `DisplayPageSize` | `int` | `10` | 子菜单弹窗中显示的最大菜单项数量 |

### 公共非样式属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `ItemKey` | `EntityKey?` | 菜单项唯一标识键 |
| `ParentNode` | `ITreeNode<IMenuItemData>?` | 父节点引用（`ITreeNode` 接口实现） |

#### 继承自 Avalonia `MenuItem` 的关键属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Header` | `object?` | 菜单项标题内容 |
| `HeaderTemplate` | `IDataTemplate?` | 标题内容模板 |
| `InputGesture` | `KeyGesture?` | 快捷键手势（右侧显示快捷键提示） |
| `Command` | `ICommand?` | 点击命令 |
| `CommandParameter` | `object?` | 命令参数 |
| `IsSubMenuOpen` | `bool` | 子菜单是否打开 |
| `ToggleType` | `MenuItemToggleType` | 切换类型（None/CheckBox/Radio） |
| `IsChecked` | `bool` | 是否选中（配合 `ToggleType` 使用） |
| `GroupName` | `string?` | Radio 分组名（配合 `ToggleType=Radio` 使用） |
| `IsEnabled` | `bool` | 是否启用 |

### 公共事件

| 事件名 | 类型 | 路由策略 | 说明 |
|---|---|---|---|
| `IsCheckStateChanged` | `EventHandler<RoutedEventArgs>` | Bubble | `IsChecked` 状态变化时触发 |

#### 继承自 Avalonia `MenuItem` 的关键事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Click` | `EventHandler<RoutedEventArgs>` | 菜单项被点击时触发 |
| `SubmenuOpened` | `EventHandler<RoutedEventArgs>` | 子菜单打开时触发 |

### 公共方法

| 方法名 | 返回类型 | 说明 |
|---|---|---|
| `CloseItemAsync()` | `Task` | 异步关闭子菜单项（支持动画） |

### 虚方法（可扩展）

| 方法名 | 说明 |
|---|---|
| `PrepareMenuItem(MenuItem, object?, int)` | 准备子菜单项容器时的扩展点 |

### 伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:toplevel` | `MenuItemPseudoClass.TopLevel` | 当前 MenuItem 为顶层菜单项（直接嵌套在 `Menu` 中） |

#### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |
| `:pressed` | 按下状态 |
| `:checked` | `IsChecked == true` |
| `:empty` | 没有子菜单项 |

---

## ContextMenu 类

### 继承关系

```
Avalonia.Controls.ContextMenu → AtomUI.Desktop.Controls.ContextMenu
```

### 实现的接口

| 接口 | 说明 |
|---|---|
| `ISizeTypeAware` | 支持 Small/Middle/Large 三种尺寸 |
| `IMotionAwareControl` | 支持开启/关闭弹出动画 |

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `Middle` | 尺寸类型 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用弹出动画 |
| `DisplayPageSize` | `int` | `10` | 弹窗中显示的最大菜单项数量 |
| `ShouldUseOverlayLayer` | `bool` | `false` | 是否使用 OverlayLayer 作为 Popup 宿主 |
| `MaskShadows` | `BoxShadows` | 默认 | 弹窗阴影效果 |

#### 继承自 Avalonia `ContextMenu` 的关键属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `IsOpen` | `bool` | 上下文菜单是否打开 |
| `HorizontalOffset` | `double` | 水平偏移量 |
| `VerticalOffset` | `double` | 垂直偏移量 |
| `Placement` | `PlacementMode` | 弹出位置 |
| `ItemsSource` | `IEnumerable?` | 数据源 |
| `ItemTemplate` | `IDataTemplate?` | 数据模板 |

### 公共方法

| 方法名 | 返回类型 | 说明 |
|---|---|---|
| `Close()` | `void` | 关闭上下文菜单（重写，支持动画关闭） |

### 继承事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Opening` | `EventHandler<CancelEventArgs>` | 菜单打开前触发，可取消 |
| `Closing` | `EventHandler<CancelEventArgs>` | 菜单关闭前触发，可取消 |
| `Opened` | `EventHandler` | 菜单已打开 |

### 虚方法（可扩展）

| 方法名 | 说明 |
|---|---|
| `PrepareMenuItem(MenuItem, object?, int)` | 准备菜单项容器时的扩展点 |

---

## MenuSeparator 类

### 继承关系

```
Avalonia.Controls.Separator → AtomUI.Desktop.Controls.MenuSeparator
```

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Orientation` | `Orientation` | `Horizontal` | 分隔线方向 |
| `LineWidth` | `double` | `1` | 线条宽度（像素） |

---

## 数据类型

### IMenuItemData 接口

菜单项数据接口，继承自 `ITreeNode<IMenuItemData>`。

```csharp
public interface IMenuItemData : ITreeNode<IMenuItemData>
{
    KeyGesture? InputGesture { get; }
}
```

### MenuItemData 类

`IMenuItemData` 的默认实现。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ParentNode` | `ITreeNode<IMenuItemData>?` | `null` | 父节点引用（`internal set`） |
| `ItemKey` | `EntityKey?` | `null` | 唯一标识键 |
| `Header` | `object?` | `null` | 显示标题 |
| `Icon` | `PathIcon?` | `null` | 图标 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `InputGesture` | `KeyGesture?` | `null` | 快捷键手势 |
| `Children` | `IList<IMenuItemData>` | `[]` | 子节点列表（设置时自动维护 `ParentNode` 关系） |

### MenuSeparatorData 类

分隔线数据模型，继承自 `MenuItemData`。当 `ItemsSource` 中的数据项类型为 `MenuSeparatorData` 时，容器自动创建为 `MenuSeparator`。

```csharp
public class MenuSeparatorData : MenuItemData { }
```

---

## 枚举类型

### SizeType（来自 `AtomUI.Controls`）

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

### MenuItemToggleType（来自 Avalonia）

| 值 | 说明 |
|---|---|
| `None` | 无切换（默认） |
| `CheckBox` | 复选框模式 |
| `Radio` | 单选框模式 |

---

## MenuItemPseudoClass 常量

```csharp
public static class MenuItemPseudoClass
{
    public const string TopLevel = ":toplevel";
}
```
