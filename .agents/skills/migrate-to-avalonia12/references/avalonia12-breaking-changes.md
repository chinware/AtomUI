# Avalonia 12 Breaking Changes Reference

## Complete List of Breaking Changes

### 1. Focus Events

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| GotFocus event args | `GotFocusEventArgs` | `FocusChangedEventArgs` |
| LostFocus event args | `RoutedEventArgs` | `FocusChangedEventArgs` |
| Unified system | No | Yes |

### 2. TopLevel API

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| Access top-level | `visual.VisualRoot` | `TopLevel.GetTopLevel(visual)` |
| Type | `IRenderRoot` | `TopLevel` |
| Visibility | Public property | Static method |
| Removed interfaces | N/A | `IInputRoot`, `IRenderRoot`, `ILayoutRoot` |

### 3. Popup Properties

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| Check if open | `popup.Host != null` | `popup.IsOpen` |
| Close popup | `popup.Host?.Close()` | `popup.IsOpen = false` |
| Property type | `PopupRoot` | `bool` |

### 4. Selection API

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| Update selection | `UpdateSelection(item, selected)` | `SetCurrentValue(IsSelectedProperty, selected)` |
| Method type | Instance method | Property setter |
| Deprecation | No | Yes |

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
| Get data | `GetTextAsync()` | `TryGetTextAsync()` |
| Data format | `IDataObject` | `IAsyncDataTransfer` |

### 7. Utilities

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| Clamp function | `MathUtilities.Clamp()` | `Math.Clamp()` |
| Source | Custom utility | .NET standard library |
| Availability | Avalonia-specific | .NET 6+ built-in |

### 8. Extension Methods & Helper Utilities

| Aspect | Avalonia 11 | Avalonia 12 |
|--------|------------|-----------|
| GetVisualRoot() | Uses `GetPresentationSource()` | Use `TopLevel.GetTopLevel()` |
| GetRootElement() | Available on IInputRoot | Removed, use TopLevel |
| Return type | `IInputRoot`, `IRenderRoot` | `TopLevel` |

## Severity Classification

### CRITICAL (Must Fix)
- Focus Events - Compilation error
- TopLevel API - Compilation error
- Popup.Host - Compilation error
- GenericTextRunProperties - Compilation error
- Extension methods using removed APIs - Compilation error

### HIGH (Should Fix)
- TopLevel interface removal (IInputRoot, IRenderRoot, ILayoutRoot)
- UpdateSelection - Obsolete warning
- Clipboard API - API change
- Helper utilities wrapping removed interfaces

### MEDIUM (Consider Fixing)
- MathUtilities.Clamp - Redundant code
- Custom extension methods - Code quality

### LOW (Nice to Fix)
- Gesture events - Minor API change

## Auto-fixable Issues

✅ **Auto-fixable:**
- Focus Events
- Popup.Host
- MathUtilities.Clamp
- Simple GetVisualRoot() → TopLevel.GetTopLevel() replacements

❌ **Manual intervention required:**
- TopLevel API (context-dependent)
- UpdateSelection (requires understanding intent)
- GenericTextRunProperties (parameter order varies)
- Clipboard API (data format changes)
- Extension methods (may need refactoring)

## Migration Checklist

- [ ] Scan entire project for breaking changes
- [ ] Fix CRITICAL severity issues first
- [ ] Review and fix HIGH severity issues
- [ ] Check for custom extension methods using removed APIs
- [ ] Update helper utilities and internal utilities
- [ ] Consider fixing MEDIUM severity issues
- [ ] Run unit tests
- [ ] Verify compilation
- [ ] Test functionality
- [ ] Update documentation if needed
