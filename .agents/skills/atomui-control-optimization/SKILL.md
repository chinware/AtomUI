---
name: atomui-control-optimization
description: Use when optimizing, refactoring, reviewing, or fixing AtomUI controls, including control API contracts, member layout, file splitting, lifecycle, AXAML structure, correctness, performance, Gallery-visible behavior, or documentation impact.
---

# AtomUI Control Optimization

## Overview

This is the entry skill for AtomUI control work. Its job is to classify the change, lock down public contracts, route to narrower skills when needed, and keep control optimization from mixing unrelated API, behavior, lifecycle, performance, and layout changes.

## Required Reading

Before touching control code, read:

- `docs/engineering/control-development-guidelines.md`
- `docs/engineering/agent-guidelines.md`

For API contract optimization, member layout, method reordering, or file splitting, also read:

- `references/api-layout-and-splitting.md`

For messy implementation, repeated edge-case bugs, unclear state ownership, or architecture-level control redesign, also read:

- `references/deep-control-optimization.md`

## Route First

Classify the request before editing.

| If the work involves | Required handling |
| --- | --- |
| API contracts, member order, method layout, partial split | Read `references/api-layout-and-splitting.md` |
| Messy implementation, boundary-condition bugs, tangled state, architecture root optimization | Read `references/deep-control-optimization.md` |
| DynamicResource, token resource binding, subscriptions, owner/container lifecycle | Also use `atomui-resource-lifecycle` |
| Performance claims, lazy visuals, binding cost, template optimization | Also use `atomui-control-performance` |
| `BindUtils.RelayBind`, C#-created bindings, or template-owned binding relationships | Prove AXAML cannot express the binding before keeping C# binding; define disposal owner |
| Bug fix or behavior change | Reproduce/trace root cause and add or update regression tests |
| Gallery-visible examples or docs | Check Gallery/docs impact and update only when required |
| AOT-sensitive binding, reflection, dynamic registration | Check `docs/engineering/aot-programming-guidelines.md` |

## Hard Boundaries

- Without explicit user approval, do not change control API, behavior, or rendered result. This is a blocking constraint, not a preference.
- Control API includes public/protected members, Avalonia properties/events, template parts, token names, resource keys, pseudo-classes, and documented or Gallery-exposed usage.
- Behavior includes default values, binding priority, event timing, state transitions, focus/keyboard/pointer interaction, popup lifecycle, validation, async/data loading, and observable side effects.
- Rendered result includes visual tree semantics, layout size, spacing, alignment, colors, typography, animations, theme response, pseudo-class visuals, and template selector behavior.
- Control optimization must not introduce logic bugs, performance regressions, or resource leaks. If a proposed optimization cannot preserve correctness, performance, and lifecycle safety, stop and redesign before editing.
- Fix root causes instead of patching trigger points. Do not use flag variables, delayed refreshes, forced sync, or suppression paths to hide broken state flow; patch-style fixes make controls harder to maintain and are a blocker unless explicitly justified as a deterministic state machine.
- Do not mix pure member reordering with behavior fixes. If a bug is found during reordering, split it into a behavior fix with tests.
- Do not split files just because a file looks long. Under about `2000` lines, prefer method order, regions, and private helper extraction.
- Do not treat the `2000` line threshold as an automatic split rule. Even after the threshold is crossed, split only when the control has real, stable responsibility boundaries.
- Do not scatter a control into many small partial files. Public API contracts must remain in the main control file.
- Do not move lifecycle entry points out of the main control file unless the control is already deliberately partial and the entry remains easy to find.
- Do not introduce performance-oriented dynamic C# visual creation unless `atomui-control-performance` allows it.
- AXAML-first binding is a hard boundary. For template-owned visual state, template part state, style state, and fixed control-to-template relationships, use AXAML binding, `TemplateBinding`, style selectors, or existing theme/resource mechanisms before considering C# binding.
- `BindUtils.RelayBind` is the last binding creation mechanism, not the default convenience API. Before adding or keeping it, prove why the same relationship cannot be expressed in AXAML without changing API, behavior, rendered result, binding mode, binding priority, or template contract.
- Allowed `BindUtils.RelayBind` cases are limited to relationships AXAML cannot naturally express, such as dynamically created runtime targets, sibling/template-part coordination that cannot be represented by `TemplateBinding` or selectors, runtime-selected source objects, or bindings whose lifecycle is owned by a non-template runtime object.
- Every accepted `BindUtils.RelayBind` must have an explicit release path matching its acquisition path, such as re-template cleanup, detach, owner disposal, popup/content clear, or container recycle. Do not create relay bindings without a matching dispose owner.
- Do not use `BindUtils.RelayBind` to work around inconvenient AXAML. If the binding source and target are both stable parts of the control template, move the relationship into AXAML unless doing so would require an approved contract or behavior change.

## Intake Checklist

Before implementation, write down the current scope:

```text
Control:
Task category: API / layout / split / correctness / lifecycle / AXAML / performance / Gallery / docs
Optimization mode: cleanup / edge-case correctness / architecture root fix
Current LOC:
Comparable controls inspected:
Contract change needed: No / L1 / L2 / L3
File split decision: No / Yes, reason:
Other required skills:
Behavior changes mixed into layout-only work: No
C# relay binding inventory: None / Reviewed, exceptions documented
Verification commands:
```

## Execution Flow

1. Inspect the target control and 2-3 comparable controls in the same package.
2. Inventory the exposed contract before deciding the edit shape.
3. Classify API risk, file split need, and optimization mode.
4. Make the smallest scoped change that matches existing control style.
5. Run targeted tests first, then broaden based on risk.
6. Always run `git diff --check` before reporting completion.

## Contract Inventory

For any non-trivial control optimization, check:

- public/protected members and constructors
- `StyledProperty`, `DirectProperty`, `RoutedEvent`, commands, .NET events
- implemented public interfaces
- template parts, pseudo-classes, theme selectors, token/resource keys
- Gallery examples and docs that expose usage
- C# relay bindings: for each `BindUtils.RelayBind`, record source, target, why AXAML cannot express it, binding mode/priority, and disposal owner
- bindings, subscriptions, timers, popup hosts, cached views, lazy-created objects

## Completion Report

Report the result in terms of contract safety and verification:

```text
API/theme contract changed: No / Yes
Behavior changed: No / Yes
Files split: No / Yes
Tests:
Diff hygiene:
Commit created: No unless explicitly requested
```
