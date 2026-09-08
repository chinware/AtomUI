#!/usr/bin/env bash

set -euo pipefail

skill_root="$(cd "$(dirname "$0")/.." && pwd)"
repo_root="$(git -C "$skill_root" rev-parse --show-toplevel)"
launcher="$skill_root/scripts/launch-gallery-debug.sh"

if [[ ! -x "$launcher" ]]
then
    printf 'FAIL: launcher is missing or not executable: %s\n' "$launcher" >&2
    exit 1
fi

bash -n "$launcher"
dry_run_output="$($launcher --dry-run --no-build)"
expected_binary="$repo_root/.artifacts/bin/Debug/net10.0/AtomUIGallery.Desktop"

if [[ "$dry_run_output" != *"REPO_ROOT=$repo_root"* ||
      "$dry_run_output" != *"BINARY=$expected_binary"* ]]
then
    printf 'FAIL: dry run did not resolve the current checkout identities\n%s\n' \
        "$dry_run_output" >&2
    exit 1
fi

if [[ "$dry_run_output" == *"/Applications/AtomUIGallery.app"* ||
      "$dry_run_output" == *"/Volumes/"* ]]
then
    printf 'FAIL: dry run resolved an installed or mounted application\n%s\n' \
        "$dry_run_output" >&2
    exit 1
fi

if "$launcher" --dry-run --configuration Unsupported >/dev/null 2>&1
then
    printf 'FAIL: launcher accepted an unsupported configuration\n' >&2
    exit 1
fi

printf 'PASS: launcher contract is checkout-local and configuration-safe\n'
