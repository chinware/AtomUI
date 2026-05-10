# ColorPicker Popup 重构方案：TriggerType 与箭头管理

## 背景

ColorPicker 已完成从 Flyout 到直接 Popup 的基本迁移（删除了 `AbstractColorPickerFlyout`、`ColorPickerFlyout`、`GradientColorPickerFlyout`），但 TriggerType 的实现和箭头管理做得不好：

- TriggerType 当前通过简单的 `OnClick`/`OnPointerEntered`/`OnPointerExited` 重写实现，缺少延迟定时器、全局指针追踪、焦点范围检测
- 箭头缺少 `IsShowArrowEffective`、`ArrowPosition`、翻转追踪等关键逻辑
- `IsLightDismissEnabled` 硬编码为 `True`，应根据 TriggerType 动态设置

## 参考实现

- `FlyoutStateHelper.cs` — TriggerType 的完整实现模式（定时器 + InputManager + 范围检测）
- `Flyout.cs` — 箭头管理模式（IsShowArrowEffective / ArrowPosition / flip tracking）
- `PopupUtils.cs` — 箭头计算工具方法

## 重构内容

### 1. TriggerType 重写

**当前问题**：
- Hover 模式没有 `MouseEnterDelay`/`MouseLeaveDelay` 定时器
- Hover 模式没有通过 `InputManager.Process` 订阅全局指针移动来检测指针是否在 Popup 内
- Focus 模式没有根级 PointerPressed 处理（点击外部关闭）
- Focus 模式没有焦点范围检测（`IsFocusWithinFlyoutScope`）
- Click 模式没有正确设置 `IsLightDismissEnabled`

**方案**：在 `AbstractColorPicker` 中内联 `FlyoutStateHelper` 的核心逻辑：

```csharp
// 新增属性
public static readonly StyledProperty<int> MouseEnterDelayProperty =
    AvaloniaProperty.Register<AbstractColorPicker, int>(nameof(MouseEnterDelay), 200);

public static readonly StyledProperty<int> MouseLeaveDelayProperty =
    AvaloniaProperty.Register<AbstractColorPicker, int>(nameof(MouseLeaveDelay), 200);

// 新增字段
private DispatcherTimer? _mouseEnterDelayTimer;
private DispatcherTimer? _mouseLeaveDelayTimer;
private IDisposable? _popupPointerSubscription;
private TopLevel? _registeredTopLevel;
private bool _isPickerShowing;
```

**Hover 模式实现要点**：
1. 监听 `IsPointerOverProperty` 变化（通过 `GetObservable`）
2. 指针进入 → 启动 `_mouseEnterDelayTimer`，到期后打开 Popup
3. 指针离开 anchor → 检查 Popup 内是否有指针，没有则启动 `_mouseLeaveDelayTimer`
4. Popup 打开后 → 通过 `InputManager.Process` 订阅全局指针移动事件
5. 全局指针移动 → hit test 判断是否在 flyout scope 内（anchor 或 popup child）
6. 离开 scope → 启动关闭定时器；回到 scope → 取消关闭定时器

**Click 模式实现要点**：
1. 设置 `IsLightDismissEnabled = true`（仅 Click 模式需要）
2. 通过 `AddHandler(PointerPressedEvent, ..., handledEventsToo: true)` 处理点击
3. 点击时 toggle `IsPickerOpen`

**Focus 模式实现要点**：
1. 监听 `GotFocus` → 打开 Popup
2. 监听 `LostFocus` → `Dispatcher.Post` 延迟检查焦点是否仍在 flyout scope 内
3. Popup 打开后 → 注册根级 `PointerPressedEvent`（Tunnel 路由）
4. 根级点击 → 判断点击目标是否在 flyout scope 内，不在则关闭
5. Flyout scope = anchor 及其子元素 + popup child 及其逻辑子元素

**Scope 检测方法**：
```csharp
private bool IsVisualInPickerScope(Visual visual)
{
    // 在 anchor 内
    if (visual == this || this.IsVisualAncestorOf(visual))
        return true;
    // 在 popup 内
    if (_popup is { Child: Visual popupChild } &&
        (visual == popupChild || popupChild.IsLogicalAncestorOf(visual)))
        return true;
    return false;
}
```

