# AtomUI 文档

AtomUI 文档按知识责任组织。跨模块系统设计、单个源码模块、具体 Control、工程规范、Gallery 和过程记录分别维护，
同一稳定契约只由一份正式文档拥有。

## 阅读入口

- [整体架构](architecture/index.md)：系统分层、源码包边界和核心运行链路。
- [架构基础](architecture/foundations/index.md)：项目依赖、运行平台、启动注册、构建和打包。
- [源码模块](modules/)：按源码项目或发布包理解职责和实现边界。
- [Control 文档](controls/overview.md)：按平台和类别查看具体 Control 的设计、实现、Token 和变更。
- [工程规范](engineering/)：开发、AOT、诊断、文档、测试和发布规则。
- [Gallery 文档](gallery/)：Gallery 组织、ShowCase 和平台维护说明。
- [Superpowers](superpowers/)：设计过程、实施计划和阶段进度，不作为正式架构的事实来源。

## 文档职责

```text
docs/
|-- index.md
|-- architecture/   # 跨模块、当前有效的系统架构
|-- modules/        # 单个源码项目或发布包
|-- controls/       # 单个 Control 或紧密 Control 家族
|-- engineering/    # 开发规范、工作流和检查清单
|-- gallery/        # AtomUIGallery 工程文档
|-- release-notes/  # 版本 API 变化
|-- AI/             # 当前 LLMS 生成输出
`-- superpowers/    # 设计与实施过程记录
```

`modules/` 与 `controls/` 的边界必须保持清楚：模块文档解释包如何组织、注册和加载；Control 文档解释具体控件的
公共契约、行为、模板、Token 和维护不变量。

## 当前专题

- [异步加载体系](AsyncLoadingArchitecture.md)
- [过滤体系](FilteringArchitecture.md)
- [Popup Anchor 作用域检查](PopupAnchorScopeGuide.md)
- [响应式机制](modules/controls-shared/responsive-system.md)
- [Native 模块](modules/native/overview.md)
- [AOT 编程规范](engineering/aot-programming-guidelines.md)
- [Control 开发规范](engineering/control-development-guidelines.md)
- [Control 文档规范](engineering/control-documentation-guidelines.md)

随着专题完成职责迁移，本入口同步切换到新的 Architecture、Guide、Reference 或 Module 路径，不保留重复副本。
