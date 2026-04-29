# Avalonia 12 Breaking Changes - Code Level Analysis

基于官方 Avalonia 源码库 (11.3.14 → 12.0.0) 的代码层面分析

## 1. Binding System Overhaul

**Commit:** f2fc8d02f4 - "Move `IBinding` to `BindingBase` and tidy up some binding APIs (#19589)"

### Removed Interfaces & Classes
```csharp
// REMOVED: IBinding interface
public interface IBinding
{
    InstancedBinding? Initiate(
        AvaloniaObject target,
        AvaloniaProperty? targetProperty,
        object? anchor,
        bool enableDataValidation);
}

// REMOVED: IBinding2 interface
public interface IBinding2 : IBinding { }

// REMOVED: InstancedBinding class (172 lines)
public sealed class InstancedBinding
{
    public BindingMode Mode { get; }
    public BindingPriority Priority { get; }
    public IObservable<object?> Observable { get; }
}
```

### New Structure
```csharp
// NEW: BindingBase as true base class
public abstract class BindingBase
{
    public abstract InstancedBinding? CreateInstance(
        AvaloniaObject target,
        AvaloniaProperty? targetProperty,
        object? anchor = null,
        bool enableDataValidation = true);
}

// NEW: ReflectionBinding (moved from Binding in Avalonia.Markup)
public sealed class ReflectionBinding : BindingBase
{
    public ReflectionBinding(string path, BindingMode mode = BindingMode.Default, ...)
    {
        // Implementation
    }
}

// RENAMED: Binding → ReflectionBinding
// Compatibility shim remains in Avalonia.Markup
```

### Impact on AtomUI
- Any code using `IBinding` must use `BindingBase`
- `InstancedBinding` usage must be refactored
- `Binding` class should be replaced with `ReflectionBinding`

---

## 2. Focus Events Unification

**Commit:** b612be79db - "Add FocusChangedEventArgs for GotFocus/LostFocus (#20859)"

### Removed Classes
```csharp
// REMOVED: GotFocusEventArgs
public class GotFocusEventArgs : RoutedEventArgs, IKeyModifiersEventArgs
{
    public NavigationMethod NavigationMethod { get; init; }
    public KeyModifiers KeyModifiers { get; init; }
}
```

### New Unified Class
```csharp
// NEW: FocusChangedEventArgs (used for both GotFocus and LostFocus)
public class FocusChangedEventArgs : RoutedEventArgs, IKeyModifiersEventArgs
{
    public FocusChangedEventArgs(RoutedEvent routedEvent) : base(routedEvent) { }
    
    public IInputElement? NewFocusedElement { get; init; }
    public IInputElement? OldFocusedElement { get; init; }
    public NavigationMethod NavigationMethod { get; init; }
    public KeyModifiers KeyModifiers { get; init; }
}
```

### Method Signature Changes
```csharp
// BEFORE
protected override void OnGotFocus(GotFocusEventArgs e)
protected override void OnLostFocus(RoutedEventArgs e)

// AFTER
protected override void OnGotFocus(FocusChangedEventArgs e)
protected override void OnLostFocus(FocusChangedEventArgs e)
```

### Files Changed
- 29 files updated across Avalonia.Controls
- All controls using focus events must update signatures

---

## 3. TopLevel Architecture Refactoring

**Commit:** fa17c22131 - "Introduce PresentationSource, move some responsibilities from TopLevel (#20624)"

### Removed Interfaces
```csharp
// REMOVED: IRenderRoot interface (42 lines)
public interface IRenderRoot
{
    Size ClientSize { get; }
    IRenderer Renderer { get; }
    IHitTester HitTester { get; }
    double RenderScaling { get; }
    Point PointToClient(PixelPoint point);
    Point PointToScreen(Point point);
}

// REMOVED: ILayoutRoot interface (moved to PresentationSource)
public interface ILayoutRoot
{
    ILayoutManager LayoutManager { get; }
}

// REMOVED: IEmbeddedLayoutRoot interface
public interface IEmbeddedLayoutRoot : ILayoutRoot { }

// REMOVED: ITextInputMethodRoot interface
public interface ITextInputMethodRoot { }
```

