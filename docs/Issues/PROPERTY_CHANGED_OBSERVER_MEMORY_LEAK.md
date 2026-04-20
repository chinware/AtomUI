# Property.Changed Observer 内存泄漏分析报告

**扫描日期**: 2026-04-20
**扫描范围**: `src/` 目录下所有 C# 文件（排除 docs）
**泄漏特征**: `Avalonia.DirectProperty<InputElement, Boolean>._changed` → `LightweightSubject<AvaloniaPropertyChangedEventArgs<Boolean>>` 观察者链无限增长

---

## 泄漏原理

Avalonia 中每个 `AvaloniaProperty` 都有一个静态的 `Changed` 字段（类型为 `LightweightSubject`）。调用 `Property.Changed.Subscribe()` 会向这个**全局**观察者列表添加一个 observer。如果订阅未被 Dispose，observer 永远留在全局列表中，同时通过 lambda 捕获保留整个控件实例树。

关键区别：
- `Property.Changed.Subscribe()` — 订阅**全局**属性变化，所有实例的变化都会触发
- `instance.GetObservable(Property).Subscribe()` — 订阅**单个实例**的属性变化
- `Property.Changed.AddClassHandler<T>()` — 内部调用 `Changed.Subscribe()`，但仅在静态构造函数中调用一次

**泄漏条件**：在实例方法（如 `OnAttachedToVisualTree`）中调用 `Property.Changed.Subscribe()`，且未在对应的生命周期方法中 Dispose。

---

## 🔴 严重问题：直接匹配泄漏特征

### 1. FlyoutStateHelper — IsPointerOverProperty.Changed.Subscribe

**文件**: `src/AtomUI.Desktop.Controls/Flyouts/FlyoutStateHelper.cs`
**行号**: 245-253

```csharp
// SetupTriggerHandler() 方法中，由 NotifyAttachedToVisualTree() 调用
_subscriptions.Add(InputElement.IsPointerOverProperty.Changed.Subscribe(args =>
{
    if (args.Sender == AnchorTarget && 
        AnchorTarget.IsEnabled &&
        AnchorTarget.IsVisible)
    {
        HandleAnchorTargetHover(args);
    }
}));
```

**泄漏分析**:
- `IsPointerOverProperty` 是 `DirectProperty<InputElement, Boolean>` — **精确匹配泄漏特征**
- 每次 `NotifyAttachedToVisualTree()` 调用都向全局 `LightweightSubject` 添加一个 observer
- lambda 捕获了 `this`（FlyoutStateHelper）和 `AnchorTarget`（Control）
- 虽然第 241 行有 `_subscriptions?.Dispose()`，但如果控件被 GC 而未经过 `NotifyDetachedFromVisualTree`，订阅永远泄漏

**保留链**:
```
IsPointerOverProperty._changed (LightweightSubject, 全局静态)
  → observer (lambda)
    → FlyoutStateHelper (this)
      → AnchorTarget (Control)
        → 整个控件子树
      → Flyout
        → Popup → PopupRoot → 整个弹出层
```

**影响范围**: 所有使用 `FlyoutTriggerType.Hover` 的控件：
- SplitButton
- DropdownButton
- 所有带 hover 触发 Flyout 的控件

**修复方案**:

方案 A — 改用实例级 `GetObservable`（推荐）:
```csharp
private void SetupTriggerHandler()
{
    if (AnchorTarget is null) return;
    _subscriptions?.Dispose();
    _subscriptions = new CompositeDisposable();
    if (TriggerType == FlyoutTriggerType.Hover)
    {
        // 改为订阅 AnchorTarget 实例的属性变化，而非全局
        _subscriptions.Add(AnchorTarget.GetObservable(InputElement.IsPointerOverProperty)
            .Subscribe(isPointerOver =>
            {
                if (AnchorTarget.IsEnabled && AnchorTarget.IsVisible)
                {
                    HandleAnchorTargetHover(isPointerOver);
                }
            }));
    }
    // ...
}
```

方案 B — 保持全局订阅但使用 WeakReference:
```csharp
_subscriptions.Add(InputElement.IsPointerOverProperty.Changed.Subscribe(args =>
{
    if (_anchorTargetRef.TryGetTarget(out var target) &&
        args.Sender == target && target.IsEnabled && target.IsVisible)
    {
        HandleAnchorTargetHover(args);
    }
}));
```

---

### 2. FlyoutStateHelper — IsFocusedProperty.Changed.Subscribe

