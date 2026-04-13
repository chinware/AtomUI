# TextBlock API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 公共属性

### AtomUI TextBlock 扩展属性

AtomUI 的 `TextBlock` 没有定义额外的 `StyledProperty`，其扩展能力完全通过 `IFormItemAware` 接口实现。所有视觉属性均继承自 Avalonia 的 `TextBlock`。

### 继承自 Avalonia.Controls.TextBlock 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Text` | `string?` | `null` | 要显示的文本字符串 |
| `TextWrapping` | `TextWrapping` | `NoWrap` | 文本换行模式（`NoWrap` / `Wrap` / `WrapWithOverflow`） |
| `TextTrimming` | `TextTrimming` | `None` | 文本裁剪模式（`None` / `CharacterEllipsis` / `WordEllipsis`） |
| `TextAlignment` | `TextAlignment` | `Left` | 文本对齐方式（`Left` / `Center` / `Right` / `Justify`） |
| `TextDecorations` | `TextDecorationCollection?` | `null` | 文本装饰（下划线、删除线等） |
| `FontSize` | `double` | 由全局样式控制 | 字体大小 |
| `FontWeight` | `FontWeight` | `Normal` | 字体粗细 |
| `FontStyle` | `FontStyle` | `Normal`（AtomUI 覆盖） | 字体样式（Normal / Italic / Oblique） |
| `FontFamily` | `FontFamily` | 由全局样式控制 | 字体族 |
| `Foreground` | `IBrush?` | 由全局样式控制 | 文本前景色 |
| `Background` | `IBrush?` | `null` | 控件背景色 |
| `LineHeight` | `double` | `NaN` | 行高 |
| `MaxLines` | `int` | `0`（无限制） | 最大显示行数 |
| `Inlines` | `InlineCollection?` | `null` | 内联元素集合，用于富文本排版 |
| `Padding` | `Thickness` | `0` | 内间距 |

---

## 事件

AtomUI `TextBlock` 没有定义额外的公共事件。`IFormItemAware.ValueChanged` 事件通过显式接口实现，仅供 Form 系统内部使用。

---

## 伪类（Pseudo-Classes）

TextBlock 不定义额外的伪类。继承的标准伪类：

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |

---

## 实现的接口

### IFormItemAware

TextBlock 通过显式接口实现 `IFormItemAware`，使其能参与 Form 表单系统：

```csharp
void IFormItemAware.SetFormValue(object? value);  // 设置 Text
object? IFormItemAware.GetFormValue();             // 获取 Text
void IFormItemAware.ClearFormValue();              // 清空 Text
void IFormItemAware.NotifyValidateStatus(FormValidateStatus status);  // 接收验证状态
event EventHandler? IFormItemAware.ValueChanged;   // Text 变化时通知
```

**行为说明：**

- `SetFormValue` 将传入值转为 `string` 后设置到 `Text` 属性
- `GetFormValue` 直接返回 `Text` 属性值
- `ClearFormValue` 将 `Text` 设置为 `null`
- `ValueChanged` 在 `Text` 属性变化时自动触发
- `NotifyValidateStatus` 当前为空实现，预留未来扩展

---

## 与 Avalonia TextBlock 的行为差异

| 行为 | Avalonia TextBlock | AtomUI TextBlock |
|---|---|---|
| `ClipToBounds` 默认值 | `true` | **`false`**（通过构造函数中的 Style 设置） |
| `FontStyle` 默认值 | 继承自父控件 | **`Normal`**（显式覆盖） |
| Form 集成 | ❌ 不支持 | ✅ `IFormItemAware` |
