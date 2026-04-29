---
name: migrate-to-avalonia12
description: Comprehensive Avalonia 12 migration tool for AtomUI. Detects and fixes all breaking changes from Avalonia 11 to 12, covering 50+ categories including focus events, TopLevel API, clipboard changes, binding system updates, PlacementMode rename, [PrivateApi] public interface handling (IInputRoot still usable), IPopupHost/Gestures internalization, window decoration redesign, dispatcher changes, obsolete member removals, renamed members, internal API extraction strategy, ReflectionExtensions for internal members, extension methods, and platform-specific changes. Use when migrating projects to Avalonia 12 or checking compatibility.
---

# Avalonia 12 Migration Skill

## Goals

**1. Detect all breaking changes** — Scan code for Avalonia 11 API usage incompatible with Avalonia 12, covering 50+ categories of changes.

**2. Provide comprehensive guidance** — Explain what changed, why, and how to fix it with clear examples.

**3. Support multi-platform migration** — Handle desktop, Android, iOS, browser, and headless platform changes.

**4. Prioritize by impact** — Categorize issues by severity and auto-fixability.

**5. Ensure AOT compatibility** — Identify and fix reflection-based API access with proper [DynamicDependency] patterns.

## Core Principle

> Avalonia 12 is a major version with significant breaking changes across binding system, focus handling, clipboard API, window decorations, TopLevel architecture, Popup positioning, extension methods, dispatcher model, obsolete member removals, renamed APIs, and platform support. This skill automates detection and fixing of the most common issues while providing guidance for complex migrations and AOT-safe reflection patterns.

## Trigger Conditions

Use this skill when the user:

- Asks to migrate to Avalonia 12
- Mentions Avalonia 12 breaking changes or compatibility
- Wants to check if code works with Avalonia 12
- Provides code that needs Avalonia 12 migration
- Asks about specific Avalonia 12 changes
- Asks to "upgrade" (升级) any code, file, or module — e.g., "请将 xxx 升级", "upgrade xxx"
- Asks to refactor or rewrite code "for Avalonia 12" / "按照 Avalonia 12" — e.g., "按照 Avalonia 12 进行重构", "refactor xxx for Avalonia 12"
- Asks to "改造" (transform/rework) code — e.g., "请将 xxx 改造为 Avalonia 12 标准"
- Uses keywords like "升级", "upgrade", "迁移", "migrate", "重构", "refactor", "改造", "适配 Avalonia 12", "升级至 Avalonia 12 标准写法" in the context of Avalonia code changes
- Any request involving modifying AtomUI source code to conform to Avalonia 12 standards, regardless of language (Chinese or English)

## Fundamental Rules

### 1. Comprehensive scanning

Check all 50+ categories of breaking changes, not just the most common ones. Pay special attention to:
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

Scan for custom extension methods that use removed or internalized APIs like:
- `GetVisualRoot()` using `GetPresentationSource()`
- `GetRootElement()` on IInputRoot (note: IInputRoot itself is still public and usable)
- Any method returning IRenderRoot or ILayoutRoot (these are truly internal)
- Any code referencing IPopupHost (now internal)
- Any code using Gestures class directly (now internal)
- Any code using BindingPlugins (now internal)
- Any code using KeyboardNavigationHandler (now internal)

### 7. Check renamed members

Scan for renamed APIs that will still compile but may cause confusion:
- `Popup.PlacementMode` → `Popup.Placement` (property renamed, enum still exists)
- `TextBox.Watermark` → `TextBox.PlaceholderText`
- `TextBox.UseFloatingWatermark` → `TextBox.UseFloatingPlaceholder`
- `Window.SystemDecorations` → `Window.WindowDecorations`
- `RenderOptions.TextRenderingMode` → `TextOptions.TextRenderingMode`
- `TextBlock.LetterSpacing` → `TextElement.LetterSpacing`
- `Color.ToUint32()` → `Color.ToUInt32()` (case change)
- `Screen.PixelDensity` → `Screen.Scaling`
- `Screen.Primary` → `Screen.IsPrimary`
- `BindingPriority.TemplatedParent` → `BindingPriority.Template`
- `CubicBezierEasing` → `SplineEasing`
- `CustomAnimatorBase` → `InterpolatingAnimator<T>`
- `ContextMenu.PlacementMode` → `ContextMenu.Placement`
- `PseudolassesExtensions` → `PseudoClassesExtensions` (typo fix)

### 8. Distinguish [PrivateApi] public interfaces from truly internal interfaces

`[PrivateApi]` is a documentation attribute — it does NOT change the accessibility of the type. A `public interface` annotated with `[PrivateApi]` is still fully compilable and usable. NEVER replace such interfaces with alternatives like `TopLevel`.

- **`[PrivateApi]` public interfaces (USE DIRECTLY):** `IInputRoot` — still `public`, compiles fine, use as-is
- **Truly `internal` interfaces (NEED reflection or extraction):** `IRenderRoot`, `ILayoutRoot` — cannot be referenced from external assemblies

When you encounter `[PrivateApi]` on a public type, check the actual C# access modifier. If it's `public`, use it directly. Only use reflection/extraction for types that are genuinely `internal` or `private`.

### 9. Prefer existing ReflectionExtensions over new reflection code

AtomUI already has a comprehensive set of ReflectionExtensions that wrap internal/private Avalonia members. Before writing new reflection code during migration, check the **AtomUI ReflectionExtensions Catalog** section below. If an extension already exists for the member you need, use it directly. Only create new ReflectionExtensions when no existing one covers the target member.

### 10. AOT-safe reflection patterns

When accessing private/internal Avalonia APIs:
- Use `[DynamicDependency]` attributes to mark members for AOT preservation
- Use `Lazy<T>` to cache reflection info
- Use `GetXxxInfoOrThrow()` for safe reflection
- Follow ReflectionExtensions naming convention
- Document in reflection-extensions-pattern.md

### 11. Always scan base classes in AtomUI.Controls

AtomUI uses a layered architecture for multi-platform support:

- **`AtomUI.Controls`** — Platform-agnostic base controls (shared across all platforms)
- **`AtomUI.Desktop.Controls`** — Desktop-specific control implementations
- **`AtomUI.Mobile.Controls`** (planned) — Mobile-specific control implementations

