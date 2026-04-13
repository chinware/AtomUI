# Rate API 参考

## 命名空间

```csharp
// 桌面端控件
namespace AtomUI.Desktop.Controls;

// 跨平台基类（AbstractRate）
namespace AtomUI.Controls.Commons;

// 数据类型（RateValueChangedEventArgs）
namespace AtomUI.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## Rate 公共属性（StyledProperty）

以下属性定义在 `AbstractRate` 基类中，`Rate` 直接继承：

| 属性名 | 类型 | 默认值 | 来源 | 说明 |
|---|---|---|---|---|
| `IsAllowClear` | `bool` | `true` | `AbstractRate` | 是否允许再次点击后清除评分 |
| `IsAllowHalf` | `bool` | `false` | `AbstractRate` | 是否允许半星评分（0.5 步长） |
| `Character` | `object?` | `StarFilled` 图标 | `AbstractRate` | 自定义评分字符，支持 `Icon`、`char`、`string` 三种类型 |
| `StarColor` | `IBrush?` | 由 Token 控制（`RateToken.StarColor`） | `AbstractRate` | 星星选中颜色 |
| `StarBgColor` | `IBrush?` | 由 Token 控制（`RateToken.StarBg`） | `AbstractRate` | 星星未选中背景色 |
| `Count` | `int` | `5` | `AbstractRate` | 星星总数 |
| `Value` | `double` | `NaN`（初始化后取 `DefaultValue`） | `AbstractRate` | 当前评分值 |
| `DefaultValue` | `double` | `0` | `AbstractRate` | 默认评分值（`Value` 未设置时的初始值） |
| `IsKeyboardEnabled` | `bool` | `true` | `AbstractRate` | 是否启用键盘操作（方向键调整评分） |
| `ToolTips` | `IList<string>?` | `null` | `AbstractRate` | 每颗星的提示文案列表，长度应等于 `Count` |
| `SizeType` | `SizeType` | `SizeType.Middle` | `AbstractRate`（共享属性） | 尺寸类型（`Large` / `Middle` / `Small`） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | `AbstractRate`（共享属性） | 是否启用过渡动画（悬浮缩放） |

### 继承自 Avalonia.Controls.Primitives.TemplatedControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsEnabled` | `bool` | `true` | 是否启用，`false` 为只读模式 |
| `Focusable` | `bool` | `true` | 是否可获得焦点（由主题设置） |
| `FontSize` | `double` | 由主题控制 | 影响文字字符的大小 |
| `ClipToBounds` | `bool` | `false` | 裁剪超出边界内容（设为 false 以支持悬浮缩放溢出） |

---

## Rate 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `ValueChanged` | `EventHandler<RateValueChangedEventArgs>` | 评分值变化事件（点击确认或键盘调整后触发） |
| `HoverValueChanged` | `EventHandler<RateValueChangedEventArgs>` | 悬浮值变化事件（鼠标在星星上移动时触发） |

### RateValueChangedEventArgs

选中变化事件参数，继承自 `EventArgs`。

| 属性 | 类型 | 说明 |
|---|---|---|
| `OldValue` | `double` | 变化前的值 |
| `NewValue` | `double` | 变化后的值 |

---

## 键盘操作

当 `IsKeyboardEnabled = true`（默认）且 Rate 获得焦点时，支持以下键盘操作：

| 按键 | 整星模式 | 半星模式 |
|---|---|---|
| `←` 左方向键 | 减 1 | 减 0.5 |
| `→` 右方向键 | 加 1 | 加 0.5 |

评分值会被自动限制在 `[0, Count]` 范围内。

---

## Character 属性支持的类型

| 类型 | 示例 | 渲染方式 |
|---|---|---|
| `Icon`（`PathIcon` 子类） | `StarFilled`、`HeartOutlined` | 通过 `FillBrush` / `StrokeBrush` 着色 |
| `char` | `'A'`、`'秦'` | 通过内部 `RateCharacter` 控件自绘制 |
| `string` | `"A"`、`"秦"` | 取首字符，通过 `RateCharacter` 渲染 |
| `null` | — | 自动使用默认 `StarFilled` 图标 |

---

## Value 属性行为说明

- `Value` 的初始值为 `double.NaN`，控件初始化时自动设置为 `DefaultValue`
- `Value` 的有效范围为 `[0, Count]`
- 当 `IsAllowHalf = true` 时，`Value` 可以是 0.5 的倍数（如 0.5、1、1.5、2...）
- 当 `IsAllowHalf = false` 时，`Value` 始终为整数
- 将 `Value` 设为 0 表示清除评分

---

## 实现的接口

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 过渡动画开关，控制悬浮缩放过渡效果 |
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 三种尺寸切换（Small / Middle / Large），映射到不同的星星大小 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可参与表单验证 |

**IFormItemAware 在 Rate 中的行为：**
- `GetFormValue()` → 返回 `Value`（`double?` 类型）
- `SetFormValue(value)` → 设置 `Value`（接受 `double?`）
- `ClearFormValue()` → 将 `Value` 设为 `0.0`
- `ValueChanged` → 当 `Value` 变化时触发
