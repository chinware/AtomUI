# ProgressBar 进度条

## 概述

进度条（ProgressBar）用于展示操作的当前进度和状态，将抽象的进度数值转化为直观的视觉反馈。AtomUI 提供四种进度条变体，覆盖不同的使用场景：

| 控件 | 说明 | 适用场景 |
|---|---|---|
| `ProgressBar` | 线形进度条（水平/垂直） | 文件上传、表单提交、批量操作等通用场景 |
| `CircleProgress` | 圆形进度条 | 仪表板卡片、统计展示、紧凑空间 |
| `DashboardProgress` | 仪表盘进度条（可设置缺口） | 仪表盘面板、数据可视化 |
| `StepsProgressBar` | 步骤进度条（分段块状） | 步骤流程、分阶段任务进度 |

AtomUI 的进度条控件完整复刻了 [Ant Design 5.0 Progress](https://ant.design/components/progress-cn) 的设计规范。

---

## 设计原理

### Ant Design 的进度条设计哲学

Ant Design 对进度条的设计遵循三个核心原则：

1. **状态可视化** — 将抽象的进度数值转化为直观的视觉反馈，用户无需阅读具体数字即可感知任务完成程度
2. **多种形态** — 线形（Line）、圆形（Circle）、仪表盘（Dashboard）、步骤（Steps）满足不同场景的空间和语义需求
3. **状态语义** — 通过颜色传达 Normal（蓝色/默认）、Success（绿色/完成）、Exception（红色/异常）、Active（动画/进行中）四种语义

**四种状态：**

| 状态 | 颜色语义 | 说明 |
|---|---|---|
| `Normal` | 主色（`ColorInfo`） | 默认进行中状态 |
| `Success` | 成功色（`ColorSuccess`） | 任务完成，自动在 `Value == Maximum` 时触发 |
| `Exception` | 错误色（`ColorError`） | 任务异常/失败 |
| `Active` | 主色 + 动画 | 活跃进行中（线形进度条专属） |

### Avalonia RangeBase 基础能力

所有进度条控件最终继承自 Avalonia 的 `RangeBase`，它提供了 `Value`、`Minimum`、`Maximum` 三个核心属性来定义值域范围。

**RangeBase 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Value` | 当前进度值 |
| `Minimum` | 最小值（默认 0） |
| `Maximum` | 最大值（默认 100） |

### AtomUI 的分层扩展设计

AtomUI 的进度条采用了精心设计的多层抽象继承体系，确保代码最大化复用：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **多种形态** | 4 个独立控件类 | 线形/圆形/仪表盘/步骤各有独立模板 |
| **水平/垂直方向** | `Orientation` 属性 | 线形和步骤进度条支持两个方向 |
| **进度文字** | `IsShowProgressInfo` + `ProgressTextFormat` | 可定制进度百分比文字格式 |
| **百分比位置** | `PercentPosition` 结构体 | 文字可在进度条外侧（Start/Center/End）或内部 |
| **状态** | `Status` 枚举 | Normal/Success/Exception/Active 语义化颜色 |
| **成功阈值** | `SuccessThreshold` + `SuccessStrokeBrush` | 达到阈值后部分区域使用成功色 |
| **不定态** | `IsIndeterminate` | 无法确定进度时的动画反馈 |
| **自定义线宽** | `IndicatorThickness` | 自定义进度条粗细 |
| **端点样式** | `StrokeLineCap` (Round/Flat/Square) | 圆角/方角端点 |
| **三种尺寸** | `ISizeTypeAware` | Large/Middle/Small 自动计算线宽 |
| **步进段** | `StepCount` + `StepGap` | 圆形/仪表盘支持分段显示 |
| **自定义颜色** | `StrokeBrush` + `TrailColor` | 支持纯色和渐变画刷 |
| **Design Token** | `ProgressBarToken` | 所有视觉值从 Token 派生 |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 线形 `type="line"` | ✅ | ✅ `ProgressBar` | ✅ 完全对齐 |
| 圆形 `type="circle"` | ✅ | ✅ `CircleProgress` | ✅ 完全对齐 |
| 仪表盘 `type="dashboard"` | ✅ | ✅ `DashboardProgress` | ✅ 完全对齐 |
| 步骤 `steps` | ✅ | ✅ `StepsProgressBar` | ✅ 完全对齐 |
| 百分比 `percent` | ✅ | ✅ `Value` (0–100) | ✅ 完全对齐 |
| 状态 `status` | ✅ normal/success/exception/active | ✅ `Status` 枚举 | ✅ 完全对齐 |
| 描边颜色 `strokeColor` | ✅ 支持渐变 | ✅ `StrokeBrush`（支持 IBrush） | ✅ 完全对齐 |
| 轨道颜色 `trailColor` | ✅ | ✅ `TrailColor` | ✅ 完全对齐 |
| 线帽 `strokeLinecap` | ✅ round/butt | ✅ `StrokeLineCap` | ✅ 完全对齐 |
| 进度信息 `showInfo` | ✅ | ✅ `IsShowProgressInfo` | ✅ 完全对齐 |
| 进度格式 `format` | ✅ 函数 | ✅ `ProgressTextFormat`（格式字符串） | ⚠️ 格式字符串替代函数 |
| 进度条粗细 `strokeWidth` | ✅ | ✅ `IndicatorThickness` | ✅ 完全对齐 |
| 百分比位置 `percentPosition` | ✅ 5.18.0+ | ✅ `PercentPosition` 结构体 | ✅ 完全对齐 |
| 成功段 `success.percent` | ✅ | ✅ `SuccessThreshold` + `SuccessStrokeBrush` | ✅ 完全对齐 |
| 缺口位置 `gapPosition` | ✅ | ✅ `DashboardGapPosition` | ✅ 完全对齐 |
| 缺口角度 `gapDegree` | ✅ | ✅ `GapDegree` | ✅ 完全对齐 |
| 三种尺寸 `size` | ✅ | ✅ `SizeType` | ✅ 完全对齐 |
| 步骤数 `steps` | ✅ | ✅ `Steps` / `StepCount` | ✅ 完全对齐 |
| 方向 | ❌ 仅水平 | ✅ `Orientation`（水平+垂直） | 🔥 AtomUI 额外支持 |

---

## 继承关系

### 核心继承链

```
Avalonia.Controls.Primitives.RangeBase
  └── AtomUI.Controls.Commons.AbstractProgressBar (ISizeTypeAware, IMotionAwareControl)
        ├── AtomUI.Controls.Commons.AbstractLineProgress
        │     ├── AtomUI.Controls.Commons.AbstractGeneralProgressBar
        │     │     └── AtomUI.Desktop.Controls.ProgressBar           ← 线形进度条
        │     └── AtomUI.Controls.Commons.AbstractGeneralStepsProgressBar
        │           └── AtomUI.Desktop.Controls.StepsProgressBar      ← 步骤进度条
        └── AtomUI.Controls.Commons.AbstractCircleProgress
              ├── AtomUI.Controls.Commons.AbstractGeneralCircleProgress
              │     └── AtomUI.Desktop.Controls.CircleProgress        ← 圆形进度条
              └── AtomUI.Controls.Commons.AbstractGeneralDashboardProgress
                    └── AtomUI.Desktop.Controls.DashboardProgress     ← 仪表盘进度条
```

### 各层级职责划分

| 层级 | 提供的能力 |
|---|---|
| `RangeBase` (Avalonia) | `Value` / `Minimum` / `Maximum` 值域管理 |
| `AbstractProgressBar` | 通用进度逻辑：百分比计算、状态管理、进度文字、尺寸感知、描边颜色、轨道颜色、线帽、伪类、过渡动画 |
| `AbstractLineProgress` | 线形方向（`Orientation`）、水平/垂直伪类、尺寸阈值计算、默认图标（CloseCircleFilled/CheckCircleFilled） |
| `AbstractCircleProgress` | 圆形尺寸计算、步进段（`StepCount`/`StepGap`）、自适应字体/图标大小、默认图标（CloseOutlined/CheckOutlined） |
| `AbstractGeneralProgressBar` | 线形渲染（凹槽+指示器）、百分比位置（`PercentPosition`）、内外标签布局、成功阈值渲染 |
| `AbstractGeneralCircleProgress` | 圆形渲染（椭圆/弧线）、步进段渲染、角度计算 |
| `AbstractGeneralDashboardProgress` | 仪表盘渲染（带缺口弧线）、`DashboardGapPosition` + `GapDegree` |
| `AbstractGeneralStepsProgressBar` | 步骤块渲染、`Steps`/`ChunkWidth`/`ChunkHeight`、分段着色 |
| 桌面具体类 (`ProgressBar` 等) | 仅注册 `ProgressBarToken.ScopeProvider` Token 作用域 |

### 实现的共享接口

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸，自动调整线宽 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持进度值过渡动画、颜色渐变动画 |

---

## 源码位置

### 设备无关层（AtomUI.Controls）

| 文件路径 | 说明 |
|---|---|
| `src/AtomUI.Controls/ProgressBar/AbstractProgressBar.cs` | 所有进度条的抽象基类 |
| `src/AtomUI.Controls/ProgressBar/AbstractLineProgress.cs` | 线形进度条抽象基类（水平/垂直） |
| `src/AtomUI.Controls/ProgressBar/AbstractGeneralProgressBar.cs` | 通用线形进度条（含渲染） |
| `src/AtomUI.Controls/ProgressBar/AbstractCircleProgress.cs` | 圆形进度条抽象基类 |
| `src/AtomUI.Controls/ProgressBar/AbstractGeneralCircleProgress.cs` | 通用圆形进度条（含渲染） |
| `src/AtomUI.Controls/ProgressBar/AbstractGeneralDashboardProgress.cs` | 仪表盘进度条（含渲染） |
| `src/AtomUI.Controls/ProgressBar/AbstractGeneralStepsProgressBar.cs` | 步骤进度条（含渲染） |
| `src/AtomUI.Controls/ProgressBar/ProgressBarEnums.cs` | 枚举定义（ProgressStatus 等） |
| `src/AtomUI.Controls/ProgressBar/ProgressBarPseudoClass.cs` | 伪类常量 |

### 桌面平台层（AtomUI.Desktop.Controls）

| 文件路径 | 说明 |
|---|---|
| `src/AtomUI.Desktop.Controls/ProgressBar/ProgressBar.cs` | 线形进度条 |
| `src/AtomUI.Desktop.Controls/ProgressBar/CircleProgress.cs` | 圆形进度条 |
| `src/AtomUI.Desktop.Controls/ProgressBar/DashboardProgress.cs` | 仪表盘进度条 |
| `src/AtomUI.Desktop.Controls/ProgressBar/StepsProgressBar.cs` | 步骤进度条 |
| `src/AtomUI.Desktop.Controls/ProgressBar/ProgressBarToken.cs` | 组件级 Design Token |
| `src/AtomUI.Desktop.Controls/ProgressBar/Themes/` | ControlTheme 文件 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ProgressBarShowCase.axaml` |

---

## 渲染机制

所有进度条都采用 **自定义渲染**（override `Render`）而非模板内控件组合，直接使用 `DrawingContext` 绘制凹槽（Groove）和指示器（Indicator）。这种设计的优势：

- **性能优秀**：无需额外的可视化树节点
- **精确控制**：端点圆角、弧线角度等视觉细节可像素级控制
- **渐变支持**：`StrokeBrush` 支持 `LinearGradientBrush` 等复杂画刷

### 线形进度条渲染流程

1. 计算凹槽矩形（`_grooveRect`），考虑百分比标签位置的偏移
2. 使用 `GrooveBrush` 绘制凹槽背景（Round → 胶囊形，Flat → 矩形）
3. 根据 `Percentage` 计算指示器矩形（从凹槽右侧/底部缩进）
4. 使用 `StrokeBrush` 绘制指示器
5. 如有 `SuccessThreshold`，再叠加绘制成功阈值段

### 圆形/仪表盘渲染流程

1. 计算圆形区域（正方形），使用 `DrawEllipse` 或 `DrawArc` 绘制凹槽环
2. 根据 `IndicatorAngle` 使用 `DrawArc` 绘制指示器弧线
3. 如有 `StepCount`，将圆环分段绘制

### 完成/异常图标

当进度达到 `Maximum` 或 `Status == Exception` 时，百分比文字会替换为图标：

| 进度条类型 | 完成图标 | 异常图标 |
|---|---|---|
| 线形 / 步骤 | `CheckCircleFilled` ✅ | `CloseCircleFilled` ❌ |
| 圆形 / 仪表盘 | `CheckOutlined` ✓ | `CloseOutlined` ✗ |
