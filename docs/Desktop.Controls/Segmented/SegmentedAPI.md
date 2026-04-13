# Segmented API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;      // Segmented, SegmentedItem
namespace AtomUI.Controls.Commons;       // AbstractSegmented, AbstractSegmentedItem
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号（高度约 24px） |
| `Middle` | 中号（默认，高度约 32px） |
| `Large` | 大号（高度约 40px） |

---

## Segmented

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `SizeType.Middle` | 尺寸类型，影响容器圆角、选项高度和字体大小（共享属性，通过 `AddOwner` 注册） |
| `IsExpanding` | `bool` | `false` | Block 模式：为 `true` 时容器撑满父元素宽度，选项均分 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用过渡动画（色块滑动、选项背景色过渡） |

### 继承自 Avalonia.Controls.SelectingItemsControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Items` | `ItemCollection` | — | 子项集合（可添加 `SegmentedItem` 或数据对象） |
| `ItemsSource` | `IEnumerable?` | `null` | 数据绑定源 |
| `ItemTemplate` | `IDataTemplate?` | 内置 `TextBlock` 模板 | 数据项的展示模板 |
| `SelectedIndex` | `int` | `0` | 当前选中项索引（默认选中第一项） |
| `SelectedItem` | `object?` | — | 当前选中的数据对象 |
| `Background` | `IBrush?` | `TrackBg` Token | 容器背景色（轨道背景色） |
| `Padding` | `Thickness` | `TrackPadding` Token | 容器内间距 |
| `CornerRadius` | `CornerRadius` | 由 SizeType 决定 | 容器圆角 |
| `HorizontalAlignment` | `HorizontalAlignment` | `Left`（Block 模式为 `Stretch`） | 水平对齐方式 |
| `VerticalAlignment` | `VerticalAlignment` | `Top` | 垂直对齐方式 |

---

## SegmentedItem

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsSelected` | `bool` | `false` | 是否被选中（通过 `SelectingItemsControl.IsSelectedProperty.AddOwner` 注册） |
| `Icon` | `PathIcon?` | `null` | 选项图标，使用 Ant Design 图标集 |

### 继承自 Avalonia.Controls.ContentControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 选项文本内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `IsEnabled` | `bool` | `true` | 是否启用（可单独禁用某些选项） |
| `Foreground` | `IBrush?` | 由 Token 和状态控制 | 文本颜色 |
| `Background` | `IBrush?` | 由 Token 和状态控制 | 选项背景色 |
| `CornerRadius` | `CornerRadius` | 由 SizeType 决定 | 选项圆角 |
| `Padding` | `Thickness` | 由 SizeType 决定 | 选项内间距 |
| `FontSize` | `double` | 由 SizeType 决定 | 字体大小 |
| `MinHeight` | `double` | 由 SizeType 决定 | 最小高度 |
| `Cursor` | `Cursor` | `Hand`（启用时） | 鼠标光标 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `SelectionChanged` | `EventHandler<SelectionChangedEventArgs>` | 选中项变化事件（继承自 `SelectingItemsControl`） |

---

## 伪类（Pseudo-Classes）

### SegmentedItem 伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:selected` | `StdPseudoClass.Selected` | `IsSelected == true` |
| `:pressed` | `StdPseudoClass.Pressed` | 鼠标按下（通过 PressedMixin） |
| `:has-icon` | `SegmentedPseudoClass.HasIcon` | `Icon != null` |
| `:disabled` | — | `IsEnabled == false` |
| `:pointerover` | — | 鼠标悬浮 |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

支持三种尺寸（Small / Middle / Large）切换，影响容器和选项的圆角、高度、字号和间距。

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制色块滑动动画和选项背景色过渡是否启用。

### IFormItemAware

可作为 `FormItem` 的子控件参与表单验证。Segmented 通过 `SelectedItem` 作为表单值。

### ISelectable（SegmentedItem）

```csharp
public bool IsSelected { get; set; }
```

参与 `SelectingItemsControl` 的选择管理逻辑。