### New Architecture
```csharp
// NEW: IPresentationSource interface
public interface IPresentationSource
{
    IInputRoot InputRoot { get; }
    ILayoutRoot LayoutRoot { get; }
    IRenderer Renderer { get; }
}

// NEW: PresentationSource class (116 lines)
public class PresentationSource : IPresentationSource
{
    public IInputRoot InputRoot { get; }
    public ILayoutRoot LayoutRoot { get; }
    public IRenderer Renderer { get; }
}
```

### Visual.VisualRoot Removal
```csharp
// REMOVED: Visual.VisualRoot property
public class Visual : AvaloniaObject
{
    // REMOVED
    public Visual? VisualRoot { get; }
}

// NEW: Use TopLevel.GetTopLevel() static method
public static class TopLevel
{
    public static TopLevel? GetTopLevel(Visual visual)
    {
        return visual.GetPresentationSource()?.RootVisual as TopLevel;
    }
}
```

### Files Changed
- 100+ files updated
- All VisualRoot usages must be replaced
- All IRenderRoot casts must be removed
- All ILayoutRoot references must be updated

---

## 4. Clipboard & Drag-Drop API Rewrite

**Commit:** 7b54818918 - "Remove IDataObject to IDataTransfer wrappers and related methods (#20521)"

### Removed Classes & Interfaces
```csharp
// REMOVED: IDataObject interface (32 lines)
public interface IDataObject
{
    bool Contains(string dataFormat);
    object? Get(string dataFormat);
    IEnumerable<string> GetDataFormats();
    IEnumerable<string> GetFileNames();
    string? GetText();
}

// REMOVED: DataObject class (45 lines)
public class DataObject : IDataObject
{
    public void Set(string dataFormat, object value);
    public void SetText(string text);
    public void SetFileNames(IEnumerable<string> fileNames);
}

// REMOVED: Wrapper classes
public class DataObjectToDataTransferWrapper { }
public class DataObjectToDataTransferItemWrapper { }
public class DataTransferToDataObjectWrapper { }
```

### New API
```csharp
// NEW: DataTransfer class
public class DataTransfer
{
    public void Add(DataTransferItem item);
    public IReadOnlyList<DataTransferItem> Items { get; }
}

// NEW: DataTransferItem class
public class DataTransferItem
{
    public void Set(DataFormat format, object value);
    public object? Get(DataFormat format);
}

// NEW: DataFormat class (replaces DataFormats)
public static class DataFormat
{
    public static readonly string Text = "text";
    public static readonly string Files = "files";
    // ... other formats
}
```

### Method Changes
```csharp
// BEFORE
public interface IClipboard
{
    Task SetDataObjectAsync(IDataObject data);
    Task<string?> GetTextAsync();
}

public static class DragDrop
{
    public static void DoDragDrop(DragEventArgs args, IDataObject data);
}

// AFTER
public interface IClipboard
{
    Task SetDataAsync(IAsyncDataTransfer data);
    Task<string?> TryGetTextAsync();
}

public static class DragDrop
{
    public static Task DoDragDropAsync(DragEventArgs args, IAsyncDataTransfer data);
}
```

### DragEventArgs Changes
```csharp
// BEFORE
public class DragEventArgs : RoutedEventArgs
{
    public IDataObject Data { get; }
}

// AFTER
public class DragEventArgs : RoutedEventArgs
{
    public IAsyncDataTransfer DataTransfer { get; }
}
```

---

## 5. Obsolete Members Removal

**Commits:**
- cac4650c98 - "Remove obsolete members from Avalonia.Base (#20613)"
- 02c5ff41ff - "Remove obsolete members from Avalonia.Controls (#20617)"
- 61f8103726 - "Remove obsolete members from various assemblies (#20628)"

