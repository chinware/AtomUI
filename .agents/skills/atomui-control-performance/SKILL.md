---
name: atomui-control-performance
description: Use when optimizing AtomUI controls, investigating control performance regressions, changing Avalonia styled-property bindings, lazy creation, templates, selectors, or Gallery performance scenarios. Applies to controls such as Space, Button, Icon, AddOnDecoratedBox, LineEdit, and shared primitives.
---

# AtomUI Control Performance Skill

## Core Rules

- Correctness bugs outrank performance work. If a performance optimization changes behavior, fix or revert that behavior before continuing.
- Performance optimizations must preserve control functionality, UI appearance, animation behavior, interaction semantics, theme behavior, and public API by default. Any visible or behavioral change caused by an optimization is a correctness regression, not an acceptable tradeoff, unless the user explicitly approves that change.
- Hard boundary: performance optimization must not introduce logic bugs. This includes invalid visual states, broken state transitions, incorrect property precedence, stale layout/render state, lost interactions, incorrect lifecycle cleanup, or Gallery-visible behavior mismatches. If any logic bug is discovered, stop performance work immediately and fix or revert the performance-only change before reporting optimization success.
- Follow the principle: unused features must not pay runtime cost.
- Do not create git commits during performance optimization work unless the user explicitly asks for a commit in the current request. Leave completed changes in the working tree for review by default.
- Hard boundary: an optimization that causes a repeatable performance regression is not acceptable. If any primary metric for the targeted control or its real Gallery ShowCase gets worse under the same measurement policy, the change must be fixed, split, reverted, or explicitly reported as a blocker before the optimization can be considered complete.
- Hard boundary: no performance optimization may introduce resource leaks. If an optimization creates, subscribes, binds, caches, lazily materializes, or reparents anything, it must also define and verify the matching release path before the work is considered complete.
- Hard boundary: when a binding source and target have the same owner and lifetime, prefer Avalonia `[!]` binding syntax over `BindUtils.RelayBind(...)` plus `CompositeDisposable` or manual disposal plumbing. Do not introduce disposable binding infrastructure for lifetime-consistent child visuals, presenters, adorners, or template-owned objects unless there is a real lifecycle mismatch, replacement path, or detach/re-template requirement.
- Hard boundary: no performance optimization may make the implementation logic materially more complex. If the measured win requires fragile state machines, duplicated template/style logic in code, broad lifecycle orchestration, unclear synchronization, or code that is harder to reason about than the original behavior warrants, stop and propose a simpler option instead of implementing it.
- Optimization attempt budget: for the same scoped target, if three implementation-and-measurement rounds fail to improve the primary speed metric under the same measurement policy, stop further speed optimization. At the end of optimization, if the measured result shows no worthwhile performance gain, restore every code change made only for performance, delete intermediate scratch code and documents, and keep only correctness fixes, leak fixes, and useful measurement tooling. Report exactly what was reverted, what correctness fixes were kept, what cleanup was done, and why no further performance optimization is recommended unless the user explicitly approves another round with new evidence.
- When scanning for performance bottlenecks, also scan for resource leaks in the same code path. Any discovered leak outranks performance-only work and must be fixed first or explicitly documented as a blocker if it cannot be fixed in the current scope. Do not proceed with an optimization that leaves a known leak in the touched lifecycle path.
- Prefer no API change. AtomUI has no formal release yet, so API changes are allowed only when required and explicitly justified.
- Every optimization that creates, removes, subscribes, binds, or lazily materializes objects must have a cleanup path and a regression verification.
- Gallery scenarios must be tested with the real Gallery example shape when the user is discussing Gallery-visible behavior. Synthetic control-only tests are not enough.
- Do not write `Debug.Assert(value != null)` immediately followed by a nullable guard for the same value. Express the invariant in the type or helper return value, or choose a real runtime guard with an explicit recovery path.
- Do not leave unused `using` directives after optimization work. Any newly introduced unused imports must be removed before the change is considered complete.
- Reuse existing shared utilities before adding local helpers or constants. For floating-point comparison, epsilon checks, zero/one checks, ordering with tolerance, angle conversion, and fixed-point rounding in AtomUI code, use `AtomUI.Utils.MathUtils` (`AreClose`, `IsZero`, `GreaterThan`, `LessThanOrClose`, etc.) instead of hand-written `Math.Abs(...) < eps`, new epsilon constants, or duplicated helper methods.
- Prefer method-group dispatcher callbacks for transition restoration. Write `Dispatcher.Post(this.EnableTransitions);` instead of `Dispatcher.Post(() => this.EnableTransitions());`.
- When moving template-created visuals into code for lazy creation, migrate the exact theme/style/selector behavior to a stable place such as the child control's own theme or explicit synchronized properties. Add verification for visual defaults and state changes such as hover, selected, disabled, loading, progress, and animation-enabled states.

## Popup Lazy Content Rule

Many AtomUI controls use `Popup` in `ControlTheme`. Keep the distinction between the lightweight popup shell and heavy popup content explicit.

