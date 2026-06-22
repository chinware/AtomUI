# AtomUI Deep Control Optimization

Use this reference when a control is hard to maintain, has repeated edge-case bugs, or needs architecture-level cleanup from the root rather than another trigger-point patch.

## Principle

Deep optimization is not permission to rewrite. It must preserve API, behavior, rendered result, performance, and lifecycle safety unless the user explicitly authorizes a scoped change.

Root-cause repair is mandatory. Trigger-point patches that add flag variables or suppress paths without fixing ownership, ordering, or lifecycle are blockers because they increase complexity and make later bugs harder to reason about.

Root-cause repair also requires artifact discipline. A fix that introduces unused classes, duplicate helpers, speculative abstractions, boolean-switch methods, or marker variables is not complete merely because the visible bug disappears. If the artifact does not model a real stable responsibility, remove it in the same pass.

There are three modes:

| Mode | Use when | Required gate |
| --- | --- | --- |
| Cleanup | Code is hard to read, duplicated, poorly ordered, or locally tangled but behavior is correct | No behavior/render/API change |
| Edge-case correctness | Boundary conditions, state transitions, collection changes, async races, popup/focus flows, or lifecycle paths are wrong | Reproduce or characterize bug, then add regression coverage |
| Architecture root fix | State ownership, data flow, lifecycle model, or control responsibilities are structurally wrong | Present root-cause design and compatibility plan before implementation |

## Investigation Checklist

Before editing, identify:

```text
User-visible symptom:
Current bad state source:
Shared root cause or trigger-only issue:
Comparable controls inspected:
Existing tests / missing regression:
State owners:
Template parts involved:
Subscriptions / bindings / timers:
Popup / async / collection paths:
API, behavior, render contract impact:
New classes/helpers/fields/flags needed and why:
```

Do not start from a preferred refactor. Start from the failing state, the hard-to-maintain responsibility boundary, or the repeated class of bug.

For broad optimization requests, the investigation checklist is mandatory output before editing. A broad request includes "整体优化", "重新优化", "代码乱", "按控件优化 skill 优化", or any request that does not name one specific bug or one specific mechanical change. If the investigation exposes potential behavior bugs, report them as findings and either add regression tests before fixing them or leave them as explicit residual risks.

## Mandatory Deep Audit Areas

For complex controls, inspect these areas before deciding the implementation shape:

| Area | What to find | Failure signs |
| --- | --- | --- |
| State ownership | Which object owns selected value, selected keys, checked items, current item, filter text, popup state, validation state, and visual state | two-way state loops, `_ignoreXxx`, stale local values, duplicated backing collections |
| Data/key flow | How `ItemsSource`, keys, display items, filters, target/source collections, and collection views are transformed | top-level-only traversal, lost nested items, stale filtered results, mismatched key defaults |
| Template lifecycle | What is acquired in `OnApplyTemplate`, content presenter changes, generated containers, popup open, attach, and detach | missing unsubscribe, container state not prepared, relay binding without owner |
| Collection changes | Add/remove/replace/move/reset, empty source, source replacement, filtered source replacement | selection not pruned, item counts stale, page index invalid |
| Interface capabilities | Public or internal interfaces and their no-op/default implementations | hidden unsupported features, behavior split between interface and concrete type |
| Repeated implementation | Base/subclass overrides and copied helpers | same algorithm in multiple places, subclass override identical to base |
| Dead code | Files/classes/helpers with no references from source, AXAML, tests, generator, reflection, or AOT registration paths | unused selection model, stale helper, untested private pipeline |
| Patch artifacts | New classes, helpers, boolean parameters, marker fields, local marker variables, fallback branches, and copied motion/state helpers | duplicates an existing primitive, exists only for the current patch, hides ordering/state ownership, or has no durable owner |
| Render/lifecycle risk | Size type, custom size, status, disabled/read-only, theme switch, token update, language/resource update | visual state owned only by C# trigger, stale theme resource, inconsistent size sync |

If any area is intentionally out of scope, say so in the plan or completion report.

## Escalation From Cleanup

Do not continue treating work as pure cleanup when any of these are found:

- a property change handler is suppressing another handler with `_ignoreXxx`, `_suppressXxx`, or backup fields
- a generated/recycled container stores domain state that is also stored on the item or control
- a base class and subclass both calculate the same domain source, key list, count, filter, or layout state
- a new private class/helper duplicates an existing AtomUI primitive or comparable-control implementation
- a new flag, marker variable, or boolean-switch parameter exists only to steer around broken ordering or a partial fix
- an interface method is implemented as empty/default in a concrete control that is used through the interface
- a test would need nested data, template reapply, detach, collection reset, or source replacement to prove safety
- a runtime-created target uses C# binding or event subscription

