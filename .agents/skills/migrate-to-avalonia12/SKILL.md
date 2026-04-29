---
name: migrate-to-avalonia12
description: Comprehensive Avalonia 12 migration tool for AtomUI. Detects and fixes all breaking changes from Avalonia 11 to 12, covering 36+ categories including focus events, TopLevel API, clipboard changes, binding system updates, PlacementMode removal, IInputRoot removal, internal API extraction strategy, ReflectionExtensions for internal members, extension methods, and platform-specific changes. Use when migrating projects to Avalonia 12 or checking compatibility.
---

# Avalonia 12 Migration Skill

## Goals

**1. Detect all breaking changes** — Scan code for Avalonia 11 API usage incompatible with Avalonia 12, covering 36+ categories of changes.

**2. Provide comprehensive guidance** — Explain what changed, why, and how to fix it with clear examples.

**3. Support multi-platform migration** — Handle desktop, Android, iOS, browser, and headless platform changes.

**4. Prioritize by impact** — Categorize issues by severity and auto-fixability.

**5. Ensure AOT compatibility** — Identify and fix reflection-based API access with proper [DynamicDependency] patterns.

## Core Principle

> Avalonia 12 is a major version with significant breaking changes across binding system, focus handling, clipboard API, window decorations, TopLevel architecture, Popup positioning, extension methods, and platform support. This skill automates detection and fixing of the most common issues while providing guidance for complex migrations and AOT-safe reflection patterns.

## Trigger Conditions

Use this skill when the user:

- Asks to migrate to Avalonia 12
- Mentions Avalonia 12 breaking changes or compatibility
- Wants to check if code works with Avalonia 12
- Provides code that needs Avalonia 12 migration
- Asks about specific Avalonia 12 changes

## Fundamental Rules

### 1. Comprehensive scanning

Check all 36+ categories of breaking changes, not just the most common ones. Pay special attention to:
- Custom extension methods wrapping removed APIs
- Helper utilities using removed interfaces
- Internal utility classes that may need updating
- Reflection-based access to private/internal members

### 2. Platform-aware migration

Consider target platforms (Desktop, Android, iOS, Browser, Headless) when suggesting fixes.

### 3. Provide context

Explain the rationale behind each breaking change from Avalonia's perspective.

### 4. Suggest alternatives

For removed APIs, always provide the recommended replacement.

### 5. Default to dry-run

Analyze and report without modifying files unless explicitly requested.

### 6. Check extension methods

Scan for custom extension methods that use removed APIs like:
- `GetVisualRoot()` using `GetPresentationSource()`
- `GetRootElement()` on IInputRoot
- Any method returning IInputRoot, IRenderRoot, or ILayoutRoot

### 7. AOT-safe reflection patterns

When accessing private/internal Avalonia APIs:
- Use `[DynamicDependency]` attributes to mark members for AOT preservation
- Use `Lazy<T>` to cache reflection info
- Use `GetXxxInfoOrThrow()` for safe reflection
- Follow ReflectionExtensions naming convention
- Document in reflection-extensions-pattern.md

## Execution Steps

### Step 1: Determine scope and platform

- Single file or entire project?
- Target platform(s): Desktop, Android, iOS, Browser, Headless?
- Current .NET version?
- Existing Avalonia version?
- AOT compilation planned?

### Step 2: Scan for all breaking changes

Check all 36+ categories:

**Core Framework (11 categories)**
1. .NET version requirements (8+ required, 10 recommended)
2. Avalonia package versions (all must be v12)
3. Binding class hierarchy (IBinding → BindingBase)
4. Compiled bindings (now enabled by default)
5. Binding plugins (removed, data validation disabled by default)
6. Text shaper configuration (must call UseHarfBuzz)
7. Touch/pen selection behavior (triggers on release, not press)
8. TopLevel API changes (VisualRoot removed, IInputRoot/IRenderRoot/ILayoutRoot removed)
9. Window decoration changes (TitleBar/CaptionButtons removed)
10. Focus event improvements (unified FocusChangedEventArgs)
11. Extension methods & helper utilities (GetVisualRoot, GetRootElement, etc.)

