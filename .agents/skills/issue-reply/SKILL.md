---
name: issue-reply
description: Use when drafting a concise GitHub issue or discussion reply for AtomUI based on repository state, bug analysis, requested behavior, workarounds, or maintainer follow-up questions.
---

# Issue Reply

Use this skill to draft maintainer-quality replies for AtomUI issues or discussions.

## Workflow

1. Read the issue text supplied by the user. If a URL is provided, open it only when browsing or network access is appropriate.
2. Inspect relevant code, tests, docs, or recent commits before making technical claims.
3. State the current understanding of the issue and the concrete outcome:
   - confirmed bug
   - expected behavior
   - needs reproduction
   - fixed by a specific change
   - workaround available
4. Keep the reply direct and actionable.
5. Ask for only the missing information needed to move the issue forward.

## Output

Return a ready-to-post Markdown reply. Include commands, versions, or reproduction steps only when they are relevant.
