# Splash Token 设计

本文档定义 Splash 相关控件 Token 的专属语义、分类、使用范围和兼容边界。控件 Token 的通用分层、命名、计算、Theme Variables 边界和预设色规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/control-token-guidelines.md)。Splash 整体架构见 [Splash 桌面版架构设计](overview.md)，内部实现原理见 [Splash 桌面版实现原理](implementation.md)，设计和契约变化记录见 [Splash Changelog](changelog.md)。

## 1. 定位

Splash Token 只表达组件级视觉变量，例如窗口尺寸、内容间距、品牌尺寸、文字规格、进度区域间距、圆角、阴影和状态色。Token 不承载启动步骤、`Status`、`Progress`、`IsIndeterminate`、异常对象、主窗口引用或服务状态。

当前 Token scope：

- `SplashToken`，scope id 为 `Splash`，源码位于 `src/AtomUI.Desktop.Controls.Extras/Splash/SplashToken.cs`。

## 2. Token 分类

Token 按控件语义分类维护：

| 分类 | 语义 | 代表 Token |
| --- | --- | --- |
| 窗口与表面 | 启动窗口默认尺寸、最小尺寸、圆角、阴影和背景。 | `WindowWidth`、`WindowMinHeight`、`SurfaceCornerRadius`、`SurfaceBoxShadow`、`SurfaceBackground` |
| 间距与布局 | 内容 padding、品牌区间距、状态区间距、底部区域间距。 | `ContentPadding`、`ContentGap`、`ProgressMarginTop`、`FooterMarginTop` |
| 品牌与标题 | Logo 尺寸、标题字号、标题行高、副标题视觉。 | `LogoSize`、`TitleFontSize`、`TitleLineHeight`、`SubtitleFontSize` |
| 状态文本 | 消息、详情和弱文本视觉。 | `MessageFontSize`、`DetailFontSize`、`SubtleForeground` |
| 进度与指示器 | 不确定加载指示器尺寸和确定进度条高度。 | `IndicatorSize`、`ProgressBarHeight` |
| 状态色 | 成功、错误和强调状态视觉。 | `SuccessColor`、`ErrorColor`、`AccentColor` |

未出现在上表中的 Token 仍按源码中的组件语义维护，不按代码顺序机械分类。

## 3. 控件专项模型中的 Token 使用

Splash 的控件专项模型通过 Theme 消费 Token：

- C# 控件负责 `Status`、`Progress`、`IsIndeterminate` 的状态归一和伪类同步。
- AXAML/ControlTheme 负责把 Token 映射到背景、前景、边框、padding、尺寸、阴影和动效。
- `SplashTokenResource` 是 Splash ControlTheme 的统一读取入口：它同时解析 Splash 的 Effective Global Token 和 Splash Own Token。
- 标题使用 Effective Global Token `ColorTextHeading`，普通消息使用 Effective Global Token `ColorText`；这两个语义不在 `SplashToken` 中重复定义 Own Token。
- 副标题和详情使用 Splash Own Token `SubtleForeground`；表面、间距、尺寸、阴影和状态色继续使用对应 Splash Own Token。
- Token 默认值从 Splash Effective Global Token 派生，不直接读取控件实例状态。
- `SuccessColor` 和 `ErrorColor` 表达状态语义色来源，不保存当前实例状态。
- Success/Error 状态 selector 分别使用 `SuccessColor` 和 `ErrorColor`，普通消息的 `ColorText` 覆盖不得替代状态色。
- Token 类型、生成数据和 token.md 应显式维护，不依赖运行时反射扫描。

资源消费边界：

| 资源类别 | Theme 入口 | 正式配置边界 |
| --- | --- | --- |
| Splash Effective Global Token | `SplashTokenResource` | 通过 `ThemeConfig` 的 Splash Control 配置覆盖对应 Global Token。 |
| Splash Own Token | `SplashTokenResource` | 通过 `ThemeConfig` 的 Splash Control 配置覆盖对应 Own Token。 |
| 真正的 Global Token snapshot | `SharedTokenResource` | 通过全局 `ThemeConfig` 配置；仅用于明确不响应 Splash Control 级覆盖的共享值。 |

系统级 Token 调整统一通过 `ThemeConfig` 进入主题编译流程。完整的应用专属启动窗口视觉由专用 `SplashWindow`、内部 Splash 子控件及其自有 AXAML `Styles` 表达；业务 C# 不得手工拼装 Token 资源键、向窗口资源字典写入此类覆盖或构造运行时 Style。

## 4. 控件家族影响

调整 Splash Token 时必须评估以下范围：

- `Splash`
- `SplashWindow`
- `SplashService` 默认选项中的尺寸和动效默认值。
- `AtomUIExtrasThemesProvider` 的主题注册顺序。
- 对应 Gallery ShowCase 的示例和源码片段。
- Light/Dark 主题、不同 DPI、窗口阴影和启动失败状态。

## 5. 兼容性要求

- 不删除或重命名已生成的 TokenKind、TokenResource key 和 AXAML 引用。
- 不把实例状态、交互状态、启动步骤或 `EffectiveXxx` 状态写成 Token。
- 不在 Token 中展开状态组合矩阵；状态关系应由 Theme selector 表达。
- 不为已有 Global Token 语义重复增加 `TitleForeground`、`MessageForeground` 等 Splash Own Token；使用 Splash Effective Global Token 保持单一真源。
- 不用 `SharedTokenResource` 绕过 Splash identity 读取需要响应 Splash Control 级覆盖的 Global Token。
- Token 默认值变更必须同步评估 Gallery 示例和截图可观察外观。
- 如需引入新 Token，必须同步 Token 类型、生成资源、主题引用和本文档。

## 6. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| Token 文档 | `git diff --check`，检查相对链接存在。 |
| Token 默认值 | 运行 Extras 控件测试，走查 Light/Dark、错误状态和确定/不确定进度。 |
| Token 名称或数量 | 检查 generated TokenResource key、AXAML 引用和 token.md。 |
| 主题映射 | 走查品牌区、状态区、进度区、底部区和 SplashWindow 表面视觉。 |
