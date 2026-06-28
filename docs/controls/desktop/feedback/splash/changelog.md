# Splash Changelog

本文档记录 Splash 控件级设计、API、主题契约、Token 和实现结构的变化。它不替代仓库根目录 `CHANGELOG.md`，也不作为正式版本发布说明。

## 2026-06-28

- Code
  - Add the first desktop Splash implementation in `AtomUI.Desktop.Controls.Extras`.
  - Add `Splash`, `SplashWindow`, `ISplashService`, `SplashService`, `SplashOptions`, `SplashStatus` and Splash theme resources.
  - Add `UseDesktopExtras()` theme registration and generated Splash token resource keys.
  - Add Splash behavior tests for visual state, progress normalization, controller updates, static API delegation and idempotent close.
  - Add the Gallery showcase under `controlgallery/AtomUIGallery/ShowCases/Other/Splash`.
  - Move the Splash window shell, surface shadow and host corner radius to `SplashWindowTheme.axaml` with `ShadowsAwareContainer`.
  - Remove the C# surface host/token bridge so window-level Splash visual overrides resolve from `SplashWindow.Resources`.
  - Move state update methods from `SplashController` into `Splash` and remove the standalone controller type.
  - Refine `SplashWindow` property contracts so `Splash` is a nullable styled content property, timing options are styled properties and close-request state remains direct runtime state.

## 2026-06-27

- Docs
  - Establish Splash desktop architecture documentation under `docs/controls/desktop/feedback/splash/overview.md`.
  - Add Splash implementation documentation covering visual control, window host, service orchestration, static API and AOT boundaries.
  - Add Splash Token documentation covering startup surface, branding, progress, status and host visual variables.
  - Add Splash control-level changelog.
