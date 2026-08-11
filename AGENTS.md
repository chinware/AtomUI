# AtomUI Agent Guide

This file gives AI coding agents the stable entry points for working in AtomUI. Keep this file small. Put concrete, detailed rules in focused documents under `docs/`, then link them here.

## Project Profile

AtomUI is an Ant Design 6 style component system for Avalonia/.NET desktop applications. It contains runtime libraries, optional desktop control packages, source generators, icon/font packages, tests, and the AtomUI Gallery sample application.

```text
AtomUI/
├── src/
│   ├── AtomUI.Native             # Native platform window capabilities
│   ├── AtomUI.Core               # Cross-cutting runtime infrastructure
│   │   ├── Theme                 # Theme manager, token system, palette, styling
│   │   ├── Language              # Localization providers and language resources
│   │   ├── Animations            # Transitions and animation helpers
│   │   ├── MotionScene           # Motion orchestration primitives
│   │   ├── Controls/Icon         # Icon base types and rendering contracts
│   │   ├── Data                  # Resource and binding support infrastructure
│   │   ├── Media                 # Text/media helpers
│   │   ├── Reactive              # Reactive integration helpers
│   │   ├── Reflection            # Explicit compatibility boundary for reflection helpers
│   │   └── Utils                 # Shared low-level utilities
│   ├── AtomUI.Controls.Shared    # Shared contracts and data infrastructure
│   ├── AtomUI.Controls           # Cross-platform base controls for mobile and desktop
│   ├── AtomUI.Desktop.Controls   # Desktop control library
│   │   └── DatePicker            # Representative localized control folder
│   │       ├── DatePicker.cs     # Control implementation
│   │       ├── DatePickerToken.cs
│   │       ├── CalendarView/     # Internal view primitives for the control
│   │       ├── Localization/
│   │       │   ├── en_US.cs
│   │       │   ├── zh_CN.cs
│   │       │   └── zh_TW.cs
│   │       └── Themes/
│   │           ├── DatePickerTheme.axaml
│   │           ├── DatePickerPresenterTheme.axaml
│   │           ├── RangeDatePickerTheme.axaml
│   │           └── DatePickerThemes.axaml
│   ├── AtomUI.Desktop.Controls.DataGrid
│   ├── AtomUI.Desktop.Controls.ColorPicker
│   ├── AtomUI.Desktop.Controls.Extras # Stable supplemental controls beyond Ant Design
│   ├── AtomUI.Toolkits.GalleryBase # Product-neutral Gallery base controls and themes
│   ├── AtomUI.Icons.*            # Icon infrastructure and Ant Design icon package
│   ├── AtomUI.Fonts.*            # Font packages
│   └── AtomUI.Generator          # Roslyn source generators
├── controlgallery/              # AtomUIGallery and desktop host
├── tests/                       # Unit and regression tests
├── docs/                        # Architecture, engineering, Gallery, and control docs
├── build/                       # Build props and supporting scripts
└── resources/                   # Images and static resources
```

Important source boundaries:

- `AtomUI.Core`: theme, token, language, resources, animation, platform infrastructure.
- `AtomUI.Controls.Shared`: cross-control data contracts, collection views, shared state.
- `AtomUI.Controls`: common controls and primitives.
- `AtomUI.Desktop.Controls`: main desktop control package.
- `AtomUI.Desktop.Controls.DataGrid`, `AtomUI.Desktop.Controls.ColorPicker`, and `AtomUI.Desktop.Controls.Extras`: optional desktop packages.
- `AtomUI.Toolkits.GalleryBase`: product-neutral Gallery base controls, models, themes, and runtime helpers.
- `AtomUI.Generator`: Roslyn source generators referenced as analyzers.
- `AtomUIGallery`: Gallery shell, showcases, API tables, token tables, and NativeAOT publish target.

## Instruction Structure

Do not let `AGENTS.md` become a rule dump. When adding durable guidance:

1. Put narrow, concrete constraints in the most relevant document under `docs/`.
2. Link that document from this file only when it is a common entry point.
3. Prefer a new focused document over expanding this file when the rule applies to one subsystem or workflow.
4. Keep this file limited to project context, routing, and non-negotiable collaboration expectations.

## Required Reading

Read the relevant document before touching the corresponding area:

- Overall architecture: [docs/architecture/overview.md](docs/architecture/overview.md)
- Module boundaries: [docs/architecture/foundations/dependency-graph.md](docs/architecture/foundations/dependency-graph.md)
- Localization target architecture: [docs/architecture/systems/localization/overview.md](docs/architecture/systems/localization/overview.md)
- AI collaboration and bug-fix discipline: [docs/engineering/contributing/agent-guidelines.md](docs/engineering/contributing/agent-guidelines.md)
- AOT, trimming, dynamic data, source generators: [docs/engineering/development/aot-programming-guidelines.md](docs/engineering/development/aot-programming-guidelines.md)
- Compiler diagnostics: [docs/engineering/development/compiler-diagnostics-guidelines.md](docs/engineering/development/compiler-diagnostics-guidelines.md)
- Documentation structure and file naming: [docs/engineering/contributing/documentation-structure-guidelines.md](docs/engineering/contributing/documentation-structure-guidelines.md)
- Changelog and release notes: [docs/engineering/contributing/changelog-guidelines.md](docs/engineering/contributing/changelog-guidelines.md)
- Apple iOS development environment: [docs/engineering/platforms/apple-ios/overview.md](docs/engineering/platforms/apple-ios/overview.md)
- Gallery NativeAOT release flow: [docs/engineering/workflows/gallery-aot-release-workflow.md](docs/engineering/workflows/gallery-aot-release-workflow.md)
- Gallery page structure: [docs/gallery/authoring/gallery-showcase-design-pattern.md](docs/gallery/authoring/gallery-showcase-design-pattern.md)
- Gallery organization: [docs/gallery/authoring/organization.md](docs/gallery/authoring/organization.md)
- Resource lifecycle case study: [docs/engineering/case-studies/avalonia-dynamic-resource-memory-leak-case-study.md](docs/engineering/case-studies/avalonia-dynamic-resource-memory-leak-case-study.md)
- Non-Visual AvaloniaObject scoped resource host generator: [docs/modules/generator/scoped-resource-host-generator.md](docs/modules/generator/scoped-resource-host-generator.md)

## Agent Behavior

Detailed AI collaboration rules live in [docs/engineering/contributing/agent-guidelines.md](docs/engineering/contributing/agent-guidelines.md). In this file, keep only the short behavioral contract:

- Understand the affected module before changing code.
- Keep changes scoped to the user request and the ownership boundary.
- Prefer root-cause fixes over trigger-point patches.
- Treat AOT compatibility as a first-class design constraint for new features and bug fixes.
- Verify with tests or publish checks that match the risk of the change.

## Common Commands

Use the command that matches the touched area, and prefer targeted tests while iterating.

```bash
dotnet test tests/AtomUI.Controls.Shared.Tests/AtomUI.Controls.Shared.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUI.Desktop.Controls.DataGrid.Tests/AtomUI.Desktop.Controls.DataGrid.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --framework net10.0 --no-restore
git diff --check
```

For NativeAOT Gallery validation:

```bash
pwsh -NoLogo -NoProfile -File controlgallery/AtomUIGallery.Desktop/scripts/PublishToLocal.ps1 -publishRootPath /tmp/atomui-gallery-aot-run -runtime osx-arm64 -buildType Release -publishAot true
```
