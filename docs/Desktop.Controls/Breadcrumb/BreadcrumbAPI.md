# Breadcrumb API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 数据类型

### IBreadcrumbItemData（接口）

面包屑项数据模型接口，用于 MVVM 数据驱动模式。

| 属性 | 类型 | 说明 |
|---|---|---|
| `Icon` | `PathIcon?` | 面包屑项图标 |
| `Content` | `object?` | 面包屑项内容（文本） |
| `NavigateUri` | `Uri?` | 导航 URI（外部链接） |
| `Separator` | `object?` | 自定义分隔符 |
| `SeparatorTemplate` | `IDataTemplate?` | 分隔符数据模板 |
| `NavigateContext` | `object?` | 导航上下文数据 |

### BreadcrumbItemData（类）

`IBreadcrumbItemData` 的默认实现类，所有属性均可读写。

```csharp
public class BreadcrumbItemData : IBreadcrumbItemData
{
    public PathIcon? Icon { get; set; }
    public object? Content { get; set; }
    public Uri? NavigateUri { get; set; }
    public object? Separator { get; set; }
    public IDataTemplate? SeparatorTemplate { get; set; }
    public object? NavigateContext { get; set; }
}
```

### BreadcrumbNavigateEventArgs（类）

导航请求事件参数，继承自 `EventArgs`。

| 属性 | 类型 | 说明 |
|---|---|---|
| `BreadcrumbItem` | `BreadcrumbItem` | 触发导航请求的面包屑项实例 |

---

## Breadcrumb 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Separator` | `object?` | `"/"` | 全局分隔符内容，默认为 `/`，所有未单独设置分隔符的面包屑项共享此值 |
| `SeparatorTemplate` | `IDataTemplate?` | `null` | 分隔符数据模板，用于自定义分隔符的呈现方式 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（共享属性，通过 `AddOwner` 注册） |

### 继承自 Avalonia.Controls.ItemsControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Items` | `ItemCollection` | 空集合 | 子项集合，可直接在 AXAML 中添加 `BreadcrumbItem` |
| `ItemsSource` | `IEnumerable?` | `null` | 数据源绑定，支持 MVVM 数据驱动模式 |
| `ItemTemplate` | `IDataTemplate?` | `null` | 子项数据模板，控制数据如何呈现为 UI |
| `ItemCount` | `int` | `0` | 只读，返回当前子项数量 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `HorizontalAlignment` | `HorizontalAlignment` | `Left` | 水平对齐 |
| `VerticalAlignment` | `VerticalAlignment` | `Stretch` | 垂直对齐 |
| `Foreground` | `IBrush?` | 由主题控制 | 前景色（文本颜色） |

### 常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `Breadcrumb.DefaultSeparator` | `"/"` | 默认分隔符字符串 |

---

## Breadcrumb 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `NavigateRequest` | `EventHandler<BreadcrumbNavigateEventArgs>` | 当面包屑项被点击且设有 `NavigateContext` 时触发，用于应用层实现自定义路由跳转 |

---

## BreadcrumbItem 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Icon` | `PathIcon?` | `null` | 面包屑项图标，使用 Ant Design 图标集 |
| `NavigateContext` | `object?` | `null` | 导航上下文数据，设置后面包屑项可点击并触发 `NavigateRequest` 事件 |
| `NavigateUri` | `Uri?` | `null` | 导航 URI，设置后点击面包屑项会通过系统启动器打开该 URI |
| `Separator` | `object?` | 继承自父 `Breadcrumb` | 面包屑项独立分隔符，优先级高于 `Breadcrumb.Separator` |
| `SeparatorTemplate` | `IDataTemplate?` | 继承自父 `Breadcrumb` | 面包屑项独立分隔符模板 |

### 继承自 Avalonia.Controls.Button 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 面包屑项文本内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `Command` | `ICommand?` | `null` | 点击命令 |
| `CommandParameter` | `object?` | `null` | 命令参数 |
| `Foreground` | `IBrush?` | 由主题控制 | 文本颜色 |
| `Background` | `IBrush?` | `Transparent` | 背景色 |
| `Padding` | `Thickness` | 由 Token 控制 | 内间距 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 | 圆角半径 |

---

## BreadcrumbItem 伪类（Pseudo-Classes）

BreadcrumbItem 支持以下伪类，可在样式选择器中使用：

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:has-icon` | `BreadcrumbPseudoClass.HasIcon` | `Icon` 不为 null |
| `:is-last` | `BreadcrumbPseudoClass.IsLast` | 当前项是 Breadcrumb 中的最后一项 |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 面包屑项被按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点 |

---

## BreadcrumbItem 内部属性

以下属性为 `internal`，供框架内部使用，不建议外部直接访问：

| 属性名 | 类型 | 说明 |
|---|---|---|
| `IsLast` | `bool` | 是否为最后一项，由父 `Breadcrumb` 自动设置 |
| `IsNavigateResponsive` | `bool` | 是否可导航响应，当 `NavigateUri` 或 `NavigateContext` 不为 null 时自动为 `true` |
| `IsMotionEnabled` | `bool` | 是否启用过渡动画，自动继承父 `Breadcrumb` 的值 |

---

## 实现的接口

### IMotionAwareControl（Breadcrumb）

```csharp
public bool IsMotionEnabled { get; set; }
```

控制是否启用过渡动画。当为 `true` 时，BreadcrumbItem 的前景色和背景色变化会有平滑过渡效果。
