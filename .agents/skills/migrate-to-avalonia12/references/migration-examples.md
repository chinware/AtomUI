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
| Top-level access | `.VisualRoot` | `TopLevel.GetTopLevel()` | Medium |
| Popup state | `.Host != null` | `.IsOpen` | Low |
| Selection update | `UpdateSelection()` | `SetCurrentValue()` | Low |
| Text properties | Wrong param order | Correct param order | Medium |
| Clipboard | `SetTextAsync()` | Extension method | Low |
| Utilities | `MathUtilities.Clamp()` | `Math.Clamp()` | Low |
| Extension methods | `GetVisualRoot()` with IInputRoot | `TopLevel.GetTopLevel()` | Medium |

## Common Mistakes to Avoid

1. ❌ Forgetting to add `using Avalonia.Input.Platform;` for clipboard methods
2. ❌ Using old parameter order for `GenericTextRunProperties`
3. ❌ Checking `.Host` instead of `.IsOpen` for popups
4. ❌ Not updating both `OnGotFocus` and `OnLostFocus` event parameters
5. ❌ Trying to access `.VisualRoot` directly instead of using `TopLevel.GetTopLevel()`
6. ❌ Forgetting to update custom extension methods that use removed APIs
7. ❌ Casting to `IInputRoot` when `e.Root` is already `TopLevel`
8. ❌ Using `GetPresentationSource()` which is no longer accessible
