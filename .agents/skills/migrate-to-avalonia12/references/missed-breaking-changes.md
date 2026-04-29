# Avalonia 12 Migration — Corrections & Newly Discovered Changes

## Corrections to Previous Analysis

### 1. PlacementMode is NOT Removed (Correction)

**Previous claim:** PlacementMode enum completely removed.
**Actual:** The `PlacementMode` enum still exists. Only the property accessor was renamed:
- `Popup.PlacementMode` → `Popup.Placement`
- `ContextMenu.PlacementMode` → `ContextMenu.Placement`

**Verified in source:** `.referenceprojects/Avalonia/src/Avalonia.Controls/PlacementMode.cs` — public enum, no `[Obsolete]` attribute.

**Impact on AtomUI:** `PopupUtils.cs` code using `PlacementMode` enum values in switch statements is fine. Only property access like `popup.PlacementMode = ...` needs to change to `popup.Placement = ...`.

---

### 2. Visual.VisualRoot is NOT Removed (Correction)

**Previous claim:** VisualRoot removed.
**Actual:** Changed from `public` to `protected internal`. Still exists at `Visual.cs:348`:
```csharp
protected internal Visual? VisualRoot => PresentationSource?.RootVisual;
```

**Impact:** Code inside Avalonia or derived classes can still access it. External code must use `TopLevel.GetTopLevel(visual)`.

---

### 3. MathUtilities.Clamp Still Exists (Correction)

**Previous claim:** Implied removal.
**Actual:** All overloads (double, decimal, float, int) still present in `MathUtilities.cs`. `Math.Clamp()` is preferred but not required.

---

### 4. IInputRoot is [PrivateApi], Not Removed (Correction)

**Previous claim:** IInputRoot interface removed.
**Actual:** Marked `[PrivateApi]` — still exists internally. External code should not use it.

**Verified in source:** `.referenceprojects/Avalonia/src/Avalonia.Base/Input/IInputRoot.cs`

---

### 5. BindingPlugins is Internal, Not Removed (Correction)

**Previous claim:** Binding plugins removed.
**Actual:** `BindingPlugins` class changed from `public` to `internal`.

**Verified in source:** `internal static class BindingPlugins`

---

## Newly Discovered Breaking Changes

### 6. Gestures Class Now Internal (HIGH)

**Discovery:** Source scan found `internal static class Gestures` in Avalonia 12.
**Impact:** Any code referencing `Gestures.TappedEvent`, `Gestures.DoubleTappedEvent`, etc. will fail.
**Fix:** Use `InputElement.TappedEvent`, `InputElement.DoubleTappedEvent` instead.

---

### 7. IPopupHost Now Internal (HIGH)

**Discovery:** Source scan found `internal interface IPopupHost` in Avalonia 12.
**Impact:** Code using `IPopupHost` directly (e.g., `popup.Host`) will fail.
**Fix:** Use `Popup.IsOpen` for state management.

---

### 8. KeyboardNavigationHandler Now Internal (HIGH)

**Discovery:** Source scan found `internal sealed class KeyboardNavigationHandler`.
**Impact:** Code using `KeyboardNavigationHandler.GetNext()` will fail.
**Fix:** Use `FocusManager.GetNextElement()`.

---

### 9. IRenderer Now [PrivateApi] (MEDIUM)

**Discovery:** `IRenderer` interface marked as private API.
**Impact:** Direct renderer access no longer supported for external code.
**Fix:** Use higher-level APIs like `InvalidateVisual()`.

---

### 10. Window Decoration Architecture Redesign (HIGH)

**Discovery:** Far more extensive than previously documented. Full list of removed types:
- `Chrome.TitleBar`
- `Chrome.CaptionButtons`
- `ChromeOverlayLayer`
- `LightDismissOverlayLayer`
- `SystemDecorations` enum
- `ExtendClientAreaChromeHints` enum
- `IPopupHostProvider` interface

New replacement system:
- `WindowDrawnDecorations` — template-based decoration manager
- `WindowDrawnDecorationsContent` — template content
- `IWindowDrawnDecorationsTemplate` — template interface
- `DrawnWindowDecorationParts` enum
- `WindowDecorationsElementRole` enum
- `WindowDecorationProperties.ElementRoleProperty` — attached property

---

### 11. Multiple Dispatchers Support (MEDIUM)

