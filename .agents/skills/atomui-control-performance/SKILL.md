---
name: atomui-control-performance
description: Use when optimizing AtomUI controls, investigating control performance regressions, changing Avalonia styled-property bindings, lazy creation, templates, selectors, or Gallery performance scenarios. Applies to controls such as Space, Button, Icon, AddOnDecoratedBox, LineEdit, and shared primitives.
---

# AtomUI Control Performance Skill

> **Correctness outranks performance.** Every rule below exists to defend that priority. If any rule conflicts with correctness, correctness wins.

---

## Tier 1 — Hard Boundaries

Violating any of these blocks merge. No exception, no tradeoff, no "it's only one case".

1. **Correctness regression is unacceptable.** Performance optimization must preserve control functionality, UI appearance, animation behavior, interaction semantics, theme behavior, and public API. Any visible or behavioral change caused by an optimization is a regression, not a tradeoff. If the user explicitly approves a behavior change, that becomes a separate non-perf change, not a perf optimization.

2. **No logic bug introduction.** This includes invalid visual states, broken state transitions, incorrect property precedence, stale layout/render state, lost interactions, incorrect lifecycle cleanup, race conditions in event chains, or Gallery-visible behavior mismatches. Discovering one mid-optimization means stop, fix or revert immediately, do not continue stacking perf changes on top.

3. **No resource leak.** Anything created/subscribed/bound/cached/lazily materialized/reparented must have a defined and verified release path before the change is considered complete. Leak scanning is mandatory during analysis, not optional cleanup afterward.

4. **No measurable speed regression.** If any primary metric for the targeted control or its real Gallery ShowCase gets worse under the same measurement policy, the change must be fixed, split, reverted, or explicitly reported as a blocker.

5. **No materially more complex implementation logic.** Fragile state machines, duplicated template/style logic in code, broad lifecycle orchestration, unclear synchronization, or code harder to reason about than the original behavior warrants — all indicate the optimization is too expensive in complexity. Stop and propose a simpler option instead. See Tier 2 §1 for the numeric thresholds that flag this.

6. **No theme element → C# dynamic creation.** See [Theme Static Rule](#theme-static-rule). This is the highest-leverage rule in this document; nearly all rollback incidents stem from violating it.

