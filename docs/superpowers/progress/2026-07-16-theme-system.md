# AtomUI Theme System Progress

This file tracks implementation progress for
[the theme system implementation plan](../plans/2026-07-16-theme-system-implementation.md).
The normative design remains in `docs/modules/core/theme-system.md` and
`docs/modules/core/theme-definition-xml.md`.

## Current State

| Item | Status | Evidence |
|---|---|---|
| Formal architecture | Complete | `docs/modules/core/theme-system.md` |
| XML v1 specification and XSD | Complete | XSD validated by `xmllint` and .NET `XmlSchemaSet` |
| Baseline Core tests | Complete | 141 passed, 0 failed on 2026-07-16 |
| Task 1: XML syntax and algorithm state | Complete | 15 focused tests; secure XSD reader implemented |
| Task 2: Schema registry and Binder | Complete | Deterministic registry and typed immutable Binder; 11 registry and 9 Binder tests |
| Task 3: Generated descriptors | Complete | 20 generator tests and 27 focused Core schema/Binder/parser tests; direct delegates cover all built-in value types |
| Task 3A: Source ownership and generated identity | Complete | Old directories removed; Theme root types use `AtomUI.Theme`; owner-specific generated pools compile across Gallery and Desktop; `BaseControlTheme` exposes only strongly typed TemplateParent bindings |
| Task 4: Config normalization and merge | Complete | 10 focused behavior tests; immutable typed capture, canonical formatter, stable fingerprint and deterministic change set implemented |
| Task 5: Dense snapshot compiler | Pending | Depends on normalized config and descriptors |
| Task 6: Bounded caches | Pending | Depends on final snapshots |
| Task 7: ThemeEngine transactions | Pending | Depends on compiler and caches |
| Task 8: Scope graph | Pending | Depends on ThemeEngine transaction contracts |
| Task 9: Resources and resolver | Pending | Depends on context and snapshot layout |
| Task 10: Manager/startup cutover | Pending | Depends on ThemeEngine, scopes and resources |
| Task 11: Legacy removal | Pending | Depends on all replacement paths |
| Task 12: Acceptance | Pending | Final cross-module validation |

## Verification Log

| Date | Command | Result |
|---|---|---|
| 2026-07-16 | `dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore` | 141 passed, 0 failed |
| 2026-07-16 | `xmllint --schema docs/modules/core/schemas/atomui-theme-v1.xsd docs/modules/core/examples/daybreak-blue.theme.xml` | Standard example valid |
| 2026-07-16 | .NET `XmlSchemaSet` validation harness | Standard example valid; 8 invalid structural cases rejected |
| 2026-07-16 | `dotnet test ... --filter FullyQualifiedName~ThemeDocumentReaderTests` | 15 passed, 0 failed |
| 2026-07-16 | `dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore` | 156 passed, 0 failed after Task 1 |
| 2026-07-16 | `dotnet test ... --filter FullyQualifiedName~ThemeSchemaRegistryTests` | 11 passed, 0 failed |
| 2026-07-16 | `dotnet test ... --filter FullyQualifiedName~ThemeDefinitionBinderTests` | 9 passed, 0 failed |
| 2026-07-16 | Full Core test with detailed hang diagnostics | One non-deterministic Avalonia Dispatcher host stall isolated to `ThemeConfigProviderTests`; focused and predecessor-combination reproductions passed |
| 2026-07-16 | `dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore` (run 1) | 176 passed, 0 failed |
| 2026-07-16 | `dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore` (run 2) | 176 passed, 0 failed |
| 2026-07-16 | `dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --framework net10.0 --no-restore` | 20 passed, 0 failed after owner-specific generated schema output |
| 2026-07-16 | Focused Core registry, Binder, generated descriptor and parser suites | 27 passed, 0 failed |
| 2026-07-16 | `dotnet test ... --filter FullyQualifiedName~ButtonThemeScopeTests` | 4 passed, 0 failed; generated FQN collision removed |
| 2026-07-16 | `dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj --framework net10.0 --no-restore` | Build succeeded, 0 warnings, 0 errors after directory reorganization |
| 2026-07-16 | `git diff --check` | Passed after Task 3/3A |
| 2026-07-16 | Focused Task 4 Normalizer and Merger suites | 10 passed, 0 failed |
| 2026-07-16 | Task 4 plus schema, Binder and generated descriptor suites | 35 passed, 0 failed |
| 2026-07-16 | Parser, formatter and Task 4 suites after performance review | 13 passed, 0 failed |
| 2026-07-16 | Full Core with `--blame-hang --blame-hang-timeout 30s` | 146 tests completed with 0 failures before the known `ThemeConfigProviderTests.Content_Replacement_Clears_Old_Scope_And_Publishes_New_Scope` host stall; hang dumps captured |
| 2026-07-16 | Isolated previously stalled ThemeConfigProvider test | 1 passed, 0 failed in 129 ms |
| 2026-07-16 | Final Generator verification after writer rename | 20 passed, 0 failed |
| 2026-07-16 | Final Gallery build after Task 4 and generated formatter rollout | Build succeeded, 0 warnings, 0 errors |
| 2026-07-16 | Final Desktop Button scope regression | 4 passed, 0 failed |
| 2026-07-16 | `dotnet test ... --filter FullyQualifiedName~BaseControlThemeTests` | 1 passed, 0 failed; string-path TemplateParent overloads are absent |
| 2026-07-16 | `dotnet test ... --filter FullyQualifiedName~ThemeCoordinatorTests` | 8 passed, 0 failed after obsolete Theme API removal |
| 2026-07-16 | Gallery build after obsolete Theme API removal | Build succeeded, 0 warnings, 0 errors |
| 2026-07-16 | Theme root namespace, coordinator, provider and snapshot cache suites | 32 passed, 0 failed |
| 2026-07-16 | Desktop Button scope regression after root namespace consolidation | 4 passed, 0 failed |
| 2026-07-16 | Gallery build after root namespace consolidation | Build succeeded, 0 warnings, 0 errors |

## Decisions Locked

- Central transaction type: `ThemeEngine`.
- Backend algorithm state: `ControlAlgorithmMode` enum.
- XML explicit values: `Disabled` and `Global`; custom mode uses `<Algorithms>`.
- Runtime truth source: immutable `ThemeSnapshot` only.
- Compatibility with the old theme API: intentionally not implemented.
- Unsupported old Theme APIs are deleted; the new system does not retain `[Obsolete]` throwing shells.
- Execution: inline only, no subagents.
- Theme source ownership: engine/context/scope runtime entries live in the `Theme` root; `Definitions` absorbs Catalog; `Algorithms` absorbs palette calculation; `Resources` absorbs Control theme loading; `Styling`, `Engine`, `ControlThemes` and `TokenSystem` directories are removed, with compile-only Token code living in `Tokens`.
- Theme root source files use `AtomUI.Theme`; only responsibility subdirectories introduce child namespaces.
- Generated schema ownership: `AtomUI.Generated.<OwnerAssemblyIdentifier>.GeneratedThemeSchema`; no shared generated full type name and no namespace nesting beneath a possibly same-named Control type.
- Token descriptor canonicalization: generated descriptors own direct parser and formatter delegates; normalized fingerprints never use raw input strings or process-random string hashes.
