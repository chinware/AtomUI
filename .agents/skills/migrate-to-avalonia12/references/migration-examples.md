# Avalonia 12 Migration Examples

## Example 1: Focus Events Migration

### Before (Avalonia 11)
```csharp
protected override void OnGotFocus(GotFocusEventArgs e)
{
    base.OnGotFocus(e);
    UpdateCommandStates();
}

protected override void OnLostFocus(RoutedEventArgs e)
{
    base.OnLostFocus(e);
    ClearSelection();
}
```

### After (Avalonia 12)
```csharp
protected override void OnGotFocus(FocusChangedEventArgs e)
{
    base.OnGotFocus(e);
    UpdateCommandStates();
}

protected override void OnLostFocus(FocusChangedEventArgs e)
{
    base.OnLostFocus(e);
    ClearSelection();
}
```

### Key Changes
- Both events now use `FocusChangedEventArgs`
- Method signatures remain the same
- No logic changes needed

---

## Example 2: TopLevel API Migration

### Before (Avalonia 11)
```csharp
var root = this.VisualRoot as IRenderRoot;
if (root != null)
{
    var scaling = root.RenderScaling;
}
```

### After (Avalonia 12)
```csharp
var topLevel = TopLevel.GetTopLevel(this);
if (topLevel is WindowBase window)
{
    var scaling = window.DesktopScaling;
}
```

### Key Changes
- Use `TopLevel.GetTopLevel(visual)` instead of `.VisualRoot`
- Check for `WindowBase` instead of `IRenderRoot`
- Access properties through the returned TopLevel object

---

## Example 3: Popup.Host Migration

### Before (Avalonia 11)
```csharp
if (popup.Host != null)
{
    popup.Host.Close();
}

if (navMenuItem._popup?.Host != null)
{
    await navMenuItem._popup.MotionAwareCloseAsync();
}
```

### After (Avalonia 12)
```csharp
if (popup.IsOpen)
{
    popup.IsOpen = false;
}

if (navMenuItem._popup?.IsOpen == true)
{
    navMenuItem._popup.IsOpen = false;
    await Task.Delay(50);  // Allow animation to complete
}
```

### Key Changes
- Replace `.Host != null` with `.IsOpen`
- Replace `.Host.Close()` with `.IsOpen = false`
- Use `Task.Delay()` instead of `MotionAwareCloseAsync()`

---

## Example 4: UpdateSelection Migration

### Before (Avalonia 11)
```csharp
protected void SelectChildItem(NavMenuItem child, bool isSelected)
{
    UpdateSelection(child, isSelected);
}
```

### After (Avalonia 12)
```csharp
protected void SelectChildItem(NavMenuItem child, bool isSelected)
{
    child.SetCurrentValue(IsSelectedProperty, isSelected);
}
```

### Key Changes
- Replace `UpdateSelection()` with `SetCurrentValue()`
- Pass the property and value directly
- More explicit and type-safe

---

## Example 5: GenericTextRunProperties Migration

### Before (Avalonia 11)
```csharp
var defaultProperties = new GenericTextRunProperties(
    typeface,
    FontFeatures,
    FontSize,
    TextDecorations,
    Foreground);
```

### After (Avalonia 12)
```csharp
var defaultProperties = new GenericTextRunProperties(
    typeface,
    FontSize,
    TextDecorations,
    Foreground,
    fontFeatures: FontFeatures);
```

### Key Changes
- Parameter order: `typeface, fontSize, textDecorations, foregroundBrush, fontFeatures`
- Use named parameter for `fontFeatures`
- FontSize comes before FontFeatures

---

## Example 6: Clipboard API Migration

### Before (Avalonia 11)
```csharp
var text = GetSelection();
if (!string.IsNullOrEmpty(text))
{
    var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
    if (clipboard != null)
    {
        await clipboard.SetTextAsync(text);
    }
}
```

### After (Avalonia 12)
```csharp
var text = GetSelection();
if (!string.IsNullOrEmpty(text))
{
    var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
    if (clipboard != null)
    {
        // SetTextAsync is now an extension method
        await clipboard.SetTextAsync(text);
    }
}
```

### Key Changes
- `SetTextAsync()` is now an extension method in `Avalonia.Input.Platform`
- Add `using Avalonia.Input.Platform;`
- For custom data, use `SetDataAsync()` with `DataTransfer`

---

## Example 7: MathUtilities.Clamp Migration

### Before (Avalonia 11)
```csharp
point = new Point(
    MathUtilities.Clamp(point.X, 0, Math.Max(TextLayout.WidthIncludingTrailingWhitespace, 0)),
    MathUtilities.Clamp(point.Y, 0, Math.Max(TextLayout.Height, 0)));
```

### After (Avalonia 12)
```csharp
point = new Point(
    Math.Clamp(point.X, 0, Math.Max(TextLayout.WidthIncludingTrailingWhitespace, 0)),
    Math.Clamp(point.Y, 0, Math.Max(TextLayout.Height, 0)));
```