**生命周期管理**：
- `OnAttachedToVisualTree` → `SetupTriggerHandler()`
- `OnDetachedFromVisualTree` → 停止所有定时器、取消订阅、清理
- `TriggerTypeProperty` 变化 → 重新 `SetupTriggerHandler()`

**删除当前的**：
- `OnClick()` 重写中的 TriggerType 逻辑
- `OnPointerEntered()` / `OnPointerExited()` 重写
- `_isPointerInPopup` 字段
- `HandlePopupPointerEntered` / `HandlePopupPointerExited`
- `ClosePickerWithDelayIfNeeded()`
- `HandleGotFocus` / `HandleLostFocus` 中的简单实现

### 2. 箭头管理重写

**当前问题**：
- 没有 `IsShowArrowEffective` 计算（某些 Placement 不支持箭头）
- 没有 `ArrowPosition` 属性（箭头方向）
- 没有翻转追踪（Popup 被翻转时箭头方向需要跟着翻转）
- Theme 模板中 `ArrowDecoratedBox` 没有绑定 `ArrowPosition` 和 `IsShowArrow`

**方案**：参照 `Flyout.cs` 的模式：

```csharp
// 新增内部属性
internal static readonly DirectProperty<AbstractColorPicker, bool> IsShowArrowEffectiveProperty = ...;
internal static readonly DirectProperty<AbstractColorPicker, bool> IsPopupHorizontalFlippedProperty = ...;
internal static readonly DirectProperty<AbstractColorPicker, bool> IsPopupVerticalFlippedProperty = ...;
internal static readonly StyledProperty<ArrowPosition> ArrowPositionProperty =
    ArrowDecoratedBox.ArrowPositionProperty.AddOwner<AbstractColorPicker>();
```

**箭头计算逻辑**：
```csharp
private void ConfigureShowArrowEffective()
{
    if (!IsShowArrow)
    {
        IsShowArrowEffective = false;
    }
    else
    {
        var placement = Placement;
        IsShowArrowEffective = PopupUtils.CanEnabledArrow(placement, PlacementAnchor, PlacementGravity);
    }
}

private void ConfigureArrowPosition()
{
    var placement = Placement;
    var arrowPosition = PopupUtils.CalculateArrowPosition(placement, PlacementAnchor, PlacementGravity);
    if (arrowPosition.HasValue)
    {
        arrowPosition = ArrowPositionUtils.FlipArrowPosition(
            arrowPosition.Value, IsPopupHorizontalFlipped, IsPopupVerticalFlipped);
        SetCurrentValue(ArrowPositionProperty, arrowPosition);
    }
}
```

**触发时机**：
- `IsShowArrowProperty` / `PlacementProperty` / `PlacementGravityProperty` 变化 → `ConfigureShowArrowEffective()`
- `PlacementProperty` / `IsPopupHorizontalFlippedProperty` / `IsPopupVerticalFlippedProperty` 变化 → `ConfigureArrowPosition()`

**Popup 翻转绑定**（在 `OnApplyTemplate` 中）：
```csharp
this[!IsPopupHorizontalFlippedProperty] = _popup[!PopupControl.IsHorizontalFlippedProperty];
this[!IsPopupVerticalFlippedProperty]   = _popup[!PopupControl.IsVerticalFlippedProperty];
```

### 3. Theme 模板修复

**ColorPickerTheme.axaml / GradientColorPickerTheme.axaml**：

```xml
<atom:Popup Name="PART_Popup"
            WindowManagerAddShadowHint="False"
            IsLightDismissEnabled="{TemplateBinding IsLightDismissEnabled}"
            IsMotionEnabled="{TemplateBinding IsMotionEnabled}"
            ShouldUseOverlayLayer="{TemplateBinding ShouldUseOverlayPopup}"
            RequestedPlacement="{TemplateBinding Placement}"
            PlacementGravity="{TemplateBinding PlacementGravity}"
            IsPointAtCenter="{TemplateBinding IsPointAtCenter}"
            IsOpen="{TemplateBinding IsPickerOpen, Mode=TwoWay}"
            MarginToAnchor="{TemplateBinding MarginToAnchor}">
    <atom:ArrowDecoratedBox Content="{TemplateBinding PickerPresenter}"
                            IsShowArrow="{TemplateBinding IsShowArrowEffective}"
                            ArrowPosition="{TemplateBinding ArrowPosition}"
                            IsMotionEnabled="{TemplateBinding IsMotionEnabled}"/>
</atom:Popup>
```

