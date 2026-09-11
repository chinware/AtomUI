# Popup Anchor 作用域检查指南

> 适用于所有带 Popup 的 anchor 控件（ColorPicker / ComboBox / Select / Cascader / TreeSelect / AutoComplete / Mentions / InfoPickerInput / 自研控件等）。

## 1. 问题

带 Popup 的 anchor 控件（Button 或 Button-like）要在两个地方判断"指针/点击位置是否在我的 popup 作用域内"：

1. **Click 模式的 `PointerPressed` handler**：点击 popup 内部（比如选颜色、选 item）时不能把 popup 关掉。
2. **Hover 模式的全局指针追踪**（订阅 `IInputManager.Process`）：指针在 popup 区域内移动时不能触发关闭定时器。

这两个场景都需要一个 `IsVisualInPickerScope(Visual visual)` 方法，判断给定的 visual 是否属于"我 + 我的 popup（含嵌套 popup）"。

**错误实现的症状**：
- 点击 popup 内的 ComboBox 选项，整个 popup 面板被一起关掉。
- Hover 模式下 popup 内打开 ComboBox dropdown，指针稍慢移向 dropdown，外层 popup 就关了。

## 2. 三档方案

按 popup 内容复杂度由低到高：

### 2.1 最简单：`Popup.IsInsidePopup`

适用于 popup 里只有自己的内容，没有 ItemsControl 容器、没有嵌套 popup。

```csharp
if (e.Source is Visual source && _popup?.IsInsidePopup(source) == true)
{
    return;
}
```

实现上 `IsInsidePopup` 就是 `popupHost.IsVisualAncestorOf(visual)`，纯 visual tree 检查。

项目里用这个的控件：TreeSelect / Select / Cascader / AutoComplete / Mentions / ComboBox / InfoPickerInput。

### 2.2 中等：logical tree 检查

适用于 popup 里有 ItemsControl（ComboBox dropdown-style 容器），但不会嵌套其它 popup。

```csharp
if (_popup is { Child: Visual popupChild } &&
    (visual == popupChild || popupChild.IsLogicalAncestorOf(visual)))
{
    return true;
}
```

**为什么要用 logical tree**：ItemsControl 生成 container 时 `LogicalParent` 直接指向 items control 本身，而 visual tree 上 container 挂在 `ItemsPresenter/ItemsPanel` 下。纯 `IsVisualAncestorOf` 追不到这种"跨层"关系，logical tree 能。

### 2.3 完整：placement-target 跳跃 + logical/visual 合查

适用于 popup 里**会开别的 popup**，比如 ColorPicker 面板里的 ComboBox、Select 里的子 Tooltip 等。

单靠 logical tree 不够，因为 popup 嵌套时：
- `OverlayPopupHost` 承载 popup 内容时把 host 的 `LogicalParent` 设成 `Popup` 控件
- 但 Popup 控件又挂在外层 popup 的 template 路径下
- 加上 ItemsControl 容器的特殊 logical 拼接，链路容易在边界断掉

**推荐直接用 `PopupUtils.IsVisualInPopupScope`**：

```csharp
using AtomUI.Desktop.Controls;

// anchor 控件内部：
private bool IsVisualInPickerScope(Visual visual)
    => PopupUtils.IsVisualInPopupScope(visual, this, _popup?.Child as Visual);
```

签名：
```csharp
internal static bool IsVisualInPopupScope(Visual? visual, Visual anchor, Visual? popupChild);
internal static Avalonia.Controls.Primitives.Popup? FindOwningPopup(Visual visual);
```

位于 `src/AtomUI.Desktop.Controls/Popup/PopupUtils.cs`。

如果需要自己实现（例如 util 所在 assembly 不方便引用），参考下面的模板：

```csharp
private bool IsVisualInPickerScope(Visual visual)
{
    // 1. anchor 子树
    if (visual == this || this.IsVisualAncestorOf(visual))
    {
        return true;
    }

    if (_popup?.Child is not Visual popupChild)
    {
        return false;
    }

    // 2. popup.Child 的 logical/visual 子树（覆盖常规 + ItemsControl 容器）
    if (visual == popupChild
        || popupChild.IsLogicalAncestorOf(visual)
        || popupChild.IsVisualAncestorOf(visual))
    {
        return true;
    }

    // 3. 嵌套 popup：找承载 visual 的 OverlayPopupHost/PopupRoot,
    //    拿它对应的 Popup.PlacementTarget, 递归往上查
    var cursor = visual;
    var guard = 0;
    while (cursor != null && guard++ < 8)
    {
        var popup = FindOwningPopup(cursor);
        if (popup == null)
        {
            break;
        }

        var target = popup.PlacementTarget
                     ?? popup.FindLogicalAncestorOfType<Control>();
        if (target == null || target == cursor)
        {
            break;
        }

        if (target == this
            || this.IsVisualAncestorOf(target)
            || target == popupChild
            || popupChild.IsLogicalAncestorOf(target)
            || popupChild.IsVisualAncestorOf(target))
        {
            return true;
        }

        cursor = target;
    }

    return false;
}

private static Avalonia.Controls.Primitives.Popup? FindOwningPopup(Visual visual)
{
    Visual? host = visual.FindAncestorOfType<OverlayPopupHost>();
    if (host == null)
    {
        host = visual.FindAncestorOfType<PopupRoot>();
    }
    if (host is not ILogical logical)
    {
        return null;
    }
    // OverlayPopupHost.LogicalParent 在 Popup.Open 里通过
    // ((ISetLogicalParent)popupHost).SetParent(this) 被设成 Popup 控件本身
    return logical.LogicalParent as Avalonia.Controls.Primitives.Popup
           ?? (host as StyledElement)?.FindLogicalAncestorOfType<Avalonia.Controls.Primitives.Popup>();
}
```