### Removed from Avalonia.Base
```csharp
// REMOVED: CubicBezierEasing class
public class CubicBezierEasing : Easing { }

// REMOVED: CustomAnimatorBase class
public abstract class CustomAnimatorBase { }

// REMOVED: BindingPriority.TemplatedParent
public enum BindingPriority
{
    // REMOVED: TemplatedParent = 1000
}

// REMOVED: IStyleable interface
public interface IStyleable
{
    Type StyleKey { get; }
    IStyleHost? StylingParent { get; }
}

// REMOVED: RadialGradientBrush.Radius property
public class RadialGradientBrush : GradientBrush
{
    // REMOVED: public double Radius { get; set; }
}

// REMOVED: Color.ToUint32() method
public struct Color
{
    // REMOVED: public uint ToUint32()
}

// REMOVED: DrawingContext.PushPreTransform/PushPostTransform
public abstract class DrawingContext
{
    // REMOVED: public void PushPreTransform(Matrix matrix)
    // REMOVED: public void PushPostTransform(Matrix matrix)
}
```

### Removed from Avalonia.Controls
```csharp
// REMOVED: IActivatableApplicationLifetime interface
public interface IActivatableApplicationLifetime { }

// REMOVED: ItemContainerGenerator methods
public class ItemContainerGenerator
{
    // REMOVED: public Control? ContainerFromIndex(int index)
    // REMOVED: public int IndexFromContainer(Control container)
}

// REMOVED: TreeItemContainerGenerator class
public class TreeItemContainerGenerator { }

// REMOVED: SystemDialog class (206 lines)
public class SystemDialog { }

// REMOVED: Screen.PixelDensity and Screen.Primary
public abstract class Screen
{
    // REMOVED: public double PixelDensity { get; }
    // REMOVED: public static Screen? Primary { get; }
}

// REMOVED: Screens.ScreenFromWindow
public static class Screens
{
    // REMOVED: public static Screen? ScreenFromWindow(Window window)
}

// REMOVED: Popup.PlacementMode property
public class Popup : Control
{
    // REMOVED: public PlacementMode PlacementMode { get; set; }
}

// REMOVED: ToggleButton.Checked/Unchecked/Indeterminate events
public class ToggleButton : Button
{
    // REMOVED: public event EventHandler<RoutedEventArgs>? Checked
    // REMOVED: public event EventHandler<RoutedEventArgs>? Unchecked
    // REMOVED: public event EventHandler<RoutedEventArgs>? Indeterminate
}

// REMOVED: AppBuilder.LifetimeOverride property
public class AppBuilder
{
    // REMOVED: public object? LifetimeOverride { get; set; }
}

// REMOVED: IApplicationPlatformEvents interface
public interface IApplicationPlatformEvents { }
```

---

## 6. Platform-Specific Removals

### Android Changes
```csharp
// REMOVED: IAndroidView interface
public interface IAndroidView { }

// CHANGED: AvaloniaMainActivity
// BEFORE
public class AvaloniaMainActivity<TApp> : Activity where TApp : Application, new()
{
    // Generic implementation
}

// AFTER
public class AvaloniaMainActivity : Activity
{
    // Non-generic, uses AvaloniaAndroidApplication
}

// NEW: AvaloniaAndroidApplication
public class AvaloniaAndroidApplication<TApp> : Application where TApp : Application, new()
{
    // Handles app initialization
}
```

### Removed Platforms
```csharp
// REMOVED: Tizen support entirely
// REMOVED: Avalonia.Browser.Blazor package
// REMOVED: ReactiveUI integration
```

---

## 7. Text Formatting Constructor Changes

**Impact:** GenericTextRunProperties, TextCollapsingProperties, TextShaperOptions

```csharp
// BEFORE (Avalonia 11)
public class GenericTextRunProperties : TextRunProperties
{
    public GenericTextRunProperties(
        Typeface typeface,
        FontFeatureCollection? fontFeatures,
        double fontSize,
        TextDecorationCollection? textDecorations,
        IBrush? foregroundBrush)
    {
    }
}

// AFTER (Avalonia 12)
public class GenericTextRunProperties : TextRunProperties
{
    public GenericTextRunProperties(
        Typeface typeface,
        double fontSize,
        TextDecorationCollection? textDecorations,
        IBrush? foregroundBrush,
        FontFeatureCollection? fontFeatures = null)
    {
    }
}
```

