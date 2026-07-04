# GroupBox 桌面版实现原理

本文档描述 GroupBox 桌面版的内部模板接入、自绘边框、Header 缺口几何和维护边界。公共设计与 API 契约见 [GroupBox 桌面版架构设计](overview.md)，Token 语义见 [GroupBox Token 设计](token.md)，变化记录见 [GroupBox Changelog](changelog.md)。

## 1. 实现定位

GroupBox 的实现重点是以较少视觉层级表达 fieldset 式 Header 缺口边框，并支持透明或半透明背景。实现文档只描述模板 part、测量数据、渲染缓存和失效条件。

GroupBox 不实现交互状态，不管理子项集合，不承担折叠或 Form 逻辑。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/GroupBox/GroupBox.cs`：公共 API、template part 获取、测量 bounds、渲染和失效逻辑。
- `src/AtomUI.Desktop.Controls/GroupBox/GroupBoxToken.cs`：GroupBox 组件 Token。
- `src/AtomUI.Desktop.Controls/GroupBox/Themes/GroupBoxTheme.axaml`：模板结构、Header 对齐、TokenResource 引用和默认视觉属性。

## 3. 核心类职责

`GroupBox` 继承 `ContentControl`，负责 Header API、Content 承载和自绘边框。它从模板 part 获取 Header 与 Frame 的 bounds，并在 `Render` 中按有效几何绘制背景和边框。

`GroupBoxToken` 提供内容内边距、Header 外边距、Header 内容内边距和图标间距等组件级语义值。

`GroupBoxTheme.axaml` 负责提供可测量的 Header 结构，不负责遮挡边框线。

## 4. 状态与数据流

渲染状态流：

```text
Header public properties
  HeaderTitle / HeaderIcon / HeaderTitlePosition / font properties
      ↓
Template measure
  PART_Frame bounds
  PART_HeaderContent bounds
      ↓
Render input
  background / border brush / border thickness / corner radius
  header gap rect
      ↓
Draw background + border geometry with gap excluded
```

Header 内容变化、字体变化、icon 可见性变化和标题位置变化会影响 Header bounds。边框厚度、圆角、背景和边框 brush 变化会影响绘制几何。

## 5. 生命周期与模板接入

`OnApplyTemplate` 获取以下 template part：

- `PART_Frame`
- `PART_HeaderContainer`
- `PART_HeaderContent`
- `PART_HeaderIconPresenter`
- `PART_HeaderPresenter`
- `PART_ContentPresenter`

模板接入后，GroupBox 依赖 layout pass 产生 `PART_HeaderContent` 与 `PART_Frame` 的有效 bounds。bounds 变化后需要使渲染缓存失效并触发重绘。

更换模板时必须清理旧 part 引用，避免旧视觉节点 bounds 被继续用于缺口计算。

`MeasureOverride` 必须测量 `PART_Frame`，而不是只依赖 `ContentControl` 默认内容测量。`PART_Frame` 包含 Header 容器、`PART_ContentPresenter` 和由 Token 注入的内容内边距，因此它的 `DesiredSize` 才能完整表达 GroupBox 自动高度。该设计保证未设置显式高度时，内容增多会推动 GroupBox 高度增长，而不会被 Header 通道、边框或内容内边距挤压。

## 6. 交互与事件处理

GroupBox 不订阅 pointer、keyboard、focus、drag/drop 或 command 事件。Header 不是按钮，也不是折叠触发器。

与实现相关的事件只来自模板 part 尺寸变化或属性变化。Header bounds、Frame bounds、边框厚度和圆角变化应触发布局或渲染失效。

## 7. 内部算法与关键流程

Header 缺口渲染流程：

1. 绘制内容区域背景，背景允许透明、半透明和普通实色。
2. 从 `PART_HeaderContent` 获取相对 GroupBox 的实际位置和尺寸。
3. 按 `PART_Frame` bounds、`BorderThickness` 和 `CornerRadius` 建立边框几何。
4. 从边框几何中排除 Header gap 几何。
5. 绘制最终边框区域。

该模型避免把 Header 背景色作为遮挡层，因此透明背景下标题区域不会露出多余边框短线。

缓存策略：

- 尺寸、边框厚度、圆角或 Header bounds 变化时重建几何。
- 普通重绘复用缓存。
- DPI 半像素对齐应在几何构建阶段处理，避免边框断裂或重叠。

自动高度测量流程：

1. `MeasureOverride` 测量 `PART_Frame`。
2. `PART_Frame` 内部的 DockPanel 同时测量 Header 容器和内容 Presenter。
3. `PART_ContentPresenter` 将 `Padding` 作为 Margin 应用于内容区域。
4. GroupBox 将模板根节点 `DesiredSize` 返回给父布局，使父容器按完整 fieldset 高度分配空间。

## 8. 资源、性能与 AOT 边界

GroupBox 渲染应使用 Avalonia 绘制 API 和稳定 template part，不使用反射读取模板内部状态。

Header 缺口通过几何排除实现，不增加遮挡用背景层。该方式保持 VisualTree 简洁，也避免透明背景下依赖父背景颜色。

Token 通过动态资源进入 Theme，不应在 `Render` 中主动查找全局资源。

## 9. 维护不变量

内部重构必须保持以下不变量：

- Header 缺口计算基于 `PART_HeaderContent` 的实际 bounds。
- `PART_Frame` 保持边框绘制的布局参考。
- `PART_Frame` 必须参与 GroupBox 测量，自动高度不能退化为只测量裸 Content。
- 不用 Header 背景遮挡边框线来模拟缺口。
- 透明背景、半透明背景和普通背景走同一渲染模型。
- Header 图标隐藏时不保留额外图标占位。
- GroupBox 不新增点击、折叠或选择行为。
- Token 只表达布局和视觉默认值，不承载实例 bounds 或渲染缓存。

## 10. 测试与验证

验证范围：

- 透明背景下 Header 标题下方无多余边框线。
- 普通背景、半透明背景和复杂父背景下缺口稳定。
- Header `Left`、`Center`、`Right` 三种位置缺口正确。
- 带图标和无图标时 Header 宽度、缺口宽度和内容布局正确。
- 未设置显式高度时，GroupBox 高度包含 Header、内容内边距和内容自身期望高度。
- 不同边框厚度、圆角和 DPI 下边框不断裂。
- Token 表、API 表和 Gallery 示例与控件实现一致。
- 文档改动运行 `git diff --check`。
