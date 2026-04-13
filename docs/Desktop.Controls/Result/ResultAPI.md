# Result API 参考

## 命名空间

- **基类（设备无关）**：`AtomUI.Controls.Commons`（`AbstractResult`）
- **桌面端控件**：`AtomUI.Desktop.Controls`（`Result`）
- **枚举**：`AtomUI.Controls`（`ResultStatus`）
- **AXAML**：`xmlns:atom="https://atomui.net"`

---

## 类定义

```csharp
// 设备无关基类
namespace AtomUI.Controls.Commons;
public abstract class AbstractResult : ContentControl { ... }

// 桌面端控件
namespace AtomUI.Desktop.Controls;
public class Result : AbstractResult { ... }

// 状态枚举
namespace AtomUI.Controls;
public enum ResultStatus
{
    Info,
    Success,
    Error,
    Warning,
    ErrorCode404,
    ErrorCode403,
    ErrorCode500
}
```

---

## 公共属性（StyledProperty）

以下属性全部定义在 `AbstractResult` 基类中，`Result` 通过继承直接获得。

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Status` | `ResultStatus` | `ResultStatus.Info` | 结果状态类型，决定显示的图标/插画和颜色 |
| `Header` | `object?` | `null` | 标题内容，支持文本字符串或自定义控件 |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 标题的数据模板 |
| `SubHeader` | `object?` | `null` | 副标题内容，支持文本字符串或自定义控件 |
| `SubHeaderTemplate` | `IDataTemplate?` | `null` | 副标题的数据模板 |
| `Extra` | `object?` | `null` | 额外操作区内容（通常放置按钮），使用 `[DependsOn(nameof(ExtraTemplate))]` |
| `ExtraTemplate` | `IDataTemplate?` | `null` | 额外操作区的数据模板 |
| `Icon` | `PathIcon?` | `null` | 自定义图标，仅对 Info/Success/Warning/Error 状态有效 |
| `HeaderFontSize` | `double` | `0` | 标题字体大小（由 Token 自动设置） |
| `SubHeaderFontSize` | `double` | `0` | 副标题字体大小（由 Token 自动设置） |
| `Content` | `object?` | `null` | 子内容区域（继承自 `ContentControl`），展示在结果区域底部 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 子内容的数据模板（继承自 `ContentControl`） |

### 属性依赖关系

- `Extra` 属性使用 `[DependsOn(nameof(ExtraTemplate))]` 标记
- `Header` 属性使用 `[DependsOn(nameof(HeaderTemplate))]` 标记
- `SubHeader` 属性使用 `[DependsOn(nameof(SubHeaderTemplate))]` 标记

---

## 内部属性（Internal）

以下属性为框架内部使用，不对外公开：

| 属性 | 类型 | 说明 |
|---|---|---|
| `RelativeHeaderLineHeight` | `double` | 标题相对行高系数（从 SharedToken 绑定） |
| `RelativeSubHeaderLineHeight` | `double` | 副标题相对行高系数（从 SharedToken 绑定） |
| `HeaderLineHeight` | `double` | 标题实际行高（= RelativeHeaderLineHeight × HeaderFontSize） |
| `SubHeaderLineHeight` | `double` | 副标题实际行高（= RelativeSubHeaderLineHeight × SubHeaderFontSize） |
| `StatusIcon` | `PathIcon?` | 当前状态对应的图标实例（DirectProperty，根据 Status 自动计算） |

---

## 枚举类型

### ResultStatus

```csharp
namespace AtomUI.Controls;

public enum ResultStatus
{
    Info,         // 一般信息提示
    Success,      // 操作成功
    Error,        // 操作失败/错误
    Warning,      // 需要注意/警告
    ErrorCode404, // 页面未找到
    ErrorCode403, // 无权限访问
    ErrorCode500  // 服务器错误
}
```

---

## 状态与默认图标映射

| `ResultStatus` | 默认图标 | 来自 |
|---|---|---|
| `Info` | `ExclamationCircleFilled` | `AtomUI.Icons.AntDesign` |
| `Success` | `CheckCircleFilled` | `AtomUI.Icons.AntDesign` |
| `Warning` | `WarningFilled` | `AtomUI.Icons.AntDesign` |
| `Error` | `CloseCircleFilled` | `AtomUI.Icons.AntDesign` |
| `ErrorCode403` | SVG 插画 (`ResultIndicator.UnauthorizedImageSource()`) | 内嵌资源 |
| `ErrorCode404` | SVG 插画 (`ResultIndicator.NotFoundImageSource()`) | 内嵌资源 |
| `ErrorCode500` | SVG 插画 (`ResultIndicator.ServerErrorImageSource()`) | 内嵌资源 |

---

## 伪类

Result 控件不定义自定义伪类。状态差异通过属性选择器 `[Status=xxx]` 在 AXAML 中实现。

---

## 模板部件（Template Parts）

| 名称 | 类型 | 说明 |
|---|---|---|
| `Frame` | `Border` | 根框架 |
| `RootLayout` | `StackPanel` | 垂直布局容器 |
| `PART_StatusIconPresenter` | `ContentPresenter` | 状态图标展示容器 |
| `PART_ErrorCodeImage` | `Svg` | HTTP 错误码 SVG 插画 |
| `Header` | `ContentPresenter` | 标题 |
| `SubHeader` | `ContentPresenter` | 副标题 |
| `ExtraContent` | `ContentPresenter` | 额外操作区 |
| `Content` | `ContentPresenter` | 子内容区域 |
