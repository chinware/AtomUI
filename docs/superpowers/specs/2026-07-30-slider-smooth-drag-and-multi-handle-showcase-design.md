# Slider Smooth Drag and Multi-handle Showcase Design

## Goal

Fix observable Slider thumb jumps at their source and align the Gallery multi-handle example with the Ant Design reference. Continuous dragging must preserve fractional `double` values. Tick snapping must occur only when the existing snapping contract is enabled.

## Scope

- Slider pointer dragging for single-value and multi-handle modes.
- Existing `IsSnapToTickEnabled` and `TickFrequency` behavior.
- The Slider Gallery multi-handle `ShowCaseItem` and its localized text.
- Focused regression tests and Gallery snapshot tests.

The change will not add or modify public API. Other Slider examples and the shared `ShowCaseItem` theme remain unchanged.

## Interaction Contract

### Continuous Mode

When `IsSnapToTickEnabled` is `false`, pointer position maps directly to a clamped `double` value. No integer rounding or tick-frequency quantization is applied. A one-pixel pointer movement must produce the corresponding fractional value movement when the value range is larger than the usable rail length.

Dragging a thumb from an off-center press point preserves the initial pointer-to-thumb value offset. Repeated pointer moves use the Slider track coordinate system so layout changes do not accumulate coordinate drift.

### Snapped Mode

When `IsSnapToTickEnabled` is `true`, the calculated continuous value is snapped to `TickFrequency`. Decimal tick frequencies remain supported without binary floating-point accumulation changing the intended tick value.

`TickFrequency` alone does not enable snapping. This preserves the existing Slider API contract and keeps continuous dragging as the default.

### Multi-handle Constraints

Each thumb keeps its handle index for the duration of a drag. A handle remains clamped between adjacent handles and the Slider bounds. Disabled handles and draggable-range behavior retain their existing contracts.

## Root-cause Verification

Before changing the pointer implementation, add regression coverage that drives the real pointer pipeline:

1. Drag a normal Slider thumb by one pixel with snapping disabled and verify a fractional value and approximately one-pixel visual movement.
2. Repeat for a multi-handle Slider thumb.
3. Enable snapping and verify values move only to `TickFrequency` boundaries.
4. Verify pressing away from the thumb center does not move the thumb before the pointer itself moves.

If the continuous tests already pass, no speculative control change will be made. In that case, the visible jump is attributed to the Gallery example's explicit `IsSnapToTickEnabled="True"` and `TickFrequency="5"` configuration. If a test fails, the fix will remain inside the pointer-to-value calculation and drag-session state without introducing public API.

## Gallery Design

Replace the current three-row multi-handle example with one horizontal Slider matching the Ant Design reference:

- Three handles initialized at `0`, `35`, and `100`.
- One active track spanning the full range.
- A horizontal green-to-yellow-to-red linear gradient using Ant Design semantic colors.
- Continuous dragging, with no tick snapping configured.
- No `v6.1.2` badge.
- Simplified Chinese title `多点组合` and description `范围多个点组合。`.
- Equivalent English and Traditional Chinese localized text.

The normal focus and hover visuals provide the active thumb outline. The example will not force a permanent focus state.

## Testing and Validation

- Add or update targeted Slider behavior tests for continuous and snapped dragging.
- Update Slider Gallery page assertions and example snapshot.
- Run the targeted Slider test set.
- Run the targeted AtomUIGallery test set.
- Build the Gallery Debug target.
- Launch the Gallery and visually inspect the multi-handle item and thumb dragging.
- Run `git diff --check`.

## Compatibility

The design is AOT-neutral: it adds no reflection, dynamic discovery, or runtime-generated binding paths. Existing Slider public properties and binding behavior remain intact.