- A lightweight `Popup` shell may stay in `ControlTheme` when it preserves placement, light-dismiss, overlay, theme styling, or required template contracts.
- Heavy popup content must not be created for the default closed state. Examples include candidate lists, `TreeView`, `CascaderView`, calendar/time panels, complex item presenters, filter lists, empty indicators, and large popup layout trees.
- Prefer first-open materialization: create heavy popup content immediately before the first open, wire events and bindings there, and sync pending selection/filter/items state after creation.
- By default, keep materialized popup content after close to avoid open/close churn. Release it on re-template, detach, or explicit disposal paths.
- Closed controls must not pay for popup-only event subscriptions, item source copies, filter setup, selection synchronization, or heavy visual tree creation.
- All lazy popup content must have lifecycle verification covering first open, close, second open, re-template, detach, visual parent cleanup, event unsubscribe, binding disposal, and state toggles such as loading/filter/selection.

## Avalonia Binding Priority Guardrails

This skill must prevent the Space `ItemSpacing`/`LineSpacing` bug from recurring.

### Incident Summary

The Space optimization changed internal token spacing bindings without fully modeling the Gallery example:

```xml
<atom:Space SizeType="{Binding SizeType}"
            LineSpacing="{Binding #CustomSizeSlider.Value, Priority=Template}"
            ItemSpacing="{Binding #CustomSizeSlider.Value, Priority=Template}" />
```

The first attempted fix moved Space internal token bindings to `BindingPriority.Style`. That avoided same-priority disposal collisions with the external `Template` binding, but introduced a new bug: the always-present slider `Template` binding won in `Small`, `Middle`, and `Large`, so those SizeType options no longer changed spacing. `Custom` still worked, which hid the bug unless the complete interaction was tested.

### Avoidance Rules

- Do not use `IsSet(property)` as a proxy for "user explicitly set this property." In Avalonia, `IsSet` returns true for any effective styled value or binding, including Template/Style values, and can block required internal default restoration.
- Do not put internal default/token bindings at the same `BindingPriority` as expected external bindings if the internal binding will be disposed/recreated. Avalonia immediate value frames are priority-based and property-keyed; disposing an entry at the same priority can remove or disturb another binding for the same property.
- For internal defaults that must override template-provided optional values in normal modes but yield in a special mode, use a stronger separate priority and dispose it at the mode boundary. Example: Space uses internal token spacing at `StyleTrigger` for non-`Custom`, then disposes it in `Custom` so the external `Template` slider binding takes over.
- When a property has both internal default binding and external override binding, verify the whole state matrix: default mode, each named mode, special/custom mode, switching out of custom mode, and source updates after switching.
- LocalValue and Animation must remain stronger than internal defaults. If a user sets a local value, internal token binding must be disposed or not observed as the effective value.

## Required Verification Pattern

For any control performance optimization:

1. Build the changed project or the smallest owning project.
2. Add or update a state/lifecycle verification under `tools/performances/AtomUI.Performance/Suites/<Control>/` for behavior that could regress.
3. Include cleanup assertions when visuals, presenters, hosts, subscriptions, or bindings are lazily created.
4. If the bug is Gallery-visible, build `controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj` and verify the exact Gallery scenario.
5. For every control optimization with a corresponding Gallery ShowCase, run or update the real ShowCase loading-time measurement and report the before/after comparison.
6. Run `git diff --check`.

## Performance Report Format

Performance summaries must be readable to a human reviewer, not just raw benchmark output.

- Report results as a compact comparison table with `Scenario`, `Before`, `After`, and `Improvement`.
- Include the units in every value, such as `ms/item`, `KB/item`, `ms`, or visual node count.
- Show both control-level results and real Gallery results when both were measured.
- Always include the corresponding ShowCase loading-time optimization comparison for controls that have a Gallery ShowCase. At minimum list cold first navigation, repeated mean, repeated median, repeated P95, and the sample policy such as warmup/iterations.
- Cold first navigation comparisons must use multiple independent process samples, for example `--cold-iterations 10`. A single in-process cold sample is only a smoke check and must not be used to claim improvement or regression.
- Use the same sample policy for before and after comparisons. Do not compare a short/noisy run against a longer or differently warmed run. If the first pass shows a regression, rerun with enough warmup/iterations to distinguish real regression from measurement noise before reporting it as the final result.
- For Gallery-visible optimizations, explain the actual user-facing impact in plain language, such as "ComboBoxShowCase repeated open went from 109ms to 91ms, about 18ms faster."
- Mention structural wins separately when they explain the result, such as "Button/IconButton count changed from 23 to 0" or "visual nodes dropped from 562 to 497."
- State whether the result matches the optimization goal, and call out fixed costs that remain when the page-level percentage is lower than the control-level percentage.

## Binding/Subscription Checklist

- Treat leak scanning as mandatory during performance analysis, not optional cleanup after optimization.
- Before using `BindUtils.RelayBind(...)`, storing an `IDisposable` binding, or adding a `CompositeDisposable`, first check whether Avalonia `[!]` binding is sufficient because the source and target lifetimes are identical.
- Use explicit disposable binding plumbing only for mismatched lifetimes, replacement paths, detach/re-template cleanup, global/window subscriptions, timers, event handlers, or lazily materialized objects that can outlive their creator.
- Store every disposable subscription or binding that is created outside XAML.
- Dispose old bindings before replacing them.
- Avoid creating duplicate bindings on repeated property changes.
- If a mode disables a feature, detach visuals and dispose subscriptions in that mode.
- For created visuals, presenters, popups, timers, event handlers, property observables, bindings, caches, and global/window subscriptions, identify the owner and the release trigger before editing.
- Add a regression test that toggles the feature on, off, and on again.
