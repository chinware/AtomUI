---
name: atomui-gallery-local-debug
description: Use when building, launching, debugging, or visually inspecting AtomUIGallery.Desktop from an AtomUI checkout where installed or mounted Gallery apps may share its name or bundle identifier.
---

# AtomUI Gallery Local Debug

## Overview

Launch and identify the Gallery from the current checkout. The build identity, process identity, and UI automation target must all resolve to the same absolute executable path.

## Required Workflow

Run from anywhere inside the target AtomUI checkout:

```bash
.agents/skills/atomui-gallery-local-debug/scripts/launch-gallery-debug.sh
```

The launcher builds `AtomUIGallery.Desktop`, directly executes the current checkout's `.artifacts` binary, verifies its PID command, and prints `REPO_ROOT`, `BINARY`, `PID`, and `LOG`. Keep that launcher command running during visual inspection; it waits on the Gallery so command runners cannot reap a detached GUI process.

If an unrecorded process already runs from that exact path, the launcher reports `EXISTING_PID` and stops without overwriting its PID file. Close or explicitly terminate that verified process before retrying so a stale loaded assembly cannot be mistaken for the new build.

Treat the printed `BINARY` and `PID` as the only valid runtime identity. Before visual inspection, confirm the PID still resolves to `BINARY`.

For Computer Use, connect with the printed absolute executable path:

```javascript
let galleryApp = await cua.getApp("<BINARY>");
```

If exact-path connection fails, stop visual automation and report the limitation. Do not fall back to `AtomUIGallery`, `AtomUI Desktop Gallery`, `net.atomui.gallery`, `open`, or an `.app` under `/Applications` or `/Volumes`.

After connecting, inspect the accessibility tree or screenshot and confirm the requested current-checkout content is present before interacting further.

## Options

| Command | Use |
| --- | --- |
| `launch-gallery-debug.sh` | Build Debug and start the verified binary. |
| `launch-gallery-debug.sh --no-build` | Start an already-built Debug binary. |
| `launch-gallery-debug.sh --configuration Release` | Build and start Release. |
| `launch-gallery-debug.sh --dry-run --no-build` | Print resolved identities without building or starting. |
| `launch-gallery-debug.sh --stop` | Stop only the PID previously recorded by this checkout after verifying its path. |

When a command tool returns a live session identifier, retain it until inspection finishes. Run `--stop` separately, then drain the original session.

## Verification

After changing the Skill, run:

```bash
bash .agents/skills/atomui-gallery-local-debug/tests/test-launcher-contract.sh
```

## Completion Contract

- Build succeeds for the requested configuration.
- `BINARY` is inside the current `git rev-parse --show-toplevel` directory.
- The launched PID's command begins with the exact `BINARY` path.
- UI automation uses that path or stops; a matching display name or bundle ID is not proof.
- Cleanup affects only the verified PID stored under this checkout's `.artifacts/run` directory.

## Common Mistakes

| Mistake | Required response |
| --- | --- |
| `dotnet run` succeeded, so any same-name window is assumed correct. | Verify the process executable path; build success does not identify the automated window. |
| Bundle ID lookup reports ambiguity, then application name is used instead. | Stop. Both identifiers use LaunchServices and can select an installed or mounted copy. |
| Exact-path UI binding is unavailable. | Report visual verification as blocked; keep build and automated test evidence separate. |
| An old PID file points to another process. | Refuse to signal it; the launcher must match the recorded PID command to the current `BINARY`. |
| An unrecorded current-path process is already running. | Report its PID and stop before overwriting the PID file or starting a possibly stale duplicate. |
