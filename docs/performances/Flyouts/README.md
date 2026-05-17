# Flyouts Performance

## Phase 0 Baseline

Date: 2026-05-17

Scope:

- `FlyoutHost`
- `Flyout`
- `MenuFlyout`
- `DropdownButton` / `SplitButton` closed flyout path
- `PopupConfirm`
- Real Gallery `InfoFlyoutShowCase`
- Real Gallery `PopupConfirmShowCase`

Measurement policy:

- Debug + Avalonia headless.
- Control suite count: 60 roots per scenario.
- Gallery navigation: 1300x900, `--cold-iterations 10 --warmup 6 --iterations 20`.
- Gallery timing is measured from settled `AboutUs` route to target ShowCase route visual tree and layout stable.

## Current Readable Results

### Control-Level Closed Baseline

| Scenario | ms/item | KB/item | Visual/root | Logical/root |
| --- | ---: | ---: | ---: | ---: |
| `FlyoutHost.Hover.Closed` | 1.795 | 283.8 | 10 | 3 |
| `FlyoutHost.Click.Closed` | 1.805 | 281.6 | 10 | 3 |
| `FlyoutHost.Focus.Closed` | 1.506 | 282.8 | 10 | 3 |
| `PopupConfirm.Closed` | 0.995 | 281.6 | 10 | 3 |
| `DropdownButton.MenuFlyout.Closed` | 1.333 | 323.2 | 11 | 2 |
| `SplitButton.MenuFlyout.Closed` | 2.780 | 721.9 | 23 | 1 |
| `Flyouts.GalleryShape` | 31.821 | 8845.8 | 309 | 89 |

### Closed-State Popup Lazy Observation

| Scenario | Flyout objects | Popup created | Popup child created | Visuals | Logical |
| --- | ---: | ---: | ---: | ---: | ---: |
| `FlyoutHost.Hover.Closed` | 1 | 1 | 0 | 10 | 3 |
| `FlyoutHost.Click.Closed` | 1 | 1 | 0 | 10 | 3 |
| `FlyoutHost.Focus.Closed` | 1 | 1 | 0 | 10 | 3 |
| `PopupConfirm.Closed` | 1 | 1 | 0 | 10 | 3 |
| `DropdownButton.MenuFlyout.Closed` | 1 | 1 | 0 | 11 | 2 |
| `SplitButton.MenuFlyout.Closed` | 1 | 1 | 0 | 23 | 1 |
| `Flyouts.GalleryShape` | 29 | 29 | 0 | 309 | 89 |

Interpretation:

- The popup presenter/content is still lazy: `Popup child created = 0`.
- The popup shell is not lazy in closed state: every closed flyout already has `Popup created = Flyout objects`.
- This violates the rule that unused popup functionality should not pay runtime cost.
- The first Phase 1 target should be avoiding `Flyout.Popup` access during closed-state setup.

### Real Gallery ShowCase Loading

| ShowCase | Cold mean | Cold median | Cold P95 | Repeated mean | Repeated median | Repeated P95 | Repeated alloc mean | Visuals | Logical | FlyoutHost | Button |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `InfoFlyoutShowCase` | 212.97ms | 204.01ms | 256.37ms | 68.62ms | 55.81ms | 115.89ms | 9613.25KB | 346 | 99 | 28 | 28 |
| `PopupConfirmShowCase` | 159.87ms | 150.94ms | 200.27ms | 44.37ms | 48.22ms | 64.63ms | 5326.27KB | 190 | 52 | 15 | 15 |

Source shape:

| ShowCase | Source shape |
| --- | --- |
| `InfoFlyoutShowCase` | 28 `FlyoutHost`, 28 `Flyout`, 28 `Button`, 4 `ShowCaseItem` |
| `PopupConfirmShowCase` | 15 `PopupConfirm`, 15 `Button`, 4 `ShowCaseItem`, 1 `AntDesignIconProvider` |

## Phase 0 Findings

1. `FlyoutStateHelper.SetupTriggerHandler()` accesses `Flyout.Popup.IsLightDismissEnabled` in closed state. This forces `PopupFlyoutBase._popupLazy` creation before the user opens the flyout.
2. `PopupConfirmFlyout` accesses `Popup.IsLightDismissEnabled` in its constructor, so every closed `PopupConfirm` creates a popup shell during attach.
3. `PopupConfirmFlyout` also calls `BindUtils.RelayBind(...)` in the constructor without saving the returned `IDisposable`. Because `PopupConfirm` owns this flyout with the same lifetime, the next fix should prefer `[!]` binding instead of manual disposable plumbing.
4. `FlyoutHost`, `DropdownButton`, and `SplitButton` still create several `RelayBind` subscriptions for flyout property forwarding. This is a Phase 4 candidate after the popup shell problem is fixed and measured.
5. `PopupConfirmContainer.OnApplyTemplate()` attaches button `Click` handlers without first detaching from old template parts. This is a correctness/lifecycle issue and should be fixed before or with the PopupConfirm lazy fix.

## Completed Optimization

Date: 2026-05-17

Implemented changes:

- `FlyoutStateHelper` no longer touches `Flyout.Popup` during closed trigger setup.
- Trigger-specific light-dismiss state is synchronized immediately before `ShowFlyout`, so closed Hover/Focus flyouts do not pay this cost.
- `PopupConfirmFlyout` no longer creates `Popup` in the constructor.
- `PopupConfirmFlyout`, `MenuFlyout`, and `TreeViewFlyout` use Avalonia `[!]` binding where source and target lifetimes match.
- `PopupConfirmContainer` detaches old template button click handlers before wiring new template parts.
- Added `--verify-flyout-states` lifecycle verification.