**Data & Clipboard (3 categories)**
11. Clipboard API (IDataObject → IAsyncDataTransfer)
12. Drag-drop API (DoDragDrop → DoDragDropAsync)
13. DataFormats (DataFormats.* → DataFormat.*)

**Text & Rendering (4 categories)**
14. Text formatting constructors (parameter order changed)
15. Access keys (now triggered by symbol, not virtual key)
16. Font support (Type 1 fonts no longer supported)
17. Direct2D1 removed (use Skia instead)

**Platform-Specific (8 categories)**
18. Android app initialization (AvaloniaMainActivity changes)
19. Android lifetime (IActivityApplicationLifetime replaces ISingleViewApplicationLifetime)
20. iOS scene-based lifecycle (AvaloniaAppDelegate.Window stays null)
21. Browser Blazor package (removed, use Avalonia.Browser)
22. Tizen support (removed)
23. Diagnostics package (Avalonia.Diagnostics → AvaloniaUI.DiagnosticsSupport)
24. xUnit.net v3 (updated from v2)
25. NUnit v4 (updated from v3)

**API Changes (11 categories)**
26. Screen class (now abstract)
27. ResourcesChangedEventArgs (now struct)
28. Gesture events (moved from Gestures class to InputElement)
29. Window.WindowState (now direct property, not styled)
30. Data validation (enabled by default in custom controls)
31. Extension methods & helper utilities (GetVisualRoot, GetRootElement, etc.)
32. ReflectionExtensions pattern for AOT (DynamicDependency, Lazy<T> caching)
33. PlacementMode removal (use PopupAnchor and PopupGravity instead)
34. IInputRoot interface removal (use TopLevel instead)
35. Internal API extraction strategy (extract vs reflect for internal APIs)
36. ReflectionExtensions for internal members (wrap internal/private access)

### Step 3: Categorize by severity and platform

- **CRITICAL** — Compilation errors, must fix
- **HIGH** — Breaking changes, should fix
- **MEDIUM** — Compatibility issues, consider fixing
- **LOW** — Improvements, optional

### Step 4: Generate comprehensive report

Include:
- Total issues by category
- Issues by severity
- Platform-specific issues
- Files affected
- Suggested fixes with code examples
- Auto-fixable vs manual fixes

### Step 5: Apply fixes (if requested)

Only auto-fix safe transformations. Flag complex changes for manual review.

## Migration Rules (36+ Categories)

### Core Framework Changes

#### 1. .NET Version Requirements (CRITICAL)

**What changed:** Avalonia 12 requires .NET 8+. Android/iOS require .NET 10.

**Detection:**
```xml
<TargetFramework>netstandard2.0</TargetFramework>
<TargetFramework>net7.0</TargetFramework>
```

**Fix:**
```xml
<TargetFramework>net10.0</TargetFramework>
```

#### 2. Avalonia Package Versions (CRITICAL)

**What changed:** All Avalonia packages must be v12.

**Detection:**
```xml
<PackageReference Include="Avalonia" Version="11.3.12" />
```

**Fix:**
```xml
<PackageReference Include="Avalonia" Version="12.0.0" />
```

#### 3. Binding Class Hierarchy (HIGH)

**What changed:** `IBinding` removed, use `BindingBase`. `InstancedBinding` removed, use `BindingExpressionBase`.

**Detection:**
```csharp
IBinding binding = new Binding("Property");
var instanced = new InstancedBinding(...);
```

**Fix:**
```csharp
BindingBase binding = new ReflectionBinding(nameof(Item.Property));
var instanced = new CompiledBinding(...);
```

#### 4. Compiled Bindings Default (MEDIUM)

**What changed:** `AvaloniaUseCompiledBindingsByDefault` is now `true` by default.

**Impact:** All `{Binding}` in XAML now use compiled bindings.

**Action:** Verify compiled bindings work with your data context.

#### 5. Binding Plugins Removed (MEDIUM)

**What changed:** Binding plugins removed. Data validation plugin disabled by default.