### Key Changes
- Replace `MathUtilities.Clamp()` with `Math.Clamp()`
- No import changes needed (built-in .NET method)
- Identical functionality

---

## Example 8: Extension Methods & Helper Utilities Migration

### Before (Avalonia 11)
```csharp
// In VisualExtensions.cs
internal static Visual? GetVisualRoot(this Visual visual)
{
    return visual.GetPresentationSource()?.RootVisual;
}

// Usage in service classes
if (Presenter?.GetVisualRoot() != null)
{
    UpdateLayout();
}

if (e.Root == currentTip.GetVisualRoot() as IInputRoot)
{
    isTooltipEvent = true;
}

public void Update(IInputRoot root, Visual? candidateToolTipHost)
{
    if (root == currentToolTip?.GetVisualRoot() as IInputRoot)
    {
        return;
    }
}
```

### After (Avalonia 12)
```csharp
// Option 1: Update extension to use TopLevel
internal static TopLevel? GetVisualRootTopLevel(this Visual visual)
{
    return TopLevel.GetTopLevel(visual);
}

// Option 2: Replace all usages directly with TopLevel.GetTopLevel()
if (Presenter != null && TopLevel.GetTopLevel(Presenter) != null)
{
    UpdateLayout();
}

if (e.Root == TopLevel.GetTopLevel(currentTip))
{
    isTooltipEvent = true;
}

public void Update(TopLevel root, Visual? candidateToolTipHost)
{
    if (root == TopLevel.GetTopLevel(currentToolTip as Visual))
    {
        return;
    }
}
```

### Key Changes
- Replace `GetVisualRoot()` extension with `TopLevel.GetTopLevel()`
- Replace `IInputRoot` parameter type with `TopLevel`
- Remove `as IInputRoot` casts
- `e.Root` in RawInputEventArgs is now `TopLevel` directly
- GetPresentationSource() is no longer accessible

---

## Migration Pattern Summary

| Pattern | Avalonia 11 | Avalonia 12 | Complexity |
|---------|------------|-----------|-----------|
| Event parameters | `GotFocusEventArgs` | `FocusChangedEventArgs` | Low |
| Top-level access | `.VisualRoot` (public) | `TopLevel.GetTopLevel()` (VisualRoot now protected internal) | Medium |
| Popup state | `.Host != null` | `.IsOpen` | Low |
| Popup placement | `.PlacementMode` | `.Placement` | Low |
| Selection update | `UpdateSelection()` | `SetCurrentValue()` | Low |
| Text properties | Wrong param order | Correct param order | Medium |
| Clipboard | `SetTextAsync()` | Extension method | Low |
| Utilities | `MathUtilities.Clamp()` | `Math.Clamp()` (both work) | Low |
| Extension methods | `GetVisualRoot()` with IInputRoot | `TopLevel.GetTopLevel()` | Medium |
| Gestures | `Gestures.TappedEvent` | `InputElement.TappedEvent` | Low |
| IPopupHost | `popup.Host` | `popup.IsOpen` | Low |
| Focus navigation | `KeyboardNavigationHandler.GetNext()` | `FocusManager.GetNextElement()` | Medium |
| Window decorations | `TitleBar`, `CaptionButtons` | `WindowDrawnDecorations` | High |
| Renamed members | `TextBox.Watermark` | `TextBox.PlaceholderText` | Low |
| Dispatcher | `Dispatcher.UIThread` | `AvaloniaObject.Dispatcher` | Medium |

---

## Example 9: Popup Placement Migration

### Before (Avalonia 11)
```csharp
popup.PlacementMode = PlacementMode.Bottom;
contextMenu.PlacementMode = PlacementMode.Right;
```

### After (Avalonia 12)
```csharp
popup.Placement = PlacementMode.Bottom;
contextMenu.Placement = PlacementMode.Right;
// Note: PlacementMode enum is unchanged, only the property name changed
```

### XAML Before
```xml
<Popup PlacementMode="Bottom" />
```

### XAML After
```xml
<Popup Placement="Bottom" />
```

### Key Changes
- Property renamed from `PlacementMode` to `Placement`
- `PlacementMode` enum itself is NOT removed
- Enum values remain the same

---

## Example 10: Gestures Class Migration

### Before (Avalonia 11)
```csharp
control.AddHandler(Gestures.TappedEvent, OnTapped);
control.AddHandler(Gestures.DoubleTappedEvent, OnDoubleTapped);
control.AddHandler(Gestures.RightTappedEvent, OnRightTapped);
```

### After (Avalonia 12)
```csharp
control.AddHandler(InputElement.TappedEvent, OnTapped);
control.AddHandler(InputElement.DoubleTappedEvent, OnDoubleTapped);
control.AddHandler(InputElement.RightTappedEvent, OnRightTapped);
```

### XAML Before
```xml
<Button Gestures.Tapped="OnTapped" />
```

