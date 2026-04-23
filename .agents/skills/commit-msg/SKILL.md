---
name: atomui-commit-msg
description: Generate a single-line commit message for AtomUI by reading the project's git staged area and recent commit style. Use when the user asks for a commit message, says "msg", "commit msg", "写提交信息", or wants one-line text that covers all staged changes. Output should match the repository's existing commit style and summarize all staged changes in one line.
---

# AtomUI Commit Message Generation Skill

## Goals

**1. Accurately summarize the commit** — Generate a one-line commit message based on the current **git staging area**, covering all staged changes.

**2. Stay consistent with repository style** — Match the repository's existing commit conventions rather than mechanically applying a template.

## Core Principle

> A commit message is not a per-file listing of the diff — it is a compressed expression of the commit's intent. Read the staging area first, then summarize, then output one line.

## Trigger Conditions

Use this skill when the user:

- Asks for a commit message or "提交信息"
- Says "msg" in a git commit context
- Wants a one-line summary generated from current changes
- Asks the agent to summarize staged changes into a commit title

## Fundamental Rules

### 1. Default to staged changes only

Generate the message based on **staged changes** only, because that is what will actually be committed.

Only include unstaged changes if the user explicitly requests it (e.g., "include unstaged" or "cover all changes").

### 2. Always check recent commit style first

Before generating a message, review recent commits to avoid producing a format inconsistent with the repository's conventions.

### 3. Default to a single line of output

Unless the user explicitly asks for an explanation or a body, the final response should be **one line of commit message only** — no analysis, bullet points, code blocks, or quotes.

## Execution Steps

### Step 1: Read git status, staging area, and recent commits

Gather the following information before generating. Do not guess or rely on filenames alone.

**Recommended commands:**

```bash
git status --short
git diff --cached --stat
git diff --cached
git log --oneline -10
```

- `git status --short` — Confirm which files are staged and whether unstaged changes exist.
- `git diff --cached --stat` — Quickly assess the scope of changes.
- `git diff --cached` — Read the actual content being committed. This is the primary basis for the message.
- `git log --oneline -10` — Check the repository's recent commit style, language, and type/scope conventions.

### Step 2: Check if generation is possible

If the staging area is empty:

- Do NOT fabricate a message.
- Clearly inform the user that there are no staged changes and an accurate commit message cannot be generated.

### Step 3: Identify the primary semantic intent

Based on the `git diff --cached` output:

1. Determine the **primary purpose** — new feature, bug fix, documentation, refactoring, performance optimization, test, Gallery ShowCase, script/dependency update, etc.
2. Identify the **primary affected scope** — specific control name, infrastructure module (TokenSystem / Theme / Motion, etc.), Gallery, scripts, docs, etc.
3. If the diff contains multiple files or mixed small changes, use a higher-level summary that covers everything. Do not concatenate per-file descriptions into one long sentence.

### Step 4: Align with AtomUI repository style

Prefer to mimic the repository's recent conventions. Common patterns in AtomUI:

- `feat(Component): ...`
- `fix(Component): ...`
- `refactor(Component): ...`
- `docs: ...` / `docs(Core): ...`
- `chore(deps): ...`
- `gallery: ...` / `gallery(Component): ...`
- `build(ci): ...`
- `perf(TokenSystem): ...`

Notes:

- `type` and `scope` **MUST be in English**.
- `subject` **defaults to English**; Chinese is acceptable if the user is conversing in Chinese or explicitly requests it.
- If the repository already uses a more natural phrasing, prefer matching the existing style.
- `gallery` is used as a type prefix in AtomUI (analogous to `site` in ant-design) for ShowCase / Gallery app changes.

### Step 5: Generate one line

The output must be:

- **One line**
- **Covering all staged changes**
- **Concise**
- **Consistent with repository style**
- **Ready to use as-is for the commit**

## Writing Rules

### Header format

Use one of the following:

```text
<type>(<scope>): <subject>
```

or (when scope cannot be determined):

```text
<type>: <subject>
```

Examples:

- `feat(Button): add danger style variant for ghost button`
- `fix(Select): prevent dropdown from closing on multi-select click`
- `refactor(Avatar): extract AbstractAvatar to AtomUI.Controls`
- `docs(Core): add three-tier architecture documentation`
- `gallery: add QRCode showcase with comprehensive examples`
- `chore(deps): bump Avalonia from 11.3.1 to 11.3.2`
- `fix(Select): 修复多选模式下点击后下拉框意外关闭`

