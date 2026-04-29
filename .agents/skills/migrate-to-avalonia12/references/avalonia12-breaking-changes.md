# Avalonia 12 Breaking Changes Reference

## Complete List of Breaking Changes

### 1. Focus Events

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| GotFocus event args | `GotFocusEventArgs` | `FocusChangedEventArgs` |
| LostFocus event args | `RoutedEventArgs` | `FocusChangedEventArgs` |
| Unified system | No | Yes |
| New properties | N/A | `NewFocusedElement`, `OldFocusedElement` |
| KeyboardNavigationHandler | Public | `internal` — use `FocusManager.GetNextElement` |

### 2. TopLevel API

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| Access top-level | `visual.VisualRoot` (public) | `TopLevel.GetTopLevel(visual)` |
| VisualRoot | Public property | `protected internal` (still exists) |
| IInputRoot | Public interface | `[PrivateApi]` (still exists internally) |
| IRenderRoot | Public interface | Removed entirely |
| ILayoutRoot | Public interface | `internal` |
| New abstraction | N/A | `IPresentationSource` |

### 3. Popup Properties

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| Check if open | `popup.Host != null` | `popup.IsOpen` |
| Close popup | `popup.Host?.Close()` | `popup.IsOpen = false` |
| IPopupHost | Public interface | `internal` |
| PlacementMode property | `popup.PlacementMode` | `popup.Placement` |
| PlacementMode enum | Exists | Still exists (NOT removed) |
| New properties | N/A | `OverlayDismissEventPassThrough`, `ShouldUseOverlayLayer` |

### 4. Selection API

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| Update selection | `UpdateSelection(item, selected)` | `SetCurrentValue(IsSelectedProperty, selected)` |
| Method type | Instance method | Property setter |
| New helper | N/A | `ItemSelectionEventTriggers` |

### 5. Text Formatting

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| Constructor params | `(typeface, features, size, decorations, brush)` | `(typeface, size, decorations, brush, features)` |
| Parameter order | Different | Reorganized |
| Named parameters | Optional | Recommended |

### 6. Clipboard API

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| Set data | `SetDataObjectAsync(IDataObject)` | `SetDataAsync(IAsyncDataTransfer)` |
| Get data | `GetTextAsync()` | `TryGetTextAsync()` (extension) |
| Data format | `IDataObject` | `IAsyncDataTransfer` |
| DataFormats | `DataFormats.*` | `DataFormat.*` |
| BinaryFormatter | Used for clipboard | Removed — explicit serialization needed |

### 7. Utilities

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| Clamp function | `MathUtilities.Clamp()` | Still exists, but `Math.Clamp()` preferred |
| Source | Custom utility | Both available |

### 8. Extension Methods & Helper Utilities

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| GetVisualRoot() | Uses `GetPresentationSource()` | Use `TopLevel.GetTopLevel()` |
| GetRootElement() | Available on IInputRoot | Use TopLevel |
| Return type | `IInputRoot`, `IRenderRoot` | `TopLevel` |

### 9. Window Decorations

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| TitleBar | `Chrome.TitleBar` | Removed → `WindowDrawnDecorations` |
| CaptionButtons | `Chrome.CaptionButtons` | Removed → `WindowDrawnDecorations` |
| ChromeOverlayLayer | Public class | Removed |
| LightDismissOverlayLayer | Public class | Removed |
| SystemDecorations enum | Exists | Removed |
| ExtendClientAreaChromeHints | Exists | Removed |
| IPopupHostProvider | Public | Removed |
| Window.SystemDecorations | Property | Renamed to `Window.WindowDecorations` |

### 10. Binding System

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| IBinding | Public interface | Removed |
| InstancedBinding | Public class | Removed |
| Binding class | Primary binding | Compatibility wrapper for `ReflectionBinding` |
| BindingBase | N/A | New abstract base class |
| CompiledBinding | N/A | New class with `Create<TIn,TOut>()` |
| BindingPlugins | Public class | `internal` |
| Compiled by default | `false` | `true` |

### 11. Gestures

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| Gestures class | Public static | `internal` |
| Event location | `Gestures.TappedEvent` | `InputElement.TappedEvent` |
| XAML prefix | `Gestures.Pinch` | `Pinch` (direct) |

### 12. Dispatcher

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| Dispatcher model | Single global | Multiple (one per thread) |
| InvokeAsync context | Not captured | Captures execution context |
| Recommended | `Dispatcher.UIThread` | `AvaloniaObject.Dispatcher` or `Dispatcher.CurrentDispatcher` |

### 13. Renamed Members

