# PopupConfirm Performance Notes

## Scope

`PopupConfirm` 本轮只处理 `PopupConfirmContainer` 按钮事件生命周期和隐藏 cancel 的正确性，不把 `PART_Content` / `PART_CancelButton` 从静态模板迁到 C# 动态创建。

## Root Cause

- `PopupConfirmContainer.OnApplyTemplate()` 原先给 `PART_OkButton` / `PART_CancelButton` 注册本地 `Click` handler。re-template 场景下这类订阅容易重复，且每个 container 都承担两条本地 routed-event handler。
- `IsShowCancelButton=false` 时模板里的 cancel button 通过 `IsVisible=false` 隐藏。旧验证错误地要求移除该模板节点，和当前静态模板规则冲突。
- 隐藏 cancel button 如果被测试或内部代码直接 raise `Button.ClickEvent`，旧逻辑仍会当成 cancel 处理，触发 `Cancelled` / `PopupClick`。

## Changes

- `PopupConfirmContainer` 改为 `Button.ClickEvent.AddClassHandler<PopupConfirmContainer>()`，由 container 统一识别模板按钮来源。
- 移除 `OnApplyTemplate()` 中的 `_okButton.Click += ...` / `_cancelButton.Click += ...`。
- cancel button 在 `IsShowCancelButton=false` 时即使收到直接 `ClickEvent` 也会被忽略。
- `--verify-popupconfirm-states` 改为验证静态模板 slot：无内容/无取消时 slot 保留但不可见；同时验证按钮 part 没有本地 `Click` handler。

## Benefit

这轮收益是结构和正确性收益：每个打开态 `PopupConfirmContainer` 少两条本地 Click 订阅，并修掉隐藏 cancel 还能触发取消的错误；页面加载 timing 没有作为本轮收益证明。

| Metric | Baseline | Optimized | Formula | Improvement | Conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Local Click handlers / PopupConfirmContainer | 2 | 0 | `(2 - 0) / 2` | 100.00% | 有效；OK/Cancel 改由 container class handler 统一处理 |
| Hidden cancel direct-click side effects / click | 2 events | 0 events | `(2 - 0) / 2` | 100.00% | 正确性修复；不再触发 `Cancelled` + `PopupClick` |
| `PopupConfirm.Container.Basic` KB/item | 968.3 KB | 967.1 KB | `(968.3 - 967.1) / 968.3` | 0.12% | smoke-only；符合少 handler 的结构变化，不作为页面速度证明 |
| `PopupConfirm.Container.TitleOnly` KB/item | 944.6 KB | 943.5 KB | `(944.6 - 943.5) / 944.6` | 0.12% | smoke-only；分配小幅下降 |
| `PopupConfirm.Container.NoCancel` KB/item | 696.9 KB | 696.0 KB | `(696.9 - 696.0) / 696.9` | 0.13% | smoke-only；分配小幅下降 |
| `PopupConfirm.GalleryShape` ms/item | 19.139 ms | 21.769 ms | `(19.139 - 21.769) / 19.139` | -13.74% | smoke-only；单次 timing 波动，不作为收益证明 |

## Verification

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-popupconfirm-states --verify-flyout-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite popupconfirm --count 80
```

Notes:

- `PART_Content` 和 `PART_CancelButton` 是静态模板节点。隐藏时通过 `IsVisible=false` 退出布局/渲染，不再把“节点不存在”当成正确性目标。
- 本轮没有证明 `PopupConfirmShowCase` 页面加载速度提升；收益以事件订阅移除、隐藏 cancel 惰性和状态验证通过为准。
