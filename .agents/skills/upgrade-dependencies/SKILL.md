---
name: atomui-upgrade-dependencies
description: Use when upgrading AtomUI NuGet dependencies, source dependency projects, ReactiveUI, Avalonia, Splat, or any third-party version where compatibility must be evaluated before implementation.
---

# AtomUI Dependency Upgrade

## Core Principle

Dependency upgrades in AtomUI are evidence-first work. The only objective basis for evaluating
version changes is comparing the dependency project's source code between the current version and
the target version. Changelogs, release notes, issues, and docs are useful references only; never
use them as the primary compatibility basis.

## Hard Rules

- Do not modify dependency versions, AtomUI source, or generated files during evaluation.
- Do not implement an upgrade until the user has reviewed and approved the upgrade task plan.
- Do not commit unless the user explicitly asks for a commit.
- Upgrade one dependency family at a time unless the user explicitly approves a grouped upgrade.
- Preserve AtomUI behavior. Compatibility fixes are allowed; unrelated refactors and behavior
  improvements are not part of dependency upgrade work.
- Check the dirty worktree first and do not revert unrelated user changes.
- If planned work changes after discovery, stop and update the plan for user review before
  continuing.

## Version Sources

Check these before evaluating an upgrade:

- `build/Version.props`
- `Directory.Packages.props`
- Relevant `*.csproj` package references
- Existing lock files, restore output, or generated props when relevant

Record the current version, requested target version, and the file that owns each version.

## Reference Source Workflow

AtomUI keeps dependency source projects under `.referenceprojects`.

1. Map the package family to its upstream source repository.
2. Ensure `.referenceprojects/<RepoName>` exists.
3. If it is missing, clone the upstream repository directly under `.referenceprojects`.
4. Run `git fetch --tags --prune` in the reference repository.
5. Resolve both the current version and target version to tags, branches, or commits.
6. If a version cannot be resolved, report the exact blocker; do not guess.
7. Use source diffs as the primary evidence:

```bash
git diff <old-ref>..<new-ref> --name-status
git diff <old-ref>..<new-ref> -- <relevant paths>
git log --oneline <old-ref>..<new-ref>
```

For Avalonia, use `.referenceprojects/Avalonia` when present.

## Evaluation Checklist

Produce an evaluation before proposing implementation. Include:

- Current version and target version.
- Exact source refs compared.
- Source diff summary:
  - public API changes
  - internal API changes AtomUI depends on
  - behavior changes
  - removed or renamed symbols
  - dependency constraint changes
  - target framework, runtime, platform, or tooling changes
- AtomUI usage impact from `rg` searches for affected symbols, namespaces, XAML members,
  extension methods, reflection access, and package references.
- Risk level with concrete reasons.
- Required AtomUI changes, if any.
- Verification commands and manual scenarios.
- Changelog or release note references, clearly marked as secondary context.

## Upgrade Plan Gate

After evaluation, create a complete trackable task plan and wait for user approval. The plan must
be ordered and concrete enough to execute item by item.

Use this shape:

```markdown
## Upgrade Plan: <Dependency> <current-version> -> <target-version>

- [ ] 1. Update version declarations in <files>.
- [ ] 2. Apply compatibility fix for <specific API or behavior>.
- [ ] 3. Update tests or Gallery scenarios for <specific affected area>.
- [ ] 4. Run restore/build verification.
- [ ] 5. Run targeted verification for <specific control or feature>.
- [ ] 6. Report residual risks and pre-existing failures.
```

The user must approve the plan before implementation. Approval must be explicit, such as
"approved", "continue", "execute this plan", or equivalent wording.

## Execution Rules

Once approved:

- Execute the task list strictly from top to bottom.
- Update task status incrementally as items complete.
- Keep edits scoped to the approved plan.
- If a new incompatibility appears, pause and revise the plan for user review.
- If verification fails, distinguish upgrade-caused failures from pre-existing failures.

## Verification

Choose verification based on the affected dependency. At minimum, use:

```bash
dotnet restore
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Debug --framework net10.0 /p:BuildInParallel=false /p:UseSharedCompilation=false
```

Add targeted tests, sample apps, Gallery scenarios, or reference-project inspections when the source
diff shows control behavior, rendering, input, binding, platform, or threading changes.