`guard` 限制 8 层是防死循环，正常嵌套深度远远用不到。

## 3. Click handler 的用法

Click 模式下 `PointerPressed` handler 要做两件事：
- 点击 **anchor** → toggle popup（打开/关闭）
- 点击 **popup 内部** → 保持打开（让 popup 内控件自己处理 click）

所以 guard 的语义是 "在 popup 作用域内但不在 anchor 上"：

```csharp
private void HandleClickModePointerPressed(object? sender, PointerPressedEventArgs e)
{
    if (!IsEnabled || !IsVisible)
    {
        return;
    }

    if (e.Source is Visual source && IsSourceInsidePickerPopup(source))
    {
        return;
    }

    if (IsPickerOpen)
    {
        HidePicker(immediately: true);
    }
    else
    {
        ShowPicker(immediately: true);
    }
}

private bool IsSourceInsidePickerPopup(Visual visual)
{
    // 明确排除 anchor 子树（anchor 上的点击要让 toggle 逻辑跑）
    if (visual == this || this.IsVisualAncestorOf(visual))
    {
        return false;
    }
    return IsVisualInPickerScope(visual);
}
```

handler 注册方式：
```csharp
AddHandler(InputElement.PointerPressedEvent, HandleClickModePointerPressed,
    RoutingStrategies.Tunnel, handledEventsToo: true);
```

**Tunnel + handledEventsToo** 是关键：popup 内部点击的事件会沿 popup host → Popup → PlacementTarget 一路冒泡回 anchor，此时其它 handler 可能已经 `e.Handled = true`，但我们仍然需要接到事件做 scope 判断。

## 4. Hover handler 的用法

Hover 模式在 popup 打开后订阅 `IInputManager.Process`，每次指针移动 hit-test 判断是否还在作用域内：

```csharp
private void HandleGlobalPointerMove(RawInputEventArgs e)
{
    if (e is not RawPointerEventArgs pe || pe.Type != RawPointerEventType.Move)
    {
        return;
    }

    var hitElement = pe.GetInputHitTestResult().element;
    if (hitElement is not Visual visual)
    {
        return;
    }

    if (IsVisualInPickerScope(visual))
    {
        StopMouseLeaveTimer();
    }
    else if (!IsPointerOver)
    {
        if (_mouseLeaveDelayTimer == null && IsPickerOpen)
        {
            StartMouseLeaveTimer();
        }
    }
}
```

订阅/取消时机：popup 开 → 订阅，popup 关 / 控件 detach → 取消。

## 5. 为什么单棵树不够

Avalonia 里 popup 的连接关系**是三条线**叠出来的：

1. **Visual tree**：popup 内容在 `PopupOverlayLayer`（或独立 `PopupRoot` 窗口）里，和外层 anchor 在 visual tree 上是**兄弟**而不是后代。
2. **Logical tree**：`Popup.SetPopupParent(placementTarget)` 和 template 关系把 Popup 控件挂回 placement target，但中间会经过 `OverlayPopupHost` 这种中转站，ItemsControl 的 container 又有自己的 logical parent 规则，链路不保证一路通到 `popup.Child`。
3. **Event routing**：`OverlayPopupHost.InteractiveParent => Parent as Interactive` 让事件路由沿 `LogicalParent` 链冒泡到 Popup 控件 → 再用 placement target 跳回 anchor。**这才是真正的连接点**，也是嵌套 popup 事件能触达外层 anchor 的机制。

所以正确的 scope 检查方式就是**模拟事件路由的跳跃**：沿 `popup host → Popup 控件 → PlacementTarget` 一级一级跳，每跳一层检查是不是已经进到我的作用域。单靠 `IsVisualAncestorOf` 或 `IsLogicalAncestorOf` 都会漏情况。

## 6. 选档决策树

```
popup 内有会开 popup 的控件（ComboBox / Select / Cascader / Tooltip ...）?
├─ 是 → §2.3 placement-target 跳跃版
└─ 否
   └─ popup 内有 ItemsControl 容器 / TemplatedControl 非对齐结构?
      ├─ 是 → §2.2 IsLogicalAncestorOf
      └─ 否 → §2.1 IsInsidePopup
```

不确定就直接上完整版（§2.3），兼容所有场景，开销可接受（guard 8 层封顶）。

## 7. 参考实现

- `src/AtomUI.Desktop.Controls/Popup/PopupUtils.cs` — `IsVisualInPopupScope` / `FindOwningPopup` 共享 util（§2.3 完整版），直接调用即可
- `src/AtomUI.Desktop.Controls.ColorPicker/AbstractColorPicker.cs` — 使用 util 的消费方示例
- `src/AtomUI.Desktop.Controls/Flyouts/FlyoutStateHelper.cs` — 简版（§2.1 + §2.2），适用于 Flyout 承载的简单内容

## 8. 常见反模式

**不要做**：基于"popup 在不同 visual 子树，事件应该到不了 anchor"的推理，把 scope guard 判成死代码删掉。Avalonia 的事件路由走 logical/Interactive 链，popup 内部事件确实能到 anchor，guard 必须保留。

**不要做**：只用 `IsInsidePopup` 就以为万事大吉。一旦 popup 内出现 ItemsControl 或嵌套 popup 就会出 bug，而且是"点了才关、不点不出"的间歇性问题，容易漏测。

**不要做**：把 guard 粒度定得过粗（比如所有 popup 内的点击都 return）。Click handler 必须区分"点 anchor"（要 toggle）和"点 popup 内部"（要忽略），两种语义不能合并。