**Detection:**
```csharp
BindingPlugins.DataValidators.Add(new ExceptionValidationPlugin());
```

**Fix:** Remove plugin registration. Enable validation per-property if needed.

#### 6. Text Shaper Configuration (HIGH)

**What changed:** Text shaper must be configured independently.

**Detection:**
```csharp
AppBuilder.Configure<App>()
    .UseSkia()
    // Missing UseHarfBuzz()
```

**Fix:**
```csharp
AppBuilder.Configure<App>()
    .UseSkia()
    .UseHarfBuzz()
```

**Also add package:**
```xml
<PackageReference Include="Avalonia.HarfBuzz" Version="12.0.0" />
```

#### 7. Touch/Pen Selection (MEDIUM)

**What changed:** Touch/pen selection triggers on release, not press. Container types handle selection directly.

**Detection:**
```csharp
protected override void UpdateSelection(ItemsControl itemsControl, int index, bool selected)
{
    // Old selection logic
}
```

**Fix:**
```csharp
protected override void UpdateSelectionFromEvent(ItemsControl itemsControl, RoutedEventArgs e)
{
    // New selection logic
}
```

#### 8. TopLevel API Changes (HIGH)

**What changed:** `VisualRoot` removed. Use `TopLevel.GetTopLevel(visual)`. Interfaces `IInputRoot`, `IRenderRoot`, `ILayoutRoot` removed.

**Detection patterns:**
```csharp
// Pattern 1: Direct VisualRoot access
var root = visual.VisualRoot as IRenderRoot;
if (root != null) { }

// Pattern 2: Extension method GetVisualRoot()
if (Presenter?.GetVisualRoot() != null)
if (e.Root == currentTip.GetVisualRoot() as IInputRoot)

// Pattern 3: IInputRoot interface usage
public void Update(IInputRoot root, Visual? candidateToolTipHost)
if (root == currentToolTip?.GetVisualRoot() as IInputRoot)

// Pattern 4: GetRootElement() method
e.Root.GetRootElement() == _tipControl?.GetVisualRoot()
```

**Fix:**
```csharp
// Pattern 1: Use TopLevel.GetTopLevel()
var topLevel = TopLevel.GetTopLevel(visual);
if (topLevel is WindowBase window) { }

// Pattern 2: Replace GetVisualRoot() with TopLevel.GetTopLevel()
if (Presenter != null && TopLevel.GetTopLevel(Presenter) != null)
if (e.Root == TopLevel.GetTopLevel(currentTip))

// Pattern 3: Replace IInputRoot with TopLevel
public void Update(TopLevel root, Visual? candidateToolTipHost)
if (root == TopLevel.GetTopLevel(currentToolTip as Visual))

// Pattern 4: e.Root is now TopLevel directly
e.Root == TopLevel.GetTopLevel(_tipControl)
```

**Why:** The visual tree architecture changed. TopLevel is now the primary way to access the root of the visual tree. IInputRoot, IRenderRoot, and ILayoutRoot interfaces are removed. RawInputEventArgs.Root is now TopLevel instead of IInputRoot.

#### 9. Window Decoration Changes (HIGH)

**What changed:** `TitleBar`, `CaptionButtons`, `ChromeOverlayLayer` removed. Use `WindowDrawnDecorations`.

**Detection:**
```xml
<Chrome:TitleBar />
<Chrome:CaptionButtons />
```

**Fix:**
```xml
<Chrome:WindowDrawnDecorations />
```

#### 10. Focus Event Changes (HIGH)

**What changed:** Both `GotFocus` and `LostFocus` now use `FocusChangedEventArgs`.

**Detection:**
```csharp
protected override void OnGotFocus(GotFocusEventArgs e)
protected override void OnLostFocus(RoutedEventArgs e)
```

**Fix:**
```csharp
protected override void OnGotFocus(FocusChangedEventArgs e)
protected override void OnLostFocus(FocusChangedEventArgs e)
```

### Data & Clipboard Changes

#### 11. Clipboard API (HIGH)

