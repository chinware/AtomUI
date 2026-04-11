# AtomUI WaveSpirit（波浪动画）系统

> 本文档描述 AtomUI 波浪动画系统（WaveSpirit）的设计原理、实现机制及开发者使用指南。WaveSpirit 是 Ant Design 5.0 中按钮、复选框、单选按钮等控件在点击时产生的涟漪/波纹扩散效果的完整 C# 实现。

---

## 文档索引

| 文档 | 内容 |
|---|---|
| [架构设计](./Architecture.md) | 系统总体架构、核心类型、Painter 机制、动画管线及各层组件职责说明 |
| [开发者指南](./DeveloperGuide.md) | 面向控件开发者的集成指南，包含 Button、CheckBox、RadioButton、ToggleSwitch 等完整示例 |

---

## 概述

WaveSpirit 是 AtomUI 中实现 Ant Design 点击波纹效果的动画子系统。当用户点击按钮、选中复选框或切换开关时，控件边缘会产生一个向外扩散并逐渐消失的波纹动画，为用户操作提供即时的视觉反馈。

### 设计目标

1. **对齐 Ant Design 5.0**：精确复现 Ant Design 的 Wave 效果，包括圆角矩形波纹、圆形波纹和药丸形波纹三种形态。
2. **Token 驱动**：波纹颜色、范围、透明度、动画时长等所有参数均通过 Design Token 系统配置，不硬编码任何值。
3. **可全局控制**：通过 `EnableWaveSpirit` Seed Token 可一键开启/关闭全局波浪动画，`ThemeManager` 提供运行时切换。
4. **跨平台抽象**：核心接口和 Painter 定义在基础共享层（`AtomUI.Controls.Shared`），`WaveSpiritDecorator` 实现在基础控件层（`AtomUI.Controls`），主题样式在平台层（`AtomUI.Desktop.Controls`）。
5. **声明式集成**：控件开发者只需在 AXAML 模板中放置 `WaveSpiritDecorator`，在 C# 中调用 `Play()` 即可触发波纹动画。

### 波纹类型

| 波纹类型 | 枚举值 | 典型控件 | 效果描述 |
|---|---|---|---|
| 圆角矩形波纹 | `RoundRectWave` | Button（默认形状）、CheckBox、OptionButton | 沿控件圆角矩形边缘向外扩散 |
| 圆形波纹 | `CircleWave` | Button（Circle 形状）、RadioButton | 以控件中心为圆心向外扩散 |
| 药丸形波纹 | `PillWave` | Button（Round 形状）、ToggleSwitch | 沿药丸形（胶囊形）边缘向外扩散 |

### 相关 Design Token

| Token 名称 | 类型 | 层级 | 默认值 | 说明 |
|---|---|---|---|---|
| `EnableWaveSpirit` | `bool` | Seed | `true` | 是否全局开启波浪动画 |
| `WaveAnimationRange` | `double` | Alias | `LineWidth × 6` | 波纹向外扩散的最大距离（像素） |
| `WaveStartOpacity` | `double` | Alias | `0.4` | 波纹初始透明度 |
| `MotionDurationSlow` | `TimeSpan` | Map | `300ms` | 波纹尺寸和透明度的动画时长 |
| `ColorPrimary` | `Color` | Seed | `#1677FF` | 波纹默认颜色（主色） |

### 与 Ant Design Wave 的关系

| 概念 | Ant Design (React) | AtomUI |
|---|---|---|
| 效果实现 | CSS `box-shadow` + `@keyframes` | `DrawingContext` 自定义绘制 + Avalonia `Animation` |
| 全局开关 | `ConfigProvider.wave.disabled` | `DesignToken.EnableWaveSpirit` / `ThemeManager.IsWaveSpiritEnabled` |
| 波纹颜色 | 从按钮的 `border-color` / `background` 取色 | 默认 `ColorPrimary`，可通过 `WaveBrush` 覆盖 |
| 支持的形状 | 自动检测 `border-radius` | 显式指定 `WaveSpiritType`（`RoundRectWave` / `CircleWave` / `PillWave`） |