### XAML After
```xml
<Button Tapped="OnTapped" />
```

### Key Changes
- `Gestures` class is now `internal`
- All events moved to `InputElement`
- Remove `Gestures.` prefix in XAML

---

## Example 11: IPopupHost Migration

### Before (Avalonia 11)
```csharp
IPopupHost host = popup.Host;
if (host != null)
{
    host.Close();
}
```

### After (Avalonia 12)
```csharp
if (popup.IsOpen)
{
    popup.IsOpen = false;
}
```

### Key Changes
- `IPopupHost` is now `internal`
- `Popup.Host` property removed
- Use `Popup.IsOpen` for state management

---

## Example 12: KeyboardNavigationHandler Migration

### Before (Avalonia 11)
```csharp
var next = KeyboardNavigationHandler.GetNext(currentElement, NavigationDirection.Next);
```

### After (Avalonia 12)
```csharp
var focusManager = TopLevel.GetTopLevel(currentElement)?.FocusManager;
var next = focusManager?.GetNextElement(currentElement, NavigationDirection.Next);
```

### Key Changes
- `KeyboardNavigationHandler` is now `internal`
- Use `FocusManager.GetNextElement()` instead

---

## Example 13: Renamed Members Migration

### Before (Avalonia 11)
```csharp
textBox.Watermark = "Enter text...";
textBox.UseFloatingWatermark = true;
window.SystemDecorations = SystemDecorations.Full;
```

### After (Avalonia 12)
```csharp
textBox.PlaceholderText = "Enter text...";
textBox.UseFloatingPlaceholder = true;
window.WindowDecorations = WindowDecorations.Full;
```

### Key Changes
- `Watermark` → `PlaceholderText`
- `UseFloatingWatermark` → `UseFloatingPlaceholder`
- `SystemDecorations` → `WindowDecorations`

---

## Example 14: Window Decorations Migration

### Before (Avalonia 11)
```xml
<Window>
    <Chrome:TitleBar />
    <Chrome:CaptionButtons />
</Window>
```
```csharp
var layer = VisualLayerManager.ChromeOverlayLayer;
var adorner = VisualLayerManager.AdornerLayer;
```

### After (Avalonia 12)
```xml
<Window>
    <Chrome:WindowDrawnDecorations />
</Window>
```
```csharp
// ChromeOverlayLayer removed — use WindowDrawnDecorations
var adorner = AdornerLayer.GetAdornerLayer(visual);
var overlay = OverlayLayer.GetOverlayLayer(visual);
```

### Key Changes
- `TitleBar`, `CaptionButtons`, `ChromeOverlayLayer` removed
- Use `WindowDrawnDecorations` template-based system
- Layer access via static methods instead of VisualLayerManager properties

---

## Example 15: Dispatcher Migration

### Before (Avalonia 11)
```csharp
Dispatcher.UIThread.InvokeAsync(() =>
{
    UpdateUI();
});
```

### After (Avalonia 12)
```csharp
// In control code, use the object's own dispatcher
this.Dispatcher.InvokeAsync(() =>
{
    UpdateUI();
});

// Or use current thread's dispatcher
Dispatcher.CurrentDispatcher.InvokeAsync(() =>
{
    UpdateUI();
});
```

### Key Changes
- Multiple dispatchers now supported (one per thread)
- Use `AvaloniaObject.Dispatcher` in control code
- `Dispatcher.UIThread` still works but less precise

---

## Common Mistakes to Avoid

1. ❌ Forgetting to add `using Avalonia.Input.Platform;` for clipboard methods
2. ❌ Using old parameter order for `GenericTextRunProperties`
3. ❌ Checking `.Host` instead of `.IsOpen` for popups
4. ❌ Not updating both `OnGotFocus` and `OnLostFocus` event parameters
5. ❌ Trying to access `.VisualRoot` directly instead of using `TopLevel.GetTopLevel()`
6. ❌ Forgetting to update custom extension methods that use removed APIs
7. ❌ Casting to `IInputRoot` when `e.Root` is already `TopLevel`
8. ❌ Using `GetPresentationSource()` which is no longer accessible externally
9. ❌ Claiming `PlacementMode` enum is removed — only the property name changed
10. ❌ Claiming `Visual.VisualRoot` is removed — it's `protected internal`
11. ❌ Claiming `MathUtilities.Clamp` is removed — it still exists
12. ❌ Using `Gestures.TappedEvent` — class is now `internal`, use `InputElement.TappedEvent`
13. ❌ Using `IPopupHost` directly — now `internal`, use `Popup.IsOpen`
14. ❌ Using `KeyboardNavigationHandler.GetNext()` — now `internal`, use `FocusManager`
15. ❌ Using old property names (`Watermark`, `PlacementMode`, `SystemDecorations`)
16. ❌ Using `TitleBar`/`CaptionButtons` — removed, use `WindowDrawnDecorations`
17. ❌ Assuming single global dispatcher — Avalonia 12 supports multiple