When escalation happens, stop mechanical reordering and either:

1. Split the work into a behavior fix with tests.
2. Report the finding as residual risk if the current pass is intentionally layout-only.

## Edge-Case Matrix

For controls with non-trivial state, check the relevant cases before claiming an optimization is safe:

- null, empty, default, and unset values
- property changes before template apply, after template reapply, and after detach
- local value vs style/template/token binding priority
- disabled, read-only, invisible, unloaded, and ancestor invisible states
- focus, keyboard, pointer, capture, and cancellation paths
- popup open, close, cancel close, light-dismiss, window deactivation, duplicate opened/closed events
- async load start, success, failure, timeout, cancellation, stale result, detach during load
- collection `Add`, `Remove`, `Replace`, `Move`, `Reset`, empty result, filtered result
- selection/current item preservation after filtering or source replacement
- size type, custom size, placeholder, clear button, validation feedback, status, add-ons
- theme switch, token update, pseudo-class change, and language/resource update
- generated or recycled item containers before/after realization, especially tree/list/checkable containers
- nested data traversal for tree, grouped, hierarchical, or virtualized controls
- pagination after transfer/remove/filter/source replacement and page index clamping

Only check relevant rows, but record what was intentionally out of scope.

## Architecture Root Fix Rules

- Fix the owner of state, not only the event that exposed stale state.
- Prefer existing AtomUI primitives, interfaces, helpers, token patterns, and comparable control architecture.
- Introduce a new abstraction only when it removes real duplication, clarifies ownership, or prevents a repeated class of bugs.
- Do not add one-off private classes when an existing motion, layout, selection, lifecycle, or resource primitive already expresses the behavior.
- Do not add boolean-switch methods to hide two operations behind one helper. Split into explicit methods unless the boolean is an existing public contract.
- Do not keep local marker variables or fields just to coordinate a patch path. Use the real state owner, cancellation token, lifecycle owner, binding owner, or collection owner as the source of truth.
- If review discovers a useless class, duplicated helper, stale flag, or speculative abstraction introduced in the current fix, delete it immediately. Do not defer it as unrelated cleanup.
- Do not use suppression flags such as `_ignoreXxx`, `_isUpdating`, `_suppressChange`, `_isInternalChange`, or `IgnorePropertyChange` to patch over broken event flow.
- A flag-like state is acceptable only when it models a real deterministic state machine, has one clear owner, has guaranteed entry and exit paths, cannot be represented by an existing owner object, and is covered by tests for normal, cancel, async, detach, and re-template paths when relevant.
- Do not add timers, dispatcher delays, forced refreshes, or catch-and-ignore blocks to mask ordering bugs.
- If the proposed fix says "skip this handler once", "ignore the next change", "delay until later", or "force refresh", stop and trace the state ownership/order problem first.
- Do not merge unrelated cleanup into an architecture fix. Keep mechanical layout changes, behavior fixes, and architecture changes reviewable.
- If the architecture fix requires API, behavior, or rendered result changes, stop and get explicit user approval first.

## Refactoring Shape

Prefer this sequence:

1. Characterize current expected behavior, failing edge cases, and structural risks.
2. Add or update focused regression tests where behavior can change.
3. Map state ownership and lifecycle acquire/release pairs.
4. Reuse existing primitives before adding new private helpers or classes.
5. Remove duplicated algorithms, dead code, useless helpers, and patch artifacts after proving they are not contract paths.
6. Move code toward clearer responsibilities without changing contracts.
7. Re-run targeted tests, owning project tests, and `git diff --check`.

For large controls, split work into reviewable phases:

```text
Phase 1: tests and behavior characterization
Phase 2: local cleanup / method layout
Phase 3: root state/lifecycle/data-flow fix
Phase 4: remove dead code and tighten docs/examples if needed
```

## Stop Conditions

Stop and ask before continuing when:

- preserving API, behavior, or rendered result is impossible
- a change requires moving public contract out of the main control file
- the refactor starts touching unrelated controls or shared primitives
- the fix needs a new abstraction whose ownership is not obvious
- verification would require a Gallery/manual step that cannot be run in the current context
- a supposed cleanup changes event ordering, binding priority, layout size, rendered output, or lifecycle release timing

## Verification Report

For deep optimization, report:

```text
Mode: cleanup / edge-case correctness / architecture root fix
Root cause:
Contracts changed: No / Yes, approved by:
Behavior/render changed: No / Yes, approved by:
Audit findings:
Findings fixed:
Findings deferred:
New artifacts introduced and why:
Artifacts removed:
Edge cases covered:
Lifecycle/performance/leak risk:
Tests:
Manual/Gallery verification:
Residual risk:
```
