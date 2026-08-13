#!/usr/bin/env bash
set -euo pipefail

MINIMUM_DESKTOP_REDUCTION_PERCENT=40
MAX_SECOND_UNIT_GROWTH_BYTES=262144

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
output_root="${ATOMUI_AOT_TRIM_OUTPUT_ROOT:-/tmp/atomui-aot-trim-registration}"
rid="${ATOMUI_AOT_TRIM_RID:-osx-arm64}"
configuration="Release"
fixture_root="$repo_root/tests/AtomUI.LinkedRegistration.Fixtures"
report_path="$output_root/size-report.tsv"
native_aot_macos_targets="$repo_root/build/AtomUI.NativeAot.MacOS.targets"

fixtures=(Minimal TwoUnits DynamicFallback Full)

usage() {
    printf '%s\n' \
        "Usage: $0 [--quick|--full] [--rid RID] [--output-root PATH]" \
        "  --quick  Verify ordinary and generated non-trimmed fixture behavior." \
        "  --full   Also publish trimmed JIT, NativeAOT, Browser AOT, and enforce size gates."
}

mode="quick"
while (($#)); do
    case "$1" in
        --quick)
            mode="quick"
            shift
            ;;
        --full)
            mode="full"
            shift
            ;;
        --rid)
            rid="$2"
            shift 2
            ;;
        --output-root)
            output_root="$2"
            report_path="$output_root/size-report.tsv"
            shift 2
            ;;
        --help|-h)
            usage
            exit 0
            ;;
        *)
            usage >&2
            exit 2
            ;;
    esac
done

mkdir -p "$output_root"

fixture_project() {
    local fixture="$1"
    printf '%s/%s/%s.csproj' "$fixture_root" "$fixture" "$fixture"
}

restore_fixture() {
    local fixture="$1"
    dotnet restore "$(fixture_project "$fixture")" --disable-build-servers
}

run_fixture() {
    local fixture="$1"
    local generated="$2"
    local output_file="$3"
    local raw_output="$output_file.raw"
    local run_log="$output_file.run.log"
    local project
    project="$(fixture_project "$fixture")"
    local fixture_assembly="$repo_root/output/bin/$configuration/net10.0/AtomUI.LinkedRegistration.Fixtures.$fixture.dll"
    local build_args=(
        build "$project"
        -c "$configuration"
        --no-restore
        --disable-build-servers
    )
    if [[ "$generated" == "true" ]]; then
        build_args+=(-p:AtomUIUseGeneratedRegistration=true)
    fi

    dotnet "${build_args[@]}"

    if ! dotnet "$fixture_assembly" >"$raw_output" 2>"$run_log"; then
        cat "$run_log" >&2
        return 1
    fi

    tail -n 1 "$raw_output" >"$output_file"
    grep -q '"PackageCoreFingerprint"' "$output_file"
    grep -q '"DesktopControlCount"' "$output_file"
}

json_string() {
    local key="$1"
    local file="$2"
    sed -n "s/.*\"$key\":\"\([^\"]*\)\".*/\1/p" "$file"
}

json_number() {
    local key="$1"
    local file="$2"
    sed -n "s/.*\"$key\":\([0-9][0-9]*\).*/\1/p" "$file"
}