Desktop and Mobile controls typically inherit from abstract base classes in `AtomUI.Controls`. For example:
- `AtomUI.Desktop.Controls.ScrollBar` → `AtomUI.Controls.Commons.AbstractScrollBar`
- `AtomUI.Desktop.Controls.ScrollViewer` → `AtomUI.Controls.Commons.AbstractScrollViewer`

**When migrating a control in `AtomUI.Desktop.Controls` (or `AtomUI.Mobile.Controls`):**
1. Identify the base class by reading the class declaration
2. If the base class is in `AtomUI.Controls`, scan it for breaking changes too
3. Apply fixes to both the base class and the derived class
4. Report issues from both layers in the migration report

**Why:** Breaking changes in the base class affect all platform-specific implementations. Missing base class issues leads to runtime bugs (e.g., `RawInputEventArgs.Root` comparison failures) that are hard to trace back to the migration.

## Execution Steps

### Step 1: Determine scope and platform

- Single file or entire project?
- Target platform(s): Desktop, Android, iOS, Browser, Headless?
- Current .NET version?
- Existing Avalonia version?
- AOT compilation planned?

### Step 2: Scan for all breaking changes

Check all 50+ categories:

**Core Framework (13 categories)**
1. .NET version requirements (8+ required, 10 recommended)
2. Avalonia package versions (all must be v12)
3. Binding class hierarchy (IBinding → BindingBase, InstancedBinding removed)
4. Compiled bindings (now enabled by default)
5. Binding plugins (now `internal`, data validation disabled by default)
6. Text shaper configuration (must call UseHarfBuzz)
7. Touch/pen selection behavior (triggers on release, not press)
8. TopLevel API changes (VisualRoot now `protected internal`, IInputRoot still public with [PrivateApi], IRenderRoot/ILayoutRoot truly internal)
9. Window decoration redesign (TitleBar/CaptionButtons/ChromeOverlayLayer removed → WindowDrawnDecorations)
10. Focus event improvements (unified FocusChangedEventArgs, KeyboardNavigationHandler now internal)
11. Extension methods & helper utilities (GetVisualRoot, GetRootElement, etc.)
12. Multiple dispatchers support (one per thread, use AvaloniaObject.Dispatcher)
13. Animations stopped on invisible controls (set PlaybackBehavior to Always to restore)

**Data & Clipboard (4 categories)**
14. Clipboard API (IDataObject → IAsyncDataTransfer)
15. Drag-drop API (DoDragDrop → DoDragDropAsync)
16. DataFormats (DataFormats.* → DataFormat.*)
17. Windows BinaryFormatter removed (explicit serialization needed for clipboard)

**Text & Rendering (5 categories)**
18. Text formatting constructors (parameter order changed)
19. Access keys (now triggered by symbol, not virtual key; AccessKey is `string?`)
20. Font support (Type 1 fonts no longer supported)
21. Direct2D1 removed (use Skia instead)
22. Render target and platform surface interfaces reworked (CRITICAL for custom backends)

**Platform-Specific (9 categories)**
23. Android app initialization (AvaloniaMainActivity non-generic + AvaloniaAndroidApplication<TApp>)
24. Android lifetime (IActivityApplicationLifetime replaces ISingleViewApplicationLifetime)
25. Android CreateAppBuilder/CustomizeAppBuilder removed
26. iOS scene-based lifecycle (AvaloniaAppDelegate.Window stays null)
27. Browser Blazor package (removed, use Avalonia.Browser)
28. Tizen support (removed)
29. Diagnostics package (Avalonia.Diagnostics → AvaloniaUI.DiagnosticsSupport)
30. xUnit.net v3 (updated from v2)
31. NUnit v4 (updated from v3)

**API Changes (12 categories)**
32. Screen class (now abstract)
33. ResourcesChangedEventArgs (now readonly record struct)
34. Gesture events (Gestures class now `internal`, events moved to InputElement)
35. Window.WindowState (now direct property, not styled)
36. Data validation (enabled by default in custom controls)
37. IPopupHost now `internal` (was public)
38. IRenderer now `[PrivateApi]` (was public)
39. VisualLayerManager changes (AdornerLayer/OverlayLayer access changed)
40. FuncMultiValueConverter (new IReadOnlyList<TIn> constructor, IEnumerable kept for compat)
41. Popup changes (new properties: OverlayDismissEventPassThrough, ShouldUseOverlayLayer, etc.)
42. Popup.PlacementMode renamed to Popup.Placement (enum PlacementMode still exists)

**Renamed & Removed Members (5 categories)**
43. Renamed members (TextBox.Watermark→PlaceholderText, RenderOptions→TextOptions, etc.)
44. Comprehensive obsolete member removals (40+ items from Avalonia 11 now removed)
45. Extension methods & helper utilities using internalized APIs
46. ReflectionExtensions pattern for AOT (DynamicDependency, Lazy<T> caching)
47. Internal API extraction strategy (extract vs reflect for internal APIs)

**AtomUI-Specific (5 categories)**
48. PlacementMode usage in PopupUtils (property renamed, refactor needed)
49. IInputRoot is [PrivateApi] but still public — use directly, only use reflection for truly internal members
50. ReflectionExtensions for internal members (wrap internal/private access)
51. Windows ExtendClientAreaToDecorationsHint behavior improved
52. Popup.MotionAwareOpen/MotionAwareClose removed in AtomUI 6.0 — use Popup.IsOpen directly

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

#### 5. Binding Plugins Now Internal (MEDIUM)

**What changed:** `BindingPlugins` class is now `internal`. Data validation plugin disabled by default. Related types removed: `DataValidationBase`, `ExceptionValidationPlugin`, `IDataValidationPlugin`, `IndeiValidationPlugin`, `IPropertyAccessorPlugin`, `IStreamPlugin`, `PropertyAccessorBase`, `PropertyError`.

**Detection:**
```csharp
BindingPlugins.DataValidators.Add(new ExceptionValidationPlugin());
```

**Fix:** Remove plugin registration. Enable validation with `.WithDataAnnotationsValidation()` in `AppBuilder` if needed.

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

