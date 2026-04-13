# Spin API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;       // Spin, SpinIndicator
namespace AtomUI.Controls.Commons;       // AbstractSpin, AbstractSpinIndicator（基类）
namespace AtomUI.Controls;               // SizeType, IMotionAwareControl, ISizeTypeAware
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### SizeType（来自 `AtomUI.Core`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Large` | 大号指示器（页面级加载） |
| `Middle` | 中号指示器（默认，卡片级加载） |
| `Small` | 小号指示器（文本级加载） |

---

## Spin 公共属性（StyledProperty）

以下属性定义在 `AbstractSpin` 基类中，`Spin` 完整继承。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `SizeType.Middle` | 指示器尺寸（共享属性，通过 `AddOwner` 注册） |
| `IsSpinning` | `bool` | `false` | 是否处于加载状态。`true` 时显示遮罩层和指示器 |
| `Tip` | `string?` | `null` | 加载提示文字，显示在指示器下方 |
| `IsShowTip` | `bool` | `false` | 是否显示提示文字。需同时设置 `Tip` 属性 |
| `CustomIndicator` | `object?` | `null` | 自定义加载指示器内容，替换默认旋转圆点 |
| `CustomIndicatorTemplate` | `IDataTemplate?` | `null` | 自定义加载指示器数据模板 |
| `IsMaskBlurEnabled` | `bool` | `false` | 加载时是否对内容启用高斯模糊效果（`BlurEffect Radius=5`） |
| `IsMaskBackgroundEnabled` | `bool` | `false` | 加载时是否显示半透明背景遮罩（`ColorBgMask`） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用 `MaskOpacity` 过渡动画（共享属性） |
| `MotionDuration` | `TimeSpan` | 由 `SpinToken.IndicatorDuration` 控制 | 旋转动画一个完整周期的时长（共享属性） |
| `MotionEasingCurve` | `Easing?` | `LinearEasing` | 旋转动画缓动曲线 |

### 继承自 ContentControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 被包裹的内容区域。不设置时 Spin 为独立模式 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容数据模板 |
| `HorizontalAlignment` | `HorizontalAlignment` | `Left` | 水平对齐（由主题设置） |
| `VerticalAlignment` | `VerticalAlignment` | `Top` | 垂直对齐（由主题设置） |

---

## SpinIndicator 公共属性（StyledProperty）

以下属性定义在 `AbstractSpinIndicator` 基类中，`SpinIndicator` 完整继承。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `SizeType.Middle` | 指示器尺寸，控制圆点大小和整体尺寸 |
| `CustomIndicator` | `object?` | `null` | 自定义指示器内容，设置后替换默认四圆点渲染 |
| `CustomIndicatorTemplate` | `IDataTemplate?` | `null` | 自定义指示器数据模板 |
| `MotionDuration` | `TimeSpan` | 由 `SpinToken.IndicatorDuration` 控制 | 旋转动画一个完整周期的时长 |
| `MotionEasingCurve` | `Easing?` | `LinearEasing` | 旋转动画缓动曲线 |

---

## 实现的接口

### AbstractSpin → IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制 `MaskOpacity` 过渡动画的启用/禁用。启用时，内容区域的透明度变化使用 `DoubleTransition` 平滑过渡；禁用时透明度变化立即生效。

### AbstractSpinIndicator → ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

控制指示器的圆点大小（`DotSize` / `DotSizeSM` / `DotSizeLG`）和整体尺寸（`IndicatorSize` / `IndicatorSizeSM` / `IndicatorSizeLG`）。

---

## 设备无关基类详情

### AbstractSpin（`AtomUI.Controls.Commons`）

`AbstractSpin` 是 Spin 的设备无关基类，定义在 `AtomUI.Controls` 项目中。所有平台（Desktop / 未来 Mobile）的 Spin 实现均继承自此基类。

**基类定义的公共属性：**

全部 Spin 公共属性均在此基类中定义（见上方 Spin 公共属性表格），桌面端 `Spin` 仅添加 Token 注册。

**基类内部属性：**

| 属性名 | 类型 | 说明 |
|---|---|---|
| `IsCustomIndicator`（internal） | `bool` | 当 `CustomIndicator` 或 `CustomIndicatorTemplate` 不为 null 时自动为 `true` |
| `MaskOpacity`（internal） | `double` | 内容区域的不透明度，`IsSpinning = true` 时由主题设为 `0.5` |

**基类关键行为：**

1. `OnPropertyChanged`：监听 `CustomIndicator` / `CustomIndicatorTemplate` 变化，自动更新 `IsCustomIndicator`
2. `OnLoaded` / `OnUnloaded`：管理 `MaskOpacity` 的 `DoubleTransition` 生命周期
3. `ConfigureTransitions`：根据 `IsMotionEnabled` 配置或清除过渡动画

### AbstractSpinIndicator（`AtomUI.Controls.Commons`）

`AbstractSpinIndicator` 是 SpinIndicator 的设备无关基类，负责四圆点的渲染和旋转动画。

**基类内部属性：**

| 属性名 | 类型 | 说明 |
|---|---|---|
| `DotSize`（internal） | `double` | 圆点直径，由主题根据 `SizeType` 设置 |
| `DotBgBrush`（internal） | `IBrush?` | 圆点填充画刷（默认 `ColorPrimary`） |
| `IndicatorAngle`（internal） | `double` | 当前旋转角度（0°~360°），由动画驱动 |

**基类关键行为：**

1. `OnAttachedToVisualTree`：构建并启动无限旋转动画
2. `OnDetachedFromVisualTree`：取消旋转动画，释放资源
3. `Render`：绘制四圆点指示器（当无 `CustomIndicator` 时）
4. `HandleIndicatorAngleChanged`：更新自定义指示器的 `RenderTransform` 旋转

**常量：**

| 常量 | 值 | 说明 |
|---|---|---|
| `DOT_START_OPACITY` | `0.3` | 圆点最低透明度（正弦波谷值） |
