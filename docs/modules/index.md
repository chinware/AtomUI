# 源码模块

本目录按源码项目或发布包描述实现边界、注册入口、关键目录和内部依赖。跨多个项目的稳定模型由
[Architecture](../architecture/index.md) 维护；具体 Control 的公共行为和模板契约由
[Control 文档](../controls/index.md) 维护。

| 模块文档 | 对应源码项目或包 | 推荐架构阅读 |
|---|---|---|
| [Native](native/index.md) | `src/AtomUI.Native` | [运行平台](../architecture/foundations/runtime-platforms.md)、[Windowing](../architecture/systems/windowing/index.md) |
| [Core](core/index.md) | `src/AtomUI.Core` | [主题系统](../architecture/systems/theming/index.md)、[本地化系统](../architecture/systems/localization/index.md) |
| [Localization](localization/index.md) | `src/AtomUI.Localization` | [本地化系统](../architecture/systems/localization/index.md) |
| [Controls.Shared](controls-shared/index.md) | `src/AtomUI.Controls.Shared` | [Control 基础设施](../architecture/systems/control-infrastructure/index.md) |
| [Controls](controls/index.md) | `src/AtomUI.Controls` | [渲染系统](../architecture/systems/rendering/index.md) |
| [Generator](generator/index.md) | `src/AtomUI.Generator` | [主题系统](../architecture/systems/theming/index.md)、[本地化系统](../architecture/systems/localization/index.md) |
| [Fonts](fonts/index.md) | `src/AtomUI.Fonts.*` | [主题系统](../architecture/systems/theming/index.md) |
| [Icons](icons/index.md) | `src/AtomUI.Icons.*` | [整体架构](../architecture/index.md) |
| [GalleryBase](toolkits-gallery-base/index.md) | `src/AtomUI.Toolkits.GalleryBase` | [启动与注册](../architecture/foundations/startup-and-registration.md) |

`src/AtomUI.Build.Tasks`、语言包项目和 GalleryBase Generator 作为跨项目系统的一部分，分别由本地化架构和
对应宿主模块说明；当其形成独立发布边界和维护入口时，再新增模块目录。
