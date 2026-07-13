# Collapse Ant Design 对齐重构设计

## 1. 目标

在不改变 `Collapse` / `CollapseItem` public API、默认值、稳定 template part、ControlTheme key、资源 key 和现有主题数值的前提下，重构 Collapse 的展开状态、手风琴模式、分隔线和内容动效，使其行为与视觉规则对齐 Ant Design 6 Collapse。

本次设计参考：

- `../ReferenceProjects/ant-design/components/collapse/Collapse.tsx`
- `../ReferenceProjects/ant-design/components/collapse/CollapsePanel.tsx`
- `../ReferenceProjects/ant-design/components/collapse/style/index.ts`
- `@rc-component/collapse` 1.2.x 的 `Collapse.tsx`、`Panel.tsx` 和 `useItems.tsx`

## 2. 兼容边界

- `Collapse` 继续继承 `SelectingItemsControl`。
- `CollapseItem` 继续使用 public `IsSelected` 表达展开状态。
- 保留 `IsAccordion`、`TriggerType`、`ExpandIconPosition`、`IsGhostStyle`、`IsBorderless`、`IsMotionEnabled`、尺寸和 padding API。
- 保留所有现有稳定 template part 名称、ControlTheme key、资源 key、Token 名称和 Token 数值。
- 不新增 public/protected API，不引入 `ActiveKeys`、key/id 或另一套展开状态。
- 可以替换 internal 属性和私有实现，只要不形成外部主题契约变化。

## 3. 状态模型

### 3.1 单一状态源

Avalonia `SelectingItemsControl` 的 selection model 是唯一展开状态 owner。`CollapseItem.IsSelected` 是 selection model 投影到容器的公开状态，也是现有绑定入口。

禁止增加内部 active-key 集合、当前展开项缓存或在 `SelectionChanged` 后再次遍历容器修正状态。内容可见性、箭头方向和动效目标只能从 `IsSelected` 派生。

### 3.2 普通模式

- `SelectionMode` 为 `Multiple | Toggle`。
- 点击未展开项将其加入 selection。
- 点击已展开项将其从 selection 移除。
- 多个 item 可以同时展开。

### 3.3 手风琴模式

- `SelectionMode` 为 `Single | Toggle`。
- 点击未展开项时，selection model 原子地关闭旧项并打开目标项。
- 点击当前展开项时清空 selection，允许所有 item 收起。
- 初始化或运行时从普通模式切换到手风琴模式时，按 Ant Design `activeKey[0]` 语义保留索引最小的已展开项。
- 归一只发生在模式切换或非法外部状态进入时，并通过 selection model 完成；不由容器逐项回写 `IsSelected`。

### 3.4 输入路径

- Header 模式：header 区域、展开图标、Enter/Space 和方向键导航均复用 Avalonia selection 入口。
- Icon 模式：只有展开图标触发展开；header 内容不触发。
- Disabled 状态不改变 selection，且不触发动效。
- Pointer 与 keyboard 不维护独立状态，所有输入最终调用同一 selection 操作。

## 4. 分隔线与圆角模型

分隔线由静态视觉结构拥有，不随 selection 或动画状态重算：

```text
Collapse / PART_Frame：外框、圆角、裁剪
└── CollapseItem shell：非末项底部分隔线
    ├── PART_HeaderDecorator：header 背景与 padding，不承担分隔线
    └── PART_ContentMotionActor
        └── PART_ContentFrame：内容背景、padding、顶部边线
```

### 4.1 默认 bordered 模式

- `PART_Frame` 绘制完整外框并按现有圆角裁剪。
- 每个非末 `CollapseItem` shell 固定绘制底部分隔线。
- `PART_ContentFrame` 固定绘制顶部边线；收起时它随内容视觉一起被裁剪和隐藏。
- 最后一项不绘制 shell 底边，避免与外框底边叠加。

### 4.2 Borderless 模式

- `PART_Frame` 不绘制外框。
- 非末 item 保留底部分隔线。
- 内容区域不绘制顶部边线。
- 颜色、背景和 padding 继续使用现有资源值。

### 4.3 Ghost 模式

- 外框、item 分隔线和内容顶部边线均为零。
- 背景、padding 和圆角资源值不变，只调整边框职责。

### 4.4 禁止的实现

- 不根据 `IsSelected`、`InAnimating` 或 animation completion 改写 header/content 边框厚度。
- 不由父控件订阅 item 动效事件后刷新所有 item 边框。
- 不给边框厚度添加 transition。
- 不通过延迟刷新、Dispatcher 回调或临时标志修复横线闪烁。

## 5. 动效模型

- 内容动效只负责 content 的布局展开、裁剪、透明度和最终可见性。
- 展开时先显示 content，再执行布局动效；收起完成后再隐藏 content。
- 快速反向切换取消前一次动效，以最新 `IsSelected` 为最终目标。
- 边框是 content 或 item shell 自身的一部分，因此随视觉自然运动，不参与独立时序协调。
- template reapply、detach 和新动效开始前取消并释放旧 `CancellationTokenSource`，清除 motion local values。

## 6. 组合与职责

- `Collapse`：items、selection mode、输入路由、容器生成、模式切换归一和 item 位置投影。
- `CollapseItem`：单项 public 内容、`IsSelected` 投影视觉、header 命中、展开按钮和 content motion 生命周期。
- `CollapseTheme.axaml`：外框、圆角和 items presenter。
- `CollapseItemTheme.axaml`：item shell、header、icon、addon、content、三种视觉模式和尺寸资源映射。
- `CollapseToken`：只提供现有视觉值，不保存运行时状态。

不新增 coordinator、store、active-key collection 或 public data abstraction。

## 7. 性能与生命周期

- 模式切换只遍历 selection indexes，不因单项展开刷新全部容器视觉。
- item 位置只在 prepare、index change 和 items collection 结构变化时更新。
- border、background、padding 和 icon placement 使用 AXAML / template binding 表达。
- 不新增运行时反射、字符串 binding、全局事件、timer 或长期缓存。
- 展开按钮事件在 template reapply 时解绑旧 part，item detach 时取消 content motion。

## 8. 测试矩阵

### 状态

- 普通模式允许多项展开和独立收起。
- 手风琴模式打开目标项时关闭旧项。
- 手风琴模式点击当前项后允许全部收起。
- 初始多个 `IsSelected=True` 时保留第一项。
- 普通模式运行时切换手风琴模式时保留第一项。
- 手风琴切回普通模式后可继续多选。
- Items add/remove/replace/reset 后 selection 与容器一致。
- Header/Icon/disabled 输入语义一致。

### 视觉

- 默认模式：外框、非末 item 底线、展开内容顶线正确。
- 收起项的分隔线不依赖 header 状态。
- 最后一项不重复绘制底线。
- Borderless 与 Ghost 的线条规则正确。
- 动画开始、反向切换和结束时边框厚度不变化。
- Small/Middle/Large、icon start/end 和自定义 padding 使用原有资源值。

### 生命周期

- template reapply 不保留旧按钮事件或旧 motion actor。
- detach 和快速切换后不存在运行中的旧 cancellation owner。

## 9. 验收标准

- public API 与主题稳定契约 diff 为零。
- 不存在第二套展开状态或 SelectionChanged 回写循环。
- 手风琴模式所有状态转换由 selection model 完成。
- 横线结果只由 item 位置、视觉模式和固定模板结构决定。
- Collapse 定向测试、Gallery 测试和 `git diff --check` 通过。