**What changed:** `Visual.VisualRoot` changed from `public` to `protected internal` (not removed, but inaccessible from outside). Use `TopLevel.GetTopLevel(visual)`. `IRenderRoot` and `ILayoutRoot` are now truly `internal`. `IInputRoot` is marked `[PrivateApi]` but remains a `public interface` — it can and should be used directly. New `IPresentationSource` interface introduced (internal). `KeyboardNavigationHandler` is now `internal` — use `FocusManager.GetNextElement` instead.

**IMPORTANT:** `[PrivateApi]` is a documentation attribute only. It does NOT change the C# access modifier. `IInputRoot` is still `public` and fully usable. Do NOT replace `IInputRoot` with `TopLevel` — this will break code because `RawInputEventArgs.Root` returns `PresentationSource` (which implements `IInputRoot`), NOT `TopLevel`.

**Detection patterns:**
```csharp
// Pattern 1: Direct VisualRoot access (now protected internal)
var root = visual.VisualRoot as IRenderRoot;
if (root != null) { }

// Pattern 2: IRenderRoot / ILayoutRoot usage (now truly internal)
IRenderRoot renderRoot = ...;
ILayoutRoot layoutRoot = ...;

// Pattern 3: KeyboardNavigationHandler usage (now internal)
KeyboardNavigationHandler.GetNext(element, direction)
```

**Fix:**
```csharp
// Pattern 1: Use TopLevel.GetTopLevel()
var topLevel = TopLevel.GetTopLevel(visual);
if (topLevel is WindowBase window) { }

// Pattern 2: Use reflection or extraction for IRenderRoot/ILayoutRoot
// See Category 35 for internal API extraction strategy

// Pattern 3: Use FocusManager instead
focusManager.TryMoveFocus(NavigationDirection.Next)
```

**NO fix needed for IInputRoot usage:**
```csharp
// These are ALL CORRECT in Avalonia 12 — do NOT change them:
public void Update(IInputRoot root, Visual? candidateToolTipHost)  // OK
if (root == currentToolTip?.GetVisualRoot() as IInputRoot)          // OK
if (e.Root == currentTip.GetVisualRoot() as IInputRoot)             // OK
e.Root.GetRootElement() == _tipControl?.GetVisualRoot()             // OK
```

**Why:** The visual tree architecture was refactored. `TopLevel` is now the primary way to access the root visual. `IPresentationSource` is the new internal abstraction that implements `IInputRoot`. `Visual.VisualRoot` still exists but is `protected internal`. `IInputRoot` is marked `[PrivateApi]` but remains `public` — it compiles and works correctly. `IRenderRoot` and `ILayoutRoot` are truly `internal` and need reflection/extraction.

#### 9. Window Decoration Redesign (HIGH)

**What changed:** Complete redesign of window decoration system. Many types removed, replaced by `WindowDrawnDecorations` template-based system.

**Removed types:**
- `Chrome.TitleBar` class
- `Chrome.CaptionButtons` class
- `ChromeOverlayLayer` class
- `LightDismissOverlayLayer` class
- `SystemDecorations` enum
- `ExtendClientAreaChromeHints` enum
- `IPopupHostProvider` interface
- `IPopupHost` interface (now `internal`)

**Removed properties:**
- `VisualLayerManager.AdornerLayer` → use `AdornerLayer.GetAdornerLayer()`
- `VisualLayerManager.ChromeOverlayLayer` → use `WindowDrawnDecorations`
- `VisualLayerManager.LightDismissOverlayLayer` → removed
- `VisualLayerManager.OverlayLayer` → use `OverlayLayer.GetOverlayLayer()`
- `Window.ExtendClientAreaChromeHints` → use `Window.WindowDecorations` + `ExtendClientAreaToDecorationsHint`

**New types:**
- `WindowDrawnDecorations` — template-based decoration manager
- `WindowDrawnDecorationsContent` — holds Overlay, Underlay, FullscreenPopover slots
- `IWindowDrawnDecorationsTemplate` — template interface
- `DrawnWindowDecorationParts` enum — flags for Shadow, Border, TitleBar, ResizeGrips
- `WindowDecorationsElementRole` enum — roles: None, TitleBar, CloseButton, MinimizeButton, MaximizeButton, ResizeN/S/E/W/NE/NW/SE/SW, etc.
- `WindowDecorationProperties.ElementRoleProperty` — attached property for marking element roles

**Detection:**
```xml
<Chrome:TitleBar />
<Chrome:CaptionButtons />
```
```csharp
var layer = VisualLayerManager.ChromeOverlayLayer;
Window.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.PreferSystemChrome;
```

**Fix:**
```xml
<Chrome:WindowDrawnDecorations />
```
```csharp
var adorner = AdornerLayer.GetAdornerLayer(visual);
var overlay = OverlayLayer.GetOverlayLayer(visual);
// Use Window.WindowDecorations instead of ExtendClientAreaChromeHints
```

**Why:** The old decoration system was inflexible. The new template-based system allows full customization of window chrome with explicit role-based hit testing.

#### 10. Focus Event Changes (HIGH)

**What changed:** Both `GotFocus` and `LostFocus` now use `FocusChangedEventArgs` (with `NewFocusedElement`, `OldFocusedElement`, `NavigationMethod`, `KeyModifiers`). `GotFocusEventArgs` class removed. `KeyboardNavigationHandler` is now `internal` — use `IFocusManager.TryMoveFocus(direction, options)` instead.

**Detection:**
```csharp
protected override void OnGotFocus(GotFocusEventArgs e)
protected override void OnLostFocus(RoutedEventArgs e)
KeyboardNavigationHandler.GetNext(element, direction)
```

**Fix:**
```csharp
protected override void OnGotFocus(FocusChangedEventArgs e)
protected override void OnLostFocus(FocusChangedEventArgs e)
focusManager.TryMoveFocus(NavigationDirection.Next)
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

**What changed:** `Avalonia.Diagnostics` package → `AvaloniaUI.DiagnosticsSupport`. The extension method `AttachDevTools()` may be renamed to `AttachDeveloperTools()` in the new package (verify with the package version you use).

**Detection:**
```csharp
AttachDevTools();
```
```xml
<PackageReference Include="Avalonia.Diagnostics" Version="11.x" />
```

**Fix:**
```csharp
// Method name depends on AvaloniaUI.DiagnosticsSupport version
AttachDeveloperTools();
// or AttachDevTools() — check the package API
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

