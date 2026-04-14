# AtomUIGallery 项目分析文档

> 本文档集对 AtomUI 演示程序（Gallery）进行了全面分析，旨在为后续安全、高效地迭代 Gallery 提供详细、准确的参考。

## 文档索引

| 序号 | 文档 | 内容 |
|------|------|------|
| 1 | [01-Overview.md](01-Overview.md) | 项目概览：项目组成、目录树、依赖关系、构建与运行、启动流程 |
| 2 | [02-Architecture.md](02-Architecture.md) | 架构分析：MVVM 架构、模块划分、核心类职责、控制流、设计模式（含 Mermaid 图） |
| 3 | [03-ShowCaseSystem.md](03-ShowCaseSystem.md) | ShowCase 体系：72 个 ShowCase 清单、路由注册、View 基类模式、布局系统、新增步骤 |
| 4 | [04-WorkspaceSystem.md](04-WorkspaceSystem.md) | Workspace 窗口系统：主窗口、导航面板、菜单系统、窗口行为控制 |
| 5 | [05-LocalizationSystem.md](05-LocalizationSystem.md) | 国际化系统：LanguageProvider + Source Generator 架构、使用方式、资源清单 |
| 6 | [06-IconSystem.md](06-IconSystem.md) | 图标系统：IconProvider + Source Generator 架构、IconGallery 控件、使用方式 |
| 7 | [07-TechStack.md](07-TechStack.md) | 技术栈清单：语言、框架、UI 技术、MVVM 框架、Source Generator、构建工具、发布部署 |
| 8 | [08-CodingConventions.md](08-CodingConventions.md) | 编码规范：命名约定、代码组织、注释规范、设计模式、ReactiveUI 规范、异常处理 |
| 9 | [09-RisksAndNotes.md](09-RisksAndNotes.md) | 风险与注意事项：架构风险、性能风险、硬编码问题、技术债、迭代检查清单、改进建议 |

## 快速导航

### 我想了解...

- **项目整体结构** → [01-Overview.md](01-Overview.md)
- **MVVM 架构和数据流** → [02-Architecture.md](02-Architecture.md)
- **如何新增一个 ShowCase** → [03-ShowCaseSystem.md §7](03-ShowCaseSystem.md#7-新增-showcase-步骤)
- **导航路由机制** → [03-ShowCaseSystem.md §4](03-ShowCaseSystem.md#4-路由注册机制)
- **主窗口菜单和主题切换** → [04-WorkspaceSystem.md](04-WorkspaceSystem.md)
- **如何添加国际化文本** → [05-LocalizationSystem.md §8](05-LocalizationSystem.md#8-新增国际化资源步骤)
- **如何添加自定义图标** → [06-IconSystem.md §7](06-IconSystem.md#7-新增自定义图标步骤)
- **使用了哪些技术栈** → [07-TechStack.md](07-TechStack.md)
- **命名和编码规范** → [08-CodingConventions.md](08-CodingConventions.md)
- **有哪些风险和技术债** → [09-RisksAndNotes.md](09-RisksAndNotes.md)

## 项目关键数据

| 指标 | 数值 |
|------|------|
| .NET 项目数 | 3 |
| ShowCase 总数 | 72 |
| ShowCase 分类 | 6（General/Layout/Navigation/DataEntry/DataDisplay/Feedback） |
| 支持语言 | 2（zh_CN / en_US） |
| 目标框架 | net10.0, net8.0 |
| UI 框架 | Avalonia 11.3.12 |
| MVVM 框架 | ReactiveUI |
| Source Generator | 4 个（Language/Icon/TokenResource/AvaloniaName） |