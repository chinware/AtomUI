# Gallery AOT Release Task Tracking

长期工程说明：`docs/engineering/gallery-aot-release-workflow.md`

## Scope

- `.github/workflows/release-gallery.yml`
- `controlgallery/AtomUIGallery.Desktop/scripts/PublishToLocal.ps1`
- `controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj`
- `docs/engineering/gallery-aot-release-workflow.md`

## Task Checklist

- [x] Add default-enabled `PublishAot` workflow input.
- [x] Pass `PublishAot` to all six Gallery publish script calls.
- [x] Keep non-AOT single-file publish available for explicit fallback.
- [x] Add script-side NativeAOT restore with explicit `Configuration=Release` and `GalleryPublishAot=true`.
- [x] Validate restore assets contain ILCompiler packages.
- [x] Validate NativeAOT output does not contain ordinary self-contained runtime files.
- [x] Require `Release` when `PublishAot=true`.
- [x] Install macOS `openssl@3` in DMG jobs.
- [x] Install Linux NativeAOT prerequisites in AppImage jobs.
- [x] Add macOS OpenSSL linker search paths for Apple Silicon and Intel Homebrew layouts.
- [x] Fix osx-arm64 DMG upload step label.
- [x] Move durable design/maintenance documentation out of `docs/superpowers`.
- [x] Update AtomUITools checkout repository to `AtomUI/AtomUITools`.
- [x] Keep `secrets.ACCESS_TOKEN` on private `AtomUI/AtomUITools` checkout.
- [x] Keep standard `actions/checkout` for `AtomUITools` and explicitly use `secrets.ACCESS_TOKEN`.
- [x] Separate `AtomUITools` source checkout and binary output directories.
- [x] Avoid passing global `PublishAot=true` to source generator projects by using `GalleryPublishAot`.
- [x] Make Gallery publish script paths independent of current working directory.

## Verification

- [x] `ruby -e 'require "yaml"; YAML.load_file(".github/workflows/release-gallery.yml"); puts "ok"'`
- [x] `pwsh -NoLogo -NoProfile -Command '$errors = $null; $null = [System.Management.Automation.Language.Parser]::ParseFile("controlgallery/AtomUIGallery.Desktop/scripts/PublishToLocal.ps1", [ref]$null, [ref]$errors); if ($errors.Count) { $errors | Format-List; exit 1 }'`
- [x] `pwsh -NoLogo -NoProfile -Command '[xml](Get-Content -Path "controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj" -Raw) | Out-Null'`
- [x] `perl -ne 'print "$.:$_" if /\t/' .github/workflows/release-gallery.yml`
- [x] `git diff --check`
- [x] `rg -n "PublishToLocal.ps1" .github/workflows/release-gallery.yml`
- [x] Static check that all `AtomUITools` checkout steps use `actions/checkout` with `secrets.ACCESS_TOKEN`.
- [x] `dotnet restore controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj --runtime osx-arm64 -p:Configuration=Release -p:GalleryPublishAot=true --disable-parallel -m:1 /nr:false --nologo -v:minimal`
- [x] `pwsh -NoLogo -NoProfile -File controlgallery/AtomUIGallery.Desktop/scripts/PublishToLocal.ps1 -publishRootPath /tmp/atomui-gallery-aot-publish-test -runtime osx-arm64 -buildType Release -publishAot true`

## Notes

- Real cross-platform NativeAOT publish was not run locally because it needs GitHub runner platforms, signing secrets, and platform toolchains. Local osx-arm64 NativeAOT publish was run successfully.
- `AtomUI/AtomUITools` is private, so anonymous `git ls-remote https://github.com/AtomUI/AtomUITools.git ...` is not used as a validity check. The release workflow relies on `secrets.ACCESS_TOKEN` having read access to that repository.
