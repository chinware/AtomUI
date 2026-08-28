---
name: version-release
description: Use when preparing or validating an AtomUI release, including version-scope confirmation, AtomUIVersion consistency, CHANGELOG and Chinese release sections, breaking-API docs under docs/releases, release validation, and the release commit. Use changelog-collect for ordinary changelog collection instead.
---

# Version Release

Use this skill when the user asks to release or prepare an AtomUI version, or review release readiness.

## Workflow

1. Confirm the intended version and release scope from the user request.
   If the target version or range is ambiguous, state the assumption and proceed with
   preparation; get explicit confirmation before tag, push, or publish.
2. Inspect repository state before changing files:
   - `git status --short`
   - `git branch --show-current`
   - `git tag --sort=-v:refname | head`
   - `git log --oneline --decorate <previous-tag>..HEAD` when a previous tag exists
3. Separate user-visible changes from internal changes, and identify breaking public API changes.

## Version consistency

- Keep `build/Versions.props` -> `AtomUIVersion` as the single source of truth.
- Packages normally inherit `$(AtomUIVersion)` through `build/PackageMetadata.props`.
  `AtomUI.Core` embeds it as assembly metadata and `PublishToLocal.ps1` reads it.
  Do not hardcode version strings in project files, package metadata, or scripts.
- Scan for stale previous-version references and update only release-required files.

## Changelog

- Update both `CHANGELOG.md` and `CHANGELOG.zh-CN.md` with the same version, release date,
  and information. Use `YYYY-MM-DD` dates.
- Group entries by control or module (`Dialog`, `ToolTip`, `Window`, `DataGrid`, `Theme`,
  `NativeAOT`, `Build`), not a mechanical Added/Changed/Fixed split.
- Write present-tense user-visible outcomes and include PR or issue references when available.
- Follow `docs/engineering/contributing/changelog-guidelines.md`.

## Breaking API changes

Only when the release contains breaking public API changes:
- Create `docs/releases/<version>-api-changes.md` and `docs/releases/<version>-api-changes.zh-CN.md`.
  Use `docs/releases/6.1.4-api-changes.md` and its Chinese sibling as templates.
- Each file must include a quick-reference before/after table and migration code examples
  for every breaking change, plus cross-links between the English and Chinese versions.
- Add the new version links to `docs/releases/overview.md`.
- Put a `Breaking Changes` group first in the root changelog section and link to the detailed file.
- Do not create these files when there are no breaking changes.

## Validation

- Run `git diff --check`.
- Run targeted tests for the controls or modules changed. Prefer
  `dotnet test <test-project> --framework net10.0 --no-restore`.
- **Run the full regression test suite. This step is mandatory when
  executing the `version-release` skill and cannot be replaced by targeted
  module tests. Use the repository's maintained full-test entry point, such
  as `dotnet test AtomUI.slnx --framework net10.0 --no-restore`, when
  applicable. Record the command, result, and any unrelated failures
  separately.**
- Run build or pack validation when packaging or build files changed.
- Run the Gallery NativeAOT publish flow when the release affects AOT, trimming,
  Window, theme, control templates, or source generators.
- Report unrelated failures separately; do not fix them as part of the release.

## Release commit

- Stage only release-required files.
- Follow repository history style, for example:
  `fix(CHANGELOG): update for AtomUI 6.1.5 release with breaking changes and new features`.

## Safety

Do not create tags, push commits, publish packages, or delete release artifacts unless the user explicitly asks for that action.

- Before tag, push, or publish, show the exact commands and stop for confirmation.
- Preferred order: prepare and validate, commit, tag, then push and trigger publication.

## Output

Summarize the changed files, validation results, release commit, and any manual steps that remain.