**What changed:** Now a `readonly record struct` (was class). Use `ResourcesChangedEventArgs.Create()` to construct with auto-incremented sequence numbers.

**Detection:**
```csharp
var args = new ResourcesChangedEventArgs();
```

**Fix:**
```csharp
var args = ResourcesChangedEventArgs.Create();
```

#### 28. Gesture Events — Gestures Class Now Internal (MEDIUM)

**What changed:** `Gestures` class is now `internal`. All attached events (`Holding`, `Tapped`, `RightTapped`, `DoubleTapped`, `Pinch`, etc.) moved to `InputElement` as direct events. Remove `Gestures.` prefix in XAML and code.

**Detection:**
```xml
<Button Gestures.Pinch="Button_Pinch" />
```
```csharp
Gestures.TappedEvent
Gestures.DoubleTappedEvent
```

**Fix:**
```xml
<Button Pinch="Button_Pinch" />
```
```csharp
InputElement.TappedEvent
InputElement.DoubleTappedEvent
```

#### 29. Window.WindowState (MEDIUM)

**What changed:** Now a direct property, not styled property. Can't set from style.

**Action:** Set `WindowState` in code-behind or binding, not in styles.

#### 30. Data Validation Default (LOW)

**What changed:** Data validation enabled by default for properties with `enableDataValidation: true`.

**Action:** Remove `UpdateDataValidation` overrides that only call `DataValidationErrors.SetError`.

#### 31. Extension Methods & Helper Utilities (HIGH)

**What changed:** Custom extension methods using removed or truly internal APIs need updating. Common patterns:
- `GetVisualRoot()` extension using `GetPresentationSource()` (needed because `Visual.VisualRoot` is now `protected internal`)
- `GetRootElement()` method on IInputRoot (IInputRoot itself is still public and usable)
- Custom utilities wrapping truly `internal` interfaces like `IRenderRoot`, `ILayoutRoot`

**Detection:**
```csharp
// In VisualExtensions.cs or similar utility files
internal static Visual? GetVisualRoot(this Visual visual)
{
    return visual.GetPresentationSource()?.RootVisual;
}

// Usage in service classes — these are FINE, no migration needed:
if (Presenter?.GetVisualRoot() != null)
if (e.Root == currentTip.GetVisualRoot() as IInputRoot)
```

