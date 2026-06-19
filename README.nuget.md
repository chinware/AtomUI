## AtomUI

AtomUI is an Ant Design 6 component system for Avalonia/.NET desktop applications. It brings Ant Design's enterprise
interaction patterns, visual language, design tokens and theme customization model to native cross-platform apps on
Windows, macOS and Linux.

### What It Includes

- Production-oriented Avalonia desktop controls for layout, navigation, data entry, feedback and data display.
- Token-driven theming built on Avalonia's style and resource system.
- Ant Design icon packages, font packages and native desktop integration.
- Optional advanced packages such as `AtomUI.Desktop.Controls.DataGrid` and `AtomUI.Desktop.Controls.ColorPicker`.
- Source generators for custom controls, theme tokens and localization infrastructure.

### Install

Install the main desktop controls package first, then add optional packages only when your application needs them.

```bash
dotnet add package AtomUI.Desktop.Controls --version 6.0.5
dotnet add package AtomUI.Desktop.Controls.DataGrid --version 6.0.5
dotnet add package AtomUI.Desktop.Controls.ColorPicker --version 6.0.5
```

### Requirements

- .NET 8 or later
- Avalonia 12.0.x
- Windows, macOS or Linux

### Links

- Repository: https://github.com/AtomUI/AtomUI
- Issues: https://github.com/AtomUI/AtomUI/issues
- Samples: https://github.com/AtomUI/AtomUI.Samples

### License

Projects using AtomUI OSS must comply with LGPL v3. Commercial applications, including internal company software,
personal commercial products and outsourced projects, may use AtomUI for free when linking to the published binaries.
If you customize AtomUI from source code, you must either open source the modified code under the license terms or
purchase a commercial license.
