---
name: atomui-resource-lifecycle
description: Use when changing AtomUI or Avalonia resource bindings, DynamicResource, TokenResourceBinder, non-Visual AvaloniaObject lifecycle, IResourceHost/IThemeVariantHost, owner/container attach cleanup, or investigating memory leaks and retained controls.
---

# AtomUI Resource Lifecycle

## Overview

Resource lifecycle correctness is a hard boundary, not a performance optimization. Any object that subscribes, binds, attaches, caches, or enters an owner/container lifecycle must define the release event before the acquire call is accepted.

The Gallery rule is strict: after navigation settles, old ShowCases must not remain alive unless a feature explicitly owns a cache and documents its eviction policy.

## DynamicResource Scoped Host Rule

> A non-Visual `AvaloniaObject` must not carry `DynamicResource` or token-resource bindings unless it owns or is attached to a scoped `IResourceHost` with a verified release path.

Violations block merge because they can root an entire Gallery ShowCase through `Application.ResourcesChanged`.

### Why This Leaks

Avalonia 12 `DynamicResourceExpression` chooses its resource host in this order:

1. If the binding target implements `IResourceHost`, use the target.
2. Otherwise use the XAML anchor / provider owner.
3. Subscribe to the selected host's `ResourcesChanged`, and to `ActualThemeVariantChanged` when the host is an `IThemeVariantHost`.

Source references:

- `.referenceprojects/Avalonia/src/Markup/Avalonia.Markup.Xaml/Data/DynamicResourceExpression.cs:37-61`
- `.referenceprojects/Avalonia/src/Markup/Avalonia.Markup.Xaml/Data/DynamicResourceExpression.cs:113-141`
- `.referenceprojects/Avalonia/src/Markup/Avalonia.Markup.Xaml/Data/DynamicResourceExpression.cs:145-153`

If the target is a non-Visual `AvaloniaObject` and does not implement `IResourceHost`, the host can fall back to `Application`. The leak chain then becomes:

```text
Application.ResourcesChanged
  DynamicResourceExpression
    ValueStore
      non-Visual AvaloniaObject
        owner/header/container
          Gallery ShowCase visual tree
```

This exact pattern leaked `DataGridColumn`, `DataGridColumnGroupItem`, and `NavMenuNode`. Full case study: `docs/engineering/avalonia-dynamic-resource-memory-leak-case-study.md`.

## Required Patterns

Owner-based objects, such as `DataGridColumn`:

- Implement `IResourceHost` and `IThemeVariantHost` on the non-Visual object.
- `TryGetResource(...)` queries the scoped owner first, then falls back to `Application.Current`.
- Owner changes unsubscribe the old owner before assigning/subscribing the new owner.
- Owner `ResourcesChanged` and `ActualThemeVariantChanged` are forwarded through the object.
- Owner attach/detach raises `ResourcesChanged` so dynamic resources republish.

Container-driven data nodes, such as `NavMenuNode`:

- Do not store a permanent last-container reference.
- Attach through an API that returns `IDisposable`.
- Store the disposable in the same owner/container `CompositeDisposable` as the node bindings.
- Release through `ClearContainerForItemOverride`, detach, re-template, unregister, or owner disposal.
- If multiple temporary attachments are possible, use attachment counting or an equivalent scoped token.

Global token bindings:

- Do not create `TokenResourceBinder.CreateGlobalTokenBinding(...)` in constructors by default.
- Create global bindings on first real use, such as first open/show.
- Dispose the returned binding on close, unregister, detach, owner disposal, or the matching clear path.

## Forbidden Patterns

- `target[!SomeProperty] = new DynamicResource...` on a non-Visual `AvaloniaObject` that is not a scoped `IResourceHost`.
- Constructor-time `TokenResourceBinder.CreateGlobalTokenBinding(...)` with no explicit release path.
- Relying on Gallery route reset, DataContext clearing, or visual cleanup to break a global resource subscription.
- Replacing dynamic resources with static values only to avoid a leak. That breaks theme/token behavior instead of fixing lifecycle.
- Permanent owner/container references used only to keep resource lookup working.

## Required Tests

Every fix or new non-Visual dynamic-resource target needs lifecycle and resource behavior coverage:

- WeakReference test: dynamic resource does not root an otherwise unreferenced target after GC.
- Owner resource test: target uses scoped owner resources before falling back to `Application`.
- Resource update test: owner/application resource updates still republish values.
- Gallery object-count test when applicable: after random navigation, only the current ShowCase remains alive.

The WeakReference test must simulate the XAML `DynamicResourceExtension` anchor shape. Hand-setting a property or using only static values does not reproduce the Application-root leak.

## Review Trigger

Before approving changes in this area, scan for:

```bash
rg "DynamicResource|CreateGlobalTokenBinding|IResourceHost|IThemeVariantHost" --type cs src/ controlgallery/
rg "class .*: AvaloniaObject" --type cs src/
rg "ResourcesChanged \\+|ActualThemeVariantChanged \\+|CreateGlobalTokenBinding" --type cs src/ controlgallery/
```

Any non-Visual `AvaloniaObject` with dynamic resource/token binding and no scoped host lifecycle is a blocker.

## Memory Validation

Use object counts, not RSS alone:

- Current `ShowCase` count should settle to 1 after navigation.
- Old `ShowCaseItem`, `DataGridColumn`, `NavMenuNode`, and `DynamicResourceExpression` counts must not grow monotonically across random navigation.
- RSS can have load peaks; persistent old object graphs are the leak signal.