**Fix:**
```csharp
// GetVisualRoot() extension is correct as-is if it uses GetPresentationSource()
// IInputRoot usage is correct as-is — do NOT replace with TopLevel
// Only fix extensions that reference truly internal types (IRenderRoot, ILayoutRoot)

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

#### 33. Popup.PlacementMode Renamed to Popup.Placement (HIGH)

**What changed:** The `Popup.PlacementMode` property is renamed to `Popup.Placement`. The `PlacementMode` enum itself still exists and is unchanged. Similarly, `ContextMenu.PlacementMode` → `ContextMenu.Placement`.

**Detection:**
```csharp
popup.PlacementMode = PlacementMode.Bottom;
contextMenu.PlacementMode = PlacementMode.Right;
// Note: PlacementMode enum usage is fine, only the property name changed
```

**Fix:**
```csharp
popup.Placement = PlacementMode.Bottom;
contextMenu.Placement = PlacementMode.Right;
// PlacementMode enum values remain the same
```

**XAML Detection:**
```xml
<Popup PlacementMode="Bottom" />
```

**XAML Fix:**
```xml
<Popup Placement="Bottom" />
```

**Why:** Property renamed for consistency. The `PlacementMode` enum is NOT removed — only the property accessor name changed. Code that uses `PlacementMode` enum values directly (e.g., in switch statements, comparisons) does NOT need changes.

#### 34. IInputRoot Interface — [PrivateApi] but Still Public (LOW)

**What changed:** `IInputRoot` interface is now marked `[PrivateApi]` but remains a `public interface`. It is fully compilable and usable. `IRenderRoot` is truly `internal`. `ILayoutRoot` is truly `internal`. `PresentationSource` (internal class) implements `IInputRoot` and is the actual object returned by `RawInputEventArgs.Root`.

**IMPORTANT:** Do NOT replace `IInputRoot` usage with `TopLevel`. `RawInputEventArgs.Root` returns a `PresentationSource` object which implements `IInputRoot` but is NOT a `TopLevel`. Replacing `IInputRoot` with `TopLevel` will cause comparisons to always fail and break functionality (e.g., tooltips stop triggering).

**No migration needed for:**
```csharp
// All of these are CORRECT in Avalonia 12:
public void Process(IInputRoot root)           // OK — IInputRoot is public
if (root is IInputRoot inputRoot)              // OK — compiles and works
e.Root == currentTip.GetVisualRoot() as IInputRoot  // OK
public void Update(IInputRoot root, Visual? candidateToolTipHost)  // OK
```

**Only use reflection for truly internal members of IInputRoot:**
```csharp
// RootElement property is not on the public interface — needs reflection
typeof(IInputRoot).GetProperty("RootElement", ...)  // Use ReflectionExtensions pattern
```

**Why:** `[PrivateApi]` is a documentation-only attribute indicating the API may change in future versions. It does NOT change the C# access modifier. `IInputRoot` is still `public` and the correct type to use when working with `RawInputEventArgs.Root`. Only members that are not part of the public interface surface (like `RootElement` on `PresentationSource`) need reflection.

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

### New Categories (37-51)

#### 37. Multiple Dispatchers Support (MEDIUM)

**What changed:** Avalonia 12 supports multiple dispatchers (one per thread). Library/control authors must use `AvaloniaObject.Dispatcher` or `Dispatcher.CurrentDispatcher` instead of assuming a single global dispatcher.

**Detection:**
```csharp
Dispatcher.UIThread.InvokeAsync(...)
```

**Fix:**
```csharp
// In control code, use the object's own dispatcher
this.Dispatcher.InvokeAsync(...)
// Or use current thread's dispatcher
Dispatcher.CurrentDispatcher.InvokeAsync(...)
```

**Note:** `DispatcherTimer` and `AvaloniaSynchronizationContext` use the current dispatcher by default. Ensure instantiations happen on the correct thread or pass the target dispatcher to the constructor.

#### 38. Animations Stopped on Invisible Controls (LOW)

**What changed:** Animations no longer tick when a control is hidden (`IsVisible = false`).

**Detection:** Controls that rely on animations continuing while hidden.

**Fix:**
```csharp
// Set PlaybackBehavior to Always to restore old behavior
animation.PlaybackBehavior = PlaybackBehavior.Always;
```

**Why:** Performance optimization — hidden controls don't need animation updates.

#### 39. Windows: BinaryFormatter Removed for Clipboard (MEDIUM - Windows)

**What changed:** Avalonia no longer uses .NET's `BinaryFormatter` for clipboard serialization. Custom objects on the clipboard must be explicitly serialized.

**Detection:**
```csharp
// Putting custom objects on clipboard without explicit serialization
clipboard.SetDataObjectAsync(myCustomObject);
```

**Fix:**
```csharp
// Explicitly serialize using preferred mechanism (e.g., JSON)
var json = JsonSerializer.Serialize(myCustomObject);
await clipboard.SetTextAsync(json);
```

**Why:** `BinaryFormatter` is deprecated in .NET for security reasons.

#### 40. Renamed Members (MEDIUM)

**What changed:** Multiple APIs renamed for consistency. Old names may be kept as `[Obsolete]` temporarily.

**Renames:**
| Old Name | New Name | Severity |
|----------|----------|----------|
| `Popup.PlacementMode` | `Popup.Placement` | HIGH |
| `ContextMenu.PlacementMode` | `ContextMenu.Placement` | HIGH |
| `TextBox.Watermark` | `TextBox.PlaceholderText` | MEDIUM |
| `TextBox.UseFloatingWatermark` | `TextBox.UseFloatingPlaceholder` | MEDIUM |
| `Window.SystemDecorations` | `Window.WindowDecorations` | MEDIUM |
| `RenderOptions.TextRenderingMode` | `TextOptions.TextRenderingMode` | MEDIUM |
| `TextBlock.LetterSpacing` | `TextElement.LetterSpacing` (attached) | MEDIUM |
| `Color.ToUint32()` | `Color.ToUInt32()` (case) | LOW |
| `Screen.PixelDensity` | `Screen.Scaling` | LOW |
| `Screen.Primary` | `Screen.IsPrimary` | LOW |
| `BindingPriority.TemplatedParent` | `BindingPriority.Template` | MEDIUM |
| `PseudolassesExtensions` | `PseudoClassesExtensions` (typo) | LOW |
| `X11PlatformOptions.ExterinalGLibMainLoopExceptionLogger` | `ExternalGLibMainLoopExceptionLogger` (typo) | LOW |
| `AttachDevTools()` | `AttachDeveloperTools()` (verify with DiagnosticsSupport package) | MEDIUM |

**Detection:** Search for old names in code and XAML.

**Fix:** Replace with new names. Use IDE rename refactoring for safety.

#### 41. Comprehensive Obsolete Member Removals (HIGH)

**What changed:** 40+ members deprecated in Avalonia 11 are now removed in Avalonia 12.

**Removed from Avalonia.Base:**
- `CubicBezierEasing` → use `SplineEasing`
- `CustomAnimatorBase` / `CustomAnimatorBase<T>` → use `InterpolatingAnimator<T>`
- `IStyleable` interface → use `StyledElement`
- `RadialGradientBrush.Radius` → use `RadiusX` and `RadiusY`
- `Color.ToUint32()` → use `Color.ToUInt32()` (case change)
- `DrawingContext.PushPreTransform()` / `PushPostTransform()` / `PushTransformContainer()` → use `DrawingContext.PushTransform()`
- `AvaloniaObjectExtensions.Bind()` → use `AvaloniaObject.Bind()`

**Removed from Avalonia.Controls:**
- `IActivatableApplicationLifetime` → use `Application.Current.TryGetFeature<IActivatableLifetime>()`
- `FileDialog` / `OpenFileDialog` / `OpenFolderDialog` / `SaveFileDialog` → use `IStorageProvider`
- `SystemDialog` class (206 lines) → use `IStorageProvider`
- `ItemContainerGenerator.ContainerFromIndex()` / `IndexFromContainer()` → use `ItemsControl` methods
- `TreeContainerIndex` → use `TreeView`
- `TreeItemContainerGenerator` → use `ItemContainerGenerator`
- `ToggleButton.Checked` / `Unchecked` / `Indeterminate` events → use `ToggleButton.IsCheckedChanged`
- `Screen.PixelDensity` → use `Screen.Scaling`
- `Screen.Primary` → use `Screen.IsPrimary`
- `Screens.ScreenFromWindow()` → use `Screens.ScreenFromTopLevel()`
- `AppBuilder.LifetimeOverride` property → removed
- `IApplicationPlatformEvents` interface → removed
- `IInsetsManager.DisplayEdgeToEdge` → use `IInsetsManager.DisplayEdgeToEdgePreference`

**Detection:** Search for any of the above names in code.

**Fix:** Replace with the corresponding new API.

#### 42. IPopupHost Now Internal (HIGH)

**What changed:** `IPopupHost` interface changed from `public` to `internal`.

**Detection:**
```csharp
IPopupHost host = popup.Host;
if (host != null) { host.Close(); }
```

**Fix:**
```csharp
// Use Popup.IsOpen instead
if (popup.IsOpen)
{
    popup.IsOpen = false;
}
```

**Why:** Popup hosting is now an internal implementation detail. Use `Popup.IsOpen` for state management.

#### 43. IRenderer Now [PrivateApi] (MEDIUM)

**What changed:** `IRenderer` interface is now marked `[PrivateApi]`. Not for public consumption.

**Detection:**
```csharp
IRenderer renderer = topLevel.Renderer;
renderer.AddDirty(visual);
```

**Fix:** Avoid direct `IRenderer` usage. Use higher-level APIs like `InvalidateVisual()` or `InvalidateArrange()`.

#### 44. VisualLayerManager Changes (MEDIUM)

**What changed:** `VisualLayerManager` is now public but layer access methods changed.

**Detection:**
```csharp
var adorner = VisualLayerManager.AdornerLayer;
var overlay = VisualLayerManager.OverlayLayer;
var chrome = VisualLayerManager.ChromeOverlayLayer;
var dismiss = VisualLayerManager.LightDismissOverlayLayer;
```

**Fix:**
```csharp
var adorner = AdornerLayer.GetAdornerLayer(visual);
var overlay = OverlayLayer.GetOverlayLayer(visual);
// ChromeOverlayLayer → use WindowDrawnDecorations
// LightDismissOverlayLayer → removed
```

#### 45. FuncMultiValueConverter Parameter Type (LOW)

**What changed:** New constructor accepting `Func<IReadOnlyList<TIn?>, TOut>`. Old `IEnumerable<TIn?>` constructor kept for backward compatibility.

**Detection:**
```csharp
new FuncMultiValueConverter<string, string>(values => string.Join(", ", values))
```

**Fix:** No change required — both constructors work. Prefer `IReadOnlyList<TIn?>` for new code as it provides indexed access.

#### 46. Popup New Properties (MEDIUM)

**What changed:** New properties added to `Popup` control.

**New properties:**
- `OverlayDismissEventPassThrough` — whether dismiss events pass through overlay
- `OverlayInputPassThroughElement` — element that receives input through overlay
- `ShouldUseOverlayLayer` — whether popup should use overlay layer
- `IsUsingOverlayLayer` — read-only, whether popup is currently using overlay

**Impact:** These provide more control over popup overlay behavior. Review existing popup customizations.

#### 47. Android: CreateAppBuilder/CustomizeAppBuilder Removed (MEDIUM - Android)

**What changed:** Virtual methods `CreateAppBuilder()` and `CustomizeAppBuilder(AppBuilder)` removed from `AvaloniaMainActivity`.

**Detection:**
```csharp
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CreateAppBuilder() => ...;
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) => ...;
}
```

**Fix:** Move logic to `AvaloniaAndroidApplication<TApp>` subclass or `App` class.

#### 48. Render Target and Platform Surface Interfaces (CRITICAL - Custom Backends)

**What changed:** Major rework of rendering interfaces. Only affects custom rendering backend implementations.

**Key changes:**
- `IRenderTarget.CreateDrawingContext` now takes `RenderTargetSceneInfo` parameter
- `IRenderTargetBitmapImpl` no longer extends `IRenderTarget`
- `IDrawingContextLayerImpl` no longer extends `IRenderTargetBitmapImpl`
- Platform surfaces use typed `IPlatformRenderSurface` instead of `IEnumerable<object>`
- `ISkiaGpu` now internal
- Versioned interfaces merged (e.g., `IRenderTarget2`, `ISkiaGpuRenderTarget2`)
- `ILockedFramebuffer` now includes `AlphaFormat` property
- `LockedFramebuffer` constructor requires `AlphaFormat` parameter
- `Bitmap.CopyPixels()` no longer accepts `AlphaFormat` parameter

**Impact:** Only affects code implementing custom rendering backends. Standard Avalonia usage is unaffected.

#### 49. Windows ExtendClientAreaToDecorationsHint Improved (LOW - Windows)

**What changed:** `ExtendClientAreaToDecorationsHint` now works correctly in all scenarios on Windows.

**Action:** Remove previous workarounds (margin adjustments, manual offset calculations) that compensated for the old buggy behavior.

#### 50. Dispatcher.InvokeAsync Captures Execution Context (LOW)

**What changed:** `Dispatcher.InvokeAsync` now captures and flows execution context from the caller (AsyncLocal, impersonation, culture).

**Impact:** Most async usages now behave as expected. Code that relied on execution context NOT flowing may need adjustment.

#### 51. AccessText.AccessKey Type Changed (MEDIUM)

**What changed:** `AccessText.AccessKey` property type changed from `char` to `string?`. Access keys now triggered by printed symbol, not virtual key. Accented characters and numbers now work as access keys.

**Detection:**
```csharp
char key = accessText.AccessKey;
```

**Fix:**
```csharp
string? key = accessText.AccessKey;
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