verify_non_trimmed_modes() {
    for fixture in "${fixtures[@]}"; do
        restore_fixture "$fixture"
        run_fixture "$fixture" false "$output_root/$fixture.ordinary.json"
        run_fixture "$fixture" true "$output_root/$fixture.generated.json"
    done

    local minimal_full_count
    local minimal_generated_count
    local two_units_generated_count
    local dynamic_generated_count
    local full_generated_count
    minimal_full_count="$(json_number DesktopControlCount "$output_root/Minimal.ordinary.json")"
    minimal_generated_count="$(json_number DesktopControlCount "$output_root/Minimal.generated.json")"
    two_units_generated_count="$(json_number DesktopControlCount "$output_root/TwoUnits.generated.json")"
    dynamic_generated_count="$(json_number DesktopControlCount "$output_root/DynamicFallback.generated.json")"
    full_generated_count="$(json_number DesktopControlCount "$output_root/Full.generated.json")"

    [[ "$minimal_full_count" -eq "$full_generated_count" ]]
    [[ "$minimal_generated_count" -lt "$minimal_full_count" ]]
    [[ "$two_units_generated_count" -gt "$minimal_generated_count" ]]
    [[ "$dynamic_generated_count" -eq "$minimal_full_count" ]]
    [[ "$full_generated_count" -eq "$minimal_full_count" ]]

    grep -q '"AtomUI:Button"' "$output_root/Minimal.generated.json"
    if grep -q '"AtomUI:DatePicker"' "$output_root/Minimal.generated.json"; then
        printf 'Minimal unexpectedly retained the DatePicker Unit.\n' >&2
        return 1
    fi
    grep -q '"AtomUI:Button"' "$output_root/TwoUnits.generated.json"
    grep -q '"AtomUI:DatePicker"' "$output_root/TwoUnits.generated.json"
    cmp -s "$output_root/DynamicFallback.generated.json" "$output_root/DynamicFallback.ordinary.json"
    cmp -s "$output_root/Full.generated.json" "$output_root/Full.ordinary.json"

    local expected_core
    expected_core="$(json_string PackageCoreFingerprint "$output_root/Minimal.ordinary.json")"
    for fixture in "${fixtures[@]}"; do
        [[ "$(json_string PackageCoreFingerprint "$output_root/$fixture.ordinary.json")" == "$expected_core" ]]
        [[ "$(json_string PackageCoreFingerprint "$output_root/$fixture.generated.json")" == "$expected_core" ]]
    done
}

publish_fixture() {
    local fixture="$1"
    local publish_kind="$2"
    local output_name="${3:-$fixture}"
    local extra_property="${4:-}"
    local publish_dir="$output_root/$publish_kind/$output_name"
    local args=(publish "$(fixture_project "$fixture")" -c "$configuration" -r "$rid" --self-contained true -o "$publish_dir")
    case "$publish_kind" in
        trimmed)
            args+=(-p:PublishTrimmed=true -p:PublishAot=false)
            ;;
        aot)
            args+=(
                -p:PublishAot=true
                "-p:CustomAfterMicrosoftCommonTargets=$native_aot_macos_targets"
            )
            ;;
        *)
            return 2
            ;;
    esac
    if [[ -n "$extra_property" ]]; then
        args+=("-p:$extra_property")
    fi
    dotnet "${args[@]}" >&2
    printf '%s' "$publish_dir"
}

directory_size() {
    find "$1" -type f -print0 | xargs -0 stat -f '%z' | awk '{ total += $1 } END { print total + 0 }'
}

main_binary_size() {
    local publish_dir="$1"
    local fixture="$2"
    local base_name="AtomUI.LinkedRegistration.Fixtures.$fixture"
    if [[ -f "$publish_dir/$base_name" ]]; then
        stat -f '%z' "$publish_dir/$base_name"
    else
        stat -f '%z' "$publish_dir/$base_name.dll"
    fi
}

verify_plan_artifacts() {
    local publish_dir="$1"
    local fixture="$2"
    local runtime_config
    runtime_config="$(find "$publish_dir" -maxdepth 1 -name '*.runtimeconfig.json' -print -quit)"
    if [[ -n "$runtime_config" ]]; then
        grep -q 'AtomUI.AotTrimRegistration.Enabled' "$runtime_config"
    fi
    local managed_assembly="$publish_dir/AtomUI.LinkedRegistration.Fixtures.$fixture.dll"
    if [[ -f "$managed_assembly" ]]; then
        strings "$managed_assembly" | grep -q 'AtomUI.Linked.Plan.v1'
    fi
}