**文件**: `src/AtomUI.Desktop.Controls/Flyouts/FlyoutStateHelper.cs`
**行号**: 263-271

```csharp
_subscriptions.Add(InputElement.IsFocusedProperty.Changed.Subscribe(args =>
{
    if (args.Sender == AnchorTarget &&
        AnchorTarget.IsEnabled &&
        AnchorTarget.IsVisible)
    {
        HandleAnchorTargetFocus(args);
    }
}));
```

**泄漏分析**:
- `IsFocusedProperty` 是 `DirectProperty<InputElement, Boolean>` — **精确匹配泄漏特征**
- 与问题 1 完全相同的泄漏模式
- 每次 `SetupTriggerHandler()` 调用都添加全局 observer

**影响范围**: 所有使用 `FlyoutTriggerType.Focus` 的控件

**修复方案**: 同问题 1，改用 `AnchorTarget.GetObservable(InputElement.IsFocusedProperty)`

---

## 🟠 高风险问题：实例方法中 Changed.Subscribe 未正确清理

### 3. DatePickerPresenter — 覆盖旧订阅未 Dispose

**文件**: `src/AtomUI.Desktop.Controls/DatePicker/DatePickerPresenter.cs`
**行号**: 131-155

```csharp
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    // ⚠️ 直接覆盖 _pointerDisposables，旧的未 Dispose
    _pointerDisposables = new CompositeDisposable(2);
    _pointerDisposables.Add(PickerCalendar.IsPointerInMonthViewProperty.Changed.Subscribe(args =>
    {
        if (CalendarView is not null)
        {
            EmitChoosingStatueChanged(args.GetNewValue<bool>());
        }
    }));
    _pointerDisposables.Add(TimeView.IsPointerInSelectorProperty.Changed.Subscribe(args =>
    {
        if (TimeView is not null)
        {
            EmitChoosingStatueChanged(args.GetNewValue<bool>());
        }
    }));
}

protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    _pointerDisposables?.Dispose();
    // ⚠️ 未设置为 null
}
```

**泄漏分析**:
- `IsPointerInMonthViewProperty` 和 `IsPointerInSelectorProperty` 是 Boolean 属性
- 第 134 行直接 `new CompositeDisposable(2)` 覆盖旧值，**旧的 `_pointerDisposables` 未被 Dispose**
- 如果 `OnAttachedToVisualTree` 被调用两次（边缘情况），旧订阅泄漏
- `OnDetachedFromVisualTree` 中 Dispose 后未设置为 null，可能导致重复 Dispose

**修复方案**:
```csharp
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    _pointerDisposables?.Dispose();  // 先清理旧的
    _pointerDisposables = new CompositeDisposable(2);
    // ... 订阅逻辑不变
}

protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    _pointerDisposables?.Dispose();
    _pointerDisposables = null;  // 设置为 null
}
```

**更好的方案 — 改用实例级 GetObservable**:
```csharp
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    _pointerDisposables?.Dispose();
    _pointerDisposables = new CompositeDisposable(2);
    if (CalendarView is not null)
    {
        _pointerDisposables.Add(CalendarView.GetObservable(PickerCalendar.IsPointerInMonthViewProperty)
            .Subscribe(isPointer => EmitChoosingStatueChanged(isPointer)));
    }
    if (TimeView is not null)
    {
        _pointerDisposables.Add(TimeView.GetObservable(TimeView.IsPointerInSelectorProperty)
            .Subscribe(isPointer => EmitChoosingStatueChanged(isPointer)));
    }
}
```

---

### 4. TimePickerPresenter — 覆盖旧订阅未 Dispose

**文件**: `src/AtomUI.Desktop.Controls/TimePicker/TimePickerPresenter.cs`
**行号**: 296-313

```csharp
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    if (_timeView is not null)
    {
        // ⚠️ 直接覆盖 _choosingStateDisposable，旧的未 Dispose
        _choosingStateDisposable = TimeView.IsPointerInSelectorProperty.Changed.Subscribe(args =>
        {
            ChoosingStatueChanged?.Invoke(this, new ChoosingStatusEventArgs(args.GetNewValue<bool>()));
        });
    }
}

protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    _choosingStateDisposable?.Dispose();
    _choosingStateDisposable = null;
}
```

**泄漏分析**:
- `IsPointerInSelectorProperty` 是 Boolean 属性
- 第 301 行直接覆盖 `_choosingStateDisposable`，**旧订阅未 Dispose**
- 使用全局 `Changed.Subscribe` 而非实例级 `GetObservable`

