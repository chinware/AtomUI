#!/usr/bin/env bash

set -euo pipefail

usage()
{
    printf '%s\n' \
        'Usage: launch-gallery-debug.sh [--configuration Debug|Release] [--no-build] [--dry-run] [--stop]'
}

configuration="Debug"
should_build="true"
dry_run="false"
stop_only="false"

while [[ $# -gt 0 ]]
do
    case "$1" in
        --configuration)
            [[ $# -ge 2 ]] || { usage >&2; exit 2; }
            configuration="$2"
            shift 2
            ;;
        --no-build)
            should_build="false"
            shift
            ;;
        --dry-run)
            dry_run="true"
            shift
            ;;
        --stop)
            stop_only="true"
            shift
            ;;
        --help|-h)
            usage
            exit 0
            ;;
        *)
            printf 'Unknown argument: %s\n' "$1" >&2
            usage >&2
            exit 2
            ;;
    esac
done

if [[ "$configuration" != "Debug" && "$configuration" != "Release" ]]
then
    printf 'Unsupported configuration: %s\n' "$configuration" >&2
    exit 2
fi

repo_root="$(git rev-parse --show-toplevel 2>/dev/null)" || {
    printf 'The current directory is not inside a Git checkout.\n' >&2
    exit 1
}
project="$repo_root/controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj"
binary="$repo_root/.artifacts/bin/$configuration/net10.0/AtomUIGallery.Desktop"
run_directory="$repo_root/.artifacts/run"
pid_file="$run_directory/AtomUIGallery.Desktop-$configuration.pid"
log_file="$run_directory/AtomUIGallery.Desktop-$configuration.log"

if [[ ! -f "$project" ]]
then
    printf 'AtomUIGallery.Desktop project was not found under the current checkout: %s\n' "$project" >&2
    exit 1
fi

print_identity()
{
    printf 'REPO_ROOT=%s\n' "$repo_root"
    printf 'PROJECT=%s\n' "$project"
    printf 'BINARY=%s\n' "$binary"
    printf 'PID_FILE=%s\n' "$pid_file"
    printf 'LOG=%s\n' "$log_file"
}

get_process_command()
{
    ps -ww -p "$1" -o command= 2>/dev/null || true
}

is_expected_command()
{
    local process_command="$1"
    [[ "$process_command" == "$binary" || "$process_command" == "$binary"\ * ]]
}

is_expected_process()
{
    local process_command
    process_command="$(get_process_command "$1")"
    is_expected_command "$process_command"
}

find_unrecorded_process()
{
    local candidate_pid
    local candidate_command

    while read -r candidate_pid candidate_command
    do
        if [[ "$candidate_pid" =~ ^[0-9]+$ ]] && is_expected_command "$candidate_command"
        then
            printf '%s\n' "$candidate_pid"
            return 0
        fi
    done < <(ps -axo pid=,command= 2>/dev/null || true)

    return 1
}

stop_recorded_process()
{
    [[ -f "$pid_file" ]] || return 0

    local recorded_pid
    read -r recorded_pid < "$pid_file" || true
    [[ "$recorded_pid" =~ ^[0-9]+$ ]] || return 0
    kill -0 "$recorded_pid" 2>/dev/null || return 0

    if ! is_expected_process "$recorded_pid"
    then
        printf 'Refusing to stop PID %s because its command does not match BINARY=%s\n' \
            "$recorded_pid" "$binary" >&2
        return 1
    fi

    kill -TERM "$recorded_pid"
    for _ in {1..30}
    do
        kill -0 "$recorded_pid" 2>/dev/null || break
        sleep 0.1
    done

    if kill -0 "$recorded_pid" 2>/dev/null
    then
        printf 'PID %s did not stop after SIGTERM; refusing to escalate to SIGKILL.\n' "$recorded_pid" >&2
        return 1
    fi

    : > "$pid_file"
    printf 'STOPPED_PID=%s\n' "$recorded_pid"
}

if [[ "$dry_run" == "true" ]]
then
    print_identity
    printf 'BUILD=%s\n' "$should_build"
    printf 'STOP_ONLY=%s\n' "$stop_only"
    exit 0
fi

mkdir -p "$run_directory"

if [[ "$stop_only" == "true" ]]
then
    print_identity
    stop_recorded_process
    exit 0
fi

if [[ "$should_build" == "true" ]]
then
    dotnet build "$project" \
        --framework net10.0 \
        --configuration "$configuration" \
        --no-restore
fi

if [[ ! -x "$binary" ]]
then
    printf 'Expected Gallery executable is missing or not executable: %s\n' "$binary" >&2
    exit 1
fi

stop_recorded_process

existing_pid="$(find_unrecorded_process || true)"
if [[ -n "$existing_pid" ]]
then
    print_identity
    printf 'EXISTING_PID=%s\n' "$existing_pid"
    printf 'Refusing to overwrite the PID file or start a duplicate. Stop this exact-path process, then retry.\n' >&2
    exit 1
fi

"$binary" > "$log_file" 2>&1 &
gallery_pid=$!
printf '%s\n' "$gallery_pid" > "$pid_file"

for _ in {1..50}
do
    if ! kill -0 "$gallery_pid" 2>/dev/null
    then
        printf 'Gallery exited during startup. Inspect LOG=%s\n' "$log_file" >&2
        exit 1
    fi

    is_expected_process "$gallery_pid" && break
    sleep 0.1
done

if ! is_expected_process "$gallery_pid"
then
    printf 'Launched PID %s does not resolve to BINARY=%s\n' "$gallery_pid" "$binary" >&2
    exit 1
fi

print_identity
printf 'PID=%s\n' "$gallery_pid"
printf 'COMMAND=%s\n' "$(get_process_command "$gallery_pid")"

# Keep the launcher attached to the GUI process. Codex command runners may reap
# detached background processes when their shell exits, even when nohup is used.
set +e
wait "$gallery_pid"
gallery_status=$?
set -e

recorded_pid=""
read -r recorded_pid < "$pid_file" || true
if [[ "$recorded_pid" == "$gallery_pid" ]]
then
    : > "$pid_file"
fi

if [[ "$gallery_status" -eq 143 ]]
then
    exit 0
fi

exit "$gallery_status"
