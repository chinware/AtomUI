# Mobile 手势协调

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

Mobile 手势系统通过 `MobileGestureCoordinator` 仲裁同一活动 pointer 链。Control 提供可测试的 gesture policy 和状态模型；
Coordinator 决定 ownership、事件流和 handoff，避免 Swiper、SwipeAction、FloatingPanel、PullToRefresh 和 ImageViewer 各自
复制方向锁定与速度算法。

## 输入模型

一个 pointer chain 至少包含：

- Pointer identity 与参与的手势候选者。
- 起点、当前位置、累计位移和时间基准。
- 方向锁定状态。
- 当前 owner、可 handoff parent 和可取消 child。
- 速度 sample、边界、snap point 和 Reduce Motion profile。
- capture、animation owner 和终态 cleanup。

坐标统一使用 TopLevel 可比较坐标；Control policy 可以把结果映射到自身内容坐标，但不能让不同 owner 使用未声明的坐标系。

## 仲裁流程

1. Pointer pressed 建立候选链，不立即提交产品动作。
2. 位移超过 slop 后，根据主要轴、候选 policy 和可滚动边界解析方向。
3. Coordinator 选择 owner 并建立 capture。
4. Owner 消费位移；到达边界时按 policy 决定 rubberband、停止或向 parent handoff。
5. Released 使用位移、速度和 commit threshold 选择目标 snap point 或稳定回滚点。
6. Canceled、capture lost 或 host detach 回到最近稳定状态，并释放所有临时资源。

方向未解析前，不触发选择、删除、刷新、导航完成或页面提交。

## Nested Scroll Handoff

handoff 同时考虑：

- Child 是否仍能沿目标方向滚动。
- Parent 是否声明接受该方向的剩余 delta。
- 当前边界是否允许 overscroll/rubberband。
- 方向锁定是否已经稳定，避免水平/垂直 owner 反复切换。
- 多指加入后原 owner 是否仍能保持语义。

ownership 转移必须是单向、可诊断的事务。旧 owner 先收到取消并释放 capture，再由新 owner 接管剩余输入；不能让两个
Control 同时提交同一 delta。

## 速度、Rubberband 与 Snap

- 速度使用统一时间基准和有限 sample window。
- 无效、过旧或跨取消边界的 sample 不参与终态计算。
- Rubberband 只改变视觉位移，不改变稳定业务值。
- Snap point 由 Control policy 提供，Coordinator 负责选择和动画调度。
- 位移与速度阈值冲突时使用 policy 明确定义的优先级。
- 公式集中在共享职责中，平台 adapter、Template 和派生 Control 不复制算法。

具体阈值和 spring 参数只有在实现与性能基线存在后进入 Control 或 Runtime 文档，不在预实现架构中虚构数值。

## 取消与降级

系统取消、parent 夺取、capture lost、窗口失活、多指冲突和 host detach 均不得提交产品动作。取消后：

- 回到最近稳定 snap point 或业务值。
- 释放 pointer capture、animation owner、timer 和 subscription。
- 清空速度 sample 和临时视觉 offset。
- 不触发选择、删除、刷新、导航或提交完成事件。

Reduce Motion 开启时，保持相同终态和事件顺序，缩短或移除非必要位移动画；不能跳过 cleanup 或直接写最终业务状态。

## 性能与生命周期

- Drag、scroll、pinch 和 animation sampling 热路径不产生逐帧托管分配。
- Coordinator 不通过 static collection 持有 Control 或 TopLevel。
- Pointer chain 在 release、cancel 和 detach 后可被回收。
- Policy 使用纯值和显式接口，不依赖反射或运行时程序集扫描。
- 动画使用现有 MotionScene/Avalonia composition；第三方算法只能替换内部策略，不能进入公共 API。

## 维护不变量

1. 同一 pointer delta 只有一个 owner。
2. 方向锁定和 nested-scroll handoff 由 Coordinator 统一实现。
3. Rubberband 视觉值不成为业务稳定值。
4. 取消不提交产品动作。
5. Control policy 可纯逻辑测试，平台输入只负责投递。
6. 热路径无逐帧托管分配。

完整性能和平台验证见 [Mobile 验证架构](verification.md)。