**What changed:** `IDataObject` removed. Use `IAsyncDataTransfer`. Methods moved to extensions.

**Detection:**
```csharp
var data = new DataObject();
data.Set(DataFormats.Text, "text");
await clipboard.SetDataObjectAsync(data);
var text = await clipboard.GetTextAsync();
```

**Fix:**
```csharp
var item = new DataTransferItem();
item.Set(DataFormat.Text, "text");
var data = new DataTransfer();
data.Add(item);
await clipboard.SetDataAsync(data);
var text = await clipboard.TryGetTextAsync();
```

**Add using:**
```csharp
using Avalonia.Input.Platform;
```

#### 12. Drag-Drop API (HIGH)

**What changed:** `DoDragDrop` → `DoDragDropAsync`. `DragEventArgs.Data` → `DragEventArgs.DataTransfer`.

**Detection:**
```csharp
DragDrop.DoDragDrop(dragEventArgs, dataObject);
var data = dragEventArgs.Data;
```

**Fix:**
```csharp
await DragDrop.DoDragDropAsync(dragEventArgs, dataTransfer);
var data = dragEventArgs.DataTransfer;
```

#### 13. DataFormats Changes (MEDIUM)

**What changed:** `DataFormats.*` → `DataFormat.*`

**Detection:**
```csharp
data.Set(DataFormats.Text, "text");
data.Set(DataFormats.Files, files);
```

**Fix:**
```csharp
data.Set(DataFormat.Text, "text");
data.Set(DataFormat.Files, files);
```

### Text & Rendering Changes

#### 14. Text Formatting Constructors (HIGH)

**What changed:** `GenericTextRunProperties`, `TextCollapsingProperties`, `TextShaperOptions` merged constructors. `FontFeatureCollection` now last parameter.

**Detection:**
```csharp
new GenericTextRunProperties(typeface, features, size, decorations, brush)
```

**Fix:**
```csharp
new GenericTextRunProperties(typeface, size, decorations, brush, fontFeatures: features)
```

#### 15. Access Keys (MEDIUM)

**What changed:** Access keys triggered by symbol, not virtual key. `AccessText.AccessKey` is now `string?` not `char`.

**Detection:**
```csharp
public char AccessKey { get; set; }
```

**Fix:**
```csharp
public string? AccessKey { get; set; }
```

#### 16. Font Support (LOW)

**What changed:** Type 1 fonts (.pfb/.pfm) no longer supported.

**Action:** Use TrueType (.ttf) or OpenType (.otf) fonts instead.

#### 17. Direct2D1 Removed (HIGH - Windows)

**What changed:** Direct2D1 backend removed. Use Skia.

**Detection:**
```xml
<PackageReference Include="Avalonia.Direct2D1" Version="11.3.12" />
```

**Fix:**
```xml
<PackageReference Include="Avalonia.Skia" Version="12.0.0" />
```

**Code:**
```csharp
AppBuilder.Configure<App>()
    .UseSkia()
```

### Platform-Specific Changes

#### 18. Android App Initialization (CRITICAL - Android)

**What changed:** `AvaloniaMainActivity<TApp>` → `AvaloniaMainActivity` + `AvaloniaAndroidApplication<TApp>`.

**Detection:**
```csharp
[Activity]
public class MainActivity : AvaloniaMainActivity<App>
{
}
```

**Fix:**
```csharp
[Activity]
public class MainActivity : AvaloniaMainActivity
{
}

[Application]
public class AndroidApp : AvaloniaAndroidApplication<App>
{
    protected AndroidApp(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }
}
```

#### 19. Android Lifetime (HIGH - Android)

**What changed:** `ISingleViewApplicationLifetime` → `IActivityApplicationLifetime` with `MainViewFactory`.

**Detection:**
```csharp
if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
    singleView.MainView = new MainView();
```

**Fix:**
```csharp
if (ApplicationLifetime is IActivityApplicationLifetime activityLifetime)
    activityLifetime.MainViewFactory = () => new MainView();
else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
    singleView.MainView = new MainView();
```

