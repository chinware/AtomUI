# BorderBeam 桌面版实现原理

本文档描述 BorderBeam 桌面版的内部边界感知、颜色归一、动画生命周期和连续流光渲染。公共设计与 API 契约见 [BorderBeam 桌面版架构设计](overview.md)，Token 语义见 [BorderBeam Token 设计](token.md)，变化记录见 [BorderBeam Changelog](changelog.md)。

## 1. 实现定位

BorderBeam 的实现目标是在不修改内容控件模板、不拦截内容交互的前提下，绘制贴合内容有效边界的动态流光。实现文档聚焦包装控件、presenter 渲染、感知接口和动画资源释放。

BorderBeam 不表达业务状态，不承担 focus、validation、selected 或 error 等状态逻辑。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/BorderBeam/BorderBeam.cs`：公共 API、Content 几何感知、颜色归一、动画控制和 template part 接入。
- `src/AtomUI.Desktop.Controls/BorderBeam/BorderBeamPresenter.cs`：internal 渲染层，绘制边框环和流动高光。
- `src/AtomUI.Desktop.Controls/BorderBeam/IBorderBeamAwareControl.cs`：被装饰控件边界感知接口。
- `src/AtomUI.Desktop.Controls/BorderBeam/BorderBeamGeometry.cs`：边框厚度和圆角几何值。
- `src/AtomUI.Desktop.Controls/BorderBeam/BorderBeamColorStop.cs`：单个渐变停靠点。
- `src/AtomUI.Desktop.Controls/BorderBeam/BorderBeamColorStops.cs`：颜色停靠点集合支持。
- `src/AtomUI.Desktop.Controls/BorderBeam/BorderBeamToken.cs`：BorderBeam 专属 Token。
- `src/AtomUI.Desktop.Controls/BorderBeam/Themes/BorderBeamTheme.axaml`：内容层和 presenter 层装配。

## 3. 核心类职责

`BorderBeam` 继承 `ContentControl`，负责公共属性、感知内容订阅、effective geometry、effective gradient、实例级 motion 开关和 presenter 状态同步。

`BorderBeamPresenter` 是纯渲染层，负责根据 bounds、border thickness、corner radius、outset、beam size、gradient stops 和 progress 绘制流光。它不参与命中测试，也不读取内容控件状态。

`IBorderBeamAwareControl` 是窄契约，只暴露边界几何和几何变化事件。BorderBeam 不通过类型判断或反射读取特定控件的 internal 属性。

## 4. 状态与数据流

状态流：

```text
BorderBeam public API
  Content / Color / ColorStops / Outset
  BorderThickness / CornerRadius
  IsMotionEnabled / Duration / BeamSize
      ↓
Geometry awareness
  Content implements IBorderBeamAwareControl
    → BorderBeamGeometry
  otherwise
    → BorderBeam explicit geometry
      ↓
Effective state
  effective border thickness
  effective corner radius
  effective gradient
  effective outset
  animation progress
      ↓
BorderBeamPresenter render
```

边界来源优先级：

1. `Content` 实现 `IBorderBeamAwareControl` 时，使用 `GetBorderBeamGeometry()` 返回值。
2. `Content` 未实现接口时，使用 `BorderBeam.BorderThickness` 和 `BorderBeam.CornerRadius`。
3. 未显式设置时，使用 Theme 默认值。

颜色来源优先级：

```text
ColorStops.Count > 0
  → normalize stops
else if Color != null
  → [Color 0%, Color 100%]
else
  → theme default gradient