**关键变化**：
- `IsLightDismissEnabled` → TemplateBinding（动态，由 TriggerType 决定）
- `ShouldUseOverlayLayer` → TemplateBinding（新增公共属性）
- 新增 `PlacementGravity`、`IsPointAtCenter` 绑定
- `ArrowDecoratedBox` 新增 `IsShowArrow` 和 `ArrowPosition` 绑定

### 4. 新增公共属性

```csharp
public static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty =
    AvaloniaProperty.Register<AbstractColorPicker, bool>(nameof(ShouldUseOverlayPopup), true);

public static readonly StyledProperty<bool> IsLightDismissEnabledProperty =
    AvaloniaProperty.Register<AbstractColorPicker, bool>(nameof(IsLightDismissEnabled), true);

public static readonly StyledProperty<PopupAnchor> PlacementAnchorProperty =
    Popup.PlacementAnchorProperty.AddOwner<AbstractColorPicker>();
```

`IsLightDismissEnabled` 在 `SetupTriggerHandler()` 中根据 TriggerType 自动设置：
```csharp
SetCurrentValue(IsLightDismissEnabledProperty, TriggerType == FlyoutTriggerType.Click);
```

### 5. 命名清理

| 旧名称 | 新名称 | 说明 |
|--------|--------|------|
| `NotifyFlyoutOpened()` | `NotifyPickerOpened()` | 子类重写点 |
| `NotifyFlyoutClosed()` | `NotifyPickerClosed()` | 子类重写点 |
| `ClosePickerFlyout()` | `ClosePicker()` | 公共 API |
| `UpdatePseudoClasses` 中 `:flyout-open` | 保持不变 | 兼容现有样式 |

### 6. Popup Overlay 配置

Hover 模式下需要特殊配置以避免 Popup overlay 拦截指针事件：
```csharp
if (_popup != null)
{
    _popup.OverlayInputPassThroughElement = this;
    _popup.OverlayDismissEventPassThrough = true;
}
```

这确保：
- 指针事件能穿透 overlay 到达 anchor（不会因为 overlay 导致 PointerExited）
- overlay 的 dismiss 事件不会直接关闭 Popup（由我们的逻辑控制关闭时机）

## 实现步骤

1. 在 `AbstractColorPicker` 中添加新属性（MouseEnterDelay、MouseLeaveDelay、箭头相关、ShouldUseOverlayPopup、IsLightDismissEnabled）
2. 删除当前的 TriggerType 简单实现（OnClick/OnPointerEntered/OnPointerExited 等）
3. 实现 `SetupTriggerHandler()` + 三种模式的完整逻辑
4. 实现箭头管理（ConfigureShowArrowEffective / ConfigureArrowPosition）
5. 更新 `OnApplyTemplate` 中的 Popup 绑定（翻转追踪、overlay 配置）
6. 更新两个 Theme AXAML 模板
7. 重命名 `NotifyFlyoutOpened`/`NotifyFlyoutClosed` → `NotifyPickerOpened`/`NotifyPickerClosed`
8. 更新 `ColorPicker.cs` 和 `GradientColorPicker.cs` 中的重写方法名
9. 构建验证

## 验证清单

- [ ] Click 模式：点击打开/关闭，点击外部关闭（light dismiss）
- [ ] Hover 模式：延迟打开，指针移到 Popup 内不关闭，离开后延迟关闭
- [ ] Focus 模式：获得焦点打开，失去焦点关闭，点击外部关闭
- [ ] 箭头：正确显示/隐藏，方向正确，翻转后方向跟随
- [ ] IsPointAtCenter：箭头指向 anchor 中心
- [ ] 窗口失活时关闭
- [ ] Disabled 状态不响应触发
- [ ] GradientColorPicker 同样正常工作
