# Switch API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

> **注意**：AtomUI 中该控件类名为 `ToggleSwitch`（非 `Switch`），AXAML 中使用 `atom:ToggleSwitch`。

---

## 枚举类型

### SizeType（来自 `AtomUI.Controls`）

| 值 | 说明 |
|---|---|
| `Small` | 小号开关 |
| `Middle` | 中号开关（默认） |

> ToggleSwitch 仅支持 `Middle` 和 `Small` 两种尺寸，`Large` 等同于 `Middle`。

---

## 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `OnContent` | `object?` | `null` | 开启状态时轨道内显示的内容（文字或图标） |
| `OnContentTemplate` | `IDataTemplate?` | `null` | 开启状态内容模板 |
| `OffContent` | `object?` | `null` | 关闭状态时轨道内显示的内容 |
| `OffContentTemplate` | `IDataTemplate?` | `null` | 关闭状态内容模板 |
| `SizeType` | `SizeType` | `SizeType.Middle` | 开关尺寸（共享属性，通过 `AddOwner` 注册） |
| `IsLoading` | `bool` | `false` | 加载中状态，显示旋转指示器并禁止切换 |
| `GrooveBackground` | `IBrush?` | 由主题控制 | 滑槽背景色 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `IsWaveSpiritEnabled` | `bool` | 跟随全局 Token | 是否启用切换波纹效果 |

### 继承自 Avalonia.Controls.Primitives.ToggleButton 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsChecked` | `bool?` | `false` | 开关状态：`true`（开）、`false`（关） |
| `Command` | `ICommand?` | `null` | 点击命令（MVVM 绑定） |
| `CommandParameter` | `object?` | `null` | 命令参数 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `IsPressed` | `bool` | `false`（只读） | 是否处于按压状态 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Click` | `EventHandler<RoutedEventArgs>` | 开关点击事件（继承自 `Button`） |
| `Checked` | `EventHandler<RoutedEventArgs>` | 开关切换为**开启**状态时触发（继承自 `ToggleButton`） |
| `Unchecked` | `EventHandler<RoutedEventArgs>` | 开关切换为**关闭**状态时触发（继承自 `ToggleButton`） |

> **提示**：监听状态变更推荐使用 `Checked` / `Unchecked` 事件，或直接绑定 `IsChecked` 属性。

---

## 伪类（Pseudo-Classes）

### 继承自 ToggleButton 的伪类

| 伪类 | 触发条件 |
|---|---|
| `:checked` | `IsChecked == true`（开启） |
| `:unchecked` | `IsChecked == false`（关闭） |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 按下 |
| `:disabled` | `IsEnabled == false` |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

### IWaveSpiritAwareControl

```csharp
public bool IsWaveSpiritEnabled { get; set; }
```

### IFormItemAware

可作为 `FormItem` 的子控件参与表单验证。`IsChecked` 值变更时自动通知表单。

---

## 键盘交互

| 按键 | 行为 |
|---|---|
| `Space` | 切换开关状态（非加载中时） |
