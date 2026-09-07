# Avatar Changelog

本文档记录 Avatar 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-09-07

- Breaking API
  - Replace `ImageLoadSource` with the closed `ImageSource` hierarchy and concrete source constructors; no compatibility shim is retained.
  - Replace `ImageCacheMode` request semantics with orthogonal `ImageCacheReadPolicy` and `ImageCacheStoragePolicy` values.
- Loading
  - Adopt source-first validation and content-addressed decode reuse through the shared application loader, so a same-path replacement cannot be hidden by an old decoded-cache entry.
  - Keep Avatar reload as a one-request `RefreshSource` override without changing the caller-owned request options or application cache lifetime.
- Docs
  - Synchronize Avatar source, cache-policy, ownership and lifecycle documentation with the shared ImagePreviewer/image-loading architecture.

## 2026-08-25

- Docs
  - Add the shared Popup pinned-open design link and record AvatarGroup as the semantic owner, with FlyoutHost used only as the relay adapter.
  - Preserve ordinary close behavior after unpinning and allow lifecycle teardown to release the Popup host.

## 2026-08-24

- Breaking API
  - Replace the old image source properties with `Source`, `FallbackSource` and `RequestOptions` based on `ImageLoadSource`.
  - Add `LoadState`, `LoadError`, `LoadProgress`, `IsLoading`, `IsLoaded`, `IsFailed`, `ImageOpened`, `ImageFailed` and `Reload()`.
  - Do not provide obsolete aliases, type forwarding or a per-control loader override.
- Ownership
  - Move `Avatar`, `AbstractAvatar`, `AvatarShape`, `AvatarToken` and `AvatarTheme` to `AtomUI.Controls`.
  - Keep `AvatarGroup` and `AvatarGroupTheme` in `AtomUI.Desktop.Controls` while consuming the shared Avatar Token.
- Behavior
  - Use the application-scoped `IImageLoader`, generation checks, 16 px physical decode buckets, fallback and result leases.
  - Keep the stable content priority `Image > Text > Icon`; loading and failure preserve available Text/Icon fallback content.
- Theme
  - Add `:loading`, `:loaded`, `:failed` and `:fallback` states.
  - Use Avalonia `TextBlock` for `PART_TextPresenter` so the common Avatar theme has no Desktop Controls dependency.

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `Avatar`.
  - Align generated output paths with `controls/avatar/index-cn.md` and `controls/avatar/semantic-cn.md`.

## 2026-06-24

- Docs
  - Complete Avatar desktop architecture and implementation docs with source-derived API groups, template parts, state flow and verification boundaries.
  - Establish Avatar desktop architecture documentation under `docs/controls/desktop/data-display/avatar/overview.md`.
  - Add Avatar implementation documentation covering source ownership, state flow, lifecycle, resources, AOT boundaries and maintenance invariants.
  - Add Avatar control-level changelog.
  - Add Avatar Token documentation covering AvatarToken.
