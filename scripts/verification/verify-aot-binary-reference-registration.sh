#!/usr/bin/env bash
set -euo pipefail

repo_root=$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)
consumer_project="$repo_root/tests/AtomUI.LinkedRegistration.BinaryReference.Fixtures/Consumer/BinaryReferenceConsumer.csproj"
missing_consumer_project="$repo_root/tests/AtomUI.LinkedRegistration.BinaryReference.Fixtures/ConsumerWithoutEntry/BinaryReferenceConsumerWithoutEntry.csproj"
host_project="$repo_root/tests/AtomUI.LinkedRegistration.BinaryReference.Fixtures/Host/BinaryReferenceHost.csproj"
missing_host_project="$repo_root/tests/AtomUI.LinkedRegistration.BinaryReference.Fixtures/MissingEntryHost/BinaryReferenceMissingEntryHost.csproj"
artifact_root="$repo_root/.artifacts"

for project in "$consumer_project" "$missing_consumer_project" "$host_project" "$missing_host_project"; do
    dotnet restore "$project" \
        --disable-build-servers \
        -m:1
done

dotnet clean "$consumer_project" \
    --framework net10.0 \
    --configuration Release \
    --verbosity quiet \
    --disable-build-servers \
    -m:1 \
    -p:PublishAot=false
dotnet build "$consumer_project" \
    --framework net10.0 \
    --configuration Release \
    --no-restore \
    --verbosity minimal \
    --disable-build-servers \
    -m:1 \
    -p:PublishAot=false
dotnet build "$missing_consumer_project" \
    --framework net10.0 \
    --configuration Release \
    --no-restore \
    --verbosity minimal \
    --disable-build-servers \
    -m:1 \
    -p:PublishAot=false

consumer_assembly="$artifact_root/bin/Release/net10.0/AtomUI.LinkedRegistration.BinaryReference.Fixtures.Consumer.dll"
if [[ ! -f "$consumer_assembly" ]]; then
    echo "Ordinary precompiled consumer assembly was not produced." >&2
    exit 1
fi
if [[ -f "$consumer_assembly.atomui-link.json" ]]; then
    echo "Ordinary precompiled consumer unexpectedly produced a companion Sidecar." >&2
    exit 1
fi

dotnet clean "$host_project" \
    --framework net10.0 \
    --configuration Debug \
    --verbosity quiet \
    --disable-build-servers \
    -m:1
dotnet build "$host_project" \
    --framework net10.0 \
    --configuration Debug \
    --no-restore \
    --verbosity minimal \
    --disable-build-servers \
    -m:1

recovered_sidecar="$artifact_root/BinaryReferenceHost/obj/Debug/net10.0/AtomUIRecoveredConsumerSidecars/AtomUI.LinkedRegistration.BinaryReference.Fixtures.Consumer.atomui-link.json"
if [[ ! -f "$recovered_sidecar" ]]; then
    echo "Precompiled consumer registration evidence was not recovered." >&2
    exit 1
fi
rg -q '"kind":"PackageRoot","identity":"AtomUI.Desktop.Controls"' "$recovered_sidecar"
rg -q '"kind":"Entry","identity":"AtomUI.Desktop.Controls"' "$recovered_sidecar"
rg -q '"reason":"ExtractedConsumerAssembly"' "$recovered_sidecar"

plan_source=$(find "$artifact_root/BinaryReferenceHost/obj/DebugGeneratedFiles" \
    -name GeneratedApplicationRegistrationPlan.g.cs \
    -print -quit)
if [[ -z "$plan_source" ]]; then
    echo "Binary Host application registration plan was not generated." >&2
    exit 1
fi
rg -q 'case "AtomUI.Desktop.Controls"' "$plan_source"
rg -q 'GeneratedFullControlPackageRegistrationFragment.Register' "$plan_source"

host_assembly="$artifact_root/bin/Debug/net10.0/AtomUI.LinkedRegistration.BinaryReference.Fixtures.Host.dll"
snapshot=$(dotnet "$host_assembly")
if ! grep -q 'Binary reference registration succeeded.' <<<"$snapshot"; then
    echo "Binary Host did not complete Desktop package registration." >&2
    exit 1
fi

dotnet clean "$missing_host_project" \
    --framework net10.0 \
    --configuration Debug \
    --verbosity quiet \
    --disable-build-servers \
    -m:1
set +e
missing_output=$(dotnet build "$missing_host_project" \
    --framework net10.0 \
    --configuration Debug \
    --no-restore \
    --verbosity minimal \
    --disable-build-servers \
    -m:1 2>&1)
missing_status=$?
set -e
printf '%s\n' "$missing_output"
if [[ $missing_status -eq 0 ]]; then
    echo "Binary consumer without an entry call unexpectedly built successfully." >&2
    exit 1
fi
if ! grep -q "ATOMUILINK008: Package 'AtomUI.Desktop.Controls' is used by 'AtomUI.LinkedRegistration.BinaryReference.Fixtures.ConsumerWithoutEntry.dll'" <<<"$missing_output"; then
    echo "Missing binary entry did not report ATOMUILINK008 against the consumer assembly." >&2
    exit 1
fi

echo "Binary-reference linked-registration recovery verified."
