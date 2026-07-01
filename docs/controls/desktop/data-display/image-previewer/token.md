# ImagePreviewer Token 设计

本文档定义 ImagePreviewer 相关组件 Token 的专属语义、分类、使用范围和兼容边界。控件 Token 的通用分层、命名、计算、Theme Variables 边界和预设色规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/control-token-guidelines.md)。ImagePreviewer 整体架构见 [ImagePreviewer 桌面版架构设计](overview.md)，内部实现原理见 [ImagePreviewer 桌面版实现原理](implementation.md)，设计和契约变化记录见 [ImagePreviewer Changelog](changelog.md)。

## 1. 定位

ImagePreviewer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ImagePreviewerToken`，scope id 为 `ImagePreviewer`，源码位于 `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerToken.cs`。

## 2. Token 分类

Token 按控件语义分类维护：

| 分类 | 语义 | 代表 Token |
| --- | --- | --- |
| 尺寸与密度 | 控件高度、宽度、图标尺寸、内容最小尺寸。 | `PreviewOperationSize`、`ImagePreviewSwitchSize`、`DialogMinWidth`、`DialogMinHeight`、`CoverImageWidth` |
| 间距与布局 | padding、margin、gap、offset、popup content padding。 | `FloatToolbarPadding`、`FloatToolbarIndicatorPadding` |
| 颜色与状态视觉 | 文本、背景、边框、hover、selected、active、disabled 视觉。 | `PreviewOperationColor`、`PreviewOperationHoverColor`、`PreviewOperationColorDisabled`、`MaskBgColor`、`TitleBarBackgroundColor`、`NavButtonBgColor` |
| 结构与装饰 | 圆角、阴影、指示器、弹层和装饰线相关变量。 | `FloatToolbarIndicatorPadding` |

未出现在上表中的 Token 仍按源码中的组件语义维护，不按代码顺序机械分类。

## 3. 控件专项模型中的 Token 使用

ImagePreviewer 的控件专项模型通过 Theme 消费 Token：

- C# 控件负责状态归一和伪类同步。
- AXAML/ControlTheme 负责把 Token 映射到背景、前景、边框、padding、尺寸和动效。
- Token 默认值从 SharedToken 派生，不直接读取控件实例状态。
- Gallery Token 表应显式维护，不依赖运行时反射扫描。
- 默认 loading/error 占位复用 Skeleton、Spin、SharedToken 和 `CoverImageWidth` 的视觉语义，不为 `Loading`、`Failed` 或网络图片失败新增实例状态 Token。
- `CoverImageWidth` 只能作为没有显式 `CoverWidth` / `CoverHeight` 且没有有效布局约束时的封面占位尺寸兜底；它不表达图片自然尺寸，也不表达失败状态。

## 4. 控件家族影响

调整 ImagePreviewer Token 时必须评估以下范围：

- `ImagePreviewer`
- `ImageGroupPreviewer`
- `ImagePreviewerDialog`
- 对应 Gallery ShowCase 的示例、API 表和 Token 表。
- Light/Dark 主题、Browser/Desktop 主题和 Compact/Form/Popup 集成场景。

## 5. 兼容性要求

- 不删除或重命名已生成的 TokenKind、TokenResource key 和 AXAML 引用。
- 不把实例状态、交互状态或 `EffectiveXxx` 状态写成 Token。
- 不在 Token 中展开颜色、variant 和状态的组合矩阵；组合关系应由 Theme selector 表达。
- 不为默认失败文案、加载中状态或远程图片失败状态新增 Token；文案走语言资源，状态走 `ImagePreviewItemState`，视觉由主题和 SharedToken 表达。
- Token 默认值变更必须同步评估 Gallery 示例和截图可观察外观。
- 如需引入新 Token，必须同步源码、生成文件、Gallery Token 表和本文档。

## 6. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| Token 文档 | `git diff --check`，检查相对链接存在。 |
| Token 默认值 | 运行对应控件测试，走查 Light/Dark 和 Browser 主题。 |
| Token 名称或数量 | 检查 generated TokenResource key、AXAML 引用和 Gallery Token 表。 |
| 主题映射 | 走查 hover、pressed、selected、disabled、loading 等状态视觉。 |
