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

For broad optimization requests, routing is a blocking gate. Requests phrased as "重新优化", "整体优化", "代码比较乱", "按照控件优化 skill 优化", "评估整体正确性", or similar must not go directly to code edits. First perform an optimization audit and report findings, phase plan, contract impact, and residual risks. Only skip the audit when the user names one narrow bug or one narrow mechanical edit.

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

## Mandatory Audit Gate

Before editing a control for broad optimization, write a short audit in the working notes or user-facing plan. The audit must answer:

```text
Optimization request type: broad / narrow
Control:
Primary responsibilities:
State owners:
Lifecycle acquire/release pairs:
Data/selection/key flows:
Template parts and generated containers:
Existing tests / missing regression:
Dead or duplicate implementation scan:
Potential API/behavior/render changes:
Recommended phases:
What will not be changed in this pass:
```

If the audit finds no issues beyond member order, say that explicitly. If it finds behavior risks, do not hide them behind "cleanup"; classify them as edge-case correctness or architecture root fix and add tests before changing behavior.

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
- Do not report broad control optimization as complete after only member reordering. If deeper risks are found but not fixed in the current pass, report them as residual risks or propose a phased follow-up.

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

## Escalation Signals

Seeing any of these in a control during broad optimization forces a deep-control audit before editing:

- `_ignoreXxx`, `_suppressXxx`, backup fields, delayed refreshes, or forced sync paths around state changes
- more than one owner for the same state, such as `SelectedItems`, `SelectedKeys`, checked nodes, current item, filter text, popup state, or validation state
- two-way sync between styled properties, template parts, collection views, and domain objects
- public interfaces with no-op/default implementations that hide capability differences
- runtime-created controls, popups, flyouts, dynamic menu items, or C# bindings
- generated/recycled containers carrying domain state, especially tree/list nodes, selection, masked/disabled state, or checked state
- custom pagination, collection view movement, filtering, source replacement, or key translation
- repeated algorithms across base/subclass implementations
- unreferenced files/classes, unused helpers, or copied selection models
- missing tests for property changes before template apply, after template reapply, after detach, collection reset, and nested/tree data

When an escalation signal appears, list it in the audit even if the current pass will only fix a subset.

## Execution Flow

1. Inspect the target control and 2-3 comparable controls in the same package.
2. For broad optimization, perform the mandatory audit gate before editing.
3. Inventory the exposed contract before deciding the edit shape.
4. Classify API risk, file split need, and optimization mode.
5. Present findings and phases when behavior, render, architecture, or broad cleanup risks exist.
6. Make the smallest scoped change that matches existing control style.
7. Run targeted tests first, then broaden based on risk.
8. Always run `git diff --check` before reporting completion.

## Contract Inventory

For any non-trivial control optimization, check:

- public/protected members and constructors
- `StyledProperty`, `DirectProperty`, `RoutedEvent`, commands, .NET events
- implemented public interfaces
- template parts, pseudo-classes, theme selectors, token/resource keys
- Gallery examples and docs that expose usage
- C# relay bindings: for each `BindUtils.RelayBind`, record source, target, why AXAML cannot express it, binding mode/priority, and disposal owner
- bindings, subscriptions, timers, popup hosts, cached views, lazy-created objects

## Required Scans

For broad optimization, run focused searches before deciding the implementation shape:

- state suppression: `_ignore`, `_suppress`, `_isUpdating`, `Backup`, `Reset`, `Refresh`, `Dispatcher`
- dynamic lifecycle: `+=`, `-=`, `IDisposable`, `CompositeDisposable`, `RelayBind`, `DynamicResource`
- data flow: `ItemsSource`, `Selected`, `Checked`, `Current`, `TargetKeys`, `SelectedKeys`, `Filter`, `Page`
- dead/duplicate code: class references, copied methods, override methods matching the base implementation

Do not treat these scans as proof by themselves. Use them to identify ownership and edge-case paths that must be reviewed.

## Completion Report

Report the result in terms of contract safety and verification:

```text
API/theme contract changed: No / Yes
Behavior changed: No / Yes
Files split: No / Yes
Audit performed: No / Yes
Residual risks:
Tests:
Diff hygiene:
Commit created: No unless explicitly requested
```
