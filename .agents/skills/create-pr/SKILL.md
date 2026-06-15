---
name: create-pr
description: Use when preparing an AtomUI pull request summary from local git changes, including the change overview, validation performed, risks, and reviewer notes.
---

# Create PR

Use this skill to prepare a pull request description for AtomUI changes.

## Workflow

1. Inspect `git status --short` to understand changed files.
2. Inspect staged and unstaged diffs as appropriate for the user's request.
3. Read nearby code when the diff alone does not explain behavior or risk.
4. Summarize the PR around behavior, not file churn.
5. Include validation that was actually run. If no validation was run, say so directly.
6. Call out risks, migration notes, or reviewer focus areas when relevant.

## PR Template

```markdown
## Summary
-

## Validation
-

## Notes
-
```

Drop sections that are not useful for the specific PR.
