# Expander 结构化分隔线重构设计

## 1. 目标

在不改变 Expander public/protected API、默认值、稳定 template part、ControlTheme key、资源 key、伪类、Token 名称和 Token 数值的前提下，重构 Header、Content、分隔线和 content motion 的职责，使 Expander 与 Collapse 使用一致的结构化边框模型。

本次重构必须完整支持 `ExpandDirection.Up`、`Down`、`Left` 和 `Right`，并保持现有 Borderless、Ghost、SizeType、自定义 padding、图标位置和触发区域行为。

## 2. 当前根因

当前 `PART_HeaderDecorator` 同时承担 Header 背景、padding 和 Header/Content 分隔线。分隔线厚度由 `HeaderBorderThickness` 根据展开方向计算，颜色再由 `IsExpanded` selector 在透明色和 `ColorBorder` 之间切换。

该模型存在以下结构性问题：

- 分隔线属于 Header，而不是被分隔的 Content 视觉。
- 收起状态仍保留边框厚度，只通过透明颜色隐藏。
- 分隔线同时依赖方向、视觉模式、展开状态和动效时序。
- Header 与 Content motion 分属不同视觉子树，需要额外状态同步才能避免闪烁或缺线。
- 测试只能验证某个时刻的 Brush 结果，无法证明动效期间边框职责稳定。

根因不是某个 selector 的颜色错误，而是分隔线 owner 选择错误。

## 3. 兼容边界

- `Expander` 继续继承 `Avalonia.Controls.Expander`。
- `IsExpanded` 继续作为唯一展开状态 owner。
- 保留 `SizeType`、`IsShowExpandIcon`、`ExpandIcon`、`AddOnContent`、`IsGhostStyle`、`IsBorderless`、`TriggerType`、`ExpandIconPosition`、`HeaderPadding`、`ContentPadding` 和 `IsMotionEnabled`。
- 保留现有稳定 template part 名称，包括 `PART_Frame`、`PART_MainLayout`、`PART_HeaderLayoutTransform`、`PART_HeaderDecorator`、`PART_HeaderLayout`、`PART_ExpandButton`、`PART_HeaderPresenter`、`PART_AddOnContentPresenter`、`PART_ContentMotionActor` 和 `PART_ContentPresenter`。
- 不新增命名 template part，不新增 public/protected API，不改变 Token 数值。
- 可以删除或替换 internal 边框属性，但不能形成新的外部主题契约。

## 4. 视觉结构与职责

目标视觉树：

```text
Expander
└── PixelAlignedBorder#PART_Frame
    └── DockPanel#PART_MainLayout
        ├── LayoutTransformControl#PART_HeaderLayoutTransform
        │   └── PixelAlignedBorder#PART_HeaderDecorator
        │       └── Grid#PART_HeaderLayout
        └── LayoutAwareMotionActor#PART_ContentMotionActor
            └── unnamed PixelAlignedBorder
                └── ContentPresenter#PART_ContentPresenter
```

职责划分：

- `PART_Frame`：继续绘制完整外框、圆角并裁剪整体内容。
- `PART_HeaderDecorator`：只负责 Header 背景和 padding，不再绘制 Header/Content 分隔线。
- unnamed content border：固定绘制 Content 靠近 Header 一侧的分隔线。
- `PART_ContentMotionActor`：包含 content border 和 Content，共同参与显示、裁剪和 motion。

新增的 content border 不命名，避免扩大 template selector 和 template part 契约。

## 5. 分隔线模型

新增 internal `ContentBorderThickness`，只由 `BorderThickness`、`ExpandDirection`、`IsBorderless` 和 `IsGhostStyle` 计算。

默认 bordered 模式映射：

| ExpandDirection | Header 位置 | Content 分隔线 |
| --- | --- | --- |
| `Down` | 上 | `Thickness(0, line, 0, 0)`，Content 顶边 |
| `Up` | 下 | `Thickness(0, 0, 0, line)`，Content 底边 |
| `Left` | 右 | `Thickness(0, 0, line, 0)`，Content 右边 |
| `Right` | 左 | `Thickness(line, 0, 0, 0)`，Content 左边 |

视觉模式：

- 默认 bordered：`PART_Frame` 绘制外框，content border 绘制 Header/Content 分隔线。
- Borderless：外框和 content 分隔线均为零，现有背景规则保持不变。
- Ghost：外框和 content 分隔线均为零，现有背景规则保持不变。

分隔线厚度和 Brush 不依赖 `IsExpanded`、motion 是否运行或 motion completion。收起时 `PART_ContentMotionActor` 隐藏，分隔线随 Content 自然消失。

