---
name: changelog-collect
description: Use when collecting AtomUI changelog entries from git history, staged changes, issue references, or release notes and turning them into a concise Markdown changelog draft grouped by change type.
---

# Changelog Collect

Use this skill to collect user-facing changes for AtomUI release notes or a changelog draft.

## Workflow

1. Inspect the requested range:
   - Prefer the user-provided commit range, tag range, branch, PR, or staged changes.
   - If no range is provided, inspect recent commits and ask only if the intended release range is ambiguous.
2. Read the relevant diffs and commit messages.
3. Group entries by practical release categories such as `Added`, `Changed`, `Fixed`, `Performance`, `Docs`, and `Build`.
4. Keep entries concise and user-facing. Avoid listing internal refactors unless they affect behavior, compatibility, performance, or maintainability that users should know.
5. Include issue or PR references when they are available from commits or branch names.
6. Report any uncertainty, especially when a commit message is unclear or a change looks incomplete.

## Output

Produce a Markdown changelog draft and mention the source range used to build it.