7. **No same-priority binding collisions for the same property.** If a property has both an internal default binding and an external override binding, they must be at different `BindingPriority` levels. See [Avalonia Binding Priority Guardrails](#avalonia-binding-priority-guardrails).

8. **No `BindUtils.RelayBind` + disposable plumbing for lifetime-consistent children.** When source and target have the same owner and lifetime, use Avalonia `[!]` binding syntax. Disposable infrastructure is reserved for mismatched lifetimes, replacement paths, detach/re-template, global/window subscriptions, timers, or lazily materialized objects.

9. **3-rounds optimization budget.** For the same scoped target, three implementation-and-measurement rounds without primary speed metric improvement = stop. At end-of-optimization, if no worthwhile gain: restore every perf-only change, delete intermediate scratch code/docs, keep only correctness fixes / leak fixes / useful measurement tooling. Report exactly what was reverted, what correctness fixes were kept, what cleanup was done.

10. **5-control pattern rollout circuit breaker.** Same refactor pattern (e.g., dynamic creation, lazy popup materialization, binding restructure) applied to ≥ 5 controls = stop and audit. Two questions: (a) did at least 3 of 5 show measurable speed gain? (b) did any one introduce ≥ 2 Gallery-visible bugs? Either "no" / "yes" stops further rollout and triggers reflection before the 6th control.

---

## Theme Static Rule

> Functional visuals defined in `ControlTemplate` / `ControlTheme` stay in axaml. Moving them to C# `EnsureXxx()` / `ClearXxx()` dynamic creation as a performance optimization is **prohibited by default**.

### Why this is a hard boundary

The cost asymmetry is overwhelming:

| Item | axaml static + `IsVisible` toggle | C# dynamic Ensure/Clear |
| --- | --- | --- |
| Instantiation | Template inflation once | Each Ensure rebuilds |
| Hidden cost | `IsVisible=False` skipped by measure/arrange/render, O(1) | Active until ClearXxx releases |
| TemplateBinding | Reactive via XAML | Manual `SetCurrentValue` + property change handlers |
| Style/Selector | Cascade automatically | Each rule re-implemented in C# |
| Lifecycle | Avalonia auto-managed | Manual disposables + ordered teardown |
| Race / ordering bugs | ~zero | High frequency (the rollback evidence) |

The "saved" cost of a hidden axaml node is ~200B–1KB allocation + zero layout/render. The complexity cost of dynamic creation is permanent and compounds across controls.

### Allowed exceptions (must be argued in commit description)

1. **Heavy popup content.** Subtree node count ≥ 20, or contains `ItemsControl` / `ScrollViewer` / `TreeView` / `CascaderView` / calendar/time panels / large layout trees. The lightweight `Popup` shell still stays in axaml; only the popup's CONTENT may be deferred. See [Popup Lazy Content Rule](#popup-lazy-content-rule).
2. **Adorner / overlay-layer content.** Adorners cannot live in `ControlTemplate` (architectural constraint). Notification cards, overlay dialogs, drawer overlays, tooltips fall here.
3. **Variable-N data items.** When N is unknown at compile time. Prefer `ItemsControl` + `ItemTemplate`; only when ItemsControl is structurally insufficient may `Children.Add` be used directly.

Any "fourth case" requires opening a discussion with the user and updating this skill before merge.

### Reasons that are NOT acceptable for an exception

- ❌ "Save X bytes of allocation"
- ❌ "Save unused PropertyChanged broadcasts"  
- ❌ "Element only used in mode Y" — use `IsVisible="False"` + selector instead
- ❌ "First-time inflation is slow" — one-time cost, dominated by runtime Ensure churn
- ❌ "Keep parity with TreeSelect / Select / etc." — consistency does not override hard boundary

### Right way to make a sometimes-shown element cheap

```xml
<!-- ✅ Correct: stay in axaml, hide by default, reveal via selector -->
<atom:CheckBox Name="ToggleCheckbox"
               IsChecked="{TemplateBinding IsChecked, Mode=TwoWay}"
               IsVisible="False" />

<Style Selector="^[ToggleType=CheckBox] /template/ atom|CheckBox#ToggleCheckbox">
    <Setter Property="IsVisible" Value="True" />
</Style>
```

```csharp
// ❌ Wrong: do not move axaml elements into C# dynamic creation
private CheckBox? _toggleCheckbox;
private CompositeDisposable? _toggleCheckboxDisposables;
private void EnsureToggleCheckbox() { /* ... */ }
private void ClearToggleCheckbox() { /* ... */ }
```

### Reviewer must check

Before approving any perf commit, grep new `EnsureXxx()` / `ClearXxx()` introductions and confirm each falls into one of the three allowed exceptions. If not, the commit fails this hard boundary.

---

## Popup Lazy Content Rule

This is the implementation of Theme Static Rule exception 1.

- A **lightweight `Popup` shell** stays in `ControlTheme`. It preserves placement, light-dismiss, overlay, theme styling, and required template contracts.
- **Heavy popup content** must not be created in the default closed state. Examples: candidate lists, `TreeView`, `CascaderView`, calendar/time panels, complex item presenters, filter lists, empty indicators, large popup layout trees.
- **First-open materialization.** Create popup content immediately before the first open, wire events and bindings there, sync pending selection/filter/items state after creation.
- **Keep materialized after close** by default — open/close churn is worse than the saved allocation. Release on re-template, detach, or explicit disposal paths.
- **Closed controls pay nothing.** No popup-only event subscriptions, no items-source copies, no filter setup, no selection sync, no heavy visual tree creation while popup is closed.
- **Lifecycle verification.** Cover first open, close, second open, re-template, detach, visual parent cleanup, event unsubscribe, binding disposal, and state toggles (loading/filter/selection).

---

## Avalonia Binding Priority Guardrails

### Rules

- **Do not use `IsSet(property)` as "user explicitly set this".** In Avalonia, `IsSet` returns true for any effective styled value or binding (Template/Style/Animation/LocalValue alike). Using it as a "user wrote this" check causes silent default restoration failures.
- **Internal default/token bindings must not share priority with expected external bindings on the same property.** Avalonia's immediate value frames are priority-based and property-keyed; disposing an entry at the same priority can remove or disturb another binding on the same property.
- **For internal defaults that override template-optional values in normal modes but yield in a special mode**, use a stronger separate priority and dispose it at the mode boundary. Example pattern: internal token at `StyleTrigger` for non-`Custom` modes, disposed when entering `Custom` so the external `Template` binding takes over.
- **State matrix must be exhaustive.** Whenever a property has both internal and external bindings, verify: default mode, every named mode, special/Custom mode, switching out of Custom, and source updates after switching.
- **`LocalValue` and `Animation` must always be stronger than internal defaults.** If user sets a local value, internal token binding must be disposed or not observed as the effective value.

### Incident: Space `ItemSpacing`/`LineSpacing` (preserved from v1)

Space optimization changed internal token spacing bindings without fully modeling the Gallery example:

```xml
<atom:Space SizeType="{Binding SizeType}"
            LineSpacing="{Binding #CustomSizeSlider.Value, Priority=Template}"
            ItemSpacing="{Binding #CustomSizeSlider.Value, Priority=Template}" />
```

First fix moved internal bindings to `BindingPriority.Style`, avoiding same-priority disposal collisions but creating a new bug: the always-present slider `Template` binding won in `Small`/`Middle`/`Large`, so those SizeType options stopped changing spacing. Only `Custom` still worked, masking the bug unless the full interaction was tested.

**Lesson:** Whenever you touch a property that has both internal default and external override, write down the priority of every binding on that property before changing anything.

---

## Required Process Gates

These are mandatory commit-time gates. A perf commit description without all of them filled in fails review.

### Gate 1 — Correctness self-attestation

```
[ ] Behavior / appearance / animation / interaction / theme / API unchanged.
    Verification method: ___________________
[ ] Passes per-control regression matrix (Suites/<Control>/Regression.md).
    Cases run: ___________________
[ ] Gallery ShowCase walkthrough completed for: ___________________
```

### Gate 2 — Resource lifecycle

```
[ ] Lists every (subscription / binding / timer / lazily materialized object /
    cached value / reparented element) added by this commit.
[ ] Each one's release trigger is identified and verified.
[ ] grep results for the patterns in "Resource Leak Detection" pasted below.
```

### Gate 3 — Complexity self-rating

```
[ ] New Ensure*/Clear*/Sync* methods: N (threshold ≥ 4 = needs justification)
[ ] New try/finally flag fields: M (threshold ≥ 2 = needs justification)
[ ] New disposable fields in same file: K (threshold ≥ 3 = needs justification)
[ ] axaml lines deleted vs C# lines added: ___ vs ___
[ ] Theme Static Rule check: did this commit move any axaml node into C#? Y/N.
    If Y, which of the three allowed exceptions applies? ___________________
```

If any threshold is exceeded, commit must include explicit justification (not just "for performance").

### Gate 4 — Measurement

```
[ ] cold-first-navigation: --cold-iterations 10 minimum, before/after numbers
[ ] repeated mean / median / P95: same warmup+iterations both before and after
[ ] Single-process / single-sample numbers explicitly excluded as smoke-only
```

### Gate 5 — Rollback path

```
[ ] One-line revert command from the merged tip:
    git revert <sha>   OR   git checkout <sha>~1 -- <files>
[ ] Files touched are confined enough that revert is mechanical.
```

### Gate 6 — Build & lint

```
[ ] Owning project builds clean (no new warnings)
[ ] git diff --check
[ ] No unused using directives newly introduced
```

---

## Per-Control Regression Matrix

Each control with non-trivial behavior must maintain a matrix at `tools/performances/AtomUI.Performance/Suites/<Control>/Regression.md`. Every perf commit on that control must declare which matrix entries it ran.

Minimal matrix shape:

```
# <Control> Regression Matrix

## Functional matrix (must run before any perf commit on this control)
- [ ] <feature 1 + interaction>
- [ ] <feature 2 + interaction>
- [ ] ...

## Multi-step user flows (Gallery ShowCase scripts)
- [ ] <ShowCase A>: open → step 1 → step 2 → expected outcome
- [ ] <ShowCase B>: ...

## Lifecycle matrix
- [ ] Mount → unmount → re-mount with state preserved
- [ ] Re-template
- [ ] Property toggle on/off/on
```

`<Control>StateVerification.cs` cases must map 1:1 with matrix items where automatable. Items that can only be verified by visual inspection (animation, hover transition, popup arrow position) must be listed and the reviewer must walk them in Gallery, with timestamps in the commit message.

---

## Pattern Rollout Budget

When a single optimization pattern is being applied across multiple controls, this is a **series planning concern**, not just a per-commit one.

- Apply the pattern to at most **4 controls** before a mandatory review.
- After 4 commits with the same pattern, halt and answer:
  1. Did at least 3 of 4 show measurable speed gain?
  2. Did any one introduce ≥ 2 Gallery-visible bugs that took > 30 minutes to discover?
  3. Did any one expand complexity beyond the Gate 3 thresholds?
- If 1 fails or 2/3 succeeds, do not roll out to the 5th control. Audit existing 4, decide which to keep / refine / revert.
- Decisions must be recorded in `docs/performances/<pattern>/rollout-audit.md`.

---

## Avalonia 12 Specific Pitfalls

These are not bugs, just non-obvious behaviors that have caused incidents.

- **`$parent[atom:Popup]` binding in `Popup.Child` is unreliable.** Popup.Child lives in a separate `PopupRoot` visual tree (Popup hosted in window). During motion or re-host, ancestor traversal can fail. If you need state Popup → Child, route through a control-level property that is already TwoWay-bound to the popup (e.g., `IsPopupHorizontalFlipped`).
- **TemplateBinding through `Popup.Child` IS reactive — but you still need to invalidate Arrange.** Property X computed in `ArrangeOverride` propagates to Popup.Child via TemplateBinding only when Arrange runs. Arrange must be invalidated by `AffectsArrange` registration on a triggering property, or by an explicit `InvalidateArrange()` call.
- **`IsVisible="False"` is genuinely free in measure/arrange/render.** It is not a "soft hide" — the subtree is skipped. This is what makes Theme Static Rule's "axaml static + IsVisible toggle" the cheap option.
- **Direct C# child-control creation does not get theme/style/selector cascade automatically.** If you create a CheckBox in `EnsureXxx()` and add it to `Children`, Avalonia applies styles, but selector matches relative to `/template/` may not behave the same way as if the CheckBox had been authored in the parent's `ControlTemplate`. This is a frequent source of "looks slightly off" regressions.

---

## Required Verification Pattern

For any control performance optimization:

1. Build the changed project or the smallest owning project.
2. Update `tools/performances/AtomUI.Performance/Suites/<Control>/Regression.md` if the matrix needs new entries.
3. Add or update `<Control>StateVerification.cs` with cleanup assertions for any visuals/presenters/hosts/subscriptions/bindings created.
4. If Gallery-visible: build `controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj`, walk the matrix's "Multi-step user flows" entries by hand, record results.
5. For controls with a Gallery ShowCase, run the real ShowCase loading-time measurement and report before/after.
6. `git diff --check`; ensure no unused `using`.

---

## Performance Report Format

Performance summaries must be readable to a human reviewer.

- Compact comparison table with `Scenario`, `Before`, `After`, `Improvement`, units in every value.
- Both control-level and Gallery-level results when both were measured.
- Always include corresponding ShowCase loading-time comparison: cold first navigation (≥10 independent process samples), repeated mean / median / P95, sample policy (warmup/iterations).
- Same sample policy before and after. If first pass shows regression, rerun with sufficient warmup/iterations to distinguish real regression from measurement noise before reporting final.
- Plain-language user-facing impact, e.g., "ComboBoxShowCase repeated open went from 109ms to 91ms, ~18ms faster."
- Structural wins separately when they explain the result, e.g., "Button/IconButton count: 23 → 0", "visual nodes: 562 → 497."
- **Mandatory complexity-burden line.** e.g., "Cascader refactor: cold first nav 87 → 73 ms (-16%); +12 methods / +4 disposable fields / +2 try/finally flags; axaml -38 lines / C# +422 lines."
- State whether the result matches the optimization goal. Call out fixed costs that remain when page-level percentage is lower than control-level percentage.

---

## Resource Leak Detection

Run before every perf commit and paste results in Gate 2. Any non-empty output requires explanation.

```bash
# Newly added Ensure*/Clear* must be paired
rg "Ensure[A-Z]\w+\(\)" --type cs -A 3 src/

# Each += event subscription needs matching -=
rg "\\+= Handle" --type cs src/

# CompositeDisposable usage — confirm each one is required by lifetime mismatch
rg "CompositeDisposable\\b" --type cs src/

# Children.Add / Insert without matching Remove
rg "Children\\.(Add|Insert)" --type cs src/

# Disposable fields — each must have a release path
rg "_[a-z]\\w+Disposables\\b" --type cs src/

# SetTemplatedParent(this) calls — each should have a matching SetTemplatedParent(null)
rg "SetTemplatedParent\\(this\\)" --type cs src/

# DisableTransitions/EnableTransitions pairing
rg "DisableTransitions\\(\\)|EnableTransitions" --type cs src/
```

---

## Binding / Subscription Checklist

- Treat leak scanning as mandatory during analysis, not optional cleanup.
- Before using `BindUtils.RelayBind(...)`, storing an `IDisposable` binding, or adding a `CompositeDisposable`: first confirm Avalonia `[!]` binding is insufficient (i.e., source/target lifetimes differ).
- Disposable plumbing is reserved for: mismatched lifetimes, replacement paths, detach/re-template cleanup, global/window subscriptions, timers, event handlers, lazily materialized objects that can outlive their creator.
- Store every disposable subscription/binding created outside XAML.
- Dispose old bindings before replacing.
- Avoid duplicate bindings on repeated property changes.
- If a mode disables a feature, detach visuals and dispose subscriptions in that mode.
- For created visuals/presenters/popups/timers/event handlers/property observables/bindings/caches/global subscriptions: identify owner and release trigger before editing.
- Add a regression test toggling the feature on/off/on.

---

## Style & Best Practice

- Reuse `AtomUI.Utils.MathUtils` (`AreClose`, `IsZero`, `GreaterThan`, `LessThanOrClose`, etc.) for floating-point comparison, epsilon checks, zero/one checks, ordering with tolerance, angle conversion, fixed-point rounding. Do not introduce hand-written `Math.Abs(...) < eps`, new epsilon constants, or duplicate helpers.
- `Dispatcher.Post(this.EnableTransitions);` — method-group form. Do not wrap in lambda.
- Never `Debug.Assert(value != null)` immediately followed by a nullable guard for the same value. Express the invariant in the type, helper return value, or use a real runtime guard with explicit recovery.
- Remove unused `using` directives introduced by the change.
- Prefer no API change. AtomUI has no formal release yet; API changes need explicit justification.
- Do not commit during perf optimization unless the user explicitly asks. Leave changes in working tree for review by default.

---

## Incident Callouts

Real bugs that cost the project significant rollback effort. New incidents must be appended here with a one-paragraph summary and a "what to check next time" line.

### Space ItemSpacing / LineSpacing (binding priority)

See [Avalonia Binding Priority Guardrails — Incident](#incident-space-itemspacing--linespacing-preserved-from-v1) above. Lesson: when changing token-default bindings, write down all bindings on that property and their priorities before editing.

### Cascader / Select / AddOnDecoratedBox 50+ commit rollback

A series of perf commits applied "theme element → C# dynamic creation" pattern (Pattern A) across ~50 controls. Side effects:

- **Cascader 5-bug cascade.** Single-mode first selection didn't sync to input box; popup couldn't reopen; async load was prematurely closed; clicking already-selected leaf didn't close popup; `_ignoreSelectedPropertyChanged` flag deadlocked.
- **OptionsSource churn.** `_cascaderView.OptionsSource = Options.Cast<...>().ToList()` on every property change triggered Remove notifications on the underlying `_options` collection, silently wiping `SelectedOptions` in multi mode. User saw filter input keystrokes erase their tag selections.
- **AbstractSelect popup strong-sync deadlock.** Adding `IsPlayingCloseMotion` cancel + `IgnorePropertyChange = true; SetCurrentValue(IsDropDownOpenProperty, false)` made all popup controls fail to reopen reliably.
- **InfoPickerInput popup arrow flip lost.** Replacing `IsHorizontalFlipped="{Binding $parent[atom:Popup].IsHorizontalFlipped}"` with a C# `SetCurrentValue` chain broke the arrow-tracks-active-input behavior in `IsShowTime` mode.
- **SelectCandidateList filter selection.** Using `GlobalIndexFromContainer` instead of `IndexFromContainer + Items[index]` caused clicks under filter to select the wrong item.
- **ListBox OnLoaded filter re-apply lost.** Buried in a perf commit that also added a static default filter; revert dropped the OnLoaded re-apply, breaking AutoComplete/Mentions popup re-open with stale filter state.

The rollback ultimately squashed 50+ commits into a single revert and restored the OLD axaml-based architecture. **Lesson:** Pattern A's complexity cost (manual SetCurrentValue chains, disposable bookkeeping, race-prone state machines) consistently outweighed its allocation savings. This is the original justification for the [Theme Static Rule](#theme-static-rule).

### What to check next time

When evaluating a "looks like Pattern A" perf proposal, immediately ask:

1. Can `IsVisible="False"` + style selector achieve the same conditional-cost outcome?
2. Is the saved cost ≥ 1 KB / instance, or only 200B?
3. Does the saved cost recur per open/close (worth optimizing) or only once at startup (not worth)?
4. Will any TemplateBinding chain need to be replaced with a manual C# property change handler? If yes, count those handlers — each one is a future regression source.

If any of (1) is yes, (2) is "only 200B", (3) is "only once", or (4) is "yes, multiple handlers" — the optimization should not be Pattern A.