#### 20. iOS Scene-Based Lifecycle (MEDIUM - iOS)

**What changed:** iOS now uses scene-based lifecycle. `AvaloniaAppDelegate.Window` stays null.

**Action:** Override `AvaloniaView.MovedToWindow` to detect window attachment.

#### 21. Browser Blazor Removed (HIGH - Browser)

**What changed:** `Avalonia.Browser.Blazor` package removed. Use `Avalonia.Browser`.

**Detection:**
```xml
<PackageReference Include="Avalonia.Browser.Blazor" Version="11.3.12" />
```

**Fix:**
```xml
<PackageReference Include="Avalonia.Browser" Version="12.0.0" />
```

#### 22. Tizen Removed (CRITICAL - Tizen)

**What changed:** Tizen platform no longer supported.

**Action:** Migrate to supported platform or maintain custom fork.

#### 23. Diagnostics Package (MEDIUM)

**What changed:** `Avalonia.Diagnostics` → `AvaloniaUI.DiagnosticsSupport`. `AttachDevTools()` → `AttachDeveloperTools()`.

**Detection:**
```csharp
AttachDevTools();
```

**Fix:**
```csharp
AttachDeveloperTools();
```

**Package:**
```xml
<PackageReference Include="AvaloniaUI.DiagnosticsSupport" Version="2.2.0" />
```

#### 24-25. Test Framework Updates (MEDIUM - Headless)

**What changed:** xUnit.net v3 (from v2), NUnit v4 (from v3).

**Action:** Update test projects and follow official migration guides.

### API Changes

#### 26. Screen Class (LOW)

**What changed:** `Screen` is now abstract. Don't construct it.

**Detection:**
```csharp
var screen = new Screen();
```

**Fix:**
```csharp
var screen = Screens.Primary;
var screens = Screens.All;
```

#### 27. ResourcesChangedEventArgs (LOW)

**What changed:** Now a struct. Use `ResourcesChangedEventArgs.Create()` to construct.

**Detection:**
```csharp
var args = new ResourcesChangedEventArgs();
```

**Fix:**
```csharp
var args = ResourcesChangedEventArgs.Create();
```

#### 28. Gesture Events (MEDIUM)

**What changed:** Moved from `Gestures` class to `InputElement`. Remove `Gestures.` prefix in XAML.

**Detection:**
```xml
<Button Gestures.Pinch="Button_Pinch" />
```

**Fix:**
```xml
<Button Pinch="Button_Pinch" />
```

#### 29. Window.WindowState (MEDIUM)

**What changed:** Now a direct property, not styled property. Can't set from style.

**Action:** Set `WindowState` in code-behind or binding, not in styles.

#### 30. Data Validation Default (LOW)

**What changed:** Data validation enabled by default for properties with `enableDataValidation: true`.

**Action:** Remove `UpdateDataValidation` overrides that only call `DataValidationErrors.SetError`.

#### 31. Extension Methods & Helper Utilities (HIGH)

**What changed:** Custom extension methods using removed APIs need updating. Common patterns:
- `GetVisualRoot()` extension using `GetPresentationSource()` 
- `GetRootElement()` method on IInputRoot
- Custom utilities wrapping removed interfaces

**Detection:**
```csharp
// In VisualExtensions.cs or similar utility files
internal static Visual? GetVisualRoot(this Visual visual)
{
    return visual.GetPresentationSource()?.RootVisual;
}

// Usage in service classes
if (Presenter?.GetVisualRoot() != null)
if (e.Root == currentTip.GetVisualRoot() as IInputRoot)
```

**Fix:**
```csharp
// Option 1: Update extension to use TopLevel
internal static TopLevel? GetVisualRootTopLevel(this Visual visual)
{
    return TopLevel.GetTopLevel(visual);
}

// Option 2: Replace all usages directly with TopLevel.GetTopLevel()
if (Presenter != null && TopLevel.GetTopLevel(Presenter) != null)
if (e.Root == TopLevel.GetTopLevel(currentTip))
```