**Discovery:** Avalonia 12 supports multiple dispatchers (one per thread).
**Impact:** Library/control authors should use `AvaloniaObject.Dispatcher` or `Dispatcher.CurrentDispatcher`.
**Previous:** Single global `Dispatcher.UIThread`.

---

### 12. Animations Stopped on Invisible Controls (LOW)

**Discovery:** Animations no longer tick when control is hidden.
**Fix:** Set `Animation.PlaybackBehavior = PlaybackBehavior.Always` to restore old behavior.

---

### 13. Windows BinaryFormatter Removed (MEDIUM)

**Discovery:** No longer uses BinaryFormatter for clipboard serialization.
**Impact:** Custom objects on clipboard need explicit serialization (e.g., JSON).

---

### 14. Comprehensive Renamed Members (MEDIUM)

**Discovery:** Many more renames than previously documented:
- `TextBox.Watermark` → `TextBox.PlaceholderText`
- `TextBox.UseFloatingWatermark` → `TextBox.UseFloatingPlaceholder`
- `Window.SystemDecorations` → `Window.WindowDecorations`
- `RenderOptions.TextRenderingMode` → `TextOptions.TextRenderingMode`
- `TextBlock.LetterSpacing` → `TextElement.LetterSpacing` (now inherited attached property)
- `PseudolassesExtensions` → `PseudoClassesExtensions` (typo fix)
- `X11PlatformOptions.ExterinalGLibMainLoopExceptionLogger` → `ExternalGLibMainLoopExceptionLogger`

---

### 15. Comprehensive Obsolete Member Removals (HIGH)

**Discovery:** 40+ members deprecated in Avalonia 11 now removed. Key items:
- `IStyleable` → `StyledElement`
- `CubicBezierEasing` → `SplineEasing`
- `CustomAnimatorBase` → `InterpolatingAnimator<T>`
- `FileDialog`/`OpenFileDialog`/`SaveFileDialog` → `IStorageProvider`
- `SystemDialog` → `IStorageProvider`
- `ToggleButton.Checked/Unchecked/Indeterminate` → `IsCheckedChanged`
- `DrawingContext.PushPreTransform/PushPostTransform` → `PushTransform`
- `IActivatableApplicationLifetime` → `TryGetFeature<IActivatableLifetime>`
- `AppBuilder.LifetimeOverride` → removed
- `IApplicationPlatformEvents` → removed

---

### 16. Android CreateAppBuilder/CustomizeAppBuilder Removed (MEDIUM)

**Discovery:** Virtual methods removed from `AvaloniaMainActivity`.
**Fix:** Move logic to `AvaloniaAndroidApplication<TApp>` subclass.

---

### 17. Render Target Interfaces Reworked (CRITICAL for custom backends)

**Discovery:** Major rework of rendering interfaces:
- `IRenderTarget.CreateDrawingContext` signature changed
- `IRenderTargetBitmapImpl` no longer extends `IRenderTarget`
- `ISkiaGpu` now internal
- Versioned interfaces merged
- `LockedFramebuffer` constructor requires `AlphaFormat`

---

### 18. FuncMultiValueConverter (LOW)

**Discovery:** New `IReadOnlyList<TIn?>` constructor added. Old `IEnumerable<TIn?>` kept for compatibility.

---

### 19. Popup New Properties (MEDIUM)

**Discovery:** New properties on Popup:
- `OverlayDismissEventPassThrough`
- `OverlayInputPassThroughElement`
- `ShouldUseOverlayLayer`
- `IsUsingOverlayLayer`

---

### 20. VisualLayerManager Changes (MEDIUM)

**Discovery:** Layer access methods changed:
- `VisualLayerManager.AdornerLayer` → `AdornerLayer.GetAdornerLayer()`
- `VisualLayerManager.OverlayLayer` → `OverlayLayer.GetOverlayLayer()`
- `VisualLayerManager.ChromeOverlayLayer` → removed (use `WindowDrawnDecorations`)
- `VisualLayerManager.LightDismissOverlayLayer` → removed

---

## Summary

**Corrections:** 5 items (PlacementMode, VisualRoot, MathUtilities.Clamp, IInputRoot, BindingPlugins)
**New discoveries:** 15 items
**Total skill categories:** Updated from 36 to 51
