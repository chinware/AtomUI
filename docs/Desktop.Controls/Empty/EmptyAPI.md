# Empty API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;   // Empty 控件
namespace AtomUI.Controls;            // AbstractEmpty 基类、PresetEmptyImage 枚举
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### PresetEmptyImage

预设空状态图片类型枚举。定义在 `AtomUI.Controls` 命名空间。

| 值 | 说明 |
|---|---|
| `Default` | 默认空状态图，彩色插图，较大尺寸，适合页面级空状态展示 |
| `Simple` | 简洁空状态图，灰色线条，较小尺寸，适合列表/表格内嵌场景 |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## 公共属性（StyledProperty）

以下属性定义在 `AbstractEmpty` 基类中，`Empty` 完整继承。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `PresetImage` | `PresetEmptyImage?` | `null` | 预设图片类型（`Default` / `Simple`）。设置后自动根据主题色动态生成 SVG 图片 |
| `ImagePath` | `string?` | `null` | 自定义 SVG 图片文件路径（支持 `avares://` 协议） |
| `ImageSource` | `string?` | `null` | 自定义 SVG 图片内容字符串 |
| `Description` | `string?` | 由本地化系统提供 | 描述文字，默认值为 `"No Data"`（中文 `"暂无数据"`） |
| `IsShowDescription` | `bool` | `true` | 是否显示描述文字 |
| `SizeType` | `SizeType` | `Middle` | 尺寸类型，控制图片高度和描述文字间距 |

> ⚠️ **互斥规则**：`PresetImage`、`ImagePath`、`ImageSource` 三者只能设置其中一个。同时设置多个会在 `OnApplyTemplate` 时抛出 `ApplicationException`。

### 继承自 TemplatedControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `HorizontalAlignment` | `HorizontalAlignment` | `Center` | 水平对齐方式（主题默认居中） |
| `VerticalAlignment` | `VerticalAlignment` | `Center` | 垂直对齐方式（主题默认居中） |
| `Foreground` | `IBrush?` | 由主题控制 | 前景色 |
| `IsVisible` | `bool` | `true` | 是否可见 |

---

## 模板部件（Template Parts）

| 部件名 | 类型 | 说明 |
|---|---|---|
| `PART_SvgImage` | `Avalonia.Svg.Svg` | SVG 图片渲染控件 |

---

## 实现的接口

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |

---

## 影响布局的属性组合

以下属性变化会触发重新测量（`AffectsMeasure`）：

- `PresetImage`
- `ImagePath`
- `ImageSource`
- `Description`
- `IsShowDescription`
- `BorderColor`（内部）
- `BorderColorSecondary`（内部）
- `ShadowColor`（内部）
- `ContentColor`（内部）
- `BgColor`（内部）