**Why:** GetPresentationSource() is internal/protected in Avalonia 12. TopLevel.GetTopLevel() is the public API for accessing the root. Update all extension methods to use the new API.

#### 32. ReflectionExtensions Pattern for AOT (CRITICAL for AOT)

**What changed:** When accessing private/internal Avalonia APIs, use standardized ReflectionExtensions pattern with `[DynamicDependency]` attributes for AOT safety.

**Pattern:**
```csharp
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;

internal static class TargetClassReflectionExtensions
{
    #region 反射信息定义
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(TargetClass))]
    private static readonly Lazy<PropertyInfo> PropertyNamePropertyInfo = new Lazy<PropertyInfo>(() => 
        typeof(TargetClass).GetPropertyInfoOrThrow("PropertyName",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    #endregion
    
    public static PropertyType GetPropertyName(this TargetClass target)
    {
        var value = PropertyNamePropertyInfo.Value.GetValue(target) as PropertyType;
        Debug.Assert(value != null);
        return value;
    }
}
```

**Key Points:**
- Use `[DynamicDependency]` to mark members for AOT preservation
- Use `Lazy<T>` to cache reflection info
- Use `GetXxxInfoOrThrow()` for safe reflection
- Use `Debug.Assert()` for null checks
- Naming: `{MemberName}{MemberType}Info`

**Why:** Prevents AOT trimming of private/internal members that are accessed via reflection. Essential for shipping AOT-compiled applications.

#### 33. PlacementMode Removal (HIGH)

**What changed:** `PlacementMode` enum completely removed. Use `PopupAnchor` and `PopupGravity` combination instead.

**Detection:**
```csharp
PlacementMode placement;
if (placement != PlacementMode.AnchorAndGravity)
if (placement == PlacementMode.Center)
Popup.PlacementMode = PlacementMode.Bottom;
```

**Fix:**
```csharp
// Use PopupAnchor and PopupGravity
PopupAnchor anchor;
PopupGravity gravity;
if (anchor != PopupAnchor.None)
if (anchor == PopupAnchor.Center)
// Use Popup.Anchor and Popup.Gravity properties
```

**Why:** Avalonia 12 simplified Popup positioning API with more explicit anchor/gravity model.

#### 34. IInputRoot Interface Removal (HIGH)

**What changed:** `IInputRoot` interface removed. Functionality distributed to `IPresentationSource` and `TopLevel`.

**Detection:**
```csharp
typeof(IInputRoot).GetProperty("RootElement", ...)
public void Process(IInputRoot root)
if (root is IInputRoot inputRoot)
```

**Fix:**
```csharp
// Use TopLevel instead
typeof(TopLevel).GetProperty("RootVisual", ...)
public void Process(TopLevel topLevel)
if (topLevel is TopLevel)
```

**Why:** IInputRoot responsibilities redistributed to more specialized interfaces.

#### 35. Internal API Extraction Strategy (MEDIUM)

**What changed:** Some Avalonia 12 classes, structs, or interfaces are `internal` but required by AtomUI.

**Options:**

**Option 1: Use ReflectionExtensions (Recommended for small APIs)**
- Wrap internal member access with `[DynamicDependency]` attributes
- Use `Lazy<T>` caching for reflection info
- Follow ReflectionExtensions naming pattern
- See Category 32 for detailed pattern

**Option 2: Extract Code (Recommended for complex APIs)**
- Copy entire internal class/struct/interface to AtomUI
- Place in most appropriate project:
  - `AtomUI.Core` — Core utilities, base classes
  - `AtomUI.Controls` — Platform-agnostic controls
  - `AtomUI.Desktop.Controls` — Desktop-specific implementations
- Update namespace to `AtomUI.XXX`
- Add XML documentation
- Mark as `internal` to avoid public API pollution

**Example - Extracting Internal Struct:**

