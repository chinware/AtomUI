# HyperLinkButton API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;        // HyperLinkButton
namespace AtomUI.Controls.Commons;         // AbstractHyperLinkButton (基类)
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## 公共属性（StyledProperty）

以下属性定义在基类 `AbstractHyperLinkButton` 中，`HyperLinkButton` 完整继承。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `NavigateUri` | `Uri?` | `null` | 点击后导航的目标 URI，设置后点击会自动打开系统浏览器 |
| `IsVisited` | `bool` | `false` | 是否已访问，导航成功后自动设为 `true`，也可手动设置 |
| `IsDanger` | `bool` | `false` | 是否为危险链接，启用后使用红色系颜色 |
| `IsGhost` | `bool` | `false` | 幽灵模式，背景透明 |
| `IsLoading` | `bool` | `false` | 加载中状态，显示旋转加载图标并降低不透明度 |
| `SizeType` | `SizeType` | `SizeType.Middle` | 按钮尺寸（共享属性，通过 `AddOwner` 注册） |
| `Icon` | `PathIcon?` | `null` | 按钮图标，使用 Ant Design 图标集 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token（`EnableMotion`） | 是否启用过渡动画（共享属性） |

### 继承自 Avalonia.Controls.Button 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 按钮文本内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `Command` | `ICommand?` | `null` | 点击命令 |
| `CommandParameter` | `object?` | `null` | 命令参数 |
| `HotKey` | `KeyGesture?` | `null` | 键盘快捷键 |
| `ClickMode` | `ClickMode` | `Release` | 点击触发时机 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Foreground` | `IBrush?` | 由主题控制（`ColorLink`） | 前景色（文本颜色） |
| `Background` | `IBrush?` | 透明 | 背景色（始终为透明） |
| `FontSize` | `double` | 由 Token 控制 | 字体大小 |
| `Padding` | `Thickness` | 由 Token 控制 | 内间距 |
| `Cursor` | `Cursor` | `Hand` | 鼠标光标 |
| `HorizontalAlignment` | `HorizontalAlignment` | `Left` | 水平对齐 |
| `VerticalAlignment` | `VerticalAlignment` | `Top` | 垂直对齐 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Click` | `EventHandler<RoutedEventArgs>` | 按钮点击事件（继承自 `Avalonia.Controls.Button`）。即使设置了 `NavigateUri`，Click 事件仍然正常触发 |

---

## 伪类（Pseudo-Classes）

HyperLinkButton 支持以下伪类，可在样式选择器中使用：

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:visited` | `ButtonPseudoClass.Visited` | `IsVisited == true`（链接已被访问） |
| `:icononly` | `ButtonPseudoClass.IconOnly` | `Icon` 不为 null 且 `Content` 为 null |
| `:loading` | `ButtonPseudoClass.Loading` | `IsLoading == true` |
| `:danger` | `ButtonPseudoClass.IsDanger` | `IsDanger == true` |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 按钮按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点 |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

支持 `SizeType`（Small / Middle / Large）三种尺寸切换，控制高度、内间距、字体大小和图标尺寸。

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制是否启用过渡动画。启用时，`Background` 和 `Foreground` 的颜色变化会平滑过渡。

---

## 键盘交互

| 按键 | 行为 |
|---|---|
| `Space` / `Enter` | 触发点击（继承自 `Avalonia.Controls.Button`） |
| `Tab` | 移动焦点到 / 离开 HyperLinkButton |

---

## 关键行为说明

### NavigateUri 与 Command 的关系

`NavigateUri` 和 `Command` 是**互不冲突**的两个机制：

1. 点击时首先触发 `OnClick()` → 执行 `Command`、触发 `Click` 事件
2. 然后检查 `NavigateUri`，如不为 null 则异步执行导航
3. 两者可同时设置，都会生效

### IsVisited 的自动管理

- `IsVisited` 仅在 `NavigateUri` 不为 null 且导航成功时自动设为 `true`
- 不设置 `NavigateUri` 时，`IsVisited` 不会自动变化
- 可通过代码或绑定手动控制 `IsVisited` 的值
