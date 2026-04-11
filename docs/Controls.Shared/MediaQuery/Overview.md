# AtomUI 媒体查询（Media Query）系统

> 本文档描述 AtomUI 响应式布局的核心架构 —— 媒体查询系统（Media Query System）的设计原理、实现机制及开发者使用指南。

---

## 文档索引

| 文档 | 内容 |
|---|---|
| [架构设计](./Architecture.md) | 系统总体架构、断点检测机制、数据流与核心类型说明 |
| [开发者指南](./DeveloperGuide.md) | 面向控件开发者的使用指南，包含响应式 Grid、Form、Descriptions 等完整示例 |

---

## 概述

AtomUI 的媒体查询系统是一套 **基于容器宽度的响应式断点机制**，完整复现了 [Ant Design 5.0 栅格系统](https://ant.design/components/grid-cn) 的六级响应式断点策略。它构建在 Avalonia 的 [Container Query](https://docs.avaloniaui.net/docs/styling/container-queries) 机制之上，为 AtomUI 控件提供统一的窗口级响应式能力。

### 设计目标

1. **对齐 Ant Design 5.0**：完整实现 `xs / sm / md / lg / xl / xxl` 六级断点体系，断点阈值与 Ant Design 完全一致。
2. **基于容器查询**：利用 Avalonia 原生的 `ContainerQuery` + `Container.Sizing` 机制实现宽度检测，而非自行监听窗口尺寸变化。
3. **统一事件分发**：通过 `Window` 集中管理断点状态，下游控件通过 `IMediaBreakAwareControl` 接口订阅断点变更事件。
4. **声明式 AXAML 友好**：所有响应式数据类型（`GridColSpanInfo`、`GridGutterInfo`、`MediaBreakGridLength`、`DescriptionsMediaBreakInfo`）均提供 `TypeConverter`，支持在 AXAML 中以字符串格式直接配置。

### 断点阈值

| 断点名称 | 缩写 | 最小宽度 (px) | 典型设备 |
|---|---|---|---|
| ExtraSmall | `xs` | 0 | 手机竖屏 |
| Small | `sm` | 576 | 手机横屏 |
| Medium | `md` | 768 | 平板竖屏 |
| Large | `lg` | 992 | 平板横屏 / 小桌面 |
| ExtraLarge | `xl` | 1200 | 标准桌面 |
| ExtraExtraLarge | `xxl` | 1600 | 大屏桌面 |

这些阈值定义在 `MediaBreakPoint` 枚举中，枚举的整数值即为该断点的最小像素宽度。

### 与 CSS 媒体查询的关系

| 概念 | CSS | AtomUI |
|---|---|---|
| 检测目标 | 视口（viewport） | Window 内容区域（ContentFrame） |
| 检测机制 | `@media (min-width: ...)` | Avalonia `ContainerQuery` + `Container.Sizing` |
| 断点传播 | CSS 变量 / JS 事件 | `IMediaBreakAwareControl.MediaBreakPointChanged` 事件 |
| 响应式属性 | CSS `grid-template-columns` 等 | `GridColSpanInfo`、`GridGutterInfo`、`MediaBreakGridLength` 等 C# record 类型 |