### Closed-State Popup Lazy Result

| Scenario | Before popup created | After popup created | Result |
| --- | ---: | ---: | ---: |
| `FlyoutHost.Hover.Closed` | 1 / 1 | 0 / 1 | 100% removed |
| `FlyoutHost.Click.Closed` | 1 / 1 | 0 / 1 | 100% removed |
| `FlyoutHost.Focus.Closed` | 1 / 1 | 0 / 1 | 100% removed |
| `PopupConfirm.Closed` | 1 / 1 | 0 / 1 | 100% removed |
| `DropdownButton.MenuFlyout.Closed` | 1 / 1 | 0 / 1 | 100% removed |
| `SplitButton.MenuFlyout.Closed` | 1 / 1 | 0 / 1 | 100% removed |
| `Flyouts.GalleryShape` | 29 / 29 | 0 / 29 | 100% removed |

Interpretation:

- This is the main structural win. Closed Flyouts now keep both `Popup` shell and popup child lazy.
- Visual/logical node counts are unchanged, because closed popup shells were not part of the visual/logical tree.
- Allocation drops are visible in both the control suite and real Gallery navigation.

### Control-Level Allocation Result

Latest after-run command:

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite flyouts --count 60 --markdown /tmp/atomui-flyouts-after-report.md
```

| Scenario | Before KB/item | After KB/item | Improvement |
| --- | ---: | ---: | ---: |
| `FlyoutHost.Hover.Closed` | 283.8 | 264.5 | 6.80% less |
| `FlyoutHost.Click.Closed` | 281.6 | 262.4 | 6.82% less |
| `FlyoutHost.Focus.Closed` | 282.8 | 263.5 | 6.82% less |
| `PopupConfirm.Closed` | 281.6 | 262.5 | 6.78% less |
| `DropdownButton.MenuFlyout.Closed` | 323.2 | 303.5 | 6.10% less |
| `SplitButton.MenuFlyout.Closed` | 721.9 | 704.8 | 2.37% less |
| `Flyouts.GalleryShape` | 8845.8 | 8527.0 | 3.60% less |

Timing note:

- The tiny per-control headless timing scenarios are too noisy for a strong conclusion; repeated runs varied more than the expected optimization size.
- `Flyouts.GalleryShape` in the latest after run changed from 31.821ms/item to 30.320ms/item, about 4.72% faster, but the real Gallery ShowCase metrics below are the primary timing signal.

### Real Gallery ShowCase Loading Result

Measurement policy is unchanged: 1300x900 headless Gallery, `--cold-iterations 10 --warmup 6 --iterations 20`.

| ShowCase | Metric | Before | After | Improvement |
| --- | --- | ---: | ---: | ---: |
| `InfoFlyoutShowCase` | Cold mean | 212.97ms | 200.92ms | 5.66% faster |
| `InfoFlyoutShowCase` | Cold median | 204.01ms | 200.63ms | 1.66% faster |
| `InfoFlyoutShowCase` | Cold P95 | 256.37ms | 204.62ms | 20.19% faster |
| `InfoFlyoutShowCase` | Repeated mean | 68.62ms | 62.98ms | 8.22% faster |
| `InfoFlyoutShowCase` | Repeated median | 55.81ms | 53.62ms | 3.92% faster |
| `InfoFlyoutShowCase` | Repeated P95 | 115.89ms | 93.77ms | 19.09% faster |
| `InfoFlyoutShowCase` | Repeated alloc mean | 9613.25KB | 9069.55KB | 5.66% less |
| `PopupConfirmShowCase` | Cold mean | 159.87ms | 153.13ms | 4.22% faster |
| `PopupConfirmShowCase` | Cold median | 150.94ms | 148.70ms | 1.48% faster |
| `PopupConfirmShowCase` | Cold P95 | 200.27ms | 172.63ms | 13.80% faster |
| `PopupConfirmShowCase` | Repeated mean | 44.37ms | 39.89ms | 10.10% faster |
| `PopupConfirmShowCase` | Repeated median | 48.22ms | 29.43ms | 38.97% faster |
| `PopupConfirmShowCase` | Repeated P95 | 64.63ms | 65.34ms | 1.10% slower |
| `PopupConfirmShowCase` | Repeated alloc mean | 5326.27KB | 5034.05KB | 5.49% less |

Conclusion:

- The optimization matches the intended direction: closed flyout popup shell cost is removed and real Gallery allocation drops by about 5.5%.
- `PopupConfirmShowCase` repeated P95 is effectively flat/slightly noisy (`+0.71ms`), while mean and median improve. It should be watched in future runs, but this is not a repeatable regression based on the full sample.
- The remaining cost is mostly the visible page itself: 28/15 buttons, show case containers, text/layout, and routing. Further large wins likely require Button/TextBlock/ShowCase infrastructure work, not more Flyout closed-popup work.

### Verification

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-flyout-states
```

## Reproduction Commands

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite flyouts --count 60 --markdown /tmp/atomui-flyouts-baseline.md
```

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase infoflyout --warmup 6 --iterations 20 --cold-iterations 10 --timeout-ms 30000 --label flyouts-phase0-infoflyout --markdown /tmp/atomui-infoflyout-phase0.md
```

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase popupconfirm --warmup 6 --iterations 20 --cold-iterations 10 --timeout-ms 30000 --label flyouts-phase0-popupconfirm --markdown /tmp/atomui-popupconfirm-phase0.md
```