禁止：

- 在 `IsExpanded` selector 中切换分隔线 Brush 或 Thickness。
- 使用透明 Brush 保留不可见边框占位。
- 为分隔线添加 Thickness/Brush transition。
- 通过 Dispatcher 延迟、状态标志或 motion callback 刷新边框。

## 6. 状态与交互

- `IsExpanded` 是唯一展开状态，继续驱动箭头方向和 content motion 目标。
- Header/Icon 触发语义保持不变，不增加第二套状态或命令协调层。
- `TriggerType=Header` 时 Header 点击切换 `IsExpanded`。
- `TriggerType=Icon` 时只有 `PART_ExpandButton` 切换 `IsExpanded`。
- Disabled 状态继续阻止用户交互，不改变已有 public 行为。
- 边框计算不读取 `IsExpanded`。

## 7. Motion 与生命周期

Content motion 只负责 Content 的布局、裁剪、透明度和最终可见性。content border 位于 motion actor 内，因此与 Content 使用同一个视觉生命周期。

必须保持：

- 新 motion 开始前取消旧 `CancellationTokenSource` 并清理 actor 临时值。
- 快速反向切换时以最新 `IsExpanded` 为最终状态。
- `OnApplyTemplate` 先解绑旧展开按钮、取消旧 motion、清理旧 actor，再获取新 part 并应用稳定状态。
- detach 时取消 motion 并清理 `Height`、`MotionTransform`、`MotionTransformOperations` 和 `Transitions`。
- template reapply 后旧按钮和旧 actor 不再影响当前 Expander。
- motion 开始、反向切换和完成过程中 `ContentBorderThickness` 不变化。

## 8. 性能与资源边界

- 分隔线计算是常量时间，只在边框、方向或视觉模式变化时执行。
- 展开/收起不重新计算边框，不触发额外模板遍历。
- 视觉结构只增加一个静态 `PixelAlignedBorder`，不在 C# 中动态创建视觉。
- 不新增长期缓存、全局 handler、timer、反射、字符串 binding 或 C# relay binding。
- 事件订阅和 cancellation owner 继续由 Expander 实例及 template lifecycle 管理。

## 9. 测试矩阵

### 结构与方向

- Header decorator 的 `BorderThickness` 始终为零。
- `Down` 使用 Content 顶边。
- `Up` 使用 Content 底边。
- `Left` 使用 Content 右边。
- `Right` 使用 Content 左边。
- 运行时切换四种方向时 content border 立即更新到正确侧。

### 视觉模式

- 默认 bordered 模式保留外框和 content 分隔线。
- Borderless 模式外框和 content 分隔线均为零。
- Ghost 模式外框和 content 分隔线均为零。
- 从 Borderless/Ghost 切回默认模式后恢复正确方向的分隔线。

### 状态与 motion

- 收起时 motion actor 不可见，分隔线不单独渲染。
- 展开时 content border 与 Content 同时可见。
- 快速展开、收起、再展开后最终状态匹配最新 `IsExpanded`。
- motion 运行期间 `ContentBorderThickness` 不变化。

### 生命周期

- template reapply 解绑旧按钮。
- template reapply 清理旧 motion actor 临时值。
- detach 清理当前 actor 临时值并取消旧 motion。

### 兼容行为

- Header/Icon 触发语义不变。
- SizeType、自定义 HeaderPadding/ContentPadding、图标位置和四方向布局不变。
- public API、Token、稳定 template part 和资源 key diff 为零。

## 10. 文档与验收

- 更新 Expander `overview.md`、`implementation.md` 和 `changelog.md`。
- 更新由 Expander 文档和主题生成的 LLMS 单控件产物及聚合段落，不带入其他控件生成差异。
- 复用现有 Gallery Direction、Borderless、Ghost 和 Basic 示例完成视觉验收；仅在现有示例无法观察四方向边框时补充示例。
- 定向 Expander 测试、Desktop Controls 构建、Gallery Expander 测试和 `git diff --check` 必须通过。

## 11. 验收标准

- 不存在 `HeaderBorderThickness` 或 Header 状态驱动的分隔线路径。
- 分隔线只由方向、边框厚度和视觉模式决定。
- `IsExpanded` 和 motion 不参与边框计算。
- 四种展开方向的分隔线均位于 Content 靠近 Header 的一侧。
- Borderless/Ghost 不绘制外框和 Content 分隔线。
- 没有新增 public/protected API、命名 template part、Token、伪类或补丁状态。
- Motion、template reapply 和 detach 不保留旧 actor、旧订阅或 cancellation owner。
