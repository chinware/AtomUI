# AtomUI.Controls.Shared 模块概览

`AtomUI.Controls.Shared` 是控件共享契约和跨控件基础能力模块。它依赖 `AtomUI.Core` 和 `AtomUI.Generator`，RootNamespace 为 `AtomUI.Controls`。

## 职责

- 提供控件通用接口，例如尺寸、状态、方向、操作系统感知、Header/Content 等共享契约。
- 提供异步加载协调器，统一搜索类和展开类控件的 debounce、超时、取消、去重行为。
- 提供集合视图能力，支撑排序、分组、过滤和虚拟化上下文。
- 提供上传调度、媒体断点、WaveSpirit 等跨控件系统。

## 关键目录

| 目录 | 说明 |
|---|---|
| `AsyncLoad/` | `AsyncSearchLoadCoordinator`、`AsyncExpandLoadCoordinator`、加载结果状态 |
| `Data/ListCollectionViews/` | 列表集合视图、排序、分组、过滤描述 |
| `Net/` | 文件上传调度与传输接口 |
| `MediaQuery/` | 媒体断点和响应式布局支持 |
| `WaveSpirit/` | 点击波纹/动效扩展 |
| `Converters/`、`Utils/` | 控件共享工具 |

## 相关专题

- [../../AsyncLoadingArchitecture.md](../../AsyncLoadingArchitecture.md)
- [../../FilteringArchitecture.md](../../FilteringArchitecture.md)

过滤专题中的部分实现位于 `AtomUI.Desktop.Controls`，但统一契约和共享数据结构属于本模块的重要背景。