```csharp
// From Avalonia (internal)
namespace Avalonia.Controls.Primitives.PopupPositioning
{
    internal struct PopupPositioningData
    {
        public Point Offset { get; set; }
        public Size Size { get; set; }
    }
}

// Extract to AtomUI.Controls
namespace AtomUI.Controls.Primitives
{
    /// <summary>
    /// Extracted from Avalonia 12 internal API for popup positioning.
    /// </summary>
    internal struct PopupPositioningData
    {
        public Point Offset { get; set; }
        public Size { get; set; }
    }
}
```

**When to Extract:**
- Internal API is complex (multiple methods, properties)
- Internal API is used in multiple places
- Reflection overhead is significant
- AOT compilation is planned
- API is stable and unlikely to change

**When to Use Reflection:**
- Internal API is simple (single property/method)
- Used in only one place
- Performance is not critical
- AOT not planned

**Why:** Extraction avoids reflection overhead, improves AOT compatibility, and makes code more maintainable than reflection-based access.

#### 36. ReflectionExtensions for Internal Members (HIGH)

**What changed:** Public classes/structs/interfaces in Avalonia 12 may have `internal` or `private` members that AtomUI needs to access.

**Strategy:** Use ReflectionExtensions pattern to safely wrap internal member access.

**When to Use:**
- Target class/struct/interface is `public`
- Need to access `internal` or `private` members
- Member is simple (single property/field/method)
- Want AOT-safe, maintainable code

**Pattern:**

```csharp
// File: {TargetClass}ReflectionExtensions.cs
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.XXX;

namespace AtomUI.XXX;

/// <summary>
/// Reflection wrapper for accessing internal members of Avalonia's {TargetClass}.
/// </summary>
internal static class TargetClassReflectionExtensions
{
    #region 反射信息定义

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(TargetClass))]
    private static readonly Lazy<PropertyInfo> InternalPropertyPropertyInfo = new(() =>
        typeof(TargetClass).GetPropertyInfoOrThrow("InternalProperty",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(TargetClass))]
    private static readonly Lazy<FieldInfo> _internalFieldFieldInfo = new(() =>
        typeof(TargetClass).GetFieldInfoOrThrow("_internalField",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(TargetClass))]
    private static readonly Lazy<MethodInfo> InternalMethodMethodInfo = new(() =>
        typeof(TargetClass).GetMethodInfoOrThrow("InternalMethod",
            BindingFlags.Instance | BindingFlags.NonPublic));

    #endregion

    /// <summary>
    /// Gets the internal property value.
    /// </summary>
    public static PropertyType GetInternalProperty(this TargetClass target)
    {
        var value = InternalPropertyPropertyInfo.Value.GetValue(target) as PropertyType;
        Debug.Assert(value != null);
        return value;
    }

    /// <summary>
    /// Sets the internal property value.
    /// </summary>
    public static void SetInternalProperty(this TargetClass target, PropertyType value)
    {
        InternalPropertyPropertyInfo.Value.SetValue(target, value);
    }

    /// <summary>
    /// Gets the internal field value.
    /// </summary>
    public static FieldType GetInternalField(this TargetClass target)
    {
        var value = _internalFieldFieldInfo.Value.GetValue(target) as FieldType;
        Debug.Assert(value != null);
        return value;
    }

    /// <summary>
    /// Invokes the internal method.
    /// </summary>
    public static ReturnType InvokeInternalMethod(this TargetClass target, ParameterType param)
    {
        var result = InternalMethodMethodInfo.Value.Invoke(target, [param]);
        Debug.Assert(result != null);
        return (ReturnType)result;
    }
}
```

**Usage:**

```csharp
// Before: Direct reflection (unsafe, not AOT-friendly)
var prop = typeof(TargetClass).GetProperty("InternalProperty", 
    BindingFlags.Instance | BindingFlags.NonPublic);
var value = prop?.GetValue(target);

// After: Using ReflectionExtensions (safe, AOT-friendly)
var value = target.GetInternalProperty();
```

**Key Points:**
- File naming: `{TargetClass}ReflectionExtensions.cs`
- Class: `internal static class {TargetClass}ReflectionExtensions`
- Each member wrapped in extension method
- `[DynamicDependency]` attribute on each reflection info field
- `Lazy<T>` for caching reflection info
- `Debug.Assert()` for null checks
- XML documentation for each method
- Naming: `{MemberName}{MemberType}Info` for reflection fields