| Old Name | New Name |
|----------|----------|
| `Popup.PlacementMode` | `Popup.Placement` |
| `ContextMenu.PlacementMode` | `ContextMenu.Placement` |
| `TextBox.Watermark` | `TextBox.PlaceholderText` |
| `TextBox.UseFloatingWatermark` | `TextBox.UseFloatingPlaceholder` |
| `Window.SystemDecorations` | `Window.WindowDecorations` |
| `RenderOptions.TextRenderingMode` | `TextOptions.TextRenderingMode` |
| `TextBlock.LetterSpacing` | `TextElement.LetterSpacing` |
| `Color.ToUint32()` | `Color.ToUInt32()` |
| `Screen.PixelDensity` | `Screen.Scaling` |
| `Screen.Primary` | `Screen.IsPrimary` |
| `BindingPriority.TemplatedParent` | `BindingPriority.Template` |
| `CubicBezierEasing` | `SplineEasing` |
| `CustomAnimatorBase` | `InterpolatingAnimator<T>` |
| `AttachDevTools()` | `AttachDeveloperTools()` |
| `PseudolassesExtensions` | `PseudoClassesExtensions` |

### 14. Removed Obsolete Members (40+)

| Removed | Replacement |
|---------|-------------|
| `IStyleable` | `StyledElement` |
| `CubicBezierEasing` | `SplineEasing` |
| `CustomAnimatorBase` | `InterpolatingAnimator<T>` |
| `RadialGradientBrush.Radius` | `RadiusX` / `RadiusY` |
| `DrawingContext.PushPreTransform` | `DrawingContext.PushTransform` |
| `DrawingContext.PushPostTransform` | `DrawingContext.PushTransform` |
| `FileDialog` / `OpenFileDialog` | `IStorageProvider` |
| `OpenFolderDialog` / `SaveFileDialog` | `IStorageProvider` |
| `SystemDialog` | `IStorageProvider` |
| `IActivatableApplicationLifetime` | `TryGetFeature<IActivatableLifetime>` |
| `ItemContainerGenerator.ContainerFromIndex` | `ItemsControl` methods |
| `TreeItemContainerGenerator` | `ItemContainerGenerator` |
| `ToggleButton.Checked/Unchecked/Indeterminate` | `IsCheckedChanged` |
| `AppBuilder.LifetimeOverride` | Removed |
| `IApplicationPlatformEvents` | Removed |
| `Screens.ScreenFromWindow` | `Screens.ScreenFromTopLevel` |
| `IInsetsManager.DisplayEdgeToEdge` | `DisplayEdgeToEdgePreference` |

## Severity Classification

### CRITICAL (Must Fix)
- Focus Events - Compilation error
- TopLevel API (VisualRoot now protected internal) - Compilation error
- Popup.Host removed - Compilation error
- GenericTextRunProperties constructor - Compilation error
- Extension methods using removed/internalized APIs - Compilation error
- IInputRoot now [PrivateApi] - Compilation error for external code
- IRenderRoot removed entirely - Compilation error
- Window decoration types removed - Compilation error

### HIGH (Should Fix)
- IPopupHost now internal
- BindingPlugins now internal
- Gestures class now internal
- KeyboardNavigationHandler now internal
- Popup.PlacementMode → Popup.Placement (property rename)
- Clipboard API changes
- Binding system overhaul (IBinding, InstancedBinding removed)
- Obsolete member removals (40+ items)
- Renamed members

### MEDIUM (Consider Fixing)
- MathUtilities.Clamp - Still works but redundant
- Multiple dispatchers - Behavioral change
- Animations on invisible controls - Behavioral change
- FuncMultiValueConverter - Backward compatible
- BinaryFormatter removal - Windows clipboard

### LOW (Nice to Fix)
- Gesture events XAML prefix
- Dispatcher execution context
- ExtendClientAreaToDecorationsHint improved

## Auto-fixable Issues

✅ **Auto-fixable:**
- Focus Events
- Popup.Host → IsOpen
- MathUtilities.Clamp → Math.Clamp
- Simple GetVisualRoot() → TopLevel.GetTopLevel() replacements
- Popup.PlacementMode → Popup.Placement
- DataFormats.* → DataFormat.*
- AttachDevTools → AttachDeveloperTools
- Gestures.* → InputElement.* in code
- Renamed members (simple find-replace)

❌ **Manual intervention required:**
- TopLevel API (context-dependent)
- UpdateSelection (requires understanding intent)
- GenericTextRunProperties (parameter order varies)
- Clipboard API (data format changes)
- Extension methods (may need refactoring)
- Window decoration redesign (architectural)
- Binding system overhaul (complex)
- IPopupHost internalization (architectural)

## Migration Checklist

- [ ] Scan entire project for breaking changes
- [ ] Fix CRITICAL severity issues first
- [ ] Review and fix HIGH severity issues
- [ ] Check for custom extension methods using removed/internalized APIs
- [ ] Update helper utilities and internal utilities
- [ ] Check for renamed members
- [ ] Check for removed obsolete members
- [ ] Check for internalized classes (Gestures, BindingPlugins, IPopupHost, KeyboardNavigationHandler)
- [ ] Consider fixing MEDIUM severity issues
- [ ] Run unit tests
- [ ] Verify compilation
- [ ] Test functionality
- [ ] Update documentation if needed
