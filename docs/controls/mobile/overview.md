# AtomUI Mobile Control 文档

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

本目录是 Mobile Control 的公共契约、分类兼容清单和未来单 Control 文档入口。当前仓库没有 Mobile Controls 源码、Theme、
测试或 Gallery 实现，因此 82 项能力只记录已批准设计覆盖范围，不声明可用、已验证或已发布。

## 分类与数量

| 分类 | 职责 | 能力数 |
| --- | --- | ---: |
| [General](general/overview.md) | 高频基础操作和通用视觉原子 | 4 |
| [Layout](layout/overview.md) | 空间、对齐、分区和安全区域 | 6 |
| [Navigation](navigation/overview.md) | 页面、分区、步骤和当前位置切换 | 9 |
| [Data Entry](data-entry/overview.md) | 获取、编辑、选择、验证和提交数据 | 28 |
| [Data Display](data-display/overview.md) | 展示结构化内容、媒体、状态数据和集合 | 11 |
| [Feedback](feedback/overview.md) | 结果、进度、加载、确认和临时覆盖反馈 | 20 |
| [Other](other/overview.md) | 横跨分类且以移动交互基础能力为主 | 4 |
| **总计** | 每项能力只由一个分类维护 | **82** |

Mobile 不创建 `window` 分类。Safe Area、system bars、系统返回和 Host 生命周期属于 Mobile Runtime/Platform；
`ConfigProvider` 也不作为第 83 个 Control，而是映射到现有 Global Token、Theme Algorithm、
`ThemeConfigProvider`、Localization、启动 options、Control 属性、`DataTemplate` 和 ControlTheme。详细映射见
[主题、本地化与 API](../../architecture/systems/mobile/theming-localization-and-api.md)。

## 状态模型

分类清单分别记录：

| 维度 | 当前基线 | 提升条件 |
| --- | --- | --- |
| Design | 能力已纳入总体设计 | 对应控件族设计与 API strategy 已评审 |
| Source | 未实现 | Public API、Theme 和注册入口存在 |
| iOS | 未验证 | 可追溯的 Simulator/真机场景与结果存在 |
| Android | 未验证 | 可追溯的 Emulator/真机场景与结果存在 |
| Release | 未验证 | 对应平台 Release/AOT/trim 产物构建、安装并启动 |
| Publication | 未发布 | NuGet 或应用版本与发布日期可追溯 |

`Tier 1 候选`、`Tier 2 候选` 不是已冻结继承关系；`波次设计未冻结` 也不表示能力未纳入总体设计。iOS 证据不能填充
Android 状态，Headless 不能替代平台验证，普通 build 不能替代 Release。

## 单 Control 文档创建门禁

不得预先创建空的 `docs/controls/mobile/<category>/<control>/`。只有下列证据同时可审查时才创建单 Control 目录：

1. Control 或紧密家族已有 Public API 源码。
2. 已有默认 ControlTheme，或有明确的无 Theme 设计证据。
3. 已有 Contract/Headless 测试入口。
4. Mobile Gallery 已有结构化 API、Token 和稳定 ShowCase 承载位置。
5. iOS 与 Android 的适用验证状态可以分别准确填写。

创建后遵循 [Control 文档规范](../../engineering/contributing/control-documentation-guidelines.md)和
[Mobile 文档规范](../../engineering/contributing/mobile-documentation-guidelines.md)。分类页继续拥有能力阶段状态，单 Control
文档只描述真实当前契约。

## 相关入口

- [Mobile 系统架构](../../architecture/systems/mobile/overview.md)：Viewport、Navigation、Overlay、Gesture、Theme 与验证。
- [Mobile Controls 模块](../../modules/mobile-controls/overview.md)：目标包、源码 ownership、注册、打包和 adapter。
- [运行平台策略](../../architecture/foundations/runtime-platforms.md)：当前 Desktop/Browser 状态与批准的 iOS/Android 架构。
- [Apple iOS 开发环境](../../engineering/platforms/apple-ios-development-environment.md)：当前 iOS 工程环境记录。
- [Gallery 文档](../../gallery/overview.md)：当前 Gallery 边界；Mobile Gallery 尚未实现。

Android 平台操作文档和 Mobile Gallery 专题只有在其正式文档边界建立后进入本页导航；当前 Android、Gallery 和所有发布
状态均保持未验证或未实现。