1. Scan all 50+ categories
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
- Ignoring any of 50+ categories
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
- Missing PlacementMode rename checks
- Replacing [PrivateApi] public interfaces (like IInputRoot) with alternatives — they are still public and usable
- Claiming APIs are "removed" or "internal" when they are actually `public` with `[PrivateApi]`
- Missing checks for renamed members (Watermark→PlaceholderText, etc.)
- Missing checks for internalized classes (Gestures, BindingPlugins, IPopupHost, KeyboardNavigationHandler)
- Writing new reflection code when an existing ReflectionExtension already covers the target member (check the catalog first)

## AtomUI ReflectionExtensions Catalog

When migrating code that accesses internal/private Avalonia members, **always check this catalog first** and use existing extensions instead of writing new reflection code.

### Infrastructure (AtomUI.Core — `AtomUI.Reflection` namespace)

| File | Class | Description |
|------|-------|-------------|
| `src/AtomUI.Core/Reflection/TypeMemberExtension.cs` | `TypeMemberExtension` | Safe reflection helpers: `TryGetPropertyInfo`, `TryGetFieldInfo`, `TryGetMethodInfo`, `TryGetEventInfo`, and `*OrThrow` variants |
| `src/AtomUI.Core/Reflection/ObjectExtension.cs` | `ObjectExtension` | Instance-level reflection: `TryGetProperty<T>`, `GetPropertyOrThrow<T>`, `TrySetProperty<T>`, `TryGetField<T>`, `GetFieldOrThrow<T>`, `TrySetField<T>`, `TryInvokeMethod`, `InvokeMethodOrThrow` |

### AtomUI.Core Extensions