record_size() {
    local fixture="$1"
    local publish_kind="$2"
    local publish_dir="$3"
    local binary_fixture="${4:-$fixture}"
    local snapshot_file="${5:-$output_root/$publish_kind.$fixture.json}"
    printf '%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\n' \
        "$(dotnet --version)" "$rid" "$configuration" "true" "$publish_kind" "$fixture" \
        "$(directory_size "$publish_dir")" "$(main_binary_size "$publish_dir" "$binary_fixture")" \
        "$(json_string PackageCoreFingerprint "$snapshot_file")" >>"$report_path"
}

verify_size_gates() {
    local minimal_size
    local unused_unit_size
    local full_size
    minimal_size="$(awk -F '\t' '$5 == "aot" && $6 == "Minimal" { print $7 }' "$report_path")"
    unused_unit_size="$(awk -F '\t' '$5 == "aot" && $6 == "MinimalWithUnusedUnit" { print $7 }' "$report_path")"
    full_size="$(awk -F '\t' '$5 == "aot" && $6 == "Full" { print $7 }' "$report_path")"

    local reduction_percent=$(( (full_size - minimal_size) * 100 / full_size ))
    local second_unit_growth=$(( unused_unit_size - minimal_size ))
    (( reduction_percent >= MINIMUM_DESKTOP_REDUCTION_PERCENT ))
    (( second_unit_growth <= MAX_SECOND_UNIT_GROWTH_BYTES ))
}

verify_full_publish_matrix() {
    printf 'sdk\trid\tconfiguration\tself_contained\ttrim_mode\tfixture\ttotal_bytes\tmain_bytes\tpackage_core_fingerprint\n' >"$report_path"
    for fixture in Minimal DynamicFallback; do
        local publish_dir
        publish_dir="$(publish_fixture "$fixture" trimmed)"
        verify_plan_artifacts "$publish_dir" "$fixture"
        "$publish_dir/AtomUI.LinkedRegistration.Fixtures.$fixture" >"$output_root/trimmed.$fixture.json"
        if [[ "$fixture" == "Minimal" ]]; then
            cmp -s "$output_root/trimmed.Minimal.json" "$output_root/Minimal.generated.json"
        else
            cmp -s "$output_root/trimmed.DynamicFallback.json" "$output_root/DynamicFallback.ordinary.json"
        fi
        record_size "$fixture" trimmed "$publish_dir"
    done
    for fixture in "${fixtures[@]}"; do
        local publish_dir
        publish_dir="$(publish_fixture "$fixture" aot)"
        verify_plan_artifacts "$publish_dir" "$fixture"
        "$publish_dir/AtomUI.LinkedRegistration.Fixtures.$fixture" >"$output_root/aot.$fixture.json"
        case "$fixture" in
            Minimal|TwoUnits)
                cmp -s "$output_root/aot.$fixture.json" "$output_root/$fixture.generated.json"
                ;;
            DynamicFallback|Full)
                cmp -s "$output_root/aot.$fixture.json" "$output_root/$fixture.ordinary.json"
                ;;
        esac
        record_size "$fixture" aot "$publish_dir"
    done

    local unused_publish_dir
    unused_publish_dir="$(publish_fixture Minimal aot MinimalWithUnusedUnit AtomUIIncludeUnusedFixtureUnit=true)"
    verify_plan_artifacts "$unused_publish_dir" Minimal
    "$unused_publish_dir/AtomUI.LinkedRegistration.Fixtures.Minimal" >"$output_root/aot.MinimalWithUnusedUnit.json"
    cmp -s "$output_root/aot.MinimalWithUnusedUnit.json" "$output_root/aot.Minimal.json"
    record_size MinimalWithUnusedUnit aot "$unused_publish_dir" Minimal \
        "$output_root/aot.MinimalWithUnusedUnit.json"

    dotnet publish "$repo_root/controlgallery/AtomUIGallery.Browser/AtomUIGallery.Browser.csproj" \
        -c Release -p:RunAOTCompilation=true
    verify_size_gates
}

verify_non_trimmed_modes
if [[ "$mode" == "full" ]]; then
    verify_full_publish_matrix
fi

printf 'Linked registration verification completed. Report: %s\n' "$report_path"
