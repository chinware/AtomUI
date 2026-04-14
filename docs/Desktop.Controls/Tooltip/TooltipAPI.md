# Tooltip API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 附加属性（AttachedProperty）

ToolTip 主要通过附加属性使用，附加到目标控件上。所有附加属性均定义在 `ToolTip` 类中。

### 内容与显示控制

| 附加属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ToolTip.Tip` | `object?` | `null` | 提示内容，可以是字符串（自动转为 `TextBlock`），也可以是任意控件 |
| `ToolTip.TipHostWidth` | `double` | `double.NaN` | 提示框内容区域宽度，`NaN` 表示自动 |
| `ToolTip.IsOpen` | `bool` | `false` | 是否打开 ToolTip。可用于程序化控制，需配合 `IsCustomShowAndHide` 使用 |
| `ToolTip.IsCustomShowAndHide` | `bool` | `false` | 是否使用自定义显示/隐藏逻辑。设为 `true` 时 `ToolTipService` 不会自动关闭 ToolTip |

### 弹出方向与定位

| 附加属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ToolTip.Placement` | `PlacementMode` | `Top` | 弹出方向，支持 12 种方向 |
| `ToolTip.HorizontalOffset` | `double` | `0` | 水平方向的额外偏移量（像素） |
| `ToolTip.VerticalOffset` | `double` | `0` | 垂直方向的额外偏移量（像素） |
| `ToolTip.MarginToAnchor` | `double` | `4` | ToolTip 距离目标元素的间距（像素），根据弹出方向自动应用到水平或垂直方向 |
| `ToolTip.CustomPopupPlacementCallback` | `CustomPopupPlacementCallback?` | `null` | 自定义弹出位置计算回调，用于高级定位场景 |

### 箭头控制

| 附加属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ToolTip.IsShowArrow` | `bool` | `true` | 是否显示箭头指示器 |
| `ToolTip.IsPointAtCenter` | `bool` | `false` | 箭头是否指向目标控件的中心点（仅在 `IsShowArrow=true` 时生效） |

### 颜色

| 附加属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ToolTip.PresetColor` | `PresetColorType?` | `null` | 预设颜色，使用 Ant Design 调色板。优先级高于 `Color` |
| `ToolTip.Color` | `Color?` | `null` | 自定义背景颜色，任意 ARGB 颜色值 |

### 延迟与行为

| 附加属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ToolTip.ShowDelay` | `int` | `400` | 鼠标悬浮后延迟显示时间（毫秒），设为 `0` 立即显示 |
| `ToolTip.BetweenShowDelay` | `int` | `100` | 快速切换时的间隔时间（毫秒）。在此时间内从一个 ToolTip 目标移到另一个时，跳过 `ShowDelay` 直接显示 |
| `ToolTip.ShowOnDisabled` | `bool` | `false` | 目标控件禁用时是否仍显示 ToolTip |
| `ToolTip.ServiceEnabled` | `bool` | `true` | 是否启用 ToolTipService 的自动管理。设为 `false` 时完全禁用该控件的 ToolTip |

---

## 实例属性（StyledProperty）

以下属性定义在 ToolTip 实例上（非附加属性）：

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsMotionEnabled` | `bool` | 跟随全局 Token（`EnableMotion`） | 是否启用弹出/关闭动画（共享属性，通过 `AddOwner` 注册） |
| `IsWaveSpiritEnabled` | `bool` | 跟随全局 Token（`EnableWaveSpirit`） | 是否启用波纹效果（共享属性，通过 `AddOwner` 注册） |

### 继承自 ContentControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | ToolTip 显示的内容，通常由 `Tip` 附加属性自动设置 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `Background` | `IBrush?` | 由 Token 控制 | 背景色。设置 `PresetColor` 或 `Color` 后会自动更新 |
| `Foreground` | `IBrush?` | 由 Token 控制 | 前景色（文本颜色） |

---

## 事件