### Parameter Order Change
- Old: `typeface, fontFeatures, fontSize, textDecorations, foregroundBrush`
- New: `typeface, fontSize, textDecorations, foregroundBrush, fontFeatures`

---

## 8. Direct2D1 Removal

**Commit:** 246cbfc566 - "Remove Direct2D1 (#20132)"

```csharp
// REMOVED: Avalonia.Direct2D1 package entirely
// REMOVED: Direct2D1 rendering backend

// MUST USE: Skia instead
AppBuilder.Configure<App>()
    .UseSkia()  // Required
    .UseHarfBuzz()  // Also required for text
```

---

## 9. Diagnostics Package Change

```csharp
// REMOVED: Avalonia.Diagnostics package
// NEW: AvaloniaUI.DiagnosticsSupport package

// BEFORE
using Avalonia.Diagnostics;
public void AttachDevTools() { }

// AFTER
using AvaloniaUI.DiagnosticsSupport;
public void AttachDeveloperTools() { }
```

---

## Summary of Code-Level Changes

| Category | Removed | New | Files Changed |
|----------|---------|-----|---------------|
| Binding System | IBinding, IBinding2, InstancedBinding | BindingBase, ReflectionBinding | 85 |
| Focus Events | GotFocusEventArgs | FocusChangedEventArgs | 29 |
| TopLevel | IRenderRoot removed, ILayoutRoot/IInputRoot internalized | IPresentationSource, PresentationSource | 100+ |
| Clipboard | IDataObject, DataObject, wrappers | DataTransfer, DataTransferItem, DataFormat | 23 |
| Obsolete | 50+ members | - | 26 |
| Platform | Tizen, Blazor, IAndroidView | AvaloniaAndroidApplication | 10+ |
| Text | Old constructor order | New parameter order | 5+ |
| Rendering | Direct2D1 | Skia only | 1 |
| Diagnostics | Avalonia.Diagnostics | AvaloniaUI.DiagnosticsSupport | 1 |
| Internalized | Gestures, BindingPlugins, IPopupHost, KeyboardNavigationHandler | InputElement events, FocusManager | 20+ |
| Window Decorations | TitleBar, CaptionButtons, ChromeOverlayLayer, SystemDecorations | WindowDrawnDecorations, DrawnWindowDecorationParts | 30+ |
| Renamed | Popup.PlacementMode, TextBox.Watermark, Window.SystemDecorations, etc. | Popup.Placement, PlaceholderText, WindowDecorations | 15+ |
| Dispatcher | Single global | Multiple per-thread | 10+ |

---

## Key Takeaways for AtomUI Migration

1. **Binding System**: Complete refactor needed if using IBinding directly
2. **Focus Events**: All OnGotFocus/OnLostFocus must use FocusChangedEventArgs
3. **TopLevel**: Replace all external VisualRoot usage with TopLevel.GetTopLevel() (VisualRoot is now `protected internal`)
4. **Clipboard**: Complete API rewrite for drag-drop operations
5. **Text Formatting**: Parameter order change in GenericTextRunProperties
6. **Internalized APIs**: Gestures, BindingPlugins, IPopupHost, KeyboardNavigationHandler are now internal — use public replacements
7. **Window Decorations**: Complete redesign — TitleBar/CaptionButtons/ChromeOverlayLayer removed, use WindowDrawnDecorations
8. **Renamed Members**: Many property renames (PlacementMode→Placement, Watermark→PlaceholderText, etc.)
9. **IInputRoot**: Now [PrivateApi], not removed — use TopLevel or IPresentationSource instead
10. **PlacementMode**: Enum still exists, only property accessor renamed (Popup.PlacementMode → Popup.Placement)
6. **Obsolete Members**: Remove all deprecated API usage
7. **Platform**: Update Android initialization if applicable
8. **Rendering**: Ensure Skia + HarfBuzz configuration