| File | Class | Extension Method | Target Type | Wrapped Member | Access |
|------|-------|-----------------|-------------|----------------|--------|
| `src/AtomUI.Core/Input/IInputRootRefectionExtensions.cs` | `IInputRootReflectionExtensions` | `GetRootElement(this IInputRoot)` | `IInputRoot` | `RootElement` property | NonPublic |
| `src/AtomUI.Core/Utils/AvaloniaPropertyReflectionExtensions.cs` | `AvaloniaPropertyReflectionExtensions` | `InvokeNotifying(this AvaloniaProperty, AvaloniaObject, bool)` | `AvaloniaProperty` | `Notifying` property (delegate) | NonPublic |
| `src/AtomUI.Core/Controls/VisualReflectionExtensions.cs` | `VisualReflectionExtensions` | `SetVisualParent(this Visual, Control?)` | `Visual` | `SetVisualParent()` method | NonPublic |
| | | `ClearVisualParentRecursive(this Visual)` | `Visual` | recursive `SetVisualParent(null)` | NonPublic |
| | | `GetVisualChildrenList(this Visual)` | `Visual` | `VisualChildren` property → `IAvaloniaList<Visual>` | NonPublic |
| | | `IndexOfVisualChildren(this Visual, Visual)` | `Visual` | via `GetVisualChildrenList` | NonPublic |
| | | `AddToVisualChildren(this Visual, Visual)` | `Visual` | via `GetVisualChildrenList` | NonPublic |
| | | `InsertToVisualChildren(this Visual, int, Control)` | `Visual` | via `GetVisualChildrenList` | NonPublic |
| `src/AtomUI.Core/Controls/ItemCollectionReflectionExtensions.cs` | `ItemCollectionReflectionExtensions` | `SetItemsSource(this ItemCollection, IEnumerable?)` | `ItemCollection` | `SetItemsSource()` method | NonPublic |
| `src/AtomUI.Core/Controls/RawPointerEventTypeReflectionExtensions.cs` | `RawPointerEventTypeReflectionExtensions` | `GetInputHitTestResult(this RawPointerEventArgs)` | `RawPointerEventArgs` | `InputHitTestResult` property | NonPublic |
| `src/AtomUI.Core/Animations/AnimatableReflectionExtensions.cs` | `AnimatableReflectionExtensions` | `EnableTransitions(this Animatable)` | `Animatable` | `EnableTransitions()` method | NonPublic |
| | | `DisableTransitions(this Animatable)` | `Animatable` | `DisableTransitions()` method | NonPublic |
| `src/AtomUI.Core/Reflection/StyledElementReflectionExtensions.cs` | `StyledElementReflectionExtensions` | `GetLogicalChildrenList(this StyledElement)` | `StyledElement` | `LogicalChildren` property → `IAvaloniaList<ILogical>` | NonPublic |
| | | `AddToLogicalChildren(this StyledElement, ILogical)` | `StyledElement` | via `GetLogicalChildrenList` | NonPublic |
| | | `InsertToLogicalChildren(this StyledElement, int, Control)` | `StyledElement` | via `GetLogicalChildrenList` | NonPublic |
| | | `SetTemplatedParent(this StyledElement, AvaloniaObject?)` | `StyledElement` | `TemplatedParent` property setter | Public |
| | | `SetTemplatedParentRecursive(this StyledElement, AvaloniaObject?)` | `StyledElement` | recursive `SetTemplatedParent` | Public |
| `src/AtomUI.Core/Media/TextFormatting/TextParagraphPropertiesReflectionExtensions.cs` | `TextParagraphPropertiesReflectionExtensions` | `GetLineSpacing(this TextParagraphProperties)` | `TextParagraphProperties` | `LineSpacing` property | NonPublic |
| | | `SetLineSpacing(this TextParagraphProperties, double)` | `TextParagraphProperties` | `LineSpacing` property | NonPublic |
| `src/AtomUI.Core/Data/DynamicResourceReflectionExtension.cs` | `DynamicResourceReflectionExtension` | `SetAnchor(this DynamicResourceExtension, object?)` | `DynamicResourceExtension` | `_anchor` field | NonPublic |
| `src/AtomUI.Core/Controls/VisualExtensions.cs` | `VisualExtensions` | `GetVisualRoot(this Visual)` | `Visual` | `GetPresentationSource()?.RootVisual` | protected internal |

### AtomUI.Controls Extensions

| File | Class | Extension Method | Target Type | Wrapped Member | Access |
|------|-------|-----------------|-------------|----------------|--------|
| `src/AtomUI.Controls/ItemsControl/ItemCollectionReflectionExtensions.cs` | `ItemCollectionReflectionExtensions` | `AddSourceChangedEvent(this ItemCollection, EventHandler?)` | `ItemCollection` | `SourceChanged` event (add) | NonPublic |
| `src/AtomUI.Controls/ItemsControl/ItemsControlReflectionExtensions.cs` | `ItemsControlReflectionExtensions` | `GetWrapFocus(this ItemsControl)` | `ItemsControl` | `WrapFocus` property | NonPublic |
| | | `SetWrapFocus(this ItemsControl, bool)` | `ItemsControl` | `WrapFocus` property | NonPublic |
| | | `GetItems(this ItemsControl)` | `ItemsControl` | `_items` field | NonPublic |
| `src/AtomUI.Controls/ItemsControl/ItemsSourceViewReflectionExtensions.cs` | `ItemsSourceViewReflectionExtensions` | `TryGetInitializedSource(this ItemsSourceView)` | `ItemsSourceView` | `TryGetInitializedSource()` method | NonPublic |
| `src/AtomUI.Controls/Primitives/TextSearchReflectionExtensions.cs` | `TextSearchUtils` | `GetEffectiveText(object?, BindingEvaluator<string?>?)` | `TextSearch` | `GetEffectiveText()` static method | NonPublic |
| `src/AtomUI.Controls/Primitives/TopLevelReflectionExtensions.cs` | `TopLevelReflectionExtensions` | `GetLastPointerPosition(this TopLevel)` | `TopLevel` | `LastPointerPosition` property | NonPublic |
| `src/AtomUI.Controls/Primitives/VisualLayers/VisualLayerManagerReflectionExtensions.cs` | `VisualLayerManagerReflectionExtensions` | `AddLayer(this VisualLayerManager, Control, int)` | `VisualLayerManager` | `AddLayer()` method | NonPublic |
| | | `GetLayers(this VisualLayerManager)` | `VisualLayerManager` | `_layers` field | NonPublic |
| `src/AtomUI.Controls/ScrollViewer/ScrollBarReflectionExtensions.cs` | `ScrollBarReflectionExtensions` | `GetTimer(this ScrollBar)` | `ScrollBar` | `_timer` field | NonPublic |
| | | `SetIsExpanded(this ScrollBar, bool)` | `ScrollBar` | `IsExpanded` property (private setter) | Public/Private |

