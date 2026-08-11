# Mobile Navigation 与 Overlay

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

Mobile Navigation 建立在 Avalonia Page 体系上；Overlay 使用每个 `TopLevel` 唯一的 session manager。两者共享系统返回、
焦点、viewport、取消和 host 生命周期边界，但不创建第二套路由框架。

## Navigation 契约

- 页面栈继续使用 Avalonia `Page`、`NavigationPage`、`TabbedPage` 和 `DrawerPage`。
- `NavBar`、`TabBar`、`Tabs`、`SideBar` 和移动页面 shell 只投射导航状态与交互。
- iOS edge swipe、Android system back 和应用命令进入同一导航事务。
- Overlay 总是先于页面栈处理返回。
- 导航事务只有未提交和已提交两个稳定结果。
- 动画取消不能留下半入栈页面、重复 pop 或失配焦点。

页面离开、host detach 和应用恢复必须具有确定的焦点、订阅和状态清理语义。平台 adapter 只产生系统输入，不直接修改
页面栈。

## 返回优先级

```text
活动的 modal Overlay
  -> 可关闭的非 modal Overlay
  -> 当前页面的受控返回策略
  -> Avalonia 页面栈 pop
  -> Host 默认行为
```

同一返回输入只能由一个 owner 提交。被更高优先级层消费后，后续层不得再次处理。

## Overlay Session

`MobileOverlayManager` 为每个 `TopLevel` 管理统一 session。Mask、Popup、CenterPopup、Dialog、Modal、Toast、ActionSheet、
Popover、ImageViewer、Picker 等共享以下状态机：

```text
Created -> Opening -> Open -> ClosePending -> Closing -> Closed
```

| 状态 | 允许行为 |
| --- | --- |
| Created | 绑定 owner、内容、结果类型和取消作用域，尚未挂载视觉树 |
| Opening | 挂载、测量、获取焦点并执行打开动效 |
| Open | 接收交互、返回、viewport 和显式 Close |
| ClosePending | 合并竞争关闭请求并冻结唯一终态 |
| Closing | 执行关闭动效、释放焦点和视觉资源 |
| Closed | 发布一次结果，释放所有 owner/session 引用 |

## 声明式与静态 API

- 声明式 `IsOpen` 和静态 `ShowAsync` 创建同一种 session。
- 静态 API 返回强类型结果和关闭原因，并接受 `CancellationToken`。
- `CancellationToken` 取消在 session 完成清理后结束 Task，不能绕过 Closing/Closed。
- 内容可以使用独立 `DataTemplate`，但不能直接越过 manager 挂到 VisualRoot。
- 不使用进程级可变 Overlay host singleton。

这些 API 名称属于目标契约；具体签名由 Foundation 和对应控件族设计固定。

## 关闭与竞争

Close、遮罩点击、系统返回、手势、超时、调用方取消和 host detach 可能竞争。session 必须：

1. 原子选择第一个有效终态请求。
2. 把后续请求折叠为 no-op。
3. 只触发一次 closing/closed 通知和一次 Task 完成。
4. 区分用户选择、主动关闭、外部取消和 `HostDetached`。
5. 在业务错误时保留内容和上下文，进入可重试状态而不是自动关闭。

打开前取消时不挂载视觉树。Opening/Open 期间取消时经过 `ClosePending -> Closing -> Closed` 完成释放。

## z-order、Mask 与 Focus

- manager 是同一 TopLevel 内 z-order 的唯一 owner。
- Mask 与内容属于同一 session 生命周期，不能独立遗留。
- Modal session 建立焦点圈闭；关闭后按可验证规则恢复到发起元素或稳定页面焦点。
- Toast 等非交互 Overlay 不夺取不必要的键盘焦点。
- viewport 变化更新 session 布局，不创建第二个 Overlay。
- 多个 session 的返回顺序与可见 z-order 一致。

## Host 生命周期

- 进入后台时暂停 transient timeout 和非必要动效。
- 恢复时在 viewport snapshot 刷新后恢复可见 session。
- Host detach 以 `HostDetached` 关闭所有 session。
- Host recreation 不能重复完成 Task、重复 opened 事件或重复 push 页面。

## 维护不变量

1. 每个 `TopLevel` 只有一个 `MobileOverlayManager`。
2. Navigation 不替代 Avalonia Page 体系。
3. Overlay 先于页面栈消费系统返回。
4. 声明式和静态 API 共享 session 状态机。
5. 一个 session 只有一个终态结果和一次释放。
6. Control 不直接把 Overlay 内容挂到 VisualRoot。

Viewport owner 见 [Runtime 与平台能力](runtime-and-platform-capabilities.md)，取消和平台验证见
[Mobile 验证架构](verification.md)。
