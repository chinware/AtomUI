#!/usr/bin/env bash
set -euo pipefail

repo_root=$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)
host_project="$repo_root/tests/AtomUI.LinkedRegistration.ProjectReference.Fixtures/Host/ProjectReferenceHost.csproj"
artifact_root="$repo_root/.artifacts"
linked_configuration="Debug"
ordinary_configuration="Release"

dotnet restore "$host_project" \
    --disable-build-servers \
    -m:1

dotnet clean "$host_project" \
    --framework net10.0 \
    --configuration "$linked_configuration" \
    --verbosity quiet \
    --disable-build-servers \
    -m:1

dotnet build "$host_project" \
    --framework net10.0 \
    --configuration "$linked_configuration" \
    --no-restore \
    --verbosity minimal \
    --disable-build-servers \
    -m:1

feature_sidecar="$artifact_root/bin/$linked_configuration/net10.0/AtomUI.LinkedRegistration.ProjectReference.Fixtures.Feature.dll.atomui-link.json"
if [[ ! -f "$feature_sidecar" ]]; then
    echo "Feature companion sidecar was not generated from the transitive ProjectReference." >&2
    exit 1
fi

plan_source=$(find "$artifact_root/ProjectReferenceHost/obj/${linked_configuration}GeneratedFiles" \
    -name GeneratedApplicationRegistrationPlan.g.cs \
    -print -quit)
if [[ -z "$plan_source" ]]; then
    echo "Host application registration plan was not generated." >&2
    exit 1
fi
rg -q 'case "AtomUI.Desktop.Controls"' "$plan_source"
rg -q 'GeneratedRegistrationUnit_Buttons_' "$plan_source"

host_assembly="$artifact_root/bin/$linked_configuration/net10.0/AtomUI.LinkedRegistration.ProjectReference.Fixtures.Host.dll"
if [[ ! -f "$host_assembly" ]]; then
    echo "Host assembly was not produced." >&2
    exit 1
fi
snapshot=$(dotnet "$host_assembly")
if ! grep -q 'ProjectReference registration succeeded.' <<<"$snapshot"; then
    echo "Host did not complete Desktop package registration." >&2
    exit 1
fi

dotnet clean "$host_project" \
    --framework net10.0 \
    --configuration "$ordinary_configuration" \
    --verbosity quiet \
    --disable-build-servers \
    -m:1 \
    -p:PublishAot=false

dotnet build "$host_project" \
    --framework net10.0 \
    --configuration "$ordinary_configuration" \
    --no-restore \
    --verbosity minimal \
    --disable-build-servers \
    -m:1 \
    -p:PublishAot=false

ordinary_sidecar=""
for candidate in \
    "$artifact_root/bin/$ordinary_configuration/net10.0/AtomUI.LinkedRegistration.ProjectReference.Fixtures.Feature.dll.atomui-link.json" \
    "$artifact_root/bin/$ordinary_configuration/net10.0/AtomUI.LinkedRegistration.ProjectReference.Fixtures.Shared.dll.atomui-link.json" \
    "$artifact_root/bin/$ordinary_configuration/net10.0/AtomUI.LinkedRegistration.ProjectReference.Fixtures.Host.dll.atomui-link.json"; do
    if [[ -f "$candidate" ]]; then
        ordinary_sidecar="$candidate"
        break
    fi
done
if [[ -z "$ordinary_sidecar" ]]; then
    ordinary_sidecar=$(find \
        "$artifact_root/ProjectReferenceFeature/obj/$ordinary_configuration" \
        "$artifact_root/ProjectReferenceShared/obj/$ordinary_configuration" \
        "$artifact_root/ProjectReferenceHost/obj/$ordinary_configuration" \
        -name '*.atomui-link.json' \
        -print -quit)
fi
if [[ -n "$ordinary_sidecar" ]]; then
    echo "Ordinary build unexpectedly emitted linked-registration sidecars: $ordinary_sidecar" >&2
    exit 1
fi

echo "ProjectReference linked-registration propagation verified."
