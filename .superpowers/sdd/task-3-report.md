# Task 3 Report: Make Token Conversion Deterministic

## RED Evidence

Command:

```text
dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~TokenValueConverterTests
```

Before the implementation change, the filtered suite reported 4 failures and 7 passes:

- `Double_Conversion_Uses_Invariant_Culture("fr-FR")` failed because `1.5` was parsed with the comma decimal separator.
- `Float_Conversion_Uses_Invariant_Culture("fr-FR")` failed for the same reason.
- `LoadConfig_Invalidates_Previously_Read_Token_Value` returned the cached `CornerRadius(6)` instead of `CornerRadius(12)`.
- `LoadConfig_Conversion_Error_Includes_Token_Context` did not include the token name in the exception chain.

## GREEN Evidence

The filtered suite passed 11/11 after the implementation change.

The full Core suite passed 47/47:

```text
dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore
```

`git diff --check` passed.

## Files

- `src/AtomUI.Core/Theme/TokenSystem/BuiltInTokenValueConverters.cs`
  - Uses invariant `NumberStyles.Integer` or `NumberStyles.Float` parsing for integer, double, and float tokens.
- `src/AtomUI.Core/Theme/TokenSystem/AbstractDesignToken.cs`
  - Removes each assigned token from the access cache before conversion/set.
  - Adds token name, raw value, and target type context at the conversion boundary while preserving the outer `ThemeLoadException`.
- `tests/AtomUI.Core.Tests/Theme/TokenValueConverterTests.cs`
  - Covers invariant conversion under en-US, fr-FR, and zh-CN, cache invalidation, and conversion error context.

## Self-Review

- The converter catch is limited to conversion; property assignment remains outside it and retains the existing assignment error path.
- No public API was added or changed.
- No unrelated converters or files were refactored.
- The generated converter registry remains unchanged.

## Concerns

No known concerns. The full `AtomUI.Core.Tests` suite is green, and the requested diff check is clean.
