# AtomUI — Ant Design for Avalonia / .NET

AtomUI leverages Avalonia's robust cross-platform capabilities to implement the Ant Design system for .NET, dedicated to delivering its refined design language and efficient user experience to cross-platform desktop application development.

---

## What is AtomUI?

[Ant Design](https://ant.design/) is an enterprise-class design system originating from Ant Group. It distills complex business scenarios into a coherent set of design values—**Natural**, **Certain**, **Meaningful**, and **Growing**—backed by comprehensive visual specifications, interaction patterns, and a proven component library. Since its introduction, Ant Design has become one of the most widely adopted UI design languages in the industry, powering products at Alibaba, Tencent, Baidu, Meituan, and thousands of other organizations worldwide.

AtomUI brings this design system—specifically the [Ant Design 5.0](https://ant.design/docs/react/introduce) specification—to .NET desktop development through [Avalonia UI](https://avaloniaui.net/), the cross-platform .NET UI framework. It is not a loose visual resemblance; AtomUI is a faithful, specification-compliant reproduction of Ant Design's component behaviors, Design Token architecture, theme algorithms, motion system, and icon set, rebuilt from the ground up in C# and AXAML.

AtomUI is officially recognized as a community implementation on the [Ant Design specification page](https://ant.design/docs/spec/introduce#front-end-implementation).

### At a Glance

- **60+ controls** spanning General, Layout, Navigation, Data Entry, Data Display, and Feedback categories—mirroring the Ant Design component taxonomy.
- **Full Design Token system** — a four-layer token derivation chain (Seed → Map → Alias → Component) equivalent to the Ant Design 5.0 TypeScript implementation.
- **Theme algorithms** — Default (Light), Dark, and Compact algorithms that can be freely composed.
- **Runtime theme switching** — change themes at runtime with zero application restart.
- **Ant Design icon set** — the complete Ant Design icon library, auto-generated from SVG sources.
- **Internationalization** — built-in localization system with English and Chinese out of the box.
- **Cross-platform** — Windows, macOS, and Linux from a single codebase.

---

## Why AtomUI?

### Bringing a Proven Design Language to the Desktop

The web development ecosystem has long enjoyed battle-tested design systems like Ant Design, Material Design, and Fluent UI. Desktop application development on .NET, however, has historically lacked an equivalent. Developers either adopt platform-native styles that differ across operating systems, or invest significant effort in custom design.

AtomUI closes this gap. By implementing the Ant Design 5.0 specification on Avalonia, it offers .NET desktop developers:

- **A mature, opinionated design language** — no more inventing spacing scales, color palettes, or interaction patterns from scratch. Ant Design's decades of enterprise product refinement are encoded directly into the controls.
- **Visual consistency across platforms** — the same application renders an identical UI on Windows, macOS, and Linux, with pixel-level fidelity to the Ant Design specification.
- **Familiar developer experience** — developers who have worked with Ant Design on the web will find the same component names, property semantics, token names, and theme customization concepts in AtomUI.

### Built on Avalonia

[Avalonia](https://avaloniaui.net/) is a cross-platform .NET UI framework with a high-performance rendering engine, a powerful style/template system, and XAML-based declarative UI. AtomUI builds on top of Avalonia rather than replacing it:

- AtomUI controls are standard Avalonia `Control` / `TemplatedControl` subclasses, fully compatible with Avalonia's styling, binding, and layout systems.
- Existing Avalonia knowledge transfers directly—developers continue to use XAML, `StyledProperty`, `ControlTheme`, and the Avalonia DevTools they already know.
- AtomUI can coexist with native Avalonia controls and third-party libraries in the same application.

### Developer Productivity

AtomUI is designed to minimize boilerplate and maximize output:

- **Out of the box** — install the NuGet packages, call `UseAtomUI()`, and start building. No manual style sheets, no theme configuration files required for the default experience.
- **Consistent APIs** — shared interfaces (`ISizeTypeAware`, `IInputControlStyleVariantAware`, `IInputControlStatusAware`) ensure uniform property names and behavior across all controls.
- **Design Token customization** — override a single Seed Token (e.g., `ColorPrimary`) and watch the change cascade through every control automatically. No need to touch individual component styles.
- **Form integration** — the Form system automatically propagates `SizeType`, `StyleVariant`, and validation status to child controls, eliminating repetitive property setting.

---

## Design Philosophy & Principles

### Specification Fidelity

AtomUI treats the Ant Design 5.0 specification as its source of truth. Every control's visual appearance, interaction behavior, spacing, sizing, and motion is derived from the specification rather than approximated. Where the Ant Design React implementation (`antd`) serves as the reference, AtomUI reproduces the same:

- **Component taxonomy** — General, Layout, Navigation, Data Entry, Data Display, Feedback.
- **Size system** — three-tier sizing (Large / Middle / Small) with token-driven height, padding, font size, and border radius per tier.
- **Style variants** — Outlined, Filled, Borderless (and Underlined as an AtomUI extension) for input controls.
- **Motion system** — Fade, Slide, Zoom, Collapse, and Move animation presets with a global enable/disable switch.
- **Color system** — HSV-based 10-color palette generation from Seed colors, matching Ant Design's `@ant-design/colors` algorithm.

### Design Token Architecture

The theme system is the backbone of AtomUI's visual consistency. It faithfully implements Ant Design 5.0's four-layer Design Token architecture:

```
Seed Token  ──(Algorithm)──►  Map Token  ──(Alias calc)──►  Alias Token  ──(Derive)──►  Component Token
  (~20)                        (~100+)                       (~100+)                     (per component)
```

| Layer | Purpose | Example |
|---|---|---|
| **Seed Token** | Designer intent — the minimal set of values that define a brand | `ColorPrimary`, `FontSize`, `BorderRadius` |
| **Map Token** | Algorithm-derived gradients — color palettes, size ladders, font scales | `ColorPrimaryHover`, `FontSizeLG`, `ControlHeightSM` |
| **Alias Token** | Semantic tokens consumed by components in batches | `ColorTextDisabled`, `ColorBgContainer`, `PaddingContentHorizontal` |
| **Component Token** | Per-component overrides scoped by component ID | `ButtonToken.PrimaryColor`, `TagToken.DefaultBg` |

Changing a Seed Token (e.g., setting `ColorPrimary` to your brand color) triggers the theme algorithm to automatically recompute every derived token—across all components, in real time.

### Theme Algorithms

Three composable algorithms expand Seed Tokens into the full Map Token set:

| Algorithm | Effect |
|---|---|
| **Default** | Standard light theme |
| **Dark** | Dark theme with inverted luminance curves |
| **Compact** | Reduced spacing and sizing for information-dense interfaces |

Algorithms can be combined—for example, Dark + Compact produces a dark, information-dense theme. Switching algorithms at runtime triggers a full token recomputation and live UI update.

---

## Technical Architecture

AtomUI is organized into a three-tier, platform-aware architecture designed for maximum code reuse:

```
┌───────────────────────────────────────────────────────────┐
│  Platform Control Layer                                    │
│  AtomUI.Desktop.Controls (+ .ColorPicker, .DataGrid)      │
│  Concrete controls with desktop-specific themes and tokens │
├───────────────────────────────────────────────────────────┤
│  Base Control Layer                                        │
│  AtomUI.Controls                                           │
│  Device-agnostic abstract controls (shared behavior)       │
├───────────────────────────────────────────────────────────┤
│  Foundation Layer                                          │
│  AtomUI.Core — Theme, Tokens, Motion, Animations, i18n    │
│  AtomUI.Controls.Shared — Interfaces, Enums, Converters   │
│  AtomUI.Native — Platform native interop                   │
├───────────────────────────────────────────────────────────┤
│  Shared Resources                                          │
│  AtomUI.Icons.AntDesign — Ant Design icon set              │
│  AtomUI.Fonts.AlibabaSans — Bundled font package           │
│  AtomUI.Generator — Roslyn source generators               │
└───────────────────────────────────────────────────────────┘
```

- **Foundation Layer** (`AtomUI.Core`, `AtomUI.Controls.Shared`, `AtomUI.Native`) — platform-independent infrastructure with zero UI control dependencies. Contains the complete Theme/Token system, animation engine, localization framework, and shared interfaces.
- **Base Control Layer** (`AtomUI.Controls`) — device-agnostic abstract controls that define shared behavior, properties, and pseudo-classes. These serve as base classes for platform-specific implementations.
- **Platform Control Layer** (`AtomUI.Desktop.Controls`) — concrete control implementations for desktop platforms. Each control registers its component token scope and desktop-specific theme. Extension packages (`ColorPicker`, `DataGrid`) are available as opt-in additions.

This layered design ensures that shared logic lives in the base layer while platform-specific theming and interaction patterns are cleanly separated—enabling future platform extensions (e.g., mobile) without duplicating core behavior.

### Key Technical Highlights

| Area | Implementation |
|---|---|
| **Runtime** | .NET 10 (development) / .NET 8 (production); multi-target builds |
| **UI Framework** | Avalonia 11.3.x |
| **Language** | C# (latest, nullable enabled) |
| **Theme Binding** | Custom `SharedTokenResource` / `TokenResource` XAML markup extensions |
| **Code Generation** | Roslyn Source Generators for Token ResourceKey enums and Language ResourceKey enums |
| **Icon System** | 800+ Ant Design icons, auto-generated from SVG via `AtomUI.Icons.AntDesign.Generator` |
| **Font** | Bundled Alibaba Sans via `AtomUI.Fonts.AlibabaSans` |
| **Reactive** | ReactiveUI.Avalonia + System.Reactive |

---

## Component Coverage

AtomUI implements a comprehensive set of Ant Design 5.0 components, organized by the same categories used in the Ant Design documentation:

### General
Button, SplitButton, FloatButton, Icon, Separator

### Layout
Space, CompactSpace, Grid, FlexPanel, BoxPanel, Splitter

### Navigation
Menu, Breadcrumb, Dropdown, Pagination, Steps, TabControl

### Data Entry
AutoComplete, Cascader, CheckBox, ColorPicker, DatePicker, RangeDatePicker, TimePicker, RangeTimePicker, Form, LineEdit, TextArea, Mentions, NumericUpDown, RadioButton, Rate, Select, Slider, Switch, Transfer, TreeSelect, Upload

### Data Display
Avatar, Badge, Calendar, Card, Carousel, Collapse, DataGrid, Descriptions, Empty, Expander, GroupBox, ImagePreviewer, List, InfoFlyout (Popover), QRCode, Segmented, Statistic, Tag, Timeline, Tooltip, Tour, TreeView

### Feedback
Alert, Drawer, Message, Modal, Notification, PopupConfirm (Popconfirm), ProgressBar, CircleProgress, Result, Skeleton, Spin, Watermark

---

## Relationship with the Ant Design Ecosystem

AtomUI is a **community implementation** of the Ant Design specification for the Avalonia / .NET platform. It occupies the same role for .NET desktop development that `antd` (React), `Ant Design Vue`, `NG-ZORRO` (Angular), and `Ant Design Blazor` occupy for their respective ecosystems.

| Ecosystem | Implementation | Platform |
|---|---|---|
| React | `antd` (official) | Web |
| Vue | Ant Design Vue | Web |
| Angular | NG-ZORRO | Web |
| Blazor | Ant Design Blazor | Web (.NET) |
| **Avalonia / .NET** | **AtomUI** | **Desktop (Win/Mac/Linux)** |

AtomUI aligns with Ant Design **5.0**, implementing the Design Token system, theme algorithms, and component API semantics introduced in that major version.

---

## Getting Started

### Install

```bash
dotnet add package AtomUI
```

Or install individual packages for finer control:

| Package | Description |
|---|---|
| `AtomUI.Core` | Core infrastructure — Theme system, Token system, animations |
| `AtomUI.Controls.Shared` | Shared interfaces and enums for control development |
| `AtomUI.Desktop.Controls` | Desktop control library — the main package |
| `AtomUI.Desktop.Controls.DataGrid` | DataGrid control (opt-in) |
| `AtomUI.Desktop.Controls.ColorPicker` | ColorPicker control (opt-in) |
| `AtomUI.Fonts.AlibabaSans` | Alibaba Sans font package |
| `AtomUI.Generator` | Source generators for custom control development |

### Enable

```csharp
public partial class App : Application
{
    public override void Initialize()
    {
        base.Initialize();
        AvaloniaXamlLoader.Load(this);
        this.UseAtomUI(builder =>
        {
            builder.WithDefaultTheme(IThemeManager.DEFAULT_THEME_ID);
            builder.UseAlibabaSansFont();
            builder.UseDesktopControls();
            builder.UseDesktopDataGrid();      // optional
            builder.UseDesktopColorPicker();   // optional
        });
    }
}
```

### Use

```xml
<atom:Window xmlns="https://github.com/avaloniaui"
             xmlns:atom="https://atomui.net"
             xmlns:antdicons="https://atomui.net/icons/antdesign">
    <atom:Space Orientation="Horizontal">
        <atom:Button ButtonType="Primary">Get Started</atom:Button>
        <atom:Button Icon="{antdicons:AntDesignIconProvider StarOutlined}">Star on GitHub</atom:Button>
    </atom:Space>
</atom:Window>
```

### Explore

Run the built-in Control Gallery to see every control in action:

```bash
git clone https://github.com/AtomUI/AtomUI.git
cd AtomUI
dotnet run --project controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj
```

Or visit [AtomUI.Samples](https://github.com/AtomUI/AtomUI.Samples) for minimal starter projects.

---

## License

AtomUI is released under the **LGPL v3** license. Commercial applications using AtomUI via binary linking are free of charge. Source-level modifications require either open-sourcing the modified code or obtaining a commercial license. For commercial licensing inquiries, please contact [Beijing Qinware Software Technology Co., Ltd.](https://github.com/AtomUI/AtomUI)

