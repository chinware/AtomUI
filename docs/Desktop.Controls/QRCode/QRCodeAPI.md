# QRCode API 参考

## 命名空间

```csharp
// 桌面端具体实现
namespace AtomUI.Desktop.Controls;

// 设备无关基类
namespace AtomUI.Controls.Commons;

// 枚举类型
namespace AtomUI.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### QRCodeEccLevel

二维码纠错等级枚举，定义于 `AtomUI.Controls` 命名空间。

| 值 | 数值 | 说明 |
|---|---|---|
| `L` | `0` | 最低容错（~7%），二维码最紧凑 |
| `M` | `1` | 默认等级（~15%），平衡容错与紧凑度 |
| `Q` | `2` | 较高容错（~25%），适合放置小图标 |
| `H` | `3` | 最高容错（~30%），适合放置较大图标 |

### QRCodeStatus

二维码状态枚举，定义于 `AtomUI.Controls` 命名空间。

| 值 | 数值 | 说明 |
|---|---|---|
| `Active` | `0` | 正常状态（默认），二维码正常显示 |
| `Expired` | `1` | 已过期，显示遮罩 + 过期文案 + 刷新按钮 |
| `Loading` | `2` | 加载中，显示遮罩 + 旋转加载指示器 |
| `Scanned` | `3` | 已扫描，显示遮罩 + 已扫描文案 |

---

## 公共属性（StyledProperty）

以下属性定义在 `AbstractQRCode` 基类中，`QRCode` 完全继承。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Value` | `string` | `""` | 二维码编码的文本或 URL 内容 |
| `IsBordered` | `bool` | `true` | 是否显示外边框和内间距 |
| `Color` | `IBrush?` | 由主题 Token 控制 | 二维码前景色（像素颜色） |
| `EccLevel` | `QRCodeEccLevel` | `QRCodeEccLevel.M` | 纠错等级（L/M/Q/H） |
| `Size` | `int` | `160` | 二维码像素尺寸（宽高相同） |
| `IconSize` | `int` | `40` | 中心图标区域大小（像素） |
| `Icon` | `IImage?` | `null` | 中心图标图像源，支持 `avares://` 协议 |
| `IconBgColor` | `IBrush?` | 由主题 Token 控制 | 中心图标背景色 |
| `Status` | `QRCodeStatus` | `QRCodeStatus.Active` | 二维码状态 |
| `LoadingContent` | `object?` | `null` | 加载状态自定义内容，为 `null` 时使用默认 Spin |
| `LoadingContentTemplate` | `IDataTemplate?` | `null` | 加载状态内容数据模板 |
| `ExpiredContent` | `object?` | `null` | 过期状态自定义内容，为 `null` 时使用默认文案+刷新按钮 |
| `ExpiredContentTemplate` | `IDataTemplate?` | `null` | 过期状态内容数据模板 |
| `ScannedContent` | `object?` | `null` | 已扫描状态自定义内容，为 `null` 时使用默认文案 |
| `ScannedContentTemplate` | `IDataTemplate?` | `null` | 已扫描状态内容数据模板 |

### 继承自 TemplatedControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Background` | `IBrush?` | `Transparent`（由主题控制） | 二维码背景色（对应 Ant Design 的 `bgColor`） |
| `Foreground` | `IBrush?` | 由 Token 控制 | 前景色（用于状态文案） |
| `BorderBrush` | `IBrush?` | `Transparent`（`IsBordered=True` 时为 `ColorSplit`） | 边框颜色 |
| `BorderThickness` | `Thickness` | 由 `SharedToken.BorderThickness` 控制 | 边框粗细 |
| `CornerRadius` | `CornerRadius` | 由 `SharedToken.BorderRadiusLG` 控制 | 圆角半径 |
| `Padding` | `Thickness` | `IsBordered=True` 时为 `SharedToken.PaddingSM` | 内间距 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `RefreshRequested` | `EventHandler?` | 用户点击过期状态下的刷新按钮时触发，用于重新生成二维码 |

---

## 模板部件

| 部件名 | 类型 | 说明 |
|---|---|---|
| `PART_RefreshButton` | `Button` | 过期状态下的刷新按钮，使用 `[TemplatePart]` 特性声明 |

---

## 类继承关系

```csharp
// 基类（AtomUI.Controls）
[TemplatePart("PART_RefreshButton", typeof(Button))]
public class AbstractQRCode : TemplatedControl
{
    // 全部属性、事件和生成逻辑
}

// 桌面端实现（AtomUI.Desktop.Controls）
public class QRCode : AbstractQRCode
{
    public QRCode()
    {
        this.RegisterTokenResourceScope(QRCodeToken.ScopeProvider);
    }
}
```