| 事件名 | 事件类型 | 路由策略 | 说明 |
|---|---|---|---|
| `ToolTipOpening` | `EventHandler<CancelRoutedEventArgs>` | `Direct` | ToolTip 即将打开时在目标控件上触发。设置 `Cancel = true` 可阻止打开 |
| `ToolTipClosing` | `EventHandler<RoutedEventArgs>` | `Direct` | ToolTip 即将关闭时在目标控件上触发 |

### 事件注册辅助方法

```csharp
// 在目标控件上注册 ToolTip 打开事件
ToolTip.AddToolTipOpeningHandler(control, (sender, args) =>
{
    // args.Cancel = true; // 可取消打开
});

// 在目标控件上注册 ToolTip 关闭事件
ToolTip.AddToolTipClosingHandler(control, (sender, args) =>
{
    // 处理关闭逻辑
});

// 移除事件处理程序
ToolTip.RemoveToolTipOpeningHandler(control, handler);
ToolTip.RemoveToolTipClosingHandler(control, handler);
```

---

## 伪类（Pseudo-Classes）

| 伪类 | 常量来源 | 触发条件 |
|---|---|---|
| `:open` | `StdPseudoClass.Open` | ToolTip 弹出层打开时激活 |

---

## PresetColorType 预设颜色枚举

Ant Design 调色板提供以下预设颜色，可通过 `ToolTip.PresetColor` 使用：

| 值 | 色系 | 说明 |
|---|---|---|
| `Blue` | 蓝色 | 默认主色调 |
| `Red` | 红色 | 警告/错误色 |
| `Green` | 绿色 | 成功色 |
| `Orange` | 橙色 | 暖色调 |
| `Gold` | 金色 | 高亮/重要 |
| `Yellow` | 黄色 | 注意色 |
| `Lime` | 青柠色 | 清新色 |
| `Cyan` | 青色 | 信息色 |
| `GeekBlue` | 极客蓝 | 深蓝色调 |
| `Purple` | 紫色 | 优雅色调 |
| `Pink` | 粉色 | 柔和色调 |
| `Magenta` | 品红 | 强烈色调 |
| `Volcano` | 火山色 | 暖红色调 |
| `Grey` | 灰色 | 中性色 |

---

## PlacementMode 方向枚举

| 值 | 说明 | 箭头方向 |
|---|---|---|
| `Top` | 上方居中 | 箭头朝下 |
| `Bottom` | 下方居中 | 箭头朝上 |
| `Left` | 左侧居中 | 箭头朝右 |
| `Right` | 右侧居中 | 箭头朝左 |
| `TopEdgeAlignedLeft` | 上方左对齐 | 箭头朝下偏左 |
| `TopEdgeAlignedRight` | 上方右对齐 | 箭头朝下偏右 |
| `BottomEdgeAlignedLeft` | 下方左对齐 | 箭头朝上偏左 |
| `BottomEdgeAlignedRight` | 下方右对齐 | 箭头朝上偏右 |
| `LeftEdgeAlignedTop` | 左侧顶对齐 | 箭头朝右偏上 |
| `LeftEdgeAlignedBottom` | 左侧底对齐 | 箭头朝右偏下 |
| `RightEdgeAlignedTop` | 右侧顶对齐 | 箭头朝左偏上 |
| `RightEdgeAlignedBottom` | 右侧底对齐 | 箭头朝左偏下 |

---

## 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制 ToolTip 弹出和关闭时是否使用动画效果。设为 `false` 时立即显示/隐藏。

### IPopupHostProvider

```csharp
IPopupHost? PopupHost { get; }
event Action<IPopupHost?>? PopupHostChanged;
```

提供当前 ToolTip 的弹出宿主信息，供弹出层系统管理。

### IArrowAwareShadowMaskInfoProvider（internal）

为 ToolTip 的阴影渲染提供箭头感知的遮罩信息，包括：
- `GetMaskCornerRadius()` — 遮罩圆角
- `GetMaskBounds()` — 遮罩边界
- `GetMaskBackground()` — 遮罩背景
- `GetArrowPosition()` — 箭头位置
- `IsShowArrow()` — 是否显示箭头
- `GetArrowIndicatorBounds()` — 箭头指示器边界