### AtomUI.Desktop.Controls Extensions

| File | Class | Extension Method | Target Type | Wrapped Member | Access |
|------|-------|-----------------|-------------|----------------|--------|
| `src/AtomUI.Desktop.Controls/Popup/PopupReflectionExtensions.cs` | `PopupReflectionExtensions` | `AddClosingEventHandler(this Popup, EventHandler<CancelEventArgs>)` | `Popup` | `Closing` event (add) | NonPublic |
| | | `RemoveClosingEventHandler(this Popup, EventHandler<CancelEventArgs>)` | `Popup` | `Closing` event (remove) | NonPublic |
| | | `SetIgnoreIsOpenChanged(this Popup, bool)` | `Popup` | `_ignoreIsOpenChanged` field | NonPublic |
| | | `GetIgnoreIsOpenChanged(this Popup)` | `Popup` | `_ignoreIsOpenChanged` field | NonPublic |
| | | `SetPopupParent(this Popup, Control?)` | `Popup` | `SetPopupParent()` method | NonPublic |
| `src/AtomUI.Desktop.Controls/TextBlock/TextBlockReflectionExtensions.cs` | `TextBlockReflectionExtensions` | `GetMaxSizeFromConstraint(this TextBlock)` | `TextBlock` | `GetMaxSizeFromConstraint()` method | NonPublic |
| | | `GetHasComplexContent(this TextBlock)` | `TextBlock` | `HasComplexContent` property | NonPublic |

### Quick Lookup by Avalonia Type

When you encounter code accessing an internal/private member of an Avalonia type, use this index:

| Avalonia Type | Available Extensions | Using Directive |
|---------------|---------------------|-----------------|
| `IInputRoot` | `GetRootElement()` | `using AtomUI.Input;` |
| `AvaloniaProperty` | `InvokeNotifying()` | `using AtomUI.Utils;` |
| `Visual` | `SetVisualParent()`, `GetVisualChildrenList()`, `AddToVisualChildren()`, `InsertToVisualChildren()`, `IndexOfVisualChildren()`, `ClearVisualParentRecursive()`, `GetVisualRoot()` | `using AtomUI.Controls;` |
| `StyledElement` | `GetLogicalChildrenList()`, `AddToLogicalChildren()`, `InsertToLogicalChildren()`, `SetTemplatedParent()`, `SetTemplatedParentRecursive()` | `using AtomUI.Reflection;` |
| `Animatable` | `EnableTransitions()`, `DisableTransitions()` | `using AtomUI.Animations;` |
| `ItemCollection` | `SetItemsSource()`, `AddSourceChangedEvent()` | `using AtomUI.Controls;` |
| `ItemsControl` | `GetWrapFocus()`, `SetWrapFocus()`, `GetItems()` | `using AtomUI.Controls;` |
| `ItemsSourceView` | `TryGetInitializedSource()` | `using AtomUI.Controls;` |
| `RawPointerEventArgs` | `GetInputHitTestResult()` | `using AtomUI.Controls;` |
| `TextSearch` | `TextSearchUtils.GetEffectiveText()` | `using AtomUI.Controls;` |
| `TopLevel` | `GetLastPointerPosition()` | `using AtomUI.Controls.Primitives;` |
| `VisualLayerManager` | `AddLayer()`, `GetLayers()` | `using AtomUI.Controls.Primitives;` |
| `ScrollBar` | `GetTimer()`, `SetIsExpanded()` | `using AtomUI.Controls.Commons;` |
| `Popup` | `AddClosingEventHandler()`, `RemoveClosingEventHandler()`, `SetIgnoreIsOpenChanged()`, `GetIgnoreIsOpenChanged()`, `SetPopupParent()` | `using AtomUI.Desktop.Controls;` |
| `TextBlock` | `GetMaxSizeFromConstraint()`, `GetHasComplexContent()` | `using AtomUI.Desktop.Controls;` |
| `TextParagraphProperties` | `GetLineSpacing()`, `SetLineSpacing()` | `using AtomUI.Media.TextFormatting;` |
| `DynamicResourceExtension` | `SetAnchor()` | `using AtomUI.Data;` |

#### 52. Popup.MotionAwareOpen/MotionAwareClose Removed (HIGH - AtomUI 6.0)

**What changed:** In AtomUI 6.0, the custom `Popup.MotionAwareOpen()` and `Popup.MotionAwareClose()` methods are removed. Popup open/close is now controlled directly via `Popup.IsOpen`.

**Detection:**
```csharp
Popup.MotionAwareOpen(() => { HandlePopupOpened(placementTarget); });
Popup.MotionAwareClose(HandlePopupClosed);
```

**Fix:**
```csharp
Popup.IsOpen = true;
HandlePopupOpened(placementTarget);

// For close:
Popup.IsOpen = false;
HandlePopupClosed();
```

**Why:** The motion/animation system for popups was redesigned in AtomUI 6.0. Open/close animations are now handled internally by the Popup infrastructure, so callers no longer need to use motion-aware wrappers.

## References

For detailed information, see `references/` directory:

- `missed-breaking-changes.md` — Recently discovered breaking changes and corrections
- `code-level-analysis.md` — Code-level analysis from official Avalonia source (11.3.14 → 12.0.0)
- `reflection-extensions-pattern.md` — AOT-safe ReflectionExtensions pattern and best practices
- `avalonia12-breaking-changes.md` — All 50+ breaking changes with severity classification
- `migration-examples.md` — Before/after code examples for each category
- `atomui-migration-guide.md` — AtomUI-specific patterns and migration workflow
