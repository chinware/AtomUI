# 源码模块

本目录按源码项目或发布包描述实现边界、注册入口、关键目录和内部依赖。跨多个项目的稳定模型由
[Architecture](../architecture/overview.md) 维护；具体 Control 的公共行为和模板契约由
[Control 文档](../controls/overview.md) 维护。

| 模块文档 | 对应源码项目或包 | 推荐架构阅读 |
|---|---|---|
| [Native](native/overview.md) | `src/AtomUI.Native` | [运行平台](../architecture/foundations/runtime-platforms.md)、[Windowing](../architecture/systems/windowing/overview.md) |
| [Core](core/overview.md) | `src/AtomUI.Core` | [主题系统](../architecture/systems/theming/overview.md)、[本地化系统](../architecture/systems/localization/overview.md) |
| [Localization](localization/overview.md) | `src/AtomUI.Localization` | [本地化系统](../architecture/systems/localization/overview.md) |
| [Controls.Shared](controls-shared/overview.md) | `src/AtomUI.Controls.Shared` | [Control 基础设施](../architecture/systems/control-infrastructure/overview.md) |
| [Controls](controls/overview.md) | `src/AtomUI.Controls` | [渲染系统](../architecture/systems/rendering/overview.md) |
| [Mobile Controls](mobile-controls/overview.md) | `AtomUI.Mobile.Controls` 预实现目标项目与包；Source 未实现 | [Mobile 系统](../architecture/systems/mobile/overview.md) |
| [Generator](generator/overview.md) | `src/AtomUI.Generator` | [主题系统](../architecture/systems/theming/overview.md)、[本地化系统](../architecture/systems/localization/overview.md) |
| [Fonts](fonts/overview.md) | `src/AtomUI.Fonts.*` | [主题系统](../architecture/systems/theming/overview.md) |
| [Icons](icons/overview.md) | `src/AtomUI.Icons.*` | [整体架构](../architecture/overview.md) |
| [GalleryBase](toolkits-gallery-base/overview.md) | `src/AtomUI.Toolkits.GalleryBase` | [启动与注册](../architecture/foundations/startup-and-registration.md) |

`src/AtomUI.Build.Tasks`、语言包项目和 GalleryBase Generator 作为跨项目系统的一部分，分别由本地化架构和
对应宿主模块说明；当其形成独立发布边界和维护入口时，再新增模块目录。

表中只有 Mobile Controls 使用预实现状态；它不是当前解决方案项目。真实项目落地后，模块文档必须按源码、注册、测试和
发布证据更新，不能只移除状态说明。
