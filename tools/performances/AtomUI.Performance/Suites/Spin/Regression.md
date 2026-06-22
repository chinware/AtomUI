# Spin Regression Matrix

## Functional Matrix

- [x] Built-in `SpinIndicator` materializes template-owned dot visuals.
- [x] Built-in `SpinIndicator` does not use UI-thread styled-property animation state.
- [x] `SpinIndicator` keeps built-in dots visible when only `CustomIndicatorTemplate` is set.
- [x] Custom `SpinIndicator` shows the custom presenter and hides built-in dots.
- [x] Custom `SpinIndicator` size follows `SizeType` changes.
- [x] `Spin` shows and hides the static indicator when `IsSpinning` changes.
- [x] `Spin` keeps tip content available and syncs tip visibility.

## Lifecycle Matrix

- [x] Invisible indicator attaches without creating UI-thread animation state.
- [x] Visible indicator starts without `_animation`, `_animationStyle`, or `_cancellationTokenSource`.
- [x] `MotionDuration` changes do not recreate UI-thread animation state.
- [x] Hidden indicator stops without leaving UI-thread animation state.
- [x] Detached custom indicator releases template-part references.

## Manual Gallery Matrix

- [ ] `SpinShowCase`: basic built-in indicator animation is visually smooth.
- [ ] `SpinShowCase`: small, middle, and large indicators keep expected visual sizes.
- [ ] `SpinShowCase`: custom LoadingOutlined indicators rotate smoothly.
- [ ] `SpinShowCase`: embedded `Spin` overlay/tip behavior remains unchanged.
