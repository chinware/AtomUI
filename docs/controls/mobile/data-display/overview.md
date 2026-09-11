# Mobile Data Display 能力清单

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

Data Display 收录结构化内容、媒体、状态数据和集合展示能力，共 11 项。涉及手势或虚拟化的能力仍消费统一 coordinator，
不在 Control 内复制平台事件算法。

| Capability | AtomUI mapping | API strategy | Wave | Design | Source | iOS | Android | Release | Publication | Evidence |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `Calendar` | Mobile `Calendar` | Tier 2 候选 | W4 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `Card` | Mobile `Card` | 波次设计未冻结 | W1 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `Collapse` | Mobile `Collapse` | 波次设计未冻结 | W2 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `Ellipsis` | Mobile `Ellipsis` | 波次设计未冻结 | W1 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `Image` | Mobile `Image` | 波次设计未冻结 | W1 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `ImageViewer` | Mobile `ImageViewer` | 波次设计未冻结 | W4 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `List` | Mobile `List` | Tier 2 候选 | W1 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `NoticeBar` | Mobile `NoticeBar` | 波次设计未冻结 | W1 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `ScrollMask` | Mobile `ScrollMask` | 波次设计未冻结 | W4 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `Swiper` | Mobile `Swiper` | 波次设计未冻结 | W4 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `WaterMark` | Mobile `WaterMark` | 波次设计未冻结 | W1 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |

状态提升和单 Control 目录创建遵循 [Mobile 文档规范](../../../engineering/contributing/mobile-documentation-guidelines.md)。
