# AtomUI Deep Control Optimization

Use this reference when a control is hard to maintain, has repeated edge-case bugs, or needs architecture-level cleanup from the root rather than another trigger-point patch.

## Principle

Deep optimization is not permission to rewrite. It must preserve API, behavior, rendered result, performance, and lifecycle safety unless the user explicitly authorizes a scoped change.

Root-cause repair is mandatory. Trigger-point patches that add flag variables or suppress paths without fixing ownership, ordering, or lifecycle are blockers because they increase complexity and make later bugs harder to reason about.

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
```

Do not start from a preferred refactor. Start from the failing state, the hard-to-maintain responsibility boundary, or the repeated class of bug.

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

Only check relevant rows, but record what was intentionally out of scope.

## Architecture Root Fix Rules

- Fix the owner of state, not only the event that exposed stale state.
- Prefer existing AtomUI primitives, interfaces, helpers, token patterns, and comparable control architecture.
- Introduce a new abstraction only when it removes real duplication, clarifies ownership, or prevents a repeated class of bugs.
- Do not use suppression flags such as `_ignoreXxx`, `_isUpdating`, `_suppressChange`, `_isInternalChange`, or `IgnorePropertyChange` to patch over broken event flow.
- A flag-like state is acceptable only when it models a real deterministic state machine, has one clear owner, has guaranteed entry and exit paths, and is covered by tests for normal, cancel, async, detach, and re-template paths when relevant.
- Do not add timers, dispatcher delays, forced refreshes, or catch-and-ignore blocks to mask ordering bugs.
- If the proposed fix says "skip this handler once", "ignore the next change", "delay until later", or "force refresh", stop and trace the state ownership/order problem first.
- Do not merge unrelated cleanup into an architecture fix. Keep mechanical layout changes, behavior fixes, and architecture changes reviewable.
- If the architecture fix requires API, behavior, or rendered result changes, stop and get explicit user approval first.

## Refactoring Shape

Prefer this sequence:

1. Characterize current expected behavior and the failing edge case.
2. Add or update focused regression tests where feasible.
3. Map state ownership and lifecycle acquire/release pairs.
4. Move code toward clearer responsibilities without changing contracts.
5. Remove obsolete code only after the replacement behavior is covered.
6. Re-run targeted tests, owning project tests, and `git diff --check`.

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
Edge cases covered:
Lifecycle/performance/leak risk:
Tests:
Manual/Gallery verification:
Residual risk:
```
