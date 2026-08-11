# Android 排障框架

当前没有已验证 Android Host，因此本页不提供成功修复配方。排障按“症状证据 -> owner 边界 -> 下一项最小诊断”组织；真实
结论只有重复执行后才进入稳定指南。

## Toolchain Discovery

| 收集证据 | Owner 判断 |
| --- | --- |
| 实际 `dotnet`、workload、JDK、Android SDK 解析来源，license 和缺失组件 | 环境/CI provision，而非 Mobile Control |
| Host target framework、ABI/RID 和 restore/build diagnostics | 集中构建配置或 Host 项目 |

不要同时升级 JDK、SDK、workload 和 target framework。先定位第一个解析/兼容边界，再只改变一个变量验证。

## Signing

| 收集证据 | Owner 判断 |
| --- | --- |
| Application ID、Debug/Release identity、keystore/alias metadata、artifact signature 和 secret 注入日志 | Host/release pipeline |
| 是否为 debug identity、是否使用错误 alias、CI material 是否解锁/清理 | Signing provision |

日志必须脱敏，不输出 keystore/private key/password。没有 Release artifact 安装启动证据时不能把 signing 标记为通过。

## Deployment 与 Launch

| 收集证据 | Owner 判断 |
| --- | --- |
| Device/AVD identity、artifact digest、安装结果、Application ID、process/Activity 日志 | Host/deployment toolchain |
| 安装失败、启动后退出、旧包覆盖或错误设备 | 分别定位 package、Host startup、cache/device selection |

先证明安装和启动是哪一步失败，再调查 Control 或 adapter；不要把所有 deployment 错误归因于 UI。

## Activity Lifecycle

收集 Activity create/destroy、foreground/background、process restart、Runtime scope acquire/release、Overlay/navigation restore 和
残留引用证据。Host/adapter 拥有生命周期桥接，Mobile Runtime 拥有稳定状态恢复，Control 只消费已发布状态。

## Resource Packaging

收集 manifest、resources、Theme、Localization、DataTemplate、icon/splash 和裁剪前后 artifact 内容。若 Debug 正常而 Release
缺失，优先定位 linker/descriptor/Build Action；若两个配置都缺失，定位 Host 项目资源输入。

## System Back

记录系统 back 来源、gesture/三键模式、当前 Overlay z-order、页面栈、消费 owner 和终态通知次数。Adapter 只投递输入，
Overlay/Navigation 决定唯一 owner；重复 pop/close 属于事务或注册问题。

## IME

记录输入法、组合文本、selection、焦点、input pane bounds、viewport snapshot、方向和 Control 状态。Android adapter 拥有平台
输入/occlusion 投射，Mobile Runtime 合并 viewport，输入 Control 拥有文本与 validation 语义。

## Adapter State

收集 adapter 注册数量、Host/TopLevel identity、subscription acquire/release、system bars/input pane/back/lifecycle snapshot 和
capability-missing fallback。缺失/重复 adapter 属于 startup/acquire 配置；能力缺失按契约退化，不能由 Control 猜测 OS 默认值。

## 结论门禁

每个排障结论必须包含环境、Host commit、构建配置、Device/AVD、复现步骤、关键日志、单一变量实验和结果。当前所有 Android
remediation 均为未验证；本页只定义证据收集框架。