**修复方案**:
```csharp
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    _choosingStateDisposable?.Dispose();  // 先清理旧的
    if (_timeView is not null)
    {
        // 改用实例级订阅
        _choosingStateDisposable = _timeView.GetObservable(TimeView.IsPointerInSelectorProperty)
            .Subscribe(isPointer =>
            {
                ChoosingStatueChanged?.Invoke(this, new ChoosingStatusEventArgs(isPointer));
            });
    }
}
```

---

### 5. GradientColorSlider — 双重订阅（冗余但已正确清理）

**文件**: `src/AtomUI.Desktop.Controls.ColorPicker/ColorSlider/GradientColorSlider.cs`
**行号**: 89-121

```csharp
protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    _activatedThumbDispose?.Dispose();  // ✓ 先清理
    // ...
    _activatedThumbDispose = GradientColorPickerTrack.ActivatedThumbProperty.Changed.Subscribe(...);
}

protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    _activatedThumbDispose?.Dispose();  // ✓ 先清理
    _activatedThumbDispose = GradientColorPickerTrack.ActivatedThumbProperty.Changed.Subscribe(...);
}

protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    _activatedThumbDispose?.Dispose();  // ✓ 清理
}
```

**泄漏分析**:
- ✓ 每次订阅前都先 Dispose 旧的
- ✓ Detach 时正确清理
- ⚠️ 在 `OnApplyTemplate` 和 `OnAttachedToVisualTree` 中都订阅是冗余的
- ⚠️ 使用全局 `Changed.Subscribe` 而非实例级 `GetObservable`

**修复方案 — 去除冗余并改用实例级订阅**:
```csharp
protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    _activatedThumbDispose?.Dispose();
    Track = e.NameScope.Find<GradientColorPickerTrack>(ColorSliderThemeConstants.TrackPart);
    if (Track is GradientColorPickerTrack track)
    {
        track.IgnoreThumbDrag = true;
        _pressDispose = this.AddDisposableHandler(PointerPressedEvent, TrackPressed, RoutingStrategies.Tunnel);
        _releaseDispose = this.AddDisposableHandler(PointerReleasedEvent, TrackReleased, RoutingStrategies.Tunnel);
        // 改用实例级订阅
        _activatedThumbDispose = track.GetObservable(GradientColorPickerTrack.ActivatedThumbProperty)
            .Subscribe(_ => HandleActivatedThumbChanged());
        if (ActivatedStopIndex != null)
        {
            track.SetActiveThumb(ActivatedStopIndex.Value);
        }
    }
}

// 删除 OnAttachedToVisualTree 中的重复订阅
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    // 不再需要在这里重复订阅
}
```

---

## 🟡 静态构造函数中的 Changed.Subscribe（不增长但永久保留）

以下订阅在静态构造函数中调用，只执行一次，不会导致观察者链增长。但它们会永久保留在 `LightweightSubject` 中。

### 6. ScopeAwareAdornerLayer — 静态构造函数

**文件**: `src/AtomUI.Controls/Primitives/VisualLayers/ScopeAwareAdornerLayer.cs`
**行号**: 44-48

```csharp
static ScopeAwareAdornerLayer()
{
    AdornedElementProperty.Changed.Subscribe(HandleAdornedElementChanged);
    AdornerProperty.Changed.Subscribe(HandleAdornerChanged);
}
```

**分析**: 一次性订阅，不增长。`HandleAdornedElementChanged` 和 `HandleAdornerChanged` 是静态方法，不捕获实例。但 `HandleAdornedElementChanged` 内部调用 `UpdateAdornedElement`，其中通过 `adorned.GetObservable(BoundsProperty).Subscribe(...)` 创建实例级订阅（第 187 行），这个订阅在 `info.Subscription!.Dispose()` 中清理（第 171 行）。

**风险**: 低。静态订阅本身不泄漏，内部的实例级订阅有正确的清理逻辑。

---

### 7. ButtonSpinner — 静态构造函数

**文件**: `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinner.cs`
**行号**: 226-229

```csharp
static ButtonSpinner()
{
    AllowSpinProperty.Changed.Subscribe(AllowSpinChanged);
}
```

**分析**: 一次性订阅，不增长。`AllowSpinChanged` 是静态方法。

**风险**: 低。

---

### 8. ToolTip — 静态构造函数