### Header rules

- Use **imperative mood** — `add` / `fix` / `update`, not `added` / `fixed` / `updated`. For Chinese subjects, start with a verb: "修复" / "添加" / "调整".
- Do **NOT** capitalize the first letter of an English subject.
- Do **NOT** end with a period (`.` or `。`).
- Keep the header line to **72 characters or fewer**.
- Avoid vague terms like `WIP`, `misc`, `update files`.

### Type selection

All types are **lowercase**:

- `feat` — New feature, control, property, or API
- `fix` — Bug fix — incorrect behavior, crash, memory leak, rendering issue
- `refactor` — Code restructuring with no behavior change — moving code between layers, renaming, splitting classes
- `docs` — Documentation only — README, XML doc comments, architecture docs, copilot-instructions
- `style` — Code formatting only (no logic change) — whitespace, indentation, code style
- `perf` — Performance improvement — rendering optimization, reduced allocations, caching
- `test` — Adding or updating tests
- `build` — Build system or dependency changes — MSBuild props, `Directory.Packages.props`, scripts
- `chore` — Maintenance that doesn't modify src/test — tooling config, `.editorconfig`, IDE settings
- `ci` — CI/CD configuration changes
- `gallery` — Gallery / ShowCase changes — demo pages, gallery navigation, gallery assets
- `release` — Version bumps, changelog updates, release preparation
- `revert` — Reverting a previous commit (must reference the reverted commit hash in the body)

When uncertain:

- User-visible behavior correction → prefer `fix`
- Only text, examples, or descriptions updated → prefer `docs`
- Only tooling, dependencies, or scripts adjusted → prefer `chore`
- ShowCase / Gallery demo changes → prefer `gallery`
- Code structure changed but behavior unchanged → prefer `refactor`

### Scope selection

- For changes to a single control, use the control name in PascalCase: `Button`, `Select`, `DatePicker`, `TreeView`, `Cascader`, `FloatButton`
- For infrastructure modules, use the module name: `TokenSystem`, `Theme`, `Motion`, `WaveSpirit`, `Core`, `Icons`, `Fonts`, `Generator`, `Native`, `i18n`, `MediaQuery`
- For cross-cutting maintenance, use a summary scope: `deps`, `scripts`, `Packaging`, `Bindings`, `MemoryLeak`
- For multiple closely related controls, use comma-separated names: `Select,AutoComplete`
- If no clear scope exists, it may be omitted

## Language Rules

- `type` and `scope` **MUST be English** (they are fixed keywords / identifiers)
- `subject` and `body` **default to English; Chinese is also acceptable**
- Use Chinese for the `subject` when the user is conversing in Chinese or explicitly requests it

## Layer Awareness

AtomUI uses a three-tier architecture: Foundation → Base Control (`AtomUI.Controls`) → Platform Control (`AtomUI.Desktop.Controls`).

When changes span layers:

- If each layer's changes can be understood independently, suggest splitting into multiple commits
- If tightly coupled and inseparable, use a single commit with the control name as the scope

## Edge Cases

### Mixed change types

If staged changes include a mix of docs, styles, types, and small fixes:

- Identify the **primary purpose** first
- If there is no single primary purpose, use a higher-level summary
- The goal is to "honestly cover all changes", not to cram every detail into the title

### Scattered / unrelated changes

If the staging area contains clearly unrelated groups of changes:

- Still produce an honest one-line message
- Do not pretend these changes share a single specific purpose
- Use a broad summary, e.g., `chore(controls): clean up styles, types and docs`

### Breaking Changes

If the changes include a breaking API change:

- Append `!` after type/scope, e.g., `refactor(TokenSystem)!: rename ControlDesignToken to AbstractControlDesignToken`
- If the user requests a body, include a `BREAKING CHANGE:` footer

## Prohibited

- Generating a message without reading `git diff --cached`
- Guessing content based only on filenames
- Describing only some files/changes while ignoring other staged content
- Outputting multi-line explanations as the commit message
- Breaking from established repository style just to fit a template
- Writing headers longer than 72 characters unless truly unavoidable
- Wrapping the final output in backticks, quotes, or code blocks
- Do not push to the remote end unless explicitly requested
- Do not add "Co-Authored-By: xxx" or information regarding other agents.

## Reference

For detailed type/scope definitions, style examples, and input→output samples, see `references/commit-message-convention.md`.