---
name: version-release
description: Use when preparing an AtomUI version release, including checking version files, changelog readiness, release commits, tags, build validation, and final release notes.
---

# Version Release

Use this skill when preparing or validating an AtomUI release.

## Workflow

1. Identify the intended version and release scope from the user request.
2. Inspect repository state before changing files:
   - `git status --short`
   - relevant version files
   - changelog or release note files
   - recent tags and commits
3. Update only the files required by the release process.
4. Keep version numbers consistent across project files, package metadata, docs, and release notes.
5. Run the release validation commands used by the repository when they are available.
6. Summarize changed files, validation results, and any manual release steps that remain.

## Safety

Do not create tags, push commits, publish packages, or delete release artifacts unless the user explicitly asks for that action.