**文件**: `src/AtomUI.Desktop.Controls/Tooltip/ToolTip.cs`
**行号**: 333-336

```csharp
static ToolTip()
{
    IsOpenProperty.Changed.Subscribe(HandleIsOpenChanged);
}
```

**分析**: 一次性订阅，不增长。`IsOpenProperty` 是 Boolean 属性。

**风险**: 低。

---

### 9. PopupFlyoutBase — 静态构造函数

**文件**: `src/AtomUI.Desktop.Controls/Flyouts/PopupFlyoutBase.cs`
**行号**: 207-210

```csharp
static PopupFlyoutBase()
{
    Control.ContextFlyoutProperty.Changed.Subscribe(OnContextFlyoutPropertyChanged);
}
```

**分析**: 一次性订阅，不增长。

**风险**: 低。

---

### 10. ToolTipService — 构造函数（单例）

**文件**: `src/AtomUI.Desktop.Controls/Tooltip/ToolTipService.cs`
**行号**: 27-34

```csharp
public ToolTipService()
{
    var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
    _subscriptions = new CompositeDisposable(
        inputManager.Process.Subscribe(HandleInputManagerOnProcess),
        ToolTip.ServiceEnabledProperty.Changed.Subscribe(HandleServiceEnabledChanged),
        ToolTip.TipProperty.Changed.Subscribe(HandleTipChanged));
}

public void Dispose()
{
    StopTimer();
    _subscriptions.Dispose();
}
```

**分析**: ToolTipService 是单例，构造一次。有 `Dispose()` 方法。

**风险**: 低。但需确认 `Dispose()` 在应用退出时被调用。

---

## 📊 AddClassHandler 统计（139 处，均在静态构造函数中）

`AddClassHandler` 内部调用 `Changed.Subscribe()`，但仅在静态构造函数中调用一次，不会导致观察者链增长。以下是按文件分组的统计：

| 文件 | 数量 | 说明 |
|------|------|------|
| DataGrid.cs | 23 | 大量属性变化处理 |
| Calendar.cs (DatePicker) | 7 | 日历属性 |
| Calendar.cs (Calendar) | 9 | 日历属性 |
| CascaderViewItem.cs | 3 | IsExpanded/IsSelected/IsChecked (Boolean) |
| CascaderView.cs | 2 | OptionsSource/SelectedOptions |
| Cascader.cs | 4 | IsMultiple/OptionsSource/SelectedOption/SelectedOptions |
| CompactSpace.cs | 2 | Orientation/TemplatedParent |
| ListBox.cs | 2 | IsSelectable/ItemCount |
| DataGridRow.cs | 4 | Header/DetailsTemplate/IsDetailsVisible |
| DataGridRowGroupHeader.cs | 1 | SublevelIndent |
| DataGridColumnHeader.cs | 1 | IsSeparatorsVisible |
| RangeDatePicker.cs | 2 | RangeStartSelectedDate/RangeEndSelectedDate |
| RangeTimePicker.cs | 2 | RangeStartSelectedTime/RangeEndSelectedTime |
| DatePicker.cs | 1 | SelectedDateTime |
| TimePicker.cs | 1 | SelectedTime |
| TextBlock.cs | 1 | Text |
| TextBox.cs | 1 | Watermark |
| TextArea.cs | 1 | Watermark |
| ColorPickerSliderTrack.cs | 3 | Thumb/IncreaseButton/DecreaseButton |
| 其他 | ~70 | 各种控件属性 |

**结论**: `AddClassHandler` 不是泄漏源。每个调用只添加一个永久 observer，总共 139 个，不会增长。

---

## 📋 修复优先级

### 第一优先级 — 直接匹配泄漏特征（立即修复）

| # | 文件 | 行号 | 属性 | 问题 |
|---|------|------|------|------|
| 1 | FlyoutStateHelper.cs | 245 | `IsPointerOverProperty` | 全局 Subscribe，lambda 捕获实例 |
| 2 | FlyoutStateHelper.cs | 263 | `IsFocusedProperty` | 全局 Subscribe，lambda 捕获实例 |

**修复核心**: 将 `InputElement.IsPointerOverProperty.Changed.Subscribe(...)` 改为 `AnchorTarget.GetObservable(InputElement.IsPointerOverProperty).Subscribe(...)`。这样订阅绑定到具体实例，实例被 GC 时订阅自动失效。

### 第二优先级 — 覆盖旧订阅未 Dispose（本周修复）