**Why:** 
- AOT-safe: `[DynamicDependency]` prevents trimming
- Maintainable: Centralized reflection access
- Performant: `Lazy<T>` caches reflection info
- Discoverable: Easy to find all internal member access
- Testable: Can mock extension methods in tests

**Example from AtomUI:**

```csharp
// TextParagraphPropertiesReflectionExtensions.cs
internal static class TextParagraphPropertiesReflectionExtensions
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, 
        typeof(TextParagraphProperties))]
    private static readonly Lazy<PropertyInfo> LineSpacingPropertyInfo = new(() =>
        typeof(TextParagraphProperties).GetPropertyInfoOrThrow("LineSpacing",
            BindingFlags.Instance | BindingFlags.NonPublic));

    public static double GetLineSpacing(this TextParagraphProperties properties)
    {
        var lineSpacing = LineSpacingPropertyInfo.Value.GetValue(properties) as double?;
        Debug.Assert(lineSpacing != null);
        return lineSpacing.Value;
    }
}

// Usage
var spacing = textParagraphProperties.GetLineSpacing();
```

## Report Format

### Markdown Report (Default)

```markdown
# Avalonia 12 Migration Report

**Total Issues Found:** 45
**Critical Issues:** 8
**High Severity:** 15
**Medium Severity:** 18
**Low Severity:** 4
**Auto-fixable:** 12

## Platform Analysis
- Desktop: 35 issues
- Android: 5 issues
- iOS: 2 issues
- Browser: 1 issue
- Headless: 2 issues

## CRITICAL Issues (Must Fix)

### .NET Version
**File:** AtomUIV6.csproj
**Current:** net8.0
**Suggested:** net10.0
**Auto-fixable:** Yes

...
```

## Execution Rules

### Before generating fixes

1. Scan all 30+ categories
2. Identify platform-specific issues
3. Categorize by severity
4. Generate comprehensive report

### When auto-fixing

1. Only fix marked auto-fixable issues
2. Preserve formatting and comments
3. Update imports if necessary
4. Verify no new issues introduced

### Code generation style

**Always use braces `{}` for code blocks, even single-line statements:**

```csharp
// ❌ Wrong - no braces
if (condition)
    DoSomething();

for (int i = 0; i < count; i++)
    Process(i);

// ✅ Correct - always use braces
if (condition)
{
    DoSomething();
}

for (int i = 0; i < count; i++)
{
    Process(i);
}
```

**Why:** Prevents bugs from accidental statement misalignment, improves readability, and maintains consistency with C# style guidelines.

### When reporting

1. Show current code
2. Show suggested fix
3. Explain why changed
4. Indicate auto-fixability
5. Group by severity and platform

## Prohibited

- Fixing without user consent
- Modifying code without showing diff
- Ignoring any of 34+ categories
- Incomplete reports
- Suggesting non-Avalonia 12 API
- Modifying files outside scope
- Unnecessary formatting changes
- Missing extension method and helper utility checks
- Reflection-based API access without [DynamicDependency] attributes
- Using reflection for complex internal APIs when extraction is more appropriate
- Extracting internal APIs without proper documentation or namespace organization
- Accessing internal/private members without ReflectionExtensions wrapper
- Reflection info not cached in Lazy<T>
- AOT-unsafe reflection patterns
- Missing PlacementMode and IInputRoot migration checks

## References

For detailed information, see `references/` directory:

- `missed-breaking-changes.md` — Recently discovered breaking changes (PlacementMode, IInputRoot)
- `code-level-analysis.md` — Code-level analysis from official Avalonia source (11.3.14 → 12.0.0)
- `reflection-extensions-pattern.md` — AOT-safe ReflectionExtensions pattern and best practices
- `avalonia12-breaking-changes.md` — All 34+ breaking changes with severity classification
- `migration-examples.md` — Before/after code examples for each category
- `atomui-migration-guide.md` — AtomUI-specific patterns and migration workflow