```

## 5. 生命周期与模板接入

`OnApplyTemplate` 获取 `PART_ContentPresenter` 与 `PART_BeamPresenter`。模板替换时必须清理旧 presenter 引用。

`Content` 变化时，BorderBeam 需要解除旧内容的 `BorderBeamGeometryChanged` 订阅，并在新内容实现 `IBorderBeamAwareControl` 时订阅新事件。几何变化事件只刷新 BorderBeam 自身状态，不要求内容控件重新模板化。

`AttachedToVisualTree`、`DetachedFromVisualTree`、有效可见性、实例级 `IsMotionEnabled` 和 bounds 变化共同决定动画启动与停止。有效可见性包含 BorderBeam 自身和 Visual 祖先链；持续 Avalonia animation 使用 `PlaybackBehavior.OnlyIfVisible`，因此隐藏页面中的实例暂停 animation clock。detached 或实例 motion 关闭时必须取消动画并释放内部取消资源。默认主题不把全局 `EnableMotion` 绑定到 `BorderBeam.IsMotionEnabled`，避免全局普通交互动效开关停止 BorderBeam 的持续流光。

## 6. 交互与事件处理

BorderBeam 不处理 pointer、keyboard、focus、command 或 drag/drop 事件。`PART_BeamPresenter` 必须 `IsHitTestVisible=false`。

内部事件只包括：

- content geometry changed。
- collection changed for `ColorStops`。
- motion / duration / beam size / visibility / bounds 变化。
- theme resource 更新导致默认颜色或参数变化。

这些事件只能刷新装饰层，不改变内容控件状态。

## 7. 内部算法与关键流程

### 7.1 几何归一

有效边框和圆角来自感知接口或 BorderBeam 自身属性。`Outset=null` 时按有效边框厚度向外扩，`Outset` 非空时使用显式外扩。圆角半径应按 Avalonia 圆角矩形规则夹取，避免半径超过控件尺寸时产生自交路径。

### 7.2 渐变归一

`ColorStops.Percent` 输入区间固定为 `0~100`。内部将停靠点映射到可见高光段，并为尾部透明渐隐保留空间。停靠点需要 clamp 并排序，避免非法输入破坏 brush。

单色 `Color` 映射为可见段单色加透明尾迹。无显式颜色时使用主题默认渐变。

### 7.3 动画生命周期

动画使用 internal `Progress` 从 `0` 到 `1` 循环，周期由 `Duration` 控制。`Progress` 变化只触发 `BorderBeamPresenter` 重绘。

实例级 `IsMotionEnabled=false`、有效不可见、detached 或 bounds 无有效尺寸时，动画必须停止或暂停。此时不应继续产生 UI 线程 invalidation。

### 7.4 渲染连续性

BorderBeamPresenter 应以一个连续圆角矩形运动路径驱动流光。边框环裁剪跟随被装饰容器的有效边框和圆角；运动路径的转角半径按 `BeamSize` 建立平滑圆角，避免高光段在容器真实小圆角处发生可见面积突变。

高光段按路径切线旋转，并使用 `90% / 50%` 锚点模型：流光矩形局部坐标的 `90% / 50%` 位置落在运动路径上。该模型让光束提前进入转角并保持尾迹连续，不应分成四条互不衔接的直线分别绘制。

## 8. 资源、性能与 AOT 边界

BorderBeam 不使用反射，不访问内容控件 internal 属性或 template part。集成通过 `IBorderBeamAwareControl` 完成。

实例级 motion 未启用时不应启动循环动画。隐藏祖先下的动画必须暂停；动画取消资源必须在 detached、模板替换、content 替换和实例 motion 关闭时释放。

ColorStops 使用实例级集合，避免共享默认集合。集合变更应触发渐变重建和 presenter 重绘，不应重建整个模板。

## 9. 维护不变量

内部重构必须保持以下不变量：

- BorderBeam 包装内容，不修改内容模板。
- beam presenter 不参与命中测试。
- 感知接口只暴露边框厚度和圆角。
- content 替换时旧事件订阅必须释放。
- 实例 motion 关闭或 detached 后不持续 invalidation。
- 渐变尾迹连续，圆角转弯处不分段卡顿。
- 非统一圆角只影响边框环裁剪，不直接拆分运动路径。
- public `ColorStops.Percent` 仍按 `0~100` 解释。

## 10. 测试与验证

验证范围：

- `Content` 为普通控件时使用 BorderBeam 自身几何。
- `Content` 实现 `IBorderBeamAwareControl` 时使用控件返回几何，并响应几何变化事件。
- `ColorStops` 优先级高于 `Color`。
- 单色、多 stop、默认渐变和非法 percent clamp。
- 统一圆角和非统一圆角下流光转角连续。
- 实例级 `IsMotionEnabled=false`、自身或祖先不可见、detached 和零尺寸不产生持续 animation tick。
- presenter 不拦截内容点击、焦点和键盘事件。
- 文档改动运行 `git diff --check`。
