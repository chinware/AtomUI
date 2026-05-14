---
name: atomui-control-performance
description: Use when optimizing AtomUI controls, investigating control performance regressions, changing Avalonia styled-property bindings, lazy creation, templates, selectors, or Gallery performance scenarios. Applies to controls such as Space, Button, Icon, AddOnDecoratedBox, LineEdit, and shared primitives.
---

# AtomUI Control Performance Skill

## Core Rules

- Correctness bugs outrank performance work. If a performance optimization changes behavior, fix or revert that behavior before continuing.
- Follow the principle: unused features must not pay runtime cost.
- Prefer no API change. AtomUI has no formal release yet, so API changes are allowed only when required and explicitly justified.
- Every optimization that creates, removes, subscribes, binds, or lazily materializes objects must have a cleanup path and a regression verification.
- Gallery scenarios must be tested with the real Gallery example shape when the user is discussing Gallery-visible behavior. Synthetic control-only tests are not enough.

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
5. Run `git diff --check`.

## Binding/Subscription Checklist

- Store every disposable subscription or binding that is created outside XAML.
- Dispose old bindings before replacing them.
- Avoid creating duplicate bindings on repeated property changes.
- If a mode disables a feature, detach visuals and dispose subscriptions in that mode.
- Add a regression test that toggles the feature on, off, and on again.

