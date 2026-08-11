# Mobile Layout 能力清单

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

Layout 收录空间、对齐、分区和安全区域能力，共 6 项。Safe Area 的数据源属于 Runtime，`SafeArea` Control 只消费统一
Viewport contract。

| Capability | AtomUI mapping | API strategy | Wave | Design | Source | iOS | Android | Release | Publication | Evidence |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `AutoCenter` | Mobile `AutoCenter` | 波次设计未冻结 | W1 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `Divider` | Mobile `Divider` | 波次设计未冻结 | W1 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `Footer` | Mobile `Footer` | 波次设计未冻结 | W1 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `Grid` | Mobile `Grid` | 波次设计未冻结 | W1 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `SafeArea` | Mobile `SafeArea` | Tier 3 | W1 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |
| `Space` | Mobile `Space` | 波次设计未冻结 | W1 | 能力已纳入总体设计 | 未实现 | 未验证 | 未验证 | 未验证 | 未发布 | [Mobile 系统架构](../../../architecture/systems/mobile/overview.md)；暂无源码或平台证据 |

状态提升和单 Control 目录创建遵循 [Mobile 文档规范](../../../engineering/contributing/mobile-documentation-guidelines.md)。