| # | 文件 | 行号 | 属性 | 问题 |
|---|------|------|------|------|
| 3 | DatePickerPresenter.cs | 135,142 | `IsPointerInMonthViewProperty`, `IsPointerInSelectorProperty` | 覆盖旧 CompositeDisposable 未 Dispose |
| 4 | TimePickerPresenter.cs | 301 | `IsPointerInSelectorProperty` | 覆盖旧订阅未 Dispose |

**修复核心**: 在创建新订阅前先 `?.Dispose()` 旧的，并改用实例级 `GetObservable`。

### 第三优先级 — 冗余订阅优化（近期修复）

| # | 文件 | 行号 | 属性 | 问题 |
|---|------|------|------|------|
| 5 | GradientColorSlider.cs | 102,114 | `ActivatedThumbProperty` | 双重订阅冗余，改用实例级 |

---

## ✅ 检查清单

- [x] FlyoutStateHelper: `IsPointerOverProperty.Changed.Subscribe` → 改用 `AnchorTarget.GetObservable` ✅ 已修复
- [x] FlyoutStateHelper: `IsFocusedProperty.Changed.Subscribe` → 改用 `AnchorTarget.GetObservable` ✅ 已修复
- [ ] DatePickerPresenter: 添加 `_pointerDisposables?.Dispose()` + 改用实例级 `GetObservable`
- [ ] TimePickerPresenter: 添加 `_choosingStateDisposable?.Dispose()` + 改用实例级 `GetObservable`
- [ ] GradientColorSlider: 去除冗余订阅 + 改用实例级 `GetObservable`
- [ ] 运行内存分析工具验证 `LightweightSubject` 观察者数量不再增长
- [ ] 验证 Flyout hover/focus 功能正常
- [ ] 验证 DatePicker/TimePicker 选择状态正常

## 📝 InputManager.Process.Subscribe 检查结果

已检查所有 18 处 `inputManager.Process.Subscribe` 调用，**全部安全**，无泄漏风险。每处都有正确的 Dispose 清理逻辑。

| 文件 | 变量名 | 清理方式 | 状态 |
|------|--------|---------|------|
| UploadTriggerContent.cs:73 | `_clickSubscription` | OnDetachedFromVisualTree | ✓ 安全 |
| FloatButtonGroup.cs:425 | `_clickTriggerDisposable` | ConfigureTriggerType + OnAttached | ✓ 安全 |
| ToolTipService.cs:31 | `_subscriptions` | Dispose() | ✓ 安全 |
| CalendarItem.cs:1191 | `_pointerPositionDisposable` | OnDetachedFromVisualTree | ✓ 安全 |
| Popup.cs:333 | `_selfLightDismissDisposable` | HandleIsLightDismissEnabledChanged | ✓ 安全 |
| TimeView.cs:259 | `_pointerPositionDisposable` | OnDetachedFromVisualTree | ✓ 安全 |
| DefaultTreeViewInteractionHandler.cs:50 | `_inputManagerSubscription` | Detach() | ✓ 安全 |
| ButtonSpinnerDecoratedBox.cs:212 | `_mouseMoveDisposable` | ConfigureMoveProcessor + OnDetached | ✓ 安全 |
| Dialog.cs:538 | `.DisposeWith(handlerCleanup)` | CompositeDisposable | ✓ 安全 |
| SliderTrack.cs:359 | `_focusProcessDisposable` | OnDetachedFromVisualTree | ✓ 安全 |
| DefaultNavMenuInteractionHandler.cs:287 | `_inputManagerSubscription` | DetachCore() | ✓ 安全 |
| TreeViewFlyout.cs:104,161 | `_detectMouseClickDisposable` | CompositeDisposable + OnPropertyChanged | ✓ 安全 |
| PopupFlyoutBase.cs:475 | `_transientDisposable` | HandlePopupClosed() | ✓ 安全 |
| FlyoutStateHelper.cs:126 | `_flyoutCloseDetectDisposable` | HandleFlyoutClosed() | ✓ 安全 |
| FlyoutStateHelper.cs:258 | `_subscriptions` | SetupTriggerHandler + NotifyDetached | ✓ 安全 |
| AbstractScrollViewer.cs:129 | `_pointerMoveSubscription` | OnDetachedFromVisualTree | ✓ 安全 |
| AbstractRate.cs:219 | `_pointerEventHandleDisposable` | OnDetachedFromVisualTree | ✓ 安全 |
