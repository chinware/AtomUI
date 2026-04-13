# Empty 空状态

## 概述

空状态（Empty）是一个用于展示「当前区域暂无数据或内容」的占位控件。当列表、表格、卡片等数据容器中没有可显示的内容时，Empty 提供一个友好的视觉提示，帮助用户理解当前状态并减少空白带来的迷惑感。它是 Ant Design [Empty](https://ant.design/components/empty-cn) 组件的 AtomUI 实现。

AtomUI 的 `Empty` 控件完整复刻了 Ant Design 5.0 Empty 的设计规范，支持两种预设图片（Default / Simple）、自定义图片源、描述文字、三种尺寸等特性。

---

## 设计原理

### Ant Design 的空状态设计哲学

Ant Design 认为「空状态」是界面中不可忽视的重要组成部分。空数据不应该以完全空白的方式呈现——这会让用户困惑「是数据还没加载？是出错了？还是确实没有数据？」。一个好的空状态展示应该：

| 设计维度 | 设计意图 |
|---|---|
| **明确传达** | 通过图片 + 文字清楚告诉用户「这里暂无数据」 |
| **视觉友好** | 使用柔和的插图和灰色调，不抢占视觉焦点，避免给用户造成焦虑 |
| **尺寸灵活** | 支持不同场景的尺寸需求——页面级空状态用大图，表格/列表内嵌用小图 |
| **可定制** | 支持自定义图片和描述文字，适应不同业务语境 |

Ant Design 提供了两种预设图片风格：

| 预设类型 | 设计特征 | 典型用途 |
|---|---|---|
| **Default**（默认） | 彩色插图，尺寸较大，细节丰富 | 页面级空状态、独立空数据提示 |
| **Simple**（简洁） | 灰色线条图，尺寸较小，简约 | 列表/表格内嵌空状态、紧凑场景 |

### Avalonia 对标

Avalonia 框架本身没有内置的 Empty 空状态控件。AtomUI 的 `Empty` 基于 `TemplatedControl` 从零构建，使用 SVG 渲染预设图片，支持主题色动态适配。

### AtomUI 的实现设计

AtomUI 的 Empty 控件采用 **二层继承模型**：

| 层级 | 类 | 职责 |
|---|---|---|
| 基类层 | `AbstractEmpty`（`AtomUI.Controls`） | 设备无关的行为、属性、图片渲染逻辑 |
| 平台层 | `Empty`（`AtomUI.Desktop.Controls`） | 桌面端 Token 注册 |

**核心实现特点：**

- **SVG 动态生成**：预设图片不是静态文件，而是通过 `BuiltInImageBuilder` 在运行时根据当前主题色动态生成 SVG 源码。这使得预设图片能自动适配亮色/暗色主题
- **颜色适配**：预设图片的各个颜色通道（边框色、阴影色、内容色、背景色）均从 `SharedToken` 派生，确保在主题切换时自然过渡
- **三种图片源**：支持预设图片（`PresetImage`）、自定义 SVG 文件路径（`ImagePath`）、自定义 SVG 内容字符串（`ImageSource`）三种互斥方式
- **尺寸系统**：通过 `ISizeTypeAware` 接口支持 Large / Middle / Small 三种尺寸，自动调整图片高度和描述文字间距

---

## 功能详解

### 预设图片

Empty 提供两种内置的预设图片，通过 `PresetImage` 枚举属性选择：

| 枚举值 | 说明 | 视觉特征 |
|---|---|---|
| `PresetEmptyImage.Default` | 默认空状态图 | 彩色，较大，包含多色渐变和阴影细节 |
| `PresetEmptyImage.Simple` | 简洁空状态图 | 灰色线条，较小，简约无填充 |

预设图片通过 `BuiltInImageBuilder` 动态生成 SVG：
- `Default` 图片调用 `BuildDefaultImage(shadowColor, borderColor, borderColorSecondary)`
- `Simple` 图片调用 `BuildSimpleImage(contentColor, borderColor, shadowColor)`

这些颜色参数均从当前主题的 `SharedToken` 派生，确保图片与主题一致。

### 自定义图片

当预设图片不满足需求时，可以使用自定义图片：

- **`ImagePath`**：SVG 文件路径（支持 `avares://` 协议），适合打包在应用资源中的图片
- **`ImageSource`**：SVG 内容字符串，适合动态生成或从网络获取的图片

> ⚠️ `PresetImage`、`ImagePath`、`ImageSource` 三者互斥，同时设置多个会抛出 `ApplicationException`。

### 描述文字

通过 `Description` 属性设置描述文字，默认值通过本地化系统自动设置为 `"No Data"`（中文环境为 `"暂无数据"`）。可通过 `IsShowDescription=False` 隐藏描述文字。

### 尺寸系统

Empty 实现了 `ISizeTypeAware` 接口，支持三种尺寸：

| 尺寸 | 图片高度 Token | 描述文字间距 Token |
|---|---|---|
| `Large` | `EmptyImgHeight`（ControlHeightLG × 2.5） | `DescriptionMargin` |
| `Middle` | `EmptyImgHeightMD`（ControlHeightLG × 1.85） | `DescriptionMarginSM` |
| `Small` | `EmptyImgHeightSM`（ControlHeightLG × 0.875） | `DescriptionMarginSM` |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 默认预设图片 `PRESENTED_IMAGE_DEFAULT` | ✅ | ✅ `PresetImage="Default"` | ✅ 完全对齐 |
| 简洁预设图片 `PRESENTED_IMAGE_SIMPLE` | ✅ | ✅ `PresetImage="Simple"` | ✅ 完全对齐 |
| 描述文字 `description` | ✅ ReactNode | ✅ `Description`（string） | ⚠️ 仅支持字符串，不支持任意 ReactNode |
| 自定义图片 `image` | ✅ ReactNode | ✅ `ImagePath` / `ImageSource`（SVG） | ⚠️ 仅支持 SVG 格式 |
| 图片尺寸 `imageStyle` | ✅ CSSProperties | ✅ `SizeType`（三种预设尺寸） | ⚠️ 不支持任意 CSS，改用枚举尺寸 |
| 主题色适配 | ✅ CSS 变量 | ✅ SVG 动态生成 + SharedToken | ✅ 完全对齐 |
| 暗色模式适配 | ✅ 自动 | ✅ `CalculateTokenValues(isDarkMode)` | ✅ 完全对齐 |
| 底部额外内容 | ✅ `children` slot | ❌ 暂未支持 | ⚠️ 可通过外部组合实现 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Controls.AbstractEmpty
        ├── implements ISizeTypeAware
        ├── [TemplatePart("PART_SvgImage", typeof(Avalonia.Svg.Svg))]
        └── AtomUI.Desktop.Controls.Empty
              └── RegisterTokenResourceScope(EmptyToken.ScopeProvider)
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础设施、`OnApplyTemplate` |
| `AbstractEmpty`（`AtomUI.Controls`） | 图片类型管理、SVG 动态生成、描述文字、尺寸接口、主题色绑定、图片源互斥验证 |
| `Empty`（`AtomUI.Desktop.Controls`） | 桌面端 Token 作用域注册 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/Empty/AbstractEmpty.cs` | 设备无关基类（行为、属性、SVG 渲染） |
| 预设图片 | `src/AtomUI.Controls/Empty/BuiltInImageBuilder.cs` | 内置 SVG 图片动态生成器 |
| 枚举 | `src/AtomUI.Controls/Empty/PresetEmptyImage.cs` | `PresetEmptyImage` 枚举定义 |
| 控件类 | `src/AtomUI.Desktop.Controls/Empty/Empty.cs` | 桌面端 Empty 实现 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Empty/EmptyToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Empty/Themes/EmptyTheme.axaml` | ControlTheme + ResourceDictionary |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/EmptyShowCase.axaml` | 使用范例 |

---

## 模板结构

Empty 的 ControlTemplate 结构简洁，由一个垂直 `StackPanel` 包含图片和描述文字两部分：

```
StackPanel (Orientation=Vertical)
├── Svg (PART_SvgImage)          ← SVG 图片渲染（预设图片或自定义 SVG）
└── TextBlock#Description        ← 描述文字（支持换行）
```

**关键绑定：**

| 模板部件 | 绑定属性 | 说明 |
|---|---|---|
| `Svg#PART_SvgImage` | 动态设置 `Source` / `Path` | 通过 `SetupImage()` 方法根据图片类型设置 |
| `TextBlock#Description` | `{TemplateBinding Description}` | 描述文字内容 |
| `Empty` 自身 | `HorizontalAlignment="Center"`、`VerticalAlignment="Center"` | 默认居中显示 |

**主题色绑定（内部属性 → SharedToken）：**

| 内部属性 | SharedToken 资源键 | 用途 |
|---|---|---|
| `BorderColor` | `ColorFill` | 预设图片边框色 |
| `BorderColorSecondary` | `ColorBorderSecondary` | 预设图片次要边框色 |
| `ShadowColor` | `ColorFillTertiary` | 预设图片阴影色 |
| `ContentColor` | `ColorFillQuaternary` | 预设图片内容区域色 |
| `BgColor` | `ColorBgElevated` | 预设图片背景色（作为混合基色） |

